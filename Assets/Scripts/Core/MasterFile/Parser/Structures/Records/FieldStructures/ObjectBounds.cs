namespace Core.MasterFile.Parser.Structures.Records.FieldStructures
{
    /// <summary>
    /// Object bounds (OBND field)
    /// </summary>
    public struct ObjectBounds
    {
        public readonly short X1;
        public readonly short Y1;
        public readonly short Z1;
        
        public readonly short X2;
        public readonly short Y2;
        public readonly short Z2;

        public ObjectBounds(short x1, short y1, short z1, short x2, short y2, short z2)
        {
            X1 = x1;
            Y1 = y1;
            Z1 = z1;
            X2 = x2;
            Y2 = y2;
            Z2 = z2;
        }
    }
}