using System.Collections.Generic;

namespace EBInstPack
{
    class SampleDirectory
    {
        public ushort aramOffset;
        public ushort loopOffset;

        public string Filename;

        public byte[] MakeHex()
        {
            var result = new List<byte>();
            result.AddRange(HexHelpers.UInt16toByteArray_LittleEndian(aramOffset));
            result.AddRange(HexHelpers.UInt16toByteArray_LittleEndian(loopOffset));
            return result.ToArray();
        }
    }
}
