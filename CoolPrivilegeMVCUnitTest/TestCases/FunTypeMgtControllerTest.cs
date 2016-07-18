/**************************************************************************
*
* NAME        : FunTypeMgtControllerTest.cs
*
* VERSION     : 1.0.0
*
* DATE        : 25-Feb-2016
*
* DESCRIPTION : FunTypeMgtControllerTest
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
    public class FunTypeMgtControllerTest
    {
        public FTManageController ftManageController;

        public CommonFixture commonFixture;

        public FunTypeMgtControllerTest(CommonFixture commonFixture)
        {
            this.commonFixture = commonFixture;

            //Generate Token
            commonFixture.LoginAndGetToken();

            FunTypeMgtHelper funTypeMgtHelper = Bootstrapper.Container.GetExportedValue<FunTypeMgtHelper>();

            ftManageController = new FTManageController(Bootstrapper.Container);

            ftManageController.ServiceAuthorizedKey = commonFixture.StrToken;

            ftManageController.funTypeMgtHelper = new Lazy<FunTypeMgtHelper>(() => funTypeMgtHelper);

            ftManageController.TempData[StaticContent.LanguageKey] = this.commonFixture.LanguageKey;

            ftManageController.FunKey = "FTManage";
        }

        [Fact, TestPriority(1)]
        public void CreateFunType()
        {
            ftManageController.FunTypeKey = "Create";
            commonFixture.MockControllerInfo(ftManageController, "http://localhost:50653/AccessControl/FTManage/Create");

            FunctionTypeVM functionTypeVM = new FunctionTypeVM()
            {
                FunctionType = "FTTest"
            };

            ActionResult result = ftManageController.Create(functionTypeVM);

            Assert.IsType<RedirectToRouteResult>(result);
        }

        [Fact, TestPriority(2)]
        public void EditFunType()
        {
            ftManageController.FunTypeKey = "Edit";
            commonFixture.MockControllerInfo(ftManageController, "http://localhost:50653/AccessControl/FTManage/Edit");

            WCFAuthInfoVM entity_WCFAuthInfoVM = new WCFAuthInfoVM("", "", commonFixture.StrToken, ftManageController.FunKey, ftManageController.FunTypeKey, this.commonFixture.LanguageKey.ToString(), "");

            List<FunctionTypeVM> entityList_FunTypeVM = ftManageController.funTypeMgtHelper.Value.GetAllFunType(entity_WCFAuthInfoVM);

            Assert.NotEmpty(entityList_FunTypeVM);

            FunctionTypeVM entity_FunTypeVM = entityList_FunTypeVM.Where(current => current.FunctionType.IndexOf("FTTest") == 0).FirstOrDefault();

            Assert.NotNull(entity_FunTypeVM);

            entity_FunTypeVM.FunctionType = "FTTest3";

            ActionResult actionResult = ftManageController.Edit(entity_FunTypeVM);

            Assert.IsType<RedirectToRouteResult>(actionResult);
        }

        [Fact, TestPriority(3)]
        public void GetListByPaging()
        {
            ftManageController.FunTypeKey = "View";
            commonFixture.MockControllerInfo(ftManageController, "http://localhost:50653/AccessControl/FTManage/Index");

            ViewResult viewResult = (ViewResult)ftManageController.Index(1, "FunctionType", "DESC");

            Assert.NotNull(viewResult);

            Assert.NotEmpty((List<FunctionTypeVM>)viewResult.Model);
        }

        [Fact, TestPriority(4)]
        public void DeleteFunType()
        {
            ftManageController.FunTypeKey = "Delete";
            commonFixture.MockControllerInfo(ftManageController, "http://localhost:50653/AccessControl/FTManage/Delete");

            WCFAuthInfoVM entity_WCFAuthInfoVM = new WCFAuthInfoVM("", "", commonFixture.StrToken, ftManageController.FunKey, ftManageController.FunTypeKey, this.commonFixture.LanguageKey.ToString(), "");

            List<FunctionTypeVM> entityList_FunTypeVM = ftManageController.funTypeMgtHelper.Value.GetAllFunType(entity_WCFAuthInfoVM);

            Assert.NotEmpty(entityList_FunTypeVM);

            FunctionTypeVM entity_FunTypeVM = entityList_FunTypeVM.Where(current => current.FunctionType == "FTTest3").FirstOrDefault();

            Assert.NotNull(entity_FunTypeVM);

            FormCollection formCollection = new FormCollection(new NameValueCollection
            {
                {"ID",entity_FunTypeVM.ID.ToString()}
            });

            JsonResult jsonResult = (JsonResult)ftManageController.Delete(formCollection);

            Assert.NotNull(jsonResult.Data);

            Assert.IsType<CommonJsonResult>(jsonResult.Data);

            Assert.True(((CommonJsonResult)jsonResult.Data).Success);
        }

        [Fact, TestPriority(5)]
        public void Search()
        {
            ftManageController.FunTypeKey = "Search";
            commonFixture.MockControllerInfo(ftManageController, "http://localhost:50653/AccessControl/LoginUserManage/Index");

            FunctionTypeVM entity_FunTypeVM = new FunctionTypeVM();
            entity_FunTypeVM.FunctionType = "C";

            ViewResult viewResult = (ViewResult)ftManageController.Index(entity_FunTypeVM);

            Assert.NotNull(viewResult);

            Assert.NotEmpty((List<FunctionTypeVM>)viewResult.Model);
        }
    }
}
