using System.Threading.Tasks;
using RtMidi.Core.Enums;
using RtMidi.Core.Messages;
using Serilog;
using StreamDeckLib;
using StreamDeckLib.Messages;
using StreamDeckMidiPlugin2.Models;

namespace StreamDeckMidiPlugin2
{
    [ActionUuid(Uuid = "com.sorskoot.midicc.action")]
    public class MidiCCAction : BaseMidiAction<MidiCCModel>
    {

        public override Task OnKeyDown(StreamDeckEventPayload args)
        {
            Log.Information("OnKeyDown");
            this.MidiCC(this.SettingsModel.Channel, this.SettingsModel.Control, this.SettingsModel.Value);
            return Task.CompletedTask;
        }

        public void MidiCC(int channel, int control, int value)
        {
            var midiChannel = (Channel)(channel - 1);
            if (MidiDevice.OutputDevices[this.SettingsModel.SelectedDevice].IsOpen)
            {
                MidiDevice.OutputDevices[this.SettingsModel.SelectedDevice].Send(new ControlChangeMessage(midiChannel, control, value));
            }
            else
            {
                Log.Warning("midi output device not connected");
            }
        }
    }
}
