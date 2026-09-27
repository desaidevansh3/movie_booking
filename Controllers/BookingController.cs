using movie_booking.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mmovie_booking.Controllers
{
    public class BookingController : Controller
    {
        public List<SelectListItem> Bind_Movie(int cat_id)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["movie"].ToString();

            List<SelectListItem> list = new List<SelectListItem>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Bind_Movie", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Cat_ID", cat_id);

                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new SelectListItem
                            {
                                Value = reader["Movie_ID"].ToString(),
                                Text = reader["Movie_name"].ToString()
                                       + " | Price : "
                                       + reader["Rate"].ToString()
                            });
                        }
                    }
                }
            }

            ViewBag.MovieList = list;

            return list;
        }

        public int Calculate_Price(int movie_id, int no_of_tickets)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["movie"].ToString();

            int price = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Get_Rate", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Movie_id", movie_id);

                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            price = Convert.ToInt32(reader["Rate"]);
                        }
                    }
                }
            }

            int total_price = price * no_of_tickets;

            ViewBag.TotalPrice = total_price;

            return total_price;
        }
        public int Get_User_id()
        {
            if (Session["Email_ID"] == null)
            {
                return 0;
            }

            string email = Session["Email_ID"].ToString();

            string connectionString =
                ConfigurationManager.ConnectionStrings["movie"].ConnectionString;

            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                       new SqlCommand("Get_User", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Email_ID", SqlDbType.NVarChar, 50)
                        .Value = email;

                    connection.Open();

                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }

            return 0;
        }


        // GET: Booking
        public ActionResult Index()
        {
            return View();
        }

        // GET: Booking/Details/5
        public ActionResult Details()
        {

            string connectionString = ConfigurationManager.ConnectionStrings["movie"].ToString();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("Get_Booking", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            List<booking> booklist = new List<booking>();
            connection.Open();
            cmd.Parameters.AddWithValue("@user_id", Get_User_id());
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                booking booking = new booking();
                booking.booking_id = Convert.ToInt32(reader["Booking_id"]);
                booking.Cat_Type = reader["Cat_type"].ToString();
                booking.Movie_name = reader["Movie_name"].ToString();
                booking.no_tickets = Convert.ToInt32(reader["no_Tickets"]);
                booking.amount = Convert.ToInt32(reader["amount"]);
                booklist.Add(booking);
            }
            reader.Close();
            connection.Close();
            return View(booklist);
        }

        // GET: Booking/Create
        public ActionResult Create(int? Cat_ID, int? Movie_ID, int? no_tickets)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["movie"].ToString();

            List<SelectListItem> categoryList = new List<SelectListItem>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Bind_Category", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categoryList.Add(new SelectListItem
                            {
                                Value = reader["Cat_ID"].ToString(),
                                Text = reader["Cat_Type"].ToString()
                            });
                        }
                    }
                }
            }

            ViewBag.CategoryList = categoryList;

            ViewBag.MovieList = new List<SelectListItem>();

            booking book = new booking();

            // Category selected
            if (Cat_ID.HasValue)
            {
                book.Cat_ID = Cat_ID.Value;

                ViewBag.MovieList = Bind_Movie(Cat_ID.Value);
            }

            // Movie selected
            if (Movie_ID.HasValue)
            {
                book.Movie_ID = Movie_ID.Value;
            }

            // Tickets selected
            if (no_tickets.HasValue)
            {
                book.no_tickets = no_tickets.Value;
            }

            // Calculate amount
            if (Movie_ID.HasValue && no_tickets.HasValue)
            {
                book.amount = Calculate_Price(
                    Movie_ID.Value,
                    no_tickets.Value
                );

                ViewBag.TotalPrice = book.amount;
            }
            else
            {
                ViewBag.TotalPrice = 0;
            }

            return View(book);
        }

        // POST: Booking/Create
        [HttpPost]
        public ActionResult Create(booking book)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Error = "Please enter all booking details.";
                    return View(book);
                }

                int User_ID = Get_User_id();

                if (User_ID == 0)
                {
                    ViewBag.Error =
                        "User account not found. Please login again.";

                    return View(book);
                }

                string connectionString =
                    ConfigurationManager.ConnectionStrings["movie"].ConnectionString;

                using (SqlConnection connection =
                       new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd =
                           new SqlCommand("GetBooking", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@User_ID", SqlDbType.Int)
                            .Value = User_ID;

                        cmd.Parameters.Add("@Cat_ID", SqlDbType.Int)
                            .Value = book.Cat_ID;

                        cmd.Parameters.Add("@Movie_ID", SqlDbType.Int)
                            .Value = book.Movie_ID;

                        cmd.Parameters.Add("@no_tickets", SqlDbType.Int)
                            .Value = book.no_tickets;

                        cmd.Parameters.Add("@amount", SqlDbType.Int)
                            .Value = book.amount;

                        connection.Open();

                        cmd.ExecuteNonQuery();
                    }
                }

                ViewBag.Message = "Booking Insert Successfully";

                return View(book);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Booking failed: " + ex.Message;
                return View(book);
            }
        }
        //public ActionResult Create(booking book)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {

        //            int User_ID = Get_User_id();

        //                    if (User_ID == 0)
        //            {
        //                ViewBag.Error = "User not found. Please login again.";
        //                return View(book);
        //            }

        //            string connectionString =
        //                ConfigurationManager.ConnectionStrings["movie"].ToString();

        //            using (SqlConnection connection = new SqlConnection(connectionString))
        //            {
        //                using (SqlCommand cmd = new SqlCommand("GetBooking", connection))
        //                {
        //                    cmd.CommandType = CommandType.StoredProcedure;

        //                    cmd.Parameters.AddWithValue("@User_ID", User_ID);
        //                    cmd.Parameters.AddWithValue("@Cat_ID", book.Cat_ID);
        //                    cmd.Parameters.AddWithValue("@Movie_ID", book.Movie_ID);
        //                    cmd.Parameters.AddWithValue("@no_tickets", book.no_tickets);
        //                    cmd.Parameters.AddWithValue("@amount", book.amount);

        //                    connection.Open();

        //                    int i = cmd.ExecuteNonQuery();

        //                    if (i > 0)
        //                    {
        //                        ViewBag.Message = "Booking Insert Successfully";
        //                    }
        //                    else
        //                    {
        //                        ViewBag.Error = "Booking insertion failed.";
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            ViewBag.Error = "Please enter all booking details.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Error = ex.Message;
        //    }

        //    return View(book);
        //}

        // GET: Booking/Edit/5
        public ActionResult Edit(int id, int? Cat_ID, int? Movie_ID, int? no_tickets)
        {
            booking book = new booking();
            string connectionString = ConfigurationManager.ConnectionStrings["movie"].ToString();

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("Get_Booking_ById", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Booking_id", id);

            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                book.booking_id = Convert.ToInt32(reader["Booking_id"]);
                book.User_ID = Convert.ToInt32(reader["User_id"]);
                book.Cat_ID = Convert.ToInt32(reader["Cat_id"]);
                book.Movie_ID = Convert.ToInt32(reader["Movie_id"]);
                book.no_tickets = Convert.ToInt32(reader["No_of_Tickets"]);
                book.amount = Convert.ToInt32(reader["Amount"]);
            }

            reader.Close();
            connection.Close();

            List<SelectListItem> list = new List<SelectListItem>();

            connection = new SqlConnection(connectionString);
            cmd = new SqlCommand("Bind_Category", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new SelectListItem
                {
                    Value = reader["Cat_id"].ToString(),
                    Text = reader["Cat_Type"].ToString()
                });
            }

            reader.Close();
            connection.Close();

            ViewBag.CategoryList = list;

            if (Cat_ID != null)
            {
                book.Cat_ID = Cat_ID.Value;
            }

            if (Movie_ID != null)
            {
                book.Movie_ID = Movie_ID.Value;
            }

            if (no_tickets != null)
            {
                book.no_tickets = no_tickets.Value;
            }

            ViewBag.MovieList = new List<SelectListItem>();

            if (book.Cat_ID != 0)
            {
                ViewBag.MovieList = Bind_Movie(book.Cat_ID);
            }

            if (Movie_ID != null && no_tickets != null)
            {
                book.amount = Calculate_Price(Movie_ID.Value, no_tickets.Value);
                ViewBag.TotalPrice = book.amount;
            }
            else
            {
                ViewBag.TotalPrice = book.amount;
            }

            return View(book);
        }

        // POST: Booking/Edit/5
        [HttpPost]
        public ActionResult Edit(booking book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["movie"].ToString();
                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand cmd = new SqlCommand("Update_Booking", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    cmd.Parameters.AddWithValue("@Booking_id", book.booking_id);
                    cmd.Parameters.AddWithValue("@User_id", Get_User_id());
                    cmd.Parameters.AddWithValue("@Cat_id", book.Cat_ID);
                    cmd.Parameters.AddWithValue("@Movie_id", book.Movie_ID);
                    cmd.Parameters.AddWithValue("@no_of_Tickets", book.no_tickets);
                    cmd.Parameters.AddWithValue("@amount", book.amount);
                    int i = cmd.ExecuteNonQuery();
                    connection.Close();
                    if (i > 0)
                    {
                        ViewBag.Message = "Booking Update Successfully";
                        return View(book);
                    }
                }

                return View(book);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex + " Updation Failed";
                return View(book);
            }
        }

        // GET: Booking/Delete/5
        public ActionResult Delete(int id)
        {
            booking booking = new booking();
            string connectionString = ConfigurationManager.ConnectionStrings["movie"].ToString();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("Get_Booking_ById", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            connection.Open();
            cmd.Parameters.AddWithValue("@Booking_id", id);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {

                booking.booking_id = Convert.ToInt32(reader["Booking_id"]);
                booking.User_ID = Convert.ToInt32(reader["User_id"]);
                booking.Cat_Type = reader["Cat_type"].ToString();
                booking.Movie_name = reader["Movie_name"].ToString();
                booking.no_tickets = Convert.ToInt32(reader["No_of_Tickets"]);
                booking.amount = Convert.ToInt32(reader["amount"]);

            }
            reader.Close();
            connection.Close();
            return View(booking);
        }

        // POST: Booking/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, booking book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["movie"].ToString();
                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand cmd = new SqlCommand("Delete_Booking", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    cmd.Parameters.AddWithValue("@Booking_id", id);

                    int i = cmd.ExecuteNonQuery();
                    connection.Close();
                    if (i > 0)
                    {
                        ViewBag.Message = "Delete Sucessfully";
                        return View(book);
                    }
                }

                return View(book);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex + " Delete Failed";
                return View(book);
            }
        }
    }
}