using System.Collections.Generic;
using System.Linq;

namespace EBInstPack
{
    class BRRCluster
    {
        readonly List<BRRFile> samples = new List<BRRFile>();
        readonly List<PointerTableEntry> entries = new List<PointerTableEntry>();
        private ushort samplesOffset;
        private ushort sampleDirectoryOffset;

        public BRRCluster(List<BRRFile> samples, ushort sampleDirectoryOffset, ushort samplesOffset)
        {
            this.samples = samples; //passed in from Program.cs
            this.samplesOffset = samplesOffset;
            this.sampleDirectoryOffset = sampleDirectoryOffset;

            //Generate the dump, keeping track of each sample's offsets all the while
            var dump = new List<byte>();
            var currentOffset = (int)samplesOffset; //start at the end of Pack 05, where instrument 1A should be (unless a different offset is passed in)
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

        public byte[] SampleData
        {
            get
            {
                var dump = samples.SelectMany(s => s.data).ToList(); //all the BRR data, squished together into a blob

                var result = new List<byte>();
                result.AddRange(HexHelpers.UInt16toByteArray_LittleEndian((ushort)dump.Count)); //take the next X bytes
                result.AddRange(HexHelpers.UInt16toByteArray_LittleEndian(samplesOffset)); //and load them into X offset
                result.AddRange(dump); //the BRR files
                return result.ToArray();
            }
        }

        public byte[] SampleDirectory
        {
            //Example:
            //18 00   (Take the next 0x18 bytes)
            //68 6C   (And load them into ARAM offset 6C68)
            //B0 95 82 A5 B0 95 82 A5 B0 95 82 A5 7A A7 62 AD B6 B3 3E B6 59 B6 BE BA  (The pointers themselves - there are 0x18 hex numbers here, pointing to 6 samples)

            get
            {

                var pointers = new List<byte>();
                foreach (var entry in entries)
                {
                    pointers.AddRange(entry.Data);
                }

                var result = new List<byte>();
                result.AddRange(HexHelpers.UInt16toByteArray_LittleEndian((ushort)pointers.Count)); //take the next X bytes
                result.AddRange(HexHelpers.UInt16toByteArray_LittleEndian(sampleDirectoryOffset)); //and load them into X offset
                result.AddRange(pointers); //the sample directory
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
