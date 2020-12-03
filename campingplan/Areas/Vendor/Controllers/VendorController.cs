using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using campingplan.App_Class;
using campingplan.Models;

namespace goshopping.Areas.Vendor.Controllers
{
    public class VendorController : Controller
    {
        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Index()
        {
            UserAccount.UploadImageMode = false;
            using (Sale sale = new Sale(DateTime.Today))
            {
                sale.CountAmount("M");
                ViewBag.MonthAmount = sale.AmountData;
                ViewBag.MonthAmountPercent = sale.PercentData;

                sale.CountQty("M");
                ViewBag.MonthCount = sale.AmountData; ;
                ViewBag.MonthCountPercent = sale.PercentData;

                sale.CountAmount("Y");
                ViewBag.YearAmount = sale.AmountData;
                ViewBag.YearAmountPercent = sale.PercentData;

                sale.CountQty("Y");
                ViewBag.YearCount = sale.AmountData; ;
                ViewBag.YearCountPercent = sale.PercentData;

                List<string> arrYearMonthName = sale.GetYearMonthNameList();
                List<int> arrYearMonthAmount = sale.GetYearMonthAmountList("Base");
                ViewBag.BaseYearAmountRank = sale.GetSaleRank("Base", "Amount", 20);
                ViewBag.BaseYearQtyRank = sale.GetSaleRank("Base", "Qty", 20);
                ViewBag.BaseYearMonthName = Newtonsoft.Json.JsonConvert.SerializeObject(arrYearMonthName);
                ViewBag.BaseYearMonthAmount = Newtonsoft.Json.JsonConvert.SerializeObject(arrYearMonthAmount);

                sale.CountAmount("W");
                List<string> arrPriorWeekName = sale.GetWeekNameList("Prior");
                List<int> arrPriorWeekAmount = sale.GetWeekAmountList("Prior");
                ViewBag.PriorWeekName = Newtonsoft.Json.JsonConvert.SerializeObject(arrPriorWeekName);
                ViewBag.PriorWeekAmount = Newtonsoft.Json.JsonConvert.SerializeObject(arrPriorWeekAmount);

                List<string> arrBaseWeekName = sale.GetWeekNameList("Base");
                List<int> arrBaseWeekAmount = sale.GetWeekAmountList("Base");
                ViewBag.BaseWeekName = Newtonsoft.Json.JsonConvert.SerializeObject(arrBaseWeekName);
                ViewBag.BaseWeekAmount = Newtonsoft.Json.JsonConvert.SerializeObject(arrBaseWeekAmount);

                return View();
            }
        }

        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult VendorProfile()
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
        [LoginAuthorize(RoleNo = "Vendor")]
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
        [LoginAuthorize(RoleNo = "Vendor")]
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

                return RedirectToAction("VendorProfile");
            }
        }

        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult UploadImage()
        {
            UserAccount.UploadImageMode = true;
            return RedirectToAction("VendorProfile");
        }

        [HttpPost]
        [LoginAuthorize(RoleNo = "Vendor")]
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
            return RedirectToAction("VendorProfile");
        }

        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult UploadCancel()
        {
            UserAccount.UploadImageMode = false;
            return RedirectToAction("VendorProfile");
        }
    }
}