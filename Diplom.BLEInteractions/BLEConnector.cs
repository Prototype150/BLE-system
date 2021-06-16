using Diplom.BLEInteractions.Interactors;
using Diplom.BLEInteractions.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;

namespace Diplom.BLEInteractions
{
    public sealed class BLEConnector:IBLEConnector, IEnumerable
    {
        private List<BluetoothLEDevice> connectedDevices;

        public BLEConnector()
        {
            connectedDevices = new List<BluetoothLEDevice>();
        }
        public int ConnectedDevices {
            get => connectedDevices.Count;
            private set { }
        }

        internal BluetoothLEDevice this[string ID]
        {
            get => connectedDevices.Find(x => x.DeviceId == ID) != null ? connectedDevices.Find(x => x.DeviceId == ID):null;
            private set { }
        }

        public async Task<bool> ConnectTo(string ID)
        {
            if (connectedDevices.Find(x => x.DeviceId == ID) == null)
            {
                connectedDevices.Add(await BluetoothLEDevice.FromIdAsync(ID));
                return this[ID] != null;
            }
            return false;
        }

        public bool DisconnectFrom(string ID)
        {
            var device = connectedDevices.Find(x => x.DeviceId == ID);
            if (device != null)
            {
                device.Dispose();
                connectedDevices.Remove(device);
                return true;
            }
            return false;
        }

        public IDeviceInteractor GetDeviceInteractor(string ID)
        {
            return this[ID] != null ? new DeviceInteractor(this[ID], new Reader(), new Writer()) : null;
        }

        public IEnumerator GetEnumerator()
        {
            return connectedDevices.Select(x => x.DeviceId).GetEnumerator();
        }

        public string GetName(string ID)
        {
            return connectedDevices.Find(x => x.DeviceId == ID).Name;
        }
    }
}
