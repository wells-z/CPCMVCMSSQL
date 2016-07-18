using CoolPrivilegeControlSerFactory.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using CoolPrivilegeControlVM.EntityVM;
using CoolPrivilegeControlDAL.Common;
using CoolPrivilegeControlVM.WCFVM;
using CoolPrivilegeControlDAL.Respositories;
using System.ServiceModel;
using System.ComponentModel.Composition;
using WCF_Infrastructure;
using CoolPrivilegeControlVM.WCFVM.OrgDetailsSerVM;
using CoolPrivilegeControlVM;
using CoolUtilities.MultiLingual;
using CoolPrivilegeControlModels.Models;
using CoolPrivilegeControlDAL.Policies;
using System.ComponentModel.Composition.Hosting;

namespace CoolPrivilegeControlService
{
    public class OrgDetailMgtSer : ServiceBase, IOrgDetailMgtSer
    {
        public OrgDetailMgtSer()
            : base()
        {

        }

        public OrgDetailMgtSer(CompositionContainer Container)
            : base(Container)
        {

        }

        public List<LUserOrgDetailsVM> GetAll(WCFAuthInfoVM entity_WCFAuthInfoVM)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);

                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                OrgDRespository orgDRespo = new OrgDRespository(dbContext, entity_BaseSession.ID);

                List<LUserOrgDetailsVM> entityList_OrgDetailsVM = new List<LUserOrgDetailsVM>();

                List<string> strList_Error = new List<string>();

                bool ret = false;

                ret = CheckTokenOnly(entity_BaseSession, ref strList_Error);

                if (ret)
                {
                    entityList_OrgDetailsVM = orgDRespo.GetLUOrgDVM_All();
                }

