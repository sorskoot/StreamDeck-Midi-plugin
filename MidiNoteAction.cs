using StreamDeckLib;
using StreamDeckLib.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RtMidi.Core;
using RtMidi.Core.Devices;
using RtMidi.Core.Messages;
using System.Linq;
using RtMidi.Core.Devices.Infos;

namespace StreamDeckMidiPlugin2
{
    [ActionUuid(Uuid = "com.sorskoot.midi.action")]
    public class MidiNoteAction : BaseStreamDeckActionWithSettingsModel<Models.MidiModel>
    {
        public override async Task OnWillAppear(StreamDeckEventPayload args){
            await base.OnWillAppear(args);
            await Manager.SetSettingsAsync(args.context, SettingsModel);
        }

        public override async Task OnKeyUp(StreamDeckEventPayload args)
        {
            Midi(this.SettingsModel.SelectedDevice, this.SettingsModel.Channel, this.SettingsModel.Note);
            //update settings
            await Manager.SetSettingsAsync(args.context, SettingsModel);
        }

        public void Midi(int deviceIndex, int channel, int note)
        {
            IMidiOutputDeviceInfo inputDeviceInfo = MidiDeviceManager.Default.OutputDevices.ElementAt(deviceIndex);
            IMidiOutputDevice outputDevice = inputDeviceInfo.CreateDevice();

            RtMidi.Core.Enums.Channel midiChannel = (RtMidi.Core.Enums.Channel)channel - 1;
            RtMidi.Core.Enums.Key midiNote = (RtMidi.Core.Enums.Key)note;

            outputDevice.Open();
            outputDevice.Send(new NoteOnMessage(midiChannel, midiNote, 127));
            outputDevice.Close();
        }
    }
}
