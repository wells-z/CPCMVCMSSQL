/**************************************************************************
*
* NAME        : FunMgtHelper.cs
*
* VERSION     : 1.0.0
*
* DATE        : 24-Feb-2016
*
* DESCRIPTION : FunMgtHelper
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
using CoolPrivilegeControlVM.WCFVM.FunSerVM;
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
    public class FunMgtHelper
    {
        IFunMgtSer _client;
        public FunMgtHelper(string str_ConfigName)
        {
            _client = new FunMgtSer();
        }

        public FunMgtHelper(FunMgtSer client)
        {
            _client = client;
        }

        public FunMgtHelper(CompositionContainer Container)
        {
            _client = new FunMgtSer(Container);
        }

        public FSerListResult GetListWithPaging(WCFAuthInfoVM entity_WCFAuthInfoVM, FunctionVM entity_SearchCriteria, int int_CurrentPage, int int_PageSize, string str_SortColumn, string str_SortDir, List<string> str_CustomFilter)
        {
            FSerListResult ret = null;

            ret = _client.GetListWithPaging(entity_WCFAuthInfoVM, entity_SearchCriteria, int_CurrentPage, int_PageSize, str_SortColumn, str_SortDir, str_CustomFilter);
            return ret;
        }

        public FSerEditResult GetEntityByID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_FunID)
        {
            FSerEditResult ret = null;
            ret = _client.GetEntityByID(entity_WCFAuthInfoVM, str_FunID);
            return ret;
        }

        public WCFReturnResult Create(WCFAuthInfoVM entity_WCFAuthInfoVM, FunctionVM functionVM)
        {
            WCFReturnResult ret = null;
            ret = _client.Create(entity_WCFAuthInfoVM, functionVM);
            return ret;
        }

        public WCFReturnResult Delete(WCFAuthInfoVM entity_WCFAuthInfoVM, string ID)
        {
            WCFReturnResult ret = null;
            ret = _client.Delete(entity_WCFAuthInfoVM, ID);
            return ret;
        }

        public WCFReturnResult Update(WCFAuthInfoVM entity_WCFAuthInfoVM, FunctionVM functionVM)
        {
            WCFReturnResult ret = null;
            ret = _client.Update(entity_WCFAuthInfoVM, functionVM);
            return ret;
        }

        public List<FunctionVM> GetAll(WCFAuthInfoVM entity_WCFAuthInfoVM)
        {
            List<FunctionVM> ret = new List<FunctionVM>();
            ret = _client.GetAll(entity_WCFAuthInfoVM);
            return ret;
        }

        public List<FunctionVM> GetParentFunctions(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_FKey)
        {
            List<FunctionVM> ret = new List<FunctionVM>();
            ret = _client.GetParentFunctions(entity_WCFAuthInfoVM, str_FKey);
            return ret;
        }

        public FunDetailInfo GetFunDetailInfo_FID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_FID)
        {
            FunDetailInfo ret = null;
            ret = _client.GetFunDetailInfo_FID(entity_WCFAuthInfoVM, str_FID);
            return ret;
        }
    }
}
