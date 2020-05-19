using System;
using System.Collections.Generic;
using System.IO;

namespace EBInstPack
{
    class FileLoading
    {
        const string TEXT_FILE_NAME = "instruments.txt";

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
                    continue; //skip the text file, or anything else that might be in there

                var fileContents = File.ReadAllBytes(info.FullName);
                result.Add(new BRRFile
                {
                    data = BRRFunctions.IsolateBRRdata(fileContents),
                    loopPoint = BRRFunctions.IsolateLoopPoint(fileContents),
                    filename = info.Name
                });
            }
            return result;
        }

        internal static List<Instrument> LoadMetadata(string folderPath, List<BRRFile> samples)
        {
            //rip through the textfile
            //for each line, check the contents of the quotation marks against every BRR File using FindDuplicate
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
                    sample = BRRFunctions.FindDuplicate(samples, lineContents[0])
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
    }
}
