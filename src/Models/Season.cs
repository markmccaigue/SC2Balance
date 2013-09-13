using System.Collections.Generic;

namespace SC2Balance.Models
{
    public class Season
    {
        public int SeasonId { get; set; }
        public int TotalGamesThisSeason { get; set; }
        public IEnumerable<Stat> Stats { get; set; }
    }
}