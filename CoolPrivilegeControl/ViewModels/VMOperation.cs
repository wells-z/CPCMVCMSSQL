using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoolPrivilegeControl.ViewModels
{
    public enum OperationMode
    {
        Search,
        Deatils,
        Create,
        Edit,
        Delete
    }

    public class VMOperation
    {
        public OperationMode OperMode
        {
            get;
            set;
        }

        public string ActionName
        {
            get;
            set;
        }

        public string ActionController
        {
            get;
            set;
        }
    }
}