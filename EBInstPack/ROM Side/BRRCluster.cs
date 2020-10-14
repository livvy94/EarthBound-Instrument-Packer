using System.Collections.Generic;
using System.Linq;

namespace EBInstPack
{
    class BRRCluster
    {
        readonly List<BRRFile> samples = new List<BRRFile>();
        readonly List<PointerTableEntry> entries = new List<PointerTableEntry>();

        public BRRCluster(List<BRRFile> samples)
        {
            this.samples = samples; //passed in from Program.cs

            //Generate the dump, keeping track of each sample's offsets all the while
            var dump = new List<byte>();
            var currentOffset = 0x95B0; //the end of Pack 05, where instrument 1A should be
            foreach (var sample in samples)
            {
                dump.AddRange(sample.data);

                entries.Add(new PointerTableEntry
                {
                    offset = currentOffset,
                    loopPoint = currentOffset + (sample.loopPoint != 0 ? BRRFunctions.EncodeLoopPoint(sample.loopPoint) : sample.data.Count)
                });

                currentOffset += sample.data.Count;
            }
        }

        public byte[] DataDump
        {
            get
            {
                var aramOffset = new byte[] { 0xB0, 0x95 };

                var dump = samples.SelectMany(s => s.data).ToList();

                var result = new List<byte>();
                result.AddRange(HexHelpers.IntToByteArray_Length2(dump.Count));
                result.AddRange(aramOffset);
                result.AddRange(dump);
                return result.ToArray();
            }
        }

        public byte[] PointerTable
        {
            //Example:
            //18 00   (A count of how many hex numbers the pointers make up - two bytes swapped)
            //68 6C   (The ARAM offset to put this data, also swapped)
            //B0 95 82 A5 B0 95 82 A5 B0 95 82 A5 7A A7 62 AD B6 B3 3E B6 59 B6 BE BA  (The pointers themselves - there are 0x18 hex numbers here, pointing to 6 samples)

            get
            {
                var aramOffset = new byte[] { 0x68, 0x6C }; //ARAM offset 6C68

                var pointers = new List<byte>();
                foreach (var entry in entries)
                {
                    pointers.AddRange(entry.Data);
                }

                var result = new List<byte>();
                result.AddRange(HexHelpers.IntToByteArray_Length2(pointers.Count));
                result.AddRange(aramOffset);
                result.AddRange(pointers);
                return result.ToArray();
            }
        }

        class PointerTableEntry
        {
            public int offset;
            public int loopPoint;
            public byte[] Data => HexHelpers.IntsToByteArray_Length2(new List<int> { offset, loopPoint });
        }
    }
}
