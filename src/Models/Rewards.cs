using System.Collections.Generic;

namespace SC2Balance.Models
{
    public class Rewards
    {
        public IEnumerable<long> Selected { get; set; }
        public IEnumerable<long> Earned { get; set; }
    }
}