/**************************************************************************
*
* NAME        : FManageController.cs
*
* VERSION     : 1.0.0
*
* DATE        : 17-Jan-2016
*
* DESCRIPTION : FManageController
*
* MODIFICATION HISTORY
* Name             Date         Description
* ===============  ===========  =======================================
* Wells Cheung     17-Jan-2016  Initial Version
*
**************************************************************************/
using CoolPrivilegeControlVM;
using CoolPrivilegeControl.Models;
using CoolPrivilegeControlVM.CommonVM;
using CoolPrivilegeControlVM.EntityVM;
using CoolPrivilegeControlVM.WCFVM;
using CoolPrivilegeControlVM.WCFVM.FunSerVM;
using CoolUtilities.MultiLingual;
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
using UI_Infrastructure.CustomHtmlHelper;
using UI_Infrastructure.SessionMaintenance;
using UI_Infrastructure.ViewModels;
using UI_Infrastructure.CommonFilter.TracerActionFilter;
using CoolPrivilegeControlSerClient;


namespace CoolPrivilegeControl.Areas.AccessControl.Controllers
{
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [Export(typeof(IController))]
    [ExportMetadata("ControllerName", "FManage")]
    [ExportMetadata("Order", 1)]
    public class FManageController : CommonController, IController
    {
        [Import(typeof(FunMgtHelper))]
        public Lazy<FunMgtHelper> funMgtHelper
        { get; set; }

        [Import(typeof(FunTypeMgtHelper))]
        public Lazy<FunTypeMgtHelper> funTypeMgtHelper
        { get; set; }

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

        [ImportingConstructor]
        public FManageController(CompositionContainer container)
        {
            this.container = container;

            this.postOffice = new PostOffice();

            ViewData[StaticContent.ActionFTMapperKey] = actionFTMapper;
        }

        public List<string> CustomFilter(FunctionVM vm)
        {
            List<string> strList_Query = new List<string>();
            if (!string.IsNullOrWhiteSpace(vm.FunctionPath))
            {
                strList_Query.Add(String.Format("{0}.StartsWith(\"{1}\")", "F_Path", vm.FunctionPath));
            }

            if (!string.IsNullOrWhiteSpace(vm.FunctionKey))
            {
                strList_Query.Add(String.Format("{0}.StartsWith(\"{1}\")", "F_Key", vm.FunctionKey));
            }
            return strList_Query;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FunctionVM selectionCriteria)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "FManage");

            //Define output variable(recordCount && entityList_Result)
            int recordCount = 0;

            List<FunctionVM> entityList_Result = new List<FunctionVM>();

