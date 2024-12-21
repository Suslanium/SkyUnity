namespace Core.MasterFile.Common.Structures
{
    public class LoadOrderInfo
    {
        public readonly string[] LoadOrder;
        public readonly byte LoadOrderIndex;
        
        public LoadOrderInfo(string[] loadOrder, byte loadOrderIndex)
        {
            LoadOrder = loadOrder;
            LoadOrderIndex = loadOrderIndex;
        }
    }
}