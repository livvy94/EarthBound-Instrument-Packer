using System;

namespace PackSPC
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
                Console.WriteLine("Input the folder name where the samples & .txt file are:");
                folderPath = Console.ReadLine();
            }

            if (FolderIO.FolderNonexistant(folderPath))
            {
                Console.WriteLine("Folder does not exist!");
                Console.WriteLine("Make sure you have a folder full of BRRs and a textfile there.");
                return;
            }

            var samples = FolderIO.LoadBRRs(folderPath);
            var instruments = FolderIO.LoadMetadata(folderPath, samples);

            //serialize to sample pointer table

            //serialize to brr dump

            //serialize to instrument configuration table

            //calculate all the headers

            //slap 'em all together and save as a .bin
        }
    }
}
