using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using campingplan.Models;
using static campingplan.App_Class.AppEnums;

namespace campingplan.App_Class
{
    public static class UserAccount
    {
        #region 私有變數
        private static int _TicketVersion = 1;
        private static int _RememberTimeout = 525600;    // 525600 分鐘 = 1 年
        private static int _NotRememberTimeout = 60;     // 60 分鐘 = 1 小時
        //private static bool _IsLogin = false;
        private static bool _IsRememberMe = false;
        //private static string _UserNo = "";
        //private static string _UserName = "";
        //private static string _UserEmail = "";
        //private static enUserRole _UserRole = enUserRole.Guest;
        #endregion


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

        public static bool IsRememberMe { get { return _IsRememberMe; } set { _IsRememberMe = value; } }

        public static bool IsAuthenticated { get { return HttpContext.Current.User.Identity.IsAuthenticated; } }

        public static int RememberTimeout { get { return _RememberTimeout; } set { _RememberTimeout = value; } }
        /// <summary>
        /// 非 RememberMe Timeout 有效分鐘數
        /// </summary>
        public static int NotRememberTimeout { get { return _NotRememberTimeout; } set { _NotRememberTimeout = value; } }
        /// <summary>
        /// RememberMe Cookie 票期版本
        /// </summary>
        public static int TicketVersion { get { return _TicketVersion; } set { _TicketVersion = value; } }


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
                string str_url = "~/Content/images/user/guest.jpg";
                string str_file = string.Format("~/Content/images/user/{0}.jpg", UserNo);
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

        public static void Login(users users, AppEnums.enUserRole roleNo)
        {
            UserNo = users.mno;
            UserOfAccount = users.maccount;
            UserName = users.mname;
            RoleNo = roleNo;
            IsLogin = true;
            Cart.LoginCart();
        }

        public static void Login(string Userno)
        {
            using (dbcon db = new dbcon())
            {
                var users = db.users.Where(m => m.mno == Userno).FirstOrDefault();
                if(users != null)
                {
                    UserNo = users.mno;
                    UserOfAccount = users.maccount;
                    UserName = users.mname;
                    RoleNo = GetRoleNo(users.role_no);
                    IsLogin = true;
                    Cart.LoginCart();
                }
            }
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

        /// <summary>
        /// 登入 RememberMe 記錄至 Cookie
        /// </summary>
        public static void LoginAuthenticate()
        {
            int int_timeout = (IsRememberMe) ? RememberTimeout : NotRememberTimeout;
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
            TicketVersion,
            UserNo,
            DateTime.Now,
            DateTime.Now.AddMinutes(int_timeout),
            IsRememberMe,
            UserRoleNo,
            FormsAuthentication.FormsCookiePath
            );
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie authCookie = new HttpCookie(
                        FormsAuthentication.FormsCookieName,
                        encryptedTicket);
            authCookie.Secure = true;
            HttpContext.Current.Response.Cookies.Add(authCookie);
            FormsAuthentication.SetAuthCookie(UserNo, IsRememberMe);
        }
        #region 函數
        /// <summary>
        /// 取得登入驗證中的 Cookie 資料
        /// </summary>
        /// <param name="identityType">資料類別</param>
        /// <returns></returns>
        public static string GetIdentityValue(enIdentityType identityType)
        {
            string str_value = "";
            FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            // 取得表單驗證的 Cookie 路徑。
            if (identityType == enIdentityType.CookiePath) str_value = (ticket.CookiePath == null) ? string.Empty : ticket.CookiePath.ToString();
            // 取得表單驗證票證到期的本機日期和時間。
            if (identityType == enIdentityType.Expiration) str_value = (ticket.Expiration == null) ? DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss") : ticket.Expiration.ToString("yyyy-MM-dd HH:mm:ss");
            // 取得值，指出表單驗證票證是否已經到期。
            if (identityType == enIdentityType.Expired) str_value = (ticket.Expired) ? "1" : "0";
            // 取得值，指出包含表單驗證票證資訊的 Cookie 是否為持續性。 持續性 cookie 會在瀏覽器關閉後保持有效，直到過期為止。
            if (identityType == enIdentityType.IsPersistent) str_value = (ticket.IsPersistent) ? "1" : "0";
            // 取得表單驗證票證核發的原始本機日期和時間。
            if (identityType == enIdentityType.IssueDate) str_value = (ticket.IssueDate == null) ? string.Empty : ticket.IssueDate.ToString("yyyy-MM-dd HH:mm:ss");
            // 取得與表單驗證票證相關聯的使用者名稱。
            if (identityType == enIdentityType.Name) str_value = (ticket.Name == null) ? string.Empty : ticket.Name.ToString();
            // 取得與票證一起存放的使用者特定字串。
            if (identityType == enIdentityType.UserData) str_value = (ticket.UserData == null) ? string.Empty : ticket.UserData.ToString();
            // 取得票證的版本號碼。
            if (identityType == enIdentityType.Version) str_value = ticket.Version.ToString();
            return str_value;
        }
        #endregion

        /// <summary>
        /// 表單驗證的欄位類別
        /// </summary>
        public enum enIdentityType
        {
            /// <summary>
            /// 表單驗證的 Cookie 路徑
            /// </summary>
            CookiePath = 0,
            /// <summary>
            /// 表單驗證票證到期的本機日期和時間
            /// </summary>
            Expiration = 1,
            /// <summary>
            /// 表單驗證票證是否已經到期
            /// </summary>
            Expired = 2,
            /// <summary>
            /// 表單驗證票證資訊的 Cookie 是否為持續性
            /// </summary>
            IsPersistent = 3,
            /// <summary>
            /// 表單驗證票證核發的原始本機日期和時間
            /// </summary>
            IssueDate = 4,
            /// <summary>
            /// 表單驗證票證相關聯的使用者名稱
            /// </summary>
            Name = 5,
            /// <summary>
            /// 與票證一起存放的使用者特定字串
            /// </summary>
            UserData = 6,
            /// <summary>
            /// 票證的版本號碼
            /// </summary>
            Version = 7
        }

    }


}
