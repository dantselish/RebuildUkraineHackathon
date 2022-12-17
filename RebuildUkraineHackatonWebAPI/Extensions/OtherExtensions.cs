namespace RebuildUkraineHackathonWebAPI.Extensions;


public static class OtherExtensions
{
  public static DateTime toDateTime( this long unix_time_stamp )
  {
    // Unix timestamp is seconds past epoch
    DateTime date_time = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    date_time = date_time.AddSeconds( unix_time_stamp );
    return date_time;
  }

  public static DateOnly toDateOnly( this long unix_time_stamp ) => DateOnly.FromDateTime( unix_time_stamp.toDateTime() );
}