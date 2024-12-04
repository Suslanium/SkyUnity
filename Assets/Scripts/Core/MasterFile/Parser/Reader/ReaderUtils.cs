using System.IO;
using Core.MasterFile.Parser.Structures.Records.FieldStructures.General;

namespace Core.MasterFile.Parser.Reader
{
    public static class ReaderUtils
    {
        private const char ZeroTerminator = '\0';

        public static char ReadWChar(this BinaryReader reader)
        {
            return (char)reader.ReadUInt16();
        }

        public static char ReadChar(this BinaryReader reader)
        {
            return (char)reader.ReadByte();
        }

        public static float ReadFloat32(this BinaryReader reader)
        {
            return reader.ReadSingle();
        }

        public static double ReadFloat64(this BinaryReader reader)
        {
            return reader.ReadDouble();
        }

        public static byte ReadUInt8(this BinaryReader reader)
        {
            return reader.ReadByte();
        }

        public static sbyte ReadInt8(this BinaryReader reader)
        {
            return reader.ReadSByte();
        }

        public static uint ReadFormId(this BinaryReader reader)
        {
            return reader.ReadUInt32();
        }

        public static string ReadZString(this BinaryReader reader, int length)
        {
            return new string(reader.ReadChars(length)).TrimEnd(ZeroTerminator);
        }
        
        public static string ReadString(this BinaryReader reader, int length)
        {
            return new string(reader.ReadChars(length));
        }

        // ReSharper disable once InconsistentNaming
        public static ColorRGBA ReadByteColorRGBA(this BinaryReader reader)
        {
            return new ColorRGBA(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
        }
        
        public static ColorRGB ReadByteColorRGB(this BinaryReader reader)
        {
            return new ColorRGB(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
        }

        public static Float32ColorRGB ReadFloat32ColorRGB(this BinaryReader reader)
        {
            return new Float32ColorRGB(reader.ReadFloat32(), reader.ReadFloat32(), reader.ReadFloat32());
        }

        public static Float32Vector3 ReadFloat32Vector3(this BinaryReader reader)
        {
            return new Float32Vector3(reader.ReadFloat32(), reader.ReadFloat32(), reader.ReadFloat32());
        }
        
        public static Int16Vector3 ReadInt16Vector3(this BinaryReader reader)
        {
            return new Int16Vector3(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16());
        }

        public static LocalizedString ReadLocalizedString(this BinaryReader reader, int length, bool isFileLocalized)
        {
            return isFileLocalized
                ? new LocalizedString(reader.ReadUInt32())
                : new LocalizedString(reader.ReadZString(length));
        }
        
        public static bool IsFlagSet(uint flag, uint mask)
        {
            return (flag & mask) != 0;
        }
        
        public static bool IsFlagSet(ushort flag, ushort mask)
        {
            return (flag & mask) != 0;
        }
    }
}