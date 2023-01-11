using System;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;

namespace FileCabinetApp.Validators
{
    public class WeightValidator : IRecordValidator
    {
        private readonly short min;
        private readonly short max;

        public WeightValidator(short min, short max)
        {
            this.min = min;
            this.max = max;
        }

        public void ValidateParameters(RecordParameterObject record)
        {
            if (record.Weight < this.min || record.Weight > this.max)
            {
                throw new ArgumentException($"Weight is less than {this.min} or more than {this.max}.");
            }
        }
    }
}
