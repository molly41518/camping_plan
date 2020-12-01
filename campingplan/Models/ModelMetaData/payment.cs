using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace campingplan.Models
{
    [MetadataType(typeof(PaymentsMetaData))]
    public partial class payments
    {
        private class PaymentsMetaData
        {
            [Key]
            [Display(Name = "記錄ID")]
            public int rowid { get; set; }
            [Display(Name = "編號")]
            [Required(ErrorMessage = "編號不可空白!!")]
            public string payment_no { get; set; }
            [Display(Name = "名稱")]
            [Required(ErrorMessage = "名稱不可空白!!")]
            public string payment { get; set; }
            [Display(Name = "備註")]
            public string remark { get; set; }
        }
    }
}