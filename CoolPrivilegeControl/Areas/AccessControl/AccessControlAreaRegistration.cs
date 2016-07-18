/**************************************************************************
*
* NAME        : AccessControlAreaRegistration.cs
*
* VERSION     : 1.0.0
*
* DATE        : 17-Jan-2016
*
* DESCRIPTION : AccessControlAreaRegistration
*
* MODIFICATION HISTORY
* Name             Date         Description
* ===============  ===========  =======================================
* Wells Cheung     17-Jan-2016  Initial Version
*
**************************************************************************/
using System.Web.Mvc;

namespace CoolPrivilegeControl.Areas.AccessControl
{
    public class AccessControlAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "AccessControl";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "AccessControl_default",
                "AccessControl/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}