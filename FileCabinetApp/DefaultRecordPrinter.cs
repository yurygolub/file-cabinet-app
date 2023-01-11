using System;
using System.Collections.Generic;
using System.Globalization;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;

namespace FileCabinetApp
{
    public class DefaultRecordPrinter : IRecordPrinter
    {
        public void Print(IEnumerable<FileCabinetRecord> records)
        {
            foreach (var record in records)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, " +
                    $"{record.DateOfBirth.ToString("yyyy'-'MMM'-'dd", CultureInfo.InvariantCulture)}, " +
                    $"{record.Weight}, {record.Account}, {record.Letter}");
            }
        }
    }
}
