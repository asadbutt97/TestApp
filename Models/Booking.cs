using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TestApp.Controllers;

namespace TestApp.Models
{
    public class Booking
    {

        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [UniqueBooking]
        public DateTime CheckOutDate { get; set; }

        public virtual User User { get; set; }
        public virtual Room Room { get; set; }
    }

}