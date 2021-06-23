using System;

namespace EBInstPack
{
    class Program
    {
        static void Main(string[] args)
        {
            const bool DEBUG = false;
            string folderPath;

            Console.Title = "EarthBound Instrument Packer";
            Console.WriteLine("Command-line usage:");
            Console.WriteLine("   EBInstPack [folder path in quotes]");
            Console.WriteLine("   Or just drag the folder onto the EXE!");
            Console.WriteLine();

            //TODO:
            //See if overwriting pack 05 works correctly
            //BRRs shouldn't need to be in alphabetical order
            //Program Icon
            //Replace the "[XX YY]" with short 0xYYXX if it looks cleaner

            //load the folder contents
            if (DEBUG)
            {
                folderPath = @"C:\Users\vince\Dropbox\Programming scratchpad\EarthBound-Instrument-Packer\EBInstPack\Examples\famicomDetectiveClub";
            }
            else
            {
                if (args.Length == 0) //If they just double-clicked the exe - no args present
                {
                    Console.WriteLine("Input the folder path where the samples & text file are:");
                    folderPath = Console.ReadLine();
                }
                else folderPath = args[0]; //Use the command-line argument if it's present

                if (FileIO.FolderNonexistant(folderPath))
                {
                    Console.WriteLine("Folder does not exist!");
                    Console.WriteLine("Make sure you have a folder full of BRRs and a config file there.");
                    Console.ReadLine();
                    return;
                }
            }

            //load the config.txt
            var config = FileIO.LoadConfig(folderPath);
            var samples = FileIO.LoadBRRs(folderPath);

            //Generate all the hex
            var sampleDirectory = BinaryBlob.GenerateSampleDirectory(config, samples);
            var brrDump = BinaryBlob.GenerateBRRDump(samples);

            var tooManyBRRs = ARAM.CheckBRRLimit(brrDump, config.offsetForBRRdump);
            if (tooManyBRRs) return;
            config.maxDelay = ARAM.GetMaxDelayPossible(brrDump, config.offsetForBRRdump);

            var ccsFile = CCScriptOutput.Generate(config, sampleDirectory, brrDump);

            FileIO.SaveTextfile(ccsFile, folderPath, config.outputFilename);

            Console.WriteLine("Highest possible delay value for this pack: " + config.maxDelay.ToString("X2"));
            Console.WriteLine();
            Console.WriteLine($"Wrote {config.outputFilename}.ccs!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
