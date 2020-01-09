using System.Linq;
using RtMidi.Core;

namespace StreamDeckMidiPlugin2.Models
{
    public class BaseMidiModel
    {
        public BaseMidiModel()
        {
            this.Devices = MidiDeviceManager.Default.OutputDevices.Select(d => d.Name).ToArray();
        }

        public int SelectedDevice { get; set; }
        public string[] Devices { get; set; }
        
        public int Channel { get; set; } = 1;
    }
}
