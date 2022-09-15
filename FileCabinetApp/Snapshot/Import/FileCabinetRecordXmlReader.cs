using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using FileCabinetApp.Record;
using FileCabinetApp.Snapshot.Import.Serialization;

namespace FileCabinetApp.Snapshot.Import
{
    public class FileCabinetRecordXmlReader
    {
        private readonly StreamReader reader;

        public FileCabinetRecordXmlReader(StreamReader reader)
        {
            _ = reader ?? throw new ArgumentNullException(nameof(reader));

            this.reader = reader;
        }

        public IList<FileCabinetRecord> ReadAll()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(FileCabinetRecordSerializable[]), new XmlRootAttribute("records"));

            XmlReader xmlReader = XmlReader.Create(this.reader);

            FileCabinetRecordSerializable[] serializableRecords = xmlSerializer.Deserialize(xmlReader) as FileCabinetRecordSerializable[];

            return this.MapRecords(serializableRecords).ToList();
        }

        private IEnumerable<FileCabinetRecord> MapRecords(IEnumerable<FileCabinetRecordSerializable> records)
        {
            foreach (var record in records)
            {
                yield return new FileCabinetRecord()
                {
                    Id = record.Id,
                    FirstName = record.FullName.FirstName,
                    LastName = record.FullName.LastName,
                    DateOfBirth = DateTime.Parse(record.DateOfBirth),
                    Weight = record.Weight,
                    Account = record.Account,
                    Letter = record.Letter,
                };
            }
        }
    }
}
