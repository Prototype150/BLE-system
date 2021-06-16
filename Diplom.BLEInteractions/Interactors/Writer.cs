using Diplom.BLEInteractions.Interfaces.Interactors;
using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using GattCharacteristicProperties = Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties;

namespace Diplom.BLEInteractions.Interactors
{
    public class Writer : IWriter
    {
        public async Task<bool> Write(string message,int serviceId, int characteristicsId, BluetoothLEDevice device)
        {
                var writer = new DataWriter();
                writer.WriteString(message);
                var servicesResult = await device.GetGattServicesAsync(BluetoothCacheMode.Uncached);
                var services = servicesResult.Services;
            try { 
                var characteristicsResult = await services[serviceId].GetCharacteristicsAsync(BluetoothCacheMode.Uncached);

                await characteristicsResult.Characteristics[characteristicsId].WriteValueAsync(writer.DetachBuffer());
                foreach (var item in services)
                {
                    item.Dispose();
                }
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                foreach (var item in services)
                {
                    item.Dispose();
                }
                return false;
            }
        }
    }
}
