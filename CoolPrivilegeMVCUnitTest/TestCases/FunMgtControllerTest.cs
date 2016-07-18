/**************************************************************************
*
* NAME        : FunMgtControllerTest.cs
*
* VERSION     : 1.0.0
*
* DATE        : 25-Feb-2016
*
* DESCRIPTION : FunMgtControllerTest
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
    public class FunMgtControllerTest
    {
        public FManageController fManageController;

        public CommonFixture commonFixture;

        private List<FunctionTypeVM> entityList_FunTypeVM = new List<FunctionTypeVM>();

        public FunMgtControllerTest(CommonFixture commonFixture)
        {
            this.commonFixture = commonFixture;

            //Generate Token
            commonFixture.LoginAndGetToken();

            FunTypeMgtHelper funTypeMgtHelper = Bootstrapper.Container.GetExportedValue<FunTypeMgtHelper>();
            FunMgtHelper funMgtHelper = Bootstrapper.Container.GetExportedValue<FunMgtHelper>();

            fManageController = new FManageController(Bootstrapper.Container);

            fManageController.ServiceAuthorizedKey = commonFixture.StrToken;

            fManageController.funTypeMgtHelper = new Lazy<FunTypeMgtHelper>(() => funTypeMgtHelper);
            fManageController.funMgtHelper = new Lazy<FunMgtHelper>(() => funMgtHelper);

            fManageController.TempData[StaticContent.LanguageKey] = this.commonFixture.LanguageKey;

            fManageController.FunKey = "FManage";

            WCFAuthInfoVM entity_WCFAuthInfoVM = new WCFAuthInfoVM("", "", commonFixture.StrToken, fManageController.FunKey, "Create", this.commonFixture.LanguageKey.ToString(), "");

            entityList_FunTypeVM = funTypeMgtHelper.GetAllFunType(entity_WCFAuthInfoVM);
        }

        [Fact, TestPriority(1)]
        public void CreateFun()
        {
            fManageController.FunTypeKey = "Create";
            commonFixture.MockControllerInfo(fManageController, "http://localhost:50653/AccessControl/FManage/Create");

            List<FunctionSelectedType> entityList_FunctionSelectedType = new List<FunctionSelectedType>();

            entityList_FunTypeVM.ForEach(current =>
            {
                entityList_FunctionSelectedType.Add(new FunctionSelectedType()
                {
                    FunctionType = current.FunctionType,
                    ID = current.ID,
                    Selected = true
                });
            });

            FunctionVM functionVM = new FunctionVM()
            {
                FunctionPath = "1100",
                FunctionName = "",
                FunctionKey = "Temp",
                SelectedTypeList = entityList_FunctionSelectedType
            };

            ActionResult result = fManageController.Create(functionVM);

            Assert.IsType<RedirectToRouteResult>(result);
        }

        [Fact, TestPriority(2)]
        public void EditFun()
        {
            fManageController.FunTypeKey = "Edit";
            commonFixture.MockControllerInfo(fManageController, "http://localhost:50653/AccessControl/FManage/Edit");

            List<FunctionSelectedType> entityList_FunctionSelectedType = new List<FunctionSelectedType>();

            entityList_FunTypeVM.ForEach(current =>
            {
                entityList_FunctionSelectedType.Add(new FunctionSelectedType()
                {
                    FunctionType = current.FunctionType,
                    ID = current.ID,
                    Selected = true
                });
            });

            if (entityList_FunctionSelectedType.Count > 0)
            {
                entityList_FunctionSelectedType[entityList_FunctionSelectedType.Count - 1].Selected = false;
            }

            WCFAuthInfoVM entity_WCFAuthInfoVM = new WCFAuthInfoVM("", "", commonFixture.StrToken, fManageController.FunKey, fManageController.FunTypeKey, this.commonFixture.LanguageKey.ToString(), "");

            List<FunctionVM> entityList_FunVM = fManageController.funMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);

            Assert.NotEmpty(entityList_FunVM);

            FunctionVM entity_FunctionVM = entityList_FunVM.Where(current => current.FunctionPath == "1100").FirstOrDefault();

            Assert.NotNull(entity_FunctionVM);

            entity_FunctionVM.FunctionKey = "1100";
            entity_FunctionVM.SelectedTypeList = entityList_FunctionSelectedType;

            ActionResult actionResult = fManageController.Edit(entity_FunctionVM);

            Assert.IsType<RedirectToRouteResult>(actionResult);
        }

        [Fact, TestPriority(3)]
        public void GetListByPaging()
        {
            fManageController.FunTypeKey = "View";
            commonFixture.MockControllerInfo(fManageController, "http://localhost:50653/AccessControl/FManage/Index");

            ViewResult viewResult = (ViewResult)fManageController.Index(1, "FunctionPath", "DESC");

            Assert.NotNull(viewResult);

            Assert.NotEmpty((List<FunctionVM>)viewResult.Model);
        }

        [Fact, TestPriority(4)]
        public void DeleteFun()
        {
            fManageController.FunTypeKey = "Delete";
            commonFixture.MockControllerInfo(fManageController, "http://localhost:50653/AccessControl/FManage/Delete");

            WCFAuthInfoVM entity_WCFAuthInfoVM = new WCFAuthInfoVM("", "", commonFixture.StrToken, fManageController.FunKey, fManageController.FunTypeKey, this.commonFixture.LanguageKey.ToString(), "");

            List<FunctionVM> entityList_FunVM = fManageController.funMgtHelper.Value.GetAll(entity_WCFAuthInfoVM);

            Assert.NotEmpty(entityList_FunVM);

            FunctionVM entity_FunctionVM = entityList_FunVM.Where(current => current.FunctionPath == "1100").FirstOrDefault();

            Assert.NotNull(entity_FunctionVM);

            FormCollection formCollection = new FormCollection(new NameValueCollection
            {
                {"ID",entity_FunctionVM.ID.ToString()}
            });

            JsonResult jsonResult = (JsonResult)fManageController.Delete(formCollection);

            Assert.NotNull(jsonResult.Data);

            Assert.IsType<CommonJsonResult>(jsonResult.Data);

            Assert.True(((CommonJsonResult)jsonResult.Data).Success);
        }

        [Fact, TestPriority(5)]
        public void Search()
        {
            fManageController.FunTypeKey = "Search";
            commonFixture.MockControllerInfo(fManageController, "http://localhost:50653/AccessControl/LoginUserManage/Index");

            FunctionVM entity_FunVM = new FunctionVM();
            entity_FunVM.FunctionPath = "0004";

            ViewResult viewResult = (ViewResult)fManageController.Index(entity_FunVM);

            Assert.NotNull(viewResult);

            Assert.NotEmpty((List<FunctionVM>)viewResult.Model);
        }
    }
}
