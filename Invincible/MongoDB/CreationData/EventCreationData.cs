using Invincible.Event;
using MongoDB.Bson;

namespace RebuildUkraineHackatonWebAPI.MongoDB;

public class EventCreationData
{
    public ObjectId  organizer           { get; set; }
    public string    name               { get; set; }
    public string    description        { get; set; }
    public string    location           { get; set; }
    public long      timestamp          { get; set; }
    public int       cityCode           { get; set; }
    public int       eventLenght        { get; set; }
    public int       volunteersRequired { get; set; }
    public EventType eventType          { get; set; }
}
