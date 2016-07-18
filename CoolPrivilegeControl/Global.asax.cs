using CoolPrivilegeControlVM.CommonVM;
using CoolUtilities.MultiLingual;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Threading;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using UI_Infrastructure;
using CoolPrivilegeControlVM.WCFVM;
using CoolPrivilegeControl.Models;
using System.ServiceModel;
using CoolPrivilegeControlSerClient;

namespace CoolPrivilegeControl
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public MvcApplication()
        {
            this.BeginRequest += MvcApplication_BeginRequest;
        }

        void MvcApplication_BeginRequest(object sender, EventArgs e)
        {
            if (!LangPack.ContainsKey(Thread.CurrentThread.CurrentUICulture.Name.ToLower()))
            {
                Thread.CurrentThread.CurrentCulture =
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(LangPack.GetLanugage(LanguageKey.en));
            }
        }

        protected void Application_Start()
        {
            #region [ Cool Privilege Control ]
            AreaRegistration.RegisterAllAreas();

            var myassembly1 = new DirectoryCatalog("bin");
            IList<ComposablePartCatalog> composableList = new List<ComposablePartCatalog>();
            composableList.Add(myassembly1);

            Bootstrapper boot = new Bootstrapper(composableList);

            boot.RunTask();

            IPostOffice postOffice = new PostOffice();

            #region [ Compose Service Helper ]
            SystemMgtHelper systemMgtHelper = new SystemMgtHelper(postOffice.SystemMgtSerPath);

            Bootstrapper.Container.ComposeExportedValue<LoginUserMgtHelper>(new LoginUserMgtHelper(postOffice.LoginUserMgtSerPath));
            Bootstrapper.Container.ComposeExportedValue<AuditLogMgtHelper>(new AuditLogMgtHelper(postOffice.AuditLogMgtSerPath));
            Bootstrapper.Container.ComposeExportedValue<AuthHisMgtHelper>(new AuthHisMgtHelper(postOffice.AuthHisMgtSerPath));
            Bootstrapper.Container.ComposeExportedValue<FunMgtHelper>(new FunMgtHelper(postOffice.FunMgtSerPath));
            Bootstrapper.Container.ComposeExportedValue<FunTypeMgtHelper>(new FunTypeMgtHelper(postOffice.FunTypeMgtSerPath));
            Bootstrapper.Container.ComposeExportedValue<OrgMgtHelper>(new OrgMgtHelper(postOffice.OrgMgtSerPath));
            Bootstrapper.Container.ComposeExportedValue<OrgDetailMgtHelper>(new OrgDetailMgtHelper(postOffice.OrgDetailMgtSerPath));
            Bootstrapper.Container.ComposeExportedValue<RoleMgtHelper>(new RoleMgtHelper(postOffice.RoleMgtSerPath));
            Bootstrapper.Container.ComposeExportedValue<SystemMgtHelper>(systemMgtHelper);
            Bootstrapper.Container.ComposeExportedValue<CheckPrivilegeHelper>(new CheckPrivilegeHelper(postOffice.CheckPrivilegeSerPath));
            #endregion

            WCFAuthInfoVM entity_WCFAuthInfoVM = new WCFAuthInfoVM("", "", "", "", "", LanguageKey.en.ToString(), "");

            StaticContent.SystemInfoInst = systemMgtHelper.GetSystemInfo(entity_WCFAuthInfoVM);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            LangPack entity_LangPack = new LangPack();

            entity_LangPack.CacheLanguages(StaticContent.LangPackProjectName, LangPack.GetLanugage(LanguageKey.en));
            entity_LangPack.CacheLanguages(StaticContent.LangPackProjectName, LangPack.GetLanugage(LanguageKey.cn));
            entity_LangPack.CacheLanguages(StaticContent.LangPackProjectName, LangPack.GetLanugage(LanguageKey.tw));
            entity_LangPack.CacheLanguages(StaticContent.LangPackProjectName, LangPack.GetLanugage(LanguageKey.esve));
            #endregion
        }
    }
}