namespace GitCode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newAtributeindetailclassaccess : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DetailClassAccess", "UserAccess", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DetailClassAccess", "UserAccess");
        }
    }
}
