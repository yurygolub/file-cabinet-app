using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();

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

            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                this.firstNameDictionary[firstName].Add(record);
            }
            else
            {
                List<FileCabinetRecord> temp = new List<FileCabinetRecord>();
                temp.Add(record);
                this.firstNameDictionary.Add(firstName, temp);
            }

            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                this.lastNameDictionary[lastName].Add(record);
            }
            else
            {
                List<FileCabinetRecord> temp = new List<FileCabinetRecord>();
                temp.Add(record);
                this.lastNameDictionary.Add(lastName, temp);
            }

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

            List<FileCabinetRecord> res = new List<FileCabinetRecord>();
            foreach (var item in this.firstNameDictionary[firstName])
            {
                if (string.Equals(item.FirstName, firstName, StringComparison.InvariantCultureIgnoreCase))
                {
                    res.Add(item);
                }
            }

            return res.ToArray();
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            if (!this.lastNameDictionary.ContainsKey(lastName))
            {
                return Array.Empty<FileCabinetRecord>();
            }

            List<FileCabinetRecord> res = new List<FileCabinetRecord>();
            foreach (var item in this.lastNameDictionary[lastName])
            {
                if (string.Equals(item.LastName, lastName, StringComparison.InvariantCultureIgnoreCase))
                {
                    res.Add(item);
                }
            }

            return res.ToArray();
        }

        public FileCabinetRecord[] FindByDateOfBirth(DateTime dateOfBirth)
        {
            List<FileCabinetRecord> res = new List<FileCabinetRecord>();
            foreach (var item in this.list)
            {
                if (DateTime.Equals(item.DateOfBirth, dateOfBirth))
                {
                    res.Add(item);
                }
            }

            return res.ToArray();
        }
    }
}
