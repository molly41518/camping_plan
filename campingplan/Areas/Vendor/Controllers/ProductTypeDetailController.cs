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
        public ActionResult Index(string pno)
        {
            Session["Pno"] = pno;
            return View();
        }

        public ActionResult GetDataList()
        {
            using (dbcon db = new dbcon())
            {
                string str_pno = Session["Pno"].ToString();
                var models = db.product_typedetail
                    .Where(m => m.pno == str_pno)
                    .OrderBy(m => m.pno).ToList();
                return Json(new { data = models }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Edit(string pno )
        {
            using (dbcon db = new dbcon())
            {
                if (pno == null)
                {
                    product_typedetail new_model = new product_typedetail();
                    return View(new_model);
                }
                var models = db.product_typedetail.Where(m => m.pno == pno).FirstOrDefault();
                return View(models);
            }
        }

        [HttpPost]
        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Edit(product_typedetail models)
        {
            bool status = false;
            if (ModelState.IsValid)
            {
                using (dbcon db = new dbcon())
                {
                    if (models.pno != null)
                    {
                        //Edit 
                        var product_typedetail = db.product_typedetail.Where(m => m.pno == models.pno).FirstOrDefault();
                        if (product_typedetail != null)
                        {
                            product_typedetail.pno = models.pno;
                            product_typedetail.parea_name = models.parea_name;
                        }
                    }
                    else
                    {
                        //Save
                        db.product_typedetail.Add(models);
                    }
                    db.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpGet]
        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Delete(string pno)
        {
            using (dbcon db = new dbcon())
            {
                var model = db.product_typedetail.Where(m => m.pno == pno).FirstOrDefault();
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
        public ActionResult DeleteData(string pno)
        {
            bool status = false;
            using (dbcon db = new dbcon())
            {
                var model = db.product_typedetail.Where(m => m.pno == pno).FirstOrDefault();
                if (model != null)
                {
                    db.product_typedetail.Remove(model);
                    db.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }

        public ActionResult ReturnToParent()
        {
            using (dbcon db = new dbcon())
            {
                string str_pno = Session["Pno"].ToString();
                var model = db.product.Where(m => m.pno == str_pno).FirstOrDefault();
                if (model != null)
                {
                    str_pno = model.pno.FirstOrDefault().ToString();
                }
                return RedirectToAction("Index", "Category", new { id = str_pno });
            }
        }
    }
}