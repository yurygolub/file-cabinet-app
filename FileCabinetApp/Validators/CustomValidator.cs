using System;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Custom validation strategy class.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        /// <summary>
        /// Checks the validity of the entered data.
        /// </summary>
        /// <param name="record">Record.</param>
        public void ValidateParameters(RecordParameterObject record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (string.IsNullOrWhiteSpace(record.FirstName))
            {
                throw new ArgumentNullException("FirstName", "FirstName is null or white space.");
            }

            if (record.FirstName.Length < 2 || record.FirstName.Length > 30)
            {
                throw new ArgumentException("firstName length is less than 2 or more than 30.");
            }

            if (string.IsNullOrWhiteSpace(record.LastName))
            {
                throw new ArgumentNullException("LastName", "LastName is null or white space.");
            }

            if (record.LastName.Length < 2 || record.LastName.Length > 30)
            {
                throw new ArgumentException("lastName length is less than 2 or more than 30.");
            }

            if (record.DateOfBirth < new DateTime(1930, 1, 1) || record.DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("dateOfBirth is less than 01-Jan-1930 or more than now.");
            }

            if (record.Weight < 1 || record.Weight > 400)
            {
                throw new ArgumentException("weight is less than 1 or more than 400.");
            }

            if (record.Account < 0)
            {
                throw new ArgumentException("account is less than zero.");
            }

            if (!char.IsLetter(record.Letter))
            {
                throw new ArgumentException("letter is not a letter.");
            }
        }
    }
}
