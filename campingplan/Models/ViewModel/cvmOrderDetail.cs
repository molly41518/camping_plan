using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace campingplan.Models
{
    public class cvmOrderDetail
    {
        [Key]
        public int rowid { get; set; }
        public order OrderData { get; set; }
        public List<order_detail> OrderDetailList { get; set; }
    }
}