/**************************************************************************
*
* NAME        : IRoleMgtSer.cs
*
* VERSION     : 1.0.0
*
* DATE        : 03-Feb-2016
*
* DESCRIPTION : IRoleMgtSer
*
* MODIFICATION HISTORY
* Name             Date         Description
* ===============  ===========  =======================================
* Wells Cheung     03-Feb-2016  Initial Version
*
**************************************************************************/
using CoolPrivilegeControlVM.EntityVM;
using CoolPrivilegeControlVM.WCFVM;
using CoolPrivilegeControlVM.WCFVM.RoleSerVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WCF_Infrastructure.UnityIntegration;

namespace CoolPrivilegeControlSerFactory.IService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRoleMgtSer" in both code and config file together.
    [ServiceContract]
    [CoolWCFBehavior]
    public interface IRoleMgtSer
    {
        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        RoleSerListResult GetListWithPaging(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserRoleVM entity_SearchCriteria, int int_CurrentPage, int int_PageSize, string str_SortColumn, string str_SortDir, List<string> str_CustomFilter);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        RoleSerEditResult GetEntityByID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_RoleID);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        List<LUserRoleVM> GetEntityListByIDList(WCFAuthInfoVM entity_WCFAuthInfoVM, List<string> strList_RoleID);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        WCFReturnResult Create(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserRoleVM entity_RoleVM);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        WCFReturnResult Delete(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_RoleID);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        WCFReturnResult Update(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserRoleVM entity_RoleVM);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        List<LUserRoleVM> GetAll(WCFAuthInfoVM entity_WCFAuthInfoVM);
    }
}
