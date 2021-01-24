using System;
using System.Collections.Generic;

namespace EBInstPack
{
    class ARAM
    {
        //This information is here for reference.
        //"default" in config.txt should cause the 1A values to be used

        //Offsets
        public const UInt16 noteDataOffset = 0x4800; //This offset is where the contents of .EBM files gets dumped
        public const UInt16 sampleDirectoryOffset = 0x6C00; //the start of the sample directory
        public const UInt16 instrumentConfigTableOffset = 0x6E00; //aka ADSR information & tuning, aka "patches"
        public const UInt16 samplesOffset = 0x7000; //the actual waveform data itself - BRR files
        public const UInt16 maxOffset = 0xFFFF; //This is as far as you go! <3

        //Offsets specific to 1A, the first entry in secondary instrument packs
        //Thanks for the clever math stuff, Catador!
        public const UInt16 sampleDirectoryOffset_1A = sampleDirectoryOffset + 0x1A * 4; //6C68
        public const UInt16 instrumentConfigTableOffset_1A = instrumentConfigTableOffset + 0x1A * 6; //6E9C
        public const UInt16 samplesOffset_1A = 0x95B0;
        public const byte defaultFirstSampleIndex = 0x1A;

        //Methods for calculating how much available space there is at any given part of ARAM
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

        //The delay offset for 00 is 0xFF00, and each subsequent value is 0x0800 bytes less than this.
        //For example, delay at 01 starts at 0xF700, and delay at 02 starts at 0xEF00, etc.
        //The start offset keeps going up the more delay you use.
        //With the exception of 00, which just uses four bytes (0xFF00 to 0xFF03) and leaves the rest unused,
        //delay memory is allocated from the starting offset until the very end of ARAM, 0xFFFF.
        //I'm not going to use the following values in the calculation, but I'd like to keep them here as reference.
        //public const UInt16 delayOffsetFor00 = 0xFF00;
        //public const UInt16 delayOffsetFor01 = 0xF700;
        //public const UInt16 delayOffsetFor02 = 0xEF00;
        //public const UInt16 delayOffsetFor03 = 0xE700;
        //public const UInt16 delayOffsetFor04 = 0xDF00;
        //public const UInt16 delayOffsetFor05 = 0xD700;
        //public const UInt16 delayOffsetFor06 = 0xCF00;
        //public const UInt16 delayOffsetFor07 = 0xC700;
        //public const UInt16 delayOffsetFor08 = 0xBF00;
        //public const UInt16 delayOffsetFor09 = 0xB700;
        //public const UInt16 delayOffsetFor0A = 0xAF00;
        //public const UInt16 delayOffsetFor0B = 0xA700;
        //public const UInt16 delayOffsetFor0C = 0x9F00;
        //public const UInt16 delayOffsetFor0D = 0x9700;
        //public const UInt16 delayOffsetFor0E = 0x8F00;
        //public const UInt16 delayOffsetFor0F = 0x8700;
        //public const UInt16 delayOffsetFor10 = 0x7F00;

        public static byte ReturnMaxDelayPossible(byte[] data, bool startAt1A)
        {
            //TODO: Make it so the startAt1A variable is passed in from Program.cs
            //TODO: actually use this method to display a message and/or add a comment to the CCScript file
            int sampleDumpStart = startAt1A ? samplesOffset : sampleDirectoryOffset_1A;
            int sampleDumpEnd = sampleDumpStart + data.Length;

            //Start at a ludicrous amount of delay that'd never be used, like 10
            byte currentDelayValue = 0x10;
            int currentStartingOffset = 0x7F00;

            while (currentStartingOffset < sampleDumpEnd)
            {
                //keep going until the data would succesfully fit with no issues
                currentDelayValue--;
                currentStartingOffset += 0x0800;
            }

            return currentDelayValue;
        }
    }
}
