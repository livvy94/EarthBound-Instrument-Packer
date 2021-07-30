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

            //TODO:
            //Some way to specify exact ARAM locations right in config.txt (to make duplicates that point to Pack 05 stuff)
            //Comission an icon for the program
            //Replace the "[XX YY]" with "short 0xYYXX" if it looks cleaner

            //load the folder contents
            if (DEBUG)
            {
                folderPath = @"C:\Users\vince\Dropbox\Programming scratchpad\EarthBound-Instrument-Packer\EBInstPack\Examples\famicomDetectiveClub";
            }
            else
            {
                if (args.Length == 0) //If they just double-clicked the exe - no args present
                {
                    Console.WriteLine("Command-line usage:");
                    Console.WriteLine("   EBInstPack [folder path in quotes]");
                    Console.WriteLine("   Or just drag the folder onto the EXE!");
                    Console.WriteLine();
                    Console.WriteLine("Input the folder path where the samples & text file are:");
                    folderPath = Console.ReadLine();
                }
                else
                {
                    folderPath = args[0]; //Use the command-line argument if it's present
                }

                if (FileIO.FolderNonexistant(folderPath)) return;
            }

            //load the config.txt
            var config = FileIO.LoadConfig(folderPath);
            var samples = FileIO.LoadBRRs(folderPath);

            //Generate all the hex
            var sampleDirectory = BinaryBlob.GenerateSampleDirectory(config, samples);
            var brrDump = BinaryBlob.GenerateBRRDump(samples);

            //Validation
            var tooManyBRRs = ARAM.CheckBRRLimit(brrDump, config.offsetForBRRdump);
            if (tooManyBRRs) return;

            config.maxDelay = ARAM.GetMaxDelayPossible(brrDump, config.offsetForBRRdump);
            Console.WriteLine($"Highest possible delay value for this pack: {config.maxDelay:X2}\n");

            var ccsFile = CCScriptOutput.Generate(config, sampleDirectory, brrDump);
            FileIO.SaveCCScriptFile(ccsFile, folderPath, config.outputFilename);

            //Check the folder for .EBM files
            var ebmFiles = FileIO.LoadEBMs(folderPath);
            foreach (var song in ebmFiles)
            {
                var spc = new PreviewSPC(config, sampleDirectory, brrDump, song);
                FileIO.SaveSPCfile(spc.filedata, folderPath, song.name);
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static void GracefulCrash(string message)
        {
            Console.WriteLine("\n**  ERROR  **");
            Console.WriteLine(message);
            Console.ReadKey();
            Environment.Exit(-1);
        }
    }
}
