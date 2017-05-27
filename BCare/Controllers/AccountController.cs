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
            //context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            //context.Register(319253365, "Maria", "Gunko", "F" , "06-01-1992",7, "AB+", "Abraham Ofer 11 Ashdod", "MashaG", "1234", false);
            //List<blood_test> BT = context.GetUserTests(34928267);
            //context.UpdateUserDetails(319253365, "Maria", "Gunko", "F" , "06-01-1992",7, "A+", "Abraham Ofer 11 Ashdod", "Shelly", "1234", "gunko@gmail.com");
            //List<BloodTestViewModel> BTD = context.GetTestResultByID(1);
            //long counter = context.CountTestsByID(304442254);
            //List<supplements_or_medication_info> SOMI = context.TopFiveMedications();
            return View();
        }

        public IActionResult Register()
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            if(cookie == null)
            {
                ViewBag.ListHMO = context.GetAllHMO();
            }
            return View();
        }
        [HttpPost]
        public IActionResult Register(int User_ID, string First_Name, string Last_Name, string Gender, string Birth_Date, int HMO_ID, string Blood_Type, string Address, string username, string password, string Email, bool isDoctor)
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            context.Register(User_ID, First_Name, Last_Name, Gender, Birth_Date, HMO_ID, Blood_Type, Address, username, password,Email, isDoctor);
            return RedirectToAction("Index","Home");
        }

        public IActionResult BloodTest()
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            if (cookie != null)
            {
                ViewBag.UserID = context.GetIDByUserName(cookie.Substring(10));
            }
            return View(context.GetUserTests(ViewBag.UserID));
        }

        public IActionResult BloodTestResult(int id)
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            if (cookie != null)
            {
                ViewBag.UserID = context.GetIDByUserName(cookie.Substring(10));
                ViewBag.UserTestResult = context.GetTestResultByID(id);
                if (ViewBag.UserTestResult.Count != 0)
                {
                    if(ViewBag.UserID == ViewBag.UserTestResult[0].user.UserID)
                    {
                        ViewBag.Message = "isCorrect";
                    } else
                    {
                        ViewBag.Message = "ErrorID";
                    }
                } else
                {
                    ViewBag.Message = "NoTests";
                }
            }
            return View();
        }

        public IActionResult AddBloodTest()
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            return View();
        }

        public IActionResult Stats()
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            return View();
        }

        public IActionResult UpdateDetails()        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            var userDetails = context.GetUserDetailsByID(context.GetIDByUserName(cookie.Substring(10)));
            return View(userDetails);
        }
        [HttpPost]
        public IActionResult updateDetails(string First_Name, string Last_Name, string Gender, string Birth_Date, int HMO_ID, string Blood_Type, string Address, string userName, string password, string Email)
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            context.UpdateUserDetails(context.GetIDByUserName(cookie.Substring(10)), First_Name, Last_Name, Gender, Birth_Date, HMO_ID, Blood_Type, Address, userName, password, Email);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AnalyzeTest(int id)
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            return View(context.GetBloodTestByID(id));
        }
        public IActionResult TestInfo()
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            return View();
        }

        public IActionResult Details()
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            ViewBag.ListHMO = context.GetAllHMO();
            if(cookie != null)
            {
                UserDetailViewModel userDetails = context.GetUserDetailsByID(context.GetIDByUserName(cookie.Substring(10)));
                return View(userDetails);
            } else
            {
                return View();
            }
            
        }
    }
}