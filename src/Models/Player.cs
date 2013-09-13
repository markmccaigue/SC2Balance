namespace SC2Balance.Models
{

    public class Player
    {
        public Character Character { get; set; }
        public Portrait Portrait { get; set; }
        public Career Career { get; set; }
        public SwarmLevels SwarmLevels { get; set; }
        public Campaign Campaign { get; set; }
        public Season Season { get; set; }
        public Rewards Rewards { get; set; }
        public Achievements Achievements { get; set; }
    }
}