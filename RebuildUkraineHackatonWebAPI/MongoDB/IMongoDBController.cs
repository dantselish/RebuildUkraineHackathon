using RebuildUkraineHackathonWebAPI;
using RebuildUkraineHackathonWebAPI.Items;

namespace RebuildUkraineHackatonWebAPI.MongoDB;

public interface IMongoDBController
{
  Task insertUser( User user );
  Task<IReadOnlyList<User>> getAllUsers();
  Task<List<EventItem>> getAllEvents();
  Task<EventItem> getEventItemById( string id );
  Task<int> registerVolunteer( string id );
}