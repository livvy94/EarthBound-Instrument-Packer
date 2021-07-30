using System;
using System.Collections.Generic;
using System.Linq;

namespace EBInstPack
{
    class BinaryBlob
    {
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
                        // Create and add a new entry.
                        var sampleDirectory = new SampleDirectory
                        {
                            aramOffset = currentAramOffset,
                            loopOffset = (ushort)(currentAramOffset + BRRFunctions.EncodeLoopPoint(currentSample.loopPoint)),
                            Filename = currentSample.filename,
                        };

                        sampleDirectoriesByFilename[patch.Filename] = sampleDirectory;
                        resultSampleDirectory.Add(sampleDirectory);

                        currentAramOffset += (ushort)currentSample.data.Count;
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

        public static byte[] GenerateBRRDump(List<BRRFile> samples)
        {
            var result = new List<byte>();

            foreach (var sample in samples)
            {
                if (sample.filename == "(Duplicate)")
                {
                    Console.WriteLine("(Duplicate Entry)".PadRight(40, '.') + "0 bytes".PadLeft(4, '.'));
                    continue;
                }

                Console.WriteLine($"Loading {sample.filename}".PadRight(40, '.') + sample.data.Count.ToString().PadLeft(4, '.') + " bytes");
                result.AddRange(sample.data);
            }
            return result.ToArray();
        }
    }
}
