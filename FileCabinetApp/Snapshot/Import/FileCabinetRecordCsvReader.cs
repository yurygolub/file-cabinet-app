using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FileCabinetApp.Converters;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;

namespace FileCabinetApp.Snapshot.Import
{
    public class FileCabinetRecordCsvReader
    {
        private readonly StreamReader reader;
        private readonly IConverter converter;

        public FileCabinetRecordCsvReader(StreamReader reader)
        {
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
            this.converter = new Converter();
        }

        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> result = new List<FileCabinetRecord>();

            string[] headers = this.reader.ReadLine().Split(",");

            string line;
            while ((line = this.reader.ReadLine()) != null)
            {
                string[] strings = line.Split(",");

                if (this.InputRecord(headers, strings, out FileCabinetRecord res, out string errorMsg))
                {
                    result.Add(res);
                }
                else
                {
                    Console.WriteLine($"A proplem occured while importing record {res.Id}: {errorMsg}");
                }
            }

            return result;
        }

        private bool InputRecord(string[] headers, string[] strings, out FileCabinetRecord fileCabinetRecord, out string errorMessage)
        {
            StringBuilder stringBuilder = new StringBuilder();

            bool result = true;

#pragma warning disable SA1101 // Prefix local calls with this
            ReadInput(this.converter.IntConvert, headers[0], strings[0], out int id);
            ReadInput(this.converter.StringConvert, headers[1], strings[1], out string firstName);
            ReadInput(this.converter.StringConvert, headers[2], strings[2], out string lastName);
            ReadInput(this.converter.DateConvert, headers[3], strings[3], out DateTime dateOfBirth);
            ReadInput(this.converter.ShortConvert, headers[4], strings[4], out short weight);
            ReadInput(this.converter.DecimalConvert, headers[5], strings[5], out decimal account);
            ReadInput(this.converter.CharConvert, headers[6], strings[6], out char letter);
#pragma warning restore SA1101 // Prefix local calls with this

            fileCabinetRecord = new FileCabinetRecord()
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Weight = weight,
                Account = account,
                Letter = letter,
            };

            errorMessage = stringBuilder.ToString();
            return result;

            void ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, string header, string input, out T output)
            {
                Tuple<bool, string, T> conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    output = default;
                    result = false;
                    stringBuilder.Append($"Conversion for element {header} failed: {conversionResult.Item2}. ");
                    return;
                }

                output = conversionResult.Item3;
            }
        }
    }
}
