using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BCare.data;

namespace BCare.Controllers
{
    public class GraphController : Controller
    {
        BcareContext context;
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult HMOChart()
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            List<Tuple<string, int>> listHMO = context.countUsersByHMOStats();
            return Json(listHMO);
        }
    }
}