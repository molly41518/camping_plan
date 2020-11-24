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
            ViewBag.detail = new product_typedetail();
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
            
            DateTime value = DateTime.Now;
            model.psetdate = value;
            db.Configuration.ValidateOnSaveEnabled = false;
            db.product.Add(model);
            db.SaveChanges();
            return RedirectToAction("BackProdutList");
        }

        //編輯商品
        public ActionResult BackProductEdit(int pr_rowid)
        {
            var model = db.product.FirstOrDefault(m => m.rowid == pr_rowid);
            if (model != null)
            {
                return View(model);
            }
            return RedirectToAction("BackProdutList");

        }

        [HttpPost]
        public ActionResult BackProductEdit(product model)
        {
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var product = db.product.Where(m => m.rowid == model.rowid).FirstOrDefault();
            db.Configuration.ValidateOnSaveEnabled = false;
            product.pno = model.pno;
            product.plocation = model.plocation;
            product.pname = model.pname;
            product.pimg = model.pimg;
            product.pdescription = model.pdescription;
            db.SaveChanges();
            return RedirectToAction("BackProdutList");

        }



    }
}