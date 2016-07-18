/**************************************************************************
*
* NAME        : CheckPrivilegeHelper.cs
*
* VERSION     : 1.0.0
*
* DATE        : 24-Feb-2016
*
* DESCRIPTION : CheckPrivilegeHelper
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
    public class CheckPrivilegeHelper
    {
        ICheckPrivilegeSer _client;
        public CheckPrivilegeHelper(string str_ConfigName)
        {
            _client = new CheckPrivilegeSer();
        }

        public CheckPrivilegeHelper(CheckPrivilegeSer client)
        {
            _client = client;
        }

        public CheckPrivilegeHelper(CompositionContainer Container)
        {
            _client = new CheckPrivilegeSer(Container);
        }

        public bool CheckPrivilege(WCFAuthInfoVM entity_WCFAuthInfoVM, bool isCheckFunType)
        {
            bool ret = false;
            ret = _client.CheckPrivilege(entity_WCFAuthInfoVM, isCheckFunType);
            return ret;
        }


        public bool CheckPrivilegeWithLUserID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_SpecificUserId, bool includeCurrentUser)
        {
            bool ret = false;
            ret = _client.CheckPrivilegeWithLUserID(entity_WCFAuthInfoVM, str_SpecificUserId, includeCurrentUser);
            return ret;
        }
    }
}
