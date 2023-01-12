using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;
using FileCabinetApp.Snapshot;

#pragma warning disable SA1118 // Parameter should not span multiple lines

namespace FileCabinetApp.FileCabinetService
{
    public class ServiceLogger : IFileCabinetService
    {
        private readonly IFileCabinetService service;
        private readonly TextWriter textWriter;

        public ServiceLogger(IFileCabinetService service, TextWriter textWriter)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.textWriter = textWriter ?? throw new ArgumentNullException(nameof(textWriter));
        }

        public int CreateRecord(RecordParameterObject record)
        {
            return this.ExecuteWithLogging(
                () => this.service.CreateRecord(record),
                nameof(this.CreateRecord),
                "with " +
                $"FirstName = '{record.FirstName}', " +
                $"LastName = '{record.LastName}', " +
                $"DateOfBirth = '{record.DateOfBirth}', " +
                $"Weight = '{record.Weight}', " +
                $"Account = '{record.Account}', " +
                $"Letter = '{record.Letter}'");
        }

        public bool EditRecord(int id, RecordParameterObject record)
        {
            return this.ExecuteWithLogging(
                () => this.service.EditRecord(id, record),
                nameof(this.EditRecord),
                "with " +
                $"id = '{id}'" +
                $"FirstName = '{record.FirstName}', " +
                $"LastName = '{record.LastName}', " +
                $"DateOfBirth = '{record.DateOfBirth}', " +
                $"Weight = '{record.Weight}', " +
                $"Account = '{record.Account}', " +
                $"Letter = '{record.Letter}'");
        }

        public IReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return this.ExecuteWithLogging(
                () => this.service.FindByFirstName(firstName),
                nameof(this.FindByFirstName),
                "with " +
                $"firstName = '{firstName}'");
        }

        public IReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            return this.ExecuteWithLogging(
                () => this.service.FindByLastName(lastName),
                nameof(this.FindByLastName),
                "with " +
                $"lastName = '{lastName}'");
        }

        public IReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            return this.ExecuteWithLogging(
                () => this.service.FindByDateOfBirth(dateOfBirth),
                nameof(this.FindByDateOfBirth),
                "with " +
                $"dateOfBirth = '{dateOfBirth}'");
        }

        public IReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return this.ExecuteWithLogging(this.service.GetRecords, nameof(this.GetRecords), string.Empty);
        }

        public int GetStat()
        {
            return this.ExecuteWithLogging(this.service.GetStat, nameof(this.GetStat), string.Empty);
        }

        public int CountOfRemoved()
        {
            return this.ExecuteWithLogging(this.service.CountOfRemoved, nameof(this.CountOfRemoved), string.Empty);
        }

        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return this.ExecuteWithLogging(this.service.MakeSnapshot, nameof(this.MakeSnapshot), string.Empty);
        }

        public bool Remove(int id)
        {
            return this.ExecuteWithLogging(
                () => this.service.Remove(id),
                nameof(this.Remove),
                "with " +
                $"id = '{id}'");
        }

        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            return this.ExecuteWithLogging(
                () => this.service.Restore(snapshot),
                nameof(this.Restore),
                "with " +
                $"snapshot = '{snapshot}'");
        }

        public int Purge()
        {
            return this.ExecuteWithLogging(this.service.Purge, nameof(this.Purge), string.Empty);
        }

        private T ExecuteWithLogging<T>(Func<T> func, string name, string log)
        {
            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm:ss.fff} - Calling {nameof(this.Purge)}() " + log);

            var result = func();

            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm:ss.fff} - {nameof(this.Purge)}() returned '{result}'");

            return result;
        }
    }
}
