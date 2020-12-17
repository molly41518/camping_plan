using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using campingplan.App_Class;
using campingplan.Models;

namespace campingplan.Controllers
{
    public class MemberController : Controller
    {
        // GET: Member
        public ActionResult Index()
        {
            return View();
        }
        [LoginAuthorize(RoleNo = "Member")]
        public ActionResult MemberProfile()
        {
            using (dbcon db = new dbcon())
            {
                var model = db.users
                    .Where(m => m.mno == UserAccount.UserNo)
                    .FirstOrDefault();
                return View(model);
            }
        }

        [HttpGet]
        [LoginAuthorize(RoleNo = "Member")]
        public ActionResult EditProfile()
        {
            using (dbcon db = new dbcon())
            {
                var model = db.users
                .Where(m => m.mno == UserAccount.UserNo)
                .FirstOrDefault();
                return View(model);
            }
        }

        [HttpPost]
        [LoginAuthorize(RoleNo = "Member")]
        public ActionResult EditProfile(users model)
        {
            if (!ModelState.IsValid) return View(model);

            using (dbcon db = new dbcon())
            {
                var user = db.users
                .Where(m => m.rowid == model.rowid)
                .FirstOrDefault();

                if (user != null)
                {
                    user.mname = model.mname;
                    user.memail = model.memail;
                    user.birthday = model.birthday;
                    user.remark = model.remark;
                    db.SaveChanges();
                }

                return RedirectToAction("MemberProfile");
            }
        }

        [LoginAuthorize(RoleNo = "Member")]
        public ActionResult UploadImage()
        {
            UserAccount.UploadImageMode = true;
            return RedirectToAction("MemberProfile");
        }

        [HttpPost]
        [LoginAuthorize(RoleNo = "Member")]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    var fileName = UserAccount.UserNo + ".jpg";
                    var path = Path.Combine(Server.MapPath("~/Images/user"), fileName);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                    file.SaveAs(path);
                }
            }
            UserAccount.UploadImageMode = false;
            return RedirectToAction("MemberProfile");
        }

        [LoginAuthorize(RoleNo = "Member")]
        public ActionResult UploadCancel()
        {
            UserAccount.UploadImageMode = false;
            return RedirectToAction("MemberProfile");
        }
    }
}