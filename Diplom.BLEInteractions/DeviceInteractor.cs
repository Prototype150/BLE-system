using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Storage.Streams;
using System.Linq;
using System.Collections;
using Diplom.BLEInteractions.Interfaces;
using Diplom.BLEInteractions.Interfaces.Interactors;

namespace Diplom.BLEInteractions
{
    public class DeviceInteractor:IDeviceInteractor
    {
        IReader reader;
        IWriter writer;
        private readonly BluetoothLEDevice device;
        public DeviceInteractor(BluetoothLEDevice device, IReader reader, IWriter writer)
        {
            this.device = device;
            this.reader = reader;
            this.writer = writer;
        }

        public async Task<bool> Write(string message,int serviceId, int characteristicId)
        {
            bool r = await writer.Write(message, serviceId, characteristicId, device);
            return r;
        }

        public async Task<string> Read(int serviceId, int characteristicId)
        {
            string result = await reader.Read(serviceId, characteristicId, device);
            return result;
        }
    }
}
