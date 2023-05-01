using Invincible.Event;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RebuildUkraineHackatonWebAPI.MongoDB;

namespace Invincible.Items;

public class EventItem
{
  [BsonId] public ObjectId _id;

  public string      name         { get; set; }
  public string      description  { get; set; }
  public string      location     { get; set; }
  public int         city_code    { get; set; }
  public long        timestamp    { get; set; }
  public int         event_lenght { get; set; }

  public EventType   event_type  { get; set; }
  public EventStatus event_status { get; set; }

  public int max_volunteer_count { get; set; }
  public int cur_volunteer_count { get; set; }

  public ObjectId organizer { get; set; }

  public byte[] confirm_secret { get; set; }

  public List<ObjectId> registered_volunteers { get; set; } = new List<ObjectId>();
  public List<ObjectId> confirmed_volunteers  { get; set; } = new List<ObjectId>();

  //TODO organizer, exp reward (const in config)

  public EventItem(EventCreationData creation_data)
  {
    organizer           = creation_data.organizer;
    name                = creation_data.name;
    description         = creation_data.description;
    location            = creation_data.location;
    timestamp           = creation_data.timestamp;
    city_code           = creation_data.cityCode;
    event_lenght        = creation_data.eventLenght;
    event_type          = creation_data.eventType;
    max_volunteer_count = creation_data.volunteersRequired;

    event_status = EventStatus.ACTIVE;
  }

  public RegisterVolunteerStatus registerVolunteer(ObjectId object_id)
  {
    if (cur_volunteer_count >= max_volunteer_count)
      return RegisterVolunteerStatus.MAX_VOLUNTEERS_COUNT;

    if (registered_volunteers.Contains(object_id))
      return RegisterVolunteerStatus.ALREADY_REGISTERED;

    if (event_status != EventStatus.ACTIVE)
      return RegisterVolunteerStatus.WRONG_EVENT_STATUS;

    ++cur_volunteer_count;
    registered_volunteers.Add(object_id);
    return RegisterVolunteerStatus.OK;
  }
}

public enum RegisterVolunteerStatus
{
  OK,

  ALREADY_REGISTERED,
  MAX_VOLUNTEERS_COUNT,
  WRONG_EVENT_STATUS
}