using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PackSPC
{
    class FolderIO
    {
        const string TEXT_FILE_NAME = "METADATA.txt";

        internal static bool FolderNonexistant(string folderPath)
        {
            return !File.Exists(GetFullPath(folderPath));
        }

        internal static string GetFullPath(string folderPath)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), folderPath, TEXT_FILE_NAME);
        }

        internal static List<BRRFile> LoadBRRs(string brrPath)
        {
            var result = new List<BRRFile>();

            foreach(string filename in Directory.GetFiles(GetFullPath(brrPath)))
            {
                var info = new FileInfo(filename);
                if (info.Extension != "brr")
                    continue;

                var fileContents = File.ReadAllBytes(info.FullName);
                var brr = new BRRFile
                {
                    data = GetBRRData(fileContents),
                    loopPoint = GetBRRLoopPoint(fileContents),
                    filename = info.Name
                };
                result.Add(brr);
            }
            return result;
        }

        internal static int GetBRRLoopPoint(byte[] data)
        {
            return 0;
        }

        internal static List<byte> GetBRRData(byte[] data)
        {
            return new List<byte>();
        }

    }
}
