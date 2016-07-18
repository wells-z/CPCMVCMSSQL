/**************************************************************************
*
* NAME        : SysMgtControllerTest.cs
*
* VERSION     : 1.0.0
*
* DATE        : 25-Feb-2016
*
* DESCRIPTION : SysMgtControllerTest
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
    [TestCaseOrderer("CoolPrivilegeMVCUnitTest.PriorityOrderer", "CoolPrivilegeMVCUnitTest")]
    public class SysMgtControllerTest
    {
        public SystemInfoManageController systemInfoMgtController;

        public CommonFixture commonFixture;

        private List<FunctionVM> entityList_FunVM = new List<FunctionVM>();

        public SysMgtControllerTest(CommonFixture commonFixture)
        {
            this.commonFixture = commonFixture;

            //Generate Token
            commonFixture.LoginAndGetToken();

            systemInfoMgtController = new SystemInfoManageController(Bootstrapper.Container);

            systemInfoMgtController.ServiceAuthorizedKey = commonFixture.StrToken;

            SystemMgtHelper sysMgtHelper = Bootstrapper.Container.GetExportedValue<SystemMgtHelper>();

            systemInfoMgtController.systemMgtHelper = new Lazy<SystemMgtHelper>(() => sysMgtHelper);

            systemInfoMgtController.TempData[StaticContent.LanguageKey] = this.commonFixture.LanguageKey;

            systemInfoMgtController.FunKey = "SystemInfoManage";
        }

        [Fact]
        public void DisplaySysInfo()
        {
            systemInfoMgtController.FunTypeKey = "Edits";
            commonFixture.MockControllerInfo(systemInfoMgtController, "http://localhost:50653/AccessControl/SystemInfoManage/Index");

            ViewResult viewResult = (ViewResult)systemInfoMgtController.Index();

            Assert.NotNull(viewResult);

            Assert.NotNull(viewResult.Model);

            Assert.IsType<SystemInfoVM>(viewResult.Model);
        }

        [Fact]
        public void EditSysInfo()
        {
            systemInfoMgtController.FunTypeKey = "Edit";
            commonFixture.MockControllerInfo(systemInfoMgtController, "http://localhost:50653/AccessControl/SystemInfoManage/Edit");

            WCFAuthInfoVM entity_WCFAuthInfoVM = new WCFAuthInfoVM("", "", commonFixture.StrToken, systemInfoMgtController.FunKey, systemInfoMgtController.FunTypeKey, this.commonFixture.LanguageKey.ToString(), "");

            SystemInfoVM entity_SysInfoVM = systemInfoMgtController.systemMgtHelper.Value.GetSystemInfo(entity_WCFAuthInfoVM);

            entity_SysInfoVM.Password_IncludeDiffCase = false;
            entity_SysInfoVM.Password_IncludeNumDigit = false;
            entity_SysInfoVM.Password_IncludeSpecialChar = false;

            ActionResult actionResult = systemInfoMgtController.Edit(entity_SysInfoVM);

            Assert.IsType<RedirectToRouteResult>(actionResult);
        }

    }
}
