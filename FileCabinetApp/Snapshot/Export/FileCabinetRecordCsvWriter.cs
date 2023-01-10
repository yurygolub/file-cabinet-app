using System;
using System.IO;
using FileCabinetApp.Record;

namespace FileCabinetApp.Snapshot.Export
{
    /// <summary>
    /// Serializes record in csv format.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private readonly TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="textWriter">Text writer.</param>
        public FileCabinetRecordCsvWriter(TextWriter textWriter)
        {
            this.writer = textWriter ?? throw new ArgumentNullException(nameof(textWriter));
        }

        /// <summary>
        /// Writes fileCabinetRecord in csv format file.
        /// </summary>
        /// <param name="fileCabinetRecord">File cabinet record.</param>
        public void Write(FileCabinetRecord fileCabinetRecord)
        {
            _ = fileCabinetRecord ?? throw new ArgumentNullException(nameof(fileCabinetRecord));

            this.writer.WriteLine($"{fileCabinetRecord.Id},{fileCabinetRecord.FirstName},{fileCabinetRecord.LastName}," +
                    $"{fileCabinetRecord.DateOfBirth.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)}," +
                    $"{fileCabinetRecord.Weight},{fileCabinetRecord.Account},{fileCabinetRecord.Letter}");
        }
    }
}
