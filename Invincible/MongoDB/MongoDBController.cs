using Invincible.Api.RequestsBodies.Organizer;
using Invincible.Api.RequestsBodies.Volunteer;
using Invincible.Items;
using MongoDB.Bson;
using MongoDB.Driver;
using Invincible.MongoDB;
using RebuildUkraineHackatonWebAPI.MongoDB;
using RebuildUkraineHackatonWebAPI.MongoDB.StatusCodes;

namespace Invincible;


public class MongoDBController : IMongoDBController
{
  private const string EVENTS_COLLECTION_NAME     = "Events";
  private const string ORGANIZERS_COLLECTION_NAME = "Organziers";
  private const string VOLUNTEERS_COLLECTION_NAME = "Volunteers";
  private const string DATABASE_NAME          = "RebuildUkraineHackathon";

  private readonly IConfiguration _configuration;

  public string connection_string;

  private MongoClient    _mongo_client;
  private IMongoDatabase _database;

  private IMongoCollection<VolunteerItem> _volunteers_collection;
  private IMongoCollection<OrganizerItem> _organizers_collection;
  private IMongoCollection<EventItem>  _events_collection;


  public MongoDBController( IConfiguration configuration )
  {
    _configuration = configuration;
    connection_string = _configuration["MongoDBConnectionString"];

    _mongo_client = new MongoClient( connection_string );
    _database = _mongo_client.GetDatabase( DATABASE_NAME );
    _volunteers_collection = _database.GetCollection<VolunteerItem>( VOLUNTEERS_COLLECTION_NAME );
    _organizers_collection = _database.GetCollection<OrganizerItem>( ORGANIZERS_COLLECTION_NAME );
    _events_collection = _database.GetCollection<EventItem>( EVENTS_COLLECTION_NAME );
  }

  #region Volunteer
  public async Task<VolunteerItem> getVolunteer(string google_id)
  {
    IAsyncCursor<VolunteerItem>? result = await _volunteers_collection.FindAsync( x => x.google_id.Equals( google_id ) );
    return await result.FirstOrDefaultAsync();
  }

  public Task createVolunteer(VolunteerCreationData volunteer_creation_data)
  {
    return _volunteers_collection.InsertOneAsync( new VolunteerItem(volunteer_creation_data) );
  }
  public Task createVolunteer() => createVolunteer(new VolunteerCreationData());

  public async Task<UpdateStatusCode> updateVolunteerInfo(string google_id, PutVolunteerRequestBody info)
  {
    var filter = Builders<VolunteerItem>.Filter.Eq( volunteer => volunteer.google_id, google_id );
    var update = Builders<VolunteerItem>.Update.Set(volunteer => volunteer.name, info.name ).Set( volunteer => volunteer.city_code, info.cityCode );
    var result = await _volunteers_collection.UpdateOneAsync(filter, update);
    if (!result.IsAcknowledged || result.MatchedCount < 1)
      return UpdateStatusCode.NOT_FOUND;

    return UpdateStatusCode.OK;
  }

  private Task updateVolunteer(VolunteerItem volunteer_item)
  {
    return _volunteers_collection.ReplaceOneAsync(x => x._id == volunteer_item._id, volunteer_item);
  }
  #endregion

  #region Organizer
  public async Task<OrganizerItem?> getOrganizer(string google_id)
  {
    IAsyncCursor<OrganizerItem>? result = await _organizers_collection.FindAsync( x => x.google_id.Equals( google_id ) );
    return await result.FirstOrDefaultAsync();
  }

  public Task createOrganizer(OrganizerCreationData organizer_creation_data)
  {
    return _organizers_collection.InsertOneAsync( new OrganizerItem(organizer_creation_data) );
  }

  public Task createOrganizer() => createOrganizer(new OrganizerCreationData());

  public async Task<UpdateStatusCode> updateOrganizerInfo(string google_id, PutOrganizerRequestBody info)
  {
    var filter = Builders<OrganizerItem>.Filter.Eq( organizer => organizer.google_id, google_id );
    var update = Builders<OrganizerItem>.Update
      .Set( organizer => organizer.name, info.name )
      .Set( organizer => organizer.description, info.description )
      .Set( organizer => organizer.phone_number, info.phone_number );
    var result = await _organizers_collection.UpdateOneAsync(filter, update);
    if (!result.IsAcknowledged || result.MatchedCount < 1)
      return UpdateStatusCode.NOT_FOUND;

    return UpdateStatusCode.OK;
  }
  #endregion

  #region Events
  public Task createEvent(EventCreationData creation_data)
  {
    return _events_collection.InsertOneAsync( new EventItem(creation_data) );
  }

  public async Task<List<EventItem>> getAllEvents()
  {
    IAsyncCursor<EventItem>? events = await _events_collection.FindAsync( _ => true );
    return await events.ToListAsync();
  }

  public async Task<EventItem?> getEventItemById( string id )
  {
    if ( !ObjectId.TryParse( id, out ObjectId object_id ) )
      return null;

    return await getEventItemById( object_id );
  }

  public async Task<EventItem> getEventItemById( ObjectId id )
  {
    IAsyncCursor<EventItem>? event_item = await _events_collection.FindAsync( getIdFilter( id ) );
    return await event_item.FirstOrDefaultAsync();
  }

  public async Task<List<EventItem>> getEventsByOrganizer(ObjectId object_id)
  {
    IAsyncCursor<EventItem>? event_item = await _events_collection.FindAsync( x => x.organizer.Equals(object_id) );
    return await event_item.ToListAsync();
  }

  public async Task<int> registerVolunteer( string id )
  {
    if ( !ObjectId.TryParse( id, out ObjectId object_id ) )
      return -1;

    EventItem event_item = await getEventItemById( object_id );
   ++event_item.cur_volunteer_count;

   if( event_item.cur_volunteer_count > event_item.max_volunteer_count )
     event_item.cur_volunteer_count = event_item.max_volunteer_count;

   await _events_collection.ReplaceOneAsync( getIdFilter( object_id ), event_item );
   return event_item.cur_volunteer_count;
  }

  public Task updateEvent(EventItem event_item)
  {
    return _events_collection.ReplaceOneAsync( x => x._id == event_item._id, event_item );
  }

  public async Task<RegisterVolunteerStatus> registerVolunteer(EventItem event_item, VolunteerItem volunteer_item)
  {
    var result = event_item.registerVolunteer(volunteer_item._id);
    if (result != RegisterVolunteerStatus.OK)
      return result;

    await updateEvent(event_item);
    if (volunteer_item.currentEvents.Contains(event_item._id))
      return RegisterVolunteerStatus.OK;

    volunteer_item.currentEvents.Add(event_item._id);
    await updateVolunteer(volunteer_item);

    return RegisterVolunteerStatus.OK;
  }

  private FilterDefinition<EventItem> getIdFilter( ObjectId id ) => Builders<EventItem>.Filter.Eq( nameof( EventItem._id ), id );
  #endregion
}