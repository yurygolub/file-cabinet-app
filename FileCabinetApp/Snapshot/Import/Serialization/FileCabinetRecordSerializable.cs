using System.Linq;
using System.Xml.Serialization;

namespace FileCabinetApp.Snapshot.Import.Serialization
{
    [XmlType("record")]
    public class FileCabinetRecordSerializable
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlElement("name")]
        public FullName FullName { get; set; }

        [XmlElement("dateOfBirth")]
        public string DateOfBirth { get; set; }

        [XmlElement("weight")]
        public short Weight { get; set; }

        [XmlElement("account")]
        public decimal Account { get; set; }

        [XmlIgnore]
        public char Letter { get; set; }

        [XmlElement("letter")]
        public string LetterString
        {
            get => this.Letter.ToString();
            set => this.Letter = value.Single();
        }
    }
}
