using System.Linq;
using RtMidi.Core;

namespace StreamDeckMidiPlugin2.Models
{
    public class MidiModel
    {
        public MidiModel()
        {
            this.Devices= MidiDeviceManager.Default.OutputDevices.Select(d => d.Name).ToArray();
        }

        public int Channel { get; set; } = 1;
        public int Note { get; set; } = 57;
        public int SelectedDevice { get; set; }
        public string[] Devices { get; private set; }
    }
}
