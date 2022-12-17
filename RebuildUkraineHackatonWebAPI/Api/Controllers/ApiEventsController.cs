using Microsoft.AspNetCore.Mvc;
using RebuildUkraineHackathonWebAPI.Api.RequestsBodies;
using RebuildUkraineHackathonWebAPI.Api.Schemas;
using RebuildUkraineHackathonWebAPI.Items;
using RebuildUkraineHackathonWebAPI.Responces;
using RebuildUkraineHackatonWebAPI.MongoDB;

namespace RebuildUkraineHackathonWebAPI.Controllers;


[ServiceFilter( typeof( BasicAuthServiceFilter ) )]
[Route( "api/events" )]
public class ApiEventsController : ControllerBase
{
  private readonly IMongoDBController _db_controller;


  public ApiEventsController( IMongoDBController mongo_db_controller )
  {
    _db_controller = mongo_db_controller;
  }

  [HttpGet]
  public async Task<GetEventsResponse> getAllEvents()
  {
    List<EventItem> event_items = await _db_controller.getAllEvents();
    return new GetEventsResponse(){ events = event_items.Select( x => (EventBasicInfo) x ).ToList() };
  }

  [HttpGet( "{id}" )]
  public async Task<GetEventByIdResponse> getEvent( string id )
  {
    EventInfo event_info = await _db_controller.getEventItemById( id );
    return new GetEventByIdResponse(){ EventInfo = event_info };
  }

  [HttpPost( "{id}/register" )]
  public async Task<EventRegisterVolunteerResponse> registerVolunteer( string id, EventRegisterVolunteerBody request_body )
  {
    return new EventRegisterVolunteerResponse(){ NewCurVolunteersCount = await _db_controller.registerVolunteer( id ) };
  }
}