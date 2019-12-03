using System.Linq;
using RtMidi.Core;

namespace StreamDeckMidiPlugin2.Models
{
    public class MidiCCModel
    {
        public MidiCCModel()
        {
            this.Devices = MidiDeviceManager.Default.OutputDevices.Select(d => d.Name).ToArray();
        }

        public int Channel { get; set; } = 1;
        public int Control { get; set; } = 0;
        public int Value { get; set; } = 0;
        public int SelectedDevice { get; set; }
        public string[] Devices { get; private set; }
    }
}
