﻿using System;
using System.Collections.Generic;
using FileCabinetApp.Record;
using FileCabinetApp.Snapshot;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Interface for working with file cabinet.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Сreates a record with the specified parameters.
        /// </summary>
        /// <param name="record">Record.</param>
        /// <returns>Id of the created record.</returns>
        public int CreateRecord(RecordParameterObject record);

        /// <summary>
        /// Returns the array of records.
        /// </summary>
        /// <returns>The array of records.</returns>
        public IReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Returns the count of records.
        /// </summary>
        /// <returns>The count of records.</returns>
        public int GetStat();

        public int CountOfRemoved();

        /// <summary>
        /// Edits a record with the specified parameters.
        /// </summary>
        /// <param name="id">Id of editing record.</param>
        /// <param name="record">Record.</param>
        /// <returns>True if the record exists, otherwise false.</returns>
        public bool EditRecord(int id, RecordParameterObject record);

        /// <summary>
        /// Finds all records with specified first name.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <returns>Returns the array of found records.</returns>
        public IReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Finds all records with specified last name.
        /// </summary>
        /// <param name="lastName">Last name.</param>
        /// <returns>Returns the array of found records.</returns>
        public IReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Finds all records with specified date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <returns>Returns the array of found records.</returns>
        public IReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth);

        /// <summary>
        /// Creates FileCabinetServiceSnapshot object.
        /// </summary>
        /// <returns>Returns new FileCabinetServiceSnapshot object.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Restores service data from <paramref name="snapshot"/> object.
        /// </summary>
        /// <param name="snapshot">FileCabinetServiceSnapshot object.</param>
        /// <returns>Count of restored records.</returns>
        public int Restore(FileCabinetServiceSnapshot snapshot);

        public bool Remove(int id);

        public int Purge();
    }
}
