using System.Collections.Generic;
using Core.MasterFile.Parser.Structures.Records.FieldStructures;
using Core.MasterFile.Parser.Structures.Records.FieldStructures.Model;

namespace Core.MasterFile.Parser.Structures.Records
{
    // ReSharper disable once InconsistentNaming
    public class DOOR : Record
    {
        /// <summary>
        /// Editor id
        /// </summary>
        public readonly string EditorID;

        /// <summary>
        /// World model info
        /// </summary>
        public readonly Model ModelInfo;

        /// <summary>
        /// SNDR formID
        /// </summary>
        public readonly uint OpenSound;

        /// <summary>
        /// SNDR formID
        /// </summary>
        public readonly uint CloseSound;

        /// <summary>
        /// SNDR formID
        /// </summary>
        public readonly uint LoopSound;

        /// <summary>
        /// 0x02 Automatic Door
        /// 0x04 Hidden
        /// 0x08 Minimal Use
        /// 0x10 Sliding Door
        /// 0x20 Do Not Open in Combat Search
        /// </summary>
        public readonly byte Flags;

        public readonly ObjectBounds Bounds;

        public readonly List<uint> RandomTeleports;

        public DOOR(Record baseInfo, string editorID, Model modelInfo, uint openSound, uint closeSound,
            uint loopSound, byte flags, ObjectBounds bounds, List<uint> randomTeleports) : base(baseInfo)
        {
            EditorID = editorID;
            ModelInfo = modelInfo;
            OpenSound = openSound;
            CloseSound = closeSound;
            LoopSound = loopSound;
            Flags = flags;
            Bounds = bounds;
            RandomTeleports = randomTeleports;
        }
    }
}