/**************************************************************************
*
* NAME        : SystemSetupController.cs
*
* VERSION     : 1.0.0
*
* DATE        : 17-Jan-2016
*
* DESCRIPTION : SystemSetupController
*
* MODIFICATION HISTORY
* Name             Date         Description
* ===============  ===========  =======================================
* Wells Cheung     17-Jan-2016  Initial Version
*
**************************************************************************/
using CoolPrivilegeControlVM.CommonVM;
using CoolPrivilegeControlVM.EntityVM;
using CoolPrivilegeControlVM;
using CoolUtilities.MultiLingual;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Web.Mvc;
using UI_Infrastructure.ComController;
using CoolPrivilegeControlVM.ExportMetadata;
using UI_Infrastructure.ViewModels;
using CoolPrivilegeControlVM.WCFVM.SysInfoSerVM;
using UI_Infrastructure.CustomHtmlHelper;
using CoolPrivilegeControl.Models;
using CoolPrivilegeControlVM.WCFVM;
using CoolPrivilegeControlSerClient;
using System;


namespace CoolPrivilegeControl.Areas.AccessControl.Controllers
{
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [Export(typeof(IController))]
    [ExportMetadata("ControllerName", "SystemInfoManage")]
    [ExportMetadata("Order", 1)]
    public class SystemInfoManageController : CommonController, IController
    {
        private IDictionary<string, string> actionFTMapper = new Dictionary<string, string>()
        {
            {"Index_GET","Edit"}
        };

        private CompositionContainer container
        {
            get;
            set;
        }

        [Import(typeof(SystemMgtHelper))]
        public Lazy<SystemMgtHelper> systemMgtHelper
        { get; set; }

        [ImportingConstructor]
        public SystemInfoManageController(CompositionContainer container)
        {
            this.container = container;

            this.postOffice = new PostOffice();

            ViewData[StaticContent.ActionFTMapperKey] = actionFTMapper;

            if (TempData[ActionMessageKey] != null)
            {
                ViewBag.ActionMessage = TempData[ActionMessageKey];
            }
        }

        // GET: AccessControl/SystemInfoManage
        public ActionResult Index()
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "SystemInfoManage");

            string strError = "";

            SystemInfoVM entity_SystemInfoVM = null;

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_SystemInfoVM = systemMgtHelper.Value.GetSystemInfo(entity_WCFAuthInfoVM);
            });

            if (entity_SystemInfoVM == null)
            {
                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = strError;
                errorMsgInfo.MsgType = MessageType.ValidationError;

                LUserRoleVM selectionCriteria = new LUserRoleVM();

                if (TempData.ContainsKey(SelectionCriteriaKey))
                {
                    selectionCriteria = (LUserRoleVM)TempData[SelectionCriteriaKey];
                }

                TempData[SelectionCriteriaKey] = selectionCriteria;

                TempData[ActionMessageKey] = errorMsgInfo;

                return View("Index");
            }
            else
            {
                if (TempData[ActionMessageKey] != null)
                {
                    ViewBag.ActionMessage = TempData[ActionMessageKey];
                }

                return View(entity_SystemInfoVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SystemInfoVM entity_VM)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "SystemInfoManage");

            ModelState.Clear();
            TryValidateModel(entity_VM);

            if (!ErrorMsgHelper.CustomValiation(str_MsgBoxTitle, ModelState, ViewBag))
            {
                return View(entity_VM);
            }

            string strError = "";

            WCFReturnResult entity_Return = new WCFReturnResult();

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_Return = systemMgtHelper.Value.Update(entity_WCFAuthInfoVM, entity_VM);
            });

            if (entity_Return != null && entity_Return.StrList_Error.Count() > 0)
                strError = string.Join("<br/>", entity_Return.StrList_Error.ToArray());

            if (entity_Return != null && entity_Return.IsSuccess)
            {
                webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
                {
                    //Update the static content
                    StaticContent.SystemInfoInst = systemMgtHelper.Value.GetSystemInfo(entity_WCFAuthInfoVM);
                });

                MsgInfo successMsgInfo = new MsgInfo();
                successMsgInfo.MsgTitle = str_MsgBoxTitle;
                successMsgInfo.MsgDesc = MultilingualHelper.GetStringFromResource(languageKey, "I000");
                successMsgInfo.MsgType = MessageType.Success;
                TempData[ActionMessageKey] = successMsgInfo;
                return RedirectToAction("Index");
            }
            else
            {
                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = strError;
                errorMsgInfo.MsgType = MessageType.ValidationError;
                ViewBag.ActionMessage = errorMsgInfo;
                return View("Index", entity_VM);
            }
        }
    }
}