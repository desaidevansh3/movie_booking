using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace movie_booking.Models
{
    public class User
    {   
        public int User_ID {  get; set; }
        public string User_Name { get; set;}
        public string Email_ID { get; set;}
        public string User_password { get; set;}
        public string City {  get; set;}
        public int PhoneNo { get; set;}
    }
}