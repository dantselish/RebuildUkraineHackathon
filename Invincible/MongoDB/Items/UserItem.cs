using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Invincible.Items;

public class UserItem
{
  [BsonId] public ObjectId _id;

  public string google_id { get; set; }
}
