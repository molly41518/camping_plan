using campingplan.App_Class;
using campingplan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace campingplan.Controllers
{
    public class HomeController : Controller
    {
        dbcon db = new dbcon();
        public ActionResult Index()
        {
            dbcon db = new dbcon();
            var model = db.product
               .Where(m => m.istop == 1)
               .ToList();
            return View(model);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}