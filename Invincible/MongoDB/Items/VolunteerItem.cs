using MongoDB.Bson;
using RebuildUkraineHackatonWebAPI.MongoDB;

namespace Invincible.Items;

public class VolunteerItem : UserItem
{
    public string    name                  { get; set; }
    public int       city_code             { get; set; }
    public long      experience            { get; set; }
    public int       current_puzzle_id     { get; set; }
    public int[]     current_puzzle_pieces { get; set; } = new int[3];

    public List<int> completed_puzzles_ids { get; set; } = new List<int>();

    public VolunteerStatistics statistics  { get; set; } = new VolunteerStatistics();

    public List<ObjectId> currentEvents    { get; set; } = new List<ObjectId>();

    public int level => (int) (experience / 100L);

    public VolunteerItem(VolunteerCreationData volunteer_creation_data)
    {
        google_id = volunteer_creation_data.googleId;
    }
}
