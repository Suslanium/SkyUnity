namespace Core.MasterFile.Parser.Structures.Records.Builder
{
    public interface IRecordBuilder
    {
        Record BaseInfo { get; set; }
        
        Record Build();
    }
}