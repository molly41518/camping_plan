using campingplan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using campingplan.App_Class;

namespace campingplan.Controllers
{
    public class CustomerController : Controller
    {
        dbcon db = new dbcon();

        public ActionResult Login()
        {
            cvmLogin model = new cvmLogin()
            {
                CustomerAccount = "",
                CustomerPassword = "",
                Remember = false,
                ErrorMessage = "xxxx"
            };
            return View("Login", model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(cvmLogin model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Account or Password Error!!";
                return View(model);
            }
            var customers = db.customer
                .Where(m => m.maccount == model.CustomerAccount)
                .Where(m => m.mpassword == model.CustomerPassword)
                .FirstOrDefault();
            if (customers == null)
            {
                ViewBag.Message = "帳號或密碼錯誤！";
                return View(model);
            }

            CustomerAccount.Login(customers.mname);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            CustomerAccount.LogOut();
            return RedirectToAction("Index", "Home");
        }



        [HttpGet]
        public ActionResult Register()
        {
            customer model = new customer();
            return View(model);
        }

        [HttpPost]
        public ActionResult Register(customer model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //自定義檢查
            bool bln_error = false;
            var customer = db.customer.Where(m => m.maccount == model.maccount).FirstOrDefault();
            if (customer != null) { ModelState.AddModelError("", "帳號重複註冊！"); bln_error = true; }
            customer = db.customer.Where(m => m.memail == model.memail).FirstOrDefault();
            if (customer != null) { ModelState.AddModelError("", "電子信箱重複註冊！"); bln_error = true; }
            if (bln_error) return View(model);

            //密碼加密
            using (Cryptographys cryp = new Cryptographys())
            {
                model.mpassword = cryp.SHA256Encode(model.mpassword);
                model.ConfirmPassword = model.mpassword;
            }

            //產生驗證碼
            model.varify_code = Guid.NewGuid().ToString().ToUpper();
            model.isvarify = 0;

            //寫入資料庫
            db.Configuration.ValidateOnSaveEnabled = false;
            db.customer.Add(model);
            db.SaveChanges();

            //寄出驗證信
            SendVerifyMail(model.memail, model.varify_code);
            return RedirectToAction("SendEmailResult");

        }

        private string SendVerifyMail(string userEmail, string varifyCode)
        {
            string str_app_name = "營火計畫";
            var str_url = string.Format("/Customer/VerifyEmail/{0}", varifyCode);
            var str_link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, str_url);
            string str_subject = str_app_name + " - 帳號成功建立通知!!";
            string str_body = "<br/><br/>";
            str_body += "很高興告訴您，您的 " + str_app_name + " 帳戶已經成功建立. <br/>";
            str_body += "請按下下方連結完成驗證您的帳號程序!!<br/><br/>";
            str_body += "<a href='" + str_link + "'>" + str_link + "</a> ";
            str_body += "<br/><br/>";
            str_body += "本信件由電腦系統自動寄出,請勿回信!!<br/><br/>";
            str_body += string.Format("{0} 系統開發團隊敬上", str_app_name);

            GmailService gmail = new GmailService();
            gmail.ReceiveEmail = userEmail;
            gmail.Subject = str_subject;
            gmail.Body = str_body;
            gmail.Send();
            return gmail.MessageText;
        }

        public ActionResult SendEmailResult()
        {
            return View();
        }

        public ActionResult VerifyEmail(string id)
        {
            bool Status = false;
            db.Configuration.ValidateOnSaveEnabled = false;
            var customer = db.customer.Where(m => m.varify_code == id).FirstOrDefault();
            if (customer == null)
            {
                ViewBag.Message = "驗證碼錯誤！";
                Status = false;
            }
            else
            {
                if (customer.isvarify == 1)
                {
                    ViewBag.Message = "已經完成驗證，無須重複執行！";
                }
                else
                {
                    customer.isvarify = 1;
                    db.SaveChanges();
                    Status = true;
                }
                ViewBag.Status = Status;
            }
            return View();
        }
    }
}
