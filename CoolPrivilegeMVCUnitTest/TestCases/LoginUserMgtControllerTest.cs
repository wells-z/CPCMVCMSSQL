/**************************************************************************
*
* NAME        : LoginUserMgtHelper.cs
*
* VERSION     : 1.0.0
*
* DATE        : 25-Feb-2016
*
* DESCRIPTION : LoginUserMgtHelper
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

namespace CoolPrivilegeMVCUnitTest
{
    [Collection("Common collection")]
    [TestCaseOrderer("CoolPrivilegeMVCUnitTest.PriorityOrderer", "CoolPrivilegeMVCUnitTest")]
    public class LoginUserMgtControllerTest
    {
        public LoginUserManageController loginUserMgtController;

        public CommonFixture commonFixture;

        private List<FunctionVM> entityList_FunVM = new List<FunctionVM>();
        private List<LUserOrganizationVM> entityList_OrgVM = new List<LUserOrganizationVM>();
        private List<LUserOrgDetailsVM> entityList_OrgDetailsVM = new List<LUserOrgDetailsVM>();
        private List<LUserRoleVM> entityList_RoleVM = new List<LUserRoleVM>();

        public LoginUserMgtControllerTest(CommonFixture commonFixture)
        {
            this.commonFixture = commonFixture;

            //Generate Token
            commonFixture.LoginAndGetToken();

            LoginUserMgtHelper luMgtHelper = Bootstrapper.Container.GetExportedValue<LoginUserMgtHelper>();
            FunMgtHelper funMgtHelper = Bootstrapper.Container.GetExportedValue<FunMgtHelper>();
            OrgDetailMgtHelper orgDetailMgtHelper = Bootstrapper.Container.GetExportedValue<OrgDetailMgtHelper>();
            OrgMgtHelper orgMgtHelper = Bootstrapper.Container.GetExportedValue<OrgMgtHelper>();
            RoleMgtHelper roleMgtHelper = Bootstrapper.Container.GetExportedValue<RoleMgtHelper>();

            loginUserMgtController = new LoginUserManageController(Bootstrapper.Container);

            loginUserMgtController.loginUserMgtHelper = new Lazy<LoginUserMgtHelper>(() => luMgtHelper);
            loginUserMgtController.orgDetailMgtHelper = new Lazy<OrgDetailMgtHelper>(() => orgDetailMgtHelper);
            loginUserMgtController.orgMgtHelper = new Lazy<OrgMgtHelper>(() => orgMgtHelper);
            loginUserMgtController.roleMgtHelper = new Lazy<RoleMgtHelper>(() => roleMgtHelper);
            loginUserMgtController.funMgtHelper = new Lazy<FunMgtHelper>(() => funMgtHelper);

            loginUserMgtController.TempData[StaticContent.LanguageKey] = this.commonFixture.LanguageKey;

            #region [ Initialize List ]
            loginUserMgtController.FunKey = "LoginUserManage";
            loginUserMgtController.FunTypeKey = "Create";
            loginUserMgtController.ServiceAuthorizedKey = commonFixture.StrToken;

            WCFAuthInfoVM entity_WCFAuthInfoVM = new WCFAuthInfoVM("", "", commonFixture.StrToken, loginUserMgtController.FunKey, loginUserMgtController.FunTypeKey, this.commonFixture.LanguageKey.ToString(), "");
            //All Fun
            entityList_FunVM = loginUserMgtController.funMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);
            //All Org
            entityList_OrgVM = loginUserMgtController.orgMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);
            //All Org Details
            entityList_OrgDetailsVM = loginUserMgtController.orgDetailMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);
            //All Role
            entityList_RoleVM = loginUserMgtController.roleMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);

            #endregion

            loginUserMgtController.guidList_AccessedOrgID = loginUserMgtController.GetOrgIDList(commonFixture.entity_BaseSession, "LoginUserManage", "Create", entityList_OrgVM);
            loginUserMgtController.guidList_AccessedLUserID = loginUserMgtController.GetLUserIDList(commonFixture.entity_BaseSession, "LoginUserManage", "Create", luMgtHelper, entity_WCFAuthInfoVM);
        }

        [Fact, TestPriority(1)]
        public void CreateLoginUser()
        {
            loginUserMgtController.FunTypeKey = "Create";
            commonFixture.MockControllerInfo(loginUserMgtController, "http://localhost:50653/AccessControl/LoginUserManage/Create");

            Random random = new Random();
            #region [ Create Login User with Specific Functions ]
            //00010001
            FunctionVM entity_FunVM = entityList_FunVM.Where(current => current.FunctionPath == "00010001").FirstOrDefault();

            string str_FunID = entity_FunVM.ID.ToString();

            FunDetailInfo entity_FunDetailInfo = new FunDetailInfo();

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(loginUserMgtController, (entity_WCFAuthInfoVM) =>
            {
                entity_FunDetailInfo = loginUserMgtController.funMgtHelper.Value.GetFunDetailInfo_FID(entity_WCFAuthInfoVM, str_FunID);
                entity_FunDetailInfo.FDSelected.ForEach(current => current = true);
            });

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

            List<FunDetailInfo> entityList_FunDetailInfo = new List<FunDetailInfo>();
            entityList_FunDetailInfo.Add(entity_FunDetailInfo);

            int r = random.Next(1, 1000);
            string str_LoginName = "Test" + r;

            //LoginUser
            LoginUserVM loginUserVM = new LoginUserVM()
            {
                LoginName = str_LoginName,
                NewPwd = "A12346.b",
                ConfirmNewPwd = "A12346.b",
                Status = 1,
                UserType = 1,
                funDListJson = javaScriptSerializer.Serialize(entityList_FunDetailInfo)
            };

            ActionResult actionResult = loginUserMgtController.Create(loginUserVM);

            Assert.IsType<RedirectToRouteResult>(actionResult);
            #endregion

            #region [ Create Login User with Role ]
            r = random.Next(1, 1000);
            str_LoginName = "Test" + r;
            loginUserVM = new LoginUserVM()
            {
                LoginName = str_LoginName,
                NewPwd = "A12346.b",
                ConfirmNewPwd = "A12346.b",
                Status = 1,
                UserType = 2,
                roleListIDList = String.Join("|", entityList_RoleVM.Select(current => current.ID.ToString()).ToArray())
            };
            actionResult = loginUserMgtController.Create(loginUserVM);

            Assert.IsType<RedirectToRouteResult>(actionResult);
            #endregion

            #region [ Create Login User with Org ]
            List<string> strList_Org = entityList_OrgVM.Where(current => current.OrganizationPath == "00010001" || current.OrganizationPath == "000100010002").Select(current => current.ID.ToString()).ToList();

            List<string> strList_OrgDetail = entityList_OrgDetailsVM.Select(current => current.ID.ToString()).ToList();

            r = random.Next(1, 1000);
            str_LoginName = "Test" + r;
            loginUserVM = new LoginUserVM()
            {
                LoginName = str_LoginName,
                NewPwd = "A12346.b",
                ConfirmNewPwd = "A12346.b",
                Status = 1,
                UserType = 3,
                orgListIDList = String.Join("|", strList_Org),
                orgDetailsIDList = String.Join("|", strList_OrgDetail)
            };
            actionResult = loginUserMgtController.Create(loginUserVM);

            Assert.IsType<RedirectToRouteResult>(actionResult);
            #endregion
        }

        [Fact, TestPriority(2)]
        public void EditLoginUser()
        {
            loginUserMgtController.FunTypeKey = "Edit";
            commonFixture.MockControllerInfo(loginUserMgtController, "http://localhost:50653/AccessControl/LoginUserManage/Edit");

            WCFAuthInfoVM entity_WCFAuthInfoVM = new WCFAuthInfoVM("", "", commonFixture.StrToken, "LoginUserManage", "Edit", this.commonFixture.LanguageKey.ToString(), "");

            LUSerListResult entity_LUSerListResult = loginUserMgtController.loginUserMgtHelper.Value.GetListWithPaging(entity_WCFAuthInfoVM, new LoginUserVM(), 1, 10, "", "", new List<string>(), loginUserMgtController.guidList_AccessedLUserID);

            LoginUserVM entity_LUVM = null;

            if (entity_LUSerListResult != null)
            {
                entity_LUVM = entity_LUSerListResult.EntityList_LoginUserVM.Where(current => current.LoginName.IndexOf("Test") == 0).FirstOrDefault();
            }

            Assert.NotNull(entity_LUVM);

            entity_LUVM.UserType = 2;
            entity_LUVM.roleListIDList = String.Join("|", entityList_RoleVM.Select(current => current.ID.ToString()).ToArray());

            ActionResult actionResult = loginUserMgtController.Edit(entity_LUVM);

            Assert.IsType<RedirectToRouteResult>(actionResult);
        }

        [Fact, TestPriority(3)]
        public void GetListByPaging()
        {
            loginUserMgtController.FunTypeKey = "View";
            commonFixture.MockControllerInfo(loginUserMgtController, "http://localhost:50653/AccessControl/LoginUserManage/Index");

            ViewResult viewResult = (ViewResult)loginUserMgtController.Index(1, "LoginName", "DESC");

            Assert.NotNull(viewResult);

            Assert.NotEmpty((List<LoginUserVM>)viewResult.Model);
        }

        [Fact, TestPriority(4)]
        public void DeleteLoginUser()
        {
            loginUserMgtController.FunTypeKey = "Delete";
            commonFixture.MockControllerInfo(loginUserMgtController, "http://localhost:50653/AccessControl/LoginUserManage/Delete");

            WCFAuthInfoVM entity_WCFAuthInfoVM = new WCFAuthInfoVM("", "", commonFixture.StrToken, "LoginUserManage", "Delete", this.commonFixture.LanguageKey.ToString(), "");

            loginUserMgtController.guidList_AccessedLUserID = loginUserMgtController.GetLUserIDList(commonFixture.entity_BaseSession, "LoginUserManage", "Create", loginUserMgtController.loginUserMgtHelper.Value, entity_WCFAuthInfoVM);

            LUSerListResult entity_LUSerListResult = loginUserMgtController.loginUserMgtHelper.Value.GetListWithPaging(entity_WCFAuthInfoVM, new LoginUserVM(), 1, int.MaxValue, "", "", new List<string>(), loginUserMgtController.guidList_AccessedLUserID);

            Assert.NotEmpty(entity_LUSerListResult.EntityList_LoginUserVM);


            List<LoginUserVM> entityList_LUVM = entity_LUSerListResult.EntityList_LoginUserVM.Where(current => current.LoginName.IndexOf("Test") == 0).ToList();

            foreach (var item in entityList_LUVM)
            {
                FormCollection formCollection = new FormCollection(new NameValueCollection
                {
                    {"UserID",item.ID.ToString()}
                });

                JsonResult jsonResult = (JsonResult)loginUserMgtController.Delete(formCollection);

                Assert.NotNull(jsonResult.Data);

                Assert.IsType<CommonJsonResult>(jsonResult.Data);

                Assert.True(((CommonJsonResult)jsonResult.Data).Success);
            }
        }

        [Fact, TestPriority(5)]
        public void Search()
        {
            loginUserMgtController.FunTypeKey = "Search";
            commonFixture.MockControllerInfo(loginUserMgtController, "http://localhost:50653/AccessControl/LoginUserManage/Index");

            LoginUserVM entity_LUVM = new LoginUserVM();
            entity_LUVM.LoginName = "Admin";

            ViewResult viewResult = (ViewResult)loginUserMgtController.Index(entity_LUVM);

            Assert.NotNull(viewResult);

            Assert.NotEmpty((List<LoginUserVM>)viewResult.Model);
        }
    }
}
