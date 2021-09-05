using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.IO;
using System.Linq;

namespace EBInstPack
{
    class FileIO
    {
        const string CONFIG_TEXTFILE_NAME = "config.txt";

        const string PACK_NUMBER = "pack num";
        const string BASE_INSTRUMENT = "base inst";
        const string SAMPLE_OFFSET = "offset";
        const string DEFAULT = "default";
        const string OVERWRITE = "overwrite";
        const string SEPARATOR = "$";

        internal static bool FolderNonexistant(string folderPath)
        {
            if (!File.Exists(GetFullConfigFilepath(folderPath)))
            {
                Console.WriteLine("Folder does not exist!");
                Console.WriteLine("Make sure you have a folder full of BRRs and a config file there.");
                Console.ReadLine();
                return true;
            }
            else return false;
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
            var allFiles = Directory.GetFiles(GetFullPath(folderPath));
            var brrFileList = LoadFilenames(folderPath); //Get just the filenames from config.txt
            var dupeIndex = 0;

            //Find matches based on the list, add them to the result
            var result = new List<BRRFile>();
            foreach (var nameToLookFor in brrFileList)
            {
                if (nameToLookFor.Contains("0x"))
                {
                    //this line is a duplicate, not a reference to a file - it contains manual ARAM offsets
                    var offsets = nameToLookFor.Split("0x");
                    result.Add(new BRRFile
                    {
                        dupeStartOffset = HexHelpers.HexStringToUInt16(offsets[1]),
                        dupeLoopOffset = HexHelpers.HexStringToUInt16(offsets[2]),
                        data = new byte[0], //empty so the running offset count doesn't get incremented
                        filename = $"Duplicate #{dupeIndex} {nameToLookFor}", //it breaks if there's identical filenames here
                    });
                    dupeIndex++;
                }
                else
                {
                    foreach (string filename in allFiles)
                    {
                        bool alreadyInserted = false;

                        var info = new FileInfo(filename);
                        if (info.Extension != ".brr" || info.Name != nameToLookFor)
                            continue; //skip everything that isn't a BRR file, and files that aren't the next one in the list

                        foreach (var foo in result)
                        {
                            if (foo.filename.Contains(nameToLookFor))
                                alreadyInserted = true;
                        }

                        if (alreadyInserted) continue;

                        var fileContents = File.ReadAllBytes(info.FullName);

                        if (BRRFunctions.FileHasNoLoopHeader(fileContents.Length))
                        {
                            result.Add(new BRRFile
                            {
                                data = fileContents,
                                loopPoint = 0, //rudimentary support for raw, non-AMK BRRs
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
                }
            }
            return result;
        }

        internal static List<EBMFile> LoadEBMs(string folderPath)
        {
            var result = new List<EBMFile>();

            var allFiles = Directory.GetFiles(GetFullPath(folderPath));
            foreach (string filename in allFiles)
            {
                var extension = Path.GetExtension(filename);
                var name = Path.GetFileNameWithoutExtension(filename);

                if (extension != ".ebm")
                    continue;

                result.Add(new EBMFile(name, File.ReadAllBytes(filename)));
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
                var line = lines[i].ToLower().Split(":");
                if (line[0].Contains(PACK_NUMBER))
                {
                    if (line[1].Contains(DEFAULT))
                        Program.GracefulCrash("Please specify a pack number in your config.txt!");
                    else
                        tempPackNum = HexHelpers.HexStringToByte(line[1]);
                }
                else if (line[0].Contains(BASE_INSTRUMENT))
                {
                    if (line[1].Contains(DEFAULT))
                        tempBaseInst = 0x1A;
                    else if (line[1].Contains(OVERWRITE))
                        tempBaseInst = 0x00;
                    else
                        tempBaseInst = HexHelpers.HexStringToByte(line[1]);
                }
                else if (line[0].Contains(SAMPLE_OFFSET))
                {
                    if (line[1].Contains(DEFAULT))
                        tempBRRoffset = ARAM.samplesOffset_1A;
                    else if (line[1].Contains(OVERWRITE))
                        tempBRRoffset = ARAM.samplesOffset;
                    else
                        tempBRRoffset = HexHelpers.HexStringToUInt16(line[1]);
                }
            }

            //load patches here
            var tempPatches = LoadMetadata(folderPath);

            var result = new Config(GetFolderName(folderPath), tempPackNum, tempBaseInst, tempBRRoffset, tempPatches);

            //Check that everything's there
            if (result.offsetForSampleDir == 0xFFFF)
                Program.GracefulCrash("Couldn't find a value for Sample Directory Offset in config.txt!");
            else if (result.offsetForBRRdump == 0xFFFF)
                Program.GracefulCrash("Couldn't find a value for BRR Sample Dump Offset in config.txt!");
            else if (result.offsetForInstrumentConfig == 0xFFFF)
                Program.GracefulCrash("Couldn't find a value for Instrument Config Table Offset in config.txt!");
            else if (result.packNumber == 0xFF)
                Program.GracefulCrash("Couldn't find a value for Pack Number in config.txt!");

            return result;
        }

        internal static List<Patch> LoadMetadata(string folderPath)
        {
            //rip through the textfile
            var lines = File.ReadLines(GetFullConfigFilepath(folderPath));

            byte instIndex = ARAM.defaultFirstSampleIndex; //set instIndex to the default value before even checking
            var dupeIndex = 0;
            var result = new List<Patch>();
            foreach (var line in lines)
            {
                var tempLine = line;

                if (tempLine.Contains(';'))
                {
                    tempLine = tempLine.Split(';')[0].Trim(); //get rid of comments if there are any
                }

                if (tempLine.ToLower().Contains(BASE_INSTRUMENT))
                {
                    if (tempLine.Contains(OVERWRITE)) //change instIndex if "default" isn't in the textfile after all
                        instIndex = 0x00;
                    else if (!tempLine.Contains(DEFAULT))
                    {
                        var splitLine = line.Split(": ")[1];
                        instIndex = HexHelpers.HexStringToByte(splitLine);
                    }
                }

                if (LineShouldBeSkipped(line)) continue;

                var lineContents = CleanTextFileLine(tempLine);

                if (lineContents[0].Contains("0x"))
                {
                    //if the Patch filename isn't the same as the BRR filename, glitches happen
                    lineContents[0] = $"Duplicate #{dupeIndex} {lineContents[0]}";
                    dupeIndex++;
                }

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

        internal static string[] LoadFilenames(string folderPath)
        {
            //rip through the textfile
            var lines = File.ReadLines(GetFullConfigFilepath(folderPath));
            var result = new List<string>();
            foreach (var line in lines)
            {
                if (!line.Contains('\"')) continue; //only process the lines with filenames in them
                var temp = CleanTextFileLine(line);
                result.Add(temp[0]); //add the filename
            }
            return result.ToArray();
        }

        static readonly string[] skippableStrings = new string[]
        {
            "{",
            "}",
            "#",
            SAMPLE_OFFSET,
            PACK_NUMBER,
            BASE_INSTRUMENT,
        };

        internal static bool LineShouldBeSkipped(string line) => string.IsNullOrEmpty(line) || skippableStrings.Any(line.ToLower().Contains);

        private static void CheckFormatting(string line)
        {
            int quoteCount = line.Count(f => f == '"');
            int dollarSignCount = line.Count(f => f == '$');

            if (quoteCount < 2 || dollarSignCount < 5)
            {
                Program.GracefulCrash($"This line isn't formatted correctly! Please check your config.txt.\n{line}");
            }
        }

        internal static List<string> CleanTextFileLine(string line)
        {
            var result = new List<string>();

            CheckFormatting(line);

            var splitLine = line.Split('"'); //seperate the "filename.brr" from the "   $XX $XX $XX $XX $XX"
            var filename = splitLine[1];
            var numbers = splitLine[2].Split(SEPARATOR);

            result.Add(filename);

            for (int i = 1; i < numbers.Length; i++) //skip index 0, which will have leftover spaces in it
            {
                result.Add(numbers[i].Trim());
            }

            return result;
        }

        public static void SaveCCScriptFile(string result, string outputPath, string filename)
        {
            var outfile = Path.Combine(outputPath, $"{filename}.ccs");
            using var writer = new StreamWriter(outfile);
            writer.Write(result);
            Console.WriteLine($"Wrote {filename}.ccs!");
        }

        public static void SaveSPCfile(byte[] result, string outputPath, string filename)
        {
            var outfile = Path.Combine(outputPath, $"{filename}.spc");
            using var writer = new BinaryWriter(File.Open(outfile, FileMode.Create));
            writer.Write(result);
            Console.WriteLine($"Wrote {filename}.spc!");
        }

        public static byte[] GetDummySPC()
        {
            //https://stackoverflow.com/questions/10412401/how-to-read-an-embedded-resource-as-array-of-bytes-without-writing-it-to-disk
            var asm = Assembly.GetExecutingAssembly();
            using Stream resFilestream = asm.GetManifestResourceStream("EBInstPack.Resources.N-SPC Test.spc");
            byte[] result = new byte[resFilestream.Length];
            resFilestream.Read(result, 0, result.Length);
            return result;
        }
    }
}
