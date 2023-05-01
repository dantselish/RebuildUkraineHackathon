using Invincible.MongoDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Invincible.Api.Schemas;
using Invincible.Event;
using Invincible.Extensions;
using Invincible.Items;
using Invincible.Responces;
using Invincible.User;
using MongoDB.Bson;
using RebuildUkraineHackatonWebAPI.Api.Controllers;
using RebuildUkraineHackatonWebAPI.MongoDB;

namespace Invincible.Api.Controllers;

[ApiController]
[Authorize]
[Route( "api/events" )]
public class ApiEventsController : MyController
{
  public ApiEventsController( IMongoDBController mongo_db_controller ) : base(mongo_db_controller){}

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

  [HttpGet("my")]
  public async Task<ActionResult<GetEventsResponse>> getMyEvents()
  {
    if (!tryGetGoogleId(out string google_id))
      return NotFound();

    OrganizerItem? organizer = await _db_controller.getOrganizer(google_id);
    var event_items = await _db_controller.getEventsByOrganizer(organizer._id);
    List<EventBasicInfo> events = new List<EventBasicInfo>();
    foreach (EventItem event_item in event_items)
    {
      if (event_item.event_status != EventStatus.ARCHIVED)
        events.Add( new EventBasicInfo(event_item) );
    }
    return new GetEventsResponse
    {
       events = events
    };
  }

  [HttpGet("my/archive")]
  public async Task<ActionResult<GetEventsResponse>> getMyArchivedEvents()
  {
    if (!tryGetGoogleId(out string google_id))
      return NotFound();

    OrganizerItem? organizer = await _db_controller.getOrganizer(google_id);
    var event_items = await _db_controller.getEventsByOrganizer(organizer._id);
    List<EventBasicInfo> events = new List<EventBasicInfo>();
    foreach (EventItem event_item in event_items)
    {
      if (event_item.event_status == EventStatus.ARCHIVED)
        events.Add( new EventBasicInfo(event_item) );
    }
    return new GetEventsResponse
    {
       events = events
    };
  }

  [HttpPost("create")]
  [ProducesResponseType( 200 )]
  [ProducesResponseType( 400 )]
  [ProducesResponseType( 401 )]
  public async Task<IActionResult> createEvent(PostEventRequestBody request_body)
  {
    if (!tryGetGoogleId(out string google_id))
      return BadRequest();

    var organizer = await _db_controller.getOrganizer(google_id);
    if (organizer == null)
      return Unauthorized();

    EventCreationData creation_data = new EventCreationData()
    {
      organizer = organizer._id,
      name = request_body.name,
      description = request_body.description,
      location = request_body.location,
      timestamp = request_body.timestamp,
      cityCode = request_body.cityCode,
      eventLenght = request_body.eventLenght,
      volunteersRequired = request_body.volunteersRequired,
      eventType = request_body.eventType
    };
    await _db_controller.createEvent(creation_data);
    return Ok();
  }

  [HttpGet("{id}/register")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(401)]
  [ProducesResponseType(404)]
  [ProducesResponseType(409)]
  public async Task<IActionResult> registerVolunteer( string id )
  {
    if (!tryGetGoogleId(out string google_id))
      return BadRequest();

    var volunteer = await _db_controller.getVolunteer(google_id);
    if (volunteer == null)
      return Unauthorized();

    var event_item = await _db_controller.getEventItemById(id);
    if (event_item == null)
      return NotFound();

    var result = await _db_controller.registerVolunteer(event_item, volunteer);
    return result switch
           {
             RegisterVolunteerStatus.OK                   => Ok(),
             RegisterVolunteerStatus.MAX_VOLUNTEERS_COUNT => Conflict("Max volunteers count"),
             RegisterVolunteerStatus.ALREADY_REGISTERED   => Conflict("Already registered"),
             RegisterVolunteerStatus.WRONG_EVENT_STATUS   => Conflict($"Event status must be {nameof(EventStatus.ACTIVE)}")
             , _                                          => throw new ArgumentOutOfRangeException()
           };
  }

  [HttpGet("{id}/start")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(401)]
  [ProducesResponseType(403)]
  [ProducesResponseType(404)]
  [ProducesResponseType(409)]
  public async Task<ActionResult<StartEventResponse>> startEvent(string id)
  {
    if (!tryGetGoogleId(out string google_id))
      return BadRequest();

    OrganizerItem? organizer = await _db_controller.getOrganizer(google_id);
    if (organizer == null)
      return Unauthorized();

    EventItem? event_item = await _db_controller.getEventItemById(id);
    if (event_item == null)
      return NotFound();

    if (!event_item.organizer.Equals(organizer._id))
      return Forbid();

    if (event_item.event_status != EventStatus.ACTIVE)
      return Conflict($"To start event it's status must be {nameof(EventStatus.ACTIVE.ToString)}");

    byte[] event_secret = await EventConfirmSecretGenerator.getSecret(event_item._id);
    event_item.confirm_secret = event_secret;
    await _db_controller.startEvent(event_item);
    return new StartEventResponse(){ qrCodeContent = event_secret };
  }

  [HttpGet("{id}/finish")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(401)]
  [ProducesResponseType(403)]
  [ProducesResponseType(404)]
  [ProducesResponseType(409)]
  public async Task<ActionResult<FinishEventResponse>> finishEvent(string id)
  {
    if (!tryGetGoogleId(out string google_id))
      return BadRequest();

    OrganizerItem? organizer = await _db_controller.getOrganizer(google_id);
    if (organizer == null)
      return Unauthorized();

    EventItem? event_item = await _db_controller.getEventItemById(id);
    if (event_item == null)
      return NotFound();

    if (!event_item.organizer.Equals(organizer._id))
      return Forbid();

    if (event_item.event_status != EventStatus.IN_PROGRESS)
      return Conflict($"To finish event it's status must be {nameof(EventStatus.IN_PROGRESS.ToString)}");

    await _db_controller.finishEvent(event_item);
    var volunteer_infos = new List<BasicVolunteerInfo>();
    foreach (ObjectId volunteer_id in event_item.confirmed_volunteers)
    {
      VolunteerItem? volunteer_item = await _db_controller.getVolunteer(volunteer_id);
      if (volunteer_item == null)
        continue;

      volunteer_infos.Add( new BasicVolunteerInfo(){ objectId = volunteer_id, name = volunteer_item.name } );
    }
    return new FinishEventResponse{ volunteers = volunteer_infos };
  }

  [HttpPost("confirm")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(401)]
  [ProducesResponseType(403)]
  [ProducesResponseType(404)]
  [ProducesResponseType(409)]
  public async Task<IActionResult> confirmVolunteerParticipation(ConfirmVolunteerParticipationRequestBody request_body)
  {
    if (!tryGetGoogleId(out string google_id))
      return BadRequest();

    VolunteerItem? volunteer = await _db_controller.getVolunteer(google_id);
    if (volunteer == null)
      return Unauthorized();

    var event_id = await EventConfirmSecretGenerator.readSecret(request_body.qrCodeContent);
    var event_item = await _db_controller.getEventItemById(event_id);
    if (event_item == null)
      return NotFound();

    if (!event_item.registered_volunteers.Contains(volunteer._id))
      return Forbid();

    if (event_item.confirmed_volunteers.Contains(volunteer._id))
      return Conflict("Already confirmed");

    await _db_controller.confirmRegistration(event_item, volunteer._id);
    return Ok();
  }
}