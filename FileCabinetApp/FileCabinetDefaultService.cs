using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides methods for working with default file cabinet.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        /// <summary>
        /// Checks the validity of the entered data.
        /// </summary>
        /// <param name="record">Record.</param>
        public override void ValidateParameters(Record record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (string.IsNullOrWhiteSpace(record.FirstName))
            {
                throw new ArgumentNullException("FirstName", "FirstName is null or white space.");
            }

            if (record.FirstName.Length < 2 || record.FirstName.Length > 60)
            {
                throw new ArgumentException("firstName length is less than 2 or more than 60.");
            }

            if (string.IsNullOrWhiteSpace(record.LastName))
            {
                throw new ArgumentNullException("LastName", "LastName is null or white space.");
            }

            if (record.LastName.Length < 2 || record.LastName.Length > 60)
            {
                throw new ArgumentException("lastName length is less than 2 or more than 60.");
            }

            if (record.DateOfBirth < new DateTime(1950, 1, 1) || record.DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("dateOfBirth is less than 01-Jan-1950 or more than now.");
            }

            if (record.Weight < 1 || record.Weight > 500)
            {
                throw new ArgumentException("weight is less than 1 or more than 500.");
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
