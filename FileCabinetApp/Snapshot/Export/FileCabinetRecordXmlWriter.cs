using System;
using System.Xml;
using FileCabinetApp.Record;

namespace FileCabinetApp.Snapshot.Export
{
    /// <summary>
    /// Serializes record in xml format.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private readonly XmlWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="xmlWriter">Xml writer.</param>
        public FileCabinetRecordXmlWriter(XmlWriter xmlWriter)
        {
            this.writer = xmlWriter;
        }

        /// <summary>
        /// Writes fileCabinetRecord in xml format file.
        /// </summary>
        /// <param name="fileCabinetRecord">File cabinet record.</param>
        public void Write(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord is null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            this.writer.WriteStartElement("record");
            this.writer.WriteAttributeString("id", $"{fileCabinetRecord.Id}");

            this.writer.WriteStartElement("name");
            this.writer.WriteAttributeString("first", $"{fileCabinetRecord.FirstName}");
            this.writer.WriteAttributeString("last", $"{fileCabinetRecord.LastName}");
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("dateOfBirth");
            this.writer.WriteString(fileCabinetRecord.DateOfBirth.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("weight");
            this.writer.WriteString($"{fileCabinetRecord.Weight}");
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("account");
            this.writer.WriteString($"{fileCabinetRecord.Account}");
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("letter");
            this.writer.WriteString($"{fileCabinetRecord.Letter}");
            this.writer.WriteEndElement();

            this.writer.WriteEndElement();
        }
    }
}
