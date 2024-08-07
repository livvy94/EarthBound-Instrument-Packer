﻿using System;
using System.Collections.Generic;

namespace EBInstPack
{
    class BRRFile
    {
        public ushort loopPoint;
        public byte[] data;
        public string filename;

        public ushort dupeStartOffset; //for duplicates - patches in a secondary pack that point to a primary pack
        public ushort dupeLoopOffset;
    }

    class BRRFunctions
    {
        internal static BRRFile Find(string filename, List<BRRFile> samples)
        {
            foreach (var sample in samples)
            {
                if (sample.filename == filename)
                    return sample;
            }

            throw new Exception($"Can't find a file named {filename}.brr! This should never happen.");
        }

        public static ushort DecodeLoopPoint(byte[] fileData)
        {
            //This function takes in an AddMusicK-format BRR file and returns its loop point's numerical value.
            //Special thanks to Milon in the SMW Central discord for this explanation.
            //A BRR loop point is stored by taking the raw value, dividing it by 16, and multiplying it by 9.

            //For example, Chrono Trigger's Choir sample has a loop point of 2288.
            //2288 / 16 = 143
            //143 * 9 = 1287
            //1287 in hex is [05 07], which is then swapped as [07 05].
            //(Which is what you see if you open that file in a hex editor!)

            //So to do the inverse, we need to take the first two bytes of a file,
            //reverse them, divide the number by 9, and multiply it by 16.
            var amkLoopHeader = IsolateLoopPoint(fileData);
            return (ushort)(HexHelpers.ByteArrayToInt(amkLoopHeader) / 9 * 16);
        }

        public static ushort EncodeLoopPoint(ushort rawLoopPoint)
        {
            return (ushort)(rawLoopPoint / 16 * 9);
        }

        private static byte[] IsolateLoopPoint(byte[] fileData)
        {
            return new byte[] { fileData[0], fileData[1] };
        }

        public static byte[] IsolateBRRdata(byte[] fileData)
        {
            var newArrayLength = fileData.Length - 2;
            var result = new byte[newArrayLength];
            Array.Copy(fileData, 2, result, 0, newArrayLength);
            return result;
        }

        public static bool FileHasNoLoopHeader(int fileLength)
        {
            return fileLength % 9 == 0; //BRR blocks are 9 bytes long
        }

        public static byte[] Combine(List<BRRFile> samples)
        {
            var result = new List<byte>();

            foreach (var sample in samples)
            {
                if (sample.filename.Contains("Duplicate"))
                {
                    Console.WriteLine("(Manual ARAM Reference)".PadRight(40, '.') + "0 bytes".PadLeft(4, '.'));
                    continue;
                }

                Console.WriteLine($"Loading {sample.filename}".PadRight(40, '.') + sample.data.Length.ToString().PadLeft(4, '.') + " bytes");
                result.AddRange(sample.data);
            }
            return result.ToArray();
        }
    }
}
