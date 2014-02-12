namespace SC2Balance.Ingest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ingestions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LadderMembers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        JoinTimestamp = c.Int(nullable: false),
                        Points = c.Single(nullable: false),
                        Wins = c.Int(nullable: false),
                        Losses = c.Int(nullable: false),
                        HighestRank = c.Int(nullable: false),
                        PreviousRank = c.Int(nullable: false),
                        FavoriteRaceP1 = c.String(),
                        LadderRegion = c.Int(nullable: false),
                        Character_EntityId = c.Int(),
                        Ingestion_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Characters", t => t.Character_EntityId)
                .ForeignKey("dbo.Ingestions", t => t.Ingestion_Id)
                .Index(t => t.Character_EntityId)
                .Index(t => t.Ingestion_Id);
            
            CreateTable(
                "dbo.Characters",
                c => new
                    {
                        EntityId = c.Int(nullable: false, identity: true),
                        Id = c.Int(nullable: false),
                        Realm = c.Int(nullable: false),
                        DisplayName = c.String(),
                        ClanName = c.String(),
                        ClanTag = c.String(),
                        ProfilePath = c.String(),
                    })
                .PrimaryKey(t => t.EntityId);
            
            CreateTable(
                "dbo.Matches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Map = c.String(),
                        Type = c.String(),
                        Decision = c.String(),
                        Speed = c.String(),
                        Date = c.Int(nullable: false),
                        LadderMemberId = c.Int(nullable: false),
                        IngestionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Ingestions", t => t.IngestionId, cascadeDelete: true)
                .ForeignKey("dbo.LadderMembers", t => t.LadderMemberId, cascadeDelete: true)
                .Index(t => t.IngestionId)
                .Index(t => t.LadderMemberId);
            
            CreateTable(
                "dbo.PostProcessingOutputs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProcessingRunId = c.Int(nullable: false),
                        PostProcessingJobType = c.String(),
                        JsonResults = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProcessingRuns", t => t.ProcessingRunId, cascadeDelete: true)
                .Index(t => t.ProcessingRunId);
            
            CreateTable(
                "dbo.ProcessingRuns",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UniqueGmMatches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Map = c.String(),
                        Type = c.String(),
                        Speed = c.String(),
                        DateTime = c.DateTime(nullable: false),
                        IngestionId = c.Int(nullable: false),
                        ProcessingRunId = c.Int(nullable: false),
                        LadderMember1_Id = c.Int(),
                        LadderMember2_Id = c.Int(),
                        Winner_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Ingestions", t => t.IngestionId, cascadeDelete: true)
                .ForeignKey("dbo.LadderMembers", t => t.LadderMember1_Id)
                .ForeignKey("dbo.LadderMembers", t => t.LadderMember2_Id)
                .ForeignKey("dbo.ProcessingRuns", t => t.ProcessingRunId, cascadeDelete: true)
                .ForeignKey("dbo.LadderMembers", t => t.Winner_Id)
                .Index(t => t.IngestionId)
                .Index(t => t.LadderMember1_Id)
                .Index(t => t.LadderMember2_Id)
                .Index(t => t.ProcessingRunId)
                .Index(t => t.Winner_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UniqueGmMatches", "Winner_Id", "dbo.LadderMembers");
            DropForeignKey("dbo.UniqueGmMatches", "ProcessingRunId", "dbo.ProcessingRuns");
            DropForeignKey("dbo.UniqueGmMatches", "LadderMember2_Id", "dbo.LadderMembers");
            DropForeignKey("dbo.UniqueGmMatches", "LadderMember1_Id", "dbo.LadderMembers");
            DropForeignKey("dbo.UniqueGmMatches", "IngestionId", "dbo.Ingestions");
            DropForeignKey("dbo.PostProcessingOutputs", "ProcessingRunId", "dbo.ProcessingRuns");
            DropForeignKey("dbo.LadderMembers", "Ingestion_Id", "dbo.Ingestions");
            DropForeignKey("dbo.Matches", "LadderMemberId", "dbo.LadderMembers");
            DropForeignKey("dbo.Matches", "IngestionId", "dbo.Ingestions");
            DropForeignKey("dbo.LadderMembers", "Character_EntityId", "dbo.Characters");
            DropIndex("dbo.UniqueGmMatches", new[] { "Winner_Id" });
            DropIndex("dbo.UniqueGmMatches", new[] { "ProcessingRunId" });
            DropIndex("dbo.UniqueGmMatches", new[] { "LadderMember2_Id" });
            DropIndex("dbo.UniqueGmMatches", new[] { "LadderMember1_Id" });
            DropIndex("dbo.UniqueGmMatches", new[] { "IngestionId" });
            DropIndex("dbo.PostProcessingOutputs", new[] { "ProcessingRunId" });
            DropIndex("dbo.LadderMembers", new[] { "Ingestion_Id" });
            DropIndex("dbo.Matches", new[] { "LadderMemberId" });
            DropIndex("dbo.Matches", new[] { "IngestionId" });
            DropIndex("dbo.LadderMembers", new[] { "Character_EntityId" });
            DropTable("dbo.UniqueGmMatches");
            DropTable("dbo.ProcessingRuns");
            DropTable("dbo.PostProcessingOutputs");
            DropTable("dbo.Matches");
            DropTable("dbo.Characters");
            DropTable("dbo.LadderMembers");
            DropTable("dbo.Ingestions");
        }
    }
}
