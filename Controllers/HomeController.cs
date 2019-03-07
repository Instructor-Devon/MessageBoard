using Microsoft.AspNetCore.Mvc;
using FakeReddit.Models;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private RedditContext dbContext;
    public HomeController(RedditContext context)
    {
        dbContext = context;
    }
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }
    // [HttpGet("login")]
    // public IActionResult LoginView()
    // {
    //     return View("Login");
    // }
    [HttpPost("login")]
    public IActionResult Login(LoginUser user)
    {
        if(ModelState.IsValid)
        {
            // log user in!

            // is there a user with the email provided?
            var userFromDB = dbContext.Users
                .FirstOrDefault(u => u.Email == user.LogEmail);

            if(userFromDB == null)
                ModelState.AddModelError("LogEmail", "Invalid Email/Password");

            // if so, is the password they provided correct?
            else
            {
                var hasher = new PasswordHasher<LoginUser>();

                PasswordVerificationResult result = hasher.VerifyHashedPassword(user, userFromDB.Password, user.LogPassword);
                // mismatch is 0
                if(result == 0)
                {
                    ModelState.AddModelError("LogEmail", "Invalid Email/Password");
                }

                if(ModelState.IsValid)
                {
                    // PUT USER IN SESSION!
                    HttpContext.Session.SetInt32("UserId", userFromDB.UserId);
                    return RedirectToAction("Index", "Posts");
                }

            }
            return View("Index");

        }
        return View("Index");
    }
    [HttpPost("create")]
    public IActionResult Create(User newUser)
    {
        if(ModelState.IsValid)
        {
            // validate DOB
            if(newUser.DOB > DateTime.Now)
            {
                ModelState.AddModelError("DOB", "Birthdays must be in the past!");
            }

            // Check uniqueness of EMAIL!
            var UserWithEmail = dbContext.Users
                .FirstOrDefault(u => u.Email == newUser.Email);

            

            if(UserWithEmail != null)
                ModelState.AddModelError("Email", "Email is in use.");

            if(dbContext.Users.Any(u => u.Email == newUser.Email))
                Console.WriteLine("USER EXISTS!");

            
            if(ModelState.IsValid)
            {
                // HASH PASSWORD!
                var hasher = new PasswordHasher<User>();
                string hashedPW = hasher.HashPassword(newUser, newUser.Password);

                newUser.Password = hashedPW;

                dbContext.Users.Add(newUser);
                dbContext.SaveChanges();
                // TODO: Log User in
                TempData["message"] = "You may now log in!";
                return RedirectToAction("Index");
            }
            
        }
        return View("Index");
    }
    [HttpGet("user/{userId}")]
    public IActionResult UserDetails(int userId)
    {
        User model = dbContext.Users
            .Include(u => u.Posts)
            .FirstOrDefault(u => u.UserId == userId);
        if(model == null)
            return RedirectToAction("Index");

        return View(model);

    }
}