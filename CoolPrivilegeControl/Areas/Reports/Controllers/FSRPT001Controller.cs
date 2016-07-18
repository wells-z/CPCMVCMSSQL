/**************************************************************************
*
* NAME        : RPTFS001Controller.cs
*
* VERSION     : 1.0.0
*
* DATE        : 17-Jan-2016
*
* DESCRIPTION : RPTFS001Controller
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
using System.Web;
using System.Web.Mvc;
using UI_Infrastructure.CommonFilter.AuthorizationFilter;
using UI_Infrastructure.CustomHtmlHelper;
using UI_Infrastructure.ViewModels;
using CoolUtilities;
using UI_Infrastructure;
using System.Web.Script.Serialization;
using System.IO;
using CoolPrivilegeControlVM;
using CoolPrivilegeControlVM.CommonVM;
using UI_Infrastructure.ComController;


namespace CoolPrivilegeControl.Areas.Reports.Controllers
{
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [Export(typeof(IController))]
    [ExportMetadata("ControllerName", "FSRPT001")]
    [ExportMetadata("Order", 1)]
    public class FSRPT001Controller : CommonController, IController
    {
        #region [ Fields ]
        private CompositionContainer container
        {
            get;
            set;
        }

        private IDictionary<string, string> actionFTMapper = new Dictionary<string, string>()
        {
            {"Index_GET","Generate"}
        };
        #endregion

        [ImportingConstructor]
        public FSRPT001Controller(CompositionContainer container)
        {
            this.container = container;

            ViewData[StaticContent.ActionFTMapperKey] = actionFTMapper;
        }


        public ActionResult Index()
        {
            //WebCommonHelper CommonHelper = new WebCommonHelper();
            //List<SelectListItem> _list = new List<SelectListItem>();
            //_list.Add(new SelectListItem(){
            //    Value="1",
            //    Text="IS-ISLANDS DISTRICT 離島區"
            //});
            //_list.Add(new SelectListItem(){
            //    Value="2",
            //    Text="KT-KWAI TSING DISTRICT 葵青區"
            //});
            //_list.Add(new SelectListItem(){
            //    Value="3",
            //    Text="N-NORTH DISTRICT	北區"
            //});
            //_list.Add(new SelectListItem(){
            //    Value="4",
            //    Text="SK-SAI KUNG DISTRICT 西貢區"
            //});
            //_list.Add(new SelectListItem(){
            //    Value="5",
            //    Text="ST-SHATIN DISTRICT 沙田區"
            //});
            //_list.Add(new SelectListItem(){
            //    Value="6",
            //    Text="TM-TUEN MUN DISTRICT 屯門區"
            //});
            //_list.Add(new SelectListItem(){
            //    Value="7",
            //    Text="TP-TAI PO DISTRICT 大埔區"
            //});
            //_list.Add(new SelectListItem(){
            //    Value="8",
            //    Text="TW-TSUEN WAN DISTRICT	荃灣區"
            //});
            //_list.Add(new SelectListItem(){
            //    Value="9",
            //    Text="YL-YUEN LONG DISTRICT 元朗區"
            //});
            //ViewBag.DistrictList = new SelectList((IEnumerable<SelectListItem>)_list, "Value", "Text");
            return View();
        }

        [HttpPost]
        public ActionResult Generate(FormCollection collection)
        {
            string str_FromDt = collection["FromDate"];
            string str_ToDt = collection["ToDate"];

            ExportFileJsonResult commonJsonResult = new ExportFileJsonResult();

            string str_FileName = "FS-RPT-001";
            string Key = Guid.NewGuid().ToString();

            string sTemplatePath = System.Configuration.ConfigurationManager.AppSettings["templatePath"];
            string sFileRelativePath = @"~/" + sTemplatePath + @"Land Use report for current year by District (in all categories).xlsx";

            MemoryStream ms = new MemoryStream();
            using (FileStream file = new FileStream(Server.MapPath(sFileRelativePath), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] bytes = new byte[file.Length];
                file.Read(bytes, 0, (int)file.Length);
                ms.Write(bytes, 0, (int)file.Length);
            }

            TempData[Key] = ms;
            commonJsonResult = new ExportFileJsonResult();
            commonJsonResult.MsgTitle = "Export FSRPT001";
            commonJsonResult.ReturnUrl = Url.Action("ExportExcel", "Export", new { Area = "Common" }, Request.Url.Scheme);
            commonJsonResult.Success = true;
            commonJsonResult.Key = Key;
            commonJsonResult.OutputFileName = str_FileName + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            //WebCommonHelper CommonHelper = new WebCommonHelper();
            //List<SelectListItem> _list = new List<SelectListItem>();
            //_list.Add(new SelectListItem()
            //{
            //    Value = "1",
            //    Text = "IS-ISLANDS DISTRICT 離島區"
            //});
            //_list.Add(new SelectListItem()
            //{
            //    Value = "2",
            //    Text = "KT-KWAI TSING DISTRICT 葵青區"
            //});
            //_list.Add(new SelectListItem()
            //{
            //    Value = "3",
            //    Text = "N-NORTH DISTRICT	北區"
            //});
            //_list.Add(new SelectListItem()
            //{
            //    Value = "4",
            //    Text = "SK-SAI KUNG DISTRICT 西貢區"
            //});
            //_list.Add(new SelectListItem()
            //{
            //    Value = "5",
            //    Text = "ST-SHATIN DISTRICT 沙田區"
            //});
            //_list.Add(new SelectListItem()
            //{
            //    Value = "6",
            //    Text = "TM-TUEN MUN DISTRICT 屯門區"
            //});
            //_list.Add(new SelectListItem()
            //{
            //    Value = "7",
            //    Text = "TP-TAI PO DISTRICT 大埔區"
            //});
            //_list.Add(new SelectListItem()
            //{
            //    Value = "8",
            //    Text = "TW-TSUEN WAN DISTRICT	荃灣區"
            //});
            //_list.Add(new SelectListItem()
            //{
            //    Value = "9",
            //    Text = "YL-YUEN LONG DISTRICT 元朗區"
            //});
            //ViewBag.DistrictList = new SelectList((IEnumerable<SelectListItem>)_list, "Value", "Text");
            return View();
        }
    }
}