using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using campingplan.App_Class;
using campingplan.Models;

namespace campingplan.Areas.Admin.Controllers
{
    public class CompanyController : Controller
    {
        [LoginAuthorize(RoleNo = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDataList()
        {
            using (dbcon db = new dbcon())
            {
                var models = db.companys.OrderBy(m => m.mno).ToList();
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
                    companys new_model = new companys();
                    return View(new_model);
                }
                var models = db.companys.Where(m => m.rowid == id).FirstOrDefault();
                return View(models);
            }
        }

        [HttpPost]
        [LoginAuthorize(RoleNo = "Admin")]
        public ActionResult Edit(companys models)
        {
            bool status = false;
            if (ModelState.IsValid)
            {
                using (dbcon db = new dbcon())
                {
                    if (models.rowid > 0)
                    {
                        //Edit 
                        var Companys = db.companys.Where(m => m.rowid == models.rowid).FirstOrDefault();
                        if (Companys != null)
                        {
                            Companys.mno = models.mno;
                            Companys.mname = models.mname;
                            Companys.remark = models.remark;
                        }
                    }
                    else
                    {
                        //Save
                        db.companys.Add(models);
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
                var model = db.companys.Where(m => m.rowid == id).FirstOrDefault();
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
                var model = db.companys.Where(m => m.rowid == id).FirstOrDefault();
                if (model != null)
                {
                    db.companys.Remove(model);
                    db.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }
    }
}