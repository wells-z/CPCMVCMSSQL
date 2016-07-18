/**************************************************************************
*
* NAME        : ILoginUserMgtSer.cs
*
* VERSION     : 1.0.0
*
* DATE        : 03-Feb-2016
*
* DESCRIPTION : ILoginUserMgtSer
*
* MODIFICATION HISTORY
* Name             Date         Description
* ===============  ===========  =======================================
* Wells Cheung     03-Feb-2016  Initial Version
*
**************************************************************************/
using CoolPrivilegeControlVM.EntityVM;
using CoolPrivilegeControlVM.WCFVM;
using CoolPrivilegeControlVM.WCFVM.LoginUserSerVM;
using CoolPrivilegeControlVM.WEBVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WCF_Infrastructure.UnityIntegration;

namespace CoolPrivilegeControlSerFactory.IService
{
    [ServiceContract]
    [CoolWCFBehavior]
    public interface ILoginUserMgtSer
    {
        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        LUSerListResult GetListWithPaging(WCFAuthInfoVM entity_WCFAuthInfoVM, LoginUserVM entity_SearchCriteria, int int_CurrentPage, int int_PageSize, string str_SortColumn, string str_SortDir, List<string> str_CustomFilter, List<Guid> guidList_AccessedLUserID);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        LUSerEditResult GetEntityByID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_LUID);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        LUSerEditResult GetEntityByIDWDetails(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_LUID);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        WCFReturnResult Create(WCFAuthInfoVM entity_WCFAuthInfoVM, LoginUserVM entity_LUVM);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        WCFReturnResult Delete(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_LUID);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        WCFReturnResult Update(WCFAuthInfoVM entity_WCFAuthInfoVM, LoginUserVM entity_LUVM);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        WCFReturnResult ResetPwd(WCFAuthInfoVM entity_WCFAuthInfoVM, LoginUserVM entity_LUVM);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        LUSerLoginResult Login(LoginUserVM entityInst, string str_Language, string str_IpAdd, string str_HostName);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        WCFReturnResult Logout(WCFAuthInfoVM entity_WCFAuthInfoVM);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        SessionWUserInfo GetAuthInfo(WCFAuthInfoVM entity_WCFAuthInfoVM);

        [OperationContract]
        [FaultContract(typeof(WCFErrorContract))]
        [CoolWCFBehavior]
        List<Guid> GetLUIDList(WCFAuthInfoVM entity_WCFAuthInfoVM, List<string> strList_OrgPath);
    }
}
