using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC2Balance.Models
{
    public class UniqueGmMatch
    {
        public int Id{ get; set; }
        public LadderMember LadderMember1 { get; set; }
        public LadderMember LadderMember2 { get; set; }
        public Ingestion Ingestion { get; set; }
        public String Map { get; set; }
        public String Type { get; set; }
        public String Speed { get; set; }
        public DateTime DateTime { get; set; }
        public LadderMember Winner { get; set; }

        public int IngestionId { get; set; }
        public int ProcessingRunId { get; set; }
        public virtual ProcessingRun ProcessingRun { get; set; }
    }
}
