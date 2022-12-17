using RebuildUkraineHackathonWebAPI.Event;
using RebuildUkraineHackathonWebAPI.Extensions;
using RebuildUkraineHackathonWebAPI.Items;

namespace RebuildUkraineHackathonWebAPI.Api.Schemas;

public class EventInfo : EventBasicInfo
{
  public string Desctiption  { get; set; }
  public string Duties       { get; set; }
  public string Organizer    { get; set; }
  public bool   IsRegistered { get; set; }


  public EventInfo()
  {
  }

  public EventInfo( EventItem event_item )
    : base( event_item )
  {
    Desctiption  = event_item.description;
    Duties       = event_item.duties;
    Organizer    = event_item.organizer;
    IsRegistered = event_item.is_registered;
  }

  public static implicit operator EventInfo ( EventItem event_item ) => new EventInfo( event_item );
}