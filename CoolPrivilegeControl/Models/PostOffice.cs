using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UI_Infrastructure;

namespace CoolPrivilegeControl.Models
{
    public class PostOffice : IPostOffice
    {
        public string LoginUserMgtSerPath
        {
            get;
            set;
        }

        public string AuditLogMgtSerPath
        {
            get;
            set;
        }

        public string AuthHisMgtSerPath
        {
            get;
            set;
        }

        public string FunTypeMgtSerPath
        {
            get;
            set;
        }

        public string FunMgtSerPath
        {
            get;
            set;
        }

        public string OrgMgtSerPath
        {
            get;
            set;
        }

        public string OrgDetailMgtSerPath
        {
            get;
            set;
        }

        public string RoleMgtSerPath
        {
            get;
            set;
        }

        public string SystemMgtSerPath
        {
            get;
            set;
        }

        public string CheckPrivilegeSerPath
        {
            get;
            set;
        }
        public PostOffice()
        {
            LoginUserMgtSerPath = "BasicHttpBinding_ILoginUserMgtSer";
            AuditLogMgtSerPath = "BasicHttpBinding_IAuditLogMgtSer";
            AuthHisMgtSerPath = "BasicHttpBinding_IAuthHisMgtSer";

            FunTypeMgtSerPath = "BasicHttpBinding_IFunTypeMgtSer";
            FunMgtSerPath = "BasicHttpBinding_IFunMgtSer";
            OrgMgtSerPath = "BasicHttpBinding_IOrgMgtSer";
            OrgDetailMgtSerPath = "BasicHttpBinding_IOrgDetailMgtSer";
            RoleMgtSerPath = "BasicHttpBinding_IRoleMgtSer";
            SystemMgtSerPath = "BasicHttpBinding_ISystemMgtSer";

            CheckPrivilegeSerPath = "BasicHttpBinding_ICheckPrivilegeSer";
        }

    }
}