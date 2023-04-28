using Invincible.Api.Schemas;

namespace Invincible.Responces;

public class GetEventsResponse
{
  public List<EventBasicInfo> events { get; set; }
}