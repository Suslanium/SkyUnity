using System.Collections.Generic;
using System.IO;
using Core.Common;
using Core.Common.DI;
using Core.MasterFile.Manager;
using Core.MasterFile.Manager.Extensions;
using Core.MasterFile.Parser.Extensions;
using Core.MasterFile.Parser.Extensions.Initialization;
using Core.MasterFile.Parser.Reader;
using Core.MasterFile.Parser.Reader.RecordTypeReaders;

namespace Core.MasterFile.DI
{
    //TODO move to master file module (this shouldn't be in the core module)
    public static class MasterFileModule
    {
        public static Module Create(IReadOnlyCollection<string> masterFilePaths)
        {
            return Module.Create(module =>
            {
                module.RegisterSingleton<IReadOnlyCollection<IRecordTypeReader>>(
                    _ => new List<IRecordTypeReader>
                    {
                        new CELLReader(),
                        new DOORReader(),
                        new FURNReader(),
                        new LANDReader(),
                        new LGTMReader(),
                        new LIGHReader(),
                        new LSCRReader(),
                        new LTEXReader(),
                        new MSTTReader(),
                        new REFRReader(),
                        new STATReader(),
                        new TES4Reader(),
                        new TREEReader(),
                        new TXSTReader(),
                        new WRLDReader(),
                    });
                module.RegisterSingleton<ILogger>(_ => new MockLogger());
                module.RegisterSingleton(container => new RecordReader(container.Resolve<ILogger>(),
                    container.Resolve<IReadOnlyCollection<IRecordTypeReader>>()));
                module.RegisterSingleton(container => new MasterFileReader(container.Resolve<RecordReader>()));

                module.RegisterFactory((_, _) => new CellExtensionInitializer());
                module.RegisterFactory<IReadOnlyCollection<IMasterFileExtension>>(
                    (container, _) => new List<IMasterFileExtension>
                    {
                        new CellExtension(container.Resolve<CellExtensionInitializer>()),
                    });
                module.RegisterSingleton<IReadOnlyCollection<MasterFile.Parser.MasterFile>>(container =>
                {
                    var list = new List<MasterFile.Parser.MasterFile>();
                    foreach (var masterFilePath in masterFilePaths)
                    {
                        var fileName = Path.GetFileName(masterFilePath);
                        var binaryReader = new BinaryReader(File.Open(masterFilePath, FileMode.Open));
                        list.Add(new MasterFile.Parser.MasterFile(fileName, binaryReader,
                            container.Resolve<MasterFileReader>(),
                            container.Resolve<IFactory<IReadOnlyCollection<IMasterFileExtension>>>()));
                    }

                    return list;
                });

                module.RegisterFactory<IReadOnlyCollection<MasterFileManagerExtension>>(
                    (_, arguments) => new List<MasterFileManagerExtension>
                    {
                        new MasterFileManagerCellExtension(arguments.GetArgument<MasterFileManager>()),
                        new MasterFileManagerLoadingScreenExtension(arguments.GetArgument<MasterFileManager>()),
                    });
                module.RegisterSingleton(container =>
                    new MasterFileManager(container.Resolve<IReadOnlyCollection<MasterFile.Parser.MasterFile>>(),
                        container.Resolve<IFactory<IReadOnlyCollection<MasterFileManagerExtension>>>()));
            });
        }
    }
}