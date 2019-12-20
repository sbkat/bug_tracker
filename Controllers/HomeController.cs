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
    public IActionResult RegisterAdmin(User newUser, Admin newAdmin)
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
                dbContext.Admins.Add(newAdmin);
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
    [HttpPost("login/guest")]
    public IActionResult GuestUser()
    {
        HttpContext.Session.SetString("User", "Guest");
        HttpContext.Session.SetString("UserName", "Guest");
        return RedirectToAction("Dashboard");
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
            List<Ticket> allTickets = dbContext.Tickets.OrderBy(t => t.Deadline).Include(u=>u.Assignment).Include(t=>t.Creator).ToList();
            return View(new TicketViewModel{Tickets = allTickets});
        }
    }
    [HttpGet("tickets/search")]
    public IActionResult SearchTicket() 
    {
        if(HttpContext.Session.GetString("User")==null)
        {
            return RedirectToAction("Index");
        }
        else
        {
            List<Ticket> allTickets = dbContext.Tickets.OrderBy(t => t.CreatedAt).ToList();
            return View(allTickets);
        }
    }
    [HttpGet("tickets/{id}")]
    public IActionResult TicketDetails(int id) 
    {
        if(HttpContext.Session.GetString("User")==null)
        {
            return RedirectToAction("Index");
        }
        else
        {
            Ticket thisTicket = dbContext.Tickets.Include(ticket => ticket.Assignment).Include(ticket => ticket.Creator).Include(ticket => ticket.Comments).FirstOrDefault(ticket => ticket.TicketId == id);
            List<User> allUsers = dbContext.Users.ToList();
            return View(new TicketViewModel{Ticket = thisTicket, Users = allUsers});
        }
    }
    [HttpPost("tickets/{id}/comments/new")]
    public IActionResult AddComment(int id, TicketViewModel TicketComment)
    {
        Ticket queryTicket = dbContext.Tickets.OrderByDescending(t => t.CreatedAt).Include(t => t.Assignment).Include(t => t.Creator).Include(t => t.Comments).FirstOrDefault(t => t.TicketId == id);
        var newId = queryTicket.TicketId;
        List<User> allUsers = dbContext.Users.ToList();
        if(HttpContext.Session.GetString("User")==null)
        {
            return RedirectToAction("Index");
        }
        if(HttpContext.Session.GetString("UserName") == "Guest")
        {
            return RedirectToAction("TicketDetails", new {id = newId});
        }
        if(TicketComment.Comment.Content == null)
        {
            ModelState.AddModelError("Comment.Content", "Message content field cannot be empty.");
            return View("TicketDetails", new TicketViewModel{Ticket = queryTicket, Users = allUsers});
        }
        if(ModelState.IsValid)
        {
            if(TicketComment.Comment.Content.Length > 5)
            {
                Ticket thisTicket = dbContext.Tickets.FirstOrDefault(t => t.TicketId == id);
                User thisUser = dbContext.Users.FirstOrDefault(user => user.UserId == HttpContext.Session.GetInt32("UserId"));
                Comment newComment = new Comment();
                newComment.Content = TicketComment.Comment.Content;
                newComment.UserId = thisUser.UserId;
                newComment.TicketId = thisTicket.TicketId;
                newComment.UserCommented = thisUser;
                newComment.TicketCommentedOn = thisTicket;
                dbContext.Add(newComment);
                dbContext.SaveChanges();
                return RedirectToAction("TicketDetails", new {id = newId});
            }
            else
            {
                ModelState.AddModelError("Comment.Content", "Message must be at least 5 characters (250 max)");
                return View("TicketDetails", new TicketViewModel{Ticket = queryTicket, Users = allUsers});
            }
        }
        else
        {
            return View("TicketDetails", new TicketViewModel{Ticket = queryTicket, Users = allUsers});
        }
    }
    [HttpPost("tickets/{tid}/comments/{cid}/delete")]
    public IActionResult DeleteComment(int tid, int cid)
    {
        if(HttpContext.Session.GetString("UserName") == "Guest")
        {
            return RedirectToAction("Dashboard");
        }
        else
        {
            Comment thisComment = dbContext.Comments.FirstOrDefault(comment => comment.CommentId == cid);
            dbContext.Comments.Remove(thisComment);
            dbContext.SaveChanges();
            Ticket thisTicket = dbContext.Tickets.FirstOrDefault(ticket => ticket.TicketId == tid);
            var newId = thisTicket.TicketId;
            return RedirectToAction("TicketDetails", new {id = newId});
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
        if(HttpContext.Session.GetString("User")==null)
        {
            return RedirectToAction("Index");
        }
        if(HttpContext.Session.GetString("UserName") == "Guest")
        {
            return RedirectToAction("Dashboard");
        }
        if(ModelState.IsValid)
        {
            User ticketCreator = dbContext.Users.FirstOrDefault(user => user.UserId == HttpContext.Session.GetInt32("UserId"));
            Admin Creator = dbContext.Admins.FirstOrDefault(admin => admin.AdminId == HttpContext.Session.GetInt32("UserId"));
            User assignedUser = dbContext.Users.FirstOrDefault(user => user.UserId == newTicket.Ticket.UserId);
            if(ticketCreator.UserPrivilege == 1)
            {
                if(newTicket.Ticket.Deadline > DateTime.Now)
                {
                    newTicket.Ticket.Assignment = assignedUser;
                    newTicket.Ticket.Creator = Creator;
                    newTicket.Ticket.Creator.UserId = ticketCreator.UserId;
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
    [HttpGet("tickets/{id}/edit")]
    public IActionResult EditTicketView(int id)
    {
        if(HttpContext.Session.GetString("User")==null)
        {
            return RedirectToAction("Index");
        }
        else
        {
            Ticket thisTicket = dbContext.Tickets.Include(ticket => ticket.Assignment).FirstOrDefault(ticket => ticket.TicketId == id);
            List<User> allUsers = dbContext.Users.ToList();
            return View(new TicketViewModel{Ticket = thisTicket, Users = allUsers});
        }
    }
    [HttpPost("tickets/{id}/edit/admin")]
    public IActionResult AdminEditTicket(int id, TicketViewModel updateTicket)
    {
        if(HttpContext.Session.GetString("User")==null)
        {
            return RedirectToAction("Index");
        }
        if(HttpContext.Session.GetString("UserName") == "Guest")
        {
            return RedirectToAction("Dashboard");
        }
        if(ModelState.IsValid) {
            if(updateTicket.Ticket.Deadline > DateTime.Now)
            {
                Ticket thisTicket = dbContext.Tickets.FirstOrDefault(ticket => ticket.TicketId == id);
                User assignedUser = dbContext.Users.FirstOrDefault(user => user.UserId == updateTicket.UserId);
                User ticketCreator = dbContext.Users.FirstOrDefault(user => user.UserId == HttpContext.Session.GetInt32("UserId"));
                Admin Creator = dbContext.Admins.FirstOrDefault(admin => admin.AdminId == HttpContext.Session.GetInt32("UserId"));
                thisTicket.Title = updateTicket.Ticket.Title;
                thisTicket.Task = updateTicket.Ticket.Task;
                thisTicket.Priority = updateTicket.Ticket.Priority;
                thisTicket.Deadline = updateTicket.Ticket.Deadline;
                thisTicket.Status = updateTicket.Ticket.Status;
                thisTicket.UserId = updateTicket.Ticket.UserId;
                thisTicket.Creator = Creator;
                thisTicket.Creator.UserId = ticketCreator.UserId;
                thisTicket.Assignment = assignedUser;
                thisTicket.UpdatedAt = DateTime.Now;
                dbContext.Update(thisTicket);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                ModelState.AddModelError("Ticket.Deadline", "Deadline must be set to a future date.");
                Ticket thisTicket = dbContext.Tickets.Include(ticket => ticket.Assignment).FirstOrDefault(ticket => ticket.TicketId == id);
                List<User> allUsers = dbContext.Users.ToList();
                return View("EditTicketView", new TicketViewModel{Ticket = thisTicket, Users = allUsers});
            }
        }
        return View("EditTicketView");
    }
    [HttpPost("tickets/{id}/edit")]
    public IActionResult EditTicket(int id, TicketViewModel updateTicket)
    {
        if(HttpContext.Session.GetString("UserName") == "Guest")
        {
            return RedirectToAction("Dashboard");
        }
        else
        {
            Ticket thisTicket = dbContext.Tickets.FirstOrDefault(ticket => ticket.TicketId == id);
            thisTicket.Status = updateTicket.Ticket.Status;
            dbContext.Update(thisTicket);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
    }
    [HttpPost("tickets/{id}/delete")]
    public IActionResult DeleteTicket(int id)
    {
        if(HttpContext.Session.GetString("UserName") == "Guest")
        {
            return RedirectToAction("Dashboard");
        }
        else
        {
            Ticket thisTicket = dbContext.Tickets.FirstOrDefault(ticket => ticket.TicketId == id);
            dbContext.Tickets.Remove(thisTicket);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard"); 
        }
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
