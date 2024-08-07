﻿using System;
using System.Buffers.Binary;
using System.Collections.Generic;

namespace EBInstPack
{
    class HexHelpers
    {
        //This is a collection of methods that'd probably be useful in any romhacking tool
        //I'm putting them here so I can save them for future projects

        internal static int OffsetConvert_PCtoHiROM(int input) => input + 0xC00000; //Including these so I remember how the conversion works
        //internal static int OffsetConvert_HiROMtoPC(int input) => input - 0xC00000; //This one's unused though

        internal static byte[] UInt16toByteArray_LittleEndian(UInt16 number)
        {
            var result = new byte[2];
            BinaryPrimitives.TryWriteUInt16LittleEndian(result, number);
            return result;
        }

        public static Int16 ByteArrayToInt(byte[] input)
        {
            return BitConverter.ToInt16(input);
        }

        public static byte[] IntsToByteArray_Length2(List<int> input)
        {
            var result = new List<byte>();

            foreach (var num in input)
            {
                result.AddRange(UInt16toByteArray_LittleEndian((ushort)num));
            }

            return result.ToArray();
        }

        public static UInt16 HexStringToUInt16(string input)
        {
            return Convert.ToUInt16(input.Trim(), 16);
        }

        public static byte HexStringToByte(string input)
        {
            try
            {
                return Convert.ToByte(input.Trim(), 16);
            }
            catch
            {
                Program.GracefulCrash("Something went wrong while trying to convert this to a byte: " + input);
                return 0;
            }
        }

        //https://docs.microsoft.com/en-us/dotnet/api/system.buffers.binary.binaryprimitives
        //TryReadUInt16LittleEndian(ReadOnlySpan<Byte>, UInt16)	- Reads a UInt16 from the beginning of a read-only span of bytes, as little endian.
        //TryReadUInt16BigEndian(ReadOnlySpan<Byte>, UInt16) - Reads a UInt16 from the beginning of a read-only span of bytes, as big endian.

        //TryWriteUInt16BigEndian(Span<Byte>, UInt16) - Writes a UInt16 into a span of bytes, as big endian.
        //TryWriteUInt16LittleEndian(Span<Byte>, UInt16) - Writes a UInt16 into a span of bytes, as little endian.

        //ReverseEndianness(UInt16)
    }
}