            //Define wcf output object;
            FSerListResult entity_FSerListResult = null;

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_FSerListResult = funMgtHelper.Value.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, 1, PageSize, null, null, CustomFilter(selectionCriteria));
            });

            //Assign data to local variable
            if (entity_FSerListResult != null)
            {
                recordCount = entity_FSerListResult.Int_TotalRecordCount;
                entityList_Result = entity_FSerListResult.EntityList_FunctionVM;
            }

            //Set paging bar info (Total Record Count and Page Index)
            StorePageInfo(recordCount, 1);
            //Cache selection criteria
            StoreSelectionCriteria<FunctionVM>(selectionCriteria);

            //Pass Error To UI
            string strError = "";
            if (entity_FSerListResult.StrList_Error.Count() > 0)
                strError = string.Join("<br/>", entity_FSerListResult.StrList_Error.ToArray());

            if (entity_FSerListResult.StrList_Error.Count > 0)
            {
                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = strError;
                errorMsgInfo.MsgType = MessageType.ValidationError;
                ViewBag.ActionMessage = errorMsgInfo;
            }

            return View(entityList_Result);
        }

        public ActionResult Index(int page = 1, string sort = "", string sortDir = "")
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "FManage");

            //Define output variable(recordCount && entityList_Result)
            int recordCount = 0;

            FunctionVM selectionCriteria = new FunctionVM();

            GetSelectionCriteriaFromViewData(ref selectionCriteria);

            List<FunctionVM> entityList_Result = new List<FunctionVM>();

            //Define wcf output object;
            FSerListResult entity_FSerListResult = null;

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_FSerListResult = funMgtHelper.Value.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, page, PageSize, sort, sortDir, CustomFilter(selectionCriteria));
            });

            //Assign data to local variable
            if (entity_FSerListResult != null)
            {
                recordCount = entity_FSerListResult.Int_TotalRecordCount;
                entityList_Result = entity_FSerListResult.EntityList_FunctionVM;
            }

            //Set paging bar info (Total Record Count and Page Index)
            StorePageInfo(recordCount, page);

            //Cache selection criteria
            StoreSelectionCriteria<FunctionVM>(null);

            //Pass Error To UI
            string strError = "";
            if (entity_FSerListResult.StrList_Error.Count() > 0)
                strError = string.Join("<br/>", entity_FSerListResult.StrList_Error.ToArray());

            if (entity_FSerListResult.StrList_Error.Count > 0)
            {
                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = strError;
                errorMsgInfo.MsgType = MessageType.ValidationError;
                ViewBag.ActionMessage = errorMsgInfo;
            }

            return View(entityList_Result);
        }

        public ActionResult Create()
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "FManage_Create");

            List<FunctionTypeVM> entityList_VM = new List<FunctionTypeVM>();

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entityList_VM = funTypeMgtHelper.Value.GetAllFunType(entity_WCFAuthInfoVM);
            });

            FunctionVM entity_Function = new FunctionVM();
            if (entityList_VM != null && entityList_VM.Count() > 0)
            {
                foreach (var item in entityList_VM)
                {
                    entity_Function.SelectedTypeList.Add(new FunctionSelectedType()
                    {
                        ID = item.ID,
                        FunctionType = item.FunctionType,
                        Selected = false
                    });
                }
            }
            return View(entity_Function);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FunctionVM functionVM)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "FManage_Create");

            ModelState.Clear();
            TryValidateModel(functionVM);

            if (!ErrorMsgHelper.CustomValiation(str_MsgBoxTitle, ModelState, ViewBag))
            {
                return View(functionVM);
            }

            string strError = "";

            WCFReturnResult ret = new WCFReturnResult();

            //Call WCF Service
            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                ret = funMgtHelper.Value.Create(entity_WCFAuthInfoVM, functionVM);
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
                return View(functionVM);
            }
        }

        public ActionResult Edit(string guid)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "FManage_Edit");

            string strError = "";

            FSerEditResult entity_Return = new FSerEditResult();
            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_Return = funMgtHelper.Value.GetEntityByID(entity_WCFAuthInfoVM, guid);
            });

            if (entity_Return.StrList_Error.Count > 0 || entity_Return.Entity_FunctionVM == null)
            {
                if (entity_Return.StrList_Error.Count() > 0)
                    strError = string.Join("<br/>", entity_Return.StrList_Error.ToArray());

                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = strError;
                errorMsgInfo.MsgType = MessageType.ValidationError;

                FunctionVM selectionCriteria = new FunctionVM();

                if (TempData.ContainsKey(SelectionCriteriaKey))
                {
                    selectionCriteria = (FunctionVM)TempData[SelectionCriteriaKey];
                }

                TempData[SelectionCriteriaKey] = selectionCriteria;

                TempData[ActionMessageKey] = errorMsgInfo;
                return RedirectToAction("Index");
            }
            else
                return View(entity_Return.Entity_FunctionVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FunctionVM functionVM)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "FManage_Edit");

            ModelState.Clear();
            TryValidateModel(functionVM);

            if (!ErrorMsgHelper.CustomValiation(str_MsgBoxTitle, ModelState, ViewBag))
            {
                return View(functionVM);
            }

            string strError = "";

            WCFReturnResult entity_Return = new WCFReturnResult();
            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_Return = funMgtHelper.Value.Update(entity_WCFAuthInfoVM, functionVM);
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
                return View(functionVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(FormCollection collection)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "FManage_Edit");
            string str_ID = collection["ID"];
            string str_ErrorMsg = "";
            CommonJsonResult commonJsonResult = new CommonJsonResult();
            commonJsonResult.MsgTitle = str_MsgBoxTitle;

            WCFReturnResult ret = new WCFReturnResult();
            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                ret = funMgtHelper.Value.Delete(entity_WCFAuthInfoVM, str_ID);
            });

            if (ret != null && ret.IsSuccess)
            {
                //Refresh Server Side Session
                webCommonHelper.RefreshSeverSideSession(this, this.HttpContext, postOffice.LoginUserMgtSerPath);

                commonJsonResult.ReturnUrl = Url.Action("Index", "FManage", new { Area = "AccessControl" }, Request.Url.Scheme);
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

                    commonJsonResult.ReturnUrl = Url.Action("Index", "FManage", new { Area = "AccessControl" }, Request.Url.Scheme);
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
                    commonJsonResult.ReturnUrl = Url.Action("Index", "FManage", new { Area = "AccessControl" }, Request.Url.Scheme);
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
