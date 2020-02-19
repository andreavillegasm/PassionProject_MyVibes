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
    public class PlaylistController : Controller
    {
        // Instance of the database send
        private MyVibesContext db = new MyVibesContext();

        // GET: Playlist
        public ActionResult List()
        {
            string query = "Select * from Playlists  ";
            //Checks to see if the query is being sent
            Debug.WriteLine(query);
            List<Playlist> playlists = db.Playlists.SqlQuery(query).ToList();
            return View(playlists);
        }

        //ADD A PLAYLIST
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
        public ActionResult Add(string PlaylistName)
        {

            //CHECK IF THE VALUES ARE BEING PASSED INTO THE METHOD
            Debug.WriteLine("The values passed into the method are: " + PlaylistName );

            //CREATE THE INSERT INTO QUERY
            string query = "insert into playlists (PlaylistName) values (@PlaylistName)";

            //Binding the variables to the parameters
            SqlParameter[] sqlparams = new SqlParameter[1]; //0 pieces of information to add
            //each piece of information is a key and value pair
            sqlparams[0] = new SqlParameter("@PlaylistName", PlaylistName);

            //RUN THE QUERY WITH THE PARAMETERS 
            db.Database.ExecuteSqlCommand(query, sqlparams);

            return RedirectToAction("List");
        }

        //GETTING THE DETAILS OF A PLAYLIST
        public ActionResult Detail(int id)
        {
            //find data about a particular playlist
            string main_query = "select * from playlists where PlaylistID = @id";

            //Check if the query looks like it is supposed to
            Debug.WriteLine(main_query);
            var pk_parameter = new SqlParameter("@id", id);
            Playlist playlistinfo = db.Playlists.SqlQuery(main_query, pk_parameter).FirstOrDefault();

            //find data about all songs on this playlist
            string aside_query = "select * from Songs inner join SongPlaylists on Songs.SongID = SongPlaylists.Song_SongID where SongPlaylists.Playlist_PlaylistID=@id";
            Debug.WriteLine(aside_query);

            //List all the songs in this playlist
            var fk_parameter = new SqlParameter("@id", id);
            List<Song> songs = db.Songs.SqlQuery(aside_query, fk_parameter).ToList();

            //Lis of all the songs available
            string all_songs_query = "select * from Songs";
            Debug.WriteLine(all_songs_query);
            List<Song> allsongs = db.Songs.SqlQuery(all_songs_query).ToList();

            //ViewModel does 3 things
            //(1) showing the classic information about a song
            //(2) showing all the artists in that song
            //(3) showing all the artists to add a new artist to the song
            DetailPlaylist viewmodel = new DetailPlaylist();
            viewmodel.playlist = playlistinfo;
            viewmodel.songs = songs;
            viewmodel.all_songs = allsongs;

            return View(viewmodel);
        }

        //Gets called from the Detail page to attach a song to a playlist
        [HttpPost]
        public ActionResult AttachSong(int id, int SongID)
        {
            Debug.WriteLine("playlistid is" + id + " and songid is " + SongID);

            //first, check if the song is already on the playlist
            string check_query = "select * from Songs inner join SongPlaylists on Songs.SongID = SongPlaylists.Song_SongID where Song_SongID=@SongID and Playlist_PlaylistID=@id";

            Debug.WriteLine(check_query);

            SqlParameter[] check_params = new SqlParameter[2];
            check_params[0] = new SqlParameter("@id", id);
            check_params[1] = new SqlParameter("@SongID", SongID);
            List<Song> songs = db.Songs.SqlQuery(check_query, check_params).ToList();

            //only execute add if the song and playlist reference do not exist
            if (songs.Count <= 0)
            {


                //first id above is the playlistid, then the songid
                //Insert references into the bridging table
                string query = "insert into SongPlaylists (Song_SongID, Playlist_PlaylistID) values (@SongID, @id)";
                SqlParameter[] sqlparams = new SqlParameter[2];
                sqlparams[0] = new SqlParameter("@id", id);
                sqlparams[1] = new SqlParameter("@SongID", SongID);


                db.Database.ExecuteSqlCommand(query, sqlparams);
            }

            return RedirectToAction("Detail/" + id);

        }

        // Detach the song
        [HttpGet]
        public ActionResult DetachSong(int id, int SongID)
        {
            //two items are passed through a GET URL
            Debug.WriteLine("playlist id is" + id + " and songID is " + SongID);

            string query = "delete from SongPlaylists where Song_SongID=@SongID and Playlist_PlaylistID=@id";

            SqlParameter[] sqlparams = new SqlParameter[2];
            sqlparams[0] = new SqlParameter("@SongID", SongID);
            sqlparams[1] = new SqlParameter("@id", id);

            db.Database.ExecuteSqlCommand(query, sqlparams);

            return RedirectToAction("Detail/" + id);
        }

        //UPDATE 
        //Update controller that pull information for the page
        public ActionResult Update(int id)
        {
            string query = "select * from playlists where playlistid = @id";
            var parameter = new SqlParameter("@id", id);
            Playlist selectedplaylist = db.Playlists.SqlQuery(query, parameter).FirstOrDefault();

            return View(selectedplaylist);
        }

        //UPDATE that actually changes the query
        [HttpPost]
        public ActionResult Update(int id, string PlaylistName)
        {

            Debug.WriteLine("I am trying to edit the follwoing values: " + PlaylistName );

            string query = "update playlists set PlaylistName=@PlaylistName where PlaylistID=@id";
            SqlParameter[] sqlparams = new SqlParameter[2];
            sqlparams[0] = new SqlParameter("@PlaylistName", PlaylistName);
            sqlparams[1] = new SqlParameter("@id", id);

            db.Database.ExecuteSqlCommand(query, sqlparams);


            return RedirectToAction("List");
        }

        //DELETE CONFIRM PAGE
        //Sends the view of the delete confirmation with the info of the song requested
        public ActionResult DeleteConfirm(int id)
        {
            string query = "select * from playlists where PlaylistID=@id";
            SqlParameter param = new SqlParameter("@id", id);
            Playlist selectedplaylist = db.Playlists.SqlQuery(query, param).FirstOrDefault();
            return View(selectedplaylist);
        }

        //DELETING THE RECORDS FROM THE DATABASE
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string query = "delete from playlists where playlistid=@id";
            SqlParameter param = new SqlParameter("@id", id);
            db.Database.ExecuteSqlCommand(query, param);


            return RedirectToAction("List");
        }
    }
}