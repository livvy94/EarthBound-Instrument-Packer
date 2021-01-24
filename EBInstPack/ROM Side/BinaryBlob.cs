using System.Collections.Generic;

namespace EBInstPack
{
    class BinaryBlob
    {
        //TODO: make it so it isn't an assembled binary blob and everything is separated...
        internal static byte[] AssembleBin(BRRCluster cluster, InstrumentConfigurationTable metadata)
        {
            var END_OF_TRANSFER = new byte[] { 0x00, 0x00 };
            var pack = new List<byte>();
            pack.AddRange(cluster.SampleDirectory);
            pack.AddRange(cluster.SampleData);
            pack.AddRange(metadata.Header);
            pack.AddRange(metadata.DataDump);
            pack.AddRange(END_OF_TRANSFER);
            return pack.ToArray();
        }
    }
}
