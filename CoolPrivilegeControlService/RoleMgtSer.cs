using CoolPrivilegeControlSerFactory.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoolPrivilegeControlVM.EntityVM;
using System.ServiceModel;
using WCF_Infrastructure.Policies;
using CoolPrivilegeControlDAL.Common;
using CoolPrivilegeControlVM.CommonVM;
using CoolPrivilegeControlVM.WCFVM;
using CoolPrivilegeControlDAL.Respositories;
using System.ComponentModel.Composition;
using WCF_Infrastructure;
using CoolUtilities.MultiLingual;
using CoolPrivilegeControlVM;
using CoolPrivilegeControlVM.WCFVM.RoleSerVM;
using CoolPrivilegeControlModels.Models;
using System.ComponentModel.Composition.Hosting;

namespace CoolPrivilegeControlService
{
    public class RoleMgtSer : ServiceBase, IRoleMgtSer
    {
        public RoleMgtSer()
            : base()
        {

        }

        public RoleMgtSer(CompositionContainer Container)
            : base(Container)
        {

        }

        public List<LUserRoleVM> GetAll(WCFAuthInfoVM entity_WCFAuthInfoVM)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);

                List<string> strList_Error = new List<string>();

                bool ret_CheckPrivilege = false;

                ret_CheckPrivilege = CheckTokenOnly(entity_BaseSession, ref strList_Error);

                List<LUserRoleVM> entityList_RoleVM = new List<LUserRoleVM>();

                if (ret_CheckPrivilege)
                {
                    CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                    UserRoleRespository roleRespo = new UserRoleRespository(dbContext, entity_BaseSession.ID);

                    entityList_RoleVM = roleRespo.GetLURoleVM_All();
                }

