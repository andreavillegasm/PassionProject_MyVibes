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
    public class Playlist
    {
        /*
         A playlist is a collection of songs
            - PlaylistID
            - PlaylistName
            - PlaylistDuration


        A playlist references a list of songs
    */
        [Key]
        public int PlaylistID { get; set; }
        public string PlaylistName { get; set; }

        public double TotalDuration { get; set; }
        

        //Representing the Many to Many relationship with songs     
        public ICollection<Song> Songs { get; set; }




    }
}