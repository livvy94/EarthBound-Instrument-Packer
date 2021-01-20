using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;

namespace EBInstPack
{
    class FileIO
    {
        const string TEXT_FILE_NAME = "instruments.txt";
        const string OUTPUT_FILENAME = "pack.bin";

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

        internal static string GetFolderNameFromPath(string folderPath)
        {
            return new DirectoryInfo(folderPath).Name;
        }

        internal static List<BRRFile> LoadBRRs(string folderPath)
        {
            var filenames = Directory.GetFiles(GetFullPath(folderPath));
            Array.Sort(filenames); //this will let the order of insertion be specified by renaming the files to "1 trumpet.brr", "2 strings.brr", etc.

            var result = new List<BRRFile>();
            foreach (string filename in filenames)
            {
                var info = new FileInfo(filename);
                if (info.Extension != ".brr")
                    continue; //skip the text file, or anything else that might be in there

                var fileContents = File.ReadAllBytes(info.FullName);
                result.Add(new BRRFile
                {
                    data = BRRFunctions.IsolateBRRdata(fileContents),
                    loopPoint = BRRFunctions.DecodeLoopPoint(fileContents),
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

                //TODO: Make it so you can have spaces in the BRR filename...
                //It's splitting incorrectly on filenames like "Piano (High).brr"

                var temp = new Instrument
                {
                    index = instIndex,
                    ADSR1 = byte.Parse(lineContents[1], NumberStyles.HexNumber),
                    ADSR2 = byte.Parse(lineContents[2], NumberStyles.HexNumber),
                    Gain = byte.Parse(lineContents[3], NumberStyles.HexNumber),
                    Multiplier = byte.Parse(lineContents[4], NumberStyles.HexNumber),
                    Sub = byte.Parse(lineContents[5], NumberStyles.HexNumber),
                    sample = BRRFunctions.FindDuplicate(samples, lineContents[0])
                };

                result.Add(temp);
                instIndex++;
            }

            return result;
        }

        static readonly string[] skippableStrings = new string[] { "{", "}", "#instruments", "#samples" };
        internal static bool LineShouldBeSkipped(string line) => string.IsNullOrEmpty(line) || skippableStrings.Any(line.Contains);

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


        public static void SaveTextfile(string result, string outputPath, string filename)
        {
            var outfile = Path.Combine(outputPath, $"{filename}.ccs");
            using var writer = new StreamWriter(outfile);
            writer.Write(result);
        }
    }
}
