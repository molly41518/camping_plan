using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace campingplan.Models
{
    [MetadataType(typeof(ProductTypeDetailMetaData))]
    public partial class product_typedetail
    {
        private class ProductTypeDetailMetaData
        {
            [Key]
            [Display(Name = "記錄ID")]
            public int rowid { get; set; }
            [Display(Name = "商品編號")]
            public string pno { get; set; }
            [Display(Name = "區域名稱")]
            public string parea_name { get; set; }
            [Display(Name = "營地編號")]
            public string ptype_no { get; set; }
            [Display(Name = "營地名稱")]
            public string ptype_name { get; set; }
            [Display(Name = "營地單價")]
            public Nullable<int> ptype_price { get; set; }
            [Display(Name = "營地介紹")]
            public string ptype_dep { get; set; }
            [Display(Name = "備註")]
            public string remark { get; set; }

            [JsonIgnore]
            public virtual product product { get; set; }

            [JsonIgnore]
            public virtual ICollection<product_typedetail_everydaystock> product_typedetail_everydaystock { get; set; }
        }
    }
}
