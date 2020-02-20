using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyVibes.Models.ViewModels
{
    public class DetailSong
    {
        //information about an individual song
        public virtual Song song { get; set; }

        //information about multiple artists associated with that song
        public List<Artist> artists { get; set; }

        //List of all artists on the database so thay can add an artist to the song if they wish to
        public List<Artist> all_artists { get; set; }

        //All the playlist this song appears on
        public List<Playlist> playlits { get; set; }
    }
}