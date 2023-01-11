using System;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;

namespace FileCabinetApp.Validators
{
    public class AccountValidator : IRecordValidator
    {
        private readonly decimal min;
        private readonly decimal max;

        public AccountValidator(decimal min, decimal max)
        {
            this.min = min;
            this.max = max;
        }

        public void ValidateParameters(RecordParameterObject record)
        {
            if (record.Account < this.min || record.Account > this.max)
            {
                throw new ArgumentException($"Account is less than {this.min} or more than {this.max}.");
            }
        }
    }
}
