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
            [JsonIgnore]
            public virtual product product { get; set; }

            [JsonIgnore]
            public virtual ICollection<product_typedetail_everydaystock> product_typedetail_everydaystock { get; set; }
        }
    }
}
