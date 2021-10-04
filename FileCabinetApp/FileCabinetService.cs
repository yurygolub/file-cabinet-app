using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        public static void CheckInput(string firstName, string lastName, DateTime dateOfBirth, short weight, decimal account, char letter)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException("firstName length is less than 2 or more than 60.");
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            if (lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException("lastName length is less than 2 or more than 60.");
            }

            if (dateOfBirth < new DateTime(1950, 1, 1) || dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("dateOfBirth is less than 01-Jan-1950 or more than now.");
            }

            if (weight < 1 || weight > 500)
            {
                throw new ArgumentException("weight is less than 1 or more than 500.");
            }

            if (account < 0)
            {
                throw new ArgumentException("account is less than zero.");
            }

            if (!char.IsLetter(letter))
            {
                throw new ArgumentException("letter is not a letter.");
            }
        }

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short weight, decimal account, char letter)
        {
            CheckInput(firstName, lastName, dateOfBirth, weight, account, letter);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Weight = weight,
                Account = account,
                Letter = letter,
            };

            UpdateDictionary(this.firstNameDictionary, record, firstName);
            UpdateDictionary(this.lastNameDictionary, record, lastName);
            UpdateDictionary(this.dateOfBirthDictionary, record, dateOfBirth);

            this.list.Add(record);
            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        public int GetStat()
        {
            return this.list.Count;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short weight, decimal account, char letter)
        {
            this.IsExist(id);

            UpdateDictionary(this.firstNameDictionary, this.list[id - 1], this.list[id - 1].FirstName, firstName);
            UpdateDictionary(this.lastNameDictionary, this.list[id - 1], this.list[id - 1].LastName, lastName);
            UpdateDictionary(this.dateOfBirthDictionary, this.list[id - 1], this.list[id - 1].DateOfBirth, dateOfBirth);

            this.list[id - 1].FirstName = firstName;
            this.list[id - 1].LastName = lastName;
            this.list[id - 1].DateOfBirth = dateOfBirth;
            this.list[id - 1].Weight = weight;
            this.list[id - 1].Account = account;
            this.list[id - 1].Letter = letter;
        }

        public void IsExist(int id)
        {
            if (id < 1 || id > this.list.Count)
            {
                throw new ArgumentException($"#{id} record is not found.");
            }
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            if (!this.firstNameDictionary.ContainsKey(firstName))
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return this.firstNameDictionary[firstName].ToArray();
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            if (!this.lastNameDictionary.ContainsKey(lastName))
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return this.lastNameDictionary[lastName].ToArray();
        }

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
