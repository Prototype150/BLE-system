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
    public class Reader : IReader
    {
        public async Task<string> Read(int serviceId, int characteristicsId, BluetoothLEDevice device)
        {
            var servicesResult = await device.GetGattServicesAsync(BluetoothCacheMode.Uncached);
            var services = servicesResult.Services;
            try
            {
                var characteristicsResult = await services[serviceId].GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
                var characteristics = characteristicsResult.Characteristics;
                var result = await characteristics[characteristicsId].ReadValueAsync(BluetoothCacheMode.Uncached);
                var reader = DataReader.FromBuffer(result.Value);
                byte[] input = new byte[reader.UnconsumedBufferLength];
                reader.ReadBytes(input);
                string str = Encoding.ASCII.GetString(input);
                foreach (var item in services)
                {
                    item.Dispose();
                }
                return str;
            }
            catch (ArgumentOutOfRangeException)
            {
                foreach (var item in services)
                {
                    item.Dispose();
                }
                return null;
            }
        }
    }
}
