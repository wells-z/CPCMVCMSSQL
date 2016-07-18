/**************************************************************************
*
* NAME        : RoleMgtHelper.cs
*
* VERSION     : 1.0.0
*
* DATE        : 24-Feb-2016
*
* DESCRIPTION : RoleMgtHelper
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
using CoolPrivilegeControlVM.WCFVM.RoleSerVM;
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
    public class RoleMgtHelper
    {
        IRoleMgtSer _client;
        public RoleMgtHelper(string str_ConfigName)
        {
            _client = new RoleMgtSer();
        }

        public RoleMgtHelper(RoleMgtSer client)
        {
            _client = client;
        }

        public RoleMgtHelper(CompositionContainer Container)
        {
            _client = new RoleMgtSer(Container);
        }

        public RoleSerListResult GetListWithPaging(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserRoleVM entity_SearchCriteria, int int_CurrentPage, int int_PageSize, string str_SortColumn, string str_SortDir, List<string> str_CustomFilter)
        {
            RoleSerListResult ret = null;
            ret = _client.GetListWithPaging(entity_WCFAuthInfoVM, entity_SearchCriteria, int_CurrentPage, int_PageSize, str_SortColumn, str_SortDir, str_CustomFilter);
            return ret;
        }


        public RoleSerEditResult GetEntityByID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_ID)
        {
            RoleSerEditResult ret = null;
            ret = _client.GetEntityByID(entity_WCFAuthInfoVM, str_ID);
            return ret;
        }


        public List<LUserRoleVM> GetEntityListByIDList(WCFAuthInfoVM entity_WCFAuthInfoVM, List<string> strList_ID)
        {
            List<LUserRoleVM> ret = new List<LUserRoleVM>();
            ret = _client.GetEntityListByIDList(entity_WCFAuthInfoVM, strList_ID);
            return ret;
        }


        public WCFReturnResult Create(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserRoleVM roleVM)
        {
            WCFReturnResult ret = null;
            ret = _client.Create(entity_WCFAuthInfoVM, roleVM);
            return ret;
        }


        public WCFReturnResult Delete(WCFAuthInfoVM entity_WCFAuthInfoVM, string ID)
        {
            WCFReturnResult ret = null;
            ret = _client.Delete(entity_WCFAuthInfoVM, ID);
            return ret;
        }


        public WCFReturnResult Update(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserRoleVM roleVM)
        {
            WCFReturnResult ret = null;
            ret = _client.Update(entity_WCFAuthInfoVM, roleVM);
            return ret;
        }


        public List<LUserRoleVM> GetAll(WCFAuthInfoVM entity_WCFAuthInfoVM)
        {
            List<LUserRoleVM> ret = new List<LUserRoleVM>();
            ret = _client.GetAll(entity_WCFAuthInfoVM);
            return ret;
        }
    }
}
