using System.Collections.Generic;
using System.Text;

namespace EBInstPack
{
    class CCScriptOutput
    {
        readonly static string NEWLINE = System.Environment.NewLine;
        readonly static byte[] END_OF_TRANSFER = { 0x00, 0x00 };

        public static string Generate(Config config, byte[] sampleDirectory, byte[] brrDump)
        {
            var filename = config.outputFilename;
            var patches = config.GetPatches();

            var result = new StringBuilder();
            result.Append("command inst_pack_loc (target) \"[{byte[2] target} {byte[0] target} {byte[1] target}]\"");
            result.Append(NEWLINE);
            result.Append(NEWLINE);
            result.Append($"//Instrument Pack {config.packNumber:X2}");
            result.Append(NEWLINE);
            result.Append($"ROM[{GetPointerOffset(config.packNumber)}] = inst_pack_loc({filename})");
            result.Append(NEWLINE);
            result.Append(NEWLINE);
            result.Append("//Highest possible delay value for this pack: " + config.maxDelay.ToString("X2"));
            result.Append(NEWLINE);
            result.Append(NEWLINE);
            result.Append(filename);
            result.Append(": {");
            result.Append(NEWLINE);

            //All the SPC transfer blocks, seperated and in neat little rows
            result.Append("//SAMPLE DIRECTORY");
            result.Append(NEWLINE);
            result.Append(GetSizeAndOffsetComments(sampleDirectory.Length, config.offsetForSampleDir));
            result.Append(BytesToCCScript(sampleDirectory, 4));
            result.Append(NEWLINE);

            result.Append("//PATCHES");
            result.Append(NEWLINE);
            result.Append(GetSizeAndOffsetComments(patches.Length, config.offsetForInstrumentConfig));
            result.Append(BytesToCCScript(patches, 6));
            result.Append(NEWLINE);

            result.Append("//BRR FILES");
            result.Append(NEWLINE);
            result.Append(GetSizeAndOffsetComments(brrDump.Length, config.offsetForBRRdump));
            result.Append(BytesToCCScript(brrDump, true)); //no way to seperate file by file so binary blob it is
            result.Append(NEWLINE);

            result.Append(BytesToCCScript(END_OF_TRANSFER, false));
            result.Append(" //END OF TRANSFER");
            result.Append(NEWLINE);
            result.Append("}");
            result.Append(NEWLINE);

            return result.ToString();
        }

        private static string BytesToCCScript(byte[] data, bool includeNewline)
        {
            var builder = new StringBuilder();
            builder.Append("\"[");
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append($"{data[i]:X2} ");
            }

            builder.Length--; //remove the last space
            builder.Append("]\"");

            if (includeNewline)
                builder.Append(NEWLINE);

            return builder.ToString();
        }

        private static string BytesToCCScript(byte[] data, int numbersPerRow)
        {
            var result = new StringBuilder();
            int currentOffset = 0;

            while (currentOffset < data.Length)
            {
                var partialData = new List<byte>();
                for (int i = 0; i < numbersPerRow; i++)
                {
                    partialData.Add(data[i + currentOffset]); //this won't work if the data is shorter than it's expecting
                }
                result.Append(BytesToCCScript(partialData.ToArray(), true));
                currentOffset += numbersPerRow;
            }

            return result.ToString(); //data in nice even rows, surrounded by quote marks
        }

        private static string GetSizeAndOffsetComments(int size, ushort offset)
        {
            var builder = new StringBuilder();
            builder.Append($"short {size} //Copy the next {size} bytes" + NEWLINE);
            builder.Append($"short 0x{offset:X4} //To ARAM offset {offset:X4}" + NEWLINE);
            return builder.ToString();
        }

        private static string GetPointerOffset(int packNumber)
        {
            //This method calculates where the pack pointer is, so we can make CoilSnake overwrite
            //it with whatever ROM offset the contents of the CCScript file gets inserted into!

            //A documentation of instrument packs can be found here:
            //https://gist.github.com/vince94/cb70ddd4c5309c0c52e662f985d6648b

            //The disassembly shows this pointer table here:
            //https://github.com/Herringway/ebsrc/blob/master/src/data/music/pack_pointer_table.asm

            //Note to self - the pointer data at these locations have swapped bytes.
            //A pointer that reads E2F077 in a hex editor is pointing to E277F0.
            //This is reflected in the CCScript command, which swaps things around before writing to the ROM.

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
