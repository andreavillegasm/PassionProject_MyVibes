namespace MyVibes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class year : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Artists", "StartYear", c => c.Int(nullable: false));
            DropColumn("dbo.Artists", "ActiveDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Artists", "ActiveDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Artists", "StartYear");
        }
    }
}
