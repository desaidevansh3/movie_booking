using movie_booking.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace movie_booking.Models
{
    public class Movies_Handler
    {
        public SqlConnection con;
        public SqlConnection Connection()
        {
            String con = ConfigurationManager.ConnectionStrings["movie"].ConnectionString;
            return new SqlConnection(con);
        }

        public bool Add_Movie_Category(Movie_Category mc)
        {
            con = Connection();
            SqlCommand cmd = new SqlCommand("Add_Movie_Category", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Cat_Type", mc.Cat_Type);
            con.Open();

            int i = cmd.ExecuteNonQuery();
            if (i > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Add_Movie(Movie m)
        {
            con = Connection();
            SqlCommand cmd = new SqlCommand("Add_Movie", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Movie_name", m.Movie_name);
            cmd.Parameters.AddWithValue("@Release_date", m.Release_Date);
            cmd.Parameters.AddWithValue("@cat_Id", m.Cat_ID);
            cmd.Parameters.AddWithValue("@rate", m.rate);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            if (i > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
       

        public List<Movie_Category> displaymoviecategory()
        {
            List<Movie_Category> movie_list = new List<Movie_Category>();
            using (SqlConnection con = Connection())
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Tbl_Movie_Category", con))
                {
                    con.Open();
                    SqlDataAdapter sd = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sd.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        movie_list.Add(new Movie_Category
                        {
                            Cat_ID = Convert.ToInt32(dr["Cat_ID"]),

                            Cat_Type = Convert.ToString(dr["Cat_Type"])
                        });
                    }
                }
            }
            return movie_list;
        }

        public List<Movie> displaymovie()
        {
            List<Movie> movie_list = new List<Movie>();
            using (SqlConnection con = Connection())
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Tbl_Movie", con))
                {
                    con.Open();
                    SqlDataAdapter sd = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sd.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        movie_list.Add(new Movie
                        {
                            Movie_ID = Convert.ToInt32(dr["Movie_ID"]),
                            Movie_name = Convert.ToString(dr["Movie_name"]),
                            Release_Date = System.Convert.ToDateTime(dr["Release_Date"]),
                            Cat_ID = Convert.ToInt32(dr["Cat_ID"]),
                            rate = Convert.ToInt32(dr["rate"]),

                        });
                    }
                }
            }
            return movie_list;
        }
        public bool Add_User(User u)
        {
            con = Connection();
            SqlCommand cmd = new SqlCommand("Add_User", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@User_Name", u.User_Name);
            cmd.Parameters.AddWithValue("@Email_ID", u.Email_ID);
            cmd.Parameters.AddWithValue("@User_password", u.User_password);
            cmd.Parameters.AddWithValue("@City", u.City);
            cmd.Parameters.AddWithValue("@PhoneNo", u.PhoneNo);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            if(i>0)
            {
                return true;
            }
            else 
            {
                return false; 
            }

        }
    }
}





 












