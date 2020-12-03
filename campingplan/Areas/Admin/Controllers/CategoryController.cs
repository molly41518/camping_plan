﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using campingplan.App_Class;
using campingplan.Models;

namespace campingplan.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        [LoginAuthorize(RoleNo = "Admin")]
        public ActionResult Index(int id = 0)
        {
            SetSessionValue(id);
            return View();
        }

        public ActionResult GetDataList()
        {
            using (dbcon db = new dbcon())
            {
                int int_pid = int.Parse(Session["CategoryID"].ToString());
                var models = db.categorys
                    .Where(m => m.parentid == int_pid)
                    .OrderBy(m => m.category_no).ToList();
                return Json(new { data = models }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [LoginAuthorize(RoleNo = "Admin")]
        //此為新增商品
        public ActionResult Edit(int id = 0)
        {
            using (dbcon db = new dbcon())
            {
                if (id == 0)
                {
                    categorys new_model = new categorys();
                    return View(new_model);
                }
                var models = db.categorys.Where(m => m.rowid == id).FirstOrDefault();
                return View(models);
            }
        }

        [HttpPost]
        [LoginAuthorize(RoleNo = "Admin")]
        public ActionResult Edit(categorys models)
        {
            bool status = false;
            if (ModelState.IsValid)
            {
                using (dbcon db = new dbcon())
                {
                    if (models.rowid > 0)
                    {
                        //Edit 
                        var Categorys = db.categorys.Where(m => m.rowid == models.rowid).FirstOrDefault();
                        if (Categorys != null)
                        {
                            Categorys.category_no = models.category_no;
                            Categorys.category_name = models.category_name;
                        }
                    }
                    else
                    {
                        //新增嗎????
                        //Save
                        if (Session["CategoryID"] == null)
                        { models.parentid = 0; }
                        else
                        { models.parentid = int.Parse(Session["CategoryID"].ToString()); }
                        db.categorys.Add(models);
                    }
                    db.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpGet]
        [LoginAuthorize(RoleNo = "Admin")]
        public ActionResult Delete(int id)
        {
            using (dbcon db = new dbcon())
            {
                var model = db.categorys.Where(m => m.rowid == id).FirstOrDefault();
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
        [LoginAuthorize(RoleNo = "Admin")]
        public ActionResult DeleteData(int id)
        {
            bool status = false;
            using (dbcon db = new dbcon())
            {
                var model = db.categorys.Where(m => m.rowid == id).FirstOrDefault();
                if (model != null)
                {
                    db.categorys.Remove(model);
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
                int int_pid = int.Parse(Session["CategoryID"].ToString());
                var model = db.categorys.Where(m => m.rowid == int_pid).FirstOrDefault();
                if (model != null)
                {
                    int_pid = model.parentid.GetValueOrDefault();
                }
                return RedirectToAction("Index", "Category", new { id = int_pid });
            }
        }

        private void SetSessionValue(int id = 0)
        {
            Session["CategoryID"] = id;
            Session["CategoryNo"] = "";
            Session["CategoryName"] = "";
            Session["ParentID"] = "";
            Session["ParentName"] = "";
            int int_pid = 0;
            string str_parent_id = id.ToString();
            string str_parent_name = "";
            using (dbcon db = new dbcon())
            {
                var model = db.categorys.Where(m => m.rowid == id).FirstOrDefault();
                if (model != null)
                {
                    Session["CategoryNo"] = model.category_no;
                    Session["CategoryName"] = model.category_name;
                    int_pid = model.parentid.GetValueOrDefault();
                    str_parent_name = model.category_name;

                    while (int_pid > 0)
                    {
                        var parent = db.categorys.Where(m => m.rowid == int_pid).FirstOrDefault();
                        if (parent == null) break;

                        int_pid = parent.parentid.GetValueOrDefault();
                        str_parent_id = parent.rowid.ToString() + "," + str_parent_id;
                        str_parent_name = parent.category_name + "," + str_parent_name;
                    }
                }
            }
            Session["ParentID"] = str_parent_id;
            Session["ParentName"] = str_parent_name;
        }
    }
}