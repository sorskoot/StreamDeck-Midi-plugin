using System.Collections.Generic;
using RtMidi.Core.Devices;

namespace StreamDeckMidiPlugin2
{
    static class MidiDevice
    {
        public static Dictionary<int, IMidiOutputDevice> OutputDevices = new Dictionary<int, IMidiOutputDevice>();
    }
}
