using System;
using System.Collections.Generic;

namespace EBInstPack
{
    class ARAM
    {
        //Offsets
        public const UInt16 noteDataOffset = 0x4800; //This offset is where the contents of .EBM files gets dumped
        public const UInt16 sampleDirectoryOffset = 0x6C00; //the start of the sample directory
        public const UInt16 instrumentConfigTableOffset = 0x6E00; //aka ADSR information and tuning
        public const UInt16 samplesOffset = 0x7000; //the actual waveform data itself
        public const UInt16 maxOffset = 0xFFFF; //This is as far as you go! <3

        //Offsets specific to 1A, the first entry in secondary instrument packs
        //Thanks for the clever math stuff Catador!
        public const UInt16 sampleDirectoryOffset_1A = sampleDirectoryOffset + 0x1A * 4; //6C68
        public const UInt16 instrumentConfigTableOffset_1A = instrumentConfigTableOffset + 0x1A * 6; //6E9C
        public const UInt16 samplesOffset_1A = 0x95B0;

        public const UInt16 echoOffsetFor00 = 0xFF00; //According to the SPC Debugger, it's just four bytes. FF04 to FFFF are unused
        public const UInt16 echoOffsetFor01 = 0xFFFF; //TODO: finish finding these values. also none of them are used yet
        public const UInt16 echoOffsetFor02 = 0xFFFF;
        public const UInt16 echoOffsetFor03 = 0xFFFF;
        public const UInt16 echoOffsetFor04 = 0xFFFF;
        public const UInt16 echoOffsetFor05 = 0xFFFF;
        public const UInt16 echoOffsetFor06 = 0xFFFF;
        public const UInt16 echoOffsetFor07 = 0xFFFF;
        public const UInt16 echoOffsetFor08 = 0xFFFF;
        public const UInt16 echoOffsetFor09 = 0xFFFF;
        public const UInt16 echoOffsetFor0A = 0xFFFF; //You probably can't go up to 0A though. But wouldn't that be fun


        //Methods for calculate how much available space there is at any given part of ARAM
        public static int CalculateOverwrittenBytes(byte[] data, int maxPossibleValue)
        {
            return maxPossibleValue - data.Length;
        }

        public static bool CheckLimit(byte[] data, int maxPossibleValue) //TODO: test this lol
        {
            if (CalculateOverwrittenBytes(data, maxPossibleValue) > 0)
                return false;
            else
                return true;
        }

        public static bool CheckLimit(List<byte> data, int maxPossibleValue)
        {
            var temp = data.ToArray();
            return CheckLimit(temp, maxPossibleValue);
        }
    }
}
