using MessageBus.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Notification.Models;
using System.Security.Claims;

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

            var collection = _mongoDatabase.GetCollection<AppNotification>("Notifications");

            var filter = Builders<AppNotification>.Filter.Eq(x => x.UserId, userId);
            var result = await collection.Find(filter).ToListAsync();

            return Ok(result);
        }
    }
}
