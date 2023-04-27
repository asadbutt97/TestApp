using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TestApp.Models;


namespace TestApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly HotelContext db = new HotelContext();
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = db.Users.SingleOrDefault(u => u.Username == model.Username && u.Password == model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }

            FormsAuthentication.SetAuthCookie(user.Id.ToString(), false);
            // create a new FormsAuthenticationTicket
            var ticket = new FormsAuthenticationTicket(
                version: 1,
                name: user.Username,
                issueDate: DateTime.Now,
                expiration: DateTime.Now.AddMinutes(30),
                isPersistent: false,
                userData: user.Id.ToString(),
                cookiePath: FormsAuthentication.FormsCookiePath);

            // encrypt the ticket
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            // create a new authentication cookie
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
                Path = FormsAuthentication.FormsCookiePath,
                Domain = FormsAuthentication.CookieDomain
            };

            // add the cookie to the response
            Response.Cookies.Add(cookie);

            return RedirectToAction("Index", "Bookings");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Account");
        }

    }
}