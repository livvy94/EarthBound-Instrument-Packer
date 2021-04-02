using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EBInstPack
{
    class FileIO
    {
        const string CONFIG_TEXTFILE_NAME = "config.txt";

        internal static bool FolderNonexistant(string folderPath)
        {
            return !File.Exists(GetFullConfigFilepath(folderPath));
        }

        internal static string GetFullConfigFilepath(string folderPath)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), folderPath, CONFIG_TEXTFILE_NAME);
        }
        internal static string GetFullPath(string folderPath)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), folderPath);
        }

        internal static string GetFolderName(string folderPath)
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

                if (BRRFunctions.FileHasNoLoopHeader(fileContents))
                {
                    //rudimentary support for raw, non-AMK BRRs
                    result.Add(new BRRFile
                    {
                        data = fileContents.ToList(),
                        loopPoint = 0,
                        filename = info.Name,
                    });
                }
                else
                {
                    //separate the loop point header and the actual BRR data, and add them to the list
                    result.Add(new BRRFile
                    {
                        data = BRRFunctions.IsolateBRRdata(fileContents),
                        loopPoint = BRRFunctions.DecodeLoopPoint(fileContents),
                        filename = info.Name,
                    });
                }
            }
            return result;
        }

        internal static Config LoadConfig(string folderPath)
        {
            byte tempPackNum = 0xFF;
            byte tempBaseInst = 0xFF;
            ushort tempBRRoffset = 0xFFFF;

            //load the first three lines of the config.txt
            var lines = File.ReadLines(GetFullConfigFilepath(folderPath)).ToList();
            for (int i = 0; i < 3; i++)
            {
                var line = lines[i].ToLower().Split(": ");
                if (line[0].Contains("pack num"))
                {
                    if (line[1].Contains("default"))
                        throw new Exception("Please specify a pack number!"); //TODO: Try this and see if it works as expected
                    else
                        tempPackNum = HexHelpers.HexStringToByte(line[1]);
                }
                else if (line[0].Contains("base inst"))
                {
                    if (line[1].Contains("default"))
                        tempBaseInst = 0x1A;
                    else
                        tempBaseInst = HexHelpers.HexStringToByte(line[1]);
                }
                else if (line[0].Contains("brr") || line[0].Contains("sample"))
                {
                    if (line[1].Contains("default"))
                        tempBRRoffset = ARAM.samplesOffset_1A;
                    else
                        tempBRRoffset = HexHelpers.HexStringToUInt16(line[1]);
                }
            }

            //load patches here
            var tempPatches = LoadMetadata(folderPath);

            var result = new Config(GetFolderName(folderPath), tempPackNum, tempBaseInst, tempBRRoffset, tempPatches);

            //Check that everything's there
            if (result.offsetForSampleDir == 0xFFFF)
                throw new Exception("Couldn't find a value for Sample Directory Offset in config.txt!");
            else if (result.offsetForBRRdump == 0xFFFF)
                throw new Exception("Couldn't find a value for BRR Sample Dump Offset in config.txt!");
            else if (result.offsetForInstrumentConfig == 0xFFFF)
                throw new Exception("Couldn't find a value for Instrument Config Table Offset in config.txt!");
            else if (result.packNumber == 0xFF)
                throw new Exception("Couldn't find a value for Pack Number in config.txt!");

            return result;
        }

        internal static List<Patch> LoadMetadata(string folderPath)
        {
            //rip through the textfile
            var lines = File.ReadLines(GetFullConfigFilepath(folderPath));

            byte instIndex = ARAM.defaultFirstSampleIndex;
            var result = new List<Patch>();
            foreach (var line in lines)
            {
                //set the base instrument if it's set to something other than "default"
                if (line.ToLower().Contains("base instrument") && !line.Contains("default"))
                {
                    var splitLine = line.Split(": ")[1];
                    instIndex = HexHelpers.HexStringToByte(splitLine);
                }

                if (LineShouldBeSkipped(line)) continue;
                var lineContents = CleanTextFileLine(line);

                //TODO: Make it so you can have spaces in the BRR filename...
                //It splits incorrectly if stuff like "Piano (High).brr" is in the textfile
                //Maybe split on quote marks, then do this kind of splitting on the rest of it?? Ugh

                var temp = new Patch
                {
                    index = instIndex,
                    ADSR1 = byte.Parse(lineContents[1], NumberStyles.HexNumber),
                    ADSR2 = byte.Parse(lineContents[2], NumberStyles.HexNumber),
                    Gain = byte.Parse(lineContents[3], NumberStyles.HexNumber),
                    Multiplier = byte.Parse(lineContents[4], NumberStyles.HexNumber),
                    Sub = byte.Parse(lineContents[5], NumberStyles.HexNumber),
                    Filename = lineContents[0]
                };

                result.Add(temp);
                instIndex++;
            }

            return result;
        }

        static readonly string[] skippableStrings = new string[]
        {
            "{",
            "}",
            "#",
            "dump offset",
            "pack num",
            "base instrument",
        };

        internal static bool LineShouldBeSkipped(string line) => string.IsNullOrEmpty(line) || skippableStrings.Any(line.ToLower().Contains);

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
