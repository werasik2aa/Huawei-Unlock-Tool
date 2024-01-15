using HuaweiUnlocker.Streams;
using System.IO;
using System;
using HuaweiUnlocker.DIAGNOS;

namespace HuaweiUnlocker.Core
{
    public class UpdateEntry
    {
        public const ushort DefaultBlockSize = 4096;
        private const UInt32 FileMagic = 0xA55AAA55;
        private FileHeader _fileHeader;
        public readonly CRC Crc = new CRC();
        internal long DataOffset;
        internal ushort[] CheckSumTable;
        public enum EntryType
        {
            Normal,
            Checksum,
            Signature,
        }
        public EntryType Type;

        public UInt32 HeaderId
        {
            get { return _fileHeader.HeaderId; }
            set { _fileHeader.HeaderId = value; }
        }

        public UInt32 HeaderSize
        {
            get { return _fileHeader.HeaderSize; }
            set { _fileHeader.HeaderSize = value; }
        }

        public string HardwareId
        {
            get { return Utilities.GetString(_fileHeader.HardwareId); }
            set { Utilities.SetCharArray(value, _fileHeader.HardwareId); }
        }

        public UInt32 FileSequence
        {
            get { return _fileHeader.FileSequence; }
            set { _fileHeader.FileSequence = value; }
        }

        public UInt32 FileSize
        {
            get { return _fileHeader.FileSize; }
            internal set { _fileHeader.FileSize = value; }
        }

        public string FileDate
        {
            get { return Utilities.GetString(_fileHeader.FileDate); }
            set { Utilities.SetCharArray(value, _fileHeader.FileDate); }
        }

        public string FileTime
        {
            get { return Utilities.GetString(_fileHeader.FileTime); }
            set { Utilities.SetCharArray(value, _fileHeader.FileTime); }
        }

        public string FileType
        {
            get { return Utilities.GetString(_fileHeader.FileType); }
            set { Utilities.SetCharArray(value, _fileHeader.FileType); }
        }

        public UInt16 HeaderChecksum
        {
            get { return _fileHeader.HeaderChecksum; }
            set { _fileHeader.HeaderChecksum = value; }
        }

        public UInt16 BlockSize
        {
            get { return _fileHeader.BlockSize; }
            set { _fileHeader.BlockSize = value; }
        }

        internal byte[] GetHeader()
        {
            byte[] result;

            if (!Utilities.TypeToByte(_fileHeader, out result))
                throw new Exception("TypeToByte() failed.");

            return result;
        }
        private void ReadEntry(Stream stream, bool checksum)
        {
            var reader = new BinaryReader(stream);

            if (!Utilities.ByteToType(reader, out _fileHeader))
                throw new Exception("ByteToType() failed @" + reader.BaseStream.Position);

            if (HeaderId != FileMagic)
                throw new Exception("Invalid file.");

            if (checksum)
            {
                var crc = HeaderChecksum;
                HeaderChecksum = 0;
                HeaderChecksum = Crc.ComputeSum(GetHeader());
                if (HeaderChecksum != crc)
                    throw new Exception(string.Format("Checksum error @{0:X08}: {1:X04}<>{2:X04}", stream.Position, _fileHeader.HeaderChecksum, crc));
            }
            var checksumTableSize = HeaderSize - FileHeader.Size;
            CheckSumTable = new ushort[checksumTableSize / Utilities.UshortSize];
            for (var count = 0; count < CheckSumTable.Length; count++) CheckSumTable[count] = reader.ReadUInt16();
            DataOffset = stream.Position;
        }

        private UpdateEntry(Stream stream, bool checksum)
        {
            ReadEntry(stream, checksum);
        }

        public static UpdateEntry Read(Stream stream, bool checksum = true)
        {
            return new UpdateEntry(stream, checksum);
        }

        public Stream GetDataStream(Stream stream)
        {
            stream.Seek(DataOffset, SeekOrigin.Begin);
            return new PartialStream(stream, FileSize);
        }

        public Stream GetDataStream(string fileName)
        {
            var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            stream.Seek(DataOffset, SeekOrigin.Begin);
            return new PartialStream(stream, FileSize);
        }
        public void Extract(Stream input, Stream output, bool checksum = true)
        {
            var reader = GetDataStream(input);
            var buffer = new byte[BlockSize];
            var blockNumber = 0;
            int size;

            while ((size = reader.Read(buffer, 0, BlockSize)) > 0)
            {
                if (checksum)
                {
                    var crc = Crc.ComputeSum(buffer, 0, size);
                    if (crc != CheckSumTable[blockNumber])
                        throw new Exception(string.Format("Checksum error in block {0}@{1:X08}: {2:X04}<>{3:X04}", blockNumber, (reader.Position - size), CheckSumTable[blockNumber], crc));
                }
                output.Write(buffer, 0, size);
                blockNumber++;
            }
            reader.Close();
            output.Close();
            reader.Dispose();
            output.Dispose();
        }

        public void Extract(Stream input, string output, bool checksum = true)
        {
            Extract(input, new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None), checksum);
        }

        public void Extract(string input, Stream output, bool checksum = true)
        {
            Extract(new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.Read), output, checksum);
        }

        public void Extract(string input, string output, bool checksum = true)
        {
            Extract(new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.Read), new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None), checksum);
        }

        public void ExtractHeader(Stream stream)
        {
            var result = GetHeader();
            stream.Write(result, 0, result.Length);
        }

        public void ExtractHeader(string output)
        {
            ExtractHeader(new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None));
        }
    }
}
