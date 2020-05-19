using System.Collections.Generic;

namespace EBInstPack
{
    class BRRCluster
    {
        readonly List<BRRFile> samples = new List<BRRFile>(); //passed in from Program.cs
        readonly List<Instrument> instruments = new List<Instrument>(); //also passed in

        public BRRCluster(List<BRRFile> samples, List<Instrument> instruments)
        {
            this.samples = samples;
            this.instruments = instruments;
        }

        public byte[] Data
        {
            get
            {
                var result = new List<byte>();
                foreach (var sample in samples)
                {
                    foreach (var b in sample.data)
                    {
                        result.Add(b); //Combine all BRR files into one big blob. There's GOTTA be a better way to do this
                    }
                }
                return result.ToArray();
            }
        }
        public byte[] PointerTable
        {
            //Example:
            //18 00   (A count of how many hex numbers the pointers make up. Two bytes swapped)
            //68 6C   (The ARAM offset to put this data)
            //B0 95 82 A5 B0 95 82 A5 B0 95 82 A5 7A A7 62 AD B6 B3 3E B6 59 B6 BE BA  (The pointers themselves)

            get
            {
                var result = new List<byte>();

                //TODO: Build the list of pointers. These are the offsets of each file inside Data
                //PROBLEM:
                //In the case of duplicate samples (there's a one to many relationship
                //between BRRFile and instrument!), how to tell to write a duplicate pointer?

                //SOLUTION:
                //Make list of ints and a list of samples
                //put offsets in the list of ints. Do it with some kind of calculation based on size of each BRRFile.data
                //Don't forget to add the loop point to the pointer. good lord


                result.Add(0x68); //The SPC700 ARAM offset this data should get dumped into, 6C68
                result.Add(0x6C);

                return result.ToArray();
            }
        }
    }
}
