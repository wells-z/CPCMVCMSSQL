/**************************************************************************
*
* NAME        : OrgDetailsMgtControllerTest.cs
*
* VERSION     : 1.0.0
*
* DATE        : 25-Feb-2016
*
* DESCRIPTION : OrgDetailsMgtControllerTest
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
    public class OrgDetailsMgtControllerTest
    {
        public LUOrgDetailsManageController orgDetailsMgtController;

        public CommonFixture commonFixture;

        private List<LUserRoleVM> entityList_RoleVM = new List<LUserRoleVM>();
        private List<FunctionVM> entityList_FunVM = new List<FunctionVM>();

        public OrgDetailsMgtControllerTest(CommonFixture commonFixture)
        {
            this.commonFixture = commonFixture;

            //Generate Token
            commonFixture.LoginAndGetToken();

            orgDetailsMgtController = new LUOrgDetailsManageController(Bootstrapper.Container);

            orgDetailsMgtController.ServiceAuthorizedKey = commonFixture.StrToken;

            FunMgtHelper funMgtHelper = Bootstrapper.Container.GetExportedValue<FunMgtHelper>();
            OrgDetailMgtHelper orgDetailMgtHelper = Bootstrapper.Container.GetExportedValue<OrgDetailMgtHelper>();
            RoleMgtHelper roleMgtHelper = Bootstrapper.Container.GetExportedValue<RoleMgtHelper>();

            orgDetailsMgtController.funMgtHelper = new Lazy<FunMgtHelper>(() => funMgtHelper);
            orgDetailsMgtController.orgDetailMgtHelper = new Lazy<OrgDetailMgtHelper>(() => orgDetailMgtHelper);
            orgDetailsMgtController.roleMgtHelper = new Lazy<RoleMgtHelper>(() => roleMgtHelper);

            orgDetailsMgtController.TempData[StaticContent.LanguageKey] = this.commonFixture.LanguageKey;

            orgDetailsMgtController.FunKey = "LUOrgDetailsManage";
            orgDetailsMgtController.FunTypeKey = "Create";

            #region [ Initialize List ]
            WCFAuthInfoVM entity_WCFAuthInfoVM = new WCFAuthInfoVM("", "", commonFixture.StrToken, orgDetailsMgtController.FunKey, orgDetailsMgtController.FunTypeKey, this.commonFixture.LanguageKey.ToString(), "");
            //All Fun
            entityList_FunVM = orgDetailsMgtController.funMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);
            //All Role
            entityList_RoleVM = orgDetailsMgtController.roleMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);
            #endregion
        }

        [Fact, TestPriority(1)]
        public void CreateOrgDetails()
        {
            orgDetailsMgtController.FunTypeKey = "Create";
            commonFixture.MockControllerInfo(orgDetailsMgtController, "http://localhost:50653/AccessControl/LUOrgDetailsManage/Create");

            Random random = new Random();
            #region [ Create Organization Details with Specific Functions ]
            //00010001
            FunctionVM entity_FunctionVM = entityList_FunVM.Where(current => current.FunctionPath == "00010001").FirstOrDefault();

            string str_FunID = entity_FunctionVM.ID.ToString();

            FunDetailInfo entity_FunDetailInfo = new FunDetailInfo();

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(orgDetailsMgtController, (entity_WCFAuthInfoVM) =>
            {
                entity_FunDetailInfo = orgDetailsMgtController.funMgtHelper.Value.GetFunDetailInfo_FID(entity_WCFAuthInfoVM, str_FunID);
                entity_FunDetailInfo.FDSelected.ForEach(current => current = true);
            });

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

            List<FunDetailInfo> entityList_FunDetailInfo = new List<FunDetailInfo>();
            entityList_FunDetailInfo.Add(entity_FunDetailInfo);

            int r = random.Next(1, 1000);
            string str_OrgDetailsKey = "Test" + r;

            //User Role
            LUserOrgDetailsVM orgDetailsVM = new LUserOrgDetailsVM()
            {
                OrgDetailsKey = str_OrgDetailsKey,
                OrgDetailsType = 1,
                funDListJson = javaScriptSerializer.Serialize(entityList_FunDetailInfo)
            };

            ActionResult actionResult = orgDetailsMgtController.Create(orgDetailsVM);

            Assert.IsType<RedirectToRouteResult>(actionResult);
            #endregion

            #region [ Create Organization Details with Role ]
            r = random.Next(1, 1000);
            str_OrgDetailsKey = "Test" + r;
            orgDetailsVM = new LUserOrgDetailsVM()
            {
                OrgDetailsKey = str_OrgDetailsKey,
                OrgDetailsType = 2,
                roleListIDList = String.Join("|", entityList_RoleVM.Select(current => current.ID.ToString()).ToArray())
            };
            actionResult = orgDetailsMgtController.Create(orgDetailsVM);

            Assert.IsType<RedirectToRouteResult>(actionResult);
            #endregion
        }

        [Fact, TestPriority(2)]
        public void EditOrgDetails()
        {
            orgDetailsMgtController.FunTypeKey = "Edit";
            commonFixture.MockControllerInfo(orgDetailsMgtController, "http://localhost:50653/AccessControl/LUOrgDetailsManage/Edit");

            //00010001
            FunctionVM entity_FunctionVM = entityList_FunVM.Where(current => current.FunctionPath == "00010001").FirstOrDefault();

            string str_FunID = entity_FunctionVM.ID.ToString();

            FunDetailInfo entity_FunDetailInfo = new FunDetailInfo();

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            WCFAuthInfoVM entity_WCFAuthInfoVM = new WCFAuthInfoVM("", "", commonFixture.StrToken, orgDetailsMgtController.FunKey, orgDetailsMgtController.FunTypeKey, this.commonFixture.LanguageKey.ToString(), "");

            entity_FunDetailInfo = orgDetailsMgtController.funMgtHelper.Value.GetFunDetailInfo_FID(entity_WCFAuthInfoVM, str_FunID);
            entity_FunDetailInfo.FDSelected.ForEach(current => current = true);

            if (entity_FunDetailInfo.FDSelected.Count > 0)
                entity_FunDetailInfo.FDSelected[entity_FunDetailInfo.FDSelected.Count - 1] = false;

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

            List<FunDetailInfo> entityList_FunDetailInfo = new List<FunDetailInfo>();
            entityList_FunDetailInfo.Add(entity_FunDetailInfo);

            List<LUserOrgDetailsVM> entityList_OrgDetailsVM = orgDetailsMgtController.orgDetailMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);

            Assert.NotEmpty(entityList_OrgDetailsVM);

            LUserOrgDetailsVM entity_OrgDetailsVM = entityList_OrgDetailsVM.Where(current => current.OrgDetailsKey.IndexOf("Test") == 0).FirstOrDefault();

            Assert.NotNull(entity_OrgDetailsVM);

            entity_OrgDetailsVM.funDListJson = javaScriptSerializer.Serialize(entityList_FunDetailInfo);

            ActionResult actionResult = orgDetailsMgtController.Edit(entity_OrgDetailsVM);

            Assert.IsType<RedirectToRouteResult>(actionResult);
        }

        [Fact, TestPriority(3)]
        public void GetListByPaging()
        {
            orgDetailsMgtController.FunTypeKey = "View";
            commonFixture.MockControllerInfo(orgDetailsMgtController, "http://localhost:50653/AccessControl/LUOrgDetailsManage/Index");

            ViewResult viewResult = (ViewResult)orgDetailsMgtController.Index(1, "OrgDetailsKey", "DESC");

            Assert.NotNull(viewResult);

            Assert.NotEmpty((List<LUserOrgDetailsVM>)viewResult.Model);
        }

        [Fact, TestPriority(5)]
        public void Search()
        {
            orgDetailsMgtController.FunTypeKey = "Search";
            commonFixture.MockControllerInfo(orgDetailsMgtController, "http://localhost:50653/AccessControl/LUOrgDetailsManage/Index");

            LUserOrgDetailsVM entity_OrgDetailsVM = new LUserOrgDetailsVM();
            entity_OrgDetailsVM.OrgDetailsKey = "Read";

            ViewResult viewResult = (ViewResult)orgDetailsMgtController.Index(entity_OrgDetailsVM);

            Assert.NotNull(viewResult);

            Assert.NotEmpty((List<LUserOrgDetailsVM>)viewResult.Model);
        }

        [Fact, TestPriority(4)]
        public void DeleteOrgDetails()
        {
            orgDetailsMgtController.FunTypeKey = "Delete";
            commonFixture.MockControllerInfo(orgDetailsMgtController, "http://localhost:50653/AccessControl/LUOrgDetailsManage/Delete");

            WCFAuthInfoVM entity_WCFAuthInfoVM = new WCFAuthInfoVM("", "", commonFixture.StrToken, orgDetailsMgtController.FunKey, orgDetailsMgtController.FunTypeKey, this.commonFixture.LanguageKey.ToString(), "");

            List<LUserOrgDetailsVM> entityList_OrgDetailsVM = orgDetailsMgtController.orgDetailMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);

            Assert.NotEmpty(entityList_OrgDetailsVM);

            entityList_OrgDetailsVM = entityList_OrgDetailsVM.Where(current => current.OrgDetailsKey.IndexOf("Test") == 0).ToList();

            Assert.NotEmpty(entityList_OrgDetailsVM);

            foreach (var entity_OrgDetailsVM in entityList_OrgDetailsVM)
            {
                FormCollection formCollection = new FormCollection(new NameValueCollection
                {
                    {"ID",entity_OrgDetailsVM.ID.ToString()}
                });

                JsonResult jsonResult = (JsonResult)orgDetailsMgtController.Delete(formCollection);

                Assert.NotNull(jsonResult.Data);

                Assert.IsType<CommonJsonResult>(jsonResult.Data);

                Assert.True(((CommonJsonResult)jsonResult.Data).Success);
            }
        }
    }
}
