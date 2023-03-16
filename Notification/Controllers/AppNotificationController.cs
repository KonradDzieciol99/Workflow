using MediatR;
using MessageBus.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using Notification.Models;
using System.Security.Claims;
using System.Text.Json;

namespace Notification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppNotificationController : ControllerBase
    {
        private readonly IMongoDatabase _mongoDatabase;

        public AppNotificationController(IMongoDatabase mongoDatabase)
        {
            this._mongoDatabase = mongoDatabase;
        }
        [HttpGet]
        public async Task<ActionResult<List<AppNotification>>> Get()//[FromQuery] string UserId
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
                return BadRequest("User cannot be identified");

            var collection = _mongoDatabase.GetCollection<AppNotificationMongo>("Notifications");

            var filter = Builders<AppNotificationMongo>.Filter.Eq(x => x.UserId, userId);
            //var projection = Builders<AppNotificationMongo>.Projection.Expression(
            //        x => new AppNotification
            //        {
            //            Id = x.Id,
            //            UserId = x.UserId,
            //            //ObjectId = x.ObjectId,
            //            ObjectId = BsonTypeMapper.MapToDotNetValue(x.ObjectId),
            //            EventType = x.EventType,
            //            NotificationPartner = x.NotificationPartner,
            //            NotificationType = x.NotificationType,
            //            CreationDate = x.CreationDate,
            //            Displayed = x.Displayed,
            //            Description = x.Description
            //        });

            var result = await collection.Find(filter).ToListAsync();

            //var transformedResult = result.Select(x => new AppNotification
            //{
            //    Id = x.Id,
            //    UserId = x.UserId,
            //    ObjectId = x.ObjectId.IsNullOrEmpty() ? null : JsonSerializer.Deserialize<JsonElement>(x.ObjectId),
            //    EventType = x.EventType,
            //    NotificationType = x.NotificationType,
            //    Data = null/*JsonSerializer.Deserialize<JsonElement>(x.Data)*/,
            //    Description = x.Description,
            //    CreationDate = x.CreationDate,
            //    NotificationPartner = x.NotificationPartner
            //}).ToList();
            var notifications = result.Select(x => new AppNotification
            {
                Id = x.Id,
                UserId = x.UserId,
                ObjectId = x.ObjectId != null ? BsonTypeMapper.MapToDotNetValue(x.ObjectId) : null,
                EventType = x.EventType,
                NotificationPartner = x.NotificationPartner,
                NotificationType = x.NotificationType,
                CreationDate = x.CreationDate,
                Displayed = x.Displayed,
                Description = x.Description
            }).ToList();

            return Ok(notifications);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
                return BadRequest("User cannot be identified");
            var collection = _mongoDatabase.GetCollection<AppNotification>("Notifications");


            var filter = Builders<AppNotification>.Filter.Eq(n => n.Id, id);
            var update = Builders<AppNotification>.Update.Set(n => n.Displayed, true);
            var result = await collection.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 1)
            {
                return Ok();
            }
            return BadRequest("failed to update");
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
                return BadRequest("User cannot be identified");
            var collection = _mongoDatabase.GetCollection<AppNotification>("Notifications");

            var filter = Builders<AppNotification>.Filter.Eq(n => n.Id, id);

            var result=await collection.DeleteOneAsync(filter);

            if (result.DeletedCount == 1)
            {
                return Ok();
            }
            return BadRequest("failed to delete");
        }
    }
}
