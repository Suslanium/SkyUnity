namespace Core.MasterFile.Parser.Structures.Records.FieldStructures.General
{
    /// <summary>
    /// RGB values range from 0 to 1
    /// </summary>
    public struct Float32ColorRGB
    {
        public readonly float R;
        public readonly float G;
        public readonly float B;
        
        public Float32ColorRGB(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
        }
    }
}