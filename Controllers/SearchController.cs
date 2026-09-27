
using movie_booking.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace  movie_booking.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public ActionResult Index()
        {
            return View();
        }


        // GET: Search/Details
        public ActionResult Details(int? cat_id)
        {
            List<Movie> movlist = new List<Movie>();

            string connectionString =
                ConfigurationManager.ConnectionStrings["movie"].ConnectionString;

            List<SelectListItem> list = new List<SelectListItem>();

            SqlConnection connection =
                new SqlConnection(connectionString);

            connection.Open();


            // =========================================
            // BIND CATEGORY
            // =========================================

            SqlCommand cmd1 =
                new SqlCommand("Bind_Category", connection);

            cmd1.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader =
                cmd1.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new SelectListItem
                {
                    Value = reader["Cat_ID"].ToString(),
                    Text = reader["Cat_Type"].ToString()
                });
            }

            reader.Close();

            ViewBag.CategoryList = list;


            // =========================================
            // BIND MOVIE
            // =========================================

            if (cat_id.HasValue)
            {
                SqlCommand cmd =
                    new SqlCommand("Bind_Movie", connection);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Cat_ID", SqlDbType.Int).Value =
                    cat_id.Value;

                SqlDataReader reader1 =
                    cmd.ExecuteReader();

                while (reader1.Read())
                {
                    Movie mov = new Movie();

                    // Movie ID
                    mov.Movie_ID =
                        Convert.ToInt32(reader1["Movie_ID"]);

                    // Movie Name
                    mov.Movie_name =
                        reader1["Movie_name"].ToString();

                    // Release Date
                    if (reader1["Release_Date"] != DBNull.Value)
                    {
                        mov.Release_Date =
                            Convert.ToDateTime(reader1["Release_Date"]);
                    }

                    // Category ID
                    if (reader1["Cat_ID"] != DBNull.Value)
                    {
                        mov.Cat_ID =
                            Convert.ToInt32(reader1["Cat_ID"]);
                    }

                    // Rate
                    if (reader1["rate"] != DBNull.Value)
                    {
                        mov.rate =
                            Convert.ToInt32(reader1["rate"]);
                    }

                    movlist.Add(mov);
                }

                reader1.Close();

                ViewBag.SelectedCatId = cat_id.Value;
            }

            connection.Close();

            return View(movlist);
        }


        // =========================================
        // CREATE
        // =========================================

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        // =========================================
        // EDIT
        // =========================================

        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        // =========================================
        // DELETE
        // =========================================

        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
