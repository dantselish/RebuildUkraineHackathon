using Invincible.Event;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Invincible.Items;

public class EventItem
{
  [BsonId] public ObjectId _id;

  public string      name              { get; set; }
  public string      description       { get; set; }
  public string      duties            { get; set; }
  public string      location          { get; set; }
  public int         city_code         { get; set; }
  public long        timestamp         { get; set; }
  public int         event_lenght      { get; set; }
  public int         city_geoname_code { get; set; }

  public EventType   event_type  { get; set; }
  public EventStatus event_status { get; set; }

  public int max_volunteer_count { get; set; }
  public int cur_volunteer_count { get; set; }

  //TODO organizer, city, exp reward (const in config)
}