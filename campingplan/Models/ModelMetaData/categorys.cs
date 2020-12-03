using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace campingplan.Models
{
    [MetadataType(typeof(CategorysMetaData))]
    public partial class categorys
    {
        private class CategorysMetaData
        {
            [Key]
            [Display(Name = "記錄ID")]
            public int rowid { get; set; }
            [Display(Name = "父記錄ID")]
            public string parentid { get; set; }
            [Display(Name = "編號")]
            [Required(ErrorMessage = "編號不可空白!!")]
            public string category_no { get; set; }
            [Display(Name = "名稱")]
            [Required(ErrorMessage = "名稱不可空白!!")]
            public string category_name { get; set; }
        }
    }
}