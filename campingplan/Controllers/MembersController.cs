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
    public class MembersController : Controller
    {
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
                    user.mnickname = model.mnickname;
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


        [HttpGet]
        [LoginAuthorize(RoleNo = "Member")]
        public ActionResult GetDataList(int isclose)
        {
            ViewBag.isclose = isclose;
            using (dbcon db = new dbcon())
            {
                var models = db.order
                    .Join(db.payments, p => p.payment_no, d => d.payment_no,
                    (p1, d1) => new { p1, payment_name = d1.payment }).ToList();
                var models1 = models.Join(db.status, p => p.p1.order_status, d => d.status_no,
                    (p2, d2) => new cvmGetDataList
                    {
                        rowid = p2.p1.rowid,
                        order_closed = p2.p1.order_closed,
                        user_no = p2.p1.mno,
                        order_no = p2.p1.order_no,
                        order_date = p2.p1.order_date,
                        status_name = d2.status_name,
                        status_no = p2.p1.order_status,
                        payment_name = p2.payment_name,
                        receive_name = p2.p1.receive_name,
                        receive_address = p2.p1.receive_address,
                        remark = p2.p1.remark
                    }) 
                     .Where(m => m.user_no == UserAccount.UserNo)
                     .Where(m => m.order_closed == isclose)
                     .OrderByDescending(m => m.order_no).ToList();
                return View(models1);
            }
        }

        //退貨
        [HttpGet]
        [LoginAuthorize(RoleNo = "Member")]
        public ActionResult ReturnProduct(int order_rowid = 0)
        {
            using (dbcon db = new dbcon())
            {
                var model = db.order.Where(m => m.rowid == order_rowid).FirstOrDefault();
                if (model != null)
                {
                    if (Shop.IsUnCloseOrder(model.order_status))
                    {
                        if (model.order_status == "DS"|| model.order_status == "SR" || model.order_status == "CP" || model.order_status == "DU")
                        {
                            model.order_status = "RT";
                            model.order_closed = 1;
                            db.SaveChanges();
                        }

                    }
                }
            }
            return RedirectToAction("GetDataList", "Members", new { isclose = 0 });
        }

        //取消訂單
        [HttpGet]
        [LoginAuthorize(RoleNo = "Member")]
        public ActionResult CancelOrder(int order_rowid = 0)
        {
            using (dbcon db = new dbcon())
            {
                var model = db.order.Where(m => m.rowid == order_rowid).FirstOrDefault();
                if (model != null)
                {
                    if (Shop.IsUnCloseOrder(model.order_status))
                    {
                        if (model.order_status == "ON" || model.order_status == "PP" || model.order_status == "PN" || model.order_status == "OP")
                        {
                            model.order_status = "OR";
                            model.order_closed = 1;
                            db.SaveChanges();
                        }
                    }
                }
            }
            return RedirectToAction("GetDataList", "Members", new { isclose = 0 });
        }
        //訂單詳細頁
        [HttpGet]
        [LoginAuthorize(RoleNo = "Member")]
        public ActionResult OrderProductDetail(int order_rowid = 0)
        {
            using (dbcon db = new dbcon())
            {
                string str_order_no = "";
                var order = db.order.Where(m => m.rowid == order_rowid).FirstOrDefault();
                if (order != null) { str_order_no = order.order_no; }
                var details = db.order_detail.Where(m => m.order_no == str_order_no).ToList();
                ViewBag.OrderNo = str_order_no;
                ViewBag.tax = order.taxs;
                ViewBag.total = order.totals;
                return View(details);
            }
        }
    }
}