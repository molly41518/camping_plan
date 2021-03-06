﻿using System;
using System.Collections.Generic;
using System.IO;
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
        public ActionResult Index(int id)
        {
            using (dbcon db = new dbcon())
            {
                Shop.Pno = db.product.Where(p => p.rowid == id).FirstOrDefault().pno;
                return View();
            }

        }

        public ActionResult GetDataList()
        {
            using (dbcon db = new dbcon())
            {
                string str_pno = Shop.Pno.ToString();
                var models = db.product_typedetail
                    .Where(m => m.pno == str_pno)
                    .OrderBy(m => m.pno).ToList();
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { data = models }), "application/json");

            }
        }

        [HttpGet]
        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Edit(int id)
        {
            using (dbcon db = new dbcon())
            {
                var ptype_models = db.product_typedetail.Where(p => p.rowid == id).FirstOrDefault();
                if (ptype_models == null)
                {
                    product_typedetail new_model = new product_typedetail();
                    return View(new_model);
                }
                var models = db.product_typedetail.Where(m => m.ptype_no == ptype_models.ptype_no).FirstOrDefault();
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
                    if (models.rowid > 0)
                    {
                        //Edit 
                        var product_typedetail = db.product_typedetail.Where(m => m.ptype_no == models.ptype_no).FirstOrDefault();
                        if (product_typedetail != null)
                        {
                            product_typedetail.pno = Shop.Pno;
                            product_typedetail.parea_name = models.parea_name;
                            product_typedetail.ptype_no = models.ptype_no;
                            product_typedetail.ptype_name = models.ptype_name;
                            product_typedetail.ptype_price = models.ptype_price;
                            product_typedetail.remark = models.remark;
                        }
                    }
                    else
                    {
                        //Save
                        models.pno = Shop.Pno;
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
        public ActionResult Delete(int id)
        {
            using (dbcon db = new dbcon())
            {
                var model = db.product_typedetail.Where(m => m.rowid == id).FirstOrDefault();
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
                var model = db.product_typedetail.Where(m => m.rowid == id).FirstOrDefault();
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
                string str_pno = Shop.Pno;
                var model = db.product.Where(m => m.pno == str_pno).FirstOrDefault();
                if (model != null)
                {
                    str_pno = model.pno.FirstOrDefault().ToString();
                }
                return RedirectToAction("Index", "Product", new { id = model.rowid });
            }
        }

        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Upload(int id = 0)
        {
            using (dbcon db = new dbcon())
            {
                var model = db.product_typedetail.Where(m => m.rowid == id).FirstOrDefault();
                if (model != null)
                {
                    Shop.ProductNo = model.pno;
                    Shop.ProductTypeNo = model.ptype_no;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        [HttpPost]
        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    string str_folder = string.Format("~/Content/images/product/{0}/product_type_detail/{1}", Shop.ProductNo, Shop.ProductTypeNo);
                    string str_folder_path = Server.MapPath(str_folder);
                    if (!Directory.Exists(str_folder_path)) Directory.CreateDirectory(str_folder_path);
                    string str_file_name = Shop.ProductTypeNo + ".jpg";
                    var path = Path.Combine(str_folder_path, str_file_name);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                    file.SaveAs(path);
                }
            }
            using (dbcon db = new dbcon())
            {
                var pno = Shop.ProductNo;
                var rowid = db.product.Where(p => p.pno == pno).SingleOrDefault().rowid;
                return RedirectToAction("Index", "ProductTypeDetail", new { id = rowid });
            }
        }



        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Pdescription(int id)
        {
            using (dbcon db = new dbcon())
            {
                var model = db.product_typedetail.Where(m => m.rowid == id).FirstOrDefault();
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
        [LoginAuthorize(RoleNo = "Vendor")]
        [ValidateInput(false)]
        public ActionResult Pdescription(product_typedetail product_Typedetail)
        {
            bool status = false;
            using (dbcon db = new dbcon())
            {
                var model = db.product_typedetail.Where(m => m.rowid == product_Typedetail.rowid).FirstOrDefault();
                if (model != null)
                {
                    model.ptype_dep = product_Typedetail.ptype_dep;
                    db.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadImage(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string url; // url to return
            string message; // message to display (optional)

            // 設定圖片上傳路徑
            string path = Server.MapPath("~/Images/uploads");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path = System.IO.Path.Combine(path, Shop.ProductNo);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            string ImageName = upload.FileName;
            string str_file_name = System.IO.Path.Combine(path, ImageName);
            if (System.IO.File.Exists(str_file_name)) System.IO.File.Delete(str_file_name);
            upload.SaveAs(str_file_name);


            // 取得網址
            // http://localhost:9999/Images/uploads/00001/ImageName.jpg
            url = Request.Url.GetLeftPart(UriPartial.Authority) + "/Images/uploads/" + Shop.ProductNo + "/" + Shop.ProductTypeNo + "/" + ImageName;

            // passing message success/failure
            message = "圖片儲存成功!!";

            // since it is an ajax request it requires this string
            string output = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + url + "\", \"" + message + "\");</script></body></html>";
            return Content(output);
        }
    }
}
