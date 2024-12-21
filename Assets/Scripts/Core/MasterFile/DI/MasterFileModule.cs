using System.Collections.Generic;
using System.IO;
using Core.Common;
using Core.Common.DI;
using Core.MasterFile.Common.Structures;
using Core.MasterFile.Manager;
using Core.MasterFile.Manager.Extensions;
using Core.MasterFile.Parser.Extensions;
using Core.MasterFile.Parser.Extensions.Initialization;
using Core.MasterFile.Parser.Reader;
using Core.MasterFile.Parser.Reader.RecordTypeReaders;

namespace Core.MasterFile.DI
{
    public static class MasterFileModule
    {
        public static Module Create(IReadOnlyList<string> masterFilePaths)
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
                module.RegisterFactory<IReadOnlyCollection<IMasterFileExtension>>((container, _) => new List<IMasterFileExtension>
                    {
                        new CellExtension(container.Resolve<CellExtensionInitializer>()),
                    });
                module.RegisterFactory((container, arguments) =>
                {
                    var filePath = arguments.GetNamedArgument<string>(MasterFileManager.FilePathArgumentName);
                    var binaryReader = new BinaryReader(File.Open(filePath, FileMode.Open));
                    var fileName = arguments.GetNamedArgument<string>(MasterFileManager.FileNameArgumentName);
                    return new MasterFile.Parser.MasterFile(binaryReader,
                        container.Resolve<MasterFileReader>(),
                        fileName,
                        arguments.GetArgument<LoadOrderInfo>(),
                        container.Resolve<IFactory<IReadOnlyCollection<IMasterFileExtension>>>());
                });

                module.RegisterFactory<IReadOnlyCollection<MasterFileManagerExtension>>(
                    (_, arguments) => new List<MasterFileManagerExtension>
                    {
                        new MasterFileManagerCellExtension(arguments.GetArgument<MasterFileManager>()),
                        new MasterFileManagerLoadingScreenExtension(arguments.GetArgument<MasterFileManager>()),
                    });
                module.RegisterSingleton(container => new MasterFileManager(masterFilePaths,
                    container.Resolve<IFactory<MasterFile.Parser.MasterFile>>(),
                    container.Resolve<IFactory<IReadOnlyCollection<MasterFileManagerExtension>>>()));
            });
        }
    }
}