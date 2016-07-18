using CoolPrivilegeControlVM;
using CoolPrivilegeControlDAL.Common;
using CoolPrivilegeControlDAL.Respositories;
using CoolPrivilegeControlSerFactory.IService;
using CoolPrivilegeControlVM.EntityVM;
using CoolPrivilegeControlVM.WCFVM;
using CoolPrivilegeControlVM.WCFVM.FunTypeSerVM;
using CoolUtilities.MultiLingual;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WCF_Infrastructure;
using System.ComponentModel.Composition.Hosting;

namespace CoolPrivilegeControlService
{
    public class FunTypeMgtSer : ServiceBase, IFunTypeMgtSer
    {
        public FunTypeMgtSer()
            : base()
        {

        }

        public FunTypeMgtSer(CompositionContainer Container)
            : base(Container)
        {

        }

        public FTSerListResult GetListWithPaging(WCFAuthInfoVM entity_WCFAuthInfoVM, FunctionTypeVM entity_SearchCriteria, int int_CurrentPage, int int_PageSize, string str_SortColumn, string str_SortDir, List<string> str_CustomFilter)
        {
            try
            {
                //Restore Server Session by token
                RetrieveServerSideSession(entity_WCFAuthInfoVM);

                //Flag Success or Fail
                bool ret = false;

                //Define error list
                List<string> strList_Error = new List<string>();

                //Instantiate  FTSerListResult
                FTSerListResult returnResult = new FTSerListResult();

                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                FunctionTypeRespository entityRepos_FT = new FunctionTypeRespository(dbContext, entity_BaseSession.ID);

                #region [ Check Privilege ]
                ret = CheckAccPrivilege(entity_BaseSession.ID, entity_WCFAuthInfoVM.RequestFunKey, entity_WCFAuthInfoVM.RequestFunTypeKey, ref strList_Error);
                #endregion

                //Initialize FTSerListResult instance 
                returnResult.StrList_Error = strList_Error;
                returnResult.Int_TotalRecordCount = 0;
                returnResult.EntityList_FunctionTypeVM = new List<FunctionTypeVM>();

                //Success
                if (ret)
                {
                    int recordCount = 0;

                    List<FunctionTypeVM> vmList = entityRepos_FT.GetEntityListByPage(entity_SearchCriteria, int_CurrentPage, int_PageSize, str_SortColumn, str_SortDir, out recordCount, str_CustomFilter);

                    //Assign data to FTSerListResult instance 
                    returnResult.EntityList_FunctionTypeVM = vmList;
                    returnResult.Int_TotalRecordCount = recordCount;
                }

                return returnResult;
            }
            catch (Exception ex)
            {
                throw new FaultException<WCFErrorContract>(new WCFErrorContract(ex), ex.Message);
            }
        }

