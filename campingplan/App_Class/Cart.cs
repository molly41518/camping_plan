using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using campingplan.Models;

namespace campingplan.App_Class
{
    public static class Cart
    {
        //第一步 建立購物車相關屬性
        //1.訂單編號 2.購物批號 3.批號建立時間 4.購物車筆數 5.購物車合計

        /// <summary>
        /// 訂單編號
        /// </summary>
        public static string OrderNo { get; set; }

        /// <summary>
        /// 購物批號 
        /// </summary>
        public static string LotNo
        {
            get { return GetLotNo(); }
            set { HttpContext.Current.Session["CartLotNo"] = value; }
        }
        /// <summary>
        /// 取得購物批號
        /// </summary>
        /// <returns></returns>
        private static string GetLotNo()
        {
            object obj_lotno = HttpContext.Current.Session["CartLotNo"];
            return (obj_lotno == null) ? NewLotNo() : obj_lotno.ToString();
        }

        /// <summary>
        /// 購物批號建立時間
        /// </summary>
        public static DateTime LotCreateTime
        {
            get { return GetLotCreateTime(); }
            set { HttpContext.Current.Session["CartCreateTime"] = value; }
        }
        /// <summary>
        /// 取得購物批號建立時間
        /// </summary>
        /// <returns></returns>
        private static DateTime GetLotCreateTime()
        {
            object obj_time = HttpContext.Current.Session["CartCreateTime"];
            return (obj_time == null) ? DateTime.Now : DateTime.Parse(obj_time.ToString());
        }

        /// <summary>
        /// 購物車筆數
        /// </summary>
        public static int CartCount { get { return GetCartCount(); } }

        /// <summary>
        /// 取得目前購物車筆數
        /// </summary>
        /// <returns></returns>
        private static int GetCartCount()
        {
            int int_count = 0;
            using (dbcon db = new dbcon())
            {
                if (CustomerAccount.IsLogin)
                {
                    var data1 = db.carts
                        .Where(m => m.mno == CustomerAccount.CustomerNo)
                        .ToList();
                    if (data1 != null) int_count = data1.Count;
                }
                else
                {
                    var data2 = db.carts
                       .Where(m => m.lot_no == LotNo)
                       .ToList();
                    if (data2 != null) int_count = data2.Count;
                }
            }
            return int_count;
        }

        /// <summary>
        /// 購物車合計
        /// </summary>
        public static int CartTotal { get { return GetCartTotals(); } }

        /// <summary>
        /// 取得目前購物車金額合計
        /// </summary>
        /// <returns></returns>
        private static int GetCartTotals()
        {
            int? int_totals = 0;
            using (dbcon db = new dbcon())
            {
                if (CustomerAccount.IsLogin)
                {
                    var data1 = db.carts
                        .Where(m => m.mno == CustomerAccount.CustomerNo)
                        .ToList();
                    if (data1 != null) int_totals = data1.Sum(m => m.amount);
                }
                else
                {
                    var data2 = db.carts
                       .Where(m => m.lot_no == LotNo)
                       .ToList();
                    if (data2 != null) int_totals = data2.Sum(m => m.amount);
                }
            }
            if (int_totals == null) int_totals = 0;
            return int_totals.GetValueOrDefault();
        }

