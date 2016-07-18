/**************************************************************************
*
* NAME        : AuthHisMgtHelper.cs
*
* VERSION     : 1.0.0
*
* DATE        : 24-Feb-2016
*
* DESCRIPTION : AuthHisMgtHelper
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
using CoolPrivilegeControlVM.WCFVM.AuthorizedMgtSerVM;
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
    public class AuthHisMgtHelper
    {
        IAuthHisMgtSer _client;
        public AuthHisMgtHelper(string str_ConfigName)
        {
            _client = new AuthHisMgtSer();
        }

        public AuthHisMgtHelper(AuthHisMgtSer client)
        {
            _client = client;
        }

        public AuthHisMgtHelper(CompositionContainer Container)
        {
            _client = new AuthHisMgtSer(Container);
        }

        public AHSerListResult GetListWithPaging(WCFAuthInfoVM entity_WCFAuthInfoVM, AuthorizedHistoryVM entity_SearchCriteria, int int_CurrentPage, int int_PageSize, string str_SortColumn, string str_SortDir, List<string> str_CustomFilter)
        {
            AHSerListResult ret = null;

            ret = _client.GetListWithPaging(entity_WCFAuthInfoVM, entity_SearchCriteria, int_CurrentPage, int_PageSize, str_SortColumn, str_SortDir, str_CustomFilter);

            return ret;
        }


        public int GetTotalAuthorizationCount(WCFAuthInfoVM entity_WCFAuthInfoVM)
        {
            int ret = 0;
            ret = _client.GetTotalAuthorizationCount(entity_WCFAuthInfoVM);
            return ret;
        }

        public WCFReturnResult Delete(WCFAuthInfoVM entity_WCFAuthInfoVM, string ID)
        {
            WCFReturnResult ret = null;
            ret = _client.Delete(entity_WCFAuthInfoVM, ID);
            return ret;
        }
    }
}
