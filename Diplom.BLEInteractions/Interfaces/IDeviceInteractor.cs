using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.BLEInteractions.Interfaces
{
    public interface IDeviceInteractor
    {
        Task<bool> Write(string message, int serviceId, int characteristicId);
        Task<string> Read(int serviceId, int characteristicId);
    }
}
