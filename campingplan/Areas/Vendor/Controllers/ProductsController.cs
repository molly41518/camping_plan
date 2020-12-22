using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using campingplan.App_Class;
using campingplan.Models;
using System.Web.Script.Serialization;
using System.Net.Http.Formatting;

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
                    }
                }


                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { data = models }), "application/json");
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
                List<categorys> clists = db.categorys.Where(m => m.parentid == 0).OrderBy(m => m.rowid).ToList();
                foreach (var item in clists)
                {
                    SelectListItem list = new SelectListItem
                    {
                        Value = item.rowid.ToString(),
                        Text = item.category_name
                    };
                    categoryList.Add(list);
                    if (id == 0)
                    { if (str_rowid == "0") str_rowid = item.rowid.ToString(); }
                }

                if (models != null) str_rowid = models.categoryid.ToString();

                //預設選擇哪一筆
                categoryList.First(m => m.Value == str_rowid).Selected = true;
                ViewBag.CategoryList = categoryList;

                var locationTypeList = new List<SelectListItem>();
                List<product_features_type> localtions = db.product_features_type.Where(f => f.features_parents_id == 1).ToList();
                foreach (var l in localtions)
                {
                    SelectListItem list = new SelectListItem
                    {
                        Value = l.rowid.ToString(),
                        Text = l.features_name
                    };
                    locationTypeList.Add(list);
                }
                ViewBag.LocationTypeList = locationTypeList;

                if (id == 0)
                {
                    product new_model = new product
                    {
                        remark = "",
                        start_count = 5,
                        browse_count = 0,
                        product_features = new product_features()
                    };
                    return View(new_model);
                }
                Console.WriteLine(models.product_features.near_river);
                Console.WriteLine(models.product_typedetail.Count);
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
                            product.categoryid = int_cate_id;
                            product.category_name = Shop.GetCategoryNameByid(int_cate_id);
                            product.istop = (models.bool_istop) ? 1 : 0;
                            product.issale = (models.bool_issale) ? 1 : 0;
                            product.start_count = models.start_count;
                            product.browse_count = models.browse_count;
                            product.plocation = models.plocation;
                            product.pmapurl = models.pmapurl;
                            product.vendor_no = UserAccount.UserOfAccount;
                            product.remark = models.remark;
                            var feature = db.product_features.Where(f => f.pno == models.pno).FirstOrDefault();
                            if (feature != null)
                            {
                                feature.location_type = models.product_features.location_type;
                                feature.near_river = models.product_features.near_river;
                                feature.near_sea = models.product_features.near_sea;
                                feature.no_tent = models.product_features.no_tent;
                                feature.have_canopy = models.product_features.have_canopy;
                                feature.have_clouds = models.product_features.have_clouds;
                                feature.have_firefly = models.product_features.have_firefly;
                                feature.could_book_all = models.product_features.could_book_all;
                                feature.have_rental_equipment = models.product_features.have_rental_equipment;
                                feature.have_game_area = models.product_features.have_game_area;
                                feature.elevation_under_300m = models.product_features.elevation_under_300m;
                                feature.elevation_301m_to_500m = models.product_features.elevation_301m_to_500m;
                                feature.elevation_over_501m = models.product_features.elevation_over_501m;
                            }
                            else
                            {
                                db.product_features.Add(models.product_features);
                            }
                        }
                    }
                    else
                    {
                        //Save
                        models.vendor_no = UserAccount.UserOfAccount;
                        int_cate_id = models.categoryid.GetValueOrDefault();
                        models.category_name = Shop.GetCategoryNameByid(int_cate_id);
                        models.istop = (models.bool_istop) ? 1 : 0;
                        models.issale = (models.bool_issale) ? 1 : 0;
                        models.product_features.pno = models.pno;
                        models.product_features.product = models;
                        db.product.Add(models);
                        db.product_features.Add(models.product_features);
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
                    var pno = Shop.ProductNo;
                    using (dbcon db = new dbcon())
                    {
                        var product = db.product.Where(p => p.pno == pno).SingleOrDefault();
                        product.pimg = pno;
                        db.SaveChanges();
                    }
                    string str_folder = string.Format("~/Content/images/product/{0}", pno);
                    string str_folder_path = Server.MapPath(str_folder);
                    if (!Directory.Exists(str_folder_path)) Directory.CreateDirectory(str_folder_path);
                    string str_file_name = pno + ".jpg";
                    var path = Path.Combine(str_folder_path, str_file_name);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                    file.SaveAs(path);
                }
            }
            return RedirectToAction("Index");
        }

        [LoginAuthorize(RoleNo = "Vendor")]
        public ActionResult Pdescription(int id = 0)
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
        public ActionResult Pdescription(product product)
        {
            bool status = false;
            using (dbcon db = new dbcon())
            {
                var model = db.product.Where(m => m.rowid == product.rowid).FirstOrDefault();
                if (model != null)
                {
                    model.pdescription = product.pdescription;
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
