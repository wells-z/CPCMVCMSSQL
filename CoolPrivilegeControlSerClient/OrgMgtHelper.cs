/**************************************************************************
*
* NAME        : OrgMgtHelper.cs
*
* VERSION     : 1.0.0
*
* DATE        : 24-Feb-2016
*
* DESCRIPTION : OrgMgtHelper
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
using CoolPrivilegeControlVM.WCFVM.OrgSerVM;
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
    public class OrgMgtHelper
    {
        IOrgMgtSer _client;
        public OrgMgtHelper(string str_ConfigName)
        {
            _client = new OrgMgtSer();
        }

        public OrgMgtHelper(OrgMgtSer client)
        {
            _client = client;
        }

        public OrgMgtHelper(CompositionContainer Container)
        {
            _client = new OrgMgtSer(Container);
        }

        public OrgSerListResult GetListWithPaging(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserOrganizationVM entity_SearchCriteria, int int_CurrentPage, int int_PageSize, string str_SortColumn, string str_SortDir, List<string> str_CustomFilter)
        {
            OrgSerListResult ret = null;
            ret = _client.GetListWithPaging(entity_WCFAuthInfoVM, entity_SearchCriteria, int_CurrentPage, int_PageSize, str_SortColumn, str_SortDir, str_CustomFilter);
            return ret;
        }

        public OrgSerEditResult GetEntityByID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_ID)
        {
            OrgSerEditResult ret = null;
            ret = _client.GetEntityByID(entity_WCFAuthInfoVM, str_ID);
            return ret;
        }

        public WCFReturnResult Create(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserOrganizationVM orgVM)
        {
            WCFReturnResult ret = null;
            ret = _client.Create(entity_WCFAuthInfoVM, orgVM);
            return ret;
        }

        public WCFReturnResult Delete(WCFAuthInfoVM entity_WCFAuthInfoVM, string ID)
        {
            WCFReturnResult ret = null;
            ret = _client.Delete(entity_WCFAuthInfoVM, ID);
            return ret;
        }

        public WCFReturnResult Update(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserOrganizationVM orgVM)
        {
            WCFReturnResult ret = null;
            ret = _client.Update(entity_WCFAuthInfoVM, orgVM);
            return ret;
        }

        public List<LUserOrganizationVM> GetAll(WCFAuthInfoVM entity_WCFAuthInfoVM)
        {
            List<LUserOrganizationVM> ret = new List<LUserOrganizationVM>();
            ret = _client.GetAll(entity_WCFAuthInfoVM);
            return ret;
        }

        public List<LUserAccessByOrgVM> GetEntityListByIDList_LUserAccessByOrgVM(WCFAuthInfoVM entity_WCFAuthInfoVM, List<string> strList_OrgID, List<string> strList_OrgDetailsID)
        {
            List<LUserAccessByOrgVM> ret = new List<LUserAccessByOrgVM>();
            ret = _client.GetEntityListByIDList_LUserAccessByOrgVM(entity_WCFAuthInfoVM, strList_OrgID, strList_OrgDetailsID);
            return ret;
        }

        public List<string> GetOrgPathListByLUID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_UserID)
        {
            List<string> ret = new List<string>();
            ret = _client.GetOrgPathListByLUID(entity_WCFAuthInfoVM, str_UserID);
            return ret;
        }
    }
}
