using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;

namespace FileCabinetApp.CommandHandlers
{
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        private readonly IRecordPrinter printer;

        public FindCommandHandler(IFileCabinetService fileCabinetService, IRecordPrinter printer)
            : base(fileCabinetService)
        {
            this.printer = printer ?? throw new ArgumentNullException(nameof(printer));
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Command == "find")
            {
                const int IndexPropertyName = 0;
                const int IndexOfTextToSearch = 1;

                if (string.IsNullOrEmpty(request.Parameters))
                {
                    Console.WriteLine($"You should write the parameters.");
                    return null;
                }

                string[] arrayOfParameters = request.Parameters.Split(" ", 2);
                if (arrayOfParameters.Length < 2)
                {
                    Console.WriteLine($"You should write the text to search for.");
                    return null;
                }

                IReadOnlyCollection<FileCabinetRecord> result = new List<FileCabinetRecord>();
                if (string.Equals("firstname", arrayOfParameters[IndexPropertyName], StringComparison.InvariantCultureIgnoreCase))
                {
                    result = this.service.FindByFirstName(arrayOfParameters[IndexOfTextToSearch].Replace("\"", string.Empty));
                }
                else if (string.Equals("lastname", arrayOfParameters[IndexPropertyName], StringComparison.InvariantCultureIgnoreCase))
                {
                    result = this.service.FindByLastName(arrayOfParameters[IndexOfTextToSearch].Replace("\"", string.Empty));
                }
                else if (string.Equals("dateofbirth", arrayOfParameters[IndexPropertyName], StringComparison.InvariantCultureIgnoreCase))
                {
                    if (DateTime.TryParse(arrayOfParameters[IndexOfTextToSearch].Replace("\"", string.Empty), out DateTime dateOfBirth))
                    {
                        result = this.service.FindByDateOfBirth(dateOfBirth);
                    }
                    else
                    {
                        Console.WriteLine($"The following date '{arrayOfParameters[IndexOfTextToSearch].Replace("\"", string.Empty)}' has incorrect format.");
                    }
                }
                else
                {
                    Console.WriteLine($"The '{arrayOfParameters[IndexPropertyName]}' property is not exist.");
                }

                this.printer.Print(result);
            }
            else
            {
                return base.Handle(request);
            }

            return request;
        }
    }
}
