using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace campingplan.Models
{
    public class cvmGetDataList
    {
        public int rowid { get; set; }
        public int? order_closed { get; set; }
        public string user_no { get; set; }
        public string order_no { get; set; }
        public DateTime? order_date { get; set; }
        public string status_no { get; set; }
        public string status_name { get; set; }
        public string payment_name { get; set; }
        public string receive_name { get; set; }
        public string receive_address { get; set; }
        public string remark { get; set; }

    }
}