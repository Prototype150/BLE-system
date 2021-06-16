using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace Diplom.BLEInteractions.Interfaces
{
    public abstract class BaseBLEWatcher
    {
        public delegate void ChangeHappened(string ID);
        public abstract event ChangeHappened Removed;
        public abstract event ChangeHappened Added;
        public abstract void StartWatcher();
        public abstract void StopWatcher();
        public abstract IEnumerable<string> GetIdList();
    }
}
