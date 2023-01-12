using System;
using System.Collections.Generic;
using System.Diagnostics;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;
using FileCabinetApp.Snapshot;

namespace FileCabinetApp.FileCabinetService
{
    public class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService service;
        private readonly Stopwatch stopwatch = new ();

        public ServiceMeter(IFileCabinetService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public int CreateRecord(RecordParameterObject record)
        {
            return this.ExecuteWithTiming(() => this.service.CreateRecord(record), nameof(this.CreateRecord));
        }

        public bool EditRecord(int id, RecordParameterObject record)
        {
            return this.ExecuteWithTiming(() => this.service.EditRecord(id, record), nameof(this.EditRecord));
        }

        public IReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return this.ExecuteWithTiming(() => this.service.FindByFirstName(firstName), nameof(this.FindByFirstName));
        }

        public IReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            return this.ExecuteWithTiming(() => this.service.FindByLastName(lastName), nameof(this.FindByLastName));
        }

        public IReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            return this.ExecuteWithTiming(() => this.service.FindByDateOfBirth(dateOfBirth), nameof(this.FindByDateOfBirth));
        }

        public IReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return this.ExecuteWithTiming(this.service.GetRecords, nameof(this.GetRecords));
        }

        public int GetStat()
        {
            return this.ExecuteWithTiming(this.service.GetStat, nameof(this.GetStat));
        }

        public int CountOfRemoved()
        {
            return this.ExecuteWithTiming(this.service.CountOfRemoved, nameof(this.CountOfRemoved));
        }

        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return this.ExecuteWithTiming(this.service.MakeSnapshot, nameof(this.MakeSnapshot));
        }

        public bool Remove(int id)
        {
            return this.ExecuteWithTiming(() => this.service.Remove(id), nameof(this.Remove));
        }

        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            return this.ExecuteWithTiming(() => this.service.Restore(snapshot), nameof(this.Restore));
        }

        public int Purge()
        {
            return this.ExecuteWithTiming(this.service.Purge, nameof(this.Purge));
        }

        private T ExecuteWithTiming<T>(Func<T> func, string name)
        {
            this.stopwatch.Restart();
            var result = func();
            this.stopwatch.Stop();

            Console.WriteLine($"{name} method execution duration is {this.stopwatch.ElapsedTicks} ticks.");

            return result;
        }
    }
}
