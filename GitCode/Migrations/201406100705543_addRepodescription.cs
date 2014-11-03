namespace GitCode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addRepodescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Repository", "ShortDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Repository", "ShortDescription");
        }
    }
}
