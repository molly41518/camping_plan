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
    [MetadataType(typeof(ProductFeaturesMetaData))]
    public partial class product_features
    {
        [Display(Name = "近河流")]
        public bool bool_near_river {
            get { return near_river == 1; } 
            set { near_river = value ? 1 : 0; } 
        }

        [Display(Name = "近海邊")]
        public bool bool_near_sea
        {
            get { return near_sea == 1; }
            set { near_sea = value ? 1 : 0; }
        }
        [Display(Name = "免搭帳")]
        public bool bool_no_tent
        {
            get { return no_tent == 1; }
            set { no_tent = value ? 1 : 0; }
        }
        [Display(Name = "有雨棚")]
        public bool bool_have_canopy
        {
            get { return have_canopy == 1; }
            set { have_canopy = value ? 1 : 0; }
        }
        [Display(Name = "有雲海")]
        public bool bool_have_clouds
        {
            get { return have_clouds == 1; }
            set { have_clouds = value ? 1 : 0; }
        }
        [Display(Name = "有螢火蟲")]
        public bool bool_have_firefly
        {
            get { return have_firefly == 1; }
            set { have_firefly = value ? 1 : 0; }
        }
        [Display(Name = "可包場")]
        public bool bool_could_book_all
        {
            get { return could_book_all == 1; }
            set { could_book_all = value ? 1 : 0; }
        }
        [Display(Name = "裝備出租")]
        public bool bool_have_rental_equipment
        {
            get { return have_rental_equipment == 1; }
            set { have_rental_equipment = value ? 1 : 0; }
        }
        [Display(Name = "有遊戲區")]
        public bool bool_have_game_area
        {
            get { return have_game_area == 1; }
            set { have_game_area = value ? 1 : 0; }
        }
        [Display(Name = "300m以下")]
        public bool bool_elevation_under_300m
        {
            get { return elevation_under_300m == 1; }
            set { elevation_under_300m = value ? 1 : 0; }
        }
        [Display(Name = "301-500m")]
        public bool bool_elevation_301m_to_500m
        {
            get { return elevation_301m_to_500m == 1; }
            set { elevation_301m_to_500m = value ? 1 : 0; }
        }
        [Display(Name = "500m以上")]
        public bool bool_elevation_over_501m
        {
            get { return elevation_over_501m == 1; }
            set { elevation_over_501m = value ? 1 : 0; }
        }

        private class ProductFeaturesMetaData
        {
            [Key]
            [Display(Name = "記錄ID")]
            public int rowid { get; set; }

            [JsonIgnore]
            public virtual product product { get; set; }
        }
    }
}
