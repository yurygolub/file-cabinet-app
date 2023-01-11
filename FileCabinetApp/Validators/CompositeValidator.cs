using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;

namespace FileCabinetApp.Validators
{
    public class CompositeValidator : IRecordValidator
    {
        private readonly IEnumerable<IRecordValidator> validators;

        public CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.validators = validators ?? throw new ArgumentNullException(nameof(validators));
        }

        public void ValidateParameters(RecordParameterObject record)
        {
            foreach (var validator in this.validators)
            {
                validator.ValidateParameters(record);
            }
        }
    }
}
