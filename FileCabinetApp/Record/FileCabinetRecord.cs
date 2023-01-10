using System;

namespace FileCabinetApp.Record
{
    /// <summary>
    /// Presents a record.
    /// </summary>
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        public short Weight { get; set; }

        /// <summary>
        /// Gets or sets the account.
        /// </summary>
        public decimal Account { get; set; }

        /// <summary>
        /// Gets or sets the letter.
        /// </summary>
        public char Letter { get; set; }
    }
}
