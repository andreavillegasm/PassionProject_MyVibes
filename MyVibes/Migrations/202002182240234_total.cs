namespace MyVibes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class total : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Playlists", "TotalDuration", c => c.Double(nullable: false));
            DropColumn("dbo.Playlists", "PlaylistDuration");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Playlists", "PlaylistDuration", c => c.DateTime(nullable: false));
            DropColumn("dbo.Playlists", "TotalDuration");
        }
    }
}
