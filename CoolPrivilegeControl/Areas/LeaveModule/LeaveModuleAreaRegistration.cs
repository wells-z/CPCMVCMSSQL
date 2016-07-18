using System.Web.Mvc;

namespace CoolPrivilegeControl.Areas.LeaveModule
{
    public class LeaveModuleAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "LeaveModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "LeaveModule_default",
                "LeaveModule/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}