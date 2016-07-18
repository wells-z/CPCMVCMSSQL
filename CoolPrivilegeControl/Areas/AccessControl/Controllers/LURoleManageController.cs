/**************************************************************************
*
* NAME        : LURoleManageController.cs
*
* VERSION     : 1.0.0
*
* DATE        : 17-Jan-2016
*
* DESCRIPTION : LURoleManageController
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
using System.Web.Mvc;
using UI_Infrastructure.CustomHtmlHelper;
using UI_Infrastructure.ViewModels;
using System.Web.Script.Serialization;
using UI_Infrastructure.ComController;
using CoolPrivilegeControlVM.EntityVM;
using CoolUtilities.MultiLingual;
using CoolPrivilegeControlVM;
using CoolPrivilegeControlVM.CommonVM;
using CoolPrivilegeControl.Models;
using CoolPrivilegeControlVM.WCFVM.RoleSerVM;
using System.ServiceModel;
using System.Linq;
using CoolPrivilegeControlVM.WCFVM;
using CoolPrivilegeControlSerClient;


namespace CoolPrivilegeControl.Areas.AccessControl.Controllers
{
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [Export(typeof(IController))]
    [ExportMetadata("ControllerName", "LURoleManage")]
    [ExportMetadata("Order", 1)]
    public class LURoleManageController : CommonController, IController
    {
        #region [ Fields ]
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

        [Import(typeof(FunMgtHelper))]
        public Lazy<FunMgtHelper> funMgtHelper
        { get; set; }

        [Import(typeof(OrgDetailMgtHelper))]
        public Lazy<OrgDetailMgtHelper> orgDetailMgtHelper
        { get; set; }

        [Import(typeof(RoleMgtHelper))]
        public Lazy<RoleMgtHelper> roleMgtHelper
        { get; set; }
        #endregion

        [ImportingConstructor]
        public LURoleManageController(CompositionContainer container)
        {
            this.container = container;

            this.postOffice = new PostOffice();

            ViewData[StaticContent.ActionFTMapperKey] = actionFTMapper;
        }

        public List<string> CustomFilter(LUserRoleVM vm)
        {
            List<string> strList_Query = new List<string>();
            if (!string.IsNullOrWhiteSpace(vm.RoleName))
            {
                strList_Query.Add(String.Format("{0}.StartsWith(\"{1}\")", "UR_Name", vm.RoleName));
            }
            return strList_Query;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LUserRoleVM selectionCriteria)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LoginUserManage_Create");

            //Define output variable(recordCount && entityList_Result)
            int recordCount = 0;

            List<LUserRoleVM> entityList_Result = new List<LUserRoleVM>();

            //Define wcf output object;
            RoleSerListResult entity_RoleSerListResult = null;

            //Instantiate WebCommonHelper in order to call wcf service
            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_RoleSerListResult = roleMgtHelper.Value.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, 1, PageSize, null, null, CustomFilter(selectionCriteria));
            });

            if (entity_RoleSerListResult != null)
            {
                recordCount = entity_RoleSerListResult.Int_TotalRecordCount;
                entityList_Result = entity_RoleSerListResult.EntityList_LUserRoleVM;
            }

            StorePageInfo(recordCount, 1);
            StoreSelectionCriteria<LUserRoleVM>(selectionCriteria);

            string strError = "";
            if (entity_RoleSerListResult.StrList_Error.Count() > 0)
                strError = string.Join("<br/>", entity_RoleSerListResult.StrList_Error.ToArray());

            if (entity_RoleSerListResult.StrList_Error.Count > 0)
            {
                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = strError;
                errorMsgInfo.MsgType = MessageType.ValidationError;
                ViewBag.ActionMessage = errorMsgInfo;
            }

            return View(entityList_Result);
        }

        // GET: AccessControl/LURoleManage
        public ActionResult Index(int page = 1, string sort = "", string sortDir = "")
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LoginUserManage_Create");

            LUserRoleVM selectionCriteria = new LUserRoleVM();

            GetSelectionCriteriaFromViewData(ref selectionCriteria);

            //Define output variable(recordCount && entityList_Result)
            int recordCount = 0;

            List<LUserRoleVM> entityList_Result = new List<LUserRoleVM>();

            //Define wcf output object;
            RoleSerListResult entity_RoleSerListResult = null;

            //Instantiate WebCommonHelper in order to call wcf service
            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_RoleSerListResult = roleMgtHelper.Value.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, page, PageSize, sort, sortDir, CustomFilter(selectionCriteria));
            });

            if (entity_RoleSerListResult != null)
            {
                recordCount = entity_RoleSerListResult.Int_TotalRecordCount;
                entityList_Result = entity_RoleSerListResult.EntityList_LUserRoleVM;
            }

            StorePageInfo(recordCount, page);

            StoreSelectionCriteria<LUserRoleVM>(null);

            string strError = "";
            if (entity_RoleSerListResult.StrList_Error.Count() > 0)
                strError = string.Join("<br/>", entity_RoleSerListResult.StrList_Error.ToArray());

            if (entity_RoleSerListResult.StrList_Error.Count > 0)
            {
                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = strError;
                errorMsgInfo.MsgType = MessageType.ValidationError;
                ViewBag.ActionMessage = errorMsgInfo;
            }

            return View(entityList_Result);
        }

        public void initFunsList()
        {
            WebCommonHelper webCommonHelper = new WebCommonHelper();

            List<FunctionVM> entityList_FunVM = new List<FunctionVM>();

            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entityList_FunVM = funMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);
            });

            entityList_FunVM.ForEach(current => current.FunctionKey = current.FunctionPath + " - " + MultilingualHelper.GetStringFromResource(current.FunctionKey));

            ViewBag.FunIDs = webCommonHelper.GetSelectList(entityList_FunVM, "FunctionKey", "ID", true);
        }

        public ActionResult Create()
        {
            initFunsList();

            return View(new LUserRoleVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LUserRoleVM entity_VM)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LURoleManage_Create");

            initFunsList();

            ModelState.Clear();
            TryValidateModel(entity_VM);

            if (!ErrorMsgHelper.CustomValiation(str_MsgBoxTitle, ModelState, ViewBag))
            {
                return View(entity_VM);
            }

            string strError = "";

            if (!string.IsNullOrWhiteSpace(entity_VM.funDListJson))
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                entity_VM.EntityList_FDInfo = javaScriptSerializer.Deserialize<List<FunDetailInfo>>(entity_VM.funDListJson);
            }

            WCFReturnResult entity_Return = new WCFReturnResult();

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_Return = roleMgtHelper.Value.Create(entity_WCFAuthInfoVM, entity_VM);
            });

            if (entity_Return.StrList_Error.Count() > 0)
                strError = string.Join("<br/>", entity_Return.StrList_Error.ToArray());

            if (entity_Return.IsSuccess)
            {
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
                return View(entity_VM);
            }
        }

        public ActionResult Edit(string ID)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LURoleManage_Edit");

            initFunsList();

            string strError = "";

            RoleSerEditResult wcf_Return = new RoleSerEditResult();

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                wcf_Return = roleMgtHelper.Value.GetEntityByID(entity_WCFAuthInfoVM, ID);
            });

            if (wcf_Return.StrList_Error.Count > 0 || wcf_Return.Entity_LUserRoleVM == null)
            {
                if (wcf_Return.StrList_Error.Count() > 0)
                    strError = string.Join("<br/>", wcf_Return.StrList_Error.ToArray());

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

                return RedirectToAction("Index");
            }
            else
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
                {
                    List<FunDetailInfo> entityList_FunDetailInfo = orgDetailMgtHelper.Value.GetPrivilegeByUserID(entity_WCFAuthInfoVM, ID, RoleType.Role);

                    wcf_Return.Entity_LUserRoleVM.EntityList_FDInfo = entityList_FunDetailInfo;
                    wcf_Return.Entity_LUserRoleVM.funDListJson = javaScriptSerializer.Serialize(entityList_FunDetailInfo);
                });
            }

            return View(wcf_Return.Entity_LUserRoleVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LUserRoleVM entity_VM)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LURoleManage_Edit");

            initFunsList();

            ModelState.Clear();
            TryValidateModel(entity_VM);

            if (!ErrorMsgHelper.CustomValiation(str_MsgBoxTitle, ModelState, ViewBag))
            {
                return View(entity_VM);
            }

            string strError = "";

            if (!string.IsNullOrWhiteSpace(entity_VM.funDListJson))
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

                entity_VM.EntityList_FDInfo = javaScriptSerializer.Deserialize<List<FunDetailInfo>>(entity_VM.funDListJson);
            }

            WCFReturnResult entity_Return = new WCFReturnResult();

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_Return = roleMgtHelper.Value.Update(entity_WCFAuthInfoVM, entity_VM);
            });

            if (entity_Return != null && entity_Return.StrList_Error.Count() > 0)
                strError = string.Join("<br/>", entity_Return.StrList_Error.ToArray());

            if (entity_Return != null && entity_Return.IsSuccess)
            {
                webCommonHelper.RefreshSeverSideSession(this, this.HttpContext, postOffice.LoginUserMgtSerPath);

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
                return View(entity_VM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(FormCollection collection)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LURoleManage_Delete");

            string str_ID = collection["ID"];

            CommonJsonResult commonJsonResult = new CommonJsonResult();
            commonJsonResult.MsgTitle = str_MsgBoxTitle;

            WCFReturnResult ret = new WCFReturnResult();

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                ret = roleMgtHelper.Value.Delete(entity_WCFAuthInfoVM, str_ID);
            });

            if (ret.IsSuccess)
            {
                webCommonHelper.RefreshSeverSideSession(this, this.HttpContext, postOffice.LoginUserMgtSerPath);

                commonJsonResult.ReturnUrl = Url.Action("Index", "LURoleManage", new { Area = "AccessControl" }, Request.Url.Scheme);
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
                    string str_ErrorMsg = "";
                    if (ret.StrList_Error.Count() > 0)
                        str_ErrorMsg = string.Join("<br/>", ret.StrList_Error.ToArray());

                    commonJsonResult.ReturnUrl = Url.Action("Index", "LURoleManage", new { Area = "AccessControl" }, Request.Url.Scheme);
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
                    commonJsonResult.ReturnUrl = Url.Action("Index", "LURoleManage", new { Area = "AccessControl" }, Request.Url.Scheme);
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
