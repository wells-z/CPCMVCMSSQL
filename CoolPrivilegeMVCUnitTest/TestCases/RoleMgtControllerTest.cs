/**************************************************************************
*
* NAME        : RoleMgtControllerTest.cs
*
* VERSION     : 1.0.0
*
* DATE        : 25-Feb-2016
*
* DESCRIPTION : RoleMgtControllerTest
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
    public class RoleMgtControllerTest
    {
        public LURoleManageController roleManageController;

        public CommonFixture commonFixture;

        private List<FunctionVM> entityList_FunVM = new List<FunctionVM>();

        public RoleMgtControllerTest(CommonFixture commonFixture)
        {
            this.commonFixture = commonFixture;

            //Generate Token
            commonFixture.LoginAndGetToken();

            roleManageController = new LURoleManageController(Bootstrapper.Container);

            roleManageController.ServiceAuthorizedKey = commonFixture.StrToken;

            FunTypeMgtHelper funTypeMgtHelper = Bootstrapper.Container.GetExportedValue<FunTypeMgtHelper>();
            FunMgtHelper funMgtHelper = Bootstrapper.Container.GetExportedValue<FunMgtHelper>();
            OrgDetailMgtHelper orgDetailMgtHelper = Bootstrapper.Container.GetExportedValue<OrgDetailMgtHelper>();
            RoleMgtHelper roleMgtHelper = Bootstrapper.Container.GetExportedValue<RoleMgtHelper>();

            roleManageController.funMgtHelper = new Lazy<FunMgtHelper>(() => funMgtHelper);
            roleManageController.orgDetailMgtHelper = new Lazy<OrgDetailMgtHelper>(() => orgDetailMgtHelper);
            roleManageController.roleMgtHelper = new Lazy<RoleMgtHelper>(() => roleMgtHelper);

            roleManageController.TempData[StaticContent.LanguageKey] = this.commonFixture.LanguageKey;

            roleManageController.FunKey = "LURoleManage";
            roleManageController.FunTypeKey = "Create";

            WCFAuthInfoVM entity_WCFAuthInfoVM = new WCFAuthInfoVM("", "", commonFixture.StrToken, roleManageController.FunKey, roleManageController.FunTypeKey, this.commonFixture.LanguageKey.ToString(), "");
            //All Fun
            entityList_FunVM = roleManageController.funMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);
        }

        [Fact, TestPriority(1)]
        public void CreateRole()
        {
            roleManageController.FunTypeKey = "Create";
            commonFixture.MockControllerInfo(roleManageController, "http://localhost:50653/AccessControl/LURoleManage/Create");

            Random random = new Random();
            #region [ Create User Role with Specific Functions ]
            //00010001
            FunctionVM entity_FunctionVM = entityList_FunVM.Where(current => current.FunctionPath == "00010001").FirstOrDefault();

            string str_FunID = entity_FunctionVM.ID.ToString();

            FunDetailInfo entity_FunDetailInfo = new FunDetailInfo();

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(roleManageController, (entity_WCFAuthInfoVM) =>
            {
                entity_FunDetailInfo = roleManageController.funMgtHelper.Value.GetFunDetailInfo_FID(entity_WCFAuthInfoVM, str_FunID);
                entity_FunDetailInfo.FDSelected.ForEach(current => current = true);
            });

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

            List<FunDetailInfo> entityList_FunDetailInfo = new List<FunDetailInfo>();
            entityList_FunDetailInfo.Add(entity_FunDetailInfo);

            int r = random.Next(1, 1000);
            string str_RoleName = "Test" + r;

            //User Role
            LUserRoleVM orgVM = new LUserRoleVM()
            {
                RoleName = str_RoleName,
                funDListJson = javaScriptSerializer.Serialize(entityList_FunDetailInfo)
            };

            ActionResult actionResult = roleManageController.Create(orgVM);

            Assert.IsType<RedirectToRouteResult>(actionResult);
            #endregion
        }

        [Fact, TestPriority(2)]
        public void EditRole()
        {
            roleManageController.FunTypeKey = "Edit";
            commonFixture.MockControllerInfo(roleManageController, "http://localhost:50653/AccessControl/LURoleManage/Edit");

            //00010001
            FunctionVM entity_FunctionVM = entityList_FunVM.Where(current => current.FunctionPath == "00010001").FirstOrDefault();

            string str_FunID = entity_FunctionVM.ID.ToString();

            FunDetailInfo entity_FunDetailInfo = new FunDetailInfo();

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            WCFAuthInfoVM entity_WCFAuthInfoVM = new WCFAuthInfoVM("", "", commonFixture.StrToken, roleManageController.FunKey, roleManageController.FunTypeKey, this.commonFixture.LanguageKey.ToString(), "");

            entity_FunDetailInfo = roleManageController.funMgtHelper.Value.GetFunDetailInfo_FID(entity_WCFAuthInfoVM, str_FunID);
            entity_FunDetailInfo.FDSelected.ForEach(current => current = true);

            if (entity_FunDetailInfo.FDSelected.Count > 0)
                entity_FunDetailInfo.FDSelected[entity_FunDetailInfo.FDSelected.Count - 1] = false;

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

            List<FunDetailInfo> entityList_FunDetailInfo = new List<FunDetailInfo>();
            entityList_FunDetailInfo.Add(entity_FunDetailInfo);

            List<LUserRoleVM> entityList_RoleVM = roleManageController.roleMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);

            Assert.NotEmpty(entityList_RoleVM);

            LUserRoleVM entity_RoleVM = entityList_RoleVM.Where(current => current.RoleName.IndexOf("Test") == 0).FirstOrDefault();

            Assert.NotNull(entity_RoleVM);

            entity_RoleVM.funDListJson = javaScriptSerializer.Serialize(entityList_FunDetailInfo);

            ActionResult actionResult = roleManageController.Edit(entity_RoleVM);

            Assert.IsType<RedirectToRouteResult>(actionResult);
        }

        [Fact, TestPriority(3)]
        public void GetListByPaging()
        {
            roleManageController.FunTypeKey = "View";
            commonFixture.MockControllerInfo(roleManageController, "http://localhost:50653/AccessControl/LURoleManage/Index");

            ViewResult viewResult = (ViewResult)roleManageController.Index(1, "RoleName", "DESC");

            Assert.NotNull(viewResult);

            Assert.NotEmpty((List<LUserRoleVM>)viewResult.Model);
        }

        [Fact, TestPriority(5)]
        public void Search()
        {
            roleManageController.FunTypeKey = "Search";
            commonFixture.MockControllerInfo(roleManageController, "http://localhost:50653/AccessControl/LURoleManage/Index");

            LUserRoleVM entity_RoleVM = new LUserRoleVM();
            entity_RoleVM.RoleName = "Admin";

            ViewResult viewResult = (ViewResult)roleManageController.Index(entity_RoleVM);

            Assert.NotNull(viewResult);

            Assert.NotEmpty((List<LUserRoleVM>)viewResult.Model);
        }

        [Fact, TestPriority(4)]
        public void DeleteRole()
        {
            roleManageController.FunTypeKey = "Delete";
            commonFixture.MockControllerInfo(roleManageController, "http://localhost:50653/AccessControl/LURoleManage/Delete");

            WCFAuthInfoVM entity_WCFAuthInfoVM = new WCFAuthInfoVM("", "", commonFixture.StrToken, roleManageController.FunKey, roleManageController.FunTypeKey, this.commonFixture.LanguageKey.ToString(), "");

            List<LUserRoleVM> entityList_OrgVM = roleManageController.roleMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);

            Assert.NotEmpty(entityList_OrgVM);

            LUserRoleVM entity_RoleVM = entityList_OrgVM.Where(current => current.RoleName.IndexOf("Test") == 0).FirstOrDefault();

            Assert.NotNull(entity_RoleVM);

            FormCollection formCollection = new FormCollection(new NameValueCollection
            {
                {"ID",entity_RoleVM.ID.ToString()}
            });

            JsonResult jsonResult = (JsonResult)roleManageController.Delete(formCollection);

            Assert.NotNull(jsonResult.Data);

            Assert.IsType<CommonJsonResult>(jsonResult.Data);

            Assert.True(((CommonJsonResult)jsonResult.Data).Success);
        }
    }
}
