using CoolPrivilegeControl.Models;
/**************************************************************************
*
* NAME        : ErrorController.cs
*
* VERSION     : 1.0.0
*
* DATE        : 17-Jan-2016
*
* DESCRIPTION : ErrorController
*
* MODIFICATION HISTORY
* Name             Date         Description
* ===============  ===========  =======================================
* Wells Cheung     17-Jan-2016  Initial Version
*
**************************************************************************/
using CoolPrivilegeControlVM;
using CoolPrivilegeControlVM.CommonVM;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using UI_Infrastructure;
using UI_Infrastructure.ComController;
using UI_Infrastructure.CommonFilter.AuthorizationFilter;
using UI_Infrastructure.CommonFilter.TracerActionFilter;

using UI_Infrastructure.CustomHtmlHelper;
using UI_Infrastructure.ViewModels;

namespace CoolPrivilegeControl.Controllers
{
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [Export(typeof(IController))]
    [ExportMetadata("ControllerName", "Error")]
    [ExportMetadata("Order", 1)]
    public class ErrorController : CommonController, IController
    {
        #region [ Fields ]
        private CompositionContainer container
        {
            get;
            set;
        }
        #endregion

        [ImportingConstructor]
        public ErrorController(CompositionContainer container)
        {
            this.postOffice = new PostOffice();

            this.container = container;
        }

        [UnAuthorization]
        [UnTracerAction]
        public ActionResult AccessDenied()
        {
            return View();
        }

        [UnAuthorization]
        [UnTracerAction]
        public ActionResult SessionTimeout()
        {
            return View();
        }

        [UnAuthorization]
        public ViewResult Index()
        {
            Exception ex = (Exception)TempData[StaticContent.ExceptionKey.ToString()];

            TempData.Keep(StaticContent.ExceptionKey.ToString());

            var routeDataDic = this.RouteData.Values;

            HandleErrorInfo errorInfo = new HandleErrorInfo(ex, routeDataDic["controller"].ToString(), routeDataDic["action"].ToString());

            return View(errorInfo);
        }

        [UnAuthorization]
        [UnTracerAction]
        public ViewResult NotFound()
        {
            Response.StatusCode = 404;  //you may want to set this to 200

            return View();
        }
    }
}