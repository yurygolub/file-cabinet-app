using System;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;

namespace FileCabinetApp.Validators
{
    public class DateOfBirthValidator : IRecordValidator
    {
        private readonly DateTime from;
        private readonly DateTime to;

        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
        }

        public void ValidateParameters(RecordParameterObject record)
        {
            if (record.DateOfBirth < this.from || record.DateOfBirth > this.to)
            {
                throw new ArgumentException($"DateOfBirth is less than {this.from} or more than {this.to}.");
            }
        }
    }
}
