using System.Threading.Tasks;
using RtMidi.Core.Enums;
using RtMidi.Core.Messages;
using Serilog;
using StreamDeckLib;
using StreamDeckLib.Messages;
using StreamDeckMidiPlugin2.Models;

namespace StreamDeckMidiPlugin2
{
    [ActionUuid(Uuid = "com.sorskoot.midi.action")]
    public class MidiNoteAction : BaseMidiAction<MidiNoteModel>
    {

        public override Task OnKeyDown(StreamDeckEventPayload args)
        {
            Log.Information("OnKeyDown");
            this.MidiOn(this.SettingsModel.Channel, this.SettingsModel.Note);
            return Task.CompletedTask;
        }

        public override Task OnKeyUp(StreamDeckEventPayload args)
        {
            this.MidiOff(this.SettingsModel.Channel, this.SettingsModel.Note);
            return base.OnKeyUp(args);
        }

        public void MidiOn(int channel, int note)
        {
            var midiChannel = (Channel)(channel - 1);
            var midiNote = (Key)note;
            if (MidiDevice.OutputDevices[this.SettingsModel.SelectedDevice].IsOpen)
            {
                MidiDevice.OutputDevices[this.SettingsModel.SelectedDevice].Send(new NoteOnMessage(midiChannel, midiNote, 127));
            }
            else
            {
                Log.Warning("midi output device not connected");
            }
        }

        public void MidiOff(int channel, int note)
        {
            var midiChannel = (Channel)(channel - 1);
            var midiNote = (Key)note;
            if (MidiDevice.OutputDevices[this.SettingsModel.SelectedDevice].IsOpen)
            {
                MidiDevice.OutputDevices[this.SettingsModel.SelectedDevice].Send(new NoteOffMessage(midiChannel, midiNote, 127));
            }
            else
            {
                Log.Warning("midi output device not connected");
            }
        }
    }
}
