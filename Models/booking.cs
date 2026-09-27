using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace movie_booking.Models
{
    public class booking
    {
        public int booking_id { get; set; }
        public int User_ID { get; set; }
        public int Cat_ID { get; set; }
        public string Cat_Type { get; set; }
        public string Movie_name {  get; set; }
        public int Movie_ID { get; set; }
        public int no_tickets { get; set; }
        public int amount { get; set; }

    }
}


