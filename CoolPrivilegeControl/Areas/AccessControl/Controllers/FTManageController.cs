
/**************************************************************************
*
* NAME        : FTManageController.cs
*
* VERSION     : 1.0.0
*
* DATE        : 17-Jan-2016
*
* DESCRIPTION : FTManageController
*
* MODIFICATION HISTORY
* Name             Date         Description
* ===============  ===========  =======================================
* Wells Cheung     17-Jan-2016  Initial Version
*
**************************************************************************/
using CoolPrivilegeControlVM.CommonVM;
using CoolPrivilegeControlVM.EntityVM;
using CoolUtilities.MultiLingual;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Web.Mvc;
using UI_Infrastructure.ComController;
using UI_Infrastructure.ViewModels;
using UI_Infrastructure.CustomHtmlHelper;
using CoolPrivilegeControl.Models;
using CoolPrivilegeControlVM.WCFVM.FunTypeSerVM;
using CoolPrivilegeControlVM.WCFVM;
using CoolPrivilegeControlVM;
using CoolPrivilegeControlSerClient;


namespace CoolPrivilegeControl.Areas.AccessControl.Controllers
{
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [Export(typeof(IController))]
    [ExportMetadata("ControllerName", "FTManage")]
    [ExportMetadata("Order", 1)]
    public class FTManageController : CommonController, IController
    {
        private CompositionContainer container
        {
            get;
            set;
        }

        [Import(typeof(FunTypeMgtHelper))]
        public Lazy<FunTypeMgtHelper> funTypeMgtHelper
        { get; set; }

        private IDictionary<string, string> actionFTMapper = new Dictionary<string, string>()
        {
            {"Index_POST","Search"},
            {"Index_GET","View"}
        };

        [ImportingConstructor]
        public FTManageController(CompositionContainer container)
        {
            this.container = container;

            this.postOffice = new PostOffice();

            ViewData[StaticContent.ActionFTMapperKey] = actionFTMapper;
        }

        public List<string> CustomFilter(FunctionTypeVM vm)
        {
            List<string> strList_Query = new List<string>();
            if (!string.IsNullOrWhiteSpace(vm.FunctionType))
            {
                strList_Query.Add(String.Format("{0}.StartsWith(\"{1}\")", "FT_Name", vm.FunctionType));
            }
            return strList_Query;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FunctionTypeVM selectionCriteria)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "FTManage");

            //Declare output variable(recordCount && entityList_Result)
            int recordCount = 0;

            List<FunctionTypeVM> entityList_Result = new List<FunctionTypeVM>();

            //Declare wcf output object;
            FTSerListResult entity_FTSerListResult = null;

