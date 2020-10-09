using System;

namespace EBInstPack
{
    class Program
    {
        const bool DEBUG_MODE = true;
        static void Main(string[] args)
        {
            var no_arg_mode = args.Length < 2;
            string folderPath;
            byte packNumber = 0xFF;
            string outputFilename = "output";

            if (DEBUG_MODE)
            {
                folderPath = @"C:\Users\vince\Dropbox\Programming scratchpad\EarthBound-Instrument-Packer\EBInstPack\vanilla-pack-belch";
                packNumber = 0x72;
                outputFilename = "belch_maybe";
                //Note to self: drop the output in the Missingno project since that's already loaded into CoilSnake
            }
            else if (no_arg_mode)
            {
                Console.WriteLine("Input the folder name where the samples & text file are:");
                folderPath = Console.ReadLine();

                Console.WriteLine("Which pack is this going to replace? (Type it in hex, please.)");
                packNumber = byte.Parse(Console.ReadLine(), System.Globalization.NumberStyles.HexNumber);

                Console.WriteLine("What would you like the .ccs file to be named?");
                var nameInput = Console.ReadLine().Trim();
                if (nameInput != string.Empty)
                    outputFilename = nameInput;
            }
            else
            {
                folderPath = args[0];
                packNumber = byte.Parse(args[1], System.Globalization.NumberStyles.HexNumber);
            }

            if (FileIO.FolderNonexistant(folderPath))
            {
                Console.WriteLine("Folder does not exist!");
                Console.WriteLine("Make sure you have a folder full of BRRs and a textfile there.");
                if (no_arg_mode) Console.ReadLine();
                return;
            }

            var samples = FileIO.LoadBRRs(folderPath);
            var instruments = FileIO.LoadMetadata(folderPath, samples);

            //combine all BRRs into a cluster, generate sample pointer table
            var cluster = new BRRCluster(samples);

            //serialize instrument configuration table
            var metadata = new InstrumentConfigurationTable(instruments);

            //Assemble everything into a .bin (todo: take this out of FileIO...)
            var bin = FileIO.AssembleBin(cluster, metadata);

            //Turn the bin into a CCScript file
            var ccscript = CCScriptOutput.Generate(bin, packNumber, outputFilename);

            //Save the ccscript to output.ccs
            FileIO.SaveTextfile(ccscript, outputFilename);
            Console.WriteLine($"Wrote {outputFilename}.ccs!");

            if (no_arg_mode)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
