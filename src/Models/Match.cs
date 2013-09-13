using System.Security.AccessControl;

namespace SC2Balance.Models
{
    public class Match
    {
        public int Id { get; set; }
        public string Map { get; set; }
        public string Type { get; set; }
        public string Decision { get; set; }
        public string Speed { get; set; }
        public int Date { get; set; }

        public int LadderMemberId { get; set; }
        public virtual LadderMember LadderMember { get; set; }
        public int IngestionId { get; set; }
        public virtual Ingestion Ingestion { get; set; }
    }
}