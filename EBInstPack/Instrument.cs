namespace EBInstPack
{
    class Instrument
    {
        public byte index;
        public byte ADSR1;
        public byte ADSR2;
        public byte Gain;
        public byte Multiplier;
        public byte Sub;

        public BRRFile sample = new BRRFile();
    }
}
