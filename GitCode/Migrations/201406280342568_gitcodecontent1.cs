namespace GitCode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gitcodecontent1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GitCodeBase",
                c => new
                    {
                        GitCodeBaseId = c.Int(nullable: false, identity: true),
                        AboutContent = c.String(),
                        GitPath = c.String(),
                        RepositoryPath = c.String(),
                        CachePath = c.String(),
                        CommitContent = c.String(),
                        ClassContent = c.String(),
                        TeamContent = c.String(),
                        HomeContent = c.String(),
                    })
                .PrimaryKey(t => t.GitCodeBaseId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.GitCodeBase");
        }
    }
}
