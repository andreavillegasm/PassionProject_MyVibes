using System;
using System.Collections.Generic;
using System.Data;
//required for SqlParameter class
using System.Data.SqlClient;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyVibes.Data;
using MyVibes.Models;
using MyVibes.Models.ViewModels;
using System.Diagnostics;
using System.IO;

namespace MyVibes.Controllers
{
    public class SongController : Controller
    {
        // Instance of the database send
        private MyVibesContext db = new MyVibesContext();

        // GET:
        public ActionResult List()
        {
            string query = "Select * from Songs  ";
            //Checks to see if the query is being sent
            Debug.WriteLine(query);
            List<Song> songs = db.Songs.SqlQuery(query).ToList();
            return View(songs);
        }
        //ADD A SONG
        //Method that allows us to run the add page
        //Send the view the infromation it needs
        public ActionResult New()
        {
            //Since it does not need any extra information we just redirect to the view
            return View();
        }

        
        //Method is only called when it comes from a form submission
        //Parameters are all the values from the form
        [HttpPost]
        public ActionResult Add(string SongName, double Duration, string Genre, DateTime ReleaseDate)
        {

            //CHECK IF THE VALUES ARE BEING PASSED INTO THE METHOD
            Debug.WriteLine("The values passed into the method are: " + SongName + ", " + Duration + ", " + Genre + ", " + ReleaseDate);

            //CREATE THE INSERT INTO QUERY
            string query = "insert into songs (SongName, Duration, Genre, ReleaseDate) values (@SongName, @Duration, @Genre, @ReleaseDate)";

            //Binding the variables to the parameters
            SqlParameter[] sqlparams = new SqlParameter[4]; //0,1,2,3 pieces of information to add
            //each piece of information is a key and value pair
            sqlparams[0] = new SqlParameter("@SongName", SongName);
            sqlparams[1] = new SqlParameter("@Duration", Duration);
            sqlparams[2] = new SqlParameter("@Genre", Genre);
            sqlparams[3] = new SqlParameter("@ReleaseDate", ReleaseDate);

            //RUN THE QUERY WITH THE PARAMETERS 
            db.Database.ExecuteSqlCommand(query, sqlparams);

            return RedirectToAction("List");
        }


        //GETTING THE DETAILS OF A SONG
        public ActionResult Detail(int id)
        {
            //Find data about a particular song
            string main_query = "select * from Songs where SongID = @id";

            //Check if the query looks like it is supposed to
            Debug.WriteLine(main_query);
            var pk_parameter = new SqlParameter("@id", id);
            Song song = db.Songs.SqlQuery(main_query, pk_parameter).FirstOrDefault();

            //Find data about all the artists this song has
            string aside_query = "select * from Artists inner join ArtistSongs on Artists.ArtistID = ArtistSongs.Artist_ArtistID where ArtistSongs.Song_SongID=@id";
            Debug.WriteLine(aside_query);

            //List of all the artist credited on this song
            var fk_parameter = new SqlParameter("@id", id);
            List<Artist> ArtistCredited = db.Artists.SqlQuery(aside_query, fk_parameter).ToList();

            //Lis of all the artisits
            string all_artists_query = "select * from Artists";
            Debug.WriteLine(all_artists_query);
            List<Artist> AllArtists = db.Artists.SqlQuery(all_artists_query).ToList();

            //Find data about all the playlists link to this song
            string playlist_aside_query = "select * from Playlists inner join SongPlaylists on Playlists.PlaylistID = SongPlaylists.Playlist_PlaylistID where SongPlaylists.Song_SongID=@id";
            Debug.WriteLine(playlist_aside_query);

            //List of all the playlists linked to this song
            var playlist_fk_parameter = new SqlParameter("@id", id);
            List<Playlist> PlaylistsLinked = db.Playlists.SqlQuery(playlist_aside_query, playlist_fk_parameter).ToList();


            //ViewModel does 4 things
            //(1) showing the classic information about a song
            //(2) showing all the artists in that song
            //(3) showing all the artists to add a new artist to the song
            //(4) show all the playlist this song is on
            DetailSong viewmodel = new DetailSong();
            viewmodel.song = song;
            viewmodel.artists = ArtistCredited;
            viewmodel.all_artists = AllArtists;
            viewmodel.playlits = PlaylistsLinked;

            return View(viewmodel);
        }