        //    第二步 購物流程
        //    1.將商品加到購物車 AddCart()
        //       1-1 先判斷有沒有登入會員
        //         1-1-1 有:將訪客資料加入會員資料庫，批號清空
        //       1-1-2 沒有:新增一個批號，加入商品
        //
        //        1-1-1 有的話
        //          1-1-1-1 如果已經是會員，找到會員的批號加入商品
        //          1-1-1-2 如果是訪客有加過購物車，找到訪客批號加入商品
        //                    1-1-1-1-1判斷同樣一個商品同樣時間有沒有加入過購物車
        //                          1-1-1-1-1-1 有的話直接數量加1
        //                          1-1-1-1-1-2 沒有的話新增購物車model 
        //            ********1-2-2如果後來登入會員，將訪客的購物車內容加入會員的批號，並將訪客的內容刪除******
        //    2.加入後留在商品明細頁不跳轉 view()
        //    3.點到購物車頁面，商品會自動加總顯示至頁面 CartTotal
        //    4.確認購物車商品後，按下購買，跳轉 view() 到填收付資訊頁 cvmOrder
        //    5.確認收付資訊，將收付資訊 model(cvmOrder) 填到訂購表 order(db)，購物車資訊 cart(db) 填到訂購明細表 order_detail(db)
        //    6.將原購物車的資訊 cart(db) 刪除


        //判斷有沒有登入會員，是否建立新批號
        public static string NewLotNo()
        {
            string str_lot_no = "";//已登入會員批號設為空
            if (!CustomerAccount.IsLogin)//沒有登入才新增批號
                str_lot_no = Guid.NewGuid().ToString().Substring(0, 15).ToUpper();
            LotNo = str_lot_no;
            LotCreateTime = DateTime.Now;
            return str_lot_no;
        }


        //已經登入會員
        public static void LoginCart()//已經登入
        {
            if (!string.IsNullOrEmpty(LotNo))//訪客時的批號不是空
            {
                int int_qty = 0;
                using (dbcon db = new dbcon())
                {
                    var datas = db.carts
                       .Where(m => m.lot_no == LotNo)//訪客時的批號
                       .ToList();
                    if (datas != null)//若有找到
                    {
                        foreach (var item in datas)
                        {
                            int_qty = item.ptype_qty.GetValueOrDefault();//拿到商品數量
                            AddCart(item.pno, item.ptype_no, item.ptype_spec, int_qty);//加入購物車資料庫(驗證該會員帳戶)
                            db.carts.Remove(item);//刪除訪客資料
                        }
                        db.SaveChanges();
                    }
                }
                NewLotNo();//若有登入會員，將批號改為空，非會員才建立批號
            }
            ClearCustomerLotNo();
        }

        private static void ClearCustomerLotNo()
        {
            using (dbcon db = new dbcon())
            {
                var datas = db.carts
                    .Where(m => m.mno == CustomerAccount.CustomerNo)
                    .ToList();
                if (datas != null)
                {
                    foreach (var data in datas)
                    {
                        data.lot_no = "";
                    }
                    db.SaveChanges();
                }
            }
        }


        //1.將商品加到購物車 AddCart()
        //將日期先進行處裡

        public static void AddCart(string pno, string ptype_no, DateTime startday, DateTime endday, int buyQty)
        {
            using (dbcon db = new dbcon())
            {
                int days = new TimeSpan(startday.Ticks - endday.Ticks).Days;
                string day_spec = "";
                for (int i = 0; i < days; i++)
                {
                    day_spec += startday.AddDays(i).ToString("yyyy/MM/dd");
                    day_spec += " ";
                }
                AddCart(pno, ptype_no, day_spec, buyQty);
            }
        }

        //將資料加入到購物車
        public static void AddCart(string pno, string ptype_no, string day_spec, int buyQty)
        {
            using (dbcon db = new dbcon())
            {

                //先在購物車搜尋，看有沒有加入過購物車
                var datas = db.carts.Where(m => m.lot_no == LotNo)
                    .Where(m => m.mno == CustomerAccount.CustomerNo)
                    .Where(m => m.pno == pno)
                    .Where(m => m.ptype_no == ptype_no)
                    .Where(m => m.ptype_spec == day_spec)
                    .FirstOrDefault();
                //沒有:新增一筆資料，加入商品
                int int_price = Shop.GetProductPrice(ptype_no);
                int int_amount = (buyQty * int_price);
                if (datas == null)
                {
                    carts models = new carts();
                    models.lot_no = LotNo;
                    models.mno = CustomerAccount.CustomerNo;
                    models.crete_time = LotCreateTime;
                    models.pno = pno;
                    models.pname = Shop.GetProductName(pno);
                    models.ptype_no = ptype_no;
                    models.ptype_name = Shop.GetProductTypeName(ptype_no);
                    models.ptype_spec = day_spec;
                    models.ptype_qty = buyQty;
                    models.ptype_price = int_price;
                    models.amount = int_amount;
                    
                    db.carts.Add(models);
                    db.SaveChanges();

                }
                else//有:將數量加總，總計重新計算
                {
                    datas.ptype_qty += buyQty;
                    datas.amount = int_amount;
                    db.SaveChanges();
                }
            }
        }

        

