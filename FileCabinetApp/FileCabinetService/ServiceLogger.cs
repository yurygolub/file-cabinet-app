using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;
using FileCabinetApp.Snapshot;

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
            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - Calling {nameof(this.CreateRecord)}() with " +
                $"FirstName = '{record.FirstName}', " +
                $"LastName = '{record.LastName}', " +
                $"DateOfBirth = '{record.DateOfBirth}', " +
                $"Weight = '{record.Weight}', " +
                $"Account = '{record.Account}', " +
                $"Letter = '{record.Letter}'");

            var result = this.service.CreateRecord(record);

            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - {nameof(this.CreateRecord)}() returned '{result}'");

            return result;
        }

        public bool EditRecord(int id, RecordParameterObject record)
        {
            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - Calling {nameof(this.EditRecord)}() with " +
                $"id = '{id}'" +
                $"FirstName = '{record.FirstName}', " +
                $"LastName = '{record.LastName}', " +
                $"DateOfBirth = '{record.DateOfBirth}', " +
                $"Weight = '{record.Weight}', " +
                $"Account = '{record.Account}', " +
                $"Letter = '{record.Letter}'");

            var result = this.service.EditRecord(id, record);

            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - {nameof(this.EditRecord)}() returned '{result}'");

            return result;
        }

        public IReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - Calling {nameof(this.FindByFirstName)}() with " +
                $"firstName = '{firstName}'");

            var result = this.service.FindByFirstName(firstName);

            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - {nameof(this.FindByFirstName)}() returned '{result}'");

            return result;
        }

        public IReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - Calling {nameof(this.FindByLastName)}() with " +
                $"lastName = '{lastName}'");

            var result = this.service.FindByLastName(lastName);

            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - {nameof(this.FindByLastName)}() returned '{result}'");

            return result;
        }

        public IReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - Calling {nameof(this.FindByDateOfBirth)}() with " +
                $"dateOfBirth = '{dateOfBirth}'");

            var result = this.service.FindByDateOfBirth(dateOfBirth);

            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - {nameof(this.FindByDateOfBirth)}() returned '{result}'");

            return result;
        }

        public IReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - Calling {nameof(this.GetRecords)}()");

            var result = this.service.GetRecords();

            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - {nameof(this.GetRecords)}() returned '{result}'");

            return result;
        }

        public int GetStat()
        {
            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - Calling {nameof(this.GetStat)}()");

            var result = this.service.GetStat();

            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - {nameof(this.GetStat)}() returned '{result}'");

            return result;
        }

        public int CountOfRemoved()
        {
            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - Calling {nameof(this.CountOfRemoved)}()");

            var result = this.service.CountOfRemoved();

            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - {nameof(this.CountOfRemoved)}() returned '{result}'");

            return result;
        }

        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - Calling {nameof(this.MakeSnapshot)}()");

            var result = this.service.MakeSnapshot();

            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - {nameof(this.MakeSnapshot)}() returned '{result}'");

            return result;
        }

        public bool Remove(int id)
        {
            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - Calling {nameof(this.Remove)}() with " +
                $"id = '{id}'");

            var result = this.service.Remove(id);

            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - {nameof(this.Remove)}() returned '{result}'");

            return result;
        }

        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - Calling {nameof(this.Restore)}() with " +
                $"snapshot = '{snapshot}'");

            var result = this.service.Restore(snapshot);

            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - {nameof(this.Restore)}() returned '{result}'");

            return result;
        }

        public int Purge()
        {
            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - Calling {nameof(this.Purge)}()");

            var result = this.service.Purge();

            this.textWriter.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm} - {nameof(this.Purge)}() returned '{result}'");

            return result;
        }
    }
}
