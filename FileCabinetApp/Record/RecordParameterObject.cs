using System;

namespace FileCabinetApp.Record
{
    /// <summary>
    /// Presents a record to pass to the method.
    /// </summary>
    public class RecordParameterObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordParameterObject"/> class.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <param name="weight">Weight.</param>
        /// <param name="account">Account.</param>
        /// <param name="letter">Letter.</param>
        public RecordParameterObject(string firstName, string lastName, DateTime dateOfBirth, short weight, decimal account, char letter)
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
    }
}
