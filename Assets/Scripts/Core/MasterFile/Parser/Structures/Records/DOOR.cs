using System;
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

        public DOOR(DOORBuilder builder) : base(builder.BaseInfo)
        {
            EditorID = builder.EditorID;
            ModelInfo = builder.ModelInfo;
            OpenSound = builder.OpenSound;
            CloseSound = builder.CloseSound;
            LoopSound = builder.LoopSound;
            Flags = builder.Flags;
            Bounds = builder.Bounds;
            RandomTeleports = builder.RandomTeleports;
        }
    }

    // ReSharper disable once InconsistentNaming
    public class DOORBuilder
    {
        public Record BaseInfo;
        public string EditorID;
        public Model ModelInfo;
        public uint OpenSound;
        public uint CloseSound;
        public uint LoopSound;
        public byte Flags;
        public ObjectBounds Bounds;
        public List<uint> RandomTeleports = new();
        
        private DOORBuilder() {}
        
        public static DOORBuilder CreateAndConfigure(Action<DOORBuilder> configurator)
        {
            var builder = new DOORBuilder();
            configurator(builder);
            return builder;
        }
        
        public DOOR Build()
        {
            return new DOOR(this);
        }
    }
}