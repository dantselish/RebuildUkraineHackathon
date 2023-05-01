using Invincible.Event;
using Invincible.Items;
using Invincible.Extensions;

namespace Invincible.Api.Schemas;


public class EventBasicInfo
{
  public string Id       { get; set; }
  public string Name     { get; set; }
  public string Location { get; set; }
  public DateTime Date { get; set; }
  public EventType Type { get; set; }

  public int CurVolunteers { get; set; }
  public int MaxVolunteers { get; set; }


  public EventBasicInfo()
  {
  }

  public EventBasicInfo( EventItem event_item )
  {
    Id            = event_item._id.ToString();
    Name          = event_item.name;
    Location      = event_item.location;
    Date          = event_item.timestamp.toDateTime();
    Type          = event_item.event_type;
    CurVolunteers = event_item.cur_volunteer_count;
    MaxVolunteers = event_item.max_volunteer_count;
  }

  public static explicit operator EventBasicInfo( EventItem event_item ) => new EventBasicInfo( event_item );
}