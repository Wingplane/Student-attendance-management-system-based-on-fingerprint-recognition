using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myProject.common
{
    class common
    {
        public static Machine machine = null;
        public static Manager manager = null;
        public static string manageid;
        public static Machine getMachineForm()
        {
            if(machine == null)
            {
                machine = new myProject.Machine();
            }
            return machine;
        }
        public static Manager getManagerForm()
        {
            if (manager == null)
            {
                manager = new myProject.Manager();
            }
            return manager;
        }
    }
}
