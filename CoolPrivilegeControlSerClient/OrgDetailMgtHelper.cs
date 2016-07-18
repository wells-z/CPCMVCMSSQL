/**************************************************************************
*
* NAME        : OrgDetailMgtHelper.cs
*
* VERSION     : 1.0.0
*
* DATE        : 24-Feb-2016
*
* DESCRIPTION : OrgDetailMgtHelper
*
* MODIFICATION HISTORY
* Name             Date         Description
* ===============  ===========  =======================================
* Wells Cheung     24-Feb-2016  Initial Version
*
**************************************************************************/
using CoolPrivilegeControlService;
using CoolPrivilegeControlSerFactory.IService;
using System.ComponentModel.Composition.Hosting;
using CoolPrivilegeControlVM.EntityVM;
using CoolPrivilegeControlVM.WCFVM;
using CoolPrivilegeControlVM.WCFVM.OrgDetailsSerVM;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
namespace CoolPrivilegeControlSerClient
{
    public class OrgDetailMgtHelper
    {
        IOrgDetailMgtSer _client;
        public OrgDetailMgtHelper(string str_ConfigName)
        {
            _client = new OrgDetailMgtSer();
        }

        public OrgDetailMgtHelper(OrgDetailMgtSer client)
        {
            _client = client;
        }

        public OrgDetailMgtHelper(CompositionContainer Container)
        {
            _client = new OrgDetailMgtSer(Container);
        }

        public ODSerListResult GetListWithPaging(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserOrgDetailsVM entity_SearchCriteria, int int_CurrentPage, int int_PageSize, string str_SortColumn, string str_SortDir, List<string> str_CustomFilter)
        {
            ODSerListResult ret = null;
            ret = _client.GetListWithPaging(entity_WCFAuthInfoVM, entity_SearchCriteria, int_CurrentPage, int_PageSize, str_SortColumn, str_SortDir, str_CustomFilter);
            return ret;
        }

        public ODSerEditResult GetEntityByID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_OrgDetailsID)
        {
            ODSerEditResult ret = null;
            ret = _client.GetEntityByID(entity_WCFAuthInfoVM, str_OrgDetailsID);
            return ret;
        }

        public WCFReturnResult Create(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserOrgDetailsVM orgDetailsVM)
        {
            WCFReturnResult ret = null;
            ret = _client.Create(entity_WCFAuthInfoVM, orgDetailsVM);
            return ret;
        }

        public WCFReturnResult Delete(WCFAuthInfoVM entity_WCFAuthInfoVM, string ID)
        {
            WCFReturnResult ret = null;
            ret = _client.Delete(entity_WCFAuthInfoVM, ID);
            return ret;
        }

        public WCFReturnResult Update(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserOrgDetailsVM orgDetailsVM)
        {
            WCFReturnResult ret = null;
            ret = _client.Update(entity_WCFAuthInfoVM, orgDetailsVM);
            return ret;
        }

        public List<LUserOrgDetailsVM> GetAll(WCFAuthInfoVM entity_WCFAuthInfoVM)
        {
            List<LUserOrgDetailsVM> ret = new List<LUserOrgDetailsVM>();
            ret = _client.GetAll(entity_WCFAuthInfoVM);
            return ret;
        }

        public List<LUserRoleVM> GetRoleSettingsByOrgDID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_OrgDetails)
        {
            List<LUserRoleVM> ret = new List<LUserRoleVM>();
            ret = _client.GetRoleSettingsByOrgDID(entity_WCFAuthInfoVM, str_OrgDetails);
            return ret;
        }


        public List<FunDetailInfo> GetPrivilegeByUserID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_roleID, RoleType enum_RoleType)
        {
            List<FunDetailInfo> ret = new List<FunDetailInfo>();
            ret = _client.GetPrivilegeByUserID(entity_WCFAuthInfoVM, str_roleID, enum_RoleType);
            return ret;
        }
    }
}
