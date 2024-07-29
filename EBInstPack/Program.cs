using System;

namespace EBInstPack
{
    class Program
    {
        //TODO:
        //Edge-case bugs that people find
        //Comission an icon for the program
        //Make it so it puts the BRRs in the CCScript file seperately and puts a comment with the filename
        //Recreate vanilla packs in the Examples folder to make them easier for people to modify
        //Finish songs instead of messing with this!!!

        static void Main(string[] args)
        {
            const bool DEBUG = false;
            const bool SAVE_CCS_FILE = false;
            string folderPath;

            Console.Title = "EarthBound Instrument Packer";

            //TODO: Do code-diving to get familiar with this thing's layout and structure

            if (DEBUG)
            {
                folderPath = @"C:\Users\vince\OneDrive\Desktop\EarthBound-Instrument-Packer\EBInstPack\Examples\famicomDetectiveClub";
            }
            else
            {
                if (args.Length == 0) //If they just double-clicked the exe - no args present
                {
                    Console.WriteLine("Command-line usage:");
                    Console.WriteLine("   EBInstPack [folder path in quotes, or .snake file]");
                    Console.WriteLine();
                    Console.WriteLine("Input the a path to the samples & text file:");
                    folderPath = Console.ReadLine();
                }
                else
                {
                    folderPath = args[0]; //Use the command-line argument if it's present
                }

                if (FileIO.FolderNonexistant(folderPath)) return;
            }

            if (folderPath.Contains(".snake"))
            {
                //TODO: Code to process an entire CoilSnake project!
                //Load \Music\songs.yml
                //For each entry (containing Song Pack and the name of an EBM file):
                //If there's no "Song Pack" field (for "Song to Reference" type stuff" skip it
                //If Song Pack is "in-engine" the folder name is "01"
                //Go to the Song Pack folder
                //If the EBM file doesn't exist, pause execution and say so
                //If the ebm.yml file doesn't exist, pause execution and say so

                //Check if there's an SPC file there
                //If so, check if Date Modified is younger than 7 days
                //If so, generate the SPC file again!
                //    Open the ebm.yml file and load the primary and secondary pack numbers.
                //    Do LoadBRRs using that!
                //    Do GenerateSampleDirectory and brrdump for both the primary and secondary packs.
            }
            else 
            {
                //It's a singular folder, continue how I originally designed it
                //load the config.txt
                var config = FileIO.LoadConfig(folderPath);
                var samples = FileIO.LoadBRRs(folderPath);

                //Generate all the hex
                var sampleDirectory = Config.GenerateSampleDirectory(config, samples);
                var brrDump = BRRFunctions.Combine(samples);

                //Validation
                var tooManyBRRs = ARAM.CheckBRRLimit(brrDump, config.offsetForBRRdump);

                config.maxDelay = ARAM.GetMaxDelayPossible(brrDump, config.offsetForBRRdump);
                Console.WriteLine($"Highest possible delay value for this pack: {config.maxDelay:X2}");
                Console.WriteLine($"These BRRs will be loaded into ARAM from {config.offsetForBRRdump:X4} to {config.offsetForBRRdump + brrDump.Length:X4}.\n");

                if (SAVE_CCS_FILE)
                {
                    var ccsFile = CCScriptOutput.Generate(config, sampleDirectory, brrDump);
                    FileIO.SaveCCScriptFile(ccsFile, folderPath, config);
                }

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
        }

        public static void GracefulCrash(string message)
        {
            Console.WriteLine("**  ERROR  **");
            Console.WriteLine(message);
            Console.ReadKey();
            Environment.Exit(-1);
        }
    }
}
