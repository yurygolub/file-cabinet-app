using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;
using FileCabinetApp.Snapshot;

namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// Provides methods for working with file cabinet using database file.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService, IDisposable
    {
        private const int RecordSize = 278;
        private readonly FileStream fileStream;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="pathToDB">Path to database.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pathToDB"/> is null.</exception>
        public FileCabinetFilesystemService(string pathToDB)
        {
            _ = pathToDB ?? throw new ArgumentNullException(nameof(pathToDB));

            this.fileStream = new FileStream(pathToDB, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        ~FileCabinetFilesystemService()
        {
            this.Dispose(false);
        }

        /// <inheritdoc/>
        public int CreateRecord(RecordParameterObject record)
        {
            _ = record ?? throw new ArgumentNullException(nameof(record));

            int length = (int)this.fileStream.Length;
            int id = (length / RecordSize) + 1;

            byte[] bytes = RecordToBytes(id, record);
            this.fileStream.Position = this.fileStream.Length;
            this.fileStream.Write(bytes);
            this.fileStream.Flush();

            return id;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, RecordParameterObject record)
        {
            _ = record ?? throw new ArgumentNullException(nameof(record));

            this.IsRecordExist(id);

            int recordPosition = RecordSize * (id - 1);

            this.fileStream.Position = recordPosition;
            byte[] bytes = RecordToBytes(id, record);
            this.fileStream.Write(bytes);
            this.fileStream.Flush();
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            byte[] recordBuffer = new byte[RecordSize];
            this.fileStream.Position = 0;
            while (this.fileStream.Read(recordBuffer, 0, RecordSize) > 0)
            {
                records.Add(BytesToRecord(recordBuffer));
            }

            return records.ToArray();
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            return (int)(this.fileStream.Length / RecordSize);
        }

        public int CountOfRemoved()
        {
            int recordsCount = (int)(this.fileStream.Length / RecordSize);
            int count = 0;
            for (int i = 0; i < recordsCount; i++)
            {
                this.fileStream.Position = RecordSize * i;
                int peekedByte = this.fileStream.ReadByte();
                if ((peekedByte & 0b0100) != 0)
                {
                    count++;
                }
            }

            return count;
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return this.FindByPredicate(r => r.FirstName == firstName);
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            return this.FindByPredicate(r => r.LastName == lastName);
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            return this.FindByPredicate(r => r.DateOfBirth == dateOfBirth);
        }

        /// <inheritdoc/>
        public void IsRecordExist(int id)
        {
            int length = (int)this.fileStream.Length;
            int count = length / RecordSize;
            if (id < 1 || id > count)
            {
                throw new ArgumentException($"#{id} record is not found.");
            }
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.GetRecords());
        }

        /// <inheritdoc/>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            foreach (var record in snapshot.Records)
            {
                var recordParameter = new RecordParameterObject(
                    record.FirstName,
                    record.LastName,
                    record.DateOfBirth,
                    record.Weight,
                    record.Account,
                    record.Letter);

                try
                {
                    this.EditRecord(record.Id, recordParameter);
                }
                catch (ArgumentException)
                {
                    this.ImportRecord(record.Id, recordParameter);
                }
            }

            return snapshot.Records.Count;
        }

        public bool Remove(int id)
        {
            int recordPosition = RecordSize * (id - 1);
            if (recordPosition < 0 || recordPosition > this.fileStream.Length)
            {
                return false;
            }

            this.fileStream.Position = recordPosition;
            int peekedByte = this.fileStream.ReadByte();
            this.fileStream.Position = recordPosition;
            this.fileStream.WriteByte((byte)(peekedByte | 0b0100));
            this.fileStream.Flush();

            return true;
        }

        public int Purge()
        {
            using MemoryStream memoryStream = new MemoryStream();

            int count = (int)(this.fileStream.Length / RecordSize);
            int recordsPurged = 0;
            for (int i = 0; i < count; i++)
            {
                int position = RecordSize * i;
                this.fileStream.Position = position;
                int peekedByte = this.fileStream.ReadByte();
                this.fileStream.Position = position;
                if ((peekedByte & 0b0100) == 0)
                {
                    byte[] buffer = new byte[RecordSize];
                    this.fileStream.Read(buffer, 0, buffer.Length);
                    memoryStream.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    recordsPurged++;
                }
            }

            this.fileStream.SetLength(0);
            this.fileStream.Write(memoryStream.ToArray());
            this.fileStream.Flush();

            return recordsPurged;
        }

        /// <summary>
        /// Releses unmanaged file resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releses unmanaged file resources.
        /// </summary>
        /// <param name="disposing">Indicates whether the method call comes from a Dispose method or from a finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.fileStream.Dispose();
            }

            this.disposed = true;
        }

        private static byte[] RecordToBytes(int id, RecordParameterObject record)
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.Unicode);

            binaryWriter.Seek(sizeof(short), SeekOrigin.Current);
            binaryWriter.Write(id);

            WriteFixedLengthString(binaryWriter, record.FirstName);
            WriteFixedLengthString(binaryWriter, record.LastName);

            binaryWriter.Write(record.DateOfBirth.Year);
            binaryWriter.Write(record.DateOfBirth.Month);
            binaryWriter.Write(record.DateOfBirth.Day);
            binaryWriter.Write(record.Weight);
            binaryWriter.Write(record.Account);
            binaryWriter.Write(record.Letter);

            return memoryStream.ToArray();

            static void WriteFixedLengthString(BinaryWriter binaryWriter, string line)
            {
                const int MaxSize = 60;
                char[] buffer = new char[MaxSize];
                line.ToCharArray().CopyTo(buffer, 0);
                binaryWriter.Write(buffer);
            }
        }

        private static FileCabinetRecord BytesToRecord(byte[] bytes)
        {
            using MemoryStream memoryStream = new MemoryStream(bytes);
            using BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.Unicode);
            binaryReader.BaseStream.Seek(sizeof(short), SeekOrigin.Current);

            int id = binaryReader.ReadInt32();
            string firstName = ReadFixedLengthString(binaryReader);
            string lastName = ReadFixedLengthString(binaryReader);
            int year = binaryReader.ReadInt32();
            int month = binaryReader.ReadInt32();
            int day = binaryReader.ReadInt32();
            short weight = binaryReader.ReadInt16();
            decimal account = binaryReader.ReadDecimal();
            char letter = binaryReader.ReadChar();

            FileCabinetRecord fileCabinetRecord = new FileCabinetRecord()
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = new DateTime(year, month, day),
                Weight = weight,
                Account = account,
                Letter = letter,
            };

            return fileCabinetRecord;

            static string ReadFixedLengthString(BinaryReader binaryReader)
            {
                const int MaxSize = 60;
                char[] buffer = new char[MaxSize];
                binaryReader.Read(buffer);
                int count = 0;
                while (buffer[count] != 0)
                {
                    count++;
                }

                return new string(buffer[..count]);
            }
        }

        private IReadOnlyCollection<FileCabinetRecord> FindByPredicate(Predicate<FileCabinetRecord> predicate)
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            int count = (int)(this.fileStream.Length / RecordSize);
            for (int i = 0; i < count; i++)
            {
                int position = RecordSize * i;
                this.fileStream.Position = position;
                int peekedByte = this.fileStream.ReadByte();
                this.fileStream.Position = position;
                if ((peekedByte & 0b0100) == 0)
                {
                    byte[] buffer = new byte[RecordSize];
                    this.fileStream.Read(buffer, 0, buffer.Length);
                    var record = BytesToRecord(buffer);
                    if (predicate(record))
                    {
                        records.Add(record);
                    }
                }
            }

            return records;
        }

        private int ImportRecord(int id, RecordParameterObject record)
        {
            _ = record ?? throw new ArgumentNullException(nameof(record));

            byte[] bytes = RecordToBytes(id, record);
            this.fileStream.Position = (id - 1) * RecordSize;
            this.fileStream.Write(bytes);
            this.fileStream.Flush();

            return id;
        }
    }
}
