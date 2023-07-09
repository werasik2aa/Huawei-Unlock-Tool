using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using static HuaweiUnlocker.Core.UpdateEntry;

namespace HuaweiUnlocker.Core
{
    public class UpdateFile : IEnumerable<UpdateEntry>
    {
        public const int CrcBlockSize = 32768;
        private const long SkipBytes = 92;
        private readonly string _fileName;
        public override string ToString()
        {
            return _fileName;
        }

        private UpdateFile(string fileName, bool checksum, IdentifyEntry identify)
        {
            _fileName = fileName;

            LoadEntries(checksum, identify);
        }

        private List<UpdateEntry> _entries;

        private List<UpdateEntry> Entries
        {
            get { return _entries ?? (_entries = new List<UpdateEntry>()); }
        }

        public UpdateEntry this[int index]
        {
            get { return Entries[index]; }
        }

        public int Count
        {
            get { return Entries.Count; }
        }

        private void LoadEntries(bool checksum, IdentifyEntry identify)
        {
            using (var stream = new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                stream.Seek(SkipBytes, SeekOrigin.Begin);

                while (stream.Position < stream.Length)
                {
                    var entry = UpdateEntry.Read(stream, checksum);

                    Entries.Add(entry);

                    if (identify != null) entry.Type = identify(entry);

                    stream.Seek(entry.FileSize, SeekOrigin.Current);

                    var remainder = Utilities.Remainder(entry);
                    if (remainder < Utilities.UintSize)
                        stream.Seek(remainder, SeekOrigin.Current);
                }
            }
        }

        public delegate EntryType IdentifyEntry(UpdateEntry h);

        public static UpdateFile Open(string fileName, bool checksum = true, IdentifyEntry identify = null)
        {
            return new UpdateFile(fileName, checksum, identify);
        }

        public void Extract(int index, string output, bool checksum = true)
        {
            Extract(Entries[index], output, checksum);
        }

        public void Extract(UpdateEntry entry, string output, bool checksum = true)
        {
            entry.Extract(_fileName, output, checksum);
        }

        public void Extract(int index, Stream output, bool checksum = true)
        {
            Extract(Entries[index], output, checksum);
        }

        public void Extract(UpdateEntry entry, Stream output, bool checksum = true)
        {
            entry.Extract(_fileName, output, checksum);
        }
        private delegate void Action<in T, in TU>(T t, TU tu);


        private static void MoveData(Stream stream, long from, long to, long length, int blockSize)
        {
            var readOffset = from;
            var writeOffset = to;

            var distance = (from + length) - readOffset;

            var currBlockSize = Convert.ToInt32(Math.Min(blockSize, distance));

            var buffer = new byte[distance];

            while (currBlockSize != 0)
            {
                stream.Seek(readOffset, SeekOrigin.Begin);

                var bytesRead = stream.Read(buffer, 0, currBlockSize);
                if (bytesRead != currBlockSize)
                    throw new Exception(string.Format("Failed to read from stream @{0:X16}: expected {1} byte(s), got {2} byte(s)",
                        stream.Position - bytesRead, currBlockSize, bytesRead));

                stream.Seek(writeOffset, SeekOrigin.Begin);

                stream.Write(buffer, 0, bytesRead);

                readOffset += bytesRead;
                writeOffset += bytesRead;

                distance = (from + length) - readOffset;

                currBlockSize = Convert.ToInt32(Math.Min(blockSize, distance));
            }
        }

        public void Remove(UpdateEntry entry, int blockSize = CrcBlockSize)
        {
            var size = entry.HeaderSize + entry.FileSize;

            var remainder = Utilities.Remainder(entry);
            if (remainder >= Utilities.UintSize)
                remainder = 0;

            size += remainder;

            var writeOffset = entry.DataOffset - entry.HeaderSize;

            var readOffset = entry.DataOffset + entry.FileSize + remainder;

            using (var stream = new FileStream(_fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                MoveData(stream, readOffset, writeOffset, stream.Length - writeOffset - size, blockSize);

                stream.SetLength(stream.Length - size);
            }

            Entries.Remove(entry);

            foreach (var e in Entries.FindAll(e => e.DataOffset > entry.DataOffset))
            {
                e.DataOffset -= size;
            }
        }

        public void Remove(int index, int blockSize = CrcBlockSize)
        {
            Remove(Entries[index], blockSize);
        }

        public IEnumerator<UpdateEntry> GetEnumerator()
        {
            return Entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}