/**************************************************************************
*
* NAME        : LoginControllerTest.cs
*
* VERSION     : 1.0.0
*
* DATE        : 25-Feb-2016
*
* DESCRIPTION : LoginControllerTest
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


namespace CoolPrivilegeMVCUnitTest
{
    /// <summary>
    /// Summary description for LoginControllerTest
    /// </summary>
    [Collection("Common collection")]
    public class LoginControllerTest
    {
        public CommonFixture commonFixture;

        public LoginController loginController;

        public LoginControllerTest(CommonFixture commonFixture)
        {
            this.commonFixture = commonFixture;

            loginController = new LoginController(Bootstrapper.Container);
        }

        [Fact]
        public void DisplayLoginPage()
        {
            LoginUserMgtHelper luMgtHelper = new LoginUserMgtHelper(this.commonFixture.postOffice.LoginUserMgtSerPath);

            commonFixture.MockControllerInfo(loginController, "http://localhost:50653/AccessControl/Login/Index");

            ViewResult loginPageResult_Index = (ViewResult)loginController.Index("cn");

            Assert.True(loginPageResult_Index.TempData.ContainsKey(StaticContent.LanguageKey));

            Assert.True(loginPageResult_Index.TempData[StaticContent.LanguageKey].ToString() == "cn");
        }

        [Fact]
        public void DoLogin()
        {
            //Test WCF Function
            LoginUserMgtHelper luMgtHelper = new LoginUserMgtHelper(this.commonFixture.postOffice.LoginUserMgtSerPath);

            commonFixture.MockControllerInfo(loginController, "http://localhost:50653/AccessControl/Login/Index");

            loginController.TempData[StaticContent.LanguageKey] = this.commonFixture.LanguageKey;

            loginController.loginUserMgtHelper = new Lazy<LoginUserMgtHelper>(() => luMgtHelper);

            LoginUserVM entity_LUVM = new LoginUserVM();
            entity_LUVM.LoginName = "admin";
            entity_LUVM.LoginPwd = "123456";

            LUSerLoginResult temp = luMgtHelper.Login(entity_LUVM, LanguageKey.en.ToString(), "", "");

            //Login Success Case
            loginController.Index(entity_LUVM);

            Assert.True(loginController.TempData.ContainsKey(Bootstrapper.UserClientSessionKey.ToString()));

            Assert.True(!loginController.TempData.ContainsKey(loginController.ActionMessageKey) || loginController.TempData[loginController.ActionMessageKey] == null);

            //Login Fail Case
            entity_LUVM = new LoginUserVM();
            ViewResult loginPageResult_Fail = (ViewResult)loginController.Index(entity_LUVM);
            Assert.True(loginPageResult_Fail.ViewData.ContainsKey("ActionMessage"));

            MsgInfo errorMsgInfo = (MsgInfo)loginPageResult_Fail.ViewData["ActionMessage"];

            Assert.True(errorMsgInfo.MsgType == MessageType.ValidationError);
        }

        [Fact]
        public void DoLogout()
        {
            //Test WCF Function
            LoginUserMgtHelper luMgtHelper = new LoginUserMgtHelper(this.commonFixture.postOffice.LoginUserMgtSerPath);

            loginController.TempData[StaticContent.LanguageKey] = this.commonFixture.LanguageKey;

            loginController.loginUserMgtHelper = new Lazy<LoginUserMgtHelper>(() => luMgtHelper);

            commonFixture.MockControllerInfo(loginController, "http://localhost:50653/AccessControl/Login/Logout");

            ViewResult logoutResult = (ViewResult)loginController.Logout();

            Assert.True(!logoutResult.ViewData.ContainsKey(Bootstrapper.UserClientSessionKey.ToString()));
            Assert.True(!logoutResult.TempData.ContainsKey(Bootstrapper.UserClientSessionKey.ToString()));

            Assert.True(logoutResult.ViewData.ContainsKey("ActionMessage"));

            MsgInfo entity_MsgInfo = (MsgInfo)logoutResult.ViewData["ActionMessage"];

            Assert.True(entity_MsgInfo.MsgType == MessageType.Success);
        }
    }
}
