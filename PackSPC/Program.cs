using System;

namespace PackSPC
{
    class Program
    {
        static void Main(string[] args)
        {
            string brrPath;
            if (args.Length > 0)
            {
                brrPath = args[0];
            }
            else
            {
                //ask for the folder name
                Console.WriteLine("Input the folder name where the samples & .txt file are:");
                brrPath = Console.ReadLine();
            }

            if (FolderIO.FolderNonexistant(brrPath))
            {
                Console.WriteLine("Folder does not exist!");
                return;
            }

            var samples = FolderIO.LoadBRRs(brrPath);

            //There can be multiple metadata things that point to the same file - a one-to-many relationship
            //So how are we going to deal with this?
            //The current plan is:
            //- Have the filename be an attribute of BRRFile objects
            //- Have a BRRFile attribute in the Instrument object
            //- When ripping through the textfile and creating Instrument objects,
            //  check the contents of the quotation marks with the value in every BRRFile's filename and assign the one that matches
        }
    }
}
