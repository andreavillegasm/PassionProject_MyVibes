using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//Install  entity framework 6 on Tools > Manage Nuget Packages > Microsoft Entity Framework (ver 6.4)
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace MyVibes.Models
{
    public class Artist
    {
        /*
         An artist is a person or a group that creates a song
            - ArtistID
            - ProfilePic
            - RealName
            - StageName
            - Age
            - ActiveDate (since when are they active as musicians)
            - Bio: An overview of their profile

        An artist references a list of songs
    */
        [Key]
        public int ArtistID { get; set; }
        public string RealName { get; set; }
        public string StagePersona { get; set; }

        public int Age { get; set; }

        //Star Year is an int in this case because we only need the year when they debuted
        public int StartYear { get; set; }
        public string Bio { get; set; }
        

        //Image available and extension for profile 
        public int HasPic { get; set; }
        public string PicExtension { get; set; }



        //Representing the Many to Many relationship with songs      
        public ICollection<Song> Songs { get; set; }

    }
}