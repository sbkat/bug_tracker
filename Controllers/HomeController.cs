﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bug_tracker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

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
    [HttpGet("events")]
    public IActionResult Dashboard() 
    {
        if(HttpContext.Session.GetString("User")==null)
        {
            return RedirectToAction("Index");
        }
        else
        {
            return View();
        }
    }
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
