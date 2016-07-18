
using CoolPrivilegeControlVM.CommonVM;
using CoolPrivilegeControlVM.EntityVM;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI_Infrastructure.IRegistrar;
using UI_Infrastructure.ModelBinder;

namespace CoolPrivilegeControl.ModelBinderRegistrar
{
    [Export(typeof(IModelBinderRegistrar))]
    [ExportMetadata("Order", 1)]
    public class CustomModelBinderRegistrar : IModelBinderRegistrar
    {
        private CompositionContainer _container;

        [ImportingConstructor]
        public CustomModelBinderRegistrar(CompositionContainer container)
        {
            _container = container;
        }

        public void RegisterModelBinder(System.Web.Mvc.ModelBinderDictionary modelBinderDictionary)
        {
            //if (!modelBinderDictionary.ContainsKey(typeof(LoginUserVM)))
            //{
            modelBinderDictionary.Add(typeof(AccPrivilegeVM), new CustomModelBinder<AccPrivilegeVM>(_container));
            modelBinderDictionary.Add(typeof(AuditLogVM), new CustomModelBinder<AuditLogVM>(_container));
            modelBinderDictionary.Add(typeof(AuthorizedHistoryVM), new CustomModelBinder<AuthorizedHistoryVM>(_container));
            modelBinderDictionary.Add(typeof(AuthorizedInfoVM), new CustomModelBinder<AuthorizedInfoVM>(_container));
            modelBinderDictionary.Add(typeof(CTComboBoxVM), new CustomModelBinder<CTComboBoxVM>(_container));
            modelBinderDictionary.Add(typeof(CurrentInfoVM), new CustomModelBinder<CurrentInfoVM>(_container));
            modelBinderDictionary.Add(typeof(FunctionDetailVM), new CustomModelBinder<FunctionDetailVM>(_container));
            modelBinderDictionary.Add(typeof(FunctionTypeVM), new CustomModelBinder<FunctionTypeVM>(_container));
            modelBinderDictionary.Add(typeof(FunctionVM), new CustomModelBinder<FunctionVM>(_container));
            modelBinderDictionary.Add(typeof(FunDetailInfo), new CustomModelBinder<FunDetailInfo>(_container));
            modelBinderDictionary.Add(typeof(LoginUserVM), new CustomModelBinder<LoginUserVM>(_container));

            modelBinderDictionary.Add(typeof(LUserAccessByOrgVM), new CustomModelBinder<LUserAccessByOrgVM>(_container));
            modelBinderDictionary.Add(typeof(LUserAccessVM), new CustomModelBinder<LUserAccessVM>(_container));
            modelBinderDictionary.Add(typeof(LUserOrganizationVM), new CustomModelBinder<LUserOrganizationVM>(_container));
            modelBinderDictionary.Add(typeof(LUserOrgDetailsAccessVM), new CustomModelBinder<LUserOrgDetailsAccessVM>(_container));
            modelBinderDictionary.Add(typeof(LUserOrgDetailsVM), new CustomModelBinder<LUserOrgDetailsVM>(_container));
            modelBinderDictionary.Add(typeof(LUserPwdHistoryVM), new CustomModelBinder<LUserPwdHistoryVM>(_container));
            modelBinderDictionary.Add(typeof(LUserRoleVM), new CustomModelBinder<LUserRoleVM>(_container));
            modelBinderDictionary.Add(typeof(NormalUserVM), new CustomModelBinder<NormalUserVM>(_container));
            modelBinderDictionary.Add(typeof(SystemInfoVM), new CustomModelBinder<SystemInfoVM>(_container));
            //}
        }
    }
}
