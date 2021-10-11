using System;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Memento class.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">Records.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        /// Saves records to csv.
        /// </summary>
        /// <param name="streamWriter">Stream writer.</param>
        public void SaveToCsv(StreamWriter streamWriter)
        {
            if (streamWriter is null)
            {
                throw new ArgumentNullException(nameof(streamWriter));
            }

            FileCabinetRecordCsvWriter fileCabinetRecordCsvWriter = new FileCabinetRecordCsvWriter(streamWriter);

            streamWriter.WriteLine("Id,First Name,Last Name,Date of Birth,Weight,Account,Letter");
            foreach (var record in this.records)
            {
                fileCabinetRecordCsvWriter.Write(record);
            }

            streamWriter.Close();
        }
    }
}
