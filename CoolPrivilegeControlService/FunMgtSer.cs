using CoolPrivilegeControlVM;
using CoolPrivilegeControlDAL.Common;
using CoolPrivilegeControlDAL.Policies;
using CoolPrivilegeControlDAL.Respositories;
using CoolPrivilegeControlSerFactory.IService;
using CoolPrivilegeControlVM.CommonVM;
using CoolPrivilegeControlVM.EntityVM;
using CoolPrivilegeControlVM.WCFVM;
using CoolUtilities.MultiLingual;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WCF_Infrastructure;
using WCF_Infrastructure.Policies;
using CoolPrivilegeControlVM.WCFVM.FunSerVM;
using CoolPrivilegeControlVM.WEBVM;
using System.ComponentModel.Composition.Hosting;

namespace CoolPrivilegeControlService
{
    public class FunMgtSer : ServiceBase, IFunMgtSer
    {
        public FunMgtSer()
            : base()
        {

        }

        public FunMgtSer(CompositionContainer Container)
            : base(Container)
        {

        }

        public List<FunctionVM> GetAll(WCFAuthInfoVM entity_WCFAuthInfoVM)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);

                if (entity_BaseSession != null)
                {
                    CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                    FunctionRespository funRespo = new FunctionRespository(dbContext, entity_BaseSession.ID);

                    List<FunctionVM> entityList_OrgVM = new List<FunctionVM>();

                    List<string> strList_Error = new List<string>();

                    bool ret = false;

                    ret = CheckTokenOnly(entity_BaseSession, ref strList_Error);

                    if (ret)
                    {
                        entityList_OrgVM = funRespo.GetFuns_All();
                    }

                    return entityList_OrgVM;
                }
                return new List<FunctionVM>();
            }
            catch (Exception ex)
            {
                throw new FaultException<WCFErrorContract>(new WCFErrorContract(ex), ex.Message);
            }
        }

        public FunDetailInfo GetFunDetailInfo_FID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_FunID)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);

                List<string> strList_Error = new List<string>();

                bool ret_CheckPrivilege = false;

                List<LUserAccessByOrgVM> ret = new List<LUserAccessByOrgVM>();

                //Contruct Login User Respository
                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                LoginUserRespository entityRepos_LoginUser = new LoginUserRespository(dbContext, entity_BaseSession.ID);

                SessionWUserInfo entity_SessionWUserInfo = entityRepos_LoginUser.GetLoginUserAccRight(entity_BaseSession.ID);

                string str_E025 = MultilingualHelper.GetStringFromResource(languageKey, "E025");
                if (entity_SessionWUserInfo == null)
                {
                    if (!ret_CheckPrivilege)
                        strList_Error.Add(str_E025);
                }

                FunDetailInfo entity_FunDetailInfo = null;

                if (strList_Error.Count == 0)
                {
                    FunctionDetailPolicy functionDetailPolicy = new FunctionDetailPolicy();
                    entity_FunDetailInfo = functionDetailPolicy.GetFunDetailInfo_FID(dbContext, Guid.Parse(str_FunID));
                }
                return entity_FunDetailInfo;
            }
            catch (Exception ex)
            {
                throw new FaultException<WCFErrorContract>(new WCFErrorContract(ex), ex.Message);
            }
        }

        public FSerListResult GetListWithPaging(WCFAuthInfoVM entity_WCFAuthInfoVM, FunctionVM entity_SearchCriteria, int int_CurrentPage, int int_PageSize, string str_SortColumn, string str_SortDir, List<string> str_CustomFilter)
        {
            try
            {
                //Restore Server Session
                RetrieveServerSideSession(entity_WCFAuthInfoVM);



                bool ret_CheckPrivilege = false;

                List<string> strList_Error = new List<string>();

                FSerListResult returnResult = new FSerListResult();

                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                FunctionRespository entityRepos_F = new FunctionRespository(dbContext, entity_BaseSession.ID);

                #region [ Check Privilege ]
                ret_CheckPrivilege = CheckAccPrivilege(entity_BaseSession.ID, entity_WCFAuthInfoVM.RequestFunKey, entity_WCFAuthInfoVM.RequestFunTypeKey, ref strList_Error);
                #endregion

                returnResult.StrList_Error = strList_Error;
                returnResult.Int_TotalRecordCount = 0;
                returnResult.EntityList_FunctionVM = new List<FunctionVM>();

                if (ret_CheckPrivilege)
                {
                    int recordCount = 0;

                    List<FunctionVM> vmList = entityRepos_F.GetEntityListByPage(entity_SearchCriteria, int_CurrentPage, int_PageSize, str_SortColumn, str_SortDir, out recordCount, str_CustomFilter, (entityList) =>
                    {
                        if (!string.IsNullOrWhiteSpace(entity_SearchCriteria.FunctionName))
                        {
                            entityList = entityList.Where(current => MultilingualHelper.GetStringFromResource(languageKey, current.F_Key).ToUpper().StartsWith(entity_SearchCriteria.FunctionName.ToUpper())).ToList();
                        }
                        return entityList;
                    }, (entityList_VM) =>
                    {
                        List<FunctionVM> ret = new List<FunctionVM>();
                        if (!string.IsNullOrWhiteSpace(str_SortColumn))
                        {
                            if (str_SortColumn.ToLower() == "functionpath")
                            {
                                if (str_SortDir.ToLower() == "asc")
                                {
                                    entityRepos_F.SortFunctionByPath(ret, entityList_VM, "ASC");
                                }
                                else
                                {
                                    entityRepos_F.SortFunctionByPath(ret, entityList_VM, "Desc");
                                }
                            }
                            else
                            {
                                ret = entityList_VM;
                            }
                        }
                        else
                        {
                            entityRepos_F.SortFunctionByPath(ret, entityList_VM, "ASC");
                        }
                        return ret;
                    }, (entityList_VM) =>
                    {
                        foreach (var item in entityList_VM)
                        {
                            if (!string.IsNullOrWhiteSpace(MultilingualHelper.GetStringFromResource(languageKey, item.FunctionKey)))
                                item.FunctionName = MultilingualHelper.GetStringFromResource(languageKey, item.FunctionKey);
                        }
                        return entityList_VM;
                    });

                    returnResult.EntityList_FunctionVM = vmList;
                    returnResult.Int_TotalRecordCount = recordCount;
                }

                return returnResult;
            }
            catch (Exception ex)
            {
                throw new FaultException<WCFErrorContract>(new WCFErrorContract(ex), ex.Message);
            }
        }

        public FSerEditResult GetEntityByID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_FunID)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);



                List<string> strList_Error = new List<string>();

                FSerEditResult returnResult = new FSerEditResult();

                //Contruct Login User Respository
                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                FunctionRespository Respo_F = new FunctionRespository(dbContext, entity_BaseSession.ID);

                bool ret = false;

                ret = CheckAccPrivilege(entity_BaseSession.ID, entity_WCFAuthInfoVM.RequestFunKey, entity_WCFAuthInfoVM.RequestFunTypeKey, ref strList_Error);

                if (ret)
                {
                    FunctionVM db_FunctionVM = Respo_F.GetEntityByID(Guid.Parse(str_FunID), languageKey, ref strList_Error);

                    returnResult.Entity_FunctionVM = db_FunctionVM;
                }

                returnResult.StrList_Error = strList_Error;

                return returnResult;
            }
            catch (Exception ex)
            {
                throw new FaultException<WCFErrorContract>(new WCFErrorContract(ex), ex.Message);
            }
        }

        public WCFReturnResult Create(WCFAuthInfoVM entity_WCFAuthInfoVM, FunctionVM entity_FunVM)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);



                WCFReturnResult returnResult = new WCFReturnResult();

                //Contruct Function Respository
                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                FunctionRespository Respo_F = new FunctionRespository(dbContext, entity_BaseSession.ID);

                List<string> strList_Error = new List<string>();

                bool ret = false;

                ret = CheckAccPrivilege(entity_BaseSession.ID, entity_WCFAuthInfoVM.RequestFunKey, entity_WCFAuthInfoVM.RequestFunTypeKey, ref strList_Error);

                if (ret)
                {
                    ret = Respo_F.Create(entity_FunVM, languageKey, ref strList_Error);
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

        public WCFReturnResult Delete(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_FunID)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);



                WCFReturnResult returnResult = new WCFReturnResult();

                //Contruct Login User Respository
                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                FunctionRespository Respo_F = new FunctionRespository(dbContext, entity_BaseSession.ID);

                List<string> strList_Error = new List<string>();

                bool ret = false;

                ret = CheckAccPrivilege(entity_BaseSession.ID, entity_WCFAuthInfoVM.RequestFunKey, entity_WCFAuthInfoVM.RequestFunTypeKey, ref strList_Error);

                if (ret)
                {
                    ret = Respo_F.Delete(str_FunID, languageKey, ref strList_Error);
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

        public WCFReturnResult Update(WCFAuthInfoVM entity_WCFAuthInfoVM, FunctionVM entity_FunVM)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);



                WCFReturnResult returnResult = new WCFReturnResult();

                //Contruct Login User Respository
                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                FunctionRespository Respo_F = new FunctionRespository(dbContext, entity_BaseSession.ID);

                List<string> strList_Error = new List<string>();

                bool ret = false;

                ret = CheckAccPrivilege(entity_BaseSession.ID, entity_WCFAuthInfoVM.RequestFunKey, entity_WCFAuthInfoVM.RequestFunTypeKey, ref strList_Error);

                if (ret)
                {
                    ret = Respo_F.Update(entity_FunVM, languageKey, ref strList_Error);
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

        public List<FunctionVM> GetParentFunctions(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_FunKey)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);
                List<FunctionVM> entityList_Fun = new List<FunctionVM>();

                if (entity_BaseSession != null)
                {
                    //Contruct Login User Respository
                    CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                    FunctionRespository Respo_F = new FunctionRespository(dbContext, entity_BaseSession.ID);

                    List<string> strList_Error = new List<string>();

                    bool ret = false;

                    ret = CheckTokenOnly(entity_BaseSession, ref strList_Error);

                    if (ret)
                    {
                        entityList_Fun = Respo_F.GetParents(str_FunKey);
                    }
                }

                return entityList_Fun;
            }
            catch (Exception ex)
            {
                throw new FaultException<WCFErrorContract>(new WCFErrorContract(ex), ex.Message);
            }
        }
    }
}