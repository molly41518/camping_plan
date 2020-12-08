using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using campingplan.App_Class;
using campingplan.Models;

namespace campingplan.Areas.Vendor.Controllers
{
    public class ProductTypeDetailController : Controller
    {
        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Index(string id)
        {
            Shop.ParentNo = id;
            return View();
        }

        public ActionResult GetDataList()
        {
            using (dbcon db = new dbcon())
            {
                var models = db.shippings.OrderBy(m => m.shipping_no).ToList();
                return Json(new { data = models }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Edit(int id = 0)
        {
            using (dbcon db = new dbcon())
            {
                if (id == 0)
                {
                    shippings new_model = new shippings();
                    return View(new_model);
                }
                var models = db.shippings.Where(m => m.rowid == id).FirstOrDefault();
                return View(models);
            }
        }

        [HttpPost]
        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Edit(shippings models)
        {
            bool status = false;
            if (ModelState.IsValid)
            {
                using (dbcon db = new dbcon())
                {
                    if (models.rowid > 0)
                    {
                        //Edit 
                        var shippings = db.shippings.Where(m => m.rowid == models.rowid).FirstOrDefault();
                        if (shippings != null)
                        {
                            shippings.shipping_no = models.shipping_no;
                            shippings.shipping_name = models.shipping_name;
                            shippings.remark = models.remark;
                        }
                    }
                    else
                    {
                        //Save
                        db.shippings.Add(models);
                    }
                    db.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpGet]
        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Delete(int id)
        {
            using (dbcon db = new dbcon())
            {
                var model = db.shippings.Where(m => m.rowid == id).FirstOrDefault();
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
        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult DeleteData(int id)
        {
            bool status = false;
            using (dbcon db = new dbcon())
            {
                var model = db.shippings.Where(m => m.rowid == id).FirstOrDefault();
                if (model != null)
                {
                    db.shippings.Remove(model);
                    db.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }
    }
}