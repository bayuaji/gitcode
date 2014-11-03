namespace GitCode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class repositoryChanging : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Repository", "IsPublicRead", c => c.Boolean(nullable: false));
            AddColumn("dbo.Repository", "IsPublicWrite", c => c.Boolean(nullable: false));
            AddColumn("dbo.Repository", "User", c => c.String());
            AddColumn("dbo.Repository", "Project", c => c.String());
            AlterColumn("dbo.Repository", "IsPublic", c => c.Boolean(nullable: false));
            DropColumn("dbo.Repository", "RepoHeadDirectory");
            DropColumn("dbo.Repository", "RepoChildDirectory");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Repository", "RepoChildDirectory", c => c.String());
            AddColumn("dbo.Repository", "RepoHeadDirectory", c => c.String());
            AlterColumn("dbo.Repository", "IsPublic", c => c.String());
            DropColumn("dbo.Repository", "Project");
            DropColumn("dbo.Repository", "User");
            DropColumn("dbo.Repository", "IsPublicWrite");
            DropColumn("dbo.Repository", "IsPublicRead");
        }
    }
}
