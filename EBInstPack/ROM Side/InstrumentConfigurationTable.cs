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

        public byte[] Header
        {
            get
            {
                //This is the part of the pack that's like "copy [size of the data] to [ARAM offset]"
                var aramOffset = HexHelpers.UInt16toByteArray(ARAM.instrumentConfigTableOffset_1A);

                var result = new List<byte>();
                result.AddRange(HexHelpers.UInt16toByteArray((ushort)DataDump.Length)); //Size
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
