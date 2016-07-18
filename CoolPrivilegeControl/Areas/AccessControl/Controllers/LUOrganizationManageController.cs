/**************************************************************************
*
* NAME        : LUOrganizationManageController.cs
*
* VERSION     : 1.0.0
*
* DATE        : 13-Aug-2015
*
* DESCRIPTION : LUOrganizationManage Controller
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
using UI_Infrastructure.ComController;
using CoolPrivilegeControlVM.EntityVM;
using CoolPrivilegeControlVM.CommonVM;
using CoolPrivilegeControlVM;
using CoolUtilities.MultiLingual;
using CoolPrivilegeControl.Models;
using CoolPrivilegeControlVM.WCFVM.OrgSerVM;
using CoolPrivilegeControlVM.WCFVM;
using System.ServiceModel;
using CoolPrivilegeControlSerClient;


namespace CoolPrivilegeControl.Areas.AccessControl.Controllers
{

    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [Export(typeof(IController))]
    [ExportMetadata("ControllerName", "LUOrganizationManage")]
    [ExportMetadata("Order", 1)]
    public class LUOrganizationManageController : CommonController, IController
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

        [Import(typeof(OrgMgtHelper))]
        public Lazy<OrgMgtHelper> orgMgtHelper
        { get; set; }

        [Import(typeof(RoleMgtHelper))]
        public Lazy<RoleMgtHelper> roleMgtHelper
        { get; set; }
        #endregion

        [ImportingConstructor]
        public LUOrganizationManageController(CompositionContainer container)
        {
            this.container = container;

            this.postOffice = new PostOffice();

            ViewData[StaticContent.ActionFTMapperKey] = actionFTMapper;
        }

        public List<string> CustomFilter(LUserOrganizationVM vm)
        {
            List<string> strList_Query = new List<string>();
            if (!string.IsNullOrWhiteSpace(vm.OrganizationKey))
            {
                strList_Query.Add(String.Format("{0}.StartsWith(\"{1}\")", "LUO_Key", vm.OrganizationKey));
            }
            return strList_Query;
        }

        /// <summary>
        /// POST: AccessControl/LUOrganizationManage
        /// </summary>
        /// <param name="selectionCriteria"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LUserOrganizationVM selectionCriteria)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LUOrganizationManage");

            //Define output variable(recordCount && entityList_Result)
            int recordCount = 0;

            List<LUserOrganizationVM> entityList_Result = new List<LUserOrganizationVM>();

            //Define wcf output object;
            OrgSerListResult entity_OrgSerListResult = null;

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_OrgSerListResult = orgMgtHelper.Value.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, 1, PageSize, null, null, CustomFilter(selectionCriteria));
            });

            //Assign data to local variable
            if (entity_OrgSerListResult != null)
            {
                recordCount = entity_OrgSerListResult.Int_TotalRecordCount;
                entityList_Result = entity_OrgSerListResult.EntityList_LUserOrganizationVM;
            }

            //Set paging bar info (Total Record Count and Page Index)
            StorePageInfo(recordCount, 1);
            //Cache selection criteria
            StoreSelectionCriteria<LUserOrganizationVM>(selectionCriteria);

            //Pass Error To UI
            string strError = "";
            if (entity_OrgSerListResult != null && entity_OrgSerListResult.StrList_Error.Count() > 0)
                strError = string.Join("<br/>", entity_OrgSerListResult.StrList_Error.ToArray());

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

        // GET: AccessControl/LUOrganizationManage
        public ActionResult Index(int page = 1, string sort = "", string sortDir = "")
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LUOrganizationManage");

            //Define output variable(recordCount && entityList_Result)
            int recordCount = 0;

            LUserOrganizationVM selectionCriteria = new LUserOrganizationVM();

            GetSelectionCriteriaFromViewData(ref selectionCriteria);

            List<LUserOrganizationVM> entityList_Result = new List<LUserOrganizationVM>();

            //Define wcf output object;
            OrgSerListResult entity_OrgSerListResult = null;

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_OrgSerListResult = orgMgtHelper.Value.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, page, PageSize, sort, sortDir, CustomFilter(selectionCriteria));
            });

            //Assign data to local variable
            if (entity_OrgSerListResult != null)
            {
                recordCount = entity_OrgSerListResult.Int_TotalRecordCount;
                entityList_Result = entity_OrgSerListResult.EntityList_LUserOrganizationVM;
            }

            StorePageInfo(recordCount, page);

            StoreSelectionCriteria<FunctionVM>(null);

            //Pass Error To UI
            string strError = "";
            if (entity_OrgSerListResult != null && entity_OrgSerListResult.StrList_Error.Count() > 0)
                strError = string.Join("<br/>", entity_OrgSerListResult.StrList_Error.ToArray());

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

        public List<LUserOrgDetailsVM> initOrgDetailsList()
        {
            List<LUserOrgDetailsVM> entityList_OrgDetailsVM = new List<LUserOrgDetailsVM>();
            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entityList_OrgDetailsVM = orgDetailMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);
            });
            return entityList_OrgDetailsVM;
        }

        public ActionResult Create()
        {
            WebCommonHelper webCommonHelper = new WebCommonHelper();

            List<LUserOrgDetailsVM> entityList_OrgDetailsVM = initOrgDetailsList();

            ViewBag.OrgDList = webCommonHelper.GetSelectList(entityList_OrgDetailsVM, "OrgDetailsKey", "ID", true);

            return View(new LUserOrganizationVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LUserOrganizationVM orgVM)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LUOrganizationManage_Create");

            WebCommonHelper webCommonHelper = new WebCommonHelper();

            List<LUserOrgDetailsVM> entityList_OrgDetailsVM = initOrgDetailsList();

            ViewBag.OrgDList = webCommonHelper.GetSelectList(entityList_OrgDetailsVM, "OrgDetailsKey", "ID", true);

            ModelState.Clear();
            TryValidateModel(orgVM);

            if (!ErrorMsgHelper.CustomValiation(str_MsgBoxTitle, ModelState, ViewBag))
            {
                return View(orgVM);
            }

            string strError = "";

            //Define wcf output object;
            WCFReturnResult ret = new WCFReturnResult();

            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                ret = orgMgtHelper.Value.Create(entity_WCFAuthInfoVM, orgVM);
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
                return View(orgVM);
            }
        }

        public ActionResult Edit(string guid)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LUOrganizationManage_Edit");

            WebCommonHelper webCommonHelper = new WebCommonHelper();

            List<LUserOrgDetailsVM> entityList_OrgDetailsVM = initOrgDetailsList();

            ViewBag.OrgDList = webCommonHelper.GetSelectList(entityList_OrgDetailsVM, "OrgDetailsKey", "ID", true);

            Guid ID = Guid.Parse(guid);

            string strError = "";

            OrgSerEditResult entity_Return = new OrgSerEditResult();

            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_Return = orgMgtHelper.Value.GetEntityByID(entity_WCFAuthInfoVM, guid);
            });

            if (entity_Return.StrList_Error.Count > 0 || entity_Return.Entity_LUserOrganizationVM == null)
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
                return View(entity_Return.Entity_LUserOrganizationVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LUserOrganizationVM organizationVM)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LUOrganizationManage_Edit");

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

            ModelState.Clear();
            TryValidateModel(organizationVM);

            if (!ErrorMsgHelper.CustomValiation(str_MsgBoxTitle, ModelState, ViewBag))
            {
                return View(organizationVM);
            }

            string strError = "";

            WCFReturnResult entity_Return = new WCFReturnResult();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_Return = orgMgtHelper.Value.Update(entity_WCFAuthInfoVM, organizationVM);
            });

            if (entity_Return != null && entity_Return.IsSuccess)
            {
                webCommonHelper.RefreshSeverSideSession(this, this.HttpContext, postOffice.LoginUserMgtSerPath);
            }

            if (entity_Return != null && entity_Return.StrList_Error.Count() > 0)
                strError = string.Join("<br/>", entity_Return.StrList_Error.ToArray());

            if (entity_Return != null && entity_Return.IsSuccess)
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
                return View(organizationVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(FormCollection collection)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LUOrganizationManage_Delete");
            string str_ID = collection["ID"];
            string str_ErrorMsg = "";
            CommonJsonResult commonJsonResult = new CommonJsonResult();
            commonJsonResult.MsgTitle = str_MsgBoxTitle;

            WCFReturnResult ret = new WCFReturnResult();
            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                ret = orgMgtHelper.Value.Delete(entity_WCFAuthInfoVM, str_ID);
            });

            if (ret != null && ret.IsSuccess)
            {
                //Refresh Server Side Session
                webCommonHelper.RefreshSeverSideSession(this, this.HttpContext, postOffice.LoginUserMgtSerPath);

                commonJsonResult.ReturnUrl = Url.Action("Index", "LUOrganizationManage", new { Area = "AccessControl" }, Request.Url.Scheme);
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

                    commonJsonResult.ReturnUrl = Url.Action("Index", "LUOrganizationManage", new { Area = "AccessControl" }, Request.Url.Scheme);
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
                    commonJsonResult.ReturnUrl = Url.Action("Index", "LUOrganizationManage", new { Area = "AccessControl" }, Request.Url.Scheme);
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