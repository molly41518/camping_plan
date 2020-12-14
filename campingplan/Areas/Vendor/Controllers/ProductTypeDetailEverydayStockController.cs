using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using campingplan.App_Class;
using campingplan.Models;

namespace campingplan.Areas.Vendor.Controllers
{
    public class ProductTypeDetailEverydayStockController : Controller
    {
        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Index(int id)
        {
            using (dbcon db = new dbcon())
            {
                var ptype = db.product_typedetail.Where(p => p.rowid == id).FirstOrDefault();
                Shop.ProductTypeNo = ptype.ptype_no;
                ViewBag.p_rowid = ptype.product.rowid;
                return View();
            }

        }

        public ActionResult GetDataList()
        {
            using (dbcon db = new dbcon())
            {
                string str_ptype_no = Shop.ProductTypeNo.ToString();
                var models = db.product_typedetail_everydaystock
                    .Where(m => m.ptype_no == str_ptype_no)
                    .OrderBy(m => m.ptype_no).ToList();
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { data = models }), "application/json");
            }
        }

        [HttpGet]
        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Create()
        {
            using (dbcon db = new dbcon())
            {
                ViewBag.ptype_no = Shop.ProductTypeNo;
                return View();
            }
        }


        [HttpPost]
        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Create(string id)
        {
            using (dbcon db = new dbcon())
            {
                bool added = false;
                string DayValueListStr = Request.Form["day_value_list"];
                if (!string.IsNullOrEmpty(DayValueListStr))
                {
                    var DayValueList = DayValueListStr.Split(',');
                    foreach (var dvPair in DayValueList)
                    {
                        DateTime date = Convert.ToDateTime(dvPair.Split(':')[0]);
                        int num = Convert.ToInt32(dvPair.Split(':')[1]);
                        bool hasStock = db.product_typedetail_everydaystock.Count(e => e.ptype_no == id && e.stock_date == date) != 0;
                        if (hasStock)
                        {
                            db.product_typedetail_everydaystock.SingleOrDefault(e => e.ptype_no == id && e.stock_date == date).stock = num;
                        }
                        else
                        {
                            product_typedetail_everydaystock record = new product_typedetail_everydaystock()
                            {
                                ptype_no = id,
                                stock_date = date,
                                stock = num
                            };
                            db.product_typedetail_everydaystock.Add(record);
                        }
                    }
                    db.SaveChanges();
                    added = true;
                }
                return new JsonResult { Data = new { status = added } };
            }
        }

        [HttpGet]
        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Edit(int id)
        {
            using (dbcon db = new dbcon())
            {
                var ptype_everyday_stock_models = db.product_typedetail_everydaystock.Where(p => p.rowid == id).FirstOrDefault();
                if (ptype_everyday_stock_models == null)
                {
                    product_typedetail_everydaystock new_model = new product_typedetail_everydaystock();
                    return View(new_model);
                }
                return View(ptype_everyday_stock_models);
            }
        }

        [HttpPost]
        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Edit(product_typedetail_everydaystock models)
        {
            bool status = false;
            if (ModelState.IsValid)
            {
                using (dbcon db = new dbcon())
                {
                    if (models.rowid > 0)
                    {
                        //Edit 
                        var ptype_everydaystock = db.product_typedetail_everydaystock.Where(m => m.rowid == models.rowid).FirstOrDefault();
                        if (ptype_everydaystock != null)
                        {
                            ptype_everydaystock.stock = models.stock;
                            ptype_everydaystock.stock_date = models.stock_date;
                        }
                    }
                    else
                    {
                        //Save
                        models.ptype_no = Shop.ProductTypeNo;
                        db.product_typedetail_everydaystock.Add(models);
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
                var model = db.product_typedetail_everydaystock.Where(m => m.rowid == id).FirstOrDefault();
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
                var model = db.product_typedetail_everydaystock.Where(m => m.rowid == id).FirstOrDefault();
                if (model != null)
                {
                    db.product_typedetail_everydaystock.Remove(model);
                    db.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }
    }
}
