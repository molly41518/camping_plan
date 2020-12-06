using campingplan.App_Class;
using campingplan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace campingplan.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        [LoginAuthorize(RoleNo = "Admin")]
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
                    ViewBag.OrderDetail = details;
                }
            }
            return View();
        }

        [HttpGet]
        [LoginAuthorize(RoleNo = "Admin")]
        public ActionResult GetDataList()
        {
            using (dbcon db = new dbcon())
            {
                var models = db.order
                    .Join(db.payments, p => p.payment_no, d => d.payment_no,(p1, d1) => new { p1, payment_name = d1.payment })
                    .Join(db.status, p => p.p1.order_status, d => d.status_no,(p2, d2) => new { p2, status_name = d2.status_name })
                    .Join(db.users , p => p.p2.p1.mno , d => d.mno,(p3 , d3) => new 
                    {
                        rowid = p3.p2.p1.rowid,
                        order_closed = p3.p2.p1.order_closed,
                        mno = p3.p2.p1.mno,
                        user_name = d3.mname,
                        order_no = p3.p2.p1.order_no,
                        order_date = p3.p2.p1.order_date,
                        order_status = p3.p2.p1.order_status,
                        status_name = p3.status_name,
                        payment_name = p3.p2.payment_name,
                        receive_name = p3.p2.p1.receive_name,
                        receive_address = p3.p2.p1.receive_address,
                        remark = p3.p2.p1.remark
                    })
                     .Where(m => m.order_closed == UserAccount.UserCode)
                     .OrderByDescending(m => m.order_no).ToList();

                return Json(new { data = models }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [LoginAuthorize(RoleNo = "Admin")]
        public ActionResult ChangeStatus(int id = 0)
        {
            string str_status = "ON";
            using (dbcon db = new dbcon())
            {
                var model = db.order.Where(m => m.rowid == id).FirstOrDefault();
                if (model != null) str_status = model.order_status;

                var selectList = new List<SelectListItem>();
                List<status> lists = Shop.GetStatusList();
                foreach (var item in lists)
                {
                    SelectListItem list = new SelectListItem();
                    list.Value = item.status_no;
                    list.Text = item.status_name;
                    selectList.Add(list);
                }
                //預設選擇哪一筆
                selectList.Where(m => m.Value == str_status).First().Selected = true;

                ViewBag.SelectList = selectList;
                return View(model);
            }
        }

        [HttpPost]
        [LoginAuthorize(RoleNo = "Admin")]
        public ActionResult ChangeStatus(order model)
        {
            using (dbcon db = new dbcon())
            {
                bool status = false;
                var order = db.order.Where(m => m.order_no == model.order_no).FirstOrDefault();
                if (order != null)
                {
                    order.order_status = model.order_status;
                    order.order_closed = Shop.GetOrderClosed(model.order_status);
                    order.order_validate = Shop.GetOrderValidate(model.order_status);
                    db.SaveChanges();
                    status = true;
                }
                return new JsonResult { Data = new { status = status } };
            }
        }
    }
}