using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace campingplan.Models
{
    public class cvmOrder
    {
        [Display(Name = "訂購人姓名")]
        [Required(ErrorMessage = "訂購人姓名不可空白")]
        public string receive_name { get; set; }
        [Display(Name = "訂購人電話")]
        [Required(ErrorMessage = "訂購人電話不可空白")]
        public string receive_phone { get; set; }
        [Display(Name = "訂購人信箱")]
        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = false, HtmlEncode = true, NullDisplayText = "請輸入電子信箱")]
        public string receive_email { get; set; }
        [Display(Name = "訂購人地址")]
        [Required(ErrorMessage = "訂購人地址不可空白")]
        public string receive_address { get; set; }
        [Display(Name = "付款方式")]
        [Required(ErrorMessage = "付款方式不可空白")]
        public string payment_no { get; set; }

        [Display(Name = "訂單備註")]
        public string remark { get; set; }

        public List<payments> PaymentsList { get; set; }
    }
}