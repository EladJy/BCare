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
        public IActionResult Index()
        {
            context = HttpContext.RequestServices.GetService(typeof(BCare.data.BcareContext)) as BcareContext;
            context.SignUp(319253365, "����", "�����", "Female" , "06-01-1992",7, "AB+", "����� ���� 11 �����");
            return View(context.GetAllUsers());
        }
    }
}