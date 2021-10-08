using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Parameters validation interface.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Validates the first name.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <returns>Result of first name validation.</returns>
        public Tuple<bool, string> FirstNameValidate(string firstName);

        /// <summary>
        /// Validates the last name.
        /// </summary>
        /// <param name="lastName">Last name.</param>
        /// <returns>Result of last name validation.</returns>
        public Tuple<bool, string> LastNameValidate(string lastName);

        /// <summary>
        /// Validates the date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <returns>Result of date of birth validation.</returns>
        public Tuple<bool, string> DateOfBirtheValidate(DateTime dateOfBirth);

        /// <summary>
        /// Validates the weight.
        /// </summary>
        /// <param name="weight">Weight.</param>
        /// <returns>Result of weight validation.</returns>
        public Tuple<bool, string> WeightValidate(short weight);

        /// <summary>
        /// Validates the account.
        /// </summary>
        /// <param name="account">Account.</param>
        /// <returns>Result of account validation.</returns>
        public Tuple<bool, string> AccountValidate(decimal account);

        /// <summary>
        /// Validates the letter.
        /// </summary>
        /// <param name="letter">Letter.</param>
        /// <returns>Result of letter validation.</returns>
        public Tuple<bool, string> LetterValidate(char letter);
    }
}
