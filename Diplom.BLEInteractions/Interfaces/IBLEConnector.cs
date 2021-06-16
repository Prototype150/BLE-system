using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.BLEInteractions.Interfaces
{
    public interface IBLEConnector
    {
        int ConnectedDevices { get; }
        Task<bool> ConnectTo(string ID);
        bool DisconnectFrom(string ID);
        IEnumerator GetEnumerator();
        IDeviceInteractor GetDeviceInteractor(string ID);
        string GetName(string ID);
    }
}
