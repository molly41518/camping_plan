﻿using System.Web.Mvc;

namespace campingplan.Areas.Member
{
    public class MemberAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get     
            {
                return "Member";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Member_order",
                "Member/{controller}/{action}/{id}/{code}",
                new { controller = "order" , action = "Index", id = UrlParameter.Optional , code = UrlParameter.Optional }
            );

            context.MapRoute(
                "Member_default",
                "Member/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}