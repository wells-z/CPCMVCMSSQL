/**************************************************************************
*
* NAME        : OrgMgtControllerTest.cs
*
* VERSION     : 1.0.0
*
* DATE        : 25-Feb-2016
*
* DESCRIPTION : OrgMgtControllerTest
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
    public class OrgMgtControllerTest
    {
        public LUOrganizationManageController orgMgtController;

        public CommonFixture commonFixture;

        public OrgMgtControllerTest(CommonFixture commonFixture)
        {
            this.commonFixture = commonFixture;

            //Generate Token
            commonFixture.LoginAndGetToken();

            orgMgtController = new LUOrganizationManageController(Bootstrapper.Container);

            orgMgtController.ServiceAuthorizedKey = commonFixture.StrToken;

            FunMgtHelper funMgtHelper = Bootstrapper.Container.GetExportedValue<FunMgtHelper>();
            OrgDetailMgtHelper orgDetailMgtHelper = Bootstrapper.Container.GetExportedValue<OrgDetailMgtHelper>();
            OrgMgtHelper orgMgtHelper = Bootstrapper.Container.GetExportedValue<OrgMgtHelper>();
            RoleMgtHelper roleMgtHelper = Bootstrapper.Container.GetExportedValue<RoleMgtHelper>();

            orgMgtController.funMgtHelper = new Lazy<FunMgtHelper>(() => funMgtHelper);
            orgMgtController.orgDetailMgtHelper = new Lazy<OrgDetailMgtHelper>(() => orgDetailMgtHelper);
            orgMgtController.orgMgtHelper = new Lazy<OrgMgtHelper>(() => orgMgtHelper);
            orgMgtController.roleMgtHelper = new Lazy<RoleMgtHelper>(() => roleMgtHelper);

            orgMgtController.TempData[StaticContent.LanguageKey] = this.commonFixture.LanguageKey;

            orgMgtController.FunKey = "LUOrganizationManage";
        }

        [Fact, TestPriority(1)]
        public void CreateOrg()
        {
            orgMgtController.FunTypeKey = "Create";
            commonFixture.MockControllerInfo(orgMgtController, "http://localhost:50653/AccessControl/LUOrganizationManage/Create");

            LUserOrganizationVM orgVM = new LUserOrganizationVM()
            {
                OrganizationPath = "1100",
                OrganizationKey = "1100",
                OrganizationName = "1100",
                OrganizationStatus = 1
            };

            ActionResult result = orgMgtController.Create(orgVM);

            Assert.IsType<RedirectToRouteResult>(result);
        }

        [Fact, TestPriority(2)]
        public void EditOrg()
        {
            orgMgtController.FunTypeKey = "Edit";
            commonFixture.MockControllerInfo(orgMgtController, "http://localhost:50653/AccessControl/LUOrganizationManage/Edit");

            WCFAuthInfoVM entity_WCFAuthInfoVM = new WCFAuthInfoVM("", "", commonFixture.StrToken, orgMgtController.FunKey, orgMgtController.FunTypeKey, this.commonFixture.LanguageKey.ToString(), "");

            List<LUserOrganizationVM> entityList_OrgVM = orgMgtController.orgMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);

            Assert.NotEmpty(entityList_OrgVM);

            LUserOrganizationVM entity_OrgVM = entityList_OrgVM.Where(current => current.OrganizationPath == "1100").FirstOrDefault();

            Assert.NotNull(entity_OrgVM);

            entity_OrgVM.OrganizationKey = "Test1100";

            ActionResult actionResult = orgMgtController.Edit(entity_OrgVM);

            Assert.IsType<RedirectToRouteResult>(actionResult);
        }

        [Fact, TestPriority(3)]
        public void GetListByPaging()
        {
            orgMgtController.FunTypeKey = "View";
            commonFixture.MockControllerInfo(orgMgtController, "http://localhost:50653/AccessControl/LUOrganizationManage/Index");

            ViewResult viewResult = (ViewResult)orgMgtController.Index(1, "OrganizationPath", "DESC");

            Assert.NotNull(viewResult);

            Assert.NotEmpty((List<LUserOrganizationVM>)viewResult.Model);
        }

        [Fact, TestPriority(5)]
        public void Search()
        {
            orgMgtController.FunTypeKey = "Search";
            commonFixture.MockControllerInfo(orgMgtController, "http://localhost:50653/AccessControl/LUOrganizationManage/Index");

            LUserOrganizationVM entity_OrgVM = new LUserOrganizationVM();
            entity_OrgVM.OrganizationPath = "0001";

            ViewResult viewResult = (ViewResult)orgMgtController.Index(entity_OrgVM);

            Assert.NotNull(viewResult);

            Assert.NotEmpty((List<LUserOrganizationVM>)viewResult.Model);
        }

        [Fact, TestPriority(4)]
        public void DeleteOrg()
        {
            orgMgtController.FunTypeKey = "Delete";
            commonFixture.MockControllerInfo(orgMgtController, "http://localhost:50653/AccessControl/LUOrganizationManage/Delete");

            WCFAuthInfoVM entity_WCFAuthInfoVM = new WCFAuthInfoVM("", "", commonFixture.StrToken, orgMgtController.FunKey, orgMgtController.FunTypeKey, this.commonFixture.LanguageKey.ToString(), "");

            List<LUserOrganizationVM> entityList_OrgVM = orgMgtController.orgMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);

            Assert.NotEmpty(entityList_OrgVM);

            LUserOrganizationVM entity_OrgVM = entityList_OrgVM.Where(current => current.OrganizationPath == "1100").FirstOrDefault();

            Assert.NotNull(entity_OrgVM);

            FormCollection formCollection = new FormCollection(new NameValueCollection
            {
                {"ID",entity_OrgVM.ID.ToString()}
            });

            JsonResult jsonResult = (JsonResult)orgMgtController.Delete(formCollection);

            Assert.NotNull(jsonResult.Data);

            Assert.IsType<CommonJsonResult>(jsonResult.Data);

            Assert.True(((CommonJsonResult)jsonResult.Data).Success);
        }
    }
}
