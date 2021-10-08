﻿namespace FileCabinetApp
{
    /// <summary>
    /// Parameters validation strategies interface.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Checks the validity of the entered data.
        /// </summary>
        /// <param name="record">Record.</param>
        public abstract void ValidateParameters(Record record);
    }
}
