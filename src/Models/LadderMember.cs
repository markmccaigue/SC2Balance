using System.Collections.Generic;

namespace SC2Balance.Models
{
    public class LadderMember
    {
        public int Id { get; set; }
        public Character Character { get; set; }
        public int JoinTimestamp { get; set; }
        public float Points { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int HighestRank { get; set; }
        public int PreviousRank { get; set; }
        public string FavoriteRaceP1 { get; set; }
        public virtual ICollection<Match> Matches { get; set; }
        public LadderRegion LadderRegion { get; set; }
    }
}