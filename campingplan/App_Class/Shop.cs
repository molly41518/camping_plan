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

        public static string GetCategoryName(string cat_no, ref int cat_id)
        {
            cat_id = 0;
            string str_name = "";
            using (dbcon db = new dbcon())
            {
                var model = db.categorys.Where(m => m.category_no == cat_no).FirstOrDefault();
                if (model != null)
                {
                    str_name = model.category_name;
                    cat_id = model.rowid;
                }
            }
            return str_name;
        }

        public static string GetCategoryName(string pno)
        {
            string str_name = "";
            using (dbcon db = new dbcon())
            {
                var product_model = db.product.Where(q => q.pno == pno).FirstOrDefault();
                if (product_model != null)
                {
                    var model = db.categorys.Where(w => w.rowid == product_model.categoryid).FirstOrDefault();
                    if (model != null)
                    {
                        str_name = model.category_name;
                    }
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
                if (!is_output_has_area)
                {
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

        public static int GetProductPrice(string productNo)
        {
            int int_price = 0;
            using (dbcon db = new dbcon())
            {
                var models = db.product_typedetail.Where(m => m.ptype_no == productNo).FirstOrDefault();
                if (models != null)
                {
                    if (models.ptype_price != null)
                        int.TryParse(models.ptype_price.ToString(), out int_price);
                }
            }
            return int_price;
        }

        public static string GetProductName(string productNo)
        {
            string str_name = "";
            using (dbcon db = new dbcon())
            {
                var models = db.product.Where(m => m.pno == productNo).FirstOrDefault();
                if (models != null) str_name = models.pname;
            }
            return str_name;
        }

        public static string GetProductTypeName(string get_ptype_no)
        {
            string str_name = "";
            using (dbcon db = new dbcon())
            {
                var models = db.product_typedetail.Where(m => m.ptype_no == get_ptype_no).FirstOrDefault();
                if (models != null) str_name = models.ptype_name;
            }
            return str_name;
        }

        public static string GetPaymentName(string get_payment_no)
        {
            string str_name = "";
            using (dbcon db = new dbcon())
            {
                var models = db.payments.Where(m => m.payment_no == get_payment_no).FirstOrDefault();
                if (models != null) str_name = models.payment;
            }
            return str_name;
        }


        public static string GetVendorNoByProduct(string productNo)
        {
            string str_no = "";
            using (dbcon db = new dbcon())
            {
                var prod = db.product.Where(m => m.pno == productNo).FirstOrDefault();
                if (prod != null) str_no = prod.pno;
            }
            return str_no;
        }



        public static string ProductTypeNo
        {
            get { return (HttpContext.Current.Session["ProductNo"] == null) ? "" : HttpContext.Current.Session["ProductNo"].ToString(); }
            set { HttpContext.Current.Session["ProductNo"] = value; }
        }


    }

}