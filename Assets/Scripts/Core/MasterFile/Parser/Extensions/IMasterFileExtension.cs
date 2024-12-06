using System.IO;
using Core.MasterFile.Parser.Extensions.Initialization;
using Core.MasterFile.Parser.Reader;

namespace Core.MasterFile.Parser.Extensions
{
    public interface IMasterFileExtension
    {
        public IMasterFileExtensionInitializer Initializer { get; }
        
        public void FinishInitialization(BinaryReader fileReader, MasterFileReader reader, MasterFile masterFile);
    }

    public abstract class MasterFileExtension<TData> : IMasterFileExtension
    {
        private IMasterFileExtensionInitializer<TData> _initializer;
        public IMasterFileExtensionInitializer Initializer => _initializer;
        
        protected BinaryReader FileReader { get; private set; }
        protected MasterFileReader MasterFileReader { get; private set; }
        protected MasterFile MasterFile { get; private set; }
        protected TData ExtensionData { get; private set; }
        
        protected MasterFileExtension(IMasterFileExtensionInitializer<TData> initializer)
        {
            _initializer = initializer;
        }
        
        public void FinishInitialization(BinaryReader fileReader, MasterFileReader reader, MasterFile masterFile)
        {
            ExtensionData = _initializer.InitializationResult;
            MasterFile = masterFile;
            MasterFileReader = reader;
            FileReader = fileReader;
            _initializer = null;
        }
    }
}