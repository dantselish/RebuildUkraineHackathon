// using System.Security.Claims;
// using Invincible.MongoDB;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Invincible.Api.RequestsBodies;
// using Invincible.Api.Schemas;
// using Invincible.Items;
// using Invincible.Responces;
//
// namespace Invincible.Api.Controllers;
//
//
// [Authorize]
// [Route( "api/events" )]
// public class ApiEventsController : ControllerBase
// {
//   private readonly IMongoDBController _db_controller;
//
//
//   public ApiEventsController( IMongoDBController mongo_db_controller )
//   {
//     _db_controller = mongo_db_controller;
//   }
//
//   [AllowAnonymous]
//   [HttpGet("test")]
//   public async Task<string> getTest()
//   {
//     var identity = HttpContext.User.Identity as ClaimsIdentity;
//     string res = identity.Claims.Aggregate(string.Empty, (current, claim) => current + $"{claim} ");
//     return res;
//   }
//
//   [HttpGet]
//   public async Task<GetEventsResponse> getAllEvents()
//   {
//     List<EventItem> event_items = await _db_controller.getAllEvents();
//     return new GetEventsResponse(){ events = event_items.Select( x => (EventBasicInfo) x ).ToList() };
//   }
//   
//   [HttpGet( "{id}" )]  
//   public async Task<GetEventByIdResponse> getEvent( string id )  
//   {
//     EventInfo event_info = await _db_controller.getEventItemById( id );
//     return new GetEventByIdResponse(){ EventInfo = event_info };
//   }
//
//   [HttpPost( "{id}/register" )]
//   public async Task<EventRegisterVolunteerResponse> registerVolunteer( string id, EventRegisterVolunteerBody request_body )
//   {
//     return new EventRegisterVolunteerResponse(){ NewCurVolunteersCount = await _db_controller.registerVolunteer( id ) };
//   }
// }