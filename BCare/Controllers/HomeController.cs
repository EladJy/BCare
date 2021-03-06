﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BCare.data;
using BCare.Models;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;

namespace BCare.Controllers
{
    public class HomeController : Controller
    {
        BcareContext context;
        public IActionResult Index()
        {
            if(TempData["errorMessage"] != null)
                ViewBag.Error = TempData["errorMessage"];
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            if (context.Login(username, password))
            {
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Append("Session", RandomString(10) + context.GetIDByUserName(username), options);
                Response.Cookies.Append("UserName", username, options);
            } else
            {
                TempData["errorMessage"] = "שם המשתמש או הסיסמה לא תקינים";
            }
            return RedirectToAction("Index");
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public IActionResult LogOut()
        {
            Response.Cookies.Delete("Session");
            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpPost]
        public JsonResult top5()
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            var products = context.TopFiveMedications();
            return Json(products);
        }
        public IActionResult Charts()
        {
            return View();
        }
    }
}
