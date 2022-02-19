using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;
using FileCabinetApp.Snapshot;

namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// Provides methods for working with file cabinet using database file.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService, IDisposable
    {
        private const int RecordSize = 278;
        private readonly FileStream fileStream;

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary;
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary;
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">FileStream for working with database.</param>
        public FileCabinetFilesystemService(FileStream fileStream)
        {
            this.fileStream = fileStream;

            this.firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
            this.lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
            this.dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            byte[] recordBuffer = new byte[RecordSize];
            while (this.fileStream.Read(recordBuffer, 0, RecordSize) > 0)
            {
                records.Add(BytesToRecord(recordBuffer));
            }

            LoadDictionary(this.firstNameDictionary, records, (record) => record.FirstName);
            LoadDictionary(this.lastNameDictionary, records, (record) => record.LastName);
            LoadDictionary(this.dateOfBirthDictionary, records, (record) => record.DateOfBirth);

            static void LoadDictionary<T>(Dictionary<T, List<FileCabinetRecord>> dictionary, IEnumerable<FileCabinetRecord> records, Func<FileCabinetRecord, T> getKey)
            {
                foreach (var record in records)
                {
                    if (dictionary.ContainsKey(getKey(record)))
                    {
                        dictionary[getKey(record)].Add(record);
                    }
                    else
                    {
                        List<FileCabinetRecord> newList = new List<FileCabinetRecord>
                        {
                            record,
                        };
                        dictionary.Add(getKey(record), newList);
                    }
                }
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        ~FileCabinetFilesystemService()
        {
            this.Dispose(false);
        }

        /// <inheritdoc/>
        public int CreateRecord(RecordParameterObject record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            int length = (int)this.fileStream.Length;
            int id = (length / RecordSize) + 1;

            byte[] bytes = RecordToBytes(id, record);
            this.fileStream.Position = this.fileStream.Length;
            this.fileStream.Write(bytes);
            this.fileStream.Flush();

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

            UpdateDictionary(this.firstNameDictionary, fileCabinetRecord, fileCabinetRecord.FirstName);
            UpdateDictionary(this.lastNameDictionary, fileCabinetRecord, fileCabinetRecord.LastName);
            UpdateDictionary(this.dateOfBirthDictionary, fileCabinetRecord, fileCabinetRecord.DateOfBirth);

            return id;

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
        public void EditRecord(int id, RecordParameterObject record)
        {
            this.IsRecordExist(id);

            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            int recordPosition = RecordSize * (id - 1);

            this.fileStream.Position = recordPosition;
            byte[] recordBuffer = new byte[RecordSize];
            this.fileStream.Read(recordBuffer, 0, RecordSize);
            FileCabinetRecord oldRecord = BytesToRecord(recordBuffer);

            this.fileStream.Position = recordPosition;
            byte[] bytes = RecordToBytes(id, record);
            this.fileStream.Write(bytes);
            this.fileStream.Flush();

            FileCabinetRecord newRecord = new FileCabinetRecord
            {
                Id = id,
                FirstName = record.FirstName,
                LastName = record.LastName,
                DateOfBirth = record.DateOfBirth,
                Weight = record.Weight,
                Account = record.Account,
                Letter = record.Letter,
            };

            UpdateDictionary(this.firstNameDictionary, newRecord, oldRecord.FirstName, newRecord.FirstName);
            UpdateDictionary(this.firstNameDictionary, newRecord, oldRecord.LastName, newRecord.LastName);
            UpdateDictionary(this.dateOfBirthDictionary, newRecord, oldRecord.DateOfBirth, newRecord.DateOfBirth);

            static void UpdateDictionary<T>(Dictionary<T, List<FileCabinetRecord>> dictionary, FileCabinetRecord newRecord, T oldValue, T newValue)
                where T : IEquatable<T>
            {
                if (!oldValue.Equals(newValue))
                {
                    dictionary[oldValue].Remove(newRecord);
                    if (dictionary.ContainsKey(newValue))
                    {
                        dictionary[newValue].Add(newRecord);
                        dictionary[newValue].Sort((firstValue, secondValue) => firstValue.Id.CompareTo(secondValue.Id));
                    }
                    else
                    {
                        List<FileCabinetRecord> records = new List<FileCabinetRecord>
                        {
                            newRecord,
                        };
                        dictionary.Add(newValue, records);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            byte[] recordBuffer = new byte[RecordSize];
            this.fileStream.Position = 0;
            while (this.fileStream.Read(recordBuffer, 0, RecordSize) > 0)
            {
                records.Add(BytesToRecord(recordBuffer));
            }

            return records.ToArray();
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            int length = (int)this.fileStream.Length;
            return length / RecordSize;
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
        public void IsRecordExist(int id)
        {
            int length = (int)this.fileStream.Length;
            int count = length / RecordSize;
            if (id < 1 || id > count)
            {
                throw new ArgumentException($"#{id} record is not found.");
            }
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.GetRecords());
        }

        /// <summary>
        /// Releses unmanaged file resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releses unmanaged file resources.
        /// </summary>
        /// <param name="disposing">Indicates whether the method call comes from a Dispose method or from a finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.fileStream.Dispose();
            }

            this.disposed = true;
        }

        private static byte[] RecordToBytes(int id, RecordParameterObject record)
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.Unicode);

            binaryWriter.Seek(sizeof(short), SeekOrigin.Current);
            binaryWriter.Write(id);

            WriteFixedLengthString(binaryWriter, record.FirstName);
            WriteFixedLengthString(binaryWriter, record.LastName);

            binaryWriter.Write(record.DateOfBirth.Year);
            binaryWriter.Write(record.DateOfBirth.Month);
            binaryWriter.Write(record.DateOfBirth.Day);
            binaryWriter.Write(record.Weight);
            binaryWriter.Write(record.Account);
            binaryWriter.Write(record.Letter);

            return memoryStream.ToArray();

            static void WriteFixedLengthString(BinaryWriter binaryWriter, string line)
            {
                const int MaxSize = 60;
                char[] buffer = new char[MaxSize];
                line.ToCharArray().CopyTo(buffer, 0);
                binaryWriter.Write(buffer);
            }
        }

        private static FileCabinetRecord BytesToRecord(byte[] bytes)
        {
            using MemoryStream memoryStream = new MemoryStream(bytes);
            using BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.Unicode);
            binaryReader.BaseStream.Seek(sizeof(short), SeekOrigin.Current);

            int id = binaryReader.ReadInt32();
            string firstName = ReadFixedLengthString(binaryReader);
            string lastName = ReadFixedLengthString(binaryReader);
            int year = binaryReader.ReadInt32();
            int month = binaryReader.ReadInt32();
            int day = binaryReader.ReadInt32();
            short weight = binaryReader.ReadInt16();
            decimal account = binaryReader.ReadDecimal();
            char letter = binaryReader.ReadChar();

            FileCabinetRecord fileCabinetRecord = new FileCabinetRecord()
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = new DateTime(year, month, day),
                Weight = weight,
                Account = account,
                Letter = letter,
            };

            return fileCabinetRecord;

            static string ReadFixedLengthString(BinaryReader binaryReader)
            {
                const int MaxSize = 60;
                char[] buffer = new char[MaxSize];
                binaryReader.Read(buffer);
                int count = 0;
                while (buffer[count] != 0)
                {
                    count++;
                }

                return new string(buffer[..count]);
            }
        }
    }
}
