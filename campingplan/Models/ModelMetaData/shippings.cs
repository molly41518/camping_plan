using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace campingplan.Models
{
    [MetadataType(typeof(ShippingsMetaData))]
    public partial class shippings
    {
        private class ShippingsMetaData
        {
            [Key]
            [Display(Name = "記錄ID")]
            public int rowid { get; set; }
            [Display(Name = "編號")]
            [Required(ErrorMessage = "編號不可空白!!")]
            public string shipping_no { get; set; }
            [Display(Name = "名稱")]
            [Required(ErrorMessage = "名稱不可空白!!")]
            public string shipping_name { get; set; }
            [Display(Name = "備註")]
            public string remark { get; set; }
        }
    }
}