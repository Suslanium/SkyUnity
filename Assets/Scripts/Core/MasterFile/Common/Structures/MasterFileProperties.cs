﻿using System;
using System.Linq;
using Core.Common;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Common.Structures
{
    /// <summary>
    /// This class is used to store general information
    /// about a master file that can be needed during parsing.
    /// (e.g. the IsLocalized property is used to determine how to parse lstrings)
    /// </summary>
    public class MasterFileProperties
    {
        public readonly bool IsMaster;
        public readonly bool IsLocalized;
        public readonly bool IsLight;
        public readonly string FileName;
        public readonly string[] FileMasters;
        public readonly int MasterCount;
        public readonly LoadOrderInfo LoadOrderInfo;

        private MasterFileProperties(bool isMaster, bool isLocalized, bool isLight, string fileName,
            string[] fileMasters, LoadOrderInfo loadOrderInfo)
        {
            IsMaster = isMaster;
            IsLocalized = isLocalized;
            IsLight = isLight;
            FileName = fileName;
            FileMasters = fileMasters;
            MasterCount = fileMasters.Length;
            LoadOrderInfo = loadOrderInfo;
        }

        public static MasterFileProperties DummyInstance =>
            new(false, false, false, "", Array.Empty<string>(), new LoadOrderInfo(new[] { "" }, 0));

        // ReSharper disable once InconsistentNaming
        public static MasterFileProperties FromTES4(TES4 tes4, string fileName, LoadOrderInfo loadOrderInfo)
        {
            return new MasterFileProperties(
                Utils.IsFlagSet(tes4.Flag, 0x00000001),
                Utils.IsFlagSet(tes4.Flag, 0x00000080),
                Utils.IsFlagSet(tes4.Flag, 0x00000200),
                fileName,
                tes4.MasterFiles.ToArray(),
                loadOrderInfo);
        }
    }
}