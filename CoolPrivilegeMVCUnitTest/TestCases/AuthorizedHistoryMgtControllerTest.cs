/**************************************************************************
*
* NAME        : AuthorizedHistoryMgtController.cs
*
* VERSION     : 1.0.0
*
* DATE        : 25-Feb-2016
*
* DESCRIPTION : AuthorizedHistoryMgtController
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
    public class AuthHisMgtControllerTest
    {
        public AuthorizedHistoryManageController authHisMgtController;

        public CommonFixture commonFixture;

        public AuthHisMgtControllerTest(CommonFixture commonFixture)
        {
            this.commonFixture = commonFixture;

            //Generate Token
            commonFixture.LoginAndGetToken();

            AuthHisMgtHelper authHisMgtHelper = Bootstrapper.Container.GetExportedValue<AuthHisMgtHelper>();

            authHisMgtController = new AuthorizedHistoryManageController(Bootstrapper.Container);

            authHisMgtController.ServiceAuthorizedKey = commonFixture.StrToken;

            authHisMgtController.authHisMgtHelper = new Lazy<AuthHisMgtHelper>(() => authHisMgtHelper);

            authHisMgtController.TempData[StaticContent.LanguageKey] = this.commonFixture.LanguageKey;

            authHisMgtController.FunKey = "AuthorizedHistoryManage";
        }

        [Fact, TestPriority(1)]
        public void GetListByPaging()
        {
            authHisMgtController.FunTypeKey = "View";
            commonFixture.MockControllerInfo(authHisMgtController, "http://localhost:50653/AccessControl/AuthorizedHistoryManage/Index");

            ViewResult viewResult = (ViewResult)authHisMgtController.Index(1, "LoginName", "asc");

            Assert.NotNull(viewResult);

            Assert.NotEmpty((List<AuthorizedHistoryVM>)viewResult.Model);
        }

        [Fact, TestPriority(2)]
        public void Search()
        {
            authHisMgtController.FunTypeKey = "Search";
            commonFixture.MockControllerInfo(authHisMgtController, "http://localhost:50653/AccessControl/AuthorizedHistoryManage/Index");

            AuthorizedHistoryVM entity_AuthHisVM = new AuthorizedHistoryVM();
            entity_AuthHisVM.LoginName = "Admin";

            FormCollection formCollection = new FormCollection(new NameValueCollection
            {
                {"LoginName","Admin"}
            });

            ViewResult viewResult = (ViewResult)authHisMgtController.Index(formCollection);

            Assert.NotNull(viewResult);

            Assert.NotEmpty((List<AuthorizedHistoryVM>)viewResult.Model);
        }

        [Fact, TestPriority(3)]
        public void DeleteAuthorizedHis()
        {
            authHisMgtController.FunTypeKey = "Delete";
            commonFixture.MockControllerInfo(authHisMgtController, "http://localhost:50653/AccessControl/AuthorizedHistoryManage/Delete");

            AuthorizedHistoryVM entity_AuthHisVM = new AuthorizedHistoryVM();
            entity_AuthHisVM.LoginName = "Admin";

            FormCollection formCollection = new FormCollection(new NameValueCollection
            {
                {"LoginName","Admin"}
            });

            ViewResult viewResult = (ViewResult)authHisMgtController.Index(formCollection);

            List<AuthorizedHistoryVM> entityList_AuthHis = (List<AuthorizedHistoryVM>)viewResult.Model;

            AuthorizedHistoryVM entity_AuthHis = entityList_AuthHis.Where(current => current.LoginName == "Admin").FirstOrDefault();

            Assert.NotNull(entity_AuthHis);

            formCollection = new FormCollection(new NameValueCollection
            {
                {"ID",entity_AuthHis.ID.ToString()}
            });

            JsonResult jsonResult = (JsonResult)authHisMgtController.Delete(formCollection);

            Assert.NotNull(jsonResult.Data);

            Assert.IsType<CommonJsonResult>(jsonResult.Data);

            Assert.True(((CommonJsonResult)jsonResult.Data).Success);
        }
    }
}
