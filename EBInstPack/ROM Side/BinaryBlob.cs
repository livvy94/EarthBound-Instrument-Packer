using System.Collections.Generic;

namespace EBInstPack
{
    class BinaryBlob
    {
        internal static byte[] AssembleBin(BRRCluster cluster, InstrumentConfigurationTable metadata)
        {
            var pack = new List<byte>();
            pack.AddRange(cluster.PointerTable);
            pack.AddRange(cluster.DataDump);
            pack.AddRange(metadata.Header);
            pack.AddRange(metadata.DataDump);
            return pack.ToArray();
        }
    }
}
