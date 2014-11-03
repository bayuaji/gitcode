namespace GitCode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeIssueModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Issue", "When", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Issue", "IsClear", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Issue", "IsClear", c => c.String());
            AlterColumn("dbo.Issue", "When", c => c.String());
        }
    }
}
