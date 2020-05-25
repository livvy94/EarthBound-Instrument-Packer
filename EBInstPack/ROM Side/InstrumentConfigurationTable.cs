using System.Collections.Generic;

namespace EBInstPack
{
    class InstrumentConfigurationTable
    {
        private readonly List<Instrument> instruments;

        public InstrumentConfigurationTable(List<Instrument> instruments)
        {
            this.instruments = instruments; //passed in from Program.cs
        }

        readonly byte[] aramOffset = { 0xFF, 0xFF }; //ARAM offset ???? - check DMs with pinci and see if he mentioned it

        public byte[] Header
        {
            get
            {
                var result = new List<byte>();
                result.AddRange(HexHelpers.IntToByteArray_Length2(DataDump.Length)); //Size
                result.AddRange(aramOffset); //Offset
                return result.ToArray();
            }
        }

        public byte[] DataDump
        {
            get
            {
                var result = new List<byte>();

                foreach (var inst in instruments)
                {
                    result.AddRange(inst.ConfigTableEntry);
                }

                return result.ToArray();
            }
        }
    }
}
