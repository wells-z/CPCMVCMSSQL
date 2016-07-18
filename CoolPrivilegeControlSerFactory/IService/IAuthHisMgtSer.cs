/**************************************************************************
*
* NAME        : IAuthHisMgtSer.cs
*
* VERSION     : 1.0.0
*
* DATE        : 03-Feb-2016
*
* DESCRIPTION : IAuthHisMgtSer
*
* MODIFICATION HISTORY
* Name             Date         Description
* ===============  ===========  =======================================
* Wells Cheung     03-Feb-2016  Initial Version
*
**************************************************************************/
using CoolPrivilegeControlVM.EntityVM;
using CoolPrivilegeControlVM.WCFVM;
using CoolPrivilegeControlVM.WCFVM.AuthorizedMgtSerVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WCF_Infrastructure.UnityIntegration;

namespace CoolPrivilegeControlSerFactory.IService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAuthHisMgtSer" in both code and config file together.
    [ServiceContract]
    [CoolWCFBehavior]
    public interface IAuthHisMgtSer
    {
        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        AHSerListResult GetListWithPaging(WCFAuthInfoVM entity_WCFAuthInfoVM, AuthorizedHistoryVM entity_SearchCriteria, int int_CurrentPage, int int_PageSize, string str_SortColumn, string str_SortDir, List<string> str_CustomFilter);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        int GetTotalAuthorizationCount(WCFAuthInfoVM entity_WCFAuthInfoVM);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        WCFReturnResult Delete(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_AuthHisID);
    }
}
