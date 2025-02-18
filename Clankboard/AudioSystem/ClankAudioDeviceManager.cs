using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace Clankboard.AudioSystem
{
    public class ClankAudioDeviceManager
    {
        private List<MMDevice> availableOutputDevices = new List<MMDevice>();
        private List<MMDevice> availableInputDevices = new List<MMDevice>();

        public void UpdateOutputDevices() 
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();

            foreach (MMDevice device in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.All))
            {
                Debug.WriteLine("Found output device: " + device.FriendlyName + " || " + device.ID + " || " + device.State);
            }
        }
    }
}
