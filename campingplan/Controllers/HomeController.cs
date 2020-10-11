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
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string UserId, string UserPassword)
        {
            var customer = db.customer.Where(m => m.maccount == UserId && m.mpassword == UserPassword).FirstOrDefault();

            if(customer == null)
            {
                ViewBag.Message = "帳號密碼錯誤!登入失敗!";
                return View();
            }

            Session["Welcome"] = customer.mnickname + "歡迎登入";
            Session["Customer"] = customer;

            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(customer vcustomer)
        {
            if(ModelState.IsValid == false)
            {
                return View();
            }
            
            var dcustomer = db.customer.Where(m => m.maccount == vcustomer.maccount).FirstOrDefault();

            if(dcustomer == null)
            {
                db.customer.Add(vcustomer);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            ViewBag.Message = "此帳號已有人註冊！";
            return View();
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