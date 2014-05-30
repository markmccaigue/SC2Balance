using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC2Balance.Models;
using SC2Balance.Ingest.Migrations;

namespace SC2Balance.Ingest
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
            var productionEnvironmentVariable = Environment.GetEnvironmentVariable("APPSETTING_Production");
            var isProduction = string.IsNullOrEmpty(productionEnvironmentVariable) || !Convert.ToBoolean(productionEnvironmentVariable) ? false : true;
            var initialiser = isProduction ? null : new MigrateDatabaseToLatestVersion<DataContext, Configuration>();
            
            Database.SetInitializer<DataContext>(initialiser);
        }

        public DbSet<Ingestion> Ingestions { get; set; }
        public DbSet<LadderMember> LadderMembers { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<UniqueGmMatch> UniqueGmMatches { get; set; }
        public DbSet<ProcessingRun> ProcessingRuns { get; set; }
        public DbSet<PostProcessingOutput> PostProcessingOutputs { get; set; }
    }
}
