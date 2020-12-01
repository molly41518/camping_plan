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

        public JsonResult GetStockEvent(string id, DateTime start, DateTime end)
        {
            var relayStock = db.product_typedetail_everydaystock.Where(s => s.product_typedetail.product.pno == id)
                .Where(s => s.stock_date >= start && s.stock_date < end);
            Dictionary<string, int> stockNum = new Dictionary<string, int>();
            foreach (var s in relayStock)
            {
                string dateStr = s.stock_date.GetValueOrDefault().ToString("yyyy-MM-dd");
                if (!stockNum.ContainsKey(dateStr))
                {
                    stockNum[dateStr] = 0;
                }
                stockNum[dateStr] += s.stock.GetValueOrDefault();
            }
            List<Dictionary<string, string>> output = new List<Dictionary<string, string>>();
            foreach (var kv in stockNum)
            {
                output.Add(new Dictionary<string, string>(){
                    { "id", "id-event-" + kv.Key },
                    { "start", kv.Key },
                    { "title", kv.Value.ToString() },
                    { "allDay", "true" }
                });
            }
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        // GET: Product
        public ActionResult CategoryList(string id)
        {
            int int_id = 0;
            ViewBag.CategoryNo = id;
            ViewBag.CategoryName = Shop.GetCategoryName(id, ref int_id);
            var relayModel = db.product.Where(m => m.categoryid == int_id);

            //關鍵字搜尋
            string searchwords = Request.Form["searchString"];
            if (searchwords != null)
            {
                ViewBag.SearchKeywordProductList = Shop.GetCategoryName(id, ref int_id);
                relayModel = relayModel.Where(m => m.pname.Contains(searchwords));
            }

            // 日期搜索
            string dateSearch = Request.Form["dateSearch"];
            if (dateSearch != null)
            {
                DateTime dateDT = Convert.ToDateTime(dateSearch);
                int num = 2;
                relayModel = relayModel.Where(m => m.product_typedetail.Any(t => t.product_typedetail_everydaystock.Any(s => s.stock_date == dateDT && s.stock >= num)));
            }

            var model = relayModel.OrderBy(m => m.pno).ToList();
            return View(model);
        }

        public ActionResult ProductDetail(string id)
        {
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