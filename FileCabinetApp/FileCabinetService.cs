using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides methods for working with file cabinet.
    /// </summary>
    public abstract class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        /// <summary>
        /// Creates a record validator object.
        /// </summary>
        /// <returns>Record validator object.</returns>
        public abstract IRecordValidator CreateValidator();

        /// <summary>
        /// Сreates a record with the specified parameters.
        /// </summary>
        /// <param name="record">Record.</param>
        /// <returns>Id of the created record.</returns>
        public int CreateRecord(Record record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.CreateValidator().ValidateParameters(record);

            FileCabinetRecord fileCabinetRecord = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = record.FirstName,
                LastName = record.LastName,
                DateOfBirth = record.DateOfBirth,
                Weight = record.Weight,
                Account = record.Account,
                Letter = record.Letter,
            };

            UpdateDictionary(this.firstNameDictionary, fileCabinetRecord, record.FirstName);
            UpdateDictionary(this.lastNameDictionary, fileCabinetRecord, record.LastName);
            UpdateDictionary(this.dateOfBirthDictionary, fileCabinetRecord, record.DateOfBirth);

            this.list.Add(fileCabinetRecord);
            return fileCabinetRecord.Id;
        }

        /// <summary>
        /// Returns the array of records.
        /// </summary>
        /// <returns>The array of records.</returns>
        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        /// <summary>
        /// Returns the count of records.
        /// </summary>
        /// <returns>The count of records.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Edits a record with the specified parameters.
        /// </summary>
        /// <param name="id">Id of editing record.</param>
        /// <param name="record">Record.</param>
        public void EditRecord(int id, Record record)
        {
            this.IsExist(id);

            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            UpdateDictionary(this.firstNameDictionary, this.list[id - 1], this.list[id - 1].FirstName, record.FirstName);
            UpdateDictionary(this.lastNameDictionary, this.list[id - 1], this.list[id - 1].LastName, record.LastName);
            UpdateDictionary(this.dateOfBirthDictionary, this.list[id - 1], this.list[id - 1].DateOfBirth, record.DateOfBirth);

            this.list[id - 1].FirstName = record.FirstName;
            this.list[id - 1].LastName = record.LastName;
            this.list[id - 1].DateOfBirth = record.DateOfBirth;
            this.list[id - 1].Weight = record.Weight;
            this.list[id - 1].Account = record.Account;
            this.list[id - 1].Letter = record.Letter;
        }

        /// <summary>
        /// Checks if there is a record with the specified id.
        /// </summary>
        /// <param name="id">Record id.</param>
        /// <exception cref="ArgumentException">Throw when there is not record with specified id.</exception>
        public void IsExist(int id)
        {
            if (id < 1 || id > this.list.Count)
            {
                throw new ArgumentException($"#{id} record is not found.");
            }
        }

        /// <summary>
        /// Finds all records with specified first name.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <returns>Returns the array of found records.</returns>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            if (!this.firstNameDictionary.ContainsKey(firstName))
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return this.firstNameDictionary[firstName].ToArray();
        }

        /// <summary>
        /// Finds all records with specified last name.
        /// </summary>
        /// <param name="lastName">Last name.</param>
        /// <returns>Returns the array of found records.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            if (!this.lastNameDictionary.ContainsKey(lastName))
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return this.lastNameDictionary[lastName].ToArray();
        }

        /// <summary>
        /// Finds all records with specified date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <returns>Returns the array of found records.</returns>
        public FileCabinetRecord[] FindByDateOfBirth(DateTime dateOfBirth)
        {
            if (!this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return this.dateOfBirthDictionary[dateOfBirth].ToArray();
        }

        private static void UpdateDictionary<T>(Dictionary<T, List<FileCabinetRecord>> dictionary, FileCabinetRecord record, T key)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key].Add(record);
            }
            else
            {
                List<FileCabinetRecord> temp = new List<FileCabinetRecord>();
                temp.Add(record);
                dictionary.Add(key, temp);
            }
        }

        private static void UpdateDictionary<T>(Dictionary<T, List<FileCabinetRecord>> dictionary, FileCabinetRecord record, T oldValue, T newValue)
            where T : IComparable
        {
            if (!oldValue.Equals(newValue))
            {
                dictionary[oldValue].Remove(record);
                if (dictionary.ContainsKey(newValue))
                {
                    dictionary[newValue].Add(record);
                    dictionary[newValue].Sort((firstValue, secondValue) => firstValue.Id.CompareTo(secondValue.Id));
                }
                else
                {
                    List<FileCabinetRecord> temp = new List<FileCabinetRecord>();
                    temp.Add(record);
                    dictionary.Add(newValue, temp);
                }
            }
        }
    }
}
