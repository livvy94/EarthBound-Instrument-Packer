using System.Collections.Generic;

namespace EBInstPack
{
    class Config
    {
        //config.txt contents
        public byte packNumber;
        public byte baseInstrument;
        public List<Patch> patches;

        //internal stuff
        public ushort offsetForSampleDir;
        public ushort offsetForBRRdump;
        public ushort offsetForInstrumentConfig;
        public string outputFilename;
        public byte maxDelay;

        public Config()
        {
            packNumber = 0xFF;
            baseInstrument = 0xFF;
            offsetForBRRdump = 0xFFF;
            patches = new List<Patch>();
            offsetForSampleDir = 0xFFFF;
            offsetForInstrumentConfig = 0xFFFF;
            outputFilename = "output.txt";
            maxDelay = 0xFF;
        }

        public Config(string filename, byte packnum, byte baseinst, ushort brrOffset, List<Patch> loadedPatches)
        {
            outputFilename = filename;
            packNumber = packnum;
            baseInstrument = baseinst;
            offsetForBRRdump = brrOffset;
            patches = loadedPatches;

            //set the other stuff based on baseInstrument
            offsetForSampleDir = (ushort)(0x6C00 + (baseinst * 4));
            offsetForInstrumentConfig = (ushort)(0x6E00 + (baseinst * 6));
        }

        public byte[] GetPatches()
        {
            var result = new List<byte>();
            foreach (var patch in patches)
            {
                //Console.WriteLine($"Adding patch for {patch.Filename}...");
                result.AddRange(patch.MakeHex());
            }
            return result.ToArray();
        }
    }
}
