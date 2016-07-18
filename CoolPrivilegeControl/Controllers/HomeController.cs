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
using System.Web;
using System.Web.Mvc;
using UI_Infrastructure;
using UI_Infrastructure.ComController;
using UI_Infrastructure.CommonFilter.AuthorizationFilter;
using UI_Infrastructure.CommonFilter.TracerActionFilter;

using UI_Infrastructure.CustomHtmlHelper;

namespace CoolPrivilegeControl.Controllers
{
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [Export(typeof(IController))]
    [ExportMetadata("ControllerName", "Home")]
    [ExportMetadata("Order", 1)]
    public class HomeController : CommonController, IController
    {
        #region [ Fields ]
        private CompositionContainer container
        {
            get;
            set;
        }

        private IDictionary<string, string> actionFTMapper = new Dictionary<string, string>()
        {
            {"Index_GET","View"}
        };
        #endregion

        [ImportingConstructor]
        public HomeController(CompositionContainer container)
        {
            this.container = container;

            this.postOffice = new PostOffice();

            ViewData[StaticContent.ActionFTMapperKey] = actionFTMapper;
        }

        // GET: Home
        [UnAuthorization]
        [UnTracerAction]
        public ActionResult Index()
        {
            return Redirect("IndexPart1");
        }

        [UnAuthorization]
        [UnTracerAction]
        public ActionResult IndexPart1()
        {
            return View();
        }

        [UnAuthorization]
        [UnTracerAction]
        public ActionResult IndexPart2()
        {
            return View();
        }
    }
}