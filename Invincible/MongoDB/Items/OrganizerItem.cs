using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RebuildUkraineHackatonWebAPI.MongoDB;

namespace Invincible.Items;

public class OrganizerItem : UserItem
{
  public string name         { get; set; }
  public string description  { get; set; }
  public string phone_number { get; set; }

  public List<ObjectId> events { get; set; } = new List<ObjectId>();

  public OrganizerStatistics statistics { get; set; } = new OrganizerStatistics();

  public OrganizerItem(OrganizerCreationData organizer_creation_data)
  {
    google_id = organizer_creation_data.googleId;
  }
}
