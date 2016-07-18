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
using System;
using System.Text;
using System.Collections.Generic;
using CoolPrivilegeControlService.NonWCFFun;
using WCF_Infrastructure;

namespace CoolPrivilegeMVCUnitTest
{
    public class CommonFixture : IDisposable
    {
        #region [ Properties and Fields ]
        public IPostOffice postOffice = new PostOffice();

        private LanguageKey _languagKey = LanguageKey.en;

        public LanguageKey LanguageKey
        {
            get
            {
                return _languagKey;
            }
            set
            {
                _languagKey = value;
            }
        }

        public string StrToken
        {
            get;
            set;
        }

        public BaseSession entity_BaseSession
        {
            get;
            set;
        }
        #endregion

        #region [ Constructor ]
        public CommonFixture()
        {
            #region [ cool privilege control ]
            IList<ComposablePartCatalog> composableList = new List<ComposablePartCatalog>();

            composableList.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
            composableList.Add(new AssemblyCatalog(typeof(LanguageKey).Assembly));
            composableList.Add(new AssemblyCatalog(typeof(FunctionTypeVM).Assembly));
            //composableList.Add(new AssemblyCatalog(typeof(StaticContent).Assembly));
            composableList.Add(new AssemblyCatalog(typeof(LoginController).Assembly));

            Bootstrapper boot = new Bootstrapper(composableList);

            boot.RunTask();

            composableList = new List<ComposablePartCatalog>();
            composableList.Add(new AssemblyCatalog(typeof(PrivilegeFun).Assembly));
            WCFBootstrapper wcfBootStrapper = new WCFBootstrapper(composableList);

            SystemMgtHelper systemMgtHelper = new SystemMgtHelper(WCFBootstrapper.Container);

            Bootstrapper.Container.ComposeExportedValue<LoginUserMgtHelper>(new LoginUserMgtHelper(WCFBootstrapper.Container));
            Bootstrapper.Container.ComposeExportedValue<AuditLogMgtHelper>(new AuditLogMgtHelper(WCFBootstrapper.Container));
            Bootstrapper.Container.ComposeExportedValue<AuthHisMgtHelper>(new AuthHisMgtHelper(WCFBootstrapper.Container));
            Bootstrapper.Container.ComposeExportedValue<FunMgtHelper>(new FunMgtHelper(WCFBootstrapper.Container));
            Bootstrapper.Container.ComposeExportedValue<FunTypeMgtHelper>(new FunTypeMgtHelper(WCFBootstrapper.Container));
            Bootstrapper.Container.ComposeExportedValue<OrgMgtHelper>(new OrgMgtHelper(WCFBootstrapper.Container));
            Bootstrapper.Container.ComposeExportedValue<OrgDetailMgtHelper>(new OrgDetailMgtHelper(WCFBootstrapper.Container));
            Bootstrapper.Container.ComposeExportedValue<RoleMgtHelper>(new RoleMgtHelper(WCFBootstrapper.Container));
            Bootstrapper.Container.ComposeExportedValue<SystemMgtHelper>(systemMgtHelper);
            Bootstrapper.Container.ComposeExportedValue<CheckPrivilegeHelper>(new CheckPrivilegeHelper(WCFBootstrapper.Container));

            WCFAuthInfoVM entity_WCFAuthInfoVM = new WCFAuthInfoVM("", "", "", "", "", LanguageKey.en.ToString(), "");

            StaticContent.SystemInfoInst = systemMgtHelper.GetSystemInfo(entity_WCFAuthInfoVM);
            #endregion

            #region [ Initialize Language Pack ]
            LangPack entity_LangPack = new LangPack();

            entity_LangPack.CacheLanguages(StaticContent.LangPackProjectName, LangPack.GetLanugage(LanguageKey.en));
            entity_LangPack.CacheLanguages(StaticContent.LangPackProjectName, LangPack.GetLanugage(LanguageKey.cn));
            entity_LangPack.CacheLanguages(StaticContent.LangPackProjectName, LangPack.GetLanugage(LanguageKey.tw));
            #endregion

            #region [ Set Route Table ]
            RouteTable.Routes.Clear();
            var areaRegistration = new AccessControlAreaRegistration();
            var areaRegistrationContext = new AreaRegistrationContext(
                areaRegistration.AreaName, RouteTable.Routes);
            areaRegistration.RegisterArea(areaRegistrationContext);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            #endregion
        }
        #endregion

        #region [ Methods ]
        public void LoginAndGetToken()
        {
            string str_Token = "";

            LoginUserMgtHelper loginUserMgtHelper = new LoginUserMgtHelper(postOffice.LoginUserMgtSerPath);

            LoginUserVM entity_LUVM = new LoginUserVM();
            entity_LUVM.LoginName = "admin";
            entity_LUVM.LoginPwd = "123456";

            LUSerLoginResult loginResult = loginUserMgtHelper.Login(entity_LUVM, LanguageKey.ToString(), "", "");

            if (loginResult != null)
            {
                str_Token = loginResult.Str_ServerToken;

                entity_BaseSession = loginResult.Entity_SessionWUserInfo;
            }
            StrToken = str_Token;
        }

        /// <summary>
        /// Mock HttpContext and Route Data
        /// </summary>
        public void MockControllerInfo(Controller controller, string str_Url)
        {
            var httpContext = MvcMockHelpers.MockHttpContext(new Uri(
                str_Url));

            var routeData = RouteTable.Routes.GetRouteData(httpContext);
            controller.SetMockControllerContext(
                httpContext, routeData, RouteTable.Routes);
        }
        #endregion

        #region [ Dispose ]
        public void Dispose()
        {
            StaticContent.SystemInfoInst = null;
            Bootstrapper.Container.Dispose();
            //LangPack.resDicts

            LangPack.ClearAll();
        }
        #endregion
    }


    [CollectionDefinition("Common collection")]
    public class CommonFixtureCollection : ICollectionFixture<CommonFixture>
    {

    }
}
