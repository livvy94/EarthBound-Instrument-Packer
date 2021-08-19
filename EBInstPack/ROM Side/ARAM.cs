using System;

namespace EBInstPack
{
    class ARAM
    {
        //This information is here for reference.
        //Putting "default" in base instrument and sample dump offset causes the values for 1A to be used
        //"overwrite all" causes the non-1A values to be used!

        //Offsets
        public const UInt16 noteDataOffset = 0x4800; //This offset is one of the places where .EBM files get dumped
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

        public static bool CheckBRRLimit(byte[] brrDump, ushort offset)
        {
            //check if there's too much BRR data to fit into ARAM
            var availableBytes = maxOffset - offset;
            if (CheckLimit(brrDump, availableBytes))
            {
                Console.WriteLine();
                Console.WriteLine($"WARNING - You've gone over the ARAM limit by {brrDump.Length - availableBytes} bytes!");
                Console.WriteLine("Please try again...");
                Console.ReadLine();
                return true;
            }

            return false;
        }

        public static bool CheckLimit(byte[] data, int maxPossibleValue)
        {
            var bytesLeft = maxPossibleValue - data.Length;
            Console.WriteLine($"{data.Length} total bytes of BRR data ({bytesLeft} bytes left)");
            if (CalculateOverwrittenBytes(data, maxPossibleValue) > 0)
                return false;
            else
                return true;
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

        public static byte GetMaxDelayPossible(byte[] data, ushort offsetForBRRdump)
        {
            int sampleDumpStart = offsetForBRRdump;
            int sampleDumpEnd = sampleDumpStart + data.Length;

            byte currentDelayValue = 0x10; //a ludicrous amount of delay that nobody will ever use
            int currentStartingOffset = 0x7F00; //the starting offset for that amount

            while (currentStartingOffset < sampleDumpEnd)
            {
                if (currentDelayValue == 0)   //This fixes an edge case where ARAM is VERY close to being overfull,
                    return currentDelayValue; //and currentDelayValue ends up being 0x00 minus 1, which returns 0xFF

                //keep going until the data would succesfully fit with no issues
                currentDelayValue--;
                currentStartingOffset += 0x0800; //the lower the delay value gets, the greater in ARAM the echo start offset becomes
            }

            return currentDelayValue;

            //High echo value     Low echo value
            //.................   .................
            //.................   .................
            //.................   .................
            //eeeeeeeeeeeeeeeee   .................
            //eeeeeeeeeeeeeeeee   .................
            //eeeeeeeeeeeeeeeee   eeeeeeeeeeeeeeeee  <-- start of echo in ARAM
        }
    }
}
