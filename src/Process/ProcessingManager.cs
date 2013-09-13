using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using SC2Balance.Ingest;
using SC2Balance.Models;

namespace Sc2Balance.Process
{
    public class ProcessingManager
    {
        //TODO: Move this UNIX conversion to an extention method on DateTime
        DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTimeStamp);
        }

        private int GetLatestIngestionId(DataContext db)
        {
            return db.Ingestions.OrderByDescending(x => x.Id).First().Id;
        }

        // TODO: Performance...
        public void RunUniqueGamesPass(int ingestionId = 0)
        {
            using (var db = new DataContext())
            {
                var processingRun = new ProcessingRun {DateTime = DateTime.UtcNow};
                ingestionId = ingestionId == 0 ? GetLatestIngestionId(db) : ingestionId;

                var latestMatches = db.Matches.Include(x => x.LadderMember).Where(x => x.Ingestion.Id == ingestionId);
                var duplicateMatches = latestMatches.Where(x => latestMatches.Any(y => y.Date == x.Date && x.Id != y.Id));

                // Avoid access to modified closure
                var matches = duplicateMatches;
                var badMatches = duplicateMatches.Where(x => matches.FirstOrDefault(d => d.Date == x.Date && d.Decision == "WIN") ==
                                            null);
                duplicateMatches = duplicateMatches.Except(badMatches);

                var populatedMatches = duplicateMatches.Select(x => new
                    {
                        Date = x.Date,
                        LadderMember1 = x.LadderMember,
                        LadderMember2 = duplicateMatches.FirstOrDefault(y => x.Date == y.Date && x.Id != y.Id).LadderMember,
                        Map = x.Map,
                        Speed = x.Speed,
                        Type = x.Type,
                        Winner = duplicateMatches.FirstOrDefault(d => d.Date == x.Date && d.Decision == "WIN").LadderMember
                    }
                ).ToList();

                var ingestion = db.Ingestions.FirstOrDefault(i => i.Id == ingestionId);
                var uniqueMatches = new List<UniqueGmMatch>();
                foreach (var populatedMatch in populatedMatches)
                {
                    var date = UnixTimeStampToDateTime(populatedMatch.Date);
                    if (uniqueMatches.Any(u => DateTime.Equals(date, u.DateTime)))
                    {
                        continue;
                    }

                    var hydratedMatch = new UniqueGmMatch
                    {
                        DateTime = date,
                        Ingestion = ingestion,
                        LadderMember1 = populatedMatch.LadderMember1,
                        LadderMember2 = populatedMatch.LadderMember2,
                        Map = populatedMatch.Map,
                        Speed = populatedMatch.Speed,
                        Type = populatedMatch.Type,
                        Winner = populatedMatch.Winner,
                        ProcessingRun = processingRun
                    };

                    uniqueMatches.Add(hydratedMatch);
                }

                uniqueMatches.ForEach(u => db.UniqueGmMatches.Add(u));
                db.SaveChanges();
            }
        }
    }
}