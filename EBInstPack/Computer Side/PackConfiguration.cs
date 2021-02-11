namespace EBInstPack
{
    class PackConfiguration
    {
        //loaded from config.txt
        public byte packNumber;
        public ushort sampleDirectoryOffset;
        public ushort brrDumpOffset;
        public ushort instrumentConfigOffset;

        public void SetBaseInstrument(byte baseInstrument)
        {
            sampleDirectoryOffset = (ushort)(0x6C00 + (baseInstrument * 4));
            instrumentConfigOffset = (ushort)(0x6E00 + (baseInstrument * 6));
        }
    }


}
