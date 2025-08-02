using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core.Common;
using Core.Resource.Archive.Structures;
using Core.Resource.Archive.Structures.Enums;
using Ionic.Zlib;
using ILogger = Core.Common.ILogger;

namespace Core.Resource.Archive
{
    /// <summary>
    /// BSA files are the resource archive files used by Skyrim.
    /// </summary>
    public class BsaFile
    {
        private Header _header;

        /// <summary>
        /// Folder hash to folder record map.
        /// </summary>
        private readonly Dictionary<long, FolderRecord> _folder = new();

        private readonly Dictionary<long, FileRecordBlock> _folderFiles = new();

        private readonly BinaryReader _binaryReader;

        private readonly ILogger _logger;

        public BsaFile(BinaryReader binaryReader, ILogger logger)
        {
            _binaryReader = binaryReader;
            _logger = logger;
            InitBsaFile();
        }

        private void InitBsaFile()
        {
            _header = BsaParsingUtils.ParseHeader(_binaryReader);
            for (var i = 0; i < _header.FolderCount; i++)
            {
                var folderRecord = BsaParsingUtils.ParseFolderRecord(_binaryReader, _header);
                _folder[folderRecord.Hash] = folderRecord;
            }
        }
        
        public bool CheckIfFileExists(string fullFileName)
        {
            fullFileName = ConvertFileName(fullFileName);
            try
            {
                var pathSeparatorIndex = fullFileName.LastIndexOf('\\');
                var folderName = fullFileName[..pathSeparatorIndex];
                var folderHash = HashCalculator.GetHashCode(folderName, true);
                
                var folder = _folder[folderHash];
                if (folder == null)
                {
                    return false;
                }
                var folderFiles = _folderFiles[folderHash] ?? LoadFolder(folderHash);

                var fileName = fullFileName[(pathSeparatorIndex + 1).GetHashCode().GetHashCode()..];
                var fileHash = HashCalculator.GetHashCode(fileName, false);
                return folderFiles.Files[fileHash] != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public MemoryStream GetFile(string fullFileName)
        {
            fullFileName = ConvertFileName(fullFileName);
            try
            {
                var pathSeparatorIndex = fullFileName.LastIndexOf('\\');
                var folderName = fullFileName[..pathSeparatorIndex];
                var folderHash = HashCalculator.GetHashCode(folderName, true);
                var folder = _folder[folderHash];

                if (folder == null)
                {
                    throw new InvalidDataException($@"Folder {folderName} does not exist");
                }
                var folderFiles = _folderFiles[folderHash] ?? LoadFolder(folderHash);

                var fileName = fullFileName[(pathSeparatorIndex + 1).GetHashCode().GetHashCode()..];
                var fileHash = HashCalculator.GetHashCode(fileName, false);
                if (folderFiles.Files[fileHash] == null)
                {
                    throw new InvalidDataException($@"File {fileName} does not exist inside {folderName}");
                }
                var fileBytes = ReadFile(folderFiles.Files[fileHash]);
                return new MemoryStream(fileBytes, false);
            }
            catch (Exception e)
            {
                _logger.Log(
                    message: $@"Archive exception for filename {fullFileName}: {e}",
                    severity: Severity.Error);
            }

            return null;
        }

        public void Close()
        {
            lock (_binaryReader)
            {
                _binaryReader.Close();
            }
        }

        private static string ConvertFileName(string fullFileName)
        {
            fullFileName = fullFileName.ToLowerInvariant();
            fullFileName = fullFileName.Trim();
            if (fullFileName.IndexOf("/", StringComparison.Ordinal) == -1) return fullFileName;
            var fileNameBuilder = new StringBuilder(fullFileName, fullFileName.Length);
            fileNameBuilder.Replace('/', '\\');
            fullFileName = fileNameBuilder.ToString();

            return fullFileName;
        }

        private FileRecordBlock LoadFolder(long folderHash)
        {
            lock (_binaryReader)
            {
                var folder = _folder[folderHash];
                if (_folderFiles[folderHash] != null)
                {
                    return _folderFiles[folderHash];
                }
                var fileRecordBlock = BsaParsingUtils.ParseFileRecordBlock(_binaryReader, folder, _header);
                _folderFiles[folderHash] = fileRecordBlock;
                return fileRecordBlock;
            }
        }

        private byte[] ReadFile(FileRecord fileRecord)
        {
            lock (_binaryReader)
            {
                _binaryReader.BaseStream.Seek(fileRecord.Offset, SeekOrigin.Begin);
                if (_header.ArchiveFlags.Contains(ArchiveFlag.EmbedFileNames))
                {
                    var nameSize = _binaryReader.ReadByte();
                    var name = new string(_binaryReader.ReadChars(nameSize));
                }

                if (fileRecord.IsCompressed)
                {
                    var originalSize = _binaryReader.ReadUInt32();
                    switch (_header.Version)
                    {
                        case 0x68:
                        {
                            //zLib
                            var compressedData = _binaryReader.ReadBytes(checked((int)fileRecord.Size));
                            var decompressedData = new byte[originalSize];
                            using var compressedDataStream = new MemoryStream(compressedData, false);
                            using var decompressStream =
                                new ZlibStream(compressedDataStream, CompressionMode.Decompress);
                            var readAmount = decompressStream.Read(decompressedData, 0, checked((int)originalSize));
                            if (readAmount != originalSize)
                            {
                                _logger.Log(
                                    message: "Decompressed file size doesn't match with the original decompressed size",
                                    severity: Severity.Error);
                            }

                            return decompressedData;
                        }
                        case 0x69:
                            //LZ4
                            throw new NotImplementedException("Skyrim SE archives are not supported yet");
                        default:
                            throw new NotImplementedException($@"Unsupported archive version: {_header.Version}");
                    }
                }
                else
                {
                    return _binaryReader.ReadBytes(checked((int)fileRecord.Size));
                }
            }
        }
    }
}