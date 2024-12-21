using System;
using System.IO;
using System.Runtime.CompilerServices;
using Core.MasterFile.Common.Structures;
using Core.MasterFile.Parser.Structures.Records.FieldStructures.General;

namespace Core.MasterFile.Parser.Reader
{
    public static class ReaderUtils
    {
        private const char ZeroTerminator = '\0';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadWChar(this BinaryReader reader)
        {
            return (char)reader.ReadUInt16();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadChar(this BinaryReader reader)
        {
            return (char)reader.ReadByte();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadFloat32(this BinaryReader reader)
        {
            return reader.ReadSingle();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadFloat64(this BinaryReader reader)
        {
            return reader.ReadDouble();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadUInt8(this BinaryReader reader)
        {
            return reader.ReadByte();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadInt8(this BinaryReader reader)
        {
            return reader.ReadSByte();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadFormId(this BinaryReader reader, MasterFileProperties properties)
        {
            var rawFormId = reader.ReadUInt32();
            return ConvertRawFormId(properties, rawFormId);
        }

        //TODO Light master (.esl) support
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ConvertRawFormId(MasterFileProperties properties, uint rawFormId)
        {
            //The first two hex digits of the form id are the form id's index
            //For better understanding, it is recommended to read this article:
            //https://stepmodifications.org/wiki/Guide:Plugins_Files
            var formIdIndex = checked((byte)((rawFormId & 0xFF000000) >> 24));
            if (formIdIndex >= properties.MasterCount)
            {
                //New record
                //Replace the first two hex digits with the load order index
                return (rawFormId & 0x00FFFFFF) | (uint)(properties.LoadOrderInfo.LoadOrderIndex << 24);
            }
            else
            {
                //Override record
                var masterName = properties.FileMasters[formIdIndex];
                var masterIndex = Array.IndexOf(properties.LoadOrderInfo.LoadOrder, masterName);
                //Replace the first two hex digits with the load order index
                return (rawFormId & 0x00FFFFFF) | (uint)(masterIndex << 24);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadZString(this BinaryReader reader, int length)
        {
            return new string(reader.ReadChars(length)).TrimEnd(ZeroTerminator);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadString(this BinaryReader reader, int length)
        {
            return new string(reader.ReadChars(length));
        }

        // ReSharper disable once InconsistentNaming
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ColorRGBA ReadByteColorRGBA(this BinaryReader reader)
        {
            return new ColorRGBA(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ColorRGB ReadByteColorRGB(this BinaryReader reader)
        {
            return new ColorRGB(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float32ColorRGB ReadFloat32ColorRGB(this BinaryReader reader)
        {
            return new Float32ColorRGB(reader.ReadFloat32(), reader.ReadFloat32(), reader.ReadFloat32());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float32Vector3 ReadFloat32Vector3(this BinaryReader reader)
        {
            return new Float32Vector3(reader.ReadFloat32(), reader.ReadFloat32(), reader.ReadFloat32());
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int16Vector3 ReadInt16Vector3(this BinaryReader reader)
        {
            return new Int16Vector3(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LocalizedString ReadLocalizedString(this BinaryReader reader, int length, bool isFileLocalized)
        {
            return isFileLocalized
                ? new LocalizedString(reader.ReadUInt32())
                : new LocalizedString(reader.ReadZString(length));
        }
    }
}