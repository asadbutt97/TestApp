using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TestApp.Models;
using static System.Data.Entity.Infrastructure.Design.Executor;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Security;

namespace TestApp.Controllers
{
    public class BookingsController : Controller
    {
        private HotelContext db = new HotelContext();

        // GET: Bookings
        public ActionResult Index()
        {
            try
            {

                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value);
                int userId = int.Parse(authTicket.UserData);
                if (userId > 0)
                {
                    var user = db.Users.Find(userId);
                    if (user.IsAdmin==true)
                    {
                        return View(db.Bookings.ToList());
                    }
                    else
                    {
                        var bookings = db.Bookings.Include(b => b.Room).Where(b => b.UserId == userId);

                        return View(bookings.ToList());
                    }
                  
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception)
            {

                return RedirectToAction("Login", "Account");
            }

        }

        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // GET: Bookings/Create
        public ActionResult Create()
        {

            try
            {

                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value);
                int userId = int.Parse(authTicket.UserData);
                if (userId > 0)
                {
                    var user = db.Users.Find(userId);
                    if (user.IsAdmin == true)
                    {
                        ViewBag.UserId = new SelectList(db.Users, "Id", "Username");
                       
                    }
                    else
                    {
                        ViewBag.UserId = new SelectList(db.Users, "Id", "Username", userId);
                       
                    }

                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception)
            {

                return RedirectToAction("Login", "Account");
            }

            ViewBag.RoomId = new SelectList(db.Rooms, "Id", "Description");
           
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,RoomId,CheckInDate,CheckOutDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                // Check if the room is available for the requested dates
                var isRoomAvailable = !db.Bookings.Any(b => b.RoomId == booking.RoomId &&
                                                            ((b.CheckInDate <= booking.CheckInDate && b.CheckOutDate > booking.CheckInDate) ||
                                                             (b.CheckInDate < booking.CheckOutDate && b.CheckOutDate >= booking.CheckOutDate)));

                if (isRoomAvailable)
                {
                    db.Bookings.Add(booking);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "The selected room is not available for the requested dates.");
                }
            }

            ViewBag.RoomId = new SelectList(db.Rooms, "Id", "Description", booking.RoomId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Username", booking.UserId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoomId = new SelectList(db.Rooms, "Id", "Description", booking.RoomId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Username", booking.UserId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,RoomId,CheckInDate,CheckOutDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                // Check if the room is already booked for the given dates
                var conflictingBooking = db.Bookings.FirstOrDefault(b => b.RoomId == booking.RoomId &&
                                                                          b.CheckInDate <= booking.CheckOutDate &&
                                                                          b.CheckOutDate >= booking.CheckInDate &&
                                                                          b.Id != booking.Id);

                if (conflictingBooking == null)
                {
                    db.Entry(booking).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "This room is already booked for the selected dates.");
                }
            }

            ViewBag.RoomId = new SelectList(db.Rooms, "Id", "Description", booking.RoomId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Username", booking.UserId);
            return View(booking);
        }
     

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }

            // Check if the check-in date has already passed
            if (booking.CheckInDate <= DateTime.Today)
            {
                // Booking has already checked in, don't allow deletion
                ModelState.AddModelError(string.Empty, "This booking has already checked in and cannot be deleted.");
                return View(booking);
            }

            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
    public class UniqueBookingAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var booking = (Booking)validationContext.ObjectInstance;
            var db = new HotelContext();

            // Check if the room is already booked for the specified time period
            var conflicts = db.Bookings
                .Where(b => b.RoomId == booking.RoomId && b.Id != booking.Id)
                .Where(b => b.CheckInDate < booking.CheckOutDate && b.CheckOutDate > booking.CheckInDate)
                .ToList();

            if (conflicts.Any())
            {
                return new ValidationResult("The selected room is already booked for the specified time period.");
            }

            return ValidationResult.Success;
        }
    }
}
