using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using campingplan.App_Class;
using campingplan.Models;

namespace campingplan.Controllers
{
    public class ProductController : Controller
    {
        dbcon db = new dbcon();
        // GET: Product
        public ActionResult CategoryList(string id)
        {
            int int_id = 0;
            ViewBag.CategoryNo = id;
            ViewBag.CategoryName = Shop.GetCategoryName(id, ref int_id);
            var model = db.product.Where(m => m.categoryid == int_id).OrderBy(m => m.pno).ToList();
            return View(model);
        }
    }
}