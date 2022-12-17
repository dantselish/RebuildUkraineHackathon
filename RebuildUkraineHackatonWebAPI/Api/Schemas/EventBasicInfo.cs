using RebuildUkraineHackathonWebAPI.Event;
using RebuildUkraineHackathonWebAPI.Extensions;
using RebuildUkraineHackathonWebAPI.Items;

namespace RebuildUkraineHackathonWebAPI.Api.Schemas;


public class EventBasicInfo
{
  public string Id       { get; set; }
  public string Name     { get; set; }
  public string Location { get; set; }
  public DateTime  Date { get; set; }
  public EventType Type { get; set; }

  public int  GamePoints { get; set; }

  public int CurVolunteers { get; set; }
  public int MaxVolunteers { get; set; }

  public int MoneyNeeded   { get; set; }
  public int MoneyDonated  { get; set; }


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
    GamePoints    = event_item.game_points;
    CurVolunteers = event_item.cur_volunteer_count;
    MaxVolunteers = event_item.max_volunteer_count;
    MoneyNeeded   = event_item.money_needed;
    MoneyDonated  = event_item.money_donated;
  }

  public static implicit operator EventBasicInfo( EventItem event_item ) => new EventBasicInfo( event_item );
}