using System;

namespace EBInstPack
{
    class Program
    {
        static void Main(string[] args)
        {
            var no_arg_mode = args.Length == 0;
            string folderPath;

            if (no_arg_mode)
            {
                Console.WriteLine("Input the folder name where the samples & text file are:");
                folderPath = Console.ReadLine();
            }
            else
            {
                folderPath = args[0];
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
            //Save it all
            FileIO.Save(cluster, metadata);
        }
    }
}
