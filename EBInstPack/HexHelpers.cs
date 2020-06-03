using System;
using System.Collections.Generic;

namespace EBInstPack
{
    class HexHelpers
    {
        public static Int16 ByteArrayToInt(byte[] input)
        {
            return BitConverter.ToInt16(input);
        }

        public static byte[] IntToByteArray_Length2(int input)
        {
            var temp = BitConverter.GetBytes(input);

            if (temp.Length == 1) //It'll need to be [00 XX], not just [XX]
                return new byte[] { 0x00, temp[0] };

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
    }
}
