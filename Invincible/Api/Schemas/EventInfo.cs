using Invincible.Items;
using Invincible.Event;
using Invincible.Extensions;

namespace Invincible.Api.Schemas;

public class EventInfo : EventBasicInfo
{
  public string Desctiption  { get; set; }
  public string Duties       { get; set; }


  public EventInfo()
  {
  }

  public EventInfo( EventItem event_item )
    : base( event_item )
  {
    Desctiption  = event_item.description;
    Duties       = event_item.duties;
  }

  public static implicit operator EventInfo ( EventItem event_item ) => new EventInfo( event_item );
}