namespace GitCode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNewTableRepo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DetailRepository",
                c => new
                    {
                        DetailRepositoryId = c.Int(nullable: false, identity: true),
                        IsOwner = c.String(),
                        UserId = c.Int(nullable: false),
                        RepositoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DetailRepositoryId)
                .ForeignKey("dbo.Repository", t => t.RepositoryId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RepositoryId);
            
            CreateTable(
                "dbo.Repository",
                c => new
                    {
                        RepositoryId = c.Int(nullable: false, identity: true),
                        IsPublic = c.String(),
                        TeamId = c.Int(),
                        RepoHeadDirectory = c.String(),
                        RepoChildDirectory = c.String(),
                    })
                .PrimaryKey(t => t.RepositoryId)
                .ForeignKey("dbo.Team", t => t.TeamId)
                .Index(t => t.TeamId);
            
            AddColumn("dbo.DetailTeam", "IsOwner", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DetailRepository", "UserId", "dbo.User");
            DropForeignKey("dbo.Repository", "TeamId", "dbo.Team");
            DropForeignKey("dbo.DetailRepository", "RepositoryId", "dbo.Repository");
            DropIndex("dbo.Repository", new[] { "TeamId" });
            DropIndex("dbo.DetailRepository", new[] { "RepositoryId" });
            DropIndex("dbo.DetailRepository", new[] { "UserId" });
            DropColumn("dbo.DetailTeam", "IsOwner");
            DropTable("dbo.Repository");
            DropTable("dbo.DetailRepository");
        }
    }
}
