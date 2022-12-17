using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RebuildUkraineHackathonWebAPI.Event;

namespace RebuildUkraineHackathonWebAPI.Items;

public class EventItem
{
  [BsonId] public ObjectId _id;

  public string    name        { get; set; }
  public string    description { get; set; }
  public string    duties      { get; set; }
  public string    organizer   { get; set; }
  public string    location    { get; set; }
  public long      timestamp   { get; set; }
  public EventType event_type  { get; set; }

  public int max_volunteer_count { get; set; }
  public int cur_volunteer_count { get; set; }

  public int money_needed  { get; set; }
  public int money_donated { get; set; }

  public int game_points { get; set; }

  public bool is_registered { get; set; }
}