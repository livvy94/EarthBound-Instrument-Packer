using System;

namespace EBInstPack
{
    class HexHelpers
    {
        public static Int16 ByteArrayToInt(byte[] input)
        {
            return BitConverter.ToInt16(input);
        }
    }
}
