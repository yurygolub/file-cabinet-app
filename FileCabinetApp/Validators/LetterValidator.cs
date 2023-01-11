using System;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;

namespace FileCabinetApp.Validators
{
    public class LetterValidator : IRecordValidator
    {
        public void ValidateParameters(RecordParameterObject record)
        {
            if (!char.IsLetter(record.Letter))
            {
                throw new ArgumentException("Letter is not a letter.");
            }
        }
    }
}
