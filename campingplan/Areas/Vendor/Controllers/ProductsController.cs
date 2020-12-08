using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using campingplan.App_Class;
using campingplan.Models;

namespace campingplan.Areas.Vendor.Controllers
{
    public class ProductsController : Controller
    {
        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Index()
        {
            return View();
        }

        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult GetDataList()
        {
            using (dbcon db = new dbcon())
            {
                var models = db.product
                   .Where(m => m.vendor_no == UserAccount.UserOfAccount)
                   .OrderBy(m => m.pno)
                   .ToList();
                if (models.Count > 0)
                {
                    for (int i = 0; i < models.Count; i++)
                    {
                        models[i].bool_istop = (models[i].istop == 1);
                        models[i].bool_issale = (models[i].issale == 1);
                        models[i].bool_iscolor = (models[i].iscolor == 1);
                        models[i].bool_issize = (models[i].issize == 1);
                    }
                }
                return Json(new { data = models }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Edit(int id = 0)
        {
            using (dbcon db = new dbcon())
            {
                var models = db.product.Where(m => m.rowid == id).FirstOrDefault();

                //Category DropDownList
                string str_rowid = "0";
                var categoryList = new List<SelectListItem>();
                List<categorys> clists = db.categorys.OrderBy(m => m.category_no).ToList();
                foreach (var item in clists)
                {
                    SelectListItem list = new SelectListItem();
                    list.Value = item.rowid.ToString();
                    list.Text = Shop.GetCategoryName(item.rowid.ToString());
                    categoryList.Add(list);
                    if (id == 0)
                    { if (str_rowid == "0") str_rowid = item.rowid.ToString(); }
                }

                if (models != null) str_rowid = models.categoryid.ToString();

                //預設選擇哪一筆
                categoryList.Where(m => m.Value == str_rowid).First().Selected = true;
                ViewBag.CategoryList = categoryList;

                if (id == 0)
                {
                    product new_model = new product();
                    new_model.size_name = "";
                    new_model.color_name = "";
                    new_model.remark = "";
                    new_model.start_count = 5;
                    new_model.browse_count = 0;
                    new_model.bool_istop = false;
                    new_model.bool_issale = true;
                    new_model.bool_iscolor = true;
                    new_model.bool_issize = true;
                    return View(new_model);
                }

                models.bool_istop = (models.istop == 1);
                models.bool_issale = (models.issale == 1);
                models.bool_iscolor = (models.iscolor == 1);
                models.bool_issize = (models.issize == 1);
                return View(models);
            }
        }

        [HttpPost]
        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Edit(product models)
        {
            bool status = false;
            if (ModelState.IsValid)
            {
                using (dbcon db = new dbcon())
                {
                    int int_cate_id = 0;
                    if (models.rowid > 0)
                    {
                        //Edit 
                        var product = db.product.Where(m => m.rowid == models.rowid).FirstOrDefault();
                        if (product != null)
                        {
                            int_cate_id = models.categoryid.GetValueOrDefault();
                            product.pno = models.pno;//有疑慮
                            product.pname = models.pname;//有疑慮
                            product.pdescription = models.pdescription;
                            product.categoryid = int_cate_id;
                            product.category_name = Shop.GetCategoryName(int_cate_id.ToString());
                            product.istop = (models.bool_istop) ? 1 : 0;
                            product.issale = (models.bool_issale) ? 1 : 0;
                            product.issize = (models.bool_issize) ? 1 : 0;
                            product.iscolor = (models.bool_iscolor) ? 1 : 0;
                            product.start_count = models.start_count;
                            product.browse_count = models.browse_count;
                            product.vendor_no = UserAccount.UserOfAccount;
                            product.color_name = models.color_name;
                            product.size_name = models.size_name;
                            product.remark = models.remark;
                        }
                    }
                    else
                    {
                        //Save
                        models.vendor_no = UserAccount.UserOfAccount;
                        int_cate_id = models.categoryid.GetValueOrDefault();
                        models.category_name = Shop.GetCategoryName(int_cate_id.ToString());
                        models.istop = (models.bool_istop) ? 1 : 0;
                        models.issale = (models.bool_issale) ? 1 : 0;
                        models.issize = (models.bool_issize) ? 1 : 0;
                        models.iscolor = (models.bool_iscolor) ? 1 : 0;
                        db.product.Add(models);
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
                var model = db.product.Where(m => m.rowid == id).FirstOrDefault();
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
                var model = db.product.Where(m => m.rowid == id).FirstOrDefault();
                if (model != null)
                {
                    db.product.Remove(model);
                    db.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }

        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Upload(int id = 0)
        {
            using (dbcon db = new dbcon())
            {
                var model = db.product.Where(m => m.rowid == id).FirstOrDefault();
                if (model != null)
                {
                    Shop.ProductNo = model.pno;
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
                    string str_folder = string.Format("~/Images/product/{0}", Shop.ProductNo);
                    string str_folder_path = Server.MapPath(str_folder);
                    if (!Directory.Exists(str_folder_path)) Directory.CreateDirectory(str_folder_path);
                    string str_file_name = Shop.ProductNo + ".jpg";
                    var path = Path.Combine(str_folder_path, str_file_name);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                    file.SaveAs(path);
                }
            }
            return RedirectToAction("Index");
        }

        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Remark(int id = 0)
        {
            using (dbcon db = new dbcon())
            {
                var model = db.product.Where(m => m.rowid == id).FirstOrDefault();
                if (model != null)
                {
                    Shop.ProductNo = model.pno;
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
        public ActionResult Remark(product product)
        {
            bool status = false;
            using (dbcon db = new dbcon())
            {
                var model = db.product.Where(m => m.rowid == product.rowid).FirstOrDefault();
                if (model != null)
                {
                    model.remark = product.remark;
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
            url = Request.Url.GetLeftPart(UriPartial.Authority) + "/Images/uploads/" + Shop.ProductNo + "/" + ImageName;

            // passing message success/failure
            message = "圖片儲存成功!!";

            // since it is an ajax request it requires this string
            string output = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + url + "\", \"" + message + "\");</script></body></html>";
            return Content(output);
        }
    }
}