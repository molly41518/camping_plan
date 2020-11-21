using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using campingplan.Models;

namespace campingplan.Controllers
{
    public class BackProdutController : Controller
    {
        dbcon db = new dbcon();
        // GET: BackProdut
        
        //列出商品列表
        public ActionResult BackProdutList()
        {
            var model = db.product.OrderBy(m => m.pno).ToList();
            return View(model);
        }

        //刪除商品資料
        public ActionResult BackProdutDelect(int pr_rowid)
        {
            var model = db.product.FirstOrDefault(m => m.rowid == pr_rowid);
            db.product.Remove(model);
            db.SaveChanges();
            return RedirectToAction("BackProdutList");
        }

        //新增商品資料
        [HttpGet]
        public ActionResult BackProdutInsert()
        {
            product model = new product();
            return View(model);
        }


        [HttpPost]
        public ActionResult BackProdutInsert(product model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool bln_error = false;
            var product = db.product.Where(m => m.pno == model.pno).FirstOrDefault();
            if (product != null) { ModelState.AddModelError("", "商品編號重複"); bln_error = true; }
            if (bln_error) return View(model);

            db.Configuration.ValidateOnSaveEnabled = false;
            db.product.Add(model);
            db.SaveChanges();
            return RedirectToAction("BackProdutList");
        }


    }
}