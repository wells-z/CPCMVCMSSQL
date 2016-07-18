using System.Web.Mvc;

namespace CoolPrivilegeControl.Areas.LandUseMan
{
    public class LandUseManAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "LandUseMan";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "LandUseMan_default",
                "LandUseMan/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}