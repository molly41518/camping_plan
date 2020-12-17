using campingplan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using campingplan.App_Class;
using static campingplan.App_Class.UserAccount;
using System.Web.Security;

namespace campingplan.Controllers
{
    public class UserController : Controller
    {
        dbcon db = new dbcon();

        public ActionResult Login()
        {
            if (UserAccount.IsAuthenticated)
            {
                UserAccount.Login(UserAccount.GetIdentityValue(enIdentityType.Name));
                if (UserAccount.IsLogin) return View("LoginClear");
            }

            cvmLogin model = new cvmLogin()
            {
                UserAccount = "",
                UserPassword = "",
                Remember = false,
                ErrorMessage = "xxxx"
            };
            return View("Login", model);
        }


        public ActionResult Loginclear()
        {
            if (UserAccount.IsLogin) return RedirectToAction("RedirectToUserPage");
            return View("Index");
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

            string str_password = model.UserPassword;
            using (Cryptographys cryp = new Cryptographys())
            {
                str_password = cryp.SHA256Encode(str_password);
            }

            var users = db.users
                .Where(m => m.maccount == model.UserAccount)
                .Where(m => m.mpassword == str_password)
                .FirstOrDefault();
            if (users == null)
            {
                UserAccount.LogOut();
                FormsAuthentication.SignOut();
                ViewBag.Message = "帳號或密碼錯誤！";
                return View(model);
            }

            UserAccount.Login(users, UserAccount.GetRoleNo(users.role_no));
            UserAccount.IsRememberMe = model.Remember;
            UserAccount.LoginAuthenticate();

            return RedirectToAction("RedirectToUserPage");
        }

        public ActionResult Logout()
        {
            UserAccount.LogOut();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }



        [HttpGet]
        public ActionResult Register()
        {
            users model = new users();
            return View(model);
        }

        [HttpPost]
        public ActionResult Register(users model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //自定義檢查
            bool bln_error = false;
            var user = db.users.Where(m => m.maccount == model.maccount).FirstOrDefault();
            if (user != null) { ModelState.AddModelError("", "帳號重複註冊！"); bln_error = true; }
            user = db.users.Where(m => m.memail == model.memail).FirstOrDefault();
            if (user != null) { ModelState.AddModelError("", "電子信箱重複註冊！"); bln_error = true; }
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

            model.role_no = "Member";

            //寫入資料庫
            db.Configuration.ValidateOnSaveEnabled = false;
            db.users.Add(model);
            db.SaveChanges();

            //寄出驗證信
            SendVerifyMail(model.memail, model.varify_code);
            return RedirectToAction("SendEmailResult");

        }

        private string SendVerifyMail(string usermemail, string varifyCode)
        {
            string str_app_name = "營火計畫";
            var str_url = string.Format("/User/VerifyEmail/{0}", varifyCode);
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
            gmail.ReceiveEmail = usermemail;
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
            bool status = false;
            db.Configuration.ValidateOnSaveEnabled = false;
            var user = db.users.Where(m => m.varify_code == id).FirstOrDefault();
            if (user == null)
            {
                ViewBag.Message = "驗證碼錯誤！";
                status = false;
            }
            else
            {
                if (user.isvarify == 1)
                {
                    ViewBag.Message = "已經完成驗證，無須重複執行！";
                }
                else
                {
                    user.isvarify = 1;
                    db.SaveChanges();
                    status = true;
                }
                ViewBag.status = status;
            }
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(cvmResetPassword model)
        {
            if (!ModelState.IsValid) return View(model);

            //自定義檢查
            string str_password = "";
            using (Cryptographys cryp = new Cryptographys())
            { str_password = cryp.SHA256Encode(model.CurrentPassword); }
            bool bln_error = false;

            var check = db.users
                .Where(m => m.mno == model.UserNo)
                .Where(m => m.mpassword == str_password)
                .FirstOrDefault();
            if (check == null) { ModelState.AddModelError("", "目前密碼輸入錯誤!!"); bln_error = true; }
            if (bln_error) return View(model);

            str_password = model.NewPassword;
            var user = db.users.Where(m => m.mno == model.UserNo).FirstOrDefault();
            if (user != null)
            {
                //密碼加密
                using (Cryptographys cryp = new Cryptographys())
                { str_password = cryp.SHA256Encode(str_password); }

                user.mpassword = str_password;
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
                db.Configuration.ValidateOnSaveEnabled = true;
            }
            return RedirectToAction("RedirectToUserPage");
        }

        public ActionResult RedirectToUserPage()
        {
            if (UserAccount.RoleNo == AppEnums.enUserRole.Admin) return RedirectToAction("Index", "Admin", new { area = "Admin" });
            if (UserAccount.RoleNo == AppEnums.enUserRole.Vendor) return RedirectToAction("Index", "Vendor", new { area = "Vendor" });
            return RedirectToAction("Index", "Home");
        }



    }
}
