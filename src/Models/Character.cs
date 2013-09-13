using System.ComponentModel.DataAnnotations;

namespace SC2Balance.Models
{
    public class Character
    {
        [Key]
        public int EntityId { get; set; }
        public int Id { get; set; }
        public int Realm { get; set; }
        public string DisplayName { get; set; }
        public string ClanName { get; set; }
        public string ClanTag { get; set; }
        public string ProfilePath { get; set; }
    }
}