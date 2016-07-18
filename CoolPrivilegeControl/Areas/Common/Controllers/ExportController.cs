/**************************************************************************
*
* NAME        : ExportController.cs
*
* VERSION     : 1.0.0
*
* DATE        : 17-Jan-2016
*
* DESCRIPTION : ExportController
*
* MODIFICATION HISTORY
* Name             Date         Description
* ===============  ===========  =======================================
* Wells Cheung     17-Jan-2016  Initial Version
*
**************************************************************************/
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Web;
using System.Web.Mvc;
using UI_Infrastructure.CommonFilter.AuthorizationFilter;
using System.IO;
using System.Text;

namespace CoolPrivilegeControl.Areas.Common.Controllers
{
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [Export(typeof(IController))]
    [ExportMetadata("ControllerName", "Export")]
    [ExportMetadata("Order", 1)]
    public class ExportController : Controller
    {

        private CompositionContainer container
        {
            get;
            set;
        }

        [ImportingConstructor]
        public ExportController(CompositionContainer container)
        {
            this.container = container;
        }

        [UnAuthorization]
        public ActionResult ExportExcel(string OutputFileName, string Key = "", string FilePath = "")
        {
            if (!string.IsNullOrWhiteSpace(Key))
            {
                if (TempData.ContainsKey(Key) && TempData[Key] != null)
                {
                    return new ExportFileResult()
                    {
                        FileName = OutputFileName,
                        DataStream = (MemoryStream)TempData[Key],
                    };
                }
            }
            return View();
        }
    }

    public class ExportFileResult : ActionResult
    {
        public string FileName { get; set; }
        public MemoryStream DataStream { get; set; }
        public string FilePath { get; set; }
        public ExportFileResult() { }

        public override void ExecuteResult(ControllerContext context)
        {
            if (string.IsNullOrWhiteSpace(this.FileName))
            {
                this.FileName = string.Concat("ExportData_", DateTime.Now.ToString("yyyyMMddHHmmss"), ".xlsx");
            }
            this.ExportFileEventHandler(context);
        }

        private void ExportFileEventHandler(ControllerContext context)
        {
            try
            {
                if (this.DataStream != null)
                {
                    context.HttpContext.Response.Clear();
                    // Econding                    
                    context.HttpContext.Response.ContentEncoding = Encoding.UTF8;
                    // Set ContentType                   
                    //context.HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    context.HttpContext.Response.ContentType = "application/octet-stream";
                    // Output File Name                    
                    var browser = context.HttpContext.Request.Browser.Browser;
                    var exportFileName = browser.Equals("Firefox", StringComparison.OrdinalIgnoreCase) ? this.FileName : HttpUtility.UrlEncode(this.FileName, Encoding.UTF8);
                    context.HttpContext.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", exportFileName));

                    DataStream.Position = 0;
                    DataStream.WriteTo(context.HttpContext.Response.OutputStream);
                    DataStream.Close();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}