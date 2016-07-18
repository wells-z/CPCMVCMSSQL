/**************************************************************************
*
* NAME        : LUOrgDetailsManageController.cs
*
* VERSION     : 1.0.0
*
* DATE        : 13-Aug-2015
*
* DESCRIPTION : LUOrgDetailsManage Controller
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
using System.Web.Mvc;
using UI_Infrastructure.CustomHtmlHelper;
using UI_Infrastructure.ViewModels;
using System.Web.Script.Serialization;
using UI_Infrastructure.ComController;
using CoolPrivilegeControlVM.EntityVM;
using CoolUtilities.MultiLingual;
using CoolPrivilegeControlVM.CommonVM;
using CoolPrivilegeControlVM;
using CoolPrivilegeControl.Models;
using CoolPrivilegeControlVM.WCFVM;
using CoolPrivilegeControlVM.WCFVM.OrgSerVM;
using CoolPrivilegeControlVM.WCFVM.OrgDetailsSerVM;
using System.ServiceModel;
using CoolPrivilegeControlSerClient;


namespace CoolPrivilegeControl.Areas.AccessControl.Controllers
{
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [Export(typeof(IController))]
    [ExportMetadata("ControllerName", "LUOrgDetailsManage")]
    [ExportMetadata("Order", 1)]
    public class LUOrgDetailsManageController : CommonController, IController
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
        public LUOrgDetailsManageController(CompositionContainer container)
        {
            this.container = container;

            this.postOffice = new PostOffice();

            ViewData[StaticContent.ActionFTMapperKey] = actionFTMapper;
        }

        public List<string> CustomFilter(LUserOrgDetailsVM vm)
        {
            List<string> strList_Query = new List<string>();
            if (!string.IsNullOrWhiteSpace(vm.OrgDetailsKey))
            {
                strList_Query.Add(String.Format("{0}.StartsWith(\"{1}\")", "OD_Key", vm.OrgDetailsKey));
            }
            return strList_Query;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LUserOrgDetailsVM selectionCriteria)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LUOrgDetailsManage");

            //Define output variable(recordCount && entityList_Result)
            int recordCount = 0;

            List<LUserOrgDetailsVM> entityList_Result = new List<LUserOrgDetailsVM>();

            //Define wcf output object;
            ODSerListResult entity_ODSerListResult = null;

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_ODSerListResult = orgDetailMgtHelper.Value.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, 1, PageSize, null, null, CustomFilter(selectionCriteria));
            });

            //Assign data to local variable
            if (entity_ODSerListResult != null)
            {
                recordCount = entity_ODSerListResult.Int_TotalRecordCount;
                entityList_Result = entity_ODSerListResult.EntityList_LUserOrgDetailsVM;
            }

            StorePageInfo(recordCount, 1);
            StoreSelectionCriteria<LUserOrgDetailsVM>(selectionCriteria);

            //Pass Error To UI
            string strError = "";
            if (entity_ODSerListResult != null && entity_ODSerListResult.StrList_Error.Count() > 0)
                strError = string.Join("<br/>", entity_ODSerListResult.StrList_Error.ToArray());

            if (!string.IsNullOrWhiteSpace(strError))
            {
                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = strError;
                errorMsgInfo.MsgType = MessageType.ValidationError;
                ViewBag.ActionMessage = errorMsgInfo;
            }

            return View(entityList_Result);
        }

        // GET: AccessControl/LUOrgDetailsManage
        public ActionResult Index(int page = 1, string sort = "", string sortDir = "")
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LUOrgDetailsManage");

            //Define output variable(recordCount && entityList_Result)
            int recordCount = 0;

            LUserOrgDetailsVM selectionCriteria = new LUserOrgDetailsVM();

            GetSelectionCriteriaFromViewData(ref selectionCriteria);

            List<LUserOrgDetailsVM> entityList_Result = new List<LUserOrgDetailsVM>();

            //Define wcf output object;
            ODSerListResult entity_ODSerListResult = null;

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_ODSerListResult = orgDetailMgtHelper.Value.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, 1, PageSize, null, null, CustomFilter(selectionCriteria));
            });

            //Assign data to local variable
            if (entity_ODSerListResult != null)
            {
                recordCount = entity_ODSerListResult.Int_TotalRecordCount;
                entityList_Result = entity_ODSerListResult.EntityList_LUserOrgDetailsVM;
            }

            StorePageInfo(recordCount, page);

            StoreSelectionCriteria<LUserOrganizationVM>(null);

            //Pass Error To UI
            string strError = "";
            if (entity_ODSerListResult != null && entity_ODSerListResult.StrList_Error.Count() > 0)
                strError = string.Join("<br/>", entity_ODSerListResult.StrList_Error.ToArray());

            if (!string.IsNullOrWhiteSpace(strError))
            {
                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = strError;
                errorMsgInfo.MsgType = MessageType.ValidationError;
                ViewBag.ActionMessage = errorMsgInfo;
            }

            return View(entityList_Result);
        }

        public void initOrgDetailsList()
        {
            WebCommonHelper webCommonHelper = new WebCommonHelper();

            List<LUserRoleVM> entityList_RoleVM = new List<LUserRoleVM>();
            List<FunctionVM> entityList_FunVM = new List<FunctionVM>();

            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entityList_FunVM = funMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);

                entityList_RoleVM = roleMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);
            });

            ViewBag.RoleList = webCommonHelper.GetSelectList(entityList_RoleVM, "RoleName", "ID", true);

            entityList_FunVM.ForEach(current => current.FunctionKey = current.FunctionPath + " - " + MultilingualHelper.GetStringFromResource(current.FunctionKey));

            ViewBag.FunIDs = webCommonHelper.GetSelectList(entityList_FunVM, "FunctionKey", "ID", true);
        }

        public ActionResult Create()
        {
            initOrgDetailsList();

            return View(new LUserOrgDetailsVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LUserOrgDetailsVM orgDetailsVM)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LUOrgDetailsManage_Create");

            initOrgDetailsList();

            ModelState.Clear();
            TryValidateModel(orgDetailsVM);

            if (!ErrorMsgHelper.CustomValiation(str_MsgBoxTitle, ModelState, ViewBag))
            {
                return View(orgDetailsVM);
            }

            string strError = "";

            if (!string.IsNullOrWhiteSpace(orgDetailsVM.funDListJson) && (orgDetailsVM.OrgDetailsType.HasValue && orgDetailsVM.OrgDetailsType.Value == 1))
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                orgDetailsVM.EntityList_FDInfo = javaScriptSerializer.Deserialize<List<FunDetailInfo>>(orgDetailsVM.funDListJson);
            }

            WCFReturnResult ret = new WCFReturnResult();

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                ret = orgDetailMgtHelper.Value.Create(entity_WCFAuthInfoVM, orgDetailsVM);
            });

            if (ret != null && ret.StrList_Error.Count() > 0)
                strError = string.Join("<br/>", ret.StrList_Error.ToArray());

            if (ret != null && ret.IsSuccess)
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

                return View(orgDetailsVM);
            }
        }

        public ActionResult Edit(string guid)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LUOrgDetailsManage_Edit");

            initOrgDetailsList();

            Guid ID = Guid.Parse(guid);

            string strError = "";

            ODSerEditResult entity_Return = new ODSerEditResult();
            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_Return = orgDetailMgtHelper.Value.GetEntityByID(entity_WCFAuthInfoVM, guid);
            });

            if (entity_Return.StrList_Error.Count > 0 || entity_Return.Entity_LUserOrgDetailsVM == null)
            {
                if (entity_Return.StrList_Error.Count() > 0)
                    strError = string.Join("<br/>", entity_Return.StrList_Error.ToArray());

                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = strError;
                errorMsgInfo.MsgType = MessageType.ValidationError;

                LUserOrgDetailsVM selectionCriteria = new LUserOrgDetailsVM();

                if (TempData.ContainsKey(SelectionCriteriaKey))
                {
                    selectionCriteria = (LUserOrgDetailsVM)TempData[SelectionCriteriaKey];
                }

                TempData[SelectionCriteriaKey] = selectionCriteria;

                TempData[ActionMessageKey] = errorMsgInfo;
                return RedirectToAction("Index");
            }
            else
            {
                //By Role Settings
                if (entity_Return.Entity_LUserOrgDetailsVM.OrgDetailsType.HasValue && entity_Return.Entity_LUserOrgDetailsVM.OrgDetailsType.Value == 2)
                {
                    webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
                    {
                        List<LUserRoleVM> entityList = orgDetailMgtHelper.Value.GetRoleSettingsByOrgDID(entity_WCFAuthInfoVM, guid);

                        if (entityList.Count > 0)
                        {
                            entity_Return.Entity_LUserOrgDetailsVM.roleListIDList = entityList.Select(currrent => currrent.ID.ToString()).Aggregate((first, next) =>
                            {
                                return first + "|" + next;
                            });
                        }
                    });
                }
                //By Specified Functions Settings
                else
                {
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

                    webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
                    {
                        List<FunDetailInfo> entityList_FunDetailInfo = orgDetailMgtHelper.Value.GetPrivilegeByUserID(entity_WCFAuthInfoVM, ID.ToString(), RoleType.Organization);

                        entity_Return.Entity_LUserOrgDetailsVM.EntityList_FDInfo = entityList_FunDetailInfo;
                        entity_Return.Entity_LUserOrgDetailsVM.funDListJson = javaScriptSerializer.Serialize(entityList_FunDetailInfo);
                    });
                }
                return View(entity_Return.Entity_LUserOrgDetailsVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LUserOrgDetailsVM orgDetailsVM)
        {
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LUOrgDetailsManage_Edit");

            WebCommonHelper webCommonHelper = new WebCommonHelper();

            initOrgDetailsList();

            ModelState.Clear();
            TryValidateModel(orgDetailsVM);

            if (!ErrorMsgHelper.CustomValiation(str_MsgBoxTitle, ModelState, ViewBag))
            {
                return View(orgDetailsVM);
            }

            string strError = "";

            if (orgDetailsVM.OrgDetailsType.HasValue && orgDetailsVM.OrgDetailsType.Value == 1 && !string.IsNullOrWhiteSpace(orgDetailsVM.funDListJson))
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

                orgDetailsVM.EntityList_FDInfo = javaScriptSerializer.Deserialize<List<FunDetailInfo>>(orgDetailsVM.funDListJson);
            }

            WCFReturnResult entity_Return = new WCFReturnResult();

            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_Return = orgDetailMgtHelper.Value.Update(entity_WCFAuthInfoVM, orgDetailsVM);
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
                return View(orgDetailsVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(FormCollection collection)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LUOrgDetailsManage_Delete");
            string str_ID = collection["ID"];
            string str_ErrorMsg = "";
            CommonJsonResult commonJsonResult = new CommonJsonResult();
            commonJsonResult.MsgTitle = str_MsgBoxTitle;

            WCFReturnResult ret = new WCFReturnResult();
            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                ret = orgDetailMgtHelper.Value.Delete(entity_WCFAuthInfoVM, str_ID);
            });

            if (ret != null && ret.IsSuccess)
            {
                //Refresh Server Side Session
                webCommonHelper.RefreshSeverSideSession(this, this.HttpContext, postOffice.LoginUserMgtSerPath);

                commonJsonResult.ReturnUrl = Url.Action("Index", "LUOrgDetailsManage", new { Area = "AccessControl" }, Request.Url.Scheme);
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

                    commonJsonResult.ReturnUrl = Url.Action("Index", "LUOrgDetailsManage", new { Area = "AccessControl" }, Request.Url.Scheme);
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
                    commonJsonResult.ReturnUrl = Url.Action("Index", "LUOrgDetailsManage", new { Area = "AccessControl" }, Request.Url.Scheme);
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