        public static void CartPayment(cvmOrder model)
        {
            OrderNo = CreateNewOrderNo(model);
            using (dbcon db = new dbcon())
            {
                var datas = db.carts
                   .Where(m => m.mno == CustomerAccount.CustomerNo)
                   .ToList();
                if (datas != null)
                {
                    //將同會員購買的所有品項加總 為什麼不用CartTotal??
                    int int_amount = datas.Sum(m => m.amount).GetValueOrDefault();
                    //加上稅金
                    decimal dec_tax = 0;
                    if (int_amount > 0)
                    {
                        dec_tax = Math.Round((decimal)(int_amount * 5 / 100), 0);
                    }
                    int int_total = int_amount + (int)dec_tax;
                    var data = db.order.Where(m => m.order_no == OrderNo).FirstOrDefault();
                    if (data != null)
                    {
                        data.amounts = int_amount;
                        data.taxs = (int)dec_tax;
                        data.totals = int_total;
                        db.SaveChanges();
                    }

                    foreach (var item in datas)
                    {
                        order_detail detail = new order_detail();
                        detail.order_no = OrderNo;
                        detail.pno = item.mno;
                        detail.pname = item.pname;
                        detail.ptype_no = item.ptype_no;
                        detail.ptype_name = item.ptype_name;
                        detail.vendor_no = Shop.GetVendorNoByProduct(item.pno);
                        detail.category_name = Shop.GetCategoryName(item.pno);
                        detail.ptype_spec = item.ptype_spec;
                        detail.qty = item.ptype_qty;
                        detail.price = item.ptype_price;
                        detail.amount = item.amount;
                        detail.remark = "";
                        db.order_detail.Add(detail);
                        db.SaveChanges();

                        db.carts.Remove(item);
                        db.SaveChanges();
                    }
                }
            }
        }
        private static string CreateNewOrderNo(cvmOrder model)
        {
            string str_order_no = "";
            string str_guid = Guid.NewGuid().ToString().Substring(0, 25).ToUpper();
            using (dbcon db = new dbcon())
            {
                order orders = new order();
                orders.order_closed = 0;
                orders.order_validate = 0;
                orders.order_no = "";
                orders.order_date = DateTime.Now;
                orders.mno = CustomerAccount.CustomerNo;
                orders.order_status = "ON";
                orders.order_guid = str_guid;
                orders.payment_no = model.payment_no;
                orders.payment_name = Shop.GetPaymentName(model.payment_no);
                orders.receive_name = model.receive_name;
                orders.receive_phone = model.receive_phone;
                orders.receive_email = model.receive_email;
                orders.receive_address = model.receive_address;
                orders.remark = model.remark;
                db.order.Add(orders);
                db.SaveChanges();


                //此時order_no還沒編號，待寫入guid後，再拿出來對比，使用rowid進行 +0 取8位 得出 str_order_no
                var neword = db.order.Where(m => m.order_guid == str_guid).FirstOrDefault();
                if (neword != null)
                {
                    str_order_no = neword.rowid.ToString().PadLeft(8, '0');
                    neword.order_no = str_order_no;
                    db.SaveChanges();
                }
            }
            return str_order_no;
        }



    }
}