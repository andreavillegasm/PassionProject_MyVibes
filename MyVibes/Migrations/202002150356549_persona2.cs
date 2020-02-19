namespace MyVibes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class persona2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Artists", "StagePersona", c => c.String());
            DropColumn("dbo.Artists", "StageName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Artists", "StageName", c => c.String());
            DropColumn("dbo.Artists", "StagePersona");
        }
    }
}