            //Instantiate WebCommonHelper in order to call wcf service
            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_FTSerListResult = funTypeMgtHelper.Value.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, 1, PageSize, null, null, CustomFilter(selectionCriteria));
            });

            //Assign data to local variable
            if (entity_FTSerListResult != null)
            {
                recordCount = entity_FTSerListResult.Int_TotalRecordCount;
                entityList_Result = entity_FTSerListResult.EntityList_FunctionTypeVM;
            }

            //Set paging bar info (Total Record Count and Page Index)
            StorePageInfo(recordCount, 1);

            //Cache selection criteria
            StoreSelectionCriteria<FunctionTypeVM>(selectionCriteria);

            //Pass Error To UI
            string strError = "";
            if (entity_FTSerListResult.StrList_Error.Count() > 0)
                strError = string.Join("<br/>", entity_FTSerListResult.StrList_Error.ToArray());

            //Fail
            if (entity_FTSerListResult.StrList_Error.Count > 0)
            {
                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = strError;
                errorMsgInfo.MsgType = MessageType.ValidationError;
                ViewBag.ActionMessage = errorMsgInfo;
            }

            //Success
            return View(entityList_Result);
        }

        // GET: AccessControl/FTMange
        public ActionResult Index(int page = 1, string sort = "", string sortDir = "")
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "FTManage");

            //Define output variable(recordCount && entityList_Result)
            int recordCount = 0;

            List<FunctionTypeVM> entityList_Result = new List<FunctionTypeVM>();

            //Define wcf output object;
            FTSerListResult entity_FTSerListResult = null;

            FunctionTypeVM selectionCriteria = new FunctionTypeVM();

            //Retrieve selection criteria that cached by system
            GetSelectionCriteriaFromViewData(ref selectionCriteria);

            //Instantiate WebCommonHelper in order to call wcf service
            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_FTSerListResult = funTypeMgtHelper.Value.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, page, PageSize, sort, sortDir, CustomFilter(selectionCriteria));
            });

            //Assign data to local variable
            if (entity_FTSerListResult != null)
            {
                recordCount = entity_FTSerListResult.Int_TotalRecordCount;
                entityList_Result = entity_FTSerListResult.EntityList_FunctionTypeVM;
            }

            //Set paging bar info (Total Record Count and Page Index)
            StorePageInfo(recordCount, page);
            //Cache selection criteria
            StoreSelectionCriteria<FunctionVM>(null);

            //Pass Error To UI
            string strError = "";
            if (entity_FTSerListResult.StrList_Error.Count() > 0)
                strError = string.Join("<br/>", entity_FTSerListResult.StrList_Error.ToArray());

            if (entity_FTSerListResult.StrList_Error.Count > 0)
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FunctionTypeVM functionTypeVM)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "FTManage_Create");

            ModelState.Clear();
            TryValidateModel(functionTypeVM);

            if (!ErrorMsgHelper.CustomValiation(str_MsgBoxTitle, ModelState, ViewBag))
            {
                return View();
            }

            string strError = "";

            //Define wcf output object;
            WCFReturnResult ret = new WCFReturnResult();

            //Call WCF Service
            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                ret = funTypeMgtHelper.Value.Create(entity_WCFAuthInfoVM, functionTypeVM);
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
                return View(functionTypeVM);
            }
        }

        public ActionResult Edit(string guid)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "FTManage_Edit");

            string strError = "";

            //Define wcf output object;
            FTSerEditResult entity_Return = new FTSerEditResult();

            //Call WCF Service
            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_Return = funTypeMgtHelper.Value.GetEntityByID(entity_WCFAuthInfoVM, guid);
            });

            if (entity_Return.StrList_Error.Count > 0 || entity_Return.Entity_FunctionTypeVM == null)
            {
                if (entity_Return.StrList_Error.Count() > 0)
                    strError = string.Join("<br/>", entity_Return.StrList_Error.ToArray());

                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = strError;
                errorMsgInfo.MsgType = MessageType.ValidationError;

                FunctionTypeVM selectionCriteria = new FunctionTypeVM();

                if (TempData.ContainsKey(SelectionCriteriaKey))
                {
                    selectionCriteria = (FunctionTypeVM)TempData[SelectionCriteriaKey];
                }

                TempData[SelectionCriteriaKey] = selectionCriteria;

                TempData[ActionMessageKey] = errorMsgInfo;
                return RedirectToAction("Index");
            }
            else
                return View(entity_Return.Entity_FunctionTypeVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FunctionTypeVM functionTypeVM)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "FTManage_Edit");

            ModelState.Clear();
            TryValidateModel(functionTypeVM);

            if (!ErrorMsgHelper.CustomValiation(str_MsgBoxTitle, ModelState, ViewBag))
            {
                return View(functionTypeVM);
            }

            string strError = "";

            WCFReturnResult entity_Return = new WCFReturnResult();
            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_Return = funTypeMgtHelper.Value.Update(entity_WCFAuthInfoVM, functionTypeVM);
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
                return View(functionTypeVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(FormCollection collection)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "FTManage_Delete");

            string str_ID = collection["ID"];
            string str_ErrorMsg = "";
            CommonJsonResult commonJsonResult = new CommonJsonResult();
            commonJsonResult.MsgTitle = str_MsgBoxTitle;

            WCFReturnResult ret = new WCFReturnResult();
            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                ret = funTypeMgtHelper.Value.Delete(entity_WCFAuthInfoVM, str_ID);
            });

            if (ret != null && ret.IsSuccess)
            {
                webCommonHelper.RefreshSeverSideSession(this, this.HttpContext, postOffice.LoginUserMgtSerPath);

                commonJsonResult.ReturnUrl = Url.Action("Index", "FTManage", new { Area = "AccessControl" }, Request.Url.Scheme);
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

                    commonJsonResult.ReturnUrl = Url.Action("Index", "FTManage", new { Area = "AccessControl" }, Request.Url.Scheme);
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
                    commonJsonResult.ReturnUrl = Url.Action("Index", "FTManage", new { Area = "AccessControl" }, Request.Url.Scheme);
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
