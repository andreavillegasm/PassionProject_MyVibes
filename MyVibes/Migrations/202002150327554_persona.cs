namespace MyVibes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class persona : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Artists", "StageName", c => c.String());
            DropColumn("dbo.Artists", "Persona");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Artists", "Persona", c => c.String());
            DropColumn("dbo.Artists", "StageName");
        }
    }
}
