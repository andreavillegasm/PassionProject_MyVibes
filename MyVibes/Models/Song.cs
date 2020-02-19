using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//Install  entity framework 6 on Tools > Manage Nuget Packages > Microsoft Entity Framework (ver 6.4)
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyVibes.Models
{
    public class Song
    {
        /*
         An song is a source of music created by an Artist(s) and placed on a Playlist(s)
            - SongID
            - SongName
            - SongDuration
            - Genre
            - ReleaseDate

        An Song References a list of artists and a list of playlits
    */
        [Key]
        public int SongID { get; set; }
        public string SongName { get; set; }
        public double Duration { get; set; }
        public string Genre { get; set; }
        public DateTime ReleaseDate { get; set; }



        //Representing the Many to Many relationship with artists     
        public ICollection<Artist> Artists { get; set; }

        //Representing the Many to Many relationship with playlists     
        public ICollection<Playlist> Playlists { get; set; }



    }
}