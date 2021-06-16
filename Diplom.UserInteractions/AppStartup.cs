using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Diplom.BLEInteractions;

namespace Diplom.UserInteractions
{
    public partial class AppStartup:Application
    {
        private Window wnd;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            wnd = new MainWindow(new BLEWatcher("BLE"), new BLEConnector());
            // The OpenFile() method is just an example of what you could do with the
            // parameter. The method should be declared on your MainWindow class, where
            // you could use a range of methods to process the passed file path
            wnd.Show();
        }
    }
}
