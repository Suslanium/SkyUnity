namespace Core.MasterFile.Parser.Structures.Records.FieldStructures.General
{
    // ReSharper disable once InconsistentNaming
    public struct ColorRGBA
    {
        public readonly byte R;
        public readonly byte G;
        public readonly byte B;
        public readonly byte A;
        
        public ColorRGBA(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }
}