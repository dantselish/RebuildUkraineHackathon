using MongoDB.Bson;

namespace Invincible.User;

public class BasicVolunteerInfo
{
    public ObjectId objectId { get; set; }
    public string name { get; set; }
}
