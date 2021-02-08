using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myProjectUser.common
{
    class common
    {
        public static Login machine = null;
        public static User manager = null;
        public static string stuid;
        public static Login getLoginForm()
        {
            if(machine == null)
            {
                machine = new myProjectUser.Login();
            }
            return machine;
        }
        public static User getUserForm()
        {
            if (manager == null)
            {
                manager = new myProjectUser.User();
            }
            return manager;
        }
    }
}