                return entityList_RoleVM;
            }
            catch (Exception ex)
            {
                throw new FaultException<WCFErrorContract>(new WCFErrorContract(ex), ex.Message);
            }
        }

        public List<LUserRoleVM> GetEntityListByIDList(WCFAuthInfoVM entity_WCFAuthInfoVM, List<string> strList_RoleID)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);

                

                List<string> strList_Error = new List<string>();

                bool ret_CheckPrivilege = false;

                List<LUserRoleVM> ret = new List<LUserRoleVM>();

                //Contruct Login User Respository
                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                UserRoleRespository entityRepos_Role = new UserRoleRespository(dbContext, entity_BaseSession.ID);

                ret_CheckPrivilege = CheckTokenOnly(entity_BaseSession, ref strList_Error);

                List<LUserRoleVM> entityList_RoleVM = new List<LUserRoleVM>();

                if (ret_CheckPrivilege)
                    entityList_RoleVM = (List<LUserRoleVM>)entityRepos_Role.GetLURoleVMList_ByIDList(strList_RoleID);

                return entityList_RoleVM;
            }
            catch (Exception ex)
            {
                throw new FaultException<WCFErrorContract>(new WCFErrorContract(ex), ex.Message);
            }
        }

        public RoleSerListResult GetListWithPaging(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserRoleVM entity_SearchCriteria, int int_CurrentPage, int int_PageSize, string str_SortColumn, string str_SortDir, List<string> str_CustomFilter)
        {
            try
            {
                //Restore Server Session
                RetrieveServerSideSession(entity_WCFAuthInfoVM);

                

                bool ret_CheckPrivilege = false;

                List<string> strList_Error = new List<string>();

                RoleSerListResult returnResult = new RoleSerListResult();

                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                UserRoleRespository entityRepos_Role = new UserRoleRespository(dbContext, entity_BaseSession.ID);

                #region [ Check Privilege ]
                ret_CheckPrivilege = CheckAccPrivilege(entity_BaseSession.ID, entity_WCFAuthInfoVM.RequestFunKey, entity_WCFAuthInfoVM.RequestFunTypeKey, ref strList_Error);
                #endregion

                returnResult.StrList_Error = strList_Error;
                returnResult.Int_TotalRecordCount = 0;
                returnResult.EntityList_LUserRoleVM = new List<LUserRoleVM>();

                if (ret_CheckPrivilege)
                {
                    int recordCount = 0;

                    List<LUserRoleVM> vmList = entityRepos_Role.GetEntityListByPage(entity_SearchCriteria, int_CurrentPage, int_PageSize, str_SortColumn, str_SortDir, out recordCount, str_CustomFilter);

                    returnResult.EntityList_LUserRoleVM = vmList;
                    returnResult.Int_TotalRecordCount = recordCount;
                }

                return returnResult;
            }
            catch (Exception ex)
            {
                throw new FaultException<WCFErrorContract>(new WCFErrorContract(ex), ex.Message);
            }
        }

        public RoleSerEditResult GetEntityByID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_RoleID)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);

                

                List<string> strList_Error = new List<string>();

                RoleSerEditResult returnResult = new RoleSerEditResult();

                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                UserRoleRespository Respo_Role = new UserRoleRespository(dbContext, entity_BaseSession.ID);

                bool ret = false;

                ret = CheckAccPrivilege(entity_BaseSession.ID, entity_WCFAuthInfoVM.RequestFunKey, entity_WCFAuthInfoVM.RequestFunTypeKey, ref strList_Error);

                if (ret)
                {
                    Guid guid_RoleID = default(Guid);
                    if (Guid.TryParse(str_RoleID, out guid_RoleID))
                    {
                        LUserRoleVM db_OrgVM = Respo_Role.GetEntityByID(guid_RoleID, languageKey, ref strList_Error);

                        returnResult.Entity_LUserRoleVM = db_OrgVM;
                    }
                    else
                    {
                        ret = false;
                        string str_Message = MultilingualHelper.GetStringFromResource(languageKey, "E012");
                        strList_Error.Add(string.Format(str_Message, "ID"));
                    }
                }

                returnResult.StrList_Error = strList_Error;

                return returnResult;
            }
            catch (Exception ex)
            {
                throw new FaultException<WCFErrorContract>(new WCFErrorContract(ex), ex.Message);
            }
        }

        public WCFReturnResult Create(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserRoleVM entity_RoleVM)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);

                

                WCFReturnResult returnResult = new WCFReturnResult();

                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                UserRoleRespository entityRepos_Role = new UserRoleRespository(dbContext, entity_BaseSession.ID);

                List<string> strList_Error = new List<string>();

                bool ret = false;

                ret = CheckAccPrivilege(entity_BaseSession.ID, entity_WCFAuthInfoVM.RequestFunKey, entity_WCFAuthInfoVM.RequestFunTypeKey, ref strList_Error);

                if (ret)
                {
                    ret = entityRepos_Role.Create(entity_RoleVM, languageKey, ref strList_Error);
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

        public WCFReturnResult Delete(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_RoleID)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);

                

                WCFReturnResult returnResult = new WCFReturnResult();

                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                UserRoleRespository entityRepos_Role = new UserRoleRespository(dbContext, entity_BaseSession.ID);

                List<string> strList_Error = new List<string>();

                bool ret = false;

                ret = CheckAccPrivilege(entity_BaseSession.ID, entity_WCFAuthInfoVM.RequestFunKey, entity_WCFAuthInfoVM.RequestFunTypeKey, ref strList_Error);

                if (ret)
                {
                    Guid guid_RoleID = default(Guid);
                    if (Guid.TryParse(str_RoleID, out guid_RoleID))
                    {
                        ret = entityRepos_Role.Delete(guid_RoleID, languageKey, ref strList_Error);
                    }
                    else
                    {
                        ret = false;
                        string str_Message = MultilingualHelper.GetStringFromResource(languageKey, "E012");
                        strList_Error.Add(string.Format(str_Message, "ID"));
                    }
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

        public WCFReturnResult Update(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserRoleVM entity_RoleVM)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);

                

                WCFReturnResult returnResult = new WCFReturnResult();

                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                UserRoleRespository entityRepos_Role = new UserRoleRespository(dbContext, entity_BaseSession.ID);

                List<string> strList_Error = new List<string>();

                bool ret = false;

                ret = CheckAccPrivilege(entity_BaseSession.ID, entity_WCFAuthInfoVM.RequestFunKey, entity_WCFAuthInfoVM.RequestFunTypeKey, ref strList_Error);

                if (ret)
                {
                    ret = entityRepos_Role.Update(entity_RoleVM, languageKey, ref strList_Error);
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
    }
}
