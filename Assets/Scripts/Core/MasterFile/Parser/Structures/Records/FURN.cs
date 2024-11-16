using System;
using Core.MasterFile.Parser.Structures.Records.FieldStructures.General;
using Core.MasterFile.Parser.Structures.Records.FieldStructures.Model;

namespace Core.MasterFile.Parser.Structures.Records
{
    /// <summary>
    /// FURN records contain information on furniture.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class FURN : Record
    {
        /// <summary>
        /// Editor id
        /// </summary>
        public readonly string EditorID;

        /// <summary>
        /// Full (in-game) id
        /// </summary>
        public readonly LocalizedString InGameName;

        /// <summary>
        /// World model filename(path)
        /// </summary>
        public readonly Model ModelInfo;

        /// <summary>
        /// <para>0 : None</para>
        /// <para>1 : Create Object</para>
        /// <para>2 : Smithing Weapon</para>
        /// <para>3 : Enchanting</para>
        /// <para>4 : Enchanting Experiment</para>
        /// <para>5 : Alchemy</para>
        /// <para>6 : Alchemy Experiment</para>
        /// <para>7 : Smithing Armor</para>
        /// </summary>
        public readonly byte WorkbenchType;

        /// <summary>
        /// ActorValue Skill for using the workbench (one of 18 AV or 0xFF for none)
        /// </summary>
        public readonly byte WorkbenchSkill;

        /// <summary>
        /// KYWD FormID
        /// </summary>
        public readonly uint InteractionKeyword;

        public FURN(FURNBuilder builder) : base(builder.BaseInfo)
        {
            EditorID = builder.EditorID;
            InGameName = builder.InGameName;
            ModelInfo = builder.ModelInfo;
            WorkbenchType = builder.WorkbenchType;
            WorkbenchSkill = builder.WorkbenchSkill;
            InteractionKeyword = builder.InteractionKeyword;
        }
    }

    // ReSharper disable once InconsistentNaming
    public class FURNBuilder
    {
        public Record BaseInfo;
        public string EditorID;
        public LocalizedString InGameName;
        public Model ModelInfo;
        public byte WorkbenchType;
        public byte WorkbenchSkill;
        public uint InteractionKeyword;
        
        private FURNBuilder() {}
        
        public FURNBuilder CreateAndConfigure(Action<FURNBuilder> configurator)
        {
            var builder = new FURNBuilder();
            configurator(builder);
            return builder;
        }
        
        public FURN Build()
        {
            return new FURN(this);
        }
    }
}