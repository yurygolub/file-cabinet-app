using System.Collections.Generic;
using FileCabinetApp.Record;

namespace FileCabinetApp.Interfaces
{
    public interface IRecordPrinter
    {
        void Print(IEnumerable<FileCabinetRecord> records);
    }
}
