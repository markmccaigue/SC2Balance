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

        public void RunUniqueGamesPass(int ingestionId = 0)
        {
            using (var db = new DataContext())
            {
                var processingRun = new ProcessingRun {DateTime = DateTime.UtcNow};
                ingestionId = ingestionId == 0 ? GetLatestIngestionId(db) : ingestionId;

                var latestMatches = db.Matches.Where(x => x.Ingestion.Id == ingestionId);
                var duplicateMatches = latestMatches.Where(x => latestMatches.Any(y => y.Date == x.Date && x.Id != y.Id));

                // Avoid access to modified closure
                var matches = duplicateMatches;
                var badMatches = duplicateMatches.Where(x => matches.Where(d => d.Date == x.Date).FirstOrDefault(d => d.Decision == "WIN") == null);
                duplicateMatches = duplicateMatches.Except(badMatches);

                var populatedMatches = duplicateMatches.Include(x => x.LadderMember).Select(x => new
                    {
                        Date = x.Date,
                        LadderMember1 = x.LadderMember,
                        LadderMember2 = duplicateMatches.Where(y => x.Date == y.Date).FirstOrDefault(y => x.Id != y.Id).LadderMember,
                        Map = x.Map,
                        Speed = x.Speed,
                        Type = x.Type
                    }
                );

                var ingestion = db.Ingestions.FirstOrDefault(i => i.Id == ingestionId);
                foreach (var populatedMatch in populatedMatches)
                {
                    var date = UnixTimeStampToDateTime(populatedMatch.Date);
                    if (db.UniqueGmMatches.Any(u => DateTime.Equals(date, u.DateTime)))
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
                        Winner = populatedMatch.LadderMember1.Matches.Where(x => x.Date == populatedMatch.Date).First().Decision == "WIN" ? populatedMatch.LadderMember1 : populatedMatch.LadderMember2,
                        ProcessingRun = processingRun
                    };
                    db.UniqueGmMatches.Add(hydratedMatch);
                }

                db.SaveChanges();
            }
        }
    }
}