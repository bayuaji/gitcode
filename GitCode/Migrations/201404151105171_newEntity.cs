namespace GitCode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DetailClass",
                c => new
                    {
                        DetailClassId = c.Int(nullable: false, identity: true),
                        TeamId = c.Int(nullable: false),
                        ClassId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DetailClassId)
                .ForeignKey("dbo.Team", t => t.TeamId, cascadeDelete: true)
                .ForeignKey("dbo.Class", t => t.ClassId, cascadeDelete: true)
                .Index(t => t.TeamId)
                .Index(t => t.ClassId);
            
            CreateTable(
                "dbo.Class",
                c => new
                    {
                        ClassId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ClassId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.DetailClass", new[] { "ClassId" });
            DropIndex("dbo.DetailClass", new[] { "TeamId" });
            DropForeignKey("dbo.DetailClass", "ClassId", "dbo.Class");
            DropForeignKey("dbo.DetailClass", "TeamId", "dbo.Team");
            DropTable("dbo.Class");
            DropTable("dbo.DetailClass");
        }
    }
}
