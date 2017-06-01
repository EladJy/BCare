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
            //List<BloodTestViewModel> BTD = context.GetTestResultByID(1);
            //long counter = context.CountTestsByID(304442254);
            //List<supplements_or_medication_info> SOMI = context.TopFiveMedications();
            //List<Tuple<string, int>> listHMO = context.countUsersByHMOStats();
            //List<Tuple<string, int>> listBT = context.countUsersByBloodTypeStats();
            //List<Tuple<string, int>> listBloodTestByDate = context.UserBloodTestByDateStats(39341227);
            List<Tuple<DateTime, double>> listCompStats = context.CompValuesStats(39341227, 2);
            return View();
        }
        
        public IActionResult Register()
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            if(cookie == null)
            {
                List < health_maintenance_organizations > listHMO = context.GetAllHMO();
                ViewBag.ListHMO = new SelectList(listHMO, "HMOID", "HMOName");

            }
            return View();
        }
        [HttpPost]
        public IActionResult Register(UserDetailViewModel newUser)
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            context.Register(newUser.user.UserID, newUser.user.FirstName, newUser.user.LastName, newUser.user.Gender.ToString(), newUser.user.BirthDate.ToString(), newUser.user.HMOID,
                newUser.user.BloodType.ToString(), newUser.user.Address, newUser.user.UserName, newUser.user.PWHash, newUser.user.Email, newUser.isDoctor);
            return RedirectToAction("Index","Home");
        }

        public IActionResult BloodTest()
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            if (cookie != null)
            {
                ViewBag.UserID = Int32.Parse(cookie.Substring(10));
            }
            return View(context.GetUserTests(ViewBag.UserID));
        }

        public IActionResult BloodTestResult(int id)
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            if (cookie != null)
            {
                ViewBag.UserID = Int32.Parse(cookie.Substring(10));
                ViewBag.UserTestResult = context.GetTestResultByID(id);
                if (ViewBag.UserTestResult.BTC.Count != 0)
                {
                    if(ViewBag.UserID == ViewBag.UserTestResult.User_ID)
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
            return View(ViewBag.UserTestResult);
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
            var userDetails = context.GetUserDetailsByID(Int32.Parse(cookie.Substring(10)));
            return View(userDetails);
        }
        [HttpPost]
        public IActionResult updateDetails(UserDetailViewModel userUpdated)
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            context.UpdateUserDetails(Int32.Parse(cookie.Substring(10)), userUpdated.user.FirstName, userUpdated.user.LastName, userUpdated.user.Gender.ToString(), userUpdated.user.BirthDate.ToString(),
                userUpdated.user.HMOID, userUpdated.user.BloodType.ToString(), userUpdated.user.Address, userUpdated.user.UserName, userUpdated.user.PWHash, userUpdated.user.Email, userUpdated.isDoctor);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AnalyzeTest(int id)
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            return View(context.getPrescriptionDetails(context.GetPresByBloodTest(id) , id));
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
            List<health_maintenance_organizations> listHMO = context.GetAllHMO();
            ViewBag.ListHMO = new SelectList(listHMO, "HMOID", "HMOName");
            if (cookie != null)
            {
                UserDetailViewModel userDetails = context.GetUserDetailsByID(Int32.Parse(cookie.Substring(10)));
                userDetails.user.PWHash = "";
                return View(userDetails);
            } else
            {
                return View();
            }
        }
    }
}