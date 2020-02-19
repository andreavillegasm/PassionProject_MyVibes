namespace MyVibes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Artist : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Playlists",
                c => new
                    {
                        PlaylistID = c.Int(nullable: false, identity: true),
                        PlaylistName = c.String(),
                        PlaylistDuration = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PlaylistID);
            
            CreateTable(
                "dbo.Songs",
                c => new
                    {
                        SongID = c.Int(nullable: false, identity: true),
                        SongName = c.String(),
                        SongDuration = c.DateTime(nullable: false),
                        Genre = c.String(),
                        ReleaseDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SongID);
            
            CreateTable(
                "dbo.Artists",
                c => new
                    {
                        ArtistID = c.Int(nullable: false, identity: true),
                        RealName = c.String(),
                        Persona = c.String(),
                        ActiveDate = c.DateTime(nullable: false),
                        Bio = c.String(),
                        HasPic = c.Int(nullable: false),
                        PicExtension = c.String(),
                    })
                .PrimaryKey(t => t.ArtistID);
            
            CreateTable(
                "dbo.ArtistSongs",
                c => new
                    {
                        Artist_ArtistID = c.Int(nullable: false),
                        Song_SongID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Artist_ArtistID, t.Song_SongID })
                .ForeignKey("dbo.Artists", t => t.Artist_ArtistID, cascadeDelete: true)
                .ForeignKey("dbo.Songs", t => t.Song_SongID, cascadeDelete: true)
                .Index(t => t.Artist_ArtistID)
                .Index(t => t.Song_SongID);
            
            CreateTable(
                "dbo.SongPlaylists",
                c => new
                    {
                        Song_SongID = c.Int(nullable: false),
                        Playlist_PlaylistID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Song_SongID, t.Playlist_PlaylistID })
                .ForeignKey("dbo.Songs", t => t.Song_SongID, cascadeDelete: true)
                .ForeignKey("dbo.Playlists", t => t.Playlist_PlaylistID, cascadeDelete: true)
                .Index(t => t.Song_SongID)
                .Index(t => t.Playlist_PlaylistID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SongPlaylists", "Playlist_PlaylistID", "dbo.Playlists");
            DropForeignKey("dbo.SongPlaylists", "Song_SongID", "dbo.Songs");
            DropForeignKey("dbo.ArtistSongs", "Song_SongID", "dbo.Songs");
            DropForeignKey("dbo.ArtistSongs", "Artist_ArtistID", "dbo.Artists");
            DropIndex("dbo.SongPlaylists", new[] { "Playlist_PlaylistID" });
            DropIndex("dbo.SongPlaylists", new[] { "Song_SongID" });
            DropIndex("dbo.ArtistSongs", new[] { "Song_SongID" });
            DropIndex("dbo.ArtistSongs", new[] { "Artist_ArtistID" });
            DropTable("dbo.SongPlaylists");
            DropTable("dbo.ArtistSongs");
            DropTable("dbo.Artists");
            DropTable("dbo.Songs");
            DropTable("dbo.Playlists");
        }
    }
}
