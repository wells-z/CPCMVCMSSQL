using CoolPrivilegeControl.Models;
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

using UI_Infrastructure.CustomHtmlHelper;

namespace CoolPrivilegeControl.Areas.HRModule.Controllers
{
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [Export(typeof(IController))]
    [ExportMetadata("ControllerName", "MaintainSalaryInfo")]
    [ExportMetadata("Order", 1)]
    public class MaintainSalaryInfoController : CommonController, IController
    {
        private CompositionContainer container
        {
            get;
            set;
        }

        private IDictionary<string, string> actionFTMapper = new Dictionary<string, string>()
        {
            //{"Index_POST","Search"},
            {"Index_GET","View"}
        };

        [ImportingConstructor]
        public MaintainSalaryInfoController(CompositionContainer container)
        {
            this.container = container;

            this.postOffice = new PostOffice();

            ViewData[StaticContent.ActionFTMapperKey] = actionFTMapper;
        }

        // GET: HRModule/MaintainSalaryInfo
        public ActionResult Index()
        {
            //Do your work here

            return View();
        }
    }
}