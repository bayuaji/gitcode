namespace GitCode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newEntitydetailclassaccess : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DetailClassAccess",
                c => new
                    {
                        DetailClassAccessId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ClassId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DetailClassAccessId)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Class", t => t.ClassId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ClassId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.DetailClassAccess", new[] { "ClassId" });
            DropIndex("dbo.DetailClassAccess", new[] { "UserId" });
            DropForeignKey("dbo.DetailClassAccess", "ClassId", "dbo.Class");
            DropForeignKey("dbo.DetailClassAccess", "UserId", "dbo.User");
            DropTable("dbo.DetailClassAccess");
        }
    }
}
