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
        public static Dictionary<string, List<product_typedetail>> GetDictProductTypeDetail(ICollection<product_typedetail> product_detail_list)
        {
            Dictionary<string, List<product_typedetail>> output = new Dictionary<string, List<product_typedetail>>();
            foreach (var p in product_detail_list)
            {
                bool is_output_has_area = output.ContainsKey(p.parea_name);
                if (!is_output_has_area) {
                    List<product_typedetail> newList = new List<product_typedetail>();
                    output.Add(p.parea_name, newList);
                }
                output[p.parea_name].Add(p); // Add p
            }
            return output;
        }
        public static KeyValuePair<string, string> GetProductFeature(product_features features)
        {
            KeyValuePair<string, string> output = new KeyValuePair<string, string>();
            return output;
        }
    }
    
}