using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC2Balance.Models
{
    public class Ingestion
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public ICollection<LadderMember> LadderMembers { get; set; }
    }
}
