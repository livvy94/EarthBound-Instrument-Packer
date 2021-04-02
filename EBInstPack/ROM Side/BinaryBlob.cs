using System;
using System.Collections.Generic;

namespace EBInstPack
{
    class BinaryBlob
    {
        public static byte[] GenerateSampleDirectory(Config config, List<BRRFile> samples)
        {
            var currentAramOffset = config.offsetForBRRdump;

            var resultSampleDirectory = new List<SampleDirectory>();
            foreach (var patch in config.patches)
            {
                bool alreadyExisted = false; //This is initialized at the start of each iteration, so no need to set it back
                foreach (var entry in resultSampleDirectory)
                {
                    if (entry.Filename == patch.Filename) //check for duplicates! one sample --> many patches
                    {
                        alreadyExisted = true;
                        //Console.WriteLine($"Adding a duplicate for {patch.Filename} to the sample directory!");
                        resultSampleDirectory.Add(entry); //add an identical copy of the current entry
                    }
                }

                if (alreadyExisted) continue;

                var currentSample = BRRFunctions.Find(patch.Filename, samples);
                //Console.WriteLine($"Adding {patch.Filename} to the sample directory!");
                resultSampleDirectory.Add(new SampleDirectory //add a new entry
                {
                    aramOffset = currentAramOffset,
                    loopOffset = (ushort)(currentAramOffset + currentSample.loopPoint),
                    Filename = currentSample.filename,
                });

                currentAramOffset += (ushort)currentSample.data.Count;
            }

            //turn the entire thing into a byte array
            var result = new List<byte>();
            foreach (var entry in resultSampleDirectory)
            {
                result.AddRange(entry.MakeHex());
            }
            return result.ToArray();
        }

        public static byte[] GenerateBRRDump(List<BRRFile> samples)
        {
            var result = new List<byte>();

            foreach (var sample in samples)
            {
                Console.WriteLine($"Loading {sample.filename}".PadRight(40, '.') + sample.data.Count.ToString().PadLeft(4, '.') + " bytes");
                result.AddRange(sample.data);
            }
            return result.ToArray();
        }
    }
}
