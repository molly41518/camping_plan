using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.IO;
using campingplan.Models;
using System.Web.Configuration;

namespace campingplan.App_Class
{
    public class Shop
    {
        /// <summary>
        /// 應用程式名稱
        /// </summary>
        public static string AppName { get { return GetAppConfigValue("AppName"); } }

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
        public static string GetAppConfigValue(string keyName)
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

        public static string GetUserName(string userNo)
        {
            string str_name = "";
            using (dbcon db = new dbcon())
            {
                var models = db.users.Where(m => m.mno == userNo).FirstOrDefault();
                if (models != null) str_name = models.mname;
            }
            return str_name;
        }

        public static bool IsHasStock(product_typedetail pTD, int num, DateTime startday, DateTime endday)
        {
            int days = new TimeSpan(endday.Ticks - startday.Ticks).Days;
            for (int i = 0; i < days; i++)
            {
                DateTime tmpDay = startday.AddDays(i);
                if (!pTD.product_typedetail_everydaystock.Any(s => s.stock_date == tmpDay && s.stock >= num))
                {
                    return false;
                }
            }
            return true;
        }

        public static Dictionary<string, List<product_typedetail>> GetDictProductTypeDetail(ICollection<product_typedetail> product_detail_list, int qty, DateTime startday, DateTime endday)
        {
            Dictionary<string, List<product_typedetail>> output = new Dictionary<string, List<product_typedetail>>();
            foreach (var p in product_detail_list)
            {
                if (!IsHasStock(p, qty, startday, endday))
                {
                    continue;
                }
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

        private static List<int> product_feature_key = new List<int>();
        private static Dictionary<string, string>[] product_feature_value = new Dictionary<string, string>[10];
        public static ref Dictionary<string, string> GetFeatureDict(int type = 2)
        {
            for (int i = 0; i < product_feature_key.Count(); ++i)
            {
                if (product_feature_key[i] == type && product_feature_value[i].Count != 0)
                {
                    return ref product_feature_value[i];
                }
            }
            Dictionary<string, string> f_dict = new Dictionary<string, string>();
            var db = new dbcon();
            var f_model = db.product_features_type.Where(m => m.features_parents_id == type).ToList();
            foreach (var i in f_model)
            {
                f_dict.Add(i.features_member, i.features_name);
            }
            product_feature_key.Add(type);
            product_feature_value[product_feature_key.Count - 1] = f_dict;

            return ref product_feature_value[product_feature_key.Count - 1];
        }

        public static List<KeyValuePair<System.Linq.Expressions.Expression<Func<product, bool>>, string>> product_feature_exp_to_string = new List<KeyValuePair<System.Linq.Expressions.Expression<Func<product, bool>>, string>>() {
            new KeyValuePair<System.Linq.Expressions.Expression<Func<product, bool>>, string>(m => m.product_features.near_river == 1 , "near_river"),
            new KeyValuePair<System.Linq.Expressions.Expression<Func<product, bool>>, string>(m => m.product_features.near_sea == 1 , "near_sea"),
            new KeyValuePair<System.Linq.Expressions.Expression<Func<product, bool>>, string>(m => m.product_features.no_tent == 1 , "no_tent"),
            new KeyValuePair<System.Linq.Expressions.Expression<Func<product, bool>>, string>(m => m.product_features.have_canopy == 1 , "have_canopy"),
            new KeyValuePair<System.Linq.Expressions.Expression<Func<product, bool>>, string>(m => m.product_features.have_clouds == 1 , "have_clouds"),
            new KeyValuePair<System.Linq.Expressions.Expression<Func<product, bool>>, string>(m => m.product_features.have_firefly == 1 , "have_firefly"),
            new KeyValuePair<System.Linq.Expressions.Expression<Func<product, bool>>, string>(m => m.product_features.could_book_all == 1 , "could_book_all"),
            new KeyValuePair<System.Linq.Expressions.Expression<Func<product, bool>>, string>(m => m.product_features.have_rental_equipment == 1 , "have_rental_equipment"),
            new KeyValuePair<System.Linq.Expressions.Expression<Func<product, bool>>, string>(m => m.product_features.have_game_area == 1 , "have_game_area"),
            new KeyValuePair<System.Linq.Expressions.Expression<Func<product, bool>>, string>(m => m.product_features.elevation_under_300m == 1 , "elevation_under_300m"),
            new KeyValuePair<System.Linq.Expressions.Expression<Func<product, bool>>, string>(m => m.product_features.elevation_301m_to_500m == 1 , "elevation_301m_to_500m"),
            new KeyValuePair<System.Linq.Expressions.Expression<Func<product, bool>>, string>(m => m.product_features.elevation_over_501m == 1 , "elevation_over_501m")
        };
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
                if (prod != null) str_no = prod.vendor_no;
            }
            return str_no;
        }



