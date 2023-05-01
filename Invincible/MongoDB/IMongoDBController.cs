using Invincible;
using Invincible.Api.RequestsBodies.Organizer;
using Invincible.Api.RequestsBodies.Volunteer;
using Invincible.Items;
using MongoDB.Bson;
using RebuildUkraineHackatonWebAPI.MongoDB;
using RebuildUkraineHackatonWebAPI.MongoDB.StatusCodes;

namespace Invincible.MongoDB;

public interface IMongoDBController
{
  Task<VolunteerItem?> getVolunteer(string google_id);
  Task createVolunteer(VolunteerCreationData volunteer_creation_data);
  Task createVolunteer();
  Task<UpdateStatusCode> updateVolunteerInfo(string google_id, PutVolunteerRequestBody info);
  Task<UpdateStatusCode> updateOrganizerInfo(string google_id, PutOrganizerRequestBody info);
  Task<OrganizerItem?> getOrganizer(string google_id);
  Task createOrganizer(OrganizerCreationData organizer_creation_data);
  Task createOrganizer();
  Task createEvent(EventCreationData creation_data);
  Task<List<EventItem>> getEventsByOrganizer(ObjectId object_id);
  Task<List<EventItem>> getAllEvents();
  Task<EventItem?> getEventItemById( string id );
  Task<int> registerVolunteer( string id );
  Task<RegisterVolunteerStatus> registerVolunteer(EventItem event_item, VolunteerItem volunteer_item);
}