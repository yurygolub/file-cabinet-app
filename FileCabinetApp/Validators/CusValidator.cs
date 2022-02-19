using System;
using System.Globalization;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Custom validator class.
    /// </summary>
    public class CusValidator : IValidator
    {
        /// <summary>
        /// Validates the first name using custom validation rules.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <returns>Result of first name validation.</returns>
        public Tuple<bool, string> FirstNameValidate(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 30)
            {
                return new Tuple<bool, string>(false, firstName);
            }

            return new Tuple<bool, string>(true, firstName);
        }

        /// <summary>
        /// Validates the last name using custom validation rules.
        /// </summary>
        /// <param name="lastName">Last name.</param>
        /// <returns>Result of last name validation.</returns>
        public Tuple<bool, string> LastNameValidate(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 30)
            {
                return new Tuple<bool, string>(false, lastName);
            }

            return new Tuple<bool, string>(true, lastName);
        }

        /// <summary>
        /// Validates the date of birth using custom validation rules.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <returns>Result of date of birth validation.</returns>
        public Tuple<bool, string> DateOfBirtheValidate(DateTime dateOfBirth)
        {
            if (dateOfBirth < new DateTime(1930, 1, 1) || dateOfBirth > DateTime.Now)
            {
                return new Tuple<bool, string>(false, dateOfBirth.ToString(CultureInfo.InvariantCulture));
            }

            return new Tuple<bool, string>(true, dateOfBirth.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Validates the weight using custom validation rules.
        /// </summary>
        /// <param name="weight">Weight.</param>
        /// <returns>Result of weight validation.</returns>
        public Tuple<bool, string> WeightValidate(short weight)
        {
            if (weight < 1 || weight > 400)
            {
                return new Tuple<bool, string>(false, weight.ToString(CultureInfo.InvariantCulture));
            }

            return new Tuple<bool, string>(true, weight.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Validates the account using custom validation rules.
        /// </summary>
        /// <param name="account">Account.</param>
        /// <returns>Result of account validation.</returns>
        public Tuple<bool, string> AccountValidate(decimal account)
        {
            if (account < 0)
            {
                return new Tuple<bool, string>(false, account.ToString(CultureInfo.InvariantCulture));
            }

            return new Tuple<bool, string>(true, account.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Validates the letter using custom validation rules.
        /// </summary>
        /// <param name="letter">Letter.</param>
        /// <returns>Result of letter validation.</returns>
        public Tuple<bool, string> LetterValidate(char letter)
        {
            if (!char.IsLetter(letter))
            {
                return new Tuple<bool, string>(false, letter.ToString());
            }

            return new Tuple<bool, string>(true, letter.ToString());
        }
    }
}
