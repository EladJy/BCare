using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BCare.data;
using BCare.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BCare.Controllers
{
    public class AccountController : Controller
    {
        BcareContext context;
        public IActionResult Index()
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            //context.Register(319253365, "Maria", "Gunko", "F" , "06-01-1992",7, "AB+", "Abraham Ofer 11 Ashdod", "MashaG", "1234", false);
            //List<blood_test> BT = context.GetUserTests(34928267);
            //List<blood_test_data> BTD = context.GetTestResultByID(1);
            return View();
        }

        public IActionResult Register()
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            ViewBag.ListHMO = context.GetAllHMO();
            return View();
        }
        [HttpPost]
        public IActionResult Register(int User_ID, string First_Name, string Last_Name, string Gender, string Birth_Date, int HMO_ID, string Blood_Type, string Address, string username, string password, bool isDoctor)
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            context.Register(User_ID, First_Name, Last_Name, Gender, Birth_Date, HMO_ID, Blood_Type, Address, username, password, isDoctor);
            return RedirectToAction("Index","Home");
        }

        public IActionResult BloodTest()
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            //context.getIdByUserName(cookie.Substring(10));
            return View();
        }
    }
}