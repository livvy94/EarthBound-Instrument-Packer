using System.Collections.Generic;

namespace EBInstPack
{
    class BinaryBlob
    {
        internal static byte[] AssembleBin(BRRCluster cluster, InstrumentConfigurationTable metadata)
        {
            var END_OF_TRANSFER = new byte[] { 0x00, 0x00 };
            var pack = new List<byte>();
            pack.AddRange(cluster.PointerTable);
            pack.AddRange(cluster.DataDump);
            pack.AddRange(metadata.Header);
            pack.AddRange(metadata.DataDump);
            pack.AddRange(END_OF_TRANSFER);
            return pack.ToArray();
        }
    }
}
