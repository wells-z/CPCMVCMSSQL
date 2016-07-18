/**************************************************************************
*
* NAME        : IOrgMgtSer.cs
*
* VERSION     : 1.0.0
*
* DATE        : 03-Feb-2016
*
* DESCRIPTION : IOrgMgtSer
*
* MODIFICATION HISTORY
* Name             Date         Description
* ===============  ===========  =======================================
* Wells Cheung     03-Feb-2016  Initial Version
*
**************************************************************************/
using CoolPrivilegeControlVM.EntityVM;
using CoolPrivilegeControlVM.WCFVM;
using CoolPrivilegeControlVM.WCFVM.OrgSerVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WCF_Infrastructure.UnityIntegration;

namespace CoolPrivilegeControlSerFactory.IService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IOrgMgtSer" in both code and config file together.
    [ServiceContract]
    [CoolWCFBehavior]
    public interface IOrgMgtSer
    {
        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        OrgSerListResult GetListWithPaging(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserOrganizationVM entity_SearchCriteria, int int_CurrentPage, int int_PageSize, string str_SortColumn, string str_SortDir, List<string> str_CustomFilter);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        OrgSerEditResult GetEntityByID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_OrgID);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        WCFReturnResult Create(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserOrganizationVM entity_OrgVM);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        WCFReturnResult Delete(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_OrgID);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        WCFReturnResult Update(WCFAuthInfoVM entity_WCFAuthInfoVM, LUserOrganizationVM entity_OrgVM);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        List<LUserOrganizationVM> GetAll(WCFAuthInfoVM entity_WCFAuthInfoVM);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        List<LUserAccessByOrgVM> GetEntityListByIDList_LUserAccessByOrgVM(WCFAuthInfoVM entity_WCFAuthInfoVM, List<string> strList_OrgID, List<string> strList_OrgDetailsID);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        List<string> GetOrgPathListByLUID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_UserID);
    }
}
