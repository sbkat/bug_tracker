using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bug_tracker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace bug_tracker.Controllers
{
    public class HomeController : Controller
    {

    private MyContext dbContext;
    public HomeController(MyContext context)
    {
        dbContext = context;
    }
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet("devreg")]
    public IActionResult RegisterDev()
    {
        return View();
    }
    [HttpPost("register")]
    public IActionResult Register(User newUser)
    {
        if(ModelState.IsValid)
        {
            if(dbContext.Users.Any(user => user.email == newUser.email))
            {
                ModelState.AddModelError("email", "Email is already registered.");
                return View("Index");
            }
            else
            {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.password = Hasher.HashPassword(newUser, newUser.password);
                newUser.UserPrivilege = 2;
                dbContext.Users.Add(newUser);
                dbContext.SaveChanges();
                HttpContext.Session.SetString("User", newUser.email);
                HttpContext.Session.SetString("UserName", newUser.firstName);
                HttpContext.Session.SetInt32("UserId", newUser.UserId);
                return RedirectToAction("Dashboard");
            }
        }
        else
        {
            return View("Index");
        }
    }
    [HttpPost("adminreg")]
    public IActionResult RegisterAdmin(User newUser)
    {
        if(ModelState.IsValid)
        {
            if(dbContext.Users.Any(user => user.email == newUser.email))
            {
                ModelState.AddModelError("email", "Email is already registered.");
                return View("Index");
            }
            else
            {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.password = Hasher.HashPassword(newUser, newUser.password);
                newUser.UserPrivilege = 1;
                dbContext.Users.Add(newUser);
                dbContext.SaveChanges();
                HttpContext.Session.SetString("User", newUser.email);
                HttpContext.Session.SetString("UserName", newUser.firstName);
                HttpContext.Session.SetInt32("UserId", newUser.UserId);
                return RedirectToAction("Dashboard");
            }
        }
        else
        {
            return View("Index");
        }
    }
    [HttpGet("login")]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost("login")]
    public IActionResult Login(LoginUser existingUser)
    {
        if(ModelState.IsValid)
        {
            if(dbContext.Users.Any(user => user.email == existingUser.email))
            {
                User userInDb = dbContext.Users.FirstOrDefault(user => user.email == existingUser.email);
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(existingUser, userInDb.password, existingUser.password);
                if(result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }
                else
                {
                    HttpContext.Session.SetString("User", existingUser.email);
                    HttpContext.Session.SetString("UserName", userInDb.firstName);
                    HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                    return RedirectToAction("Dashboard");
                }
            }
            else
            {
                ModelState.AddModelError("email", "Email has not been registered.");
                return View("Login");
            }
        }
        else
        {
            return View("Login");
        }
    }
    [HttpGet("tickets")]
    public IActionResult Dashboard() 
    {
        if(HttpContext.Session.GetString("User")==null)
        {
            return RedirectToAction("Index");
        }
        else
        {
            List<Ticket> allTickets = dbContext.Tickets.Include(u=>u.Assignment).ToList();
            return View(new TicketViewModel{Tickets = allTickets});
        }
    }
    [HttpGet("tickets/new")]
    public IActionResult NewTicket() 
    {
        if(HttpContext.Session.GetString("User")==null)
        {
            return RedirectToAction("Index");
        }
        else
        {
            List<User> allUsers = dbContext.Users.ToList();
            return View(new TicketViewModel{Users = allUsers});
        }
    }
    [HttpPost("tickets/new")]
    public IActionResult CreateTicket(TicketViewModel newTicket) 
    {
        if(ModelState.IsValid)
        {
            User ticketCreator = dbContext.Users.FirstOrDefault(user => user.UserId == HttpContext.Session.GetInt32("UserId"));
            User thisUser = dbContext.Users.FirstOrDefault(user => user.UserId == newTicket.Ticket.UserId);
            if(ticketCreator.UserId == 1)
            {
                if(newTicket.Ticket.Deadline > DateTime.Now)
                {
                    newTicket.Ticket.Assignment = thisUser;
                    dbContext.Add(newTicket.Ticket);
                    dbContext.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ModelState.AddModelError("Ticket.Deadline", "Deadline must be set to a future date.");
                    List<User> allUsers = dbContext.Users.ToList();
                    return View("NewTicket", new TicketViewModel{Users = allUsers});
                }
            }
            else
            {
                ModelState.AddModelError("Ticket.UserId", "Need admin privileges to create/assign a new ticket.");
                List<User> allUsers = dbContext.Users.ToList();
                return View("NewTicket", new TicketViewModel{Users = allUsers});
            }
        }
        else
        {
            List<User> allUsers = dbContext.Users.ToList();
            return View("NewTicket", new TicketViewModel{Users = allUsers});
        }
    }
    [HttpGet("tickets/{id}")]
    public IActionResult TicketDetails(int id)
    {
        Ticket thisTicket = dbContext.Tickets.Include(ticket => ticket.Assignment).FirstOrDefault(ticket => ticket.TicketId == id);
        return View(thisTicket);
    }
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
