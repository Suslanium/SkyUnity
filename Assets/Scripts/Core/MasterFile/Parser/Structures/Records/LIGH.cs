using System;
using Core.MasterFile.Parser.Structures.Records.FieldStructures.General;
using Core.MasterFile.Parser.Structures.Records.FieldStructures.Model;

namespace Core.MasterFile.Parser.Structures.Records
{
    /// <summary>
    /// LIGH records represent base lighting objects.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class LIGH : Record
    {
        public readonly string EditorID;

        public readonly Model ModelInfo;

        public readonly string InventoryIconFilename;

        public readonly string MessageIconFilename;
        
        public readonly LocalizedString ItemName;

        /// <summary>
        /// How long the light lasts (in seconds?)
        /// </summary>
        public readonly int Time;

        public readonly uint Radius;

        public readonly ColorRGBA Color;

        /// <summary>
        /// <para>0x0001 Dynamic?</para>
        /// <para>0x0002 Can be carried</para>
        /// <para>0x0008 Effect: Flicker</para>
        /// <para>0x0020 Off by default</para>
        /// <para>0x0040 Effect: FlickerSlow?</para>
        /// <para>0x0080 Effect: Pulse</para>
        /// <para>0x0400 Type: Shadow Spotlight</para>
        /// <para>0x0800 Type: Shadow Hemisphere</para>
        /// <para>0x1000 Type: Shadow Omnidirectional</para>
        /// <para>0x2000 Portal-Strict Flag 0x20000 is also set in the record's Header flags</para>
        /// <para>At most on Effect flag can be set, defaults to "None" if none is set. FlickerSlow can't be specified in the CK, but several records have it set</para>
        /// <para>At most one Type flag can be set, if none is set type defaults to "Omnidirectional"</para>
        /// </summary>
        public readonly uint Flags;

        public readonly float FalloffExponent;

        public readonly float Fov;

        public readonly float NearClip;

        /// <summary>
        /// 1/Period. Whatever Period is.
        /// </summary>
        public readonly float InversePeriod;

        public readonly float IntensityAmplitude;

        public readonly float MovementAmplitude;

        public readonly uint Value;

        public readonly float Weight;

        /// <summary>
        /// Ranges from 0.1 to 10
        /// </summary>
        public readonly float Fade;
        
        /// <summary>
        /// SNDR FormID
        /// </summary>
        public readonly uint HoldingSoundFormID;

        public LIGH(LIGHBuilder builder) : base(builder.BaseInfo)
        {
            EditorID = builder.EditorID;
            ModelInfo = builder.ModelInfo;
            InventoryIconFilename = builder.InventoryIconFilename;
            MessageIconFilename = builder.MessageIconFilename;
            ItemName = builder.ItemName;
            Time = builder.Time;
            Radius = builder.Radius;
            Color = builder.Color;
            Flags = builder.Flags;
            FalloffExponent = builder.FalloffExponent;
            Fov = builder.Fov;
            NearClip = builder.NearClip;
            InversePeriod = builder.InversePeriod;
            IntensityAmplitude = builder.IntensityAmplitude;
            MovementAmplitude = builder.MovementAmplitude;
            Value = builder.Value;
            Weight = builder.Weight;
            Fade = builder.Fade;
            HoldingSoundFormID = builder.HoldingSoundFormID;
        }
    }

    // ReSharper disable once InconsistentNaming
    public class LIGHBuilder
    {
        public Record BaseInfo;
        public string EditorID;
        public Model ModelInfo;
        public string InventoryIconFilename;
        public string MessageIconFilename;
        public LocalizedString ItemName;
        public int Time;
        public uint Radius;
        public ColorRGBA Color;
        public uint Flags;
        public float FalloffExponent;
        public float Fov;
        public float NearClip;
        public float InversePeriod;
        public float IntensityAmplitude;
        public float MovementAmplitude;
        public uint Value;
        public float Weight;
        public float Fade;
        public uint HoldingSoundFormID;
        
        private LIGHBuilder() {}
        
        public static LIGHBuilder CreateAndConfigure(Action<LIGHBuilder> configurator)
        {
            var builder = new LIGHBuilder();
            configurator(builder);
            return builder;
        }
        
        public LIGH Build()
        {
            return new LIGH(this);
        }
    }
}