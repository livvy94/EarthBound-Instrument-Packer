using System.Collections.Generic;

namespace EBInstPack
{
    class BRRCluster
    {
        readonly List<BRRFile> samples = new List<BRRFile>();
        readonly List<PointerTableEntry> entries = new List<PointerTableEntry>();

        public BRRCluster(List<BRRFile> samples)
        {
            this.samples = samples; //passed in from Program.cs
        }

        public byte[] DataDump
        {
            get
            {
                var aramOffset = new byte[] { 0xB0, 0x95 };

                //Generate the dump, keeping track of each sample's offsets all the while
                var dump = new List<byte>();
                var currentOffset = 0;
                foreach (var sample in samples)
                {
                    dump.AddRange(sample.data);

                    entries.Add(new PointerTableEntry
                    {
                        offset = currentOffset, //Is this okay or do I need to add something to it?
                        loopPoint = sample.loopPoint,
                    });

                    currentOffset += sample.data.Count;
                }

                var result = new List<byte>();
                result.AddRange(aramOffset);
                result.AddRange(HexHelpers.IntToByteArray(dump.Count));
                result.AddRange(dump);
                return result.ToArray();
            }
        }

        public byte[] PointerTable
        {
            //Example:
            //18 00   (A count of how many hex numbers the pointers make up - two bytes swapped)
            //68 6C   (The ARAM offset to put this data, also swapped)
            //B0 95 82 A5 B0 95 82 A5 B0 95 82 A5 7A A7 62 AD B6 B3 3E B6 59 B6 BE BA  (The pointers themselves - there are 0x18 hex numbers here)

            //This is one of the packs from the vanilla game. Notice how none of these are 00 00? Hmmm... Not sure if I should worry or not

            get
            {
                byte[] aramOffset = { 0x68, 0x6C }; //ARAM offset 6C68

                var pointers = new List<byte>();
                foreach (var entry in entries)
                {
                    //Each pointer is two hex numbers long, right?
                    //Ask pinci if the above example specifies 12 instruments or 6 instruments
                    pointers.AddRange(entry.howIsThisEvenFunctional);
                }

                var result = new List<byte>();
                result.AddRange(HexHelpers.IntToByteArray(pointers.Count));
                result.AddRange(aramOffset);
                result.AddRange(pointers);
                return result.ToArray();
            }
        }
        class PointerTableEntry
        {
            public int offset;
            public int loopPoint;
            public byte[] howIsThisEvenFunctional
            {
                get { return HexHelpers.IntToByteArray(offset + loopPoint); } //The bytes need to be swapped. Double check
            }
        }

    }
}
