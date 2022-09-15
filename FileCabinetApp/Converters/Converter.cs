using System;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Converters
{
    /// <summary>
    /// Provides methods for conversions.
    /// </summary>
    public class Converter : IConverter
    {
        /// <inheritdoc/>
        public Tuple<bool, string, int> IntConvert(string input)
        {
            if (int.TryParse(input, out int result))
            {
                return new Tuple<bool, string, int>(true, input, result);
            }

            return new Tuple<bool, string, int>(false, input, result);
        }

        /// <summary>
        /// Convert to string.
        /// </summary>
        /// <param name="input">Input data.</param>
        /// <returns>Result of conversion.</returns>
        public Tuple<bool, string, string> StringConvert(string input)
        {
            return new Tuple<bool, string, string>(true, input, input);
        }

        /// <summary>
        /// Convert to DateTime.
        /// </summary>
        /// <param name="input">Input data.</param>
        /// <returns>Result of conversion.</returns>
        public Tuple<bool, string, DateTime> DateConvert(string input)
        {
            if (DateTime.TryParse(input, out DateTime result))
            {
                return new Tuple<bool, string, DateTime>(true, input, result);
            }

            return new Tuple<bool, string, DateTime>(false, input, result);
        }

        /// <summary>
        /// Convert to short.
        /// </summary>
        /// <param name="input">Input data.</param>
        /// <returns>Result of conversion.</returns>
        public Tuple<bool, string, short> ShortConvert(string input)
        {
            if (short.TryParse(input, out short result))
            {
                return new Tuple<bool, string, short>(true, input, result);
            }

            return new Tuple<bool, string, short>(false, input, result);
        }

        /// <summary>
        /// Convert to decimal.
        /// </summary>
        /// <param name="input">Input data.</param>
        /// <returns>Result of conversion.</returns>
        public Tuple<bool, string, decimal> DecimalConvert(string input)
        {
            if (decimal.TryParse(input, out decimal result))
            {
                return new Tuple<bool, string, decimal>(true, input, result);
            }

            return new Tuple<bool, string, decimal>(false, input, result);
        }

        /// <summary>
        /// Convert to char.
        /// </summary>
        /// <param name="input">Input data.</param>
        /// <returns>Result of conversion.</returns>
        public Tuple<bool, string, char> CharConvert(string input)
        {
            if (char.TryParse(input, out char result))
            {
                return new Tuple<bool, string, char>(true, input, result);
            }

            return new Tuple<bool, string, char>(false, input, result);
        }
    }
}
