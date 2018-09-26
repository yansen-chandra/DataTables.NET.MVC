using DataTables.MVC.Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataTables.MVC.Demo.Controllers
{
    public class DemoController : Controller
    {
        // GET: Demo
        public ActionResult Index()
        {
            var model = DummyRepo.Instance.Persons;
            return View(model);
        }
    }
}