        public FTSerEditResult GetEntityByID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_FTID)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);

                List<string> strList_Error = new List<string>();

                FTSerEditResult returnResult = new FTSerEditResult();

                //Contruct Login User Respository
                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                FunctionTypeRespository Respo_FT = new FunctionTypeRespository(dbContext, entity_BaseSession.ID);

                bool ret = false;

                ret = CheckAccPrivilege(entity_BaseSession.ID, entity_WCFAuthInfoVM.RequestFunKey, entity_WCFAuthInfoVM.RequestFunTypeKey, ref strList_Error);

                if (ret)
                {
                    FunctionTypeVM db_FunctionTypeVM = Respo_FT.GetEntityByID(Guid.Parse(str_FTID), languageKey, out strList_Error);

                    returnResult.Entity_FunctionTypeVM = db_FunctionTypeVM;
                }

                returnResult.StrList_Error = strList_Error;

                return returnResult;
            }
            catch (Exception ex)
            {
                throw new FaultException<WCFErrorContract>(new WCFErrorContract(ex), ex.Message);
            }
        }

        public WCFReturnResult Create(WCFAuthInfoVM entity_WCFAuthInfoVM, FunctionTypeVM entity_FunTypeVM)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);

                WCFReturnResult returnResult = new WCFReturnResult();

                //Contruct Login User Respository
                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();
                FunctionTypeRespository Respo_FT = new FunctionTypeRespository(dbContext, entity_BaseSession.ID);

                List<string> strList_Error = new List<string>();

                bool ret = false;

                ret = CheckAccPrivilege(entity_BaseSession.ID, entity_WCFAuthInfoVM.RequestFunKey, entity_WCFAuthInfoVM.RequestFunTypeKey, ref strList_Error);

                if (ret)
                {
                    ret = Respo_FT.Create(entity_FunTypeVM, languageKey, ref strList_Error);
                }

                returnResult.IsSuccess = ret;

                returnResult.StrList_Error = strList_Error;

                return returnResult;
            }
            catch (Exception ex)
            {
                throw new FaultException<WCFErrorContract>(new WCFErrorContract(ex), ex.Message);
            }
        }

        public WCFReturnResult Delete(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_FTID)
        {
            try
            {
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);

                WCFReturnResult returnResult = new WCFReturnResult();

                //Contruct Login User Respository
                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                FunctionTypeRespository Respo_FT = new FunctionTypeRespository(dbContext, entity_BaseSession.ID);

                List<string> strList_Error = new List<string>();

                bool ret = false;

                ret = CheckAccPrivilege(entity_BaseSession.ID, entity_WCFAuthInfoVM.RequestFunKey, entity_WCFAuthInfoVM.RequestFunTypeKey, ref strList_Error);

                if (ret)
                {
                    ret = Respo_FT.Delete(str_FTID, languageKey, ref strList_Error);
                }

                returnResult.IsSuccess = ret;

                returnResult.StrList_Error = strList_Error;

                return returnResult;
            }
            catch (Exception ex)
            {
                throw new FaultException<WCFErrorContract>(new WCFErrorContract(ex), ex.Message);
            }
        }

        public WCFReturnResult Update(WCFAuthInfoVM entity_WCFAuthInfoVM, FunctionTypeVM entity_FunTypeVM)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);

                WCFReturnResult returnResult = new WCFReturnResult();

                //Contruct Login User Respository
                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                FunctionTypeRespository Respo_FT = new FunctionTypeRespository(dbContext, entity_BaseSession.ID);

                List<string> strList_Error = new List<string>();

                bool ret = false;

                ret = CheckAccPrivilege(entity_BaseSession.ID, entity_WCFAuthInfoVM.RequestFunKey, entity_WCFAuthInfoVM.RequestFunTypeKey, ref strList_Error);

                if (ret)
                {
                    ret = Respo_FT.Update(entity_FunTypeVM, languageKey, ref strList_Error);
                }

                returnResult.IsSuccess = ret;

                returnResult.StrList_Error = strList_Error;

                return returnResult;
            }
            catch (Exception ex)
            {
                throw new FaultException<WCFErrorContract>(new WCFErrorContract(ex), ex.Message);
            }
        }

        public List<FunctionTypeVM> GetAllFunType(WCFAuthInfoVM entity_WCFAuthInfoVM)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);

                List<FunctionTypeVM> returnResult = new List<FunctionTypeVM>();

                //Contruct Login User Respository
                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                FunctionTypeRespository Respo_FT = new FunctionTypeRespository(dbContext, entity_BaseSession.ID);

                List<string> strList_Error = new List<string>();

                bool ret = false;

                ret = CheckTokenOnly(entity_BaseSession, ref strList_Error);

                if (ret)
                {
                    returnResult = Respo_FT.GetAllFunctionType();
                }

                return returnResult;
            }
            catch (Exception ex)
            {
                throw new FaultException<WCFErrorContract>(new WCFErrorContract(ex), ex.Message);
            }
        }
    }
}
