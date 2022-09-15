using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using FileCabinetApp.Record;
using FileCabinetApp.Snapshot.Export;
using FileCabinetApp.Snapshot.Import;

namespace FileCabinetApp.Snapshot
{
    /// <summary>
    /// Memento class.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly List<FileCabinetRecord> records;
        private IList<FileCabinetRecord> importedRecords;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">Records.</param>
        public FileCabinetServiceSnapshot(IEnumerable<FileCabinetRecord> records)
        {
            this.records = new List<FileCabinetRecord>(records);
        }

        public ReadOnlyCollection<FileCabinetRecord> Records => new ReadOnlyCollection<FileCabinetRecord>(this.importedRecords);

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

        /// <summary>
        /// Saves records to xml.
        /// </summary>
        /// <param name="streamWriter">Stream writer.</param>
        public void SaveToXml(StreamWriter streamWriter)
        {
            if (streamWriter is null)
            {
                throw new ArgumentNullException(nameof(streamWriter));
            }

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
            };

            XmlWriter xmlWriter = XmlWriter.Create(streamWriter, xmlWriterSettings);
            FileCabinetRecordXmlWriter fileCabinetRecordXmlWriter = new FileCabinetRecordXmlWriter(xmlWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("records");

            foreach (var record in this.records)
            {
                fileCabinetRecordXmlWriter.Write(record);
            }

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        public void LoadFromCsv(StreamReader streamReader)
        {
            _ = streamReader ?? throw new ArgumentNullException(nameof(streamReader));

            FileCabinetRecordCsvReader recordCsvReader = new FileCabinetRecordCsvReader(streamReader);
            this.importedRecords = recordCsvReader.ReadAll();
        }
    }
}
