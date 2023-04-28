using RebuildUkraineHackatonWebAPI.MongoDB;

namespace Invincible.Items;

public class VolunteerItem : UserItem
{
    public string    name                  { get; set; }
    public int       city_code             { get; set; }
    public long      experience            { get; set; }
    public int       current_puzzle_id     { get; set; }
    public int[]     current_puzzle_pieces { get; set; }

    public List<int> completed_puzzles_ids { get; set; }

    public VolunteerStatistics statistics { get; set; }

    public int level => (int) (experience / 100L);

    public VolunteerItem(VolunteerCreationData volunteer_creation_data)
    {
        google_id = volunteer_creation_data.googleId;
    }
}
