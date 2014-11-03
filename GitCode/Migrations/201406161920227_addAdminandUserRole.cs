namespace GitCode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAdminandUserRole : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "Role", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "Role");
        }
    }
}
