using System;
using System.IO;
using System.Xml;
using FileCabinetApp.Record;
using FileCabinetApp.Snapshot.Export;

namespace FileCabinetApp.Snapshot
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
    }
}
