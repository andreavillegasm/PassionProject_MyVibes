namespace MyVibes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editsong : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Songs", "Duration", c => c.Double(nullable: false));
            DropColumn("dbo.Songs", "SongDuration");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Songs", "SongDuration", c => c.DateTime(nullable: false));
            DropColumn("dbo.Songs", "Duration");
        }
    }
}
