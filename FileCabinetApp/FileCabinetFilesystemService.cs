using System;
using System.Collections.Generic;
using System.IO;

namespace FileCabinetApp
{
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int RecordSize = 278;
        private FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="recordValidator">Record validator.</param>
        public FileCabinetFilesystemService(IRecordValidator recordValidator)
        {
            this.RecordValidator = recordValidator;
        }

        /// <summary>
        /// Gets the record validator object.
        /// </summary>
        /// <value>
        /// </value>
        public IRecordValidator RecordValidator { get; private set; }

        public int CreateRecord(Record record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            int length = (int)this.fileStream.Length;
            int id = (length / RecordSize) + 1;

            byte[] bytes = RecordToBytes(id, record);
            this.fileStream.Position = this.fileStream.Length;
            this.fileStream.Write(bytes);
            this.fileStream.Flush();

            return id;
        }

        public void EditRecord(int id, Record record)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            throw new NotImplementedException();
        }

        public int GetStat()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            throw new NotImplementedException();
        }

        public void IsExist(int id)
        {
            throw new NotImplementedException();
        }

        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }

        public void OpenFile()
        {
            this.fileStream = new FileStream("cabinet-records.db", FileMode.Open, FileAccess.ReadWrite);
        }

        public void CloseFile()
        {
            this.fileStream.Close();
        }

        private static byte[] RecordToBytes(int id, Record record)
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

            void WriteFixedLengthString(BinaryWriter binaryWriter, string line)
            {
                const int MaxSize = 60;
                char[] buffer = new char[MaxSize];
                line.ToCharArray().CopyTo(buffer, 0);
                binaryWriter.Write(buffer);
            }
        }
    }
}
