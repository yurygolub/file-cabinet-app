using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;
using FileCabinetApp.Snapshot;

namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// Provides methods for working with file cabinet.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="recordValidator">Record validator.</param>
        public FileCabinetMemoryService(IRecordValidator recordValidator)
        {
            this.RecordValidator = recordValidator;
        }

        /// <summary>
        /// Gets the record validator object.
        /// </summary>
        public IRecordValidator RecordValidator { get; }

        /// <inheritdoc/>
        public int CreateRecord(RecordParameterObject record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.RecordValidator.ValidateParameters(record);

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

            static void UpdateDictionary<T>(Dictionary<T, List<FileCabinetRecord>> dictionary, FileCabinetRecord record, T key)
            {
                if (dictionary.ContainsKey(key))
                {
                    dictionary[key].Add(record);
                }
                else
                {
                    List<FileCabinetRecord> records = new List<FileCabinetRecord>
                    {
                        record,
                    };
                    dictionary.Add(key, records);
                }
            }
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return this.list.ToArray();
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, RecordParameterObject record)
        {
            this.IsRecordExist(id);

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

            static void UpdateDictionary<T>(Dictionary<T, List<FileCabinetRecord>> dictionary, FileCabinetRecord record, T oldValue, T newValue)
                where T : IEquatable<T>
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
                        List<FileCabinetRecord> records = new List<FileCabinetRecord>
                        {
                            record,
                        };
                        dictionary.Add(newValue, records);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void IsRecordExist(int id)
        {
            if (id < 1 || id > this.list.Count)
            {
                throw new ArgumentException($"#{id} record is not found.");
            }
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (!this.firstNameDictionary.ContainsKey(firstName))
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return this.firstNameDictionary[firstName].ToArray();
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (!this.lastNameDictionary.ContainsKey(lastName))
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return this.lastNameDictionary[lastName].ToArray();
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            if (!this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return this.dateOfBirthDictionary[dateOfBirth].ToArray();
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list);
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            foreach (var record in snapshot.Records)
            {
                var recordParameter = new RecordParameterObject(
                    record.FirstName,
                    record.LastName,
                    record.DateOfBirth,
                    record.Weight,
                    record.Account,
                    record.Letter);

                try
                {
                    this.EditRecord(record.Id, recordParameter);
                }
                catch (ArgumentException)
                {
                    this.ImportRecord(record.Id, recordParameter);
                }
            }
        }

        private void ImportRecord(int id, RecordParameterObject record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.RecordValidator.ValidateParameters(record);

            FileCabinetRecord fileCabinetRecord = new FileCabinetRecord
            {
                Id = id,
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

            static void UpdateDictionary<T>(Dictionary<T, List<FileCabinetRecord>> dictionary, FileCabinetRecord record, T key)
            {
                if (dictionary.ContainsKey(key))
                {
                    dictionary[key].Add(record);
                }
                else
                {
                    List<FileCabinetRecord> records = new List<FileCabinetRecord>
                    {
                        record,
                    };
                    dictionary.Add(key, records);
                }
            }
        }
    }
}
