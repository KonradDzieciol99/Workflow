using MessageBus.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
            var result = await collection.Find(filter).ToListAsync();

            var transformedResult = result.Select(x => new AppNotification
            {
                Id = x.Id,
                UserId = x.UserId,
                ObjectId = x.ObjectId.IsNullOrEmpty() ? null : JsonSerializer.Deserialize<JsonElement>(x.ObjectId),
                EventType = x.EventType,
                NotificationType = x.NotificationType,
                Data = null/*JsonSerializer.Deserialize<JsonElement>(x.Data)*/,
                Description = x.Description,
                CreationDate = x.CreationDate,
                NotificationPartner = x.NotificationPartner
            }).ToList();

            return Ok(transformedResult);
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

            var result=collection.DeleteOne(filter);

            if (result.DeletedCount == 1)
            {
                return Ok();
            }
            return BadRequest("failed to delete");
        }
    }
}
