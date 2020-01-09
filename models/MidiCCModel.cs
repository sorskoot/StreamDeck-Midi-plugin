namespace StreamDeckMidiPlugin2.Models
{
    public class MidiCCModel : BaseMidiModel
    {
        public int Control { get; set; } = 0;
        public int Value { get; set; } = 0;
    }
}
