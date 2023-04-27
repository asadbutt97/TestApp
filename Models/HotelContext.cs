using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TestApp.Models
{
    public class HotelContext : DbContext
    {
        public HotelContext() : base(GetConnectionString())
        {
        }

        private static string GetConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = "DESKTOP-S2U53M3",
                InitialCatalog = "HotelReservation",
                UserID = "sa",
                Password = "inbox@123"
            };
            return builder.ConnectionString;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}