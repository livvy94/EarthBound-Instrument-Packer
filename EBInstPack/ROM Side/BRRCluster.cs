using System.Collections.Generic;

namespace EBInstPack
{
    class PointerTableEntry
    {
        public int offset;
        public int loopPoint;
        public byte[] howIsThisEvenFunctional
        {
            get { return HexHelpers.IntToByteArray(offset + loopPoint); } //this needs to be swapped, in case it's not doing that
        }
    }

    class BRRCluster
    {
        readonly List<BRRFile> samples = new List<BRRFile>(); //passed in from Program.cs
        List<PointerTableEntry> entries = new List<PointerTableEntry>();

        public BRRCluster(List<BRRFile> samples)
        {
            this.samples = samples;
        }

        public byte[] Data
        {
            get
            {
                var currentOffset = 0;
                var result = new List<byte>();
                foreach (var sample in samples)
                {
                    result.AddRange(sample.data);

                    entries.Add(new PointerTableEntry
                    {
                        offset = currentOffset,
                        loopPoint = sample.loopPoint,
                    });

                    currentOffset += sample.data.Count;
                }
                return result.ToArray();
            }
        }
        public byte[] PointerTable
        {
            //Example:
            //18 00   (A count of how many hex numbers the pointers make up. Two bytes swapped)
            //68 6C   (The ARAM offset to put this data)
            //B0 95 82 A5 B0 95 82 A5 B0 95 82 A5 7A A7 62 AD B6 B3 3E B6 59 B6 BE BA  (The pointers themselves. There are 0x18 numbers, count 'em!)

            get //Build the list of pointers. These are the offsets of each file inside Data
            {
                var result = new List<byte>();

                var pointerCount = HexHelpers.IntToByteArray(entries.Count * 4); //assuming each pointer is 4 hex numbers long. Ask pinci if this is correct
                byte[] aramOffset = { 0x68, 0x6C }; //ARAM offset 6C68

                var pointers = new List<byte>();
                foreach (var entry in entries)
                {
                    pointers.AddRange(entry.howIsThisEvenFunctional);
                }

                result.AddRange(pointerCount);
                result.AddRange(aramOffset);
                result.AddRange(pointers);
                return result.ToArray();
            }
        }
    }
}
