using System;
using System.Collections.Generic;
using System.Text;
using System.Buffers.Binary;

namespace EBInstPack
{
    class HexHelpers
    {
        //This is a collection of methods that'd probably be useful in any romhacking tool
        //I'm putting them here so I can save them for future projects

        internal static int OffsetConvert_PCtoHiROM(int input) => input + 0xC00000; //Including these so I remember how the conversion works

        internal static byte[] UInt16toByteArray(UInt16 number) //TODO: replace useage of BitConverter (below) with this
        {
            var result = new byte[2];
            BinaryPrimitives.TryWriteUInt16LittleEndian(result, number);
            return result;
        }

        //internal static int OffsetConvert_HiROMtoPC(int input) => input - 0xC00000; //This one's unused though
        static int HexConvert(string input) => Convert.ToInt32(input, 16);

        public static Int16 ByteArrayToInt(byte[] input)
        {
            return BitConverter.ToInt16(input);
        }

        public static byte[] IntToByteArray_Length2(int input)
        {
            var temp = BitConverter.GetBytes((Int16)input);

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(temp);

            return temp;
        }

        public static byte[] IntsToByteArray_Length2(List<int> input)
        {
            var result = new List<byte>();

            foreach (var num in input)
            {
                result.AddRange(IntToByteArray_Length2(num));
            }

            return result.ToArray();
        }

        public static string HexConvert(byte[] input, bool includeQuotesAndBrackets)
        {
            var result = new StringBuilder();
            foreach (byte bytes in input)
            {
                result.Append(bytes.ToString("X2"));
                result.Append(' ');
            }

            result.Length--; //remove the last space

            if (includeQuotesAndBrackets)
                return "\"[" + result.ToString() + "]\"";
            else
                return result.ToString();
        }

        //TODO: Make something that turns 6C68 into new byte[] { 0x68, 0x6C } so implementing the consts in ARAM.cs is easier
    }
}
