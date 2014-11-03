namespace GitCode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.User",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Email = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.DetailTeam",
                c => new
                    {
                        DetailTeamId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        TeamId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DetailTeamId)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Team", t => t.TeamId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.TeamId);
            
            CreateTable(
                "dbo.Team",
                c => new
                    {
                        TeamId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.TeamId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.DetailTeam", new[] { "TeamId" });
            DropIndex("dbo.DetailTeam", new[] { "UserId" });
            DropForeignKey("dbo.DetailTeam", "TeamId", "dbo.Team");
            DropForeignKey("dbo.DetailTeam", "UserId", "dbo.User");
            DropTable("dbo.Team");
            DropTable("dbo.DetailTeam");
            DropTable("dbo.User");
        }
    }
}
