using System;

namespace EBInstPack
{
    class Program
    {
        static void Main(string[] args)
        {
            const bool DEBUG = true;
            string folderPath;
            string outputFilename = "output";

            //TODO: Change the program structure so the output isn't one big blob.
            //It would be nice if all three transfers are isolated and labeled via CCScript comments.

            //TODO:
            //Verify duplicate patches!

            Console.Title = "EarthBound Instrument Packer";
            Console.WriteLine("Command-line usage:");
            Console.WriteLine("   EBInstPack [folder path in quotes]");
            Console.WriteLine("   Or just drag the folder onto the EXE!");
            Console.WriteLine("");

            if (DEBUG)
            {
                //folderPath = @"C:\Users\vince\Dropbox\Programming scratchpad\EarthBound-Instrument-Packer\EBInstPack\Examples\exampleAscent";
                folderPath = @"C:\Users\vince\Dropbox\Programming scratchpad\EarthBound-Instrument-Packer\EBInstPack\Examples\example-pack2a";
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

            //Start loading BRRs and stuff from instruments.txt
            var samples = FileIO.LoadBRRs(folderPath);
            var instruments = FileIO.LoadMetadata(folderPath, samples);
            var config = FileIO.LoadConfig(folderPath);

            //combine all BRRs into a cluster, generate sample pointer table
            var cluster = new BRRCluster(samples, config.sampleDirectoryOffset, config.brrDumpOffset); //TODO: load all of these passed-in offsets from a textfile too

            //check if the cluster goes over the ARAM limit
            var availableBytes = ARAM.maxOffset - config.brrDumpOffset; //Does this line need to be here? Can it be moved into ARAM.cs?
            if (ARAM.CheckLimit(cluster.SampleData, availableBytes))
            {
                Console.WriteLine();
                Console.WriteLine($"WARNING - You've gone over the ARAM limit by {cluster.SampleData.Length - availableBytes} bytes!");
                Console.WriteLine("Please try again...");
                Console.ReadLine();
                return;
            }

            //check max delay value. TODO: Print this and put the value in the CCScript file
            var maxDelay = ARAM.ReturnMaxDelayPossible(cluster.SampleData, config);
            Console.WriteLine("Max delay possible with this pack: " + maxDelay.ToString("X2"));

            //serialize instrument configuration table
            var metadata = new InstrumentConfigurationTable(instruments, config.instrumentConfigOffset);

            //Assemble everything into a .bin (TODO: make it so everything's separated, dangit)
            var bin = BinaryBlob.AssembleBin(cluster, metadata);

            //Turn the bin into a CCScript file
            outputFilename = FileIO.GetFolderNameFromPath(folderPath);
            var ccscript = CCScriptOutput.Generate(bin, config.packNumber, maxDelay, outputFilename);

            //Save the ccscript to output.ccs
            FileIO.SaveTextfile(ccscript, folderPath, outputFilename); //output to the same folder the BRRs are in
            Console.WriteLine($"Wrote {outputFilename}.ccs!");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static void PrintMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
