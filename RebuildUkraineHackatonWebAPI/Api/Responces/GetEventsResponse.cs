using RebuildUkraineHackathonWebAPI.Api.Schemas;

namespace RebuildUkraineHackathonWebAPI.Responces;

public class GetEventsResponse
{
  public List<EventBasicInfo> events { get; set; }
}