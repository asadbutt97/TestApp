using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestApp.Models
{
 

    public class Room
    {
        public int Id { get; set; }
        public int RoomNumber { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }

    

}