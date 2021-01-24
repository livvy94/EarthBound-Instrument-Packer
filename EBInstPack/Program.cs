using System;

namespace EBInstPack
{
    class Program
    {
        static void Main(string[] args)
        {
            string folderPath;
            byte packNumber = 0xFF; //TODO: change this to be passed in from the file!
            string outputFilename = "output";

            //TODO: Change the program structure so the output isn't one big blob.
            //It would be nice if all three transfers are isolated and labeled via CCScript comments.

            //TODO: make the folder structure like this!!
            //config.txt <- contains
            //  Instrument Pack to replace: XX
            //  Sample directory offset: default          //(offsets here will be in little-endian format, like 4800
            //  BRR sample dump offset: default
            //  Instrument config table offset: default

            //config.txt (song version)
            //  Song Pack to replace: XX
            //  Sequence data offset: default
            //  Instrument packs to use: 05 08
            //  Song address: 4800

            //instruments.txt <- contains AMK-like instrument definitions
            //a bunch of BRRs

            //With this structure, we can pass in custom offsets - only necessary if you wants to overwrite
            //or customize Pack 05, the default instrument set that's loaded for 99% of the game

            Console.Title = "EarthBound Instrument Packer";
            Console.WriteLine("Command-line usage:");
            Console.WriteLine("   EBInstPack [folder path in quotes] [intended pack number in hex]");
            Console.WriteLine("   Or just drag the folder on top of the EXE!");
            Console.WriteLine("");

            if (args.Length == 1 && args.ToString().EndsWith(".ebm"))
            {
                Console.WriteLine("EBM file stuff isn't finished yet!");
                Console.ReadLine();
                return;
            }

            if (args.Length == 0) //If they just double-clicked the exe - no args present
            {
                Console.WriteLine("Input the folder path where the samples & text file are:");
                folderPath = Console.ReadLine();
            }
            else folderPath = args[0]; //Use the command-line argument if it's present

            if (args.Length < 2) //If the only arg present is the folder path
            {
                Console.WriteLine("Which pack is this going to replace?"); //TODO: get this info from config.txt
                packNumber = byte.Parse(Console.ReadLine(), System.Globalization.NumberStyles.HexNumber); //TODO: Data validation?
            }
            else packNumber = byte.Parse(args[1], System.Globalization.NumberStyles.HexNumber);

            if (FileIO.FolderNonexistant(folderPath))
            {
                Console.WriteLine("Folder does not exist!");
                Console.WriteLine("Make sure you have a folder full of BRRs and a textfile there.");
                if (args.Length == 0) Console.ReadLine();
                return;
            }

            //Start loading BRRs and stuff from instruments.txt
            var samples = FileIO.LoadBRRs(folderPath);
            var instruments = FileIO.LoadMetadata(folderPath, samples);

            //combine all BRRs into a cluster, generate sample pointer table
            var cluster = new BRRCluster(samples, ARAM.sampleDirectoryOffset_1A, ARAM.samplesOffset_1A); //TODO: load all of these passed-in offsets from a textfile too

            var availableBytes = ARAM.maxOffset - ARAM.samplesOffset_1A; //Does this line need to be here? Can it be moved into ARAM.cs?
            if (ARAM.CheckLimit(cluster.SampleData, availableBytes))
            {
                Console.WriteLine($"WARNING - You've gone over the ARAM limit by {cluster.SampleData.Length - availableBytes} bytes!");
                if (args.Length == 0) Console.ReadLine();
                return;
            }

            //serialize instrument configuration table
            var metadata = new InstrumentConfigurationTable(instruments, ARAM.instrumentConfigTableOffset_1A);

            //Assemble everything into a .bin (TODO: make it so everything's separated, dangit)
            var bin = BinaryBlob.AssembleBin(cluster, metadata);

            //Turn the bin into a CCScript file
            outputFilename = FileIO.GetFolderNameFromPath(folderPath);
            var ccscript = CCScriptOutput.Generate(bin, packNumber, outputFilename);

            //Save the ccscript to output.ccs
            FileIO.SaveTextfile(ccscript, folderPath, outputFilename); //output to the same folder the BRRs are in
            Console.WriteLine($"Wrote {outputFilename}.ccs!");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
