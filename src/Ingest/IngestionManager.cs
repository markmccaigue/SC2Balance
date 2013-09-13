using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SC2Balance.Models;

namespace SC2Balance.Ingest
{
    public class IngestionManager
    {
        private List<LadderMember> GetGmPlayersWithMatchesForRegion(LadderRegion region = LadderRegion.NorthAmerica)
        {
            var api = new ApiWrapper();

            var ladderMembers = api.GetGrandmasterMembers(region).Result.ToList();

            var count = 0;
            foreach (var ladderMember in ladderMembers)
            {
                Console.WriteLine("Processing: " + ladderMember.Character.DisplayName + " Count: "+ count++);
                var matches = api.GetRecentMatchesForPlayer(ladderMember.Character.ProfilePath, region).Result.ToList();
                ladderMember.Matches = matches;
                ladderMember.LadderRegion = region;
            }

            return ladderMembers;
        }
                public void RunNewIngestion()
        {
            var ingestion = new Ingestion
            {
                Time = DateTime.UtcNow
            };

            var ladderMembers = GetGmPlayersWithMatchesForRegion(LadderRegion.NorthAmerica);
            ladderMembers.AddRange(GetGmPlayersWithMatchesForRegion(LadderRegion.Europe));
            ladderMembers.AddRange(GetGmPlayersWithMatchesForRegion(LadderRegion.Korea));

            ingestion.LadderMembers = ladderMembers;

            using (var db = new DataContext())
            {
                db.Ingestions.Add(ingestion);
                db.SaveChanges();
            }
        }
    }
}
