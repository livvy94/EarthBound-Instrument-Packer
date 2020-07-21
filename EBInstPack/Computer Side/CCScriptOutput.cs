using System;
using System.Collections.Generic;
using System.Text;

namespace EBInstPack
{
    class CCScriptOutput
    {
        const string NEWLINE = "\r\n";

        internal static string Generate(byte[] bin, byte packNumber, string outputFilename)
        {
            var result = new StringBuilder();
            result.Append("command inst_pack_loc (target) \"[{byte[2] target} {byte[0] target} {byte[1] target}]\"");
            result.Append(NEWLINE);
            result.Append(NEWLINE);
            result.Append($"//Instrument Pack {packNumber:X2}");
            result.Append(NEWLINE);
            result.Append($"ROM[{GetPointerOffset(packNumber)}] = inst_pack_loc({outputFilename})");
            result.Append(NEWLINE);
            result.Append(NEWLINE);
            result.Append(outputFilename);
            result.Append(": {");
            result.Append(NEWLINE);
            result.Append("\"[");
            result.Append(HexConvert(bin));
            result.Append("]\"");
            result.Append(NEWLINE);
            result.Append("}");

            return result.ToString();
        }

        private static string HexConvert(byte[] input) //TODO: Move to HexHelpers
        {
            var result = new StringBuilder();
            foreach (byte bytes in input)
            {
                result.Append(bytes.ToString("X2"));
                result.Append(' ');
            }

            result.Length--; //remove the last space
            return result.ToString();
        }

        private static string GetPointerOffset(int packNumber)
        {
            //A documentation of instrument packs can be found here:
            //https://gist.github.com/vince94/cb70ddd4c5309c0c52e662f985d6648b

            //The disassembly shows this pointer table here:
            //https://github.com/Herringway/ebsrc/blob/master/src/data/music/pack_pointer_table.asm

            //Note to self - the pointer data at these locations have swapped bytes.
            //A pointer that reads E2F077 in a hex editor is pointing to E277F0.
            //This is reflected in the CCScript command, which swaps things around before writing to the ROM.
            //Also - I assume these are SNES HiROM-type offset values. See the helper methods about converting them for more info.

            var initialOffset = 0x04F947;
            var sizeOfEachPointer = 3;

            var i = 0;
            var result = initialOffset;
            while (i < packNumber)
            {
                result += sizeOfEachPointer;
                i++;
            }

            result = HexHelpers.OffsetConvert_PCtoHiROM(result);
            return $"0x{result:X6}";
        }
    }
}
