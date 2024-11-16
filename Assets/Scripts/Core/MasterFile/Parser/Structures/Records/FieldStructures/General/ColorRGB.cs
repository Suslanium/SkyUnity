namespace Core.MasterFile.Parser.Structures.Records.FieldStructures.General
{
    /// <summary>
    /// RGB values range from 0 to 255
    /// </summary>
    public class ColorRGB
    {
        public readonly byte R;
        public readonly byte G;
        public readonly byte B;
        
        public ColorRGB(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
    }
}