using System;
using System.Collections.Generic;
using System.IO;

namespace EBInstPack
{
    class EBMFile
    {
        public ushort aramOffset;
        public byte[] data;
        public string name;

        public EBMFile(string filename, byte[] filedata)
        {
            //the header is "copy (size) bytes to (offset)", and we want the data and the offset seperately
            aramOffset = BitConverter.ToUInt16(new byte[] { filedata[2], filedata[3] }); //TODO: Check endian weirdness
            data = IsolateSequenceData(filedata);
            name = filename;
        }

        private byte[] IsolateSequenceData(byte[] fileData)
        {
            var newArrayLength = fileData.Length - 4;
            var result = new byte[newArrayLength];
            Array.Copy(fileData, 4, result, 0, newArrayLength);
            return result;
        }
    }
}
