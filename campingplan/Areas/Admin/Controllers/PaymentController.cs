using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using campingplan.App_Class;
using campingplan.Models;

namespace campingplan.Areas.Admin.Controllers
{
    public class PaymentController : Controller
    {
        [LoginAuthorize(RoleNo ="Admin")]
        public ActionResult Index()
        {
            return View();
        }

        [LoginAuthorize(RoleNo = "Admin")]
        public ActionResult GetDataList()
        {
            using (dbcon db = new dbcon())
            {
                var models = db.payments.OrderBy(m => m.payment_no).ToList();
                return Json(new { data = models }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [LoginAuthorize(RoleNo = "Admin")]
        public ActionResult Edit(int id = 0)
        {
            using (dbcon db = new dbcon())
            {
                if (id == 0)
                {
                    payments new_model = new payments();
                    return View(new_model);
                }
                var models = db.payments.Where(m => m.rowid == id).FirstOrDefault();
                return View(models);
            }
        }

        [HttpPost]
        [LoginAuthorize(RoleNo = "Admin")]
        public ActionResult Edit(payments models)
        {
            bool status = false;
            if (ModelState.IsValid)
            {
                using (dbcon db = new dbcon())
                {
                    if (models.rowid > 0)
                    {
                        //Edit 
                        var payments = db.payments.Where(m => m.rowid == models.rowid).FirstOrDefault();
                        if (payments != null)
                        {
                            payments.payment_no = models.payment_no;
                            payments.payment = models.payment;
                            payments.remark = models.remark;
                        }
                    }
                    else
                    {
                        //Save
                        db.payments.Add(models);
                    }
                    db.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpGet]
        [LoginAuthorize(RoleNo = "Admin")]
        public ActionResult Delete(int id)
        {
            using (dbcon db = new dbcon())
            {
                var model = db.payments.Where(m => m.rowid == id).FirstOrDefault();
                if (model != null)
                {
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        [LoginAuthorize(RoleNo = "Admin")]
        public ActionResult DeleteData(int id)
        {
            bool status = false;
            using (dbcon db = new dbcon())
            {
                var model = db.payments.Where(m => m.rowid == id).FirstOrDefault();
                if (model != null)
                {
                    db.payments.Remove(model);
                    db.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }
    }
}