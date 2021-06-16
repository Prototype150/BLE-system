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

namespace Diplom.BLEInteractions
{
    public sealed class BLEWatcher : BaseBLEWatcher
    {
        private List<DeviceInformation> devices;

        public override event ChangeHappened Added;
        public override event ChangeHappened Removed;

        private DeviceWatcher deviceWatcher;

        private string deviceName;
        /// <summary>
        /// Sets the standart watcher
        /// </summary>
        public BLEWatcher(string name)
        {
            deviceName = name;
            devices = new List<DeviceInformation>();
            deviceWatcher = DeviceInformation.CreateWatcher(BluetoothLEDevice.GetDeviceSelectorFromPairingState(false), new string[] { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" }, DeviceInformationKind.AssociationEndpoint);

            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;
        }
        /// <summary>
        /// Starts the watcher
        /// </summary>
        public override void StartWatcher()
        {
            deviceWatcher.Start();
        }

        /// <summary>
        /// Stops the watcher
        /// </summary>
        public override void StopWatcher()
        {
            deviceWatcher.Stop();
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            if (devices.Find(x => x.Id == args.Id) != null)
            {
                devices.Remove(devices.Find(x => x.Id == args.Id));
                Removed?.Invoke(args.Id);
            }
        }

        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            if(devices.Find(x => x.Id == args.Id) !=null)
                devices.Find(x => x.Id == args.Id).Update(args);
        }

        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            if (args.Name.Contains(deviceName))
            {
                devices.Add(args);
                Added?.Invoke(args.Id);
            }
        }

        public override IEnumerable<string> GetIdList()
        {
            return devices.Select(x =>x.Id);
        }
    }
}
