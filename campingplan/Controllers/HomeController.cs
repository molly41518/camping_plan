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
            //var product = db.product.ToList();
            if (Session["Customer"] == null)
            {
                return View("Index", "_Layout");
            }
            return View("Index", "_Layout - Login");
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