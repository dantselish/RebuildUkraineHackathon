using MongoDB.Bson;

namespace RebuildUkraineHackathonWebAPI;


public class UserWithId : User
{
  public ObjectId _id { get; set; }

  public UserWithId( User other )
  {
    Name    = other.Name;
    Age     = other.Age;
    IsBitch = other.IsBitch;

    _id = ObjectId.GenerateNewId();
  }
}