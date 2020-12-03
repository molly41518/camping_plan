using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using campingplan.Models;
using System.Web.Configuration;

namespace campingplan.App_Class
{
    public class Shop
    {


        //在web.config中設定除錯模式<add key="DebugMode" value="1" /> 1為除錯 0為不除錯，發行時要改為0
        /// <summary>
        /// 除錯模式
        /// </summary>
        public static bool DebugMode { get { return (GetAppConfigValue("DebugMode") == "1"); } }


        /// <summary>
        /// 取得 Web.config 中的 App.Config 設定值
        /// </summary>
        /// <param name="keyName">Key 值</param>
        /// <returns></returns>
        private static string GetAppConfigValue(string keyName)
        {
            object obj_value = WebConfigurationManager.AppSettings[keyName];
            return (obj_value == null) ? "" : obj_value.ToString();
        }


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

        private static Dictionary<string, string> product_feature_dict = new Dictionary<string, string>();
        public static ref Dictionary<string, string> GetFeatureDict()
        {
            if (product_feature_dict.Count == 0)
            {
                var db = new dbcon();
                var f_model = db.product_features_type.Where(m => m.features_parents_id == 2).ToList();
                foreach (var i in f_model)
                {
                    product_feature_dict.Add(i.features_member, i.features_name);
                }
            }
            return ref product_feature_dict;
        }

        public static List<KeyValuePair<Func<product_features, bool>, string>> product_feature_to_string = new List<KeyValuePair<Func<product_features, bool>, string>>() {
            new KeyValuePair<Func<product_features, bool>, string>(f => f.near_river == 1 , "near_river"),
            new KeyValuePair<Func<product_features, bool>, string>(f => f.near_sea == 1 , "near_sea"),
            new KeyValuePair<Func<product_features, bool>, string>(f => f.no_tent == 1 , "no_tent"),
            new KeyValuePair<Func<product_features, bool>, string>(f => f.have_canopy == 1 , "have_canopy"),
            new KeyValuePair<Func<product_features, bool>, string>(f => f.have_clouds == 1 , "have_clouds"),
            new KeyValuePair<Func<product_features, bool>, string>(f => f.have_firefly == 1 , "have_firefly"),
            new KeyValuePair<Func<product_features, bool>, string>(f => f.could_book_all == 1 , "could_book_all"),
            new KeyValuePair<Func<product_features, bool>, string>(f => f.have_rental_equipment == 1 , "have_rental_equipment"),
            new KeyValuePair<Func<product_features, bool>, string>(f => f.have_game_area == 1 , "have_game_area"),
            new KeyValuePair<Func<product_features, bool>, string>(f => f.elevation_under_300m == 1 , "elevation_under_300m"),
            new KeyValuePair<Func<product_features, bool>, string>(f => f.elevation_301m_to_500m == 1 , "elevation_301m_to_500m"),
            new KeyValuePair<Func<product_features, bool>, string>(f => f.elevation_over_501m == 1 , "elevation_over_501m")
        };
        public static List<string> GetFeatureList(product_features p_feature)
        {
            List<string> output = new List<string>();
            foreach (var kv in product_feature_to_string)
            {
                if (kv.Key(p_feature))
                {
                    output.Add(GetFeatureDict()[kv.Value]);
                }
            }
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