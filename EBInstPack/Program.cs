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
            Console.WriteLine("");

            //load the folder contents
            if (DEBUG)
            {
                folderPath = @"C:\Users\vince\Dropbox\Programming scratchpad\EarthBound-Instrument-Packer\EBInstPack\Examples\WilliamAreaTheme";
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
            //var patches = BinaryBlob.GeneratePatches(config); //this is unused! TODO: Delete this and GeneratePatches() if it's unused now!
            var brrDump = BinaryBlob.GenerateBRRDump(samples);

            //make sure we haven't gone over the ARAM limit
            var tooManyBRRs = ARAM.CheckBRRLimit(brrDump, config.offsetForBRRdump);
            if (tooManyBRRs)
            {
                Console.WriteLine("Please try again...");
                Console.ReadLine();
                return;
            }

            config.maxDelay = ARAM.GetMaxDelayPossible(brrDump, config);

            var ccsFile = CCScriptOutput.Generate(config, sampleDirectory, brrDump);

            FileIO.SaveTextfile(ccsFile, folderPath, config.outputFilename); //save the CCScript file

            Console.WriteLine("Highest possible delay value for this pack: " + config.maxDelay.ToString("X2"));
            Console.WriteLine();
            Console.WriteLine($"Wrote {config.outputFilename}.ccs!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
