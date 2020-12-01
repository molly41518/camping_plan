using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace campingplan.App_Class
{
    public static class CustomerAccount
    {
        public static bool IsLogin { get; set; } = false;
        public static string CustomerName { get; set; } = "";
        public static string CustomerNo { get; set; } = "";
        public static string CustomerInfo 
        {
            get
            {
                return string.Format("歡迎，{0}！", CustomerName);
            }
        }

        public static void Login(string customerName)
        {
            CustomerName = customerName;
            IsLogin = true;
        }

        public static void LogOut()
        {
            CustomerName = "";
            IsLogin = false;
        }
    }
}