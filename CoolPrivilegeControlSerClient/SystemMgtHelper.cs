/**************************************************************************
*
* NAME        : SystemMgtHelper.cs
*
* VERSION     : 1.0.0
*
* DATE        : 24-Feb-2016
*
* DESCRIPTION : SystemMgtHelper
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
using CoolPrivilegeControlVM.WCFVM.SysInfoSerVM;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition.Hosting;

namespace CoolPrivilegeControlSerClient
{
    public class SystemMgtHelper
    {
        ISystemMgtSer _client;

        public SystemMgtHelper(string str_ConfigName)
        {
            _client = new SystemMgtSer();
        }

        public SystemMgtHelper(ISystemMgtSer client)
        {
            _client = client;
        }

        public SystemMgtHelper(CompositionContainer Container)
        {
            _client = new SystemMgtSer(Container);
        }

        public SystemInfoVM GetSystemInfo(WCFAuthInfoVM entity_WCFAuthInfoVM)
        {
            SystemInfoVM ret = null;

            ret = _client.GetSystemInfo(entity_WCFAuthInfoVM);

            return ret;
        }

        public WCFReturnResult Update(WCFAuthInfoVM entity_WCFAuthInfoVM, SystemInfoVM systemInfoVM)
        {
            WCFReturnResult ret = null;

            ret = _client.Update(entity_WCFAuthInfoVM, systemInfoVM);

            return ret;
        }
    }
}
