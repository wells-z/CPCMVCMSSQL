/**************************************************************************
*
* NAME        : AuditLogManageController.cs
*
* VERSION     : 1.0.0
*
* DATE        : 17-Jan-2016
*
* DESCRIPTION : AuditLogManageController
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
using CoolUtilities;
using UI_Infrastructure.ComController;
using CoolExcelHelper.Policies;
using System.IO;
using CoolPrivilegeControlVM;
using CoolPrivilegeControlVM.EntityVM;
using CoolPrivilegeControlVM.CommonVM;
using CoolUtilities.MultiLingual;
using CoolPrivilegeControlVM.WCFVM.AuditLogSerVM;
using CoolPrivilegeControl.Models;
using CoolPrivilegeControlSerClient;


namespace CoolPrivilegeControl.Areas.AccessControl.Controllers
{
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [Export(typeof(IController))]
    [ExportMetadata("ControllerName", "AuditLogManage")]
    [ExportMetadata("Order", 1)]
    public class AuditLogManageController : CommonController, IController
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
        #endregion

        [Import(typeof(AuditLogMgtHelper))]
        public Lazy<AuditLogMgtHelper> auditLogMgtHelper
        { get; set; }

        [ImportingConstructor]
        public AuditLogManageController(CompositionContainer container)
        {
            this.container = container;

            this.postOffice = new PostOffice();

            ViewData[StaticContent.ActionFTMapperKey] = actionFTMapper;
        }

        public List<string> CustomFilter(AuditLogVM vm)
        {
            List<string> strList_Query = new List<string>();
            return strList_Query;
        }

        private void initFunType()
        {
            WebCommonHelper CommonHelper = new WebCommonHelper();
            List<CTComboBoxVM> entityList_CTComboBoxVM = new List<CTComboBoxVM>();
            entityList_CTComboBoxVM.Add(new CTComboBoxVM()
            {
                Text = "Create",
                Value = "A"
            });
            entityList_CTComboBoxVM.Add(new CTComboBoxVM()
            {
                Text = "Edit",
                Value = "M"
            });
            entityList_CTComboBoxVM.Add(new CTComboBoxVM()
            {
                Text = "Delete",
                Value = "D"
            });

            ViewBag.FunTypes = CommonHelper.GetSelectList(entityList_CTComboBoxVM, "Text", "Value", true);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(AuditLogVM selectionCriteria)
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "AuditLogManage");

            //Define output variable(recordCount && entityList_Result)
            int recordCount = 0;

            List<AuditLogVM> entityList_Result = new List<AuditLogVM>();

            //Define wcf output object;
            ALSerListResult entity_ALSerListResult = null;

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_ALSerListResult = auditLogMgtHelper.Value.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, 1, PageSize, null, null, CustomFilter(selectionCriteria));
            });

            //webCommonHelper.CallWCFHelper<IAuditLogMgtSer>(this, this.HttpContext, postOffice.AuditLogMgtSerPath, (entity_IAuditLogMgtSer, entity_WCFAuthInfoVM) =>
            //{
            //    entity_ALSerListResult = entity_IAuditLogMgtSer.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, 1, PageSize, null, null, CustomFilter(selectionCriteria));
            //});

            //Assign data to local variable
            if (entity_ALSerListResult != null)
            {
                recordCount = entity_ALSerListResult.Int_TotalRecordCount;
                entityList_Result = entity_ALSerListResult.EntityList_AuditLogVM;
            }

            StorePageInfo(recordCount, 1);
            StoreSelectionCriteria<AuditLogVM>(selectionCriteria);

            initFunType();

            //Pass Error To UI
            string strError = "";
            if (entity_ALSerListResult.StrList_Error.Count() > 0)
                strError = string.Join("<br/>", entity_ALSerListResult.StrList_Error.ToArray());

            if (entity_ALSerListResult.StrList_Error.Count > 0)
            {
                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = strError;
                errorMsgInfo.MsgType = MessageType.ValidationError;
                ViewBag.ActionMessage = errorMsgInfo;
            }

            return View(entityList_Result);
        }

        // GET: AccessControl/FTMange
        public ActionResult Index(int page = 1, string sort = "", string sortDir = "")
        {
            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "AuditLogManage");

            //Define output variable(recordCount && entityList_Result)
            int recordCount = 0;

            List<AuditLogVM> entityList_Result = new List<AuditLogVM>();

            //Define wcf output object;
            ALSerListResult entity_ALSerListResult = null;

            AuditLogVM selectionCriteria = new AuditLogVM();

            GetSelectionCriteriaFromViewData(ref selectionCriteria);

            WebCommonHelper webCommonHelper = new WebCommonHelper();

            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_ALSerListResult = auditLogMgtHelper.Value.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, page, PageSize, sort, sortDir, CustomFilter(selectionCriteria));
            });

            //webCommonHelper.CallWCFHelper<IAuditLogMgtSer>(this, this.HttpContext, postOffice.AuditLogMgtSerPath, (entity_IAuditLogMgtSer, entity_WCFAuthInfoVM) =>
            //{
            //    entity_ALSerListResult = entity_IAuditLogMgtSer.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, page, PageSize, sort, sortDir, CustomFilter(selectionCriteria));
            //});

            //Assign data to local variable
            if (entity_ALSerListResult != null)
            {
                recordCount = entity_ALSerListResult.Int_TotalRecordCount;
                entityList_Result = entity_ALSerListResult.EntityList_AuditLogVM;
            }

            StorePageInfo(recordCount, page);

            StoreSelectionCriteria<AuditLogVM>(null);

            initFunType();

            //Pass Error To UI
            string strError = "";
            if (entity_ALSerListResult.StrList_Error.Count() > 0)
                strError = string.Join("<br/>", entity_ALSerListResult.StrList_Error.ToArray());

            if (entity_ALSerListResult.StrList_Error.Count > 0)
            {
                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = strError;
                errorMsgInfo.MsgType = MessageType.ValidationError;
                ViewBag.ActionMessage = errorMsgInfo;
            }

            return View(entityList_Result);
        }

        [HttpPost]
        public ActionResult Export(FormCollection collection)
        {
            string sort = "";
            string sortDir = "";
            if (collection.AllKeys.Contains("sort"))
            {
                sort = collection["sort"];
            }

            if (collection.AllKeys.Contains("sortDir"))
            {
                sortDir = collection["sortDir"];
            }

            //Message Box Title -- When Error occured, Message Box would be showed.
            string str_MsgBoxTitle = MultilingualHelper.GetStringFromResource(languageKey, "AuditLogManage_Export");

            ExportFileJsonResult commonJsonResult = new ExportFileJsonResult();
            commonJsonResult.MsgTitle = str_MsgBoxTitle;

            AuditLogVM selectionCriteria = new AuditLogVM();

            if (TempData.ContainsKey(SelectionCriteriaKey))
            {
                selectionCriteria = (AuditLogVM)TempData[SelectionCriteriaKey];
            }

            TempData[SelectionCriteriaKey] = selectionCriteria;
            ViewBag.SelectionCriteria = selectionCriteria;

            int recordCount = 0;

            List<AuditLogVM> entityList_Result = new List<AuditLogVM>();

            //Define wcf output object;
            ALSerListResult entity_ALSerListResult = null;

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(this, (entity_WCFAuthInfoVM) =>
            {
                entity_ALSerListResult = auditLogMgtHelper.Value.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, 1, int.MaxValue, sort, sortDir, CustomFilter(selectionCriteria));
            });

            //webCommonHelper.CallWCFHelper<IAuditLogMgtSer>(this, this.HttpContext, postOffice.AuditLogMgtSerPath, (entity_IAuditLogMgtSer, entity_WCFAuthInfoVM) =>
            //{
            //    entity_ALSerListResult = entity_IAuditLogMgtSer.GetListWithPaging(entity_WCFAuthInfoVM, selectionCriteria, 1, int.MaxValue, sort, sortDir, CustomFilter(selectionCriteria));
            //});

            //Assign data to local variable
            if (entity_ALSerListResult != null)
            {
                recordCount = entity_ALSerListResult.Int_TotalRecordCount;
                entityList_Result = entity_ALSerListResult.EntityList_AuditLogVM;
            }

            if (entityList_Result != null && entityList_Result.Count > 0)
            {
                RendDtlToExcelPolicy rendDtlToExcelPolicy = new RendDtlToExcelPolicy();

                List<string> strList_DisplayColumn = new List<string>();
                strList_DisplayColumn.Add("Operator");
                strList_DisplayColumn.Add("AL_CreateDate");
                strList_DisplayColumn.Add("AL_TableName");
                strList_DisplayColumn.Add("AL_EventType");
                strList_DisplayColumn.Add("AL_OriginalValue");
                strList_DisplayColumn.Add("AL_NewValue");

                Dictionary<string, string> dic_OutputMapping = new Dictionary<string, string>()
                {
                    {"Operator", MultilingualHelper.GetStringFromResource("Operator")},
                    {"AL_CreateDate", MultilingualHelper.GetStringFromResource("Date")},
                    {"AL_TableName", MultilingualHelper.GetStringFromResource("TableName")},
                    {"AL_EventType", MultilingualHelper.GetStringFromResource("OperationType")},
                    {"AL_OriginalValue", MultilingualHelper.GetStringFromResource("OriginalValue")},
                    {"AL_NewValue", MultilingualHelper.GetStringFromResource("NewValue")},
                    {"AL_RecordKey", MultilingualHelper.GetStringFromResource("RecordKey")},
                };

                MemoryStream memoryStream = rendDtlToExcelPolicy.ExportEntityListToXlsx<AuditLogVM>("AuditLog", entityList_Result, true, 1, 1, strList_DisplayColumn, dic_OutputMapping);

                string Key = Guid.NewGuid().ToString();

                TempData[Key] = memoryStream;

                commonJsonResult.ReturnUrl = Url.Action("ExportExcel", "Export", new { Area = "Common" }, Request.Url.Scheme);

                commonJsonResult.Success = true;

                commonJsonResult.Key = Key;
                commonJsonResult.OutputFileName = "AuditLog" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

                return Json(commonJsonResult);
            }
            else
            {
                commonJsonResult.ReturnUrl = Url.Action("Index", "AuditLogManage", new { Area = "AccessControl" }, Request.Url.Scheme);
                commonJsonResult.Success = false;

                MsgInfo errorMsgInfo = new MsgInfo();
                errorMsgInfo.MsgTitle = str_MsgBoxTitle;
                errorMsgInfo.MsgDesc = MultilingualHelper.GetStringFromResource(languageKey, "E019");
                errorMsgInfo.MsgType = MessageType.ValidationError;
                TempData[ActionMessageKey] = errorMsgInfo;
                return Json(commonJsonResult);
            }
        }
    }
}
