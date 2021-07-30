namespace EBInstPack
{
    class Patch
    {
        public byte index;
        public byte ADSR1;
        public byte ADSR2;
        public byte Gain;
        public byte Multiplier;
        public byte Sub;

        public string Filename = "";

        public byte[] MakeHex() => new byte[] { index, ADSR1, ADSR2, Gain, Multiplier, Sub };
    }
}
