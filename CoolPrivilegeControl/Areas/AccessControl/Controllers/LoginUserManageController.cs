/**************************************************************************
*
* NAME        : LoginUserManageController.cs
*
* VERSION     : 1.0.0
*
* DATE        : 17-Jan-2016
*
* DESCRIPTION : LoginUserManageController
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
using UI_Infrastructure.CommonFilter.AuthorizationFilter;
using UI_Infrastructure.CustomHtmlHelper;
using UI_Infrastructure.ViewModels;
using CoolUtilities;
using System.Web.Script.Serialization;
using UI_Infrastructure.ComController;
using CoolPrivilegeControlVM.EntityVM;
using CoolUtilities.MultiLingual;
using CoolPrivilegeControlVM.CommonVM;
using CoolPrivilegeControlVM;
using CoolPrivilegeControl.Models;
using CoolPrivilegeControlVM.WCFVM;
using System.ServiceModel;
using CoolPrivilegeControlVM.WCFVM.LoginUserSerVM;
using CoolPrivilegeControlSerClient;

namespace CoolPrivilegeControl.Areas.AccessControl.Controllers
{
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [Export(typeof(IController))]
    [ExportMetadata("ControllerName", "LoginUserManage")]
    [ExportMetadata("Order", 1)]
    public class LoginUserManageController : CommonController, IController
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

        [Import(typeof(LoginUserMgtHelper))]
        public Lazy<LoginUserMgtHelper> loginUserMgtHelper
        { get; set; }

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
        public LoginUserManageController(CompositionContainer container)
        {
            this.container = container;

            this.postOffice = new PostOffice();

            ViewData[StaticContent.ActionFTMapperKey] = actionFTMapper;
        }

        public void RetrieveDropDownListInfo()
        {
            List<LUserOrgDetailsVM> entityList_OrgDetailsVM = new List<LUserOrgDetailsVM>();
            List<LUserRoleVM> entityList_RoleVM = new List<LUserRoleVM>();
            List<FunctionVM> entityList_FunVM = new List<FunctionVM>();
            List<LUserOrganizationVM> entityList_OrgVM = new List<LUserOrganizationVM>();

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                // Function List
                entityList_FunVM = funMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);

                // Organization List
                entityList_OrgVM = orgMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);

                // Role List
                 entityList_RoleVM = roleMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);

                // Organization Details List
                entityList_OrgDetailsVM = orgDetailMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);
            });

            #region [ Organization List ]
            entityList_OrgVM.ForEach(current => current.OrganizationKey = current.OrganizationPath + " - " + MultilingualHelper.GetStringFromResource(current.OrganizationKey));

            entityList_OrgVM = entityList_OrgVM.Where(current => guidList_AccessedOrgID.Contains(current.ID)).ToList();

            ViewBag.OrgList = webCommonHelper.GetSelectList(entityList_OrgVM, "OrganizationKey", "ID", true);
            #endregion

            #region [ Organization Details List ]
            ViewBag.OrgDetailsList = webCommonHelper.GetSelectList(entityList_OrgDetailsVM, "OrgDetailsKey", "ID", true);
            #endregion

            #region [ Role List ]
            ViewBag.RoleList = webCommonHelper.GetSelectList(entityList_RoleVM, "RoleName", "ID", true);
            #endregion

            #region [ Function List ]
            entityList_FunVM.ForEach(current => current.FunctionKey = current.FunctionPath + " - " + MultilingualHelper.GetStringFromResource(current.FunctionKey));

            ViewBag.FunIDs = webCommonHelper.GetSelectList(entityList_FunVM, "FunctionKey", "ID", true);
            #endregion
        }

        public List<string> CustomFilter(LoginUserVM vm)
        {
            List<string> strList_Query = new List<string>();
            if (!string.IsNullOrWhiteSpace(vm.LoginName))
            {
                strList_Query.Add(String.Format("{0}.StartsWith(\"{1}\")", "LU_Name", vm.LoginName));
            }
            return strList_Query;
        }

        // POST: AccessControl/LoginUserManage/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginUserVM selectionCriteria)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LoginUserManage");

            //Define output variable(recordCount && entityList_Result)
            int recordCount = 0;

            List<LoginUserVM> entityList_Result = new List<LoginUserVM>();

            //Define wcf output object;
            LUSerListResult entity_LUSerListResult = null;

            //Instantiate WebCommonHelper in order to call wcf service
            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_LUSerListResult = loginUserMgtHelper.Value.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, 1, PageSize, null, null, CustomFilter(selectionCriteria), guidList_AccessedLUserID);
            });

            if (entity_LUSerListResult != null)
            {
                recordCount = entity_LUSerListResult.Int_TotalRecordCount;
                entityList_Result = entity_LUSerListResult.EntityList_LoginUserVM;
            }

            //Set paging bar info (Total Record Count and Page Index)
            StorePageInfo(recordCount, 1);
            //Cache selection criteria
            StoreSelectionCriteria<LoginUserVM>(selectionCriteria);

            string strError = "";
            if (entity_LUSerListResult.StrList_Error.Count() > 0)
                strError = string.Join("<br/>", entity_LUSerListResult.StrList_Error.ToArray());

            if (entity_LUSerListResult.StrList_Error.Count > 0)
            {
                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = strError;
                errorMsgInfo.MsgType = MessageType.ValidationError;
                ViewBag.ActionMessage = errorMsgInfo;
            }

            return View(entityList_Result);
        }

        // GET: AccessControl/LoginUserManage/Index
        public ActionResult Index(int page = 1, string sort = "", string sortDir = "")
        {
            //Retrieve selection criteria from cache.
            LoginUserVM selectionCriteria = new LoginUserVM();

            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LoginUserManage");

            GetSelectionCriteriaFromViewData(ref selectionCriteria);

            //Define output variable(recordCount && entityList_Result)
            int recordCount = 0;

            List<LoginUserVM> entityList_Result = new List<LoginUserVM>();

            //Define wcf output object;
            LUSerListResult entity_LUSerListResult = null;

            //Instantiate WebCommonHelper in order to call wcf service
            WebCommonHelper webCommonHelper = new WebCommonHelper();
            //webCommonHelper.CallWCFHelper<ILoginUserMgtSer>(this, this.HttpContext, postOffice.LoginUserMgtSerPath, (entity_ILoginUserMgtSer, entity_WCFAuthInfoVM) =>
            //{
            //    entity_LUSerListResult = entity_ILoginUserMgtSer.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, page, PageSize, sort, sortDir, CustomFilter(selectionCriteria), guidList_AccessedLUserID);
            //});
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_LUSerListResult = loginUserMgtHelper.Value.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, page, PageSize, sort, sortDir, CustomFilter(selectionCriteria), guidList_AccessedLUserID);
            });

            if (entity_LUSerListResult != null)
            {
                recordCount = entity_LUSerListResult.Int_TotalRecordCount;
                entityList_Result = entity_LUSerListResult.EntityList_LoginUserVM;
            }

            //Set paging bar info (Total Record Count and Page Index)
            StorePageInfo(recordCount, page);
            //Cache selection criteria
            StoreSelectionCriteria<LoginUserVM>(null);

            string strError = "";
            if (entity_LUSerListResult.StrList_Error.Count() > 0)
                strError = string.Join("<br/>", entity_LUSerListResult.StrList_Error.ToArray());

            if (entity_LUSerListResult.StrList_Error.Count > 0)
            {
                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = strError;
                errorMsgInfo.MsgType = MessageType.ValidationError;
                ViewBag.ActionMessage = errorMsgInfo;
            }
            return View(entityList_Result);
        }

        // GET: AccessControl/LoginUserManage/Create
        public ActionResult Create()
        {
            //Retrieve Drop Down List Item
            RetrieveDropDownListInfo();

            return View(new LoginUserVM());
        }

        // POST: AccessControl/LoginUserManage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoginUserVM loginUserVM)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LoginUserManage_Create");

            //Retrieve Drop Down List Item
            RetrieveDropDownListInfo();

            if (!string.IsNullOrWhiteSpace(loginUserVM.NewPwd))
            {
                loginUserVM.LoginPwd = loginUserVM.NewPwd;
            }

            ModelState.Clear();
            TryValidateModel(loginUserVM);

            if (!ErrorMsgHelper.CustomValiation(str_MsgBoxTitle, ModelState, ViewBag))
            {
                return View(loginUserVM);
            }

            string strError = "";

            if (!string.IsNullOrWhiteSpace(loginUserVM.funDListJson) && (loginUserVM.UserType.HasValue && loginUserVM.UserType.Value == 1))
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                loginUserVM.EntityList_FDInfo = javaScriptSerializer.Deserialize<List<FunDetailInfo>>(loginUserVM.funDListJson);
            }

            WCFReturnResult entity_Return = new WCFReturnResult();

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            //webCommonHelper.CallWCFHelper<ILoginUserMgtSer>(this, this.HttpContext, postOffice.LoginUserMgtSerPath, (entity_ILoginUserMgtSer, entity_WCFAuthInfoVM) =>
            //{
            //    entity_Return = entity_ILoginUserMgtSer.Create(entity_WCFAuthInfoVM, loginUserVM);
            //});

            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_Return = loginUserMgtHelper.Value.Create(entity_WCFAuthInfoVM, loginUserVM);
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
                return View(loginUserVM);
            }
        }

        // GET: AccessControl/LoginUserManage/Edit
        [CustomAuthorization(UserIdKey = "UserID")]
        public ActionResult Edit(string UserID)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LoginUserManage_Edit");

            //Retrieve Drop Down List Item
            RetrieveDropDownListInfo();

            LUSerEditResult wcf_Return = new LUSerEditResult();

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            //webCommonHelper.CallWCFHelper<ILoginUserMgtSer>(this, this.HttpContext, postOffice.LoginUserMgtSerPath, (entity_ILoginUserMgtSer, entity_WCFAuthInfoVM) =>
            //{
            //    wcf_Return = entity_ILoginUserMgtSer.GetEntityByIDWDetails(entity_WCFAuthInfoVM, UserID);
            //});

            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                wcf_Return = loginUserMgtHelper.Value.GetEntityByIDWDetails(entity_WCFAuthInfoVM, UserID);
            });

            if (wcf_Return.StrList_Error.Count > 0 || wcf_Return.Entity_LoginUserVM == null)
            {
                string strError = "";
                if (wcf_Return.StrList_Error.Count() > 0)
                    strError = string.Join("<br/>", wcf_Return.StrList_Error.ToArray());

                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = strError;
                errorMsgInfo.MsgType = MessageType.ValidationError;

                LoginUserVM selectionCriteria = new LoginUserVM();

                if (TempData.ContainsKey(SelectionCriteriaKey))
                {
                    selectionCriteria = (LoginUserVM)TempData[SelectionCriteriaKey];
                }

                TempData[SelectionCriteriaKey] = selectionCriteria;

                TempData[ActionMessageKey] = errorMsgInfo;
                return RedirectToAction("Index");
            }
            else
            {
                if (wcf_Return.Entity_LoginUserVM.EntityList_FDInfo.Count > 0)
                {
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    wcf_Return.Entity_LoginUserVM.funDListJson = javaScriptSerializer.Serialize(wcf_Return.Entity_LoginUserVM.EntityList_FDInfo);
                }
                return View(wcf_Return.Entity_LoginUserVM);
            }
        }

        // POST: AccessControl/LoginUserManage/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoginUserVM loginUserVM)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LoginUserManage_Edit");

            List<string> strList_Error = new List<string>();

            //Retrieve Drop Down List Item
            RetrieveDropDownListInfo();

            //Error Message
            string strError = "";

            LoginUserVM entityInst_Existing = null;

            WebCommonHelper webCommonHelper = new WebCommonHelper();

            //Define object to receive service result
            LUSerEditResult entity_LUSerEditResult = new LUSerEditResult();

            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_LUSerEditResult = loginUserMgtHelper.Value.GetEntityByID(entity_WCFAuthInfoVM, loginUserVM.ID.ToString());
            });

            if (entity_LUSerEditResult.StrList_Error.Count > 0)
            {
                foreach (var item in entity_LUSerEditResult.StrList_Error)
                {
                    strList_Error.Add(item);
                }
            }
            else
            {
                entityInst_Existing = entity_LUSerEditResult.Entity_LoginUserVM;

                if (loginUserVM.isChangePwd)
                {
                    loginUserVM.LoginPwd = loginUserVM.NewPwd;
                }
                else
                {
                    loginUserVM.LoginPwd = loginUserVM.NewPwd = loginUserVM.ConfirmNewPwd = entityInst_Existing.LoginPwd;
                }

                ModelState.Clear();
                TryValidateModel(loginUserVM);

                if (!ErrorMsgHelper.CustomValiation(str_MsgBoxTitle, ModelState, ViewBag))
                {
                    return View(loginUserVM);
                }

                if (loginUserVM.UserType.HasValue && loginUserVM.UserType.Value == 1 && !string.IsNullOrWhiteSpace(loginUserVM.funDListJson))
                {
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

                    loginUserVM.EntityList_FDInfo = javaScriptSerializer.Deserialize<List<FunDetailInfo>>(loginUserVM.funDListJson);
                }

                WCFReturnResult entity_Return = new WCFReturnResult();

                //webCommonHelper.CallWCFHelper<ILoginUserMgtSer>(this, this.HttpContext, postOffice.LoginUserMgtSerPath, (entity_ILoginUserMgtSer, entity_WCFAuthInfoVM) =>
                //{
                //    entity_Return = entity_ILoginUserMgtSer.Update(entity_WCFAuthInfoVM, loginUserVM);
                //});

                webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
                {
                    entity_Return = loginUserMgtHelper.Value.Update(entity_WCFAuthInfoVM, loginUserVM);
                });

                // Refresh Server Side Session
                if (entity_Return != null && entity_Return.IsSuccess)
                {
                    webCommonHelper.RefreshSeverSideSession(this, this.HttpContext, postOffice.LoginUserMgtSerPath);
                }

                if (entity_Return.StrList_Error.Count > 0)
                {
                    foreach (var item in entity_Return.StrList_Error)
                    {
                        strList_Error.Add(item);
                    }
                }
            }

            if (strList_Error.Count > 0)
                strError = string.Join("<br/>", strList_Error.ToArray());

            if (strList_Error.Count == 0)
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
                return View(loginUserVM);
            }
        }

        // POST: AccessControl/LoginUserManage/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorization(UserIdKey = "UserID")]
        public ActionResult Delete(FormCollection collection)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "LoginUserManage_Delete");
            //Get User ID
            string str_ID = collection["UserID"];

            CommonJsonResult commonJsonResult = new CommonJsonResult();
            commonJsonResult.MsgTitle = str_MsgBoxTitle;

            WCFReturnResult ret = new WCFReturnResult();

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            //webCommonHelper.CallWCFHelper<ILoginUserMgtSer>(this, this.HttpContext, postOffice.LoginUserMgtSerPath, (entity_ILoginUserMgtSer, entity_WCFAuthInfoVM) =>
            //{
            //    ret = entity_ILoginUserMgtSer.Delete(entity_WCFAuthInfoVM, str_ID);
            //});

            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                ret = loginUserMgtHelper.Value.Delete(entity_WCFAuthInfoVM, str_ID);
            });

            if (ret.IsSuccess)
            {
                //Refresh Server Side Session
                webCommonHelper.RefreshSeverSideSession(this, this.HttpContext, postOffice.LoginUserMgtSerPath);

                commonJsonResult.ReturnUrl = Url.Action("Index", "LoginUserManage", new { Area = "AccessControl" }, Request.Url.Scheme);
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

                    commonJsonResult.ReturnUrl = Url.Action("Index", "LoginUserManage", new { Area = "AccessControl" }, Request.Url.Scheme);
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
                    commonJsonResult.ReturnUrl = Url.Action("Index", "LoginUserManage", new { Area = "AccessControl" }, Request.Url.Scheme);
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

        #region [ Sub Grid Methods ]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UnAuthorization]
        public ActionResult Index_RoleList(FormCollection collection)
        {
            string role_IDList = collection["roleListIDList"];
            string strpage = collection.AllKeys.Contains("roleListPage") ? collection["roleListPage"] : "";
            string sort = collection.AllKeys.Contains("roleListSort") ? collection["roleListSort"] : "";
            string sortDir = collection.AllKeys.Contains("roleListSortDir") ? collection["roleListSortDir"] : "";
            int page = int.Parse(strpage);

            List<LUserRoleVM> ret = new List<LUserRoleVM>();
            if (!string.IsNullOrWhiteSpace(role_IDList))
            {
                List<string> strList_RoleId = role_IDList.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                WebCommonHelper webCommonHelper = new WebCommonHelper();
                webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
                {
                    ret = roleMgtHelper.Value.GetEntityListByIDList(entity_WCFAuthInfoVM, strList_RoleId);
                });
            }
            if (!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(sortDir))
                ret = ret.AsQueryable().OrderBy("it." + sort + " " + sortDir).ToList();
            else
            {
                ret = ret.AsQueryable().OrderBy("it.RoleName " + "asc").ToList();
            }
            PagingInfo pagingInfo = new PagingInfo(ret.Count(), PageSize, page, ((SystemInfoVM)StaticContent.SystemInfoInst).DisplayPageNum);
            ViewBag.RolePagingInfoModel = pagingInfo;

            ret = ret.Skip((page - 1) * PageSize).Take(PageSize).ToList();

            return PartialView("Index_RoleList", ret);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [UnAuthorization]
        public ActionResult Index_OrgList(FormCollection collection)
        {
            string org_IDList = collection["orgListIDList"];
            string orgD_IDList = collection["orgDetailsListIDList"];
            string strpage = collection.AllKeys.Contains("orgListPage") ? collection["orgListPage"] : "";
            string sort = collection.AllKeys.Contains("orgListSort") ? collection["orgListSort"] : "";
            string sortDir = collection.AllKeys.Contains("orgListSortDir") ? collection["orgListSortDir"] : "";
            int page = int.Parse(strpage);

            IList<LUserAccessByOrgVM> ret = new List<LUserAccessByOrgVM>();

            if (!string.IsNullOrWhiteSpace(org_IDList) && !string.IsNullOrWhiteSpace(orgD_IDList))
            {
                List<string> strList_OrgId = org_IDList.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                List<string> strList_OrgDetailsId = orgD_IDList.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                WebCommonHelper webCommonHelper = new WebCommonHelper();
                webCommonHelper.CallWCFHelper(this,(entity_WCFAuthInfoVM)=>{
                    ret = orgMgtHelper.Value.GetEntityListByIDList_LUserAccessByOrgVM(entity_WCFAuthInfoVM, strList_OrgId, strList_OrgDetailsId);
                });
            }

            if (!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(sortDir))
                ret = ret.AsQueryable().OrderBy("it." + sort + " " + sortDir).ToList();
            else
            {
                ret = ret.AsQueryable().OrderBy("it.OrganizationName " + "asc").ToList();
            }
            PagingInfo pagingInfo = new PagingInfo(ret.Count(), PageSize, page, ((SystemInfoVM)StaticContent.SystemInfoInst).DisplayPageNum);
            ViewBag.OrgPagingInfoModel = pagingInfo;

            ret = ret.Skip((page - 1) * PageSize).Take(PageSize).ToList();

            return PartialView("Index_OrgList", ret);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [UnAuthorization]
        public ActionResult Index_FDList(FormCollection collection)
        {
            string str_SelectedFunDetail = collection["FunDetailInfoListStr"];
            string strpage = collection.AllKeys.Contains("funDListPage") ? collection["funDListPage"] : "1";
            string sort = collection.AllKeys.Contains("funDListSort") ? collection["funDListSort"] : "";
            string sortDir = collection.AllKeys.Contains("funDListSortDir") ? collection["funDListSortDir"] : "";
            int page = int.Parse(strpage);

            List<FunDetailInfo> ret = new List<FunDetailInfo>();
            if (!string.IsNullOrWhiteSpace(str_SelectedFunDetail))
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                List<FunDetailInfo> entityList_SelectedFunDetail = javaScriptSerializer.Deserialize<List<FunDetailInfo>>(str_SelectedFunDetail);

                if (entityList_SelectedFunDetail != null && entityList_SelectedFunDetail.Count > 0)
                {
                    ret = entityList_SelectedFunDetail;
                }

            }
            if (!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(sortDir))
                ret = ret.AsQueryable().OrderBy("it." + sort + " " + sortDir).ToList();
            else
            {
                ret = ret.AsQueryable().OrderBy("it.FName " + "asc").ToList();
            }
            PagingInfo pagingInfo = new PagingInfo(ret.Count(), PageSize, page, ((SystemInfoVM)StaticContent.SystemInfoInst).DisplayPageNum);
            ViewBag.FDPagingInfoModel = pagingInfo;

            ret = ret.Skip((page - 1) * PageSize).Take(PageSize).ToList();

            return PartialView("Index_FDList", ret);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [UnAuthorization]
        public ActionResult GetFTList(FormCollection collection)
        {
            FunDetailInfo selectedFD = new FunDetailInfo();
            string str_FunID = collection.AllKeys.Contains("FunID") ? collection["FunID"] : "";

            if (!string.IsNullOrWhiteSpace(str_FunID))
            {
                WebCommonHelper webCommonHelper = new WebCommonHelper();
                webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
                {
                    FunDetailInfo selectedFD_Temp = funMgtHelper.Value.GetFunDetailInfo_FID(entity_WCFAuthInfoVM, str_FunID);

                    if (selectedFD_Temp != null)
                    {
                        selectedFD = selectedFD_Temp;
                    }
                });
            }
            return Json(selectedFD);
        }
        #endregion
    }
}
