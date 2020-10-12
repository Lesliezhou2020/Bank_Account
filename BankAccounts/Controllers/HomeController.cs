using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BankAccounts.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace BankAccounts.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
        
        public HomeController(MyContext context)
        {
            _context = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index");
        }
        
        [HttpPost("")]
        public IActionResult Register(User FromForm)
        {
            if(ModelState.IsValid)
            {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                FromForm.Password = Hasher.HashPassword(FromForm, FromForm.Password);
                _context.Add(FromForm);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View("Index");
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser FromForm)
        {
            if(ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == FromForm.Email);
                // If no user exists with provided email
                if(userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return RedirectToAction("Login");
                }
                
                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();
                
                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(FromForm, userInDb.Password, FromForm.Password);
                
                // result can be compared to 0 for failure
                if(result == 0)
                {
                    // handle failure (this should be similar to how "existing email" is handled)
                    return RedirectToAction("Login");
                }
                HttpContext.Session.SetInt32("active_user", userInDb.UserId);
                return RedirectToAction("Details", new {accountId = userInDb.UserId});
            }
            return RedirectToAction("Login");
        }
        
        [HttpGet("account/{accountId}")]
        public IActionResult Details(int accountId)
        {
            int? userId = HttpContext.Session.GetInt32("active_user");
            if (userId.HasValue && userId.Value == accountId) {
                var user = _context.Users
                    .Include(u => u.Transactions)
                    .FirstOrDefault(u => u.UserId == userId);

                ViewBag.Username = $"{user.FirstName} {user.LastName}";
                ViewBag.Balance = user.Transactions.Sum(t => t.Amount);
                ViewBag.Transactions = user.Transactions.OrderByDescending(t => t.CreatedAt).ToList();

                return View("Details");
            }
            return RedirectToAction("Login");
        }

        [HttpPost("new_transaction")]
        public IActionResult CreateTransaction(Transaction FromForm)
        {
            int? userId = HttpContext.Session.GetInt32("active_user");
            var balance = _context.Users
                .Include(u => u.Transactions)
                .FirstOrDefault(u => u.UserId == userId.Value)
                .Transactions.Sum(t => t.Amount);

            if (FromForm.Amount == 0)
            {
                ModelState.AddModelError("Amount", "Cannot make transaction of amount 0.00");
            }
            if (balance < -FromForm.Amount)
            {
                ModelState.AddModelError("Amount", "Cannot withdraw more than your balance!");
            }

            if (ModelState.IsValid)
            {
                FromForm.UserId = userId.Value;
                _context.Add(FromForm);
                _context.SaveChanges();

                return RedirectToAction("Details", new {accountId = userId.Value});
            }
            return View("Details", new {accountId = userId.Value});
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}