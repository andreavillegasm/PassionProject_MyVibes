using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyVibes.Models.ViewModels
{
    public class DetailPlaylist
    {
        //Information about a particular playlist
        public virtual Playlist playlist { get; set; }

        //All the songs that are part of that playlist
        public List<Song> songs { get; set; }

        //All of the songs in the system
        public List<Song> all_songs { get; set; }


    }
}