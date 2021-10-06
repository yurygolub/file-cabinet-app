using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Presents a record to pass to the method.
    /// </summary>
    public class Record
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Record"/> class.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <param name="weight">Weight.</param>
        /// <param name="account">Account.</param>
        /// <param name="letter">Letter.</param>
        public Record(string firstName, string lastName, DateTime dateOfBirth, short weight, decimal account, char letter)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DateOfBirth = dateOfBirth;
            this.Weight = weight;
            this.Account = account;
            this.Letter = letter;
        }

        /// <summary>
        /// Gets the first name.
        /// </summary>
        /// <value>
        /// </value>
        public string FirstName { get; private set; }

        /// <summary>
        /// Gets the last name.
        /// </summary>
        /// <value>
        /// </value>
        public string LastName { get; private set; }

        /// <summary>
        /// Gets the date of birth.
        /// </summary>
        /// <value>
        /// </value>
        public DateTime DateOfBirth { get; private set; }

        /// <summary>
        /// Gets the weight.
        /// </summary>
        /// <value>
        /// </value>
        public short Weight { get; private set; }

        /// <summary>
        /// Gets the account.
        /// </summary>
        /// <value>
        /// </value>
        public decimal Account { get; private set; }

        /// <summary>
        /// Gets the letter.
        /// </summary>
        /// <value>
        /// </value>
        public char Letter { get; private set; }

        /// <summary>
        /// Checks the fields of record.
        /// </summary>
        /// <exception cref="ArgumentNullException">Throw when first name or last name is null.</exception>
        /// <exception cref="ArgumentException">
        /// Throw when first name or last name length is less than 2 or more than 60,
        /// date of birth is less than 01-Jan-1950 or more than now, weight is less than 1 or more than 500, account less than 0 and letter is not a letter.
        /// </exception>
        public void CheckInput()
        {
            if (string.IsNullOrWhiteSpace(this.FirstName))
            {
                throw new ArgumentNullException(nameof(this.FirstName));
            }

            if (this.FirstName.Length < 2 || this.FirstName.Length > 60)
            {
                throw new ArgumentException("firstName length is less than 2 or more than 60.");
            }

            if (string.IsNullOrWhiteSpace(this.LastName))
            {
                throw new ArgumentNullException(nameof(this.LastName));
            }

            if (this.LastName.Length < 2 || this.LastName.Length > 60)
            {
                throw new ArgumentException("lastName length is less than 2 or more than 60.");
            }

            if (this.DateOfBirth < new DateTime(1950, 1, 1) || this.DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("dateOfBirth is less than 01-Jan-1950 or more than now.");
            }

            if (this.Weight < 1 || this.Weight > 500)
            {
                throw new ArgumentException("weight is less than 1 or more than 500.");
            }

            if (this.Account < 0)
            {
                throw new ArgumentException("account is less than zero.");
            }

            if (!char.IsLetter(this.Letter))
            {
                throw new ArgumentException("letter is not a letter.");
            }
        }
    }
}
