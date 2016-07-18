using CoolPrivilegeControlVM.CommonVM;
using CoolPrivilegeControlVM.EntityVM;
using CoolPrivilegeControlVM.WCFVM;
using CoolPrivilegeControlVM.WCFVM.SysInfoSerVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WCF_Infrastructure.UnityIntegration;

namespace CoolPrivilegeControlSerFactory.IService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISystemMgtSer" in both code and config file together.
    [ServiceContract]
    [CoolWCFBehavior]
    public interface ISystemMgtSer
    {
        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        SystemInfoVM GetSystemInfo(WCFAuthInfoVM entity_WCFAuthInfoVM);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        WCFReturnResult Update(WCFAuthInfoVM entity_WCFAuthInfoVM, SystemInfoVM entity_SysVM);
    }
}
