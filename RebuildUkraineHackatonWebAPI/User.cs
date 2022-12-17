using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RebuildUkraineHackathonWebAPI;

public class User
{
  public string Name    { get; set; }
  public int    Age     { get; set; }
  public bool   IsBitch { get; set; }
}