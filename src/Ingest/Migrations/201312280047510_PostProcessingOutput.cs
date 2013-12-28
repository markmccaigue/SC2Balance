namespace SC2Balance.Ingest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PostProcessingOutput : DbMigration
    {
        public override void Up()
        {
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
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.PostProcessingOutputs", new[] { "ProcessingRunId" });
            DropForeignKey("dbo.PostProcessingOutputs", "ProcessingRunId", "dbo.ProcessingRuns");
            DropTable("dbo.PostProcessingOutputs");
        }
    }
}
