using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            //關鍵字搜尋
            string searchwords = Request.Form["searchString"];
            if (searchwords != null)
            {
                ViewBag.SearchKeywordProductList = Shop.GetCategoryName(id, ref int_id);
                model = db.product.Where(m => m.categoryid == int_id && m.pname.Contains(searchwords)).OrderBy(m => m.pno).ToList();
            }

            return View(model);
        }

        public ActionResult ProductDetail(string id)
        {
            if(id == null) id = "C0001";
            var modal = db.product.Where(m => m.pno == id).FirstOrDefault();
            var typedetail = db.product_typedetail.Where(m => m.pno == id).FirstOrDefault();

            return View(modal);
        }
        [HttpPost]
        public ActionResult ProductDetail(string qty, string camptype, string product_no)
        {
            string str_data = product_no + "" + qty + "" + camptype;
            return View();
        }

    }
}