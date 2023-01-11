using System;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;

namespace FileCabinetApp.Validators
{
    public class LastNameValidator : IRecordValidator
    {
        private readonly int minLength;
        private readonly int maxLength;

        public LastNameValidator(int minLength, int maxLength)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        public void ValidateParameters(RecordParameterObject record)
        {
            if (string.IsNullOrWhiteSpace(record.LastName))
            {
                throw new ArgumentException("LastName is null or white space.");
            }

            if (record.LastName.Length < this.minLength || record.LastName.Length > this.maxLength)
            {
                throw new ArgumentException($"LastName length is less than {this.minLength} or more than {this.maxLength}.");
            }
        }
    }
}
