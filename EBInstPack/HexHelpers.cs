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

        public static byte[] IntToByteArray(int input)
        {
            return BitConverter.GetBytes(input); //will these need to be reversed due to endian stuff?
        }
    }
}
