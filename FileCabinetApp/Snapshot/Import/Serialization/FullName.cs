using System.Xml.Serialization;

namespace FileCabinetApp.Snapshot.Import.Serialization
{
    public class FullName
    {
        [XmlAttribute("first")]
        public string FirstName { get; set; }

        [XmlAttribute("last")]
        public string LastName { get; set; }
    }
}
