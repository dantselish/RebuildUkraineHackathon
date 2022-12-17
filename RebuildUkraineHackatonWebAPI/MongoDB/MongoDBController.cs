using MongoDB.Bson;
using MongoDB.Driver;
using RebuildUkraineHackathonWebAPI.Event;
using RebuildUkraineHackathonWebAPI.Items;
using RebuildUkraineHackatonWebAPI.MongoDB;

namespace RebuildUkraineHackathonWebAPI;


public class MongoDBController : IMongoDBController
{
  private const string EVENTS_COLLECTION_NAME = "Events";
  private const string DATABASE_NAME          = "RebuildUkraineHackathon";

  private readonly IConfiguration _configuration;

  public string connection_string;

  private MongoClient    _mongo_client;
  private IMongoDatabase _database;

  private IMongoCollection<UserWithId> _user_collection;
  private IMongoCollection<EventItem>  _events_collection;


  public MongoDBController( IConfiguration configuration )
  {
    _configuration = configuration;
    connection_string = _configuration["MongoDBConnectionString"];

    _mongo_client = new MongoClient( connection_string );
    _database = _mongo_client.GetDatabase( DATABASE_NAME );
    _user_collection = _database.GetCollection<UserWithId>( "test" );
    _events_collection = _database.GetCollection<EventItem>( EVENTS_COLLECTION_NAME );
  }

  public Task insertUser( User user )
  {
    return _user_collection.InsertOneAsync( new UserWithId( user ) );
  }

  public async Task<IReadOnlyList<User>> getAllUsers()
  {
    IAsyncCursor<UserWithId>? users = await _user_collection.FindAsync( _ => true );
    return await users.ToListAsync();
  }

  public async Task<List<EventItem>> getAllEvents()
  {
    IAsyncCursor<EventItem>? events = await _events_collection.FindAsync( _ => true );
    return await events.ToListAsync();
  }

  public async Task<EventItem> getEventItemById( string id )
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

  public async Task<int> registerVolunteer( string id )
  {
    if ( !ObjectId.TryParse( id, out ObjectId object_id ) )
      return -1;

    EventItem event_item = await getEventItemById( object_id );
   ++event_item.cur_volunteer_count;
   event_item.is_registered = true;

   if( event_item.cur_volunteer_count > event_item.max_volunteer_count )
     event_item.cur_volunteer_count = event_item.max_volunteer_count;

   await _events_collection.ReplaceOneAsync( getIdFilter( object_id ), event_item );
   return event_item.cur_volunteer_count;
  }

  private FilterDefinition<EventItem> getIdFilter( ObjectId id ) => Builders<EventItem>.Filter.Eq( nameof( EventItem._id ), id );
}