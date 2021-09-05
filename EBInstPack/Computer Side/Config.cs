using System;
using System.Collections.Generic;
using System.Linq;

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
            outputFilename = filename.Replace(" ", "").Replace("-", ""); //CCScript doesn't like spaces and dashes
            packNumber = packnum;
            baseInstrument = baseinst;
            offsetForBRRdump = brrOffset;
            patches = loadedPatches;

            //set the other stuff based on baseInstrument
            offsetForSampleDir = (ushort)(0x6C00 + (baseinst * 4));
            offsetForInstrumentConfig = (ushort)(0x6E00 + (baseinst * 6));
        }

        public static byte[] GenerateSampleDirectory(Config config, List<BRRFile> samples)
        {
            var currentAramOffset = config.offsetForBRRdump;
            //Thanks to BlueStone for helping with the following!
            var samplesByFilename = samples.ToDictionary(sample => sample.filename);
            var sampleDirectoriesByFilename = new Dictionary<string, SampleDirectory>();
            var resultSampleDirectory = new List<SampleDirectory>();
            foreach (var patch in config.patches)
            {
                // If an entry with the same filename was already added, reuse the existing entry.
                if (sampleDirectoriesByFilename.TryGetValue(patch.Filename, out SampleDirectory entry))
                    resultSampleDirectory.Add(entry);
                else
                {
                    // We haven't added an entry with this file before, so find the sample and add the entry.
                    if (!samplesByFilename.TryGetValue(patch.Filename, out BRRFile currentSample))
                    {
                        if (!patch.Filename.Contains("0x")) //don't display this message for duplicates
                            Console.WriteLine("Sample file \"" + patch.Filename + "\" does not exist!");
                    }
                    else
                    {
                        ushort tempStartOffset = 0; //Duplicates need to use the explicitly-specified offsets
                        ushort tempLoopOffset = 0;  //instead of the running count that BRR files use
                        if (currentSample.filename.Contains("Duplicate"))
                        {
                            tempStartOffset = currentSample.dupeStartOffset;
                            tempLoopOffset = currentSample.dupeLoopOffset;
                        }
                        else
                        {
                            tempStartOffset = currentAramOffset;
                            if (currentSample.loopPoint == 0)
                            {
                                tempLoopOffset = (ushort)(currentAramOffset + currentSample.data.Length); //loopPoint is 0, so unlooped samples would otherwise loop infinitely
                            }
                            else
                            {
                                tempLoopOffset = (ushort)(currentAramOffset + BRRFunctions.EncodeLoopPoint(currentSample.loopPoint));
                            }
                        }

                        var sampleDirEntry = new SampleDirectory
                        {
                            aramOffset = tempStartOffset,
                            loopOffset = tempLoopOffset,
                            Filename = currentSample.filename,
                        };
                        sampleDirectoriesByFilename[patch.Filename] = sampleDirEntry;
                        resultSampleDirectory.Add(sampleDirEntry);

                        currentAramOffset += (ushort)currentSample.data.Length;
                    }
                }
            }

            //turn the entire thing into a byte array
            var result = new List<byte>();
            foreach (var entry in resultSampleDirectory)
            {
                result.AddRange(entry.MakeHex());
            }
            return result.ToArray();
        }

        public byte[] GetPatches()
        {
            var result = new List<byte>();
            foreach (var patch in patches)
            {
                result.AddRange(patch.MakeHex());
            }
            return result.ToArray();
        }
    }
}
