using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyVibes.Models.ViewModels
{
    public class DetailArtist
    {
        //information about an individual artist
        public virtual Artist artist { get; set; }

        //information about multiple songs associated with that artist
        public List<Song> songs { get; set; }
    }
}