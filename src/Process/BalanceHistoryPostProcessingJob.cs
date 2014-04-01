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
                var currentPointsIngestion = db.Ingestions.OrderByDescending(x => x.Id).ToList().ElementAt(1);
                var latestCurrentlyAnalysedGameTime = db.UniqueGmMatches.Where(m => m.IngestionId == currentPointsIngestion.Id).OrderByDescending(m => m.DateTime).First().DateTime;

                var newGamesTimeSpan = DateTime.UtcNow - latestCurrentlyAnalysedGameTime;

                var currentPointsJson = db.PostProcessingOutputs.OrderByDescending(x => x.ProcessingRunId).First(x => x.PostProcessingJobType == JobType).JsonResults;
                var currentPoints = Newtonsoft.Json.JsonConvert.DeserializeObject<BalanceHistoryPoint[]>(currentPointsJson);

                var newGames = GetUniqueGamesInTimeSpan(newGamesTimeSpan);
                var newPoints = new List<BalanceHistoryPoint>();
                if (newGames.Count != 0)
                {
                    newPoints = GetBalanceHistoryPointsFromUniqueGames(newGames).ToList();
                    newPoints = newPoints.SkipWhile(x => x.DateTime <= currentPointsIngestion.Time).ToList();
                }

                var newTotalPoints = currentPoints.Concat(newPoints);

                var json = JsonConvert.SerializeObject(newTotalPoints.OrderBy(x => x.DateTime));
                Save(json);
            }
        }
    }
}
