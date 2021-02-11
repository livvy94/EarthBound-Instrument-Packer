using System.Collections.Generic;

namespace EBInstPack
{
    class InstrumentConfigurationTable
    {
        private readonly List<Instrument> instruments;
        private ushort instrumentConfigTableOffset;

        public InstrumentConfigurationTable(List<Instrument> instruments, ushort offset)
        {
            this.instruments = instruments; //passed in from Program.cs
            this.instrumentConfigTableOffset = offset;
        }

        public byte[] Header
        {
            get
            {
                var result = new List<byte>();
                result.AddRange(HexHelpers.UInt16toByteArray_LittleEndian((ushort)DataDump.Length)); //take the next X bytes
                result.AddRange(HexHelpers.UInt16toByteArray_LittleEndian(instrumentConfigTableOffset)); //and load them into X offset
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
