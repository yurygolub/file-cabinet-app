using System;
using System.Collections.Generic;
using System.IO;

namespace FileCabinetApp
{
    public class FileCabinetFilesystemService : IFileCabinetService
    {
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
            throw new NotImplementedException();
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
            this.fileStream = new FileStream("cabinet-records.db", FileMode.Append);
        }

        public void CloseFile()
        {
            this.fileStream.Close();
        }
    }
}
