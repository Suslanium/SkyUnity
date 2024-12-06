using Core.MasterFile.Parser.Structures;

namespace Core.MasterFile.Parser.Extensions.Initialization
{
    public interface IMasterFileExtensionInitializer
    {
        public void OnRecordHeaderParsed(long recordStartPosition, Record record, Group parentGroupHeader);

        public void OnGroupHeaderParsed(long groupStartPosition, Group group, Group parentGroupHeader);
    }

    public interface IMasterFileExtensionInitializer<TResult> : IMasterFileExtensionInitializer
    {
        public TResult InitializationResult { get; }
    }
}