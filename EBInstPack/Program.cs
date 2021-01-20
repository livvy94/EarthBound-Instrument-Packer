using System;

namespace EBInstPack
{
    class Program
    {
        static void Main(string[] args)
        {
            string folderPath;
            byte packNumber = 0xFF;
            string outputFilename = "output";

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
                Console.WriteLine("Which pack is this going to replace?");
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

            //Start loading BRRs and stuff from the textfile
            var samples = FileIO.LoadBRRs(folderPath);
            var instruments = FileIO.LoadMetadata(folderPath, samples);

            //combine all BRRs into a cluster, generate sample pointer table
            var cluster = new BRRCluster(samples);

            var availableBytes = ARAM.maxOffset - ARAM.samplesOffset_1A;
            if (ARAM.CheckLimit(cluster.DataDump, availableBytes))
            {
                Console.WriteLine($"WARNING - You've gone over the ARAM limit by {cluster.DataDump.Length - availableBytes} bytes!");
                if (args.Length == 0) Console.ReadLine();
                return;
            }

            //serialize instrument configuration table
            var metadata = new InstrumentConfigurationTable(instruments);

            //Assemble everything into a .bin (todo: take this out of FileIO...)
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
