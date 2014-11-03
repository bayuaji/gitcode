namespace GitCode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add4Entity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comment",
                c => new
                    {
                        CommentId = c.Int(nullable: false, identity: true),
                        IssueId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Content = c.String(),
                        When = c.String(),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.Issue", t => t.IssueId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: false)
                .Index(t => t.IssueId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Issue",
                c => new
                    {
                        IssueId = c.Int(nullable: false, identity: true),
                        RepositoryId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Title = c.String(),
                        Content = c.String(),
                        When = c.String(),
                        IsClear = c.String(),
                    })
                .PrimaryKey(t => t.IssueId)
                .ForeignKey("dbo.Repository", t => t.RepositoryId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RepositoryId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.WikiDetail",
                c => new
                    {
                        WikiDetailId = c.Int(nullable: false, identity: true),
                        WikiId = c.Int(nullable: false),
                        ImagePath = c.String(),
                    })
                .PrimaryKey(t => t.WikiDetailId)
                .ForeignKey("dbo.Wiki", t => t.WikiId, cascadeDelete: true)
                .Index(t => t.WikiId);
            
            CreateTable(
                "dbo.Wiki",
                c => new
                    {
                        WikiId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Content = c.String(),
                        Repository_RepositoryId = c.Int(),
                    })
                .PrimaryKey(t => t.WikiId)
                .ForeignKey("dbo.Repository", t => t.Repository_RepositoryId)
                .Index(t => t.Repository_RepositoryId);
            
            AddColumn("dbo.DetailTeam", "Role", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WikiDetail", "WikiId", "dbo.Wiki");
            DropForeignKey("dbo.Wiki", "Repository_RepositoryId", "dbo.Repository");
            DropForeignKey("dbo.Comment", "UserId", "dbo.User");
            DropForeignKey("dbo.Issue", "UserId", "dbo.User");
            DropForeignKey("dbo.Issue", "RepositoryId", "dbo.Repository");
            DropForeignKey("dbo.Comment", "IssueId", "dbo.Issue");
            DropIndex("dbo.Wiki", new[] { "Repository_RepositoryId" });
            DropIndex("dbo.WikiDetail", new[] { "WikiId" });
            DropIndex("dbo.Issue", new[] { "UserId" });
            DropIndex("dbo.Issue", new[] { "RepositoryId" });
            DropIndex("dbo.Comment", new[] { "UserId" });
            DropIndex("dbo.Comment", new[] { "IssueId" });
            DropColumn("dbo.DetailTeam", "Role");
            DropTable("dbo.Wiki");
            DropTable("dbo.WikiDetail");
            DropTable("dbo.Issue");
            DropTable("dbo.Comment");
        }
    }
}
