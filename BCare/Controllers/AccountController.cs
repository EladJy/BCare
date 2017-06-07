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
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();

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
            //context.SetNewComment(314118456,5,1,"04-06-2017",4,"Veru helpful");
            //context.DeleteComment(314118456, 5, 1);
            //context.UpdateComment(314118456, 5, 1, "04-06-2017", 2, "Not So Good");
            List<Tuple<double, double>> range = context.GetBOARangeByID(2, 314118456);
            return View();
        }

        public JsonResult BtCount()
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            if (cookie != null)
            {
                List<Tuple<string, int>> listHMO = context.UserBloodTestByDateStats(Int32.Parse(cookie.Substring(10)));
                return Json(listHMO);
            }
            return Json("");
        }

        public IActionResult Stats()
        {
            String cookie = Request.Cookies["Session"];
            if (cookie != null)
            {
                ViewBag.UserID = Int32.Parse(cookie.Substring(10));
            }
            return View();
        }
        public IActionResult Register()
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            if (cookie == null)
            {
                List<health_maintenance_organizations> listHMO = context.GetAllHMO();
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
            return RedirectToAction("Index", "Home");
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
                    if (ViewBag.UserID == ViewBag.UserTestResult.User_ID)
                    {
                        ViewBag.Message = "isCorrect";
                    }
                    else
                    {
                        ViewBag.Message = "ErrorID";
                    }
                }
                else
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

        public IActionResult UpdateDetails()
        {
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
            int presId;
            presCommentViewModel prescription;
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            presId = context.GetPresByBloodTest(id);
            if (TempData["errorMessage"] != null)
                ViewBag.Error = TempData["errorMessage"];
            if (presId == 0)
            {
                Models.GA.Population po = new Models.GA.Population(id, context);
                for (int i = 0; i < 499; i++)
                {
                    po.NextGeneration();
                }
                po.WriteNextGeneration();
                Models.GA.Individual bestResult = po.bestList[0];
                context.SetNewPrescription(id, DateTime.Now, 123123123, bestResult.text);
                presId = context.GetPresByBloodTest(id);
                if (!bestResult.noExecptions)
                {
                    foreach (int med in bestResult.hashMed)
                    {
                        context.SetNewPrescriptionDetails(presId, med, RandomNumber(1, 3), RandomNumber(5, 8), "");
                    }
                }
                prescription = context.getPrescriptionDetails(presId, id);
                return View(prescription);
            }
            else
            {
                prescription = context.getPrescriptionDetails(presId, id);
                return View(prescription);
            }
        }

        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }

        public IActionResult TestInfo()
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            return View();
        }

        [HttpPost]
        public IActionResult addFeedback(int id, int rating , string content)
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            int presId  = context.GetPresByBloodTest(id);
            int userId = Int32.Parse(cookie.Substring(10));
            presCommentViewModel pres = context.getPrescriptionDetails(presId, id);
            HashSet<int> meds = context.getMedsByUser(userId);
            for(int i=0; i < pres.somcList.Count; i++)
            {
                if(!meds.Contains(pres.somcList[i].SOMI.SomID))
                {
                    context.SetNewComment(userId, pres.somcList[i].SOMI.SomID, presId, DateTime.Now.ToString(), rating, content);
                }
            }
            if(meds.Count > 0)
            {
                TempData["errorMessage"] = "דירגת כבר את התרופות הללו.";
            }
            return RedirectToAction("AnalyzeTest", "Account", new { id = id });
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
            }
            else
            {
                return View();
            }
        }

        public IActionResult AddBloodTestManually()
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            List<Tuple<int, string>> ListComp = context.GetBOAList();
            ViewBag.ListComp = new SelectList(ListComp, "Item1", "Item2");
            return View();
        }
        
        [HttpPost]
        public IActionResult AddBloodTestManually(BloodTestViewModel BTVM)
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            int bloodId = context.SetNewBloodTest(Int32.Parse(cookie.Substring(10)), BTVM.Doctor_Name, DateTime.Now, "N");
            for(int i=0; i < BTVM.BTC.Count; i++)
            {
                context.setNewBloodTestData(bloodId, BTVM.BTC[i].btData.BCompID , BTVM.BTC[i].btData.Value);
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult DeleteBloodTest(int id)
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            String cookie = Request.Cookies["Session"];
            if(Int32.Parse(cookie.Substring(10)) == context.GetUserIdByBloodTest(id)) {
                context.DeleteBloodTest(id);
                return RedirectToAction("BloodTest", "Account");
            }
            ViewBag.Error = "אל תנסה אתה לא תצליח!";
            return View();
        }
    }
}