                return entityList_OrgDetailsVM;
            }
            catch (Exception ex)
            {
                throw new FaultException<WCFErrorContract>(new WCFErrorContract(ex), ex.Message);
            }
        }

        public ODSerListResult GetListWithPaging(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserOrgDetailsVM entity_SearchCriteria, int int_CurrentPage, int int_PageSize, string str_SortColumn, string str_SortDir, List<string> str_CustomFilter)
        {
            try
            {
                //Restore Server Session
                RetrieveServerSideSession(entity_WCFAuthInfoVM);



                bool ret_CheckPrivilege = false;

                List<string> strList_Error = new List<string>();

                ODSerListResult returnResult = new ODSerListResult();

                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                OrgDRespository entityRepos_OrgD = new OrgDRespository(dbContext, entity_BaseSession.ID);

                #region [ Check Privilege ]
                ret_CheckPrivilege = CheckAccPrivilege(entity_BaseSession.ID, entity_WCFAuthInfoVM.RequestFunKey, entity_WCFAuthInfoVM.RequestFunTypeKey, ref strList_Error);
                #endregion

                returnResult.StrList_Error = strList_Error;
                returnResult.Int_TotalRecordCount = 0;
                returnResult.EntityList_LUserOrgDetailsVM = new List<LUserOrgDetailsVM>();

                if (ret_CheckPrivilege)
                {
                    int recordCount = 0;

                    LUOrgDetailsAccessPolicy lUOrgDetailsAccessPolicy = new LUOrgDetailsAccessPolicy();

                    Func<List<LUserOrgDetails>, List<LUserOrgDetails>> func_OtherFilter = (entityList_OrgDetails) =>
                    {
                        List<LUserOrgDetails> ret = entityList_OrgDetails;
                        if (entity_SearchCriteria.OrgDetailsType.HasValue)
                        {
                            if (entity_SearchCriteria.OrgDetailsType.Value == 1)
                            {
                                ret = ret.Where(current => current.OD_Type.HasValue && current.OD_Type.Value == 1).ToList();
                            }
                            else if (entity_SearchCriteria.OrgDetailsType.Value == 2)
                            {
                                ret = ret.Where(current => current.OD_Type.HasValue && current.OD_Type.Value == 2).ToList();
                                if (!string.IsNullOrWhiteSpace(entity_SearchCriteria.SC_RoleName))
                                {
                                    List<LUserOrgDetailsVM> entityList_LoginUservm = lUOrgDetailsAccessPolicy.Get_OrgDetailsSettings_RoleName(dbContext, entity_SearchCriteria.SC_RoleName.ToString());

                                    var IDList_LoginUserVM = entityList_LoginUservm.Select(current => current.ID).ToList();

                                    ret = ret.Where(current => IDList_LoginUserVM.Contains(current.ID)).ToList();
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(entity_SearchCriteria.OrgDetailsKey))
                        {
                            string str_OrganizationKey = entity_SearchCriteria.OrgDetailsKey.ToString();
                            ret = ret.Except(ret.Where(current => current.OD_Key.IndexOf(str_OrganizationKey) != 0)).ToList();
                        }
                        return ret;
                    };

                    List<LUserOrgDetailsVM> vmList = entityRepos_OrgD.GetEntityListByPage(entity_SearchCriteria, int_CurrentPage, int_PageSize, str_SortColumn, str_SortDir, out recordCount, str_CustomFilter, func_OtherFilter, null, (entityList_VM) =>
                    {
                        foreach (var item in entityList_VM)
                        {
                            if (item.OrgDetailsType == 2)
                            {
                                List<LUserRoleVM> entityList_RoleVM = lUOrgDetailsAccessPolicy.Get_RoleSettings_OrgDID(item.ID);

                                item.EntityList_Role = entityList_RoleVM;
                                item.OrgDetailsTypeName = MultilingualHelper.GetStringFromResource(languageKey, "AsRoleSetting");
                            }
                            else if (item.OrgDetailsType == 1)
                            {
                                item.OrgDetailsTypeName = MultilingualHelper.GetStringFromResource(languageKey, "SpecificFunctions");
                            }
                        }
                        return entityList_VM;
                    });

                    returnResult.EntityList_LUserOrgDetailsVM = vmList;
                    returnResult.Int_TotalRecordCount = recordCount;
                }

                return returnResult;
            }
            catch (Exception ex)
            {
                throw new FaultException<WCFErrorContract>(new WCFErrorContract(ex), ex.Message);
            }
        }

        public ODSerEditResult GetEntityByID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_OrgDetailsID)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);



                List<string> strList_Error = new List<string>();

                ODSerEditResult returnResult = new ODSerEditResult();

                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                OrgDRespository Respo_OD = new OrgDRespository(dbContext, entity_BaseSession.ID);

                bool ret = false;

                ret = CheckAccPrivilege(entity_BaseSession.ID, entity_WCFAuthInfoVM.RequestFunKey, entity_WCFAuthInfoVM.RequestFunTypeKey, ref strList_Error);

                if (ret)
                {
                    Guid guid_OrgDID = default(Guid);
                    if (Guid.TryParse(str_OrgDetailsID, out guid_OrgDID))
                    {
                        LUserOrgDetailsVM entity_LUserOrgDetailsVM = Respo_OD.GetEntityByID(guid_OrgDID, languageKey, ref strList_Error);

                        returnResult.Entity_LUserOrgDetailsVM = entity_LUserOrgDetailsVM;
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

        public WCFReturnResult Create(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserOrgDetailsVM entity_OrgDetailsVM)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);



                WCFReturnResult returnResult = new WCFReturnResult();

                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                OrgDRespository entityRepos_OrgD = new OrgDRespository(dbContext, entity_BaseSession.ID);

                List<string> strList_Error = new List<string>();

                bool ret = false;

                ret = CheckAccPrivilege(entity_BaseSession.ID, entity_WCFAuthInfoVM.RequestFunKey, entity_WCFAuthInfoVM.RequestFunTypeKey, ref strList_Error);

                if (ret)
                {
                    ret = entityRepos_OrgD.Create(entity_OrgDetailsVM, languageKey, ref strList_Error);
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

        public WCFReturnResult Delete(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_OrgDetailsID)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);



                WCFReturnResult returnResult = new WCFReturnResult();

                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                OrgDRespository entityRepos_OrgD = new OrgDRespository(dbContext, entity_BaseSession.ID);

                List<string> strList_Error = new List<string>();

                bool ret = false;

                ret = CheckAccPrivilege(entity_BaseSession.ID, entity_WCFAuthInfoVM.RequestFunKey, entity_WCFAuthInfoVM.RequestFunTypeKey, ref strList_Error);

                if (ret)
                {
                    ret = entityRepos_OrgD.Delete(str_OrgDetailsID, languageKey, ref strList_Error);
                }
                else
                {
                    ret = false;
                    string str_Message = MultilingualHelper.GetStringFromResource(languageKey, "E012");
                    strList_Error.Add(string.Format(str_Message, "ID"));
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

        public WCFReturnResult Update(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserOrgDetailsVM entity_OrgDetailsVM)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);



                WCFReturnResult returnResult = new WCFReturnResult();

                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                OrgDRespository Respo_OrgD = new OrgDRespository(dbContext, entity_BaseSession.ID);

                List<string> strList_Error = new List<string>();

                bool ret = false;

                ret = CheckAccPrivilege(entity_BaseSession.ID, entity_WCFAuthInfoVM.RequestFunKey, entity_WCFAuthInfoVM.RequestFunTypeKey, ref strList_Error);

                if (ret)
                {
                    ret = Respo_OrgD.Update(entity_OrgDetailsVM, languageKey, ref strList_Error);
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

        public List<LUserRoleVM> GetRoleSettingsByOrgDID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_OrgDetailsID)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);

                WCFReturnResult returnResult = new WCFReturnResult();

                LUOrgDetailsAccessPolicy lUOrgDetailsAccessPolicy = new LUOrgDetailsAccessPolicy();

                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                bool ret = false;

                List<LUserRoleVM> entityList_R = new List<LUserRoleVM>();

                List<string> strList_Error = new List<string>();

                ret = CheckTokenOnly(entity_BaseSession, ref strList_Error);

                if (ret)
                {
                    Guid guid_OrgDID = default(Guid);
                    if (Guid.TryParse(str_OrgDetailsID, out guid_OrgDID))
                    {
                        entityList_R = lUOrgDetailsAccessPolicy.Get_RoleSettings_OrgDID(guid_OrgDID);
                    }
                }
                return entityList_R;
            }
            catch (Exception ex)
            {
                throw new FaultException<WCFErrorContract>(new WCFErrorContract(ex), ex.Message);
            }
        }

        public List<FunDetailInfo> GetPrivilegeByUserID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_ID, RoleType enum_RoleType)
        {
            try
            {
                //Retrieve Language And Session
                RetrieveLanguageAndSession(entity_WCFAuthInfoVM);



                WCFReturnResult returnResult = new WCFReturnResult();

                AccPrivilegePolicy userRoleFunDetailsPolicy = new AccPrivilegePolicy();

                CoolPrivilegeControlContext dbContext = CoolPrivilegeControlContext.CreateContext();

                bool ret = false;

                List<FunDetailInfo> entityList_FunDetailInfo = new List<FunDetailInfo>();

                List<string> strList_Error = new List<string>();

                ret = CheckTokenOnly(entity_BaseSession, ref strList_Error);

                if (ret)
                {
                    Guid guid_OrgDID = default(Guid);
                    if (Guid.TryParse(str_ID, out guid_OrgDID))
                    {
                        entityList_FunDetailInfo = userRoleFunDetailsPolicy.Get_LoginUserPrivilege_UserID(dbContext, guid_OrgDID, RoleType.Role);
                    }
                }
                return entityList_FunDetailInfo;
            }
            catch (Exception ex)
            {
                throw new FaultException<WCFErrorContract>(new WCFErrorContract(ex), ex.Message);
            }
        }
    }
}
