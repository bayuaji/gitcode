namespace GitCode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newChangeDB : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DetailRepository", "IsOwner", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DetailRepository", "IsOwner", c => c.String());
        }
    }
}
