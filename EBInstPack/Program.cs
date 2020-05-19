using System;

namespace EBInstPack
{
    class Program
    {
        static void Main(string[] args)
        {
            string folderPath;
            if (args.Length > 0)
            {
                folderPath = args[0];
            }
            else
            {
                //ask for the folder name
                Console.WriteLine("Input the folder name where the samples & text file are:");
                folderPath = Console.ReadLine();
            }

            if (FileLoading.FolderNonexistant(folderPath))
            {
                Console.WriteLine("Folder does not exist!");
                Console.WriteLine("Make sure you have a folder full of BRRs and a textfile there.");
                return;
            }

            var samples = FileLoading.LoadBRRs(folderPath);
            var instruments = FileLoading.LoadMetadata(folderPath, samples);

            //combine all BRRs into a cluster, generate sample pointer table
            var cluster = new BRRCluster(samples);

            //serialize to instrument configuration table

            //calculate all the headers

            //slap 'em all together and save as a .bin
        }
    }
}