        //Gets called from the Detail page to attach an Artist to attach/add an artist to a song
        [HttpPost]
        public ActionResult AttachArtist(int id, int ArtistID)
        {
            Debug.WriteLine("songid id is" + id + " and artistid is " + ArtistID);

            //first, check if the artist is already on the song
            string check_query = "select * from Artists inner join ArtistSongs on Artists.ArtistID = ArtistSongs.Artist_ArtistID where Artist_ArtistID=@ArtistID and Song_SongID=@id";

            Debug.WriteLine(check_query);

            SqlParameter[] check_params = new SqlParameter[2];
            check_params[0] = new SqlParameter("@id", id);
            check_params[1] = new SqlParameter("@ArtistID", ArtistID);
            List<Artist> artists = db.Artists.SqlQuery(check_query, check_params).ToList();
            //only execute add if the artist and song reference do not exist
            if (artists.Count <= 0)
            {


                //first id above is the songid, then the artistid
                //Insert references into the bridging table
                string query = "insert into ArtistSongs (Artist_ArtistID, Song_SongID) values (@ArtistID, @id)";
                SqlParameter[] sqlparams = new SqlParameter[2];
                sqlparams[0] = new SqlParameter("@id", id);
                sqlparams[1] = new SqlParameter("@ArtistID", ArtistID);


                db.Database.ExecuteSqlCommand(query, sqlparams);
            }

            return RedirectToAction("Detail/" + id);

        }

        //Gets the reference of the song id and the artist id to erase that reference and therefore detach the artist from the song
        [HttpGet]
        public ActionResult DetachArtist(int id, int ArtistID)
        {
            //two items are passed through a GET URL
            Debug.WriteLine("song id is" + id + " and artistid is " + ArtistID);

            string query = "delete from ArtistSongs where Artist_ArtistID=@ArtistID and Song_SongID=@id";

            SqlParameter[] sqlparams = new SqlParameter[2];
            sqlparams[0] = new SqlParameter("@ArtistID", ArtistID);
            sqlparams[1] = new SqlParameter("@id", id);

            db.Database.ExecuteSqlCommand(query, sqlparams);

            //Redirects to the detail page
            return RedirectToAction("Detail/" + id);
        }

        //UPDATE 
        //Update contorller that pull information for the page
        public ActionResult Update(int id)
        {
            string query = "select * from songs where songid = @id";
            var parameter = new SqlParameter("@id", id);
            Song selectedsong = db.Songs.SqlQuery(query, parameter).FirstOrDefault();

            return View(selectedsong);
        }

        //UPDATE that actually changes the query
        [HttpPost]
        public ActionResult Update(int id, string SongName, double Duration , string Genre, DateTime ReleaseDate)
        {
           
            Debug.WriteLine("I am trying to edit the follwoing values: " + SongName + ", " + Duration + ", " + Genre + ", " + ReleaseDate);

            string query = "update songs set SongName=@SongName, Duration=@Duration, Genre=@Genre, ReleaseDate=@ReleaseDate where SongID=@id";
            SqlParameter[] sqlparams = new SqlParameter[5];
            sqlparams[0] = new SqlParameter("@SongName", SongName);
            sqlparams[1] = new SqlParameter("@Duration", Duration);
            sqlparams[2] = new SqlParameter("@Genre", Genre);
            sqlparams[3] = new SqlParameter("@ReleaseDate", ReleaseDate);
            sqlparams[4] = new SqlParameter("@id", id);

            db.Database.ExecuteSqlCommand(query, sqlparams);


            return RedirectToAction("List");
        }

        //DELETE CONFIRM PAGE
        //Sends the view of the delete confirmation with the info of the song requested
        public ActionResult DeleteConfirm(int id)
        {
            string query = "select * from songs where SongID=@id";
            SqlParameter param = new SqlParameter("@id", id);
            Song selectedsong = db.Songs.SqlQuery(query, param).FirstOrDefault();
            return View(selectedsong);
        }

        //DELETING THE RECORDS FROM THE DATABASE
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string query = "delete from songs where songid=@id";
            SqlParameter param = new SqlParameter("@id", id);
            db.Database.ExecuteSqlCommand(query, param);


            return RedirectToAction("List");
        }
    }
}