namespace Invincible.Responces;

public class AllUsersResponse
{
  public IReadOnlyList<User> users { get; set; } = new List<User>();
}