using System.Collections.Generic;

namespace SC2Balance.Models
{
    public class Achievements
    {
        public Points Points { get; set; }
        public IEnumerable<Achievement> AchievementsCollection { get; set; }
    }
}