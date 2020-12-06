
using campingplan.App_Class;
using campingplan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace campingplan.Areas.Member.Controllers
{
    public class OrdersController : Controller
    {
        [LoginAuthorize(RoleNo = "Member")]
        public ActionResult Index(int id = 0, int code = -1)
        {
            UserAccount.UserStatus = id;
            if (code == -1)
            {
                if (UserAccount.UserCode == -1) UserAccount.UserCode = 0;
            }
            else
                UserAccount.UserCode = code;

            if (id > 0)
            {
                using (dbcon db = new dbcon())
                {
                    string str_order_no = "";
                    var order = db.order.Where(m => m.rowid == id).FirstOrDefault();
                    if (order != null) { str_order_no = order.order_no; }
                    var details = db.order_detail.Where(m => m.order_no == str_order_no).ToList();
                    ViewBag.OrderNo = str_order_no;
                    ViewBag.order_detail = details;
                }
            }
            return View();
        }

        [HttpGet]
        [LoginAuthorize(RoleNo = "Member")]
        public ActionResult GetDataList()
        {
            using (dbcon db = new dbcon())
            {
                var models = db.order
                    .Join(db.payments , p => p.payment_no , d => d.payment_no ,
                    (p1 , d1) => new { p1 , payment_name = d1.payment })
                    .Join(db.status , p => p.p1.order_status , d => d.status_no ,
                    (p2 , d2) => new { p2 , status_name = d2.status_name })
                    .Join(db.shippings , p => p.p2.p1.shipping_no , d => d.shipping_no,
                    (p3 , d3) => new
                    { 
                        rowid = p3.p2.p1.rowid,
                        order_closed = p3.p2.p1.order_closed,
                        user_no = p3.p2.p1.mno,
                        order_no = p3.p2.p1.order_no,
                        order_date = p3.p2.p1.order_date,
                        status_no = p3.p2.p1.order_status,
                        status_name = d3.shipping_name,
                        payment_name = p3.p2.payment_name,
                        receive_name = p3.p2.p1.receive_name,
                        receive_address = p3.p2.p1.receive_address,
                        remark = p3.p2.p1.remark
                    })
                     .Where(m => m.user_no == UserAccount.UserNo)
                     .Where(m => m.order_closed == UserAccount.UserCode)
                     .OrderByDescending(m => m.order_no).ToList();

                return Json(new { data = models }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [LoginAuthorize(RoleNo = "Member")]
        public ActionResult ReturnProduct(int id= 0)
        {
            using (dbcon db = new dbcon())
            {
                var model = db.order.Where(m => m.rowid == id).FirstOrDefault();
                if (model != null)
                {
                    if (Shop.IsUnCloseOrder(model.order_status))
                    {
                        model.order_status = "RT";
                        model.order_closed = 1;
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("Index" , "order" , new { area = "Member" , id = UserAccount.UserStatus  , code = UserAccount.UserCode});
        }

        [HttpGet]
        [LoginAuthorize(RoleNo = "Member")]
        public ActionResult CancelOrder(int id = 0)
        {
            using (dbcon db = new dbcon())
            {
                var model = db.order.Where(m => m.rowid == id).FirstOrDefault();
                if (model != null)
                {
                    if (Shop.IsUnCloseOrder(model.order_status))
                    {
                        model.order_status = "OR";
                        model.order_closed = 1;
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("Index", "order", new { area = "Member", id = UserAccount.UserStatus, code = UserAccount.UserCode });
        }
    }
}