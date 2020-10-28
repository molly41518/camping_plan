using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace campingplan.Models
{
    public class cvmLogin
    {
        [Key]
        [Required(ErrorMessage = "請輸入帳號")]
        [DisplayName("帳號")]
        public string CustomerAccount { get; set; }

        [Required(ErrorMessage = "請輸入密碼")]
        [DisplayName("密碼")]
        [DataType(DataType.Password)]
        public string CustomerPassword { get; set; }

        [DisplayName("記住我")]
        public bool Remember { get; set; }

        public string ErrorMessage { get; set; }
    }
}