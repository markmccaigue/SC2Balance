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
    public class BalanceHistoryPostProcessingJob : AbstractPostProcessingJob
    {
        public override string JobType
        {
            get { return PostProcessingJobType.BALANCEHISTORY.ToString(); }
        }

        public override void Run()
        {
            using (var db = new DataContext())
            {
                var latestPointsJson = db.PostProcessingOutputs.OrderByDescending(x => x.ProcessingRunId).First(x => x.PostProcessingJobType == JobType).JsonResults;
                var latestPoints = Newtonsoft.Json.JsonConvert.DeserializeObject<BalanceHistoryPoint[]>(latestPointsJson);
                var latestGameDate = latestPoints.OrderByDescending(x => x.DateTime).First().DateTime;
                var span = DateTime.UtcNow - latestGameDate;

                var newGames = GetUniqueGamesInTimeSpan(span);
                if (newGames.Count == 0)
                {
                    return;
                }
                var newPoints = GetBalanceHistoryPointsFromUniqueGames(newGames);

                var newTotalPoints = latestPoints.Concat(newPoints);

                var json = JsonConvert.SerializeObject(newTotalPoints);
                Save(json);
            }
        }
    }
}
