namespace GitCode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newEntityFeature : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Feature",
                c => new
                    {
                        FeatureId = c.Int(nullable: false, identity: true),
                        Detail = c.String(),
                        RepositoryRepositoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FeatureId)
                .ForeignKey("dbo.Repository", t => t.RepositoryRepositoryId, cascadeDelete: true)
                .Index(t => t.RepositoryRepositoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Feature", "RepositoryRepositoryId", "dbo.Repository");
            DropIndex("dbo.Feature", new[] { "RepositoryRepositoryId" });
            DropTable("dbo.Feature");
        }
    }
}
