/**************************************************************************
*
* NAME        : AuditLogMgtControllerTest.cs
*
* VERSION     : 1.0.0
*
* DATE        : 25-Feb-2016
*
* DESCRIPTION : AuditLogMgtControllerTest
*
* MODIFICATION HISTORY
* Name             Date         Description
* ===============  ===========  =======================================
* Wells Cheung     25-Feb-2016  Initial Version
*
**************************************************************************/
using System;
using System.Text;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Hosting;
using UI_Infrastructure;
using CoolUtilities.MultiLingual;
using CoolPrivilegeControlVM.EntityVM;
using CoolPrivilegeControlVM.CommonVM;
using CoolPrivilegeControl.Areas.AccessControl.Controllers;
using CoolPrivilegeControlVM.WCFVM;
using CoolPrivilegeControl.Models;
using System.Web.Routing;
using CoolPrivilegeControl;
using CoolPrivilegeControl.Areas.AccessControl;
using System.Web.Mvc;
using CoolPrivilegeControlSerClient;
using CoolPrivilegeControlVM.WCFVM.LoginUserSerVM;
using UI_Infrastructure.ViewModels;
using CoolPrivilegeControlVM.SessionMaintenance;
using UI_Infrastructure.CustomHtmlHelper;
using System.Web.Script.Serialization;
using System.Collections.Specialized;

namespace CoolPrivilegeMVCUnitTest.TestCases
{
    [Collection("Common collection")]
    public class AuditLogMgtControllerTest
    {
        public AuditLogManageController auditLogMgtController;

        public CommonFixture commonFixture;

        public AuditLogMgtControllerTest(CommonFixture commonFixture)
        {
            this.commonFixture = commonFixture;

            //Generate Token
            commonFixture.LoginAndGetToken();

            AuditLogMgtHelper auditLogMgtHelper = Bootstrapper.Container.GetExportedValue<AuditLogMgtHelper>();

            auditLogMgtController = new AuditLogManageController(Bootstrapper.Container);

            auditLogMgtController.ServiceAuthorizedKey = commonFixture.StrToken;

            auditLogMgtController.auditLogMgtHelper = new Lazy<AuditLogMgtHelper>(() => auditLogMgtHelper);

            auditLogMgtController.TempData[StaticContent.LanguageKey] = this.commonFixture.LanguageKey;

            auditLogMgtController.FunKey = "AuditLogManage";
        }

        [Fact]
        public void GetListByPaging()
        {
            auditLogMgtController.FunTypeKey = "View";
            commonFixture.MockControllerInfo(auditLogMgtController, "http://localhost:50653/AccessControl/AuditLogManage/Index");

            ViewResult viewResult = (ViewResult)auditLogMgtController.Index(1, "AL_CreateDate", "asc");

            Assert.NotNull(viewResult);

            Assert.NotEmpty((List<AuditLogVM>)viewResult.Model);
        }

        [Fact]
        public void Search()
        {
            auditLogMgtController.FunTypeKey = "Search";
            commonFixture.MockControllerInfo(auditLogMgtController, "http://localhost:50653/AccessControl/AuditLogManage/Index");

            AuditLogVM entity_AuditLogVM = new AuditLogVM();
            entity_AuditLogVM.Operator = "admin";

            ViewResult viewResult = (ViewResult)auditLogMgtController.Index(entity_AuditLogVM);

            Assert.NotNull(viewResult);

            Assert.NotEmpty((List<AuditLogVM>)viewResult.Model);
        }
    }
}
