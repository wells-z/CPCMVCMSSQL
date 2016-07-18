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
    public class LoginUserMgtControllerTest
    {
        public LoginUserManageController loginUserMgtController;

        public CommonFixture commonFixture;

        private List<FunctionVM> entityList_FunctionVM = new List<FunctionVM>();
        private List<LUserOrganizationVM> entityList_LUserOrganizationVM = new List<LUserOrganizationVM>();
        private List<LUserOrgDetailsVM> entityList_LUserOrgDetailsVM = new List<LUserOrgDetailsVM>();
        private List<LUserRoleVM> entityList_LUserRoleVM = new List<LUserRoleVM>();

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

            WCFSessionVM entity_WCFSessionVM = new WCFSessionVM("", "", commonFixture.StrToken, loginUserMgtController.FunKey, loginUserMgtController.FunTypeKey, this.commonFixture.LanguageKey.ToString(), "");
            //All Fun
            entityList_FunctionVM = loginUserMgtController.funMgtHelper.Value.GetAll(entity_WCFSessionVM);
            //All Org
            entityList_LUserOrganizationVM = loginUserMgtController.orgMgtHelper.Value.GetAll(entity_WCFSessionVM);
            //All Org Details
            entityList_LUserOrgDetailsVM = loginUserMgtController.orgDetailMgtHelper.Value.GetAll(entity_WCFSessionVM);
            //All Role
            entityList_LUserRoleVM = loginUserMgtController.roleMgtHelper.Value.GetAll(entity_WCFSessionVM);

            #endregion

            loginUserMgtController.guidList_AccessedOrgID = loginUserMgtController.GetOrgIDList(commonFixture.entity_ServerSideSession, "LoginUserManage", "Create", entityList_LUserOrganizationVM);
            loginUserMgtController.guidList_AccessedLUserID = loginUserMgtController.GetLUserIDList(commonFixture.entity_ServerSideSession, "LoginUserManage", "Create", luMgtHelper, entity_WCFSessionVM);
        }

        [Fact]
        public void CreateLoginUser()
        {
            loginUserMgtController.FunTypeKey = "Create";
            commonFixture.MockControllerInfo(loginUserMgtController, "http://localhost:50653/AccessControl/LoginUserManage/Create");

            Random random = new Random();
            #region [ Create Login User with Specific Functions ]
            //00010001
            FunctionVM entity_FunctionVM = entityList_FunctionVM.Where(current => current.FunctionPath == "00010001").FirstOrDefault();

            string str_FunID = entity_FunctionVM.ID.ToString();

            FunDetailInfo entity_FunDetailInfo = new FunDetailInfo();

            WebCommonHelper webCommonHelper = new WebCommonHelper();
            webCommonHelper.CallWCFHelper(loginUserMgtController, (entity_WCFSessionVM) =>
            {
                entity_FunDetailInfo = loginUserMgtController.funMgtHelper.Value.GetFunDetailInfo_FID(entity_WCFSessionVM, str_FunID);
                entity_FunDetailInfo.FDSelected.ForEach(current => current = true);
            });

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

            List<FunDetailInfo> entityList_FunDetailInfo = new List<FunDetailInfo>();
            entityList_FunDetailInfo.Add(entity_FunDetailInfo);

            int r = random.Next(1, 1000);
            string str_LoginName = "A" + r;

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
            str_LoginName = "A" + r;
            loginUserVM = new LoginUserVM()
            {
                LoginName = str_LoginName,
                NewPwd = "A12346.b",
                ConfirmNewPwd = "A12346.b",
                Status = 1,
                UserType = 2,
                roleListIDList = String.Join("|", entityList_LUserRoleVM.Select(current => current.ID.ToString()).ToArray())
            };
            actionResult = loginUserMgtController.Create(loginUserVM);

            Assert.IsType<RedirectToRouteResult>(actionResult);
            #endregion

            #region [ Create Login User with Org ]
            List<string> strList_Org = entityList_LUserOrganizationVM.Where(current => current.OrganizationPath == "00010001" || current.OrganizationPath == "000100010002").Select(current => current.ID.ToString()).ToList();

            List<string> strList_OrgDetail = entityList_LUserOrgDetailsVM.Select(current => current.ID.ToString()).ToList();

            r = random.Next(1, 1000);
            str_LoginName = "A" + r;
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

        [Fact]
        public void EditLoginUser()
        {
            loginUserMgtController.FunTypeKey = "Edit";
            commonFixture.MockControllerInfo(loginUserMgtController, "http://localhost:50653/AccessControl/LoginUserManage/Edit");

            WCFSessionVM entity_WCFSessionVM = new WCFSessionVM("", "", commonFixture.StrToken, "LoginUserManage", "Edit", this.commonFixture.LanguageKey.ToString(), "");

            LUSerListResult entity_LUSerListResult = loginUserMgtController.loginUserMgtHelper.Value.GetListWithPaging(entity_WCFSessionVM, new LoginUserVM(), 1, 10, "", "", new List<string>(), loginUserMgtController.guidList_AccessedLUserID);

            LoginUserVM entity_LoginUserVM = null;

            if (entity_LUSerListResult != null)
            {
                entity_LoginUserVM = entity_LUSerListResult.EntityList_LoginUserVM.Where(current => current.LoginName == "Wells").FirstOrDefault();
            }

            Assert.NotNull(entity_LoginUserVM);

            entity_LoginUserVM.UserType = 2;
            entity_LoginUserVM.roleListIDList = String.Join("|", entityList_LUserRoleVM.Select(current => current.ID.ToString()).ToArray());

            ActionResult actionResult = loginUserMgtController.Edit(entity_LoginUserVM);

            Assert.IsType<RedirectToRouteResult>(actionResult);
        }

        [Fact]
        public void GetListByPaging()
        {
            loginUserMgtController.FunTypeKey = "View";
            commonFixture.MockControllerInfo(loginUserMgtController, "http://localhost:50653/AccessControl/LoginUserManage/Index");

            WCFSessionVM entity_WCFSessionVM = new WCFSessionVM("", "", commonFixture.StrToken, "LoginUserManage", "View", this.commonFixture.LanguageKey.ToString(), "");

            ViewResult viewResult = (ViewResult)loginUserMgtController.Index(1, "LoginName", "DESC");

            Assert.NotNull(viewResult);

            Assert.NotEmpty((List<LoginUserVM>)viewResult.Model);
        }

        [Fact]
        public void DeleteLoginUser()
        {
            loginUserMgtController.FunTypeKey = "Delete";
            commonFixture.MockControllerInfo(loginUserMgtController, "http://localhost:50653/AccessControl/LoginUserManage/Delete");

            WCFSessionVM entity_WCFSessionVM = new WCFSessionVM("", "", commonFixture.StrToken, "LoginUserManage", "Delete", this.commonFixture.LanguageKey.ToString(), "");

            Random random = new Random();

            int r = random.Next(1, 1000);
            string str_LoginName = "A" + r;
            LoginUserVM loginUserVM = new LoginUserVM()
            {
                LoginName = str_LoginName,
                NewPwd = "A12346.b",
                ConfirmNewPwd = "A12346.b",
                Status = 1,
                UserType = 2,
                roleListIDList = String.Join("|", entityList_LUserRoleVM.Select(current => current.ID.ToString()).ToArray())
            };

            WCFReturnResult createResult = loginUserMgtController.loginUserMgtHelper.Value.Create(entity_WCFSessionVM, loginUserVM);

            loginUserMgtController.guidList_AccessedLUserID = loginUserMgtController.GetLUserIDList(commonFixture.entity_ServerSideSession, "LoginUserManage", "Create", loginUserMgtController.loginUserMgtHelper.Value, entity_WCFSessionVM);

            LUSerListResult entity_LUSerListResult = loginUserMgtController.loginUserMgtHelper.Value.GetListWithPaging(entity_WCFSessionVM, new LoginUserVM(), 1, int.MaxValue, "", "", new List<string>(), loginUserMgtController.guidList_AccessedLUserID);

            Assert.NotEmpty(entity_LUSerListResult.EntityList_LoginUserVM);

            LoginUserVM entity_LoginUserVM = entity_LUSerListResult.EntityList_LoginUserVM.Where(current => current.LoginName == str_LoginName).FirstOrDefault();


            FormCollection formCollection = new FormCollection(new NameValueCollection
            {
                {"UserID",entity_LoginUserVM.ID.ToString()}
            });

            JsonResult jsonResult = (JsonResult)loginUserMgtController.Delete(formCollection);

            Assert.NotNull(jsonResult.Data);

            Assert.IsType<CommonJsonResult>(jsonResult.Data);

            Assert.True(((CommonJsonResult)jsonResult.Data).Success);
        }
    }
}
