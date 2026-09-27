using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace movie_booking.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Login/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.Login log)
        {
            try
            {
                string connectionString =
                    ConfigurationManager.ConnectionStrings["movie"].ConnectionString;

                using (SqlConnection connection =
                       new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd =
                           new SqlCommand("Login_user", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@Email_ID", SqlDbType.NVarChar, 50)
                            .Value = log.Email_ID;

                        cmd.Parameters.Add("@User_password", SqlDbType.NVarChar, 50)
                            .Value = log.User_password;

                        connection.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Store logged-in user's email in Session
                                Session["Email_ID"] =
                                    reader["Email_ID"].ToString();

                                // Redirect to Movie Create page
                                return RedirectToAction("Create", "Movie");
                            }
                            else
                            {
                                ViewBag.Error =
                                    "Email or Password Invalid.";

                                return View(log);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error: " + ex.Message;
                return View(log);
            }
        }

        // Logout
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Create", "Login");
        }
    }
}