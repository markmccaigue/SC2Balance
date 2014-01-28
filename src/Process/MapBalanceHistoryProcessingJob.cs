using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC2Balance.Ingest;
using SC2Balance.Models;
using Newtonsoft.Json;

namespace Sc2Balance.Process
{
    public class MapBalanceHistoryPostProcessingJob : AbstractPostProcessingJob
    {
        public override string JobType
        {
            get { return "MAPBALANCEHISTORY"; }
        }

        public override void Run()
        {
            var uniqueGames = GetUniqueGamesInTimeSpan(TimeSpan.MaxValue);
            MapBalanceHistoryPoints[] mapBalanceHistoryPoints = null;

            using (var db = new DataContext())
            {
                var maps = db.UniqueGmMatches.Where(g => g.Type == "SOLO").Select(x => x.Map).Distinct().ToList();

                mapBalanceHistoryPoints =  maps.Select(map => new MapBalanceHistoryPoints
                {
                    Map = map,
                    BalanceHistoryPoints = GetBalanceHistoryPointsFromUniqueGames(uniqueGames, map)
                }).ToArray();
            }

            var json = JsonConvert.SerializeObject(mapBalanceHistoryPoints);
            Save(json);
        }
    }
}
