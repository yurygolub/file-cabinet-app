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
        private readonly List<FileCabinetRecord> list = new ();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();

        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();

        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="recordValidator">Record validator.</param>
        public FileCabinetMemoryService(IRecordValidator recordValidator)
        {
            this.RecordValidator = recordValidator ?? throw new ArgumentNullException(nameof(recordValidator));
        }

        /// <summary>
        /// Gets the record validator object.
        /// </summary>
        public IRecordValidator RecordValidator { get; }

        /// <inheritdoc/>
        public int CreateRecord(RecordParameterObject record)
        {
            _ = record ?? throw new ArgumentNullException(nameof(record));

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
        public bool EditRecord(int id, RecordParameterObject record)
        {
            _ = record ?? throw new ArgumentNullException(nameof(record));

            if (!this.IsRecordExist(id))
            {
                return false;
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

            return true;

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
        public IReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            _ = firstName ?? throw new ArgumentNullException(nameof(firstName));
            return FindByKey(this.firstNameDictionary, firstName);
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            _ = lastName ?? throw new ArgumentNullException(nameof(lastName));
            return FindByKey(this.lastNameDictionary, lastName);
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            return FindByKey(this.dateOfBirthDictionary, dateOfBirth);
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list);
        }

        /// <inheritdoc/>
        public int Restore(FileCabinetServiceSnapshot snapshot)
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

                if (this.IsRecordExist(record.Id))
                {
                    this.EditRecord(record.Id, recordParameter);
                }
                else
                {
                    this.ImportRecord(record.Id, recordParameter);
                }
            }

            return snapshot.Records.Count;
        }

        public bool Remove(int id)
        {
            if (id < 1 || id > this.list.Count)
            {
                return false;
            }

            var record = this.list[id - 1];

            this.firstNameDictionary[record.FirstName].Remove(record);
            this.lastNameDictionary[record.LastName].Remove(record);
            this.dateOfBirthDictionary[record.DateOfBirth].Remove(record);

            this.list.RemoveAt(id - 1);
            return true;
        }

        public bool IsRecordExist(int id)
        {
            return id >= 1 && id <= this.list.Count;
        }

        private static IReadOnlyCollection<FileCabinetRecord> FindByKey<T>(Dictionary<T, List<FileCabinetRecord>> dictionary, T key)
        {
            if (!dictionary.ContainsKey(key))
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return dictionary[key].ToArray();
        }

        private void ImportRecord(int id, RecordParameterObject record)
        {
            _ = record ?? throw new ArgumentNullException(nameof(record));

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