        public static string ProductTypeNo
        {
            get { return (HttpContext.Current.Session["ProductTypeNo"] == null) ? "" : HttpContext.Current.Session["ProductTypeNo"].ToString(); }
            set { HttpContext.Current.Session["ProductTypeNo"] = value; }
        }

        public static string ProductNo
        {
            get { return (HttpContext.Current.Session["ProductNo"] == null) ? "" : HttpContext.Current.Session["ProductNo"].ToString(); }
            set { HttpContext.Current.Session["ProductNo"] = value; }
        }

        public static string ParentNo
        {
            get { return (HttpContext.Current.Session["ParentNo"] == null) ? "" : HttpContext.Current.Session["ParentNo"].ToString(); }
            set { HttpContext.Current.Session["ParentNo"] = value; }
        }

        public static string Pno
        {
            get { return (HttpContext.Current.Session["Pno"] == null) ? "" : HttpContext.Current.Session["Pno"].ToString(); }
            set { HttpContext.Current.Session["Pno"] = value; }
        }
        /// <summary>
        /// 布林值轉整數
        /// </summary>
        /// <param name="boolValue">布林值</param>
        /// <returns></returns>
        public static int BoolToInteger(bool boolValue)
        {
            return (boolValue) ? 1 : 0;
        }

        /// <summary>
        /// 整數轉布林值
        /// </summary>
        /// <param name="integerValue">整數</param>
        /// <returns></returns>
        public static bool IntegerToBool(int? integerValue)
        {
            return !(integerValue == null || integerValue != 1);
        }

        /// <summary>
        /// 檢查訂單狀態是否為未結案
        /// </summary>
        /// <param name="orderStatus">訂單狀態</param>
        /// <returns></returns>
        public static bool IsUnCloseOrder(string orderStatus)
        {
            bool bln_value = false;
            if (orderStatus == "CP") bln_value = true;
            if (orderStatus == "OR") bln_value = true;
            if (orderStatus == "RT") bln_value = true;
            return bln_value;
        }

        /// <summary>
        /// 取得訂單結案狀態
        /// </summary>
        /// <param name="orderStatus">訂單狀態代碼</param>
        /// <returns></returns>
        public static int? GetOrderClosed(string orderStatus)
        {
            int? int_value = 0;
            //已領取已付款
            if (orderStatus == "CP") int_value = 1;
            //取消訂單
            if (orderStatus == "OR") int_value = 1;
            //已退貨
            if (orderStatus == "RT") int_value = 1;
            return int_value;
        }

        /// <summary>
        /// 取得訂單已付款已領貨狀態
        /// </summary>
        /// <param name="orderStatus">訂單狀態代碼</param>
        /// <returns></returns>
        public static int? GetOrderValidate(string orderStatus)
        {
            int? int_value = 0;
            //已領取已付款
            if (orderStatus == "CP") int_value = 1;
            return int_value;
        }

        /// <summary>
        /// 訂單 ID
        /// </summary>
        public static int OrderID
        {
            get { return (HttpContext.Current.Session["OrderID"] == null) ? 0 : (int)HttpContext.Current.Session["OrderID"]; }
            set { HttpContext.Current.Session["OrderID"] = value; }
        }
        /// <summary>
        /// 訂單 編號
        /// </summary>
        public static string OrderNo
        {
            get { return (HttpContext.Current.Session["OrderNo"] == null) ? "0" : HttpContext.Current.Session["OrderNo"].ToString(); }
            set { HttpContext.Current.Session["OrderNo"] = value; }
        }

        public static List<status> GetStatusList()
        {
            using (dbcon db = new dbcon())
            {
                return db.status.OrderBy(m => m.status_no).ToList();
            }
        }

        public static List<users> GetVendorsList()
        {
            using (dbcon db = new dbcon())
            {
                return db.users.Where(m => m.role_no == "Vendor").OrderBy(m => m.mno).ToList();
            }
        }

        public static NameValueCollection SearchInfo
        {
            get { return (HttpContext.Current.Session["SearchInfo"] == null) ? new NameValueCollection() : (NameValueCollection) HttpContext.Current.Session["SearchInfo"]; }
            set { HttpContext.Current.Session["SearchInfo"] = value; }
        }

    }

}
