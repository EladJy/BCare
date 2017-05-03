using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BCare.data;

namespace BCare.Controllers
{
    public class UsersController : Controller
    {
        BcareContext context;
        public UsersController()
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext; 
        }
        public IActionResult Index()
        {
            yy
            return View(context.GetAllUsers());
        }
    }
}