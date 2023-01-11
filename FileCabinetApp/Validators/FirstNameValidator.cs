using System;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;

namespace FileCabinetApp.Validators
{
    public class FirstNameValidator : IRecordValidator
    {
        private readonly int minLength;
        private readonly int maxLength;

        public FirstNameValidator(int minLength, int maxLength)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        public void ValidateParameters(RecordParameterObject record)
        {
            if (string.IsNullOrWhiteSpace(record.FirstName))
            {
                throw new ArgumentException("FirstName is null or white space.");
            }

            if (record.FirstName.Length < this.minLength || record.FirstName.Length > this.maxLength)
            {
                throw new ArgumentException($"FirstName length is less than {this.minLength} or more than {this.maxLength}.");
            }
        }
    }
}
