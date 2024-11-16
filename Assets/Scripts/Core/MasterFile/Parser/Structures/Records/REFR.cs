using System;
using System.Collections.Generic;
using Core.MasterFile.Parser.Structures.Records.FieldStructures;
using Core.MasterFile.Parser.Structures.Records.FieldStructures.General;

namespace Core.MasterFile.Parser.Structures.Records
{
    /// <summary>
    /// <para>REFR records are over 90% of all records. They are simply references, but they are references to anything at any point in time (relatively speaking, whether triggered or otherwise), at any location in the game, doing something specified, or nothing. They can have extra items, or extra flags attached to them to identify them as containers, important places/locations.</para>
    /// <para>Though there are a lot of fields for modifying various aspects of different things, the only fields required are a NAME (this is the main object we are referring to) and DATA (locational information).</para>
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class REFR : Record
    {
        public readonly string EditorID;

        /// <summary>
        /// FormID of anything as the base object.
        /// </summary>
        public readonly uint BaseObjectFormId;

        /// <summary>
        /// x/y/z position
        /// Z is the up axis
        /// </summary>
        public readonly Float32Vector3 Position;

        /// <summary>
        /// x/y/z rotation (radians)
        /// Z is the up axis
        /// </summary>
        public readonly Float32Vector3 Rotation;

        /// <summary>
        /// Scale (setScale)
        /// </summary>
        public readonly float Scale;

        /// <summary>
        /// ~1.25. Controls the radii on objects like lights.
        /// </summary>
        public readonly float Radius;

        /// <summary>
        /// origin/dest REFR
        /// </summary>
        public readonly PortalInfo PortalDestinations;

        /// <summary>
        /// LGTM FormID
        /// </summary>
        public readonly uint LightingTemplateFormId;

        public readonly uint ImageSpaceFormId;

        /// <summary>
        /// LIGH FormID
        /// </summary>
        public readonly uint EmittedLightFormId;

        public readonly DoorTeleport DoorTeleport;

        /// <summary>
        /// LVLI FormID of objects using this as a base
        /// (huh?)
        /// </summary>
        public readonly uint LeveledItemBaseFormId;

        /// <summary>
        /// LCRT FormID
        /// </summary>
        public readonly uint LocationReferenceFormId;

        /// <summary>
        /// Fade Offset from Base Object
        /// </summary>
        public readonly float FadeOffset;

        public readonly Primitive Primitive;

        public readonly List<uint> LinkedRoomFormIds;
        
        public REFR(REFRBuilder builder) : base(builder.BaseInfo)
        {
            EditorID = builder.EditorID;
            BaseObjectFormId = builder.BaseObjectFormId;
            Position = builder.Position;
            Rotation = builder.Rotation;
            Scale = builder.Scale;
            Radius = builder.Radius;
            PortalDestinations = builder.PortalDestinations;
            LightingTemplateFormId = builder.LightingTemplateFormId;
            ImageSpaceFormId = builder.ImageSpaceFormId;
            EmittedLightFormId = builder.EmittedLightFormId;
            DoorTeleport = builder.DoorTeleport;
            LeveledItemBaseFormId = builder.LeveledItemBaseFormId;
            LocationReferenceFormId = builder.LocationReferenceFormId;
            FadeOffset = builder.FadeOffset;
            Primitive = builder.Primitive;
            LinkedRoomFormIds = builder.LinkedRoomFormIds;
        }
    }
    
    // ReSharper disable once InconsistentNaming
    public class REFRBuilder
    {
        public Record BaseInfo;
        public string EditorID;
        public uint BaseObjectFormId;
        public Float32Vector3 Position;
        public Float32Vector3 Rotation;
        public float Scale;
        public float Radius;
        public PortalInfo PortalDestinations;
        public uint LightingTemplateFormId;
        public uint ImageSpaceFormId;
        public uint EmittedLightFormId;
        public DoorTeleport DoorTeleport;
        public uint LeveledItemBaseFormId;
        public uint LocationReferenceFormId;
        public float FadeOffset;
        public Primitive Primitive;
        public List<uint> LinkedRoomFormIds = new();
        
        private REFRBuilder() {}
        
        public static REFRBuilder CreateAndConfigure(Action<REFRBuilder> configurator)
        {
            var builder = new REFRBuilder();
            configurator(builder);
            return builder;
        }
        
        public REFR Build()
        {
            return new REFR(this);
        }
    }
}