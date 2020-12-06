using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace campingplan.Models
{
    [MetadataType(typeof(OrdersDetailMetaData))]
    public partial class order_detail
    {
        private class OrdersDetailMetaData
        {
            [Key]
            public int rowid { get; set; }
            [Display(Name = "訂單編號")]
            public string order_no { get; set; }
            [Display(Name = "廠商編號")]
            public string vendor_no { get; set; }
            [Display(Name = "商品編號")]
            public string pno { get; set; }
            [Display(Name = "商品名稱")]
            public string pname { get; set; }
            [Display(Name = "商品規格")]
            public string ptype_spec { get; set; }
            [Display(Name = "單價")]
            public Nullable<int> price { get; set; }
            [Display(Name = "數量")]
            public Nullable<int> qty { get; set; }
            [Display(Name = "小計")]
            public Nullable<int> amount { get; set; }
            [Display(Name = "備註")]
            public string remark { get; set; }
        }
    }
}