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
    public class ArtistController : Controller
    {
        // Instance of the database send
        private MyVibesContext db = new MyVibesContext();

        // GET:
        public ActionResult List()
        {
            string query = "Select * from Artists ";
            //Checks to see if the query is being sent
            Debug.WriteLine(query);
            List<Artist> artists = db.Artists.SqlQuery(query).ToList();
            return View(artists);
        }

        // GET ARTIST DETAILS
        public ActionResult Detail(int id)

        {
            Debug.WriteLine(id);

            //Get the sql from the database, although we are getting only one result we need to specify we only one the first one the Sql Query bring back
            Artist Artist = db.Artists.SqlQuery("select * from artists where artistid= @id", new SqlParameter("@id", id)).FirstOrDefault();
            if (Artist == null)
            {
                return HttpNotFound();
            }
            //find data about all songs the artist has
            string aside_query = "select * from Songs inner join ArtistSongs on Songs.SongID = ArtistSongs.Song_SongID where ArtistSongs.Artist_ArtistID=@id";
            Debug.WriteLine(aside_query);

            //List of all the songs
            var fk_parameter = new SqlParameter("@id", id);
            List<Song> songsbyartist = db.Songs.SqlQuery(aside_query, fk_parameter).ToList();

            DetailArtist viewmodel = new DetailArtist();
            viewmodel.artist = Artist;
            viewmodel.songs = songsbyartist;

            return View(viewmodel);

        }

        //ADD AN ARTIST
        //Method is only called when it comes from a form submission
        //Parameters are all the values from the form
        [HttpPost]
        public ActionResult Add(string StagePersona, string RealName, int? Age, int StartYear, string Bio)
        {
            //Since not every artist has an age (i.e groups) this makes sure a value is given regardless
            if (Age == null)
            {
                Age = -1;
            }

            //CHECK IF THE VALUES ARE BEING PASSED INTO THE METHOD
            Debug.WriteLine("The values passed into the method are: " + StagePersona + ", " + RealName + ", " + Age + ", " + StartYear + ", " + Bio);

            //CREATE THE INSERT INTO QUERY
            string query = "insert into artists (RealName, StagePersona, Age, StartYear, Bio) values (@RealName, @StagePersona, @Age, @StartYear, @Bio)";

            //Binding the variables to the parameters
            SqlParameter[] sqlparams = new SqlParameter[5]; //0,1,2,3,4 pieces of information to add
            //each piece of information is a key and value pair
            sqlparams[0] = new SqlParameter("@RealName", RealName);
            sqlparams[1] = new SqlParameter("@StagePersona", StagePersona);
            sqlparams[2] = new SqlParameter("@Age", Age);
            sqlparams[3] = new SqlParameter("@StartYear", StartYear);
            sqlparams[4] = new SqlParameter("@Bio", Bio);

            //RUN THE QUERY WITH THE PARAMETERS 
            db.Database.ExecuteSqlCommand(query, sqlparams);

            return RedirectToAction("List");
        }

        //Method that allows us to run the add page
        //Send the view the infromation it needs
        public ActionResult New()
        {
            //Since it does not need any extra information we just redirect to the view
            return View();
        }

        //UPDATE 
        //Update contorller that pulls information for the page
        public ActionResult Update(int id)
        {
            string query = "select * from artists where artistid = @id";
            var parameter = new SqlParameter("@id", id);
            Artist selectedartist = db.Artists.SqlQuery(query, parameter).FirstOrDefault();

            return View(selectedartist);
        }

        //UPDATE that actually changes the query
        [HttpPost]
        public ActionResult Update(int id, string StagePersona, string RealName, int Age, int StartYear, string Bio, HttpPostedFileBase ArtistPic, string PicExtension, string PicDelete)
        {
            //We assume at the beggining that there is no picture
            int haspic = 0;
            string artistpicextension = "";
            
            //checking to see if some information is there
            //if they did input a picture we process it 
            if (ArtistPic != null)
            {
                Debug.WriteLine("Something identified...");

                //checking to see if the file size is greater than 0 (bytes)
                //If it is it means that the extension is one of a picture
                if (ArtistPic.ContentLength > 0)
                {
                    Debug.WriteLine("Successfully Identified Image");
                    //file extensioncheck taken from https://www.c-sharpcorner.com/article/file-upload-extension-validation-in-asp-net-mvc-and-javascript/
                    var valtypes = new[] { "jpeg", "jpg", "png", "gif" };

                    //Identifies the exension at the end of the picture
                    var extension = Path.GetExtension(ArtistPic.FileName).Substring(1);

                    //if the extension is one of the valid types
                    if (valtypes.Contains(extension))
                    {
                        try
                        {
                            //file name is the id of the image
                            string fn = id + "." + extension;

                            //get a direct file path to ~/Content/Atists/{id}.{extension}
                            string path = Path.Combine(Server.MapPath("~/Content/Artists/"), fn);

                            //save the file
                            ArtistPic.SaveAs(path);
                            //if these are all successful then we can set these fields
                            haspic = 1;
                            artistpicextension = extension;

                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Artist Image was not saved successfully.");
                            Debug.WriteLine("Exception:" + ex);
                        }



                    }
                }
                //if a picture was not sent make sure there isn't one in store already
            } else
            {
                //Check if there is a picture
                if (PicExtension != null)
                {
                    //If there is a picture do they wish to delete it?
                    if(PicDelete == "no")
                    {
                        //If they didn't choose to delete it, we assign these values os its pulls the existing picture 
                        haspic = 1;
                        artistpicextension = PicExtension;
                    } // If they choose to delete it that means we keep the has pic assigned at the beginning at 0 and assume  there is no picture on the update
                    
                }

            }


            Debug.WriteLine("I am trying to edit the follwoing values: " + StagePersona + ", " + RealName + ", " + Age + ", " + StartYear + ", " + Bio);

            string query = "update artists set StagePersona=@StagePersona, RealName=@RealName, Age=@Age, StartYear=@StartYear, Bio=@Bio, HasPic=@haspic, PicExtension=@artistpicextension where ArtistID=@id";
            SqlParameter[] sqlparams = new SqlParameter[8];
            sqlparams[0] = new SqlParameter("@StagePersona", StagePersona);
            sqlparams[1] = new SqlParameter("@RealName", RealName);
            sqlparams[2] = new SqlParameter("@Age", Age);
            sqlparams[3] = new SqlParameter("@StartYear", StartYear);
            sqlparams[4] = new SqlParameter("@Bio", Bio);
            sqlparams[5] = new SqlParameter("@id", id);
            sqlparams[6] = new SqlParameter("@HasPic", haspic);
            sqlparams[7] = new SqlParameter("@artistpicextension", artistpicextension);

            db.Database.ExecuteSqlCommand(query, sqlparams);


            return RedirectToAction("List");
        }

        //DELETE CONFIRM PAGE
        //Sends the view of the delete confirmation with the info of the artist requested
        public ActionResult DeleteConfirm(int id)
        {
            string query = "select * from artists where ArtistID=@id";
            SqlParameter param = new SqlParameter("@id", id);
            Artist selectedartist = db.Artists.SqlQuery(query, param).FirstOrDefault();
            return View(selectedartist);
        }

        //DELETING THE RECORDS FROM THE DATABASE
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string query = "delete from artists where artistid=@id";
            SqlParameter param = new SqlParameter("@id", id);
            db.Database.ExecuteSqlCommand(query, param);


            return RedirectToAction("List");
        }



    }
}