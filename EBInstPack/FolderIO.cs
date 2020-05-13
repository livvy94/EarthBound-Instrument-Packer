using System;
using System.Collections.Generic;
using System.IO;

namespace EBInstPack
{
    class FolderIO
    {
        const string TEXT_FILE_NAME = "METADATA.txt";

        internal static bool FolderNonexistant(string folderPath)
        {
            return !File.Exists(GetFullTextfilePath(folderPath));
        }

        internal static string GetFullTextfilePath(string folderPath)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), folderPath, TEXT_FILE_NAME);
        }
        internal static string GetFullPath(string folderPath)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), folderPath);
        }

        internal static List<BRRFile> LoadBRRs(string folderPath)
        {
            var result = new List<BRRFile>();

            foreach(string filename in Directory.GetFiles(GetFullPath(folderPath)))
            {
                var info = new FileInfo(filename);
                if (info.Extension != "brr")
                    continue; //skip anything that isn't a BRR file

                var fileContents = File.ReadAllBytes(info.FullName);
                result.Add(new BRRFile
                {
                    data = GetBRRData(fileContents), //not done yet, see below
                    loopPoint = GetBRRLoopPoint(fileContents), //not done yet, see below
                    filename = info.Name
                });
            }
            return result;
        }

        internal static List<byte> GetBRRData(byte[] data)
        {
            var result = new List<byte>();

            for (int i = 0; i < data.Length; i++)
            {
                if (i < 2) continue; //skip the loop point and return everything else
                result.Add(data[i]); //Something tells me this could be done in a cleaner way...
            }
            return result;
        }

        internal static int GetBRRLoopPoint(byte[] data)
        {
            //TODO: Move stuff like this to the BRRFile class (how would that work, though?)

            //Special thanks to Milon in the SMW Central discord for this explanation.
            //A BRR loop point is stored by taking the raw value, dividing it by 16, and multiplying it by 9.

            //For example:
            //In C700, Chrono Trigger's Choir sample shows up with a loop point of 2288.
            //2288 / 16 = 143
            //143 * 9 = 1287
            //1287 in hex is [05 07], which is then swapped as [07 05].
            //(Which is what you see if you open that file in a hex editor!)

            //So to do the inverse, we need to take the first two bytes of a file,
            //reverse them, divide the number by 9, and multiply it by 16.

            var amkLoopHeader = new byte[] { data[0], data[1] }; //Doesn't need to be swapped?? Maybe BitConverter assumes little-endian.
            return (BitConverter.ToInt16(amkLoopHeader) / 9) * 16;
        }

        internal static List<Instrument> LoadMetadata(string folderPath, List<BRRFile> samples)
        {
            //rip through the textfile
            //for each line, check the contents of the quotation marks against every BRR File
            //using FindMatchingBRR(samples, aNameFromTheTextFile)
            var lines = File.ReadLines(GetFullTextfilePath(folderPath));

            var result = new List<Instrument>();
            byte initialIndex = 0x1A; //This should be 1A when paired with Pack 05, which is essentially used all throughout the game
            byte instIndex = initialIndex;
            foreach (var line in lines)
            {
                if (LineShouldBeSkipped(line)) continue;
                var lineContents = CleanTextFileLine(line);

                var temp = new Instrument
                {
                    index = instIndex,
                    ADSR1 = Convert.ToByte(lineContents[1]),
                    ADSR2 = Convert.ToByte(lineContents[2]),
                    Gain = Convert.ToByte(lineContents[3]),
                    Multiplier = Convert.ToByte(lineContents[4]),
                    Sub = Convert.ToByte(lineContents[5]),
                    sample = FindMatchingBRR(samples, lineContents[0])
                };

                result.Add(temp);
                instIndex++;
            }

            return result;
        }

        internal static bool LineShouldBeSkipped(string line)
        {
            var skippableStrings = new string[] { "{", "}", "#instruments", "#samples", string.Empty };
            var result = false;

            foreach (var annoyance in skippableStrings)
            {
                if (line.Contains(annoyance))
                    result = true;
            }

            return result;
        }

        internal static List<string> CleanTextFileLine(string line)
        {
            var result = new List<string>();
            var splitLine = line.Split(' ');
            foreach (var linePiece in splitLine)
            {
                if (string.IsNullOrEmpty(linePiece)) continue;
                var cleanPiece = linePiece.Trim(new char[] { ' ', '"', '$' }); //Trim the " and $ chars from each line
                result.Add(cleanPiece);
            }

            return result;
        }

        internal static BRRFile FindMatchingBRR(List<BRRFile> samples, string name)
        {
            foreach(var brr in samples)
            {
                if (brr.filename == name)
                    return brr;
            }

            return null; //If there's nothing there with that name, what do?
        }
    }
}
