using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using campingplan.Models;

namespace campingplan.App_Class
{
    public class Shop
    {
        public static List<categorys> Getategorys(int id)
        {
            dbcon db = new dbcon();
            var lists = db.categorys.Where(m => m.parentid == id).OrderBy(m => m.category_no).ToList();
            return lists;


        }

        public static string GetCategoryName(string cat_no,ref int cat_id)
        {
            cat_id = 0;
            string str_name = "";
            using (dbcon db = new dbcon())
            {
                var model = db.categorys.Where(m => m.category_no == cat_no).FirstOrDefault();
                if(model != null)
                {
                    str_name = model.category_name;
                    cat_id = model.rowid;
                }
            }
            return str_name;
        }
        public static List<product_typedetail> Postproduct_typedetail(string id)
        {
            dbcon db = new dbcon();
            var lists = db.product_typedetail.Where(m => m.pno == id).OrderBy(m => m.ptype_no).ToList();
            return lists;
        }
    }
    
}