﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using campingplan.App_Class;
using campingplan.Models;
using PagedList;

namespace campingplan.Controllers
{
    public class ProductController : Controller
    {
        dbcon db = new dbcon();

        public JsonResult GetStockEvent(string id, DateTime start, DateTime end, string ptype_no)
        {
            var relayStock = db.product_typedetail_everydaystock.Where(s => s.product_typedetail.product.pno == id)
                .Where(s => s.stock_date >= start && s.stock_date < end);
            if (!string.IsNullOrEmpty(ptype_no))
            {
                relayStock = relayStock.Where(e => e.ptype_no == ptype_no);
            }
            Dictionary<string, int> stockNum = new Dictionary<string, int>();
            foreach (var s in relayStock)
            {
                string dateStr = s.stock_date.GetValueOrDefault().ToString("yyyy-MM-dd");
                //stockNum.ContainsKey(dateStr)這個不懂
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

        private static void UpdateSearchInfo(ref NameValueCollection SearchInfo, NameValueCollection form, string key)
        {
            string value = form[key];
            if (!string.IsNullOrEmpty(value))
            {
                SearchInfo[key] = value;
            }
        }

        private void QueryBySearchInfo(ref NameValueCollection SearchInfo, ref IQueryable<product> relayModel)
        {
            int qty = Convert.ToInt32(SearchInfo["stock_qty"]);
            //關鍵字搜尋
            string searchwords = SearchInfo["searchString"];
            if (!string.IsNullOrEmpty(searchwords))
            {
                relayModel = relayModel.Where(m => m.pname.Contains(searchwords));
            }

            // 日期搜索
            string dateSearch = SearchInfo["dateSearch"];
            if (!string.IsNullOrEmpty(dateSearch))
            {
                var dateList = dateSearch.Split(new string[] { " to " }, StringSplitOptions.None);
                DateTime startday = Convert.ToDateTime(dateList[0]);
                DateTime endday = Convert.ToDateTime(dateList[1]);
                int days = new TimeSpan(endday.Ticks - startday.Ticks).Days;
                for (int i = 0; i < days; i++)
                {
                    DateTime tmpDay = startday.AddDays(i);
                    relayModel = relayModel.Where(m => m.product_typedetail
                        .Any(t => t.product_typedetail_everydaystock
                            .Any(s => s.stock_date == tmpDay && s.stock >= qty)));
                }
                ViewBag.startday = startday.Date;
                ViewBag.endday = endday.Date;
                ViewBag.qty = qty;
            }

            // 特徵搜索
            var featureEnToCHT = Shop.GetFeatureDict();
            foreach (var kv in Shop.product_feature_exp_to_string)
            {
                var result = SearchInfo[featureEnToCHT[kv.Value]];
                if (!string.IsNullOrEmpty(result) && result != "false")
                {
                    relayModel = relayModel.Where(kv.Key);
                }
            }

        }

        private NameValueCollection NewSearchInfo()
        {
            // 準備 SearchInfo
            NameValueCollection SearchInfo = new NameValueCollection();
            UpdateSearchInfo(ref SearchInfo, Request.Form, "stock_qty");
            UpdateSearchInfo(ref SearchInfo, Request.Form, "searchString");
            UpdateSearchInfo(ref SearchInfo, Request.Form, "dateSearch");
            var featureEnToCHT = Shop.GetFeatureDict();
            foreach (var kv in Shop.product_feature_exp_to_string)
            {
                UpdateSearchInfo(ref SearchInfo, Request.Form, featureEnToCHT[kv.Value]);
            }
            return SearchInfo;
        }

        private void UpdatePrice(ref NameValueCollection SearchInfo, ref IPagedList<product> model)
        {
            string dateSearch = SearchInfo["dateSearch"];
            int qty = Convert.ToInt32(SearchInfo["stock_qty"]);
            if (!string.IsNullOrEmpty(dateSearch))
            {
                foreach (var p in model)
                {
                    int price = 9999999;
                    foreach (var ptd in p.product_typedetail)
                    {
                        bool hasStock = true;
                        int days = new TimeSpan(ViewBag.endday.Ticks - ViewBag.startday.Ticks).Days;
                        for (int i = 0; i < days; i++)
                        {
                            DateTime tmpDay = ViewBag.startday.AddDays(i);
                            hasStock &= ptd.product_typedetail_everydaystock.Any(s => s.stock_date == tmpDay && s.stock >= qty);
                        }
                        if (hasStock)
                        {
                            price = Math.Min(price, ptd.ptype_price.GetValueOrDefault());
                        }
                    }
                    p.min_price = price;
                }
            }
        }

        public ActionResult CategoryList(string id, int page = 1, bool isNew = true)
        {
            int int_id = 0;
            ViewBag.CategoryNo = id;
            ViewBag.CategoryName = Shop.GetCategoryName(id, ref int_id);
            var relayModel = db.product.Where(m => m.categoryid == int_id);

            // 準備 SearchInfo
            if(!string.IsNullOrEmpty(Request.Form["newSearch"]) && Request.Form["newSearch"] == "false")
            {
                isNew = false;
            }
            if (isNew)
            {
                Shop.SearchInfo = NewSearchInfo();
            }
            NameValueCollection SearchInfo = Shop.SearchInfo;
            ViewBag.SearchKeywordProductList = Shop.GetCategoryName(id, ref int_id);

            UpdateSearchInfo(ref SearchInfo, Request.Form, "locationSearch");

            QueryBySearchInfo(ref SearchInfo, ref relayModel);

            // 地點搜尋
            string locationSearch = SearchInfo["locationSearch"];
            if (!string.IsNullOrEmpty(locationSearch) && locationSearch != "all")
            {
                var locationType = db.product_features_type.Where(pft => pft.features_parents_id == 1 && pft.features_member == locationSearch).SingleOrDefault().rowid;
                relayModel = relayModel.Where(m => m.product_features.location_type == locationType);
            }

            //分頁
            int pagesize = 6;
            int pagecurrent = page < 1 ? 1 : page;
            IPagedList<product> model = relayModel.OrderBy(m => m.pno).ToPagedList(pagecurrent, pagesize);

            // 更新價錢
            UpdatePrice(ref SearchInfo, ref model);

            return View(model);
        }

        [HttpPost]
        public ActionResult ProductDetail(string id)
        {
            DateTime startday = DateTime.Today;
            DateTime endday = startday.AddDays(1);
            int qty = Convert.ToInt32(Request.Form["stock_qty"]);

            string dateSearch = Request.Form["dateSearch"];
            if (!string.IsNullOrEmpty(dateSearch))
            {
                var dateList = dateSearch.Split(new string[] { " to " }, StringSplitOptions.None);
                startday = Convert.ToDateTime(dateList[0]);
                endday = Convert.ToDateTime(dateList[1]);
            } 
            return ProductDetail(id, startday, endday, qty);
        }

        public ActionResult ProductDetail(string id, DateTime? startday, DateTime? endday, int? qty)
        {
            var modal = db.product.Where(m => m.pno == id).FirstOrDefault();
            if (endday != null) ViewBag.endday = endday;
            if (startday != null) ViewBag.startday = startday;
            if (qty != null) ViewBag.qty = qty;
            Shop.ProductTypeNo = id;
            return View(modal);
        }

        [LoginAuthorize(RoleNo = "Guest,Member")]
        public ActionResult AddToCart(string pno, string ptype_no, DateTime startday, DateTime endday, int qty)
        {
            Cart.AddCart(pno, ptype_no, startday, endday, qty);
            return RedirectToAction("ProductDetail", "Product", new { id = Shop.ProductTypeNo });
        }


        [LoginAuthorize(RoleNo = "Guest,Member")]
        public ActionResult CartList()
        {
            using (dbcon db = new dbcon())
            {
                if (UserAccount.IsLogin)
                {
                    var data1 = db.carts
                        .Where(m => m.mno == UserAccount.UserNo)
                        .ToList();
                    return View(data1);
                }
                var data2 = db.carts
                   .Where(m => m.lot_no == Cart.LotNo)
                   .ToList();
                return View(data2);
            }
        }

        [LoginAuthorize(RoleNo = "Guest,Member")]
        public ActionResult CartDelete(int id)
        {
            var data = db.carts
                .Where(m => m.rowid == id)
                .FirstOrDefault();
            if (data != null)
            {
                db.carts.Remove(data);
                db.SaveChanges();
            }
            return RedirectToAction("CartList");
        }

        [LoginAuthorize(RoleNo = "Guest,Member")]
        public ActionResult CartPlus(int id)
        {
            var data = db.carts
                .Where(m => m.rowid == id)
                .FirstOrDefault();
            if (data != null)
            {
                data.ptype_qty += 1;
                data.amount = data.ptype_qty * data.ptype_price;
                db.SaveChanges();
            }
            return RedirectToAction("CartList");
        }

        [LoginAuthorize(RoleNo = "Guest,Member")]
        public ActionResult CartMinus(int id)
        {
            var data = db.carts
                .Where(m => m.rowid == id)
                .FirstOrDefault();
            if (data != null)
            {
                if (data.ptype_qty > 1)
                {
                    data.ptype_qty -= 1;
                    data.amount = data.ptype_qty * data.ptype_price;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("CartList");
        }

        [LoginAuthorize(RoleNo = "Member")]
        public ActionResult Checkout()
        {
                cvmOrder models = new cvmOrder()
                {
                    receive_name = "",
                    receive_memail = "",
                    receive_address = "",
                    payment_no = "01",
                    remark = "",
                    PaymentsList = db.payments.OrderBy(m => m.payment_no).ToList(),
                };

                return View(models);
           
        }

        [HttpPost]
        [LoginAuthorize(RoleNo = "Member")]
        public ActionResult Checkout(cvmOrder model)
        {
            if (!ModelState.IsValid)
            {
                if (model.PaymentsList == null)
                {
                    model.PaymentsList = db.payments.OrderBy(m => m.payment_no).ToList();
                }
                return View(model);
            }

            if (!Cart.CartPayment(model))
            {
                TempData["message"] = "營位不足, 請再次確認.";
                if (model.PaymentsList == null)
                {
                    model.PaymentsList = db.payments.OrderBy(m => m.payment_no).ToList();
                }
                return View(model);
            }

            return Redirect("~/ECPayment.aspx");
        }

        public ActionResult CheckoutReport()
        {
            return View();
        }

    }
}