namespace MyVibes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class age : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Artists", "Age", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Artists", "Age");
        }
    }
}
