using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;

namespace Diplom.BLEInteractions.Interfaces.Interactors
{
    public interface IReader
    {
        Task<string> Read(int serviceId, int characteristicsId, BluetoothLEDevice device);
    }
}
