using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides methods for conversions.
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// Convert to string.
        /// </summary>
        /// <param name="input">Input data.</param>
        /// <returns>Result of conversion.</returns>
        public Tuple<bool, string, string> StringConvert(string input);

        /// <summary>
        /// Convert to DateTime.
        /// </summary>
        /// <param name="input">Input data.</param>
        /// <returns>Result of conversion.</returns>
        public Tuple<bool, string, DateTime> DateConvert(string input);

        /// <summary>
        /// Convert to short.
        /// </summary>
        /// <param name="input">Input data.</param>
        /// <returns>Result of conversion.</returns>
        public Tuple<bool, string, short> ShortConvert(string input);

        /// <summary>
        /// Convert to decimal.
        /// </summary>
        /// <param name="input">Input data.</param>
        /// <returns>Result of conversion.</returns>
        public Tuple<bool, string, decimal> DecimalConvert(string input);

        /// <summary>
        /// Convert to char.
        /// </summary>
        /// <param name="input">Input data.</param>
        /// <returns>Result of conversion.</returns>
        public Tuple<bool, string, char> CharConvert(string input);
    }
}
