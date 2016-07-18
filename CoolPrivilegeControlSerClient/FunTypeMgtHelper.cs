/**************************************************************************
*
* NAME        : FunTypeMgtHelper.cs
*
* VERSION     : 1.0.0
*
* DATE        : 24-Feb-2016
*
* DESCRIPTION : FunTypeMgtHelper
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
using CoolPrivilegeControlVM.WCFVM.FunTypeSerVM;
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
    public class FunTypeMgtHelper
    {
        IFunTypeMgtSer _client;
        public FunTypeMgtHelper(string str_ConfigName)
        {
            _client = new FunTypeMgtSer();
        }

        public FunTypeMgtHelper(FunTypeMgtSer client)
        {
            _client = client;
        }

        public FunTypeMgtHelper(CompositionContainer Container)
        {
            _client = new FunTypeMgtSer(Container);
        }

        public FTSerListResult GetListWithPaging(WCFAuthInfoVM entity_WCFAuthInfoVM, FunctionTypeVM entity_SearchCriteria, int int_CurrentPage, int int_PageSize, string str_SortColumn, string str_SortDir, List<string> str_CustomFilter)
        {
            FTSerListResult ret = null;
            ret = _client.GetListWithPaging(entity_WCFAuthInfoVM, entity_SearchCriteria, int_CurrentPage, int_PageSize, str_SortColumn, str_SortDir, str_CustomFilter);
            return ret;
        }

        public FTSerEditResult GetEntityByID(WCFAuthInfoVM entity_WCFAuthInfoVM, string str_FTID)
        {
            FTSerEditResult ret = null;
            ret = _client.GetEntityByID(entity_WCFAuthInfoVM, str_FTID);
            return ret;
        }


        public WCFReturnResult Create(WCFAuthInfoVM entity_WCFAuthInfoVM, FunctionTypeVM functionTypeVM)
        {
            WCFReturnResult ret = null;
            ret = _client.Create(entity_WCFAuthInfoVM, functionTypeVM);
            return ret;
        }


        public WCFReturnResult Delete(WCFAuthInfoVM entity_WCFAuthInfoVM, string ID)
        {
            WCFReturnResult ret = null;
            ret = _client.Delete(entity_WCFAuthInfoVM, ID);
            return ret;
        }


        public WCFReturnResult Update(WCFAuthInfoVM entity_WCFAuthInfoVM, FunctionTypeVM functionTypeVM)
        {
            WCFReturnResult ret = null;
            ret = _client.Update(entity_WCFAuthInfoVM, functionTypeVM);
            return ret;
        }


        public List<FunctionTypeVM> GetAllFunType(WCFAuthInfoVM entity_WCFAuthInfoVM)
        {
            List<FunctionTypeVM> ret = new List<FunctionTypeVM>();
            ret = _client.GetAllFunType(entity_WCFAuthInfoVM);
            return ret;
        }
    }
}
