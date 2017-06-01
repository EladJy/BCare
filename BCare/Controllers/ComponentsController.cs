using System;
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
    public class ComponentsController : Controller
    {
        BcareContext context;
        public IActionResult Info(int id)
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            ViewBag.Info = context.getComponentInfo(id);
            return View();
        }
    }
}
