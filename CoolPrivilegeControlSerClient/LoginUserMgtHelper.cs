/**************************************************************************
*
* NAME        : LoginUserMgtHelper.cs
*
* VERSION     : 1.0.0
*
* DATE        : 24-Feb-2016
*
* DESCRIPTION : LoginUserMgtHelper
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
using CoolPrivilegeControlVM.WCFVM.LoginUserSerVM;
using CoolPrivilegeControlVM.WEBVM;
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
    public class LoginUserMgtHelper
    {
        ILoginUserMgtSer _client;
        public LoginUserMgtHelper(string str_ConfigName)
        {
            _client = new LoginUserMgtSer();
        }

        public LoginUserMgtHelper(LoginUserMgtSer client)
        {
            _client = client;
        }

        public LoginUserMgtHelper(CompositionContainer Container)
        {
            _client = new LoginUserMgtSer(Container);
        }

        public LUSerListResult GetListWithPaging(WCFAuthInfoVM entity_WCFAuthInfoVM, LoginUserVM entity_SearchCriteria, int int_CurrentPage, int int_PageSize, string str_SortColumn, string str_SortDir, List<string> str_CustomFilter, List<Guid> guidList_AccessedLUserID)
        {
            LUSerListResult ret = null;
            ret = _client.GetListWithPaging(entity_WCFAuthInfoVM, entity_SearchCriteria, int_CurrentPage, int_PageSize, str_SortColumn, str_SortDir, str_CustomFilter, guidList_AccessedLUserID);
            return ret;
        }

        public LUSerEditResult GetEntityByID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_LUID)
        {
            LUSerEditResult ret = null;
            ret = _client.GetEntityByID(entity_WCFAuthInfoVM, str_LUID);
            return ret;
        }

        public LUSerEditResult GetEntityByIDWDetails(WCFAuthInfoVM entity_WCFAuthInfoVM, string UserID)
        {
            LUSerEditResult ret = null;
            ret = _client.GetEntityByIDWDetails(entity_WCFAuthInfoVM, UserID);
            return ret;
        }

        public WCFReturnResult Create(WCFAuthInfoVM entity_WCFAuthInfoVM, LoginUserVM loginUserVM)
        {
            WCFReturnResult ret = null;
            ret = _client.Create(entity_WCFAuthInfoVM, loginUserVM);
            return ret;
        }

        public WCFReturnResult Delete(WCFAuthInfoVM entity_WCFAuthInfoVM, string ID)
        {
            WCFReturnResult ret = null;
            ret = _client.Delete(entity_WCFAuthInfoVM, ID);
            return ret;
        }

        public WCFReturnResult Update(WCFAuthInfoVM entity_WCFAuthInfoVM, LoginUserVM loginUserVM)
        {
            WCFReturnResult ret = null;
            ret = _client.Update(entity_WCFAuthInfoVM, loginUserVM);
            return ret;
        }

        public WCFReturnResult ResetPwd(WCFAuthInfoVM entity_WCFAuthInfoVM, LoginUserVM entityInst)
        {
            WCFReturnResult ret = null;
            ret = _client.ResetPwd(entity_WCFAuthInfoVM, entityInst);
            return ret;
        }

        public LUSerLoginResult Login(LoginUserVM entityInst, string str_Language, string str_IpAdd, string str_HostName)
        {
            LUSerLoginResult ret = null;
            ret = _client.Login(entityInst, str_Language, str_IpAdd, str_HostName);
            return ret;
        }

        public WCFReturnResult Logout(WCFAuthInfoVM entity_WCFAuthInfoVM)
        {
            WCFReturnResult ret = null;
            ret = _client.Logout(entity_WCFAuthInfoVM);
            return ret;
        }

        public SessionWUserInfo GetAuthInfo(WCFAuthInfoVM entity_WCFAuthInfoVM)
        {
            SessionWUserInfo ret = null;
            ret = _client.GetAuthInfo(entity_WCFAuthInfoVM);
            return ret;
        }

        public List<Guid> GetLUIDList(WCFAuthInfoVM entity_WCFAuthInfoVM, List<string> strList_Path)
        {
            List<Guid> ret = new List<Guid>();
            ret = _client.GetLUIDList(entity_WCFAuthInfoVM, strList_Path);
            return ret;
        }
    }
}
