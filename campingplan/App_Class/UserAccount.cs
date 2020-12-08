using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace campingplan.App_Class
{
    public static class UserAccount
    {
        public static bool IsLogin { get; set; } = false;
        public static string UserOfAccount { get; set; } = "";
        public static string UserNo { get; set; } = "";

        public static AppEnums.enUserRole RoleNo { get; set; } = AppEnums.enUserRole.Guest;
        //Enum.GetName(enums中的哪一個type,對照的數字)得出角色的名字
        public static string RoleName { get { return Enum.GetName(typeof(AppEnums.enUserRole), UserAccount.RoleNo); } }
        public static string UserName { get; set; } = "未登入";
        public static string UserInfo
        {
            get
            {
                return string.Format("歡迎，{0}！", UserName);
            }
        }

        public static AppEnums.enUserRole GetRoleNo(string roleNo)
        {
            AppEnums.enUserRole roleUser = AppEnums.enUserRole.Guest;
            //若轉成功  ,更新 roleUser 拿到role數字，若不成功拿Guest數字(預設)
            Enum.TryParse(roleNo, true, out roleUser);
            return roleUser;
        }

        //在controller中使用，記錄在哪一筆使用
        public static int UserStatus
        {
            get
            {
                int int_value = 0;
                if (HttpContext.Current.Session["UserStatus"] != null)
                {
                    string str_value = HttpContext.Current.Session["UserStatus"].ToString();
                    if (!int.TryParse(str_value, out int_value)) int_value = 0;
                }
                return int_value;
            }
            set
            { HttpContext.Current.Session["UserStatus"] = value; }
        }


        //在controller中使用，記錄狀態
        public static int UserCode
        {
            get
            {
                int int_value = -1;
                if (HttpContext.Current.Session["UserCode"] != null)
                {
                    string str_value = HttpContext.Current.Session["UserCode"].ToString();
                    if (!int.TryParse(str_value, out int_value)) int_value = -1;
                }
                return int_value;
            }
            set
            { HttpContext.Current.Session["UserCode"] = value; }
        }


        public static bool UploadImageMode
        {
            get
            {
                bool bln_value = false;
                if (HttpContext.Current.Session["UploadImage"] != null)
                {
                    string str_value = HttpContext.Current.Session["UploadImage"].ToString();
                    bln_value = (str_value == "1");
                }
                return bln_value;
            }
            set
            { HttpContext.Current.Session["UploadImage"] = (value) ? "1" : "0"; }
        }

        public static string UserImageUrl
        {
            get
            {
                string str_url = "~/Images/user/guest.jpg";
                string str_file = string.Format("~/Images/user/{0}.jpg", UserOfAccount);
                //Server.MapPath轉型為絕對路徑，網頁只吃絕對路徑
                if (File.Exists(HttpContext.Current.Server.MapPath(str_file))) str_url = str_file;
                str_url += string.Format("?{0}", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                return str_url;
            }
        }

        public static string UserRoleNo
        {
            get { return (HttpContext.Current.Session["UserRoleNo"] == null) ? "Guest" : HttpContext.Current.Session["UserRoleNo"].ToString(); }
            set { HttpContext.Current.Session["UserRoleNo"] = value; }
        }

        public static void Login(string userName, string userNo, string userAccount, AppEnums.enUserRole roleNo)
        {
            UserNo = userNo;
            UserOfAccount = userAccount;
            UserName = userName;
            RoleNo = roleNo;
            IsLogin = true;
            Cart.LoginCart();
        }

        public static void LogOut()
        {
            UserNo = "";
            UserOfAccount = "";
            UserName = "";
            RoleNo = AppEnums.enUserRole.Guest;
            IsLogin = false;
            Cart.LoginCart();
        }
        public static string GetNewVarifyCode()
        {
            return Guid.NewGuid().ToString().ToUpper(); //產生驗證碼
        }
    }
}