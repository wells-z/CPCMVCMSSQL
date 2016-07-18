/**************************************************************************
*
* NAME        : AuthorizedHistoryController.cs
*
* VERSION     : 1.0.0
*
* DATE        : 17-Jan-2016
*
* DESCRIPTION : AuthorizedHistoryController
*
* MODIFICATION HISTORY
* Name             Date         Description
* ===============  ===========  =======================================
* Wells Cheung     17-Jan-2016  Initial Version
*
**************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using UI_Infrastructure.ComController;
using UI_Infrastructure.ViewModels;
using CoolUtilities.MultiLingual;
using CoolPrivilegeControlVM.EntityVM;
using CoolPrivilegeControlVM.CommonVM;
using CoolPrivilegeControlVM;
using CoolPrivilegeControlVM.WCFVM.AuthorizedMgtSerVM;
using UI_Infrastructure.CustomHtmlHelper;
using CoolPrivilegeControl.Models;
using CoolPrivilegeControlVM.WCFVM;
using CoolPrivilegeControlSerClient;



namespace CoolPrivilegeControl.Areas.AccessControl.Controllers
{
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [Export(typeof(IController))]
    [ExportMetadata("ControllerName", "AuthorizedHistoryManage")]
    [ExportMetadata("Order", 1)]
    public class AuthorizedHistoryManageController : CommonController, IController
    {
        private CompositionContainer container
        {
            get;
            set;
        }

        private IDictionary<string, string> actionFTMapper = new Dictionary<string, string>()
        {
            {"Index_POST","Search"},
            {"Index_GET","View"}
        };

        [Import(typeof(AuthHisMgtHelper))]
        public Lazy<AuthHisMgtHelper> authHisMgtHelper
        { get; set; }

        [ImportingConstructor]
        public AuthorizedHistoryManageController(CompositionContainer container)
        {
            this.container = container;

            this.postOffice = new PostOffice();

            ViewData[StaticContent.ActionFTMapperKey] = actionFTMapper;
        }

        private void initOperationType(string operationType)
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem()
            {
                Value = "",
                Text = MultilingualHelper.GetStringFromResource("PleaseSelect")
            });
            _list.Add(new SelectListItem()
            {
                Value = "L",
                Text = MultilingualHelper.GetStringFromResource("Login")
            });
            _list.Add(new SelectListItem()
            {
                Value = "O",
                Text = MultilingualHelper.GetStringFromResource("Logout")
            });
            ViewBag.OperationTypeList = new SelectList((IEnumerable<SelectListItem>)_list, "Value", "Text", operationType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection collection)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "AuditLogManage");

            //Define output variable(recordCount && entityList_Result)
            int recordCount = 0;

            List<AuthorizedHistoryVM> entityList_Result = new List<AuthorizedHistoryVM>();

            AuthorizedHistoryVM selectionCriteria = new AuthorizedHistoryVM();

            typeof(AuthorizedHistoryVM).GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance).ToList().ForEach(current =>
            {
                if (collection.AllKeys.Contains(current.Name))
                {
                    current.SetValue(selectionCriteria, collection[current.Name], null);
                }
            });

            initOperationType(selectionCriteria.OperationType);

            //Define wcf output object;
            AHSerListResult entity_AHSerListResult = null;

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_AHSerListResult = authHisMgtHelper.Value.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, 1, PageSize, null, null, null);
            });

            //Assign data to local variable
            if (entity_AHSerListResult != null)
            {
                recordCount = entity_AHSerListResult.Int_TotalRecordCount;
                entityList_Result = entity_AHSerListResult.EntityList_AuthorizedHistoryVM;
            }

            StorePageInfo(recordCount, 1);
            StoreSelectionCriteria<AuthorizedHistoryVM>(selectionCriteria);

            return View(entityList_Result);
        }

        // GET: AccessControl/AuthorizedHistory
        public ActionResult Index(int page = 1, string sort = "", string sortDir = "")
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "AuditLogManage");

            //Define output variable(recordCount && entityList_Result)
            int recordCount = 0;

            List<AuthorizedHistoryVM> entityList_Result = new List<AuthorizedHistoryVM>();

            AuthorizedHistoryVM selectionCriteria = new AuthorizedHistoryVM();

            GetSelectionCriteriaFromViewData(ref selectionCriteria);

            initOperationType(selectionCriteria.OperationType);

            //Define wcf output object;
            AHSerListResult entity_AHSerListResult = null;

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_AHSerListResult = authHisMgtHelper.Value.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, page, PageSize, sort, sortDir, null);
            });

            //Assign data to local variable
            if (entity_AHSerListResult != null)
            {
                recordCount = entity_AHSerListResult.Int_TotalRecordCount;
                entityList_Result = entity_AHSerListResult.EntityList_AuthorizedHistoryVM;
            }

            StorePageInfo(recordCount, page);

            StoreSelectionCriteria<LUserOrganizationVM>(null);

            return View(entityList_Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(FormCollection collection)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "AuthorizedHistoryManage_Delete");
            string str_ID = collection["ID"];
            string str_ErrorMsg = "";
            CommonJsonResult commonJsonResult = new CommonJsonResult();
            commonJsonResult.MsgTitle = str_MsgBoxTitle;

            WCFReturnResult ret = new WCFReturnResult();
            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                ret = authHisMgtHelper.Value.Delete(entity_WCFAuthInfoVM, str_ID);
            });

            if (ret != null && ret.IsSuccess)
            {
                commonJsonResult.ReturnUrl = Url.Action("Index", "AuthorizedHistoryManage", new { Area = "AccessControl" }, Request.Url.Scheme);
                commonJsonResult.Success = true;

                MsgInfo successMsgInfo = new MsgInfo();
                successMsgInfo.MsgTitle = str_MsgBoxTitle;
                successMsgInfo.MsgDesc = MultilingualHelper.GetStringFromResource(languageKey, "I001");
                successMsgInfo.MsgType = MessageType.Success;
                TempData[ActionMessageKey] = successMsgInfo;
                return Json(commonJsonResult);
            }
            else
            {
                if (ret.StrList_Error.Count > 0)
                {
                    str_ErrorMsg = string.Join("<br/>", ret.StrList_Error.ToArray());

                    commonJsonResult.ReturnUrl = Url.Action("Index", "AuthorizedHistoryManage", new { Area = "AccessControl" }, Request.Url.Scheme);
                    commonJsonResult.Success = false;

                    MsgInfo errorMsgInfo = new MsgInfo();
                    errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                    errorMsgInfo.MsgDesc = str_ErrorMsg;
                    errorMsgInfo.MsgType = MessageType.ValidationError;
                    TempData[ActionMessageKey] = errorMsgInfo;
                    return Json(commonJsonResult);
                }
                else
                {
                    commonJsonResult.ReturnUrl = Url.Action("Index", "AuthorizedHistoryManage", new { Area = "AccessControl" }, Request.Url.Scheme);
                    commonJsonResult.Success = false;

                    MsgInfo errorMsgInfo = new MsgInfo();
                    errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                    errorMsgInfo.MsgDesc = MultilingualHelper.GetStringFromResource(languageKey, "E006");
                    errorMsgInfo.MsgType = MessageType.ValidationError;
                    TempData[ActionMessageKey] = errorMsgInfo;
                    return Json(commonJsonResult);
                }
            }
        }
    }
}