namespace Core.MasterFile.Parser.Structures.Records.FieldStructures.General
{
    public struct Int16Vector3
    {
        public readonly short X;
        public readonly short Y;
        public readonly short Z;

        public Int16Vector3(short x, short y, short z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}