/**************************************************************************
*
* NAME        : AuditLogMgtHelper.cs
*
* VERSION     : 1.0.0
*
* DATE        : 24-Feb-2016
*
* DESCRIPTION : AuditLogMgtHelper
*
* MODIFICATION HISTORY
* Name             Date         Description
* ===============  ===========  =======================================
* Wells Cheung     24-Feb-2016  Initial Version
*
**************************************************************************/
using CoolPrivilegeControlService;
using CoolPrivilegeControlSerFactory.IService;
using CoolPrivilegeControlVM.EntityVM;
using CoolPrivilegeControlVM.WCFVM;
using CoolPrivilegeControlVM.WCFVM.AuditLogSerVM;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace CoolPrivilegeControlSerClient
{
    public class AuditLogMgtHelper
    {
        IAuditLogMgtSer _client;
        public AuditLogMgtHelper(string str_ConfigName)
        {
            _client = new AuditLogMgtSer();
        }

        public AuditLogMgtHelper(IAuditLogMgtSer client)
        {
            _client = client;
        }

        public AuditLogMgtHelper(CompositionContainer Container)
        {
            _client = new AuditLogMgtSer(Container);
        }

        public ALSerListResult GetListWithPaging(WCFAuthInfoVM entity_WCFAuthInfoVM, AuditLogVM entity_SearchCriteria, int int_CurrentPage, int int_PageSize, string str_SortColumn, string str_SortDir, List<string> str_CustomFilter)
        {
            ALSerListResult ret = null;
            ret = _client.GetListWithPaging(entity_WCFAuthInfoVM, entity_SearchCriteria, int_CurrentPage, int_PageSize, str_SortColumn, str_SortDir, str_CustomFilter);
            return ret;
        }
    }
}
