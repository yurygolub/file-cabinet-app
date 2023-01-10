using System;
using System.Collections.Generic;
using FileCabinetApp.Record;

namespace FileCabinetApp.CommandHandlers
{
    public class FindCommandHandler : CommandHandlerBase
    {
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
                    result = Program.fileCabinetService.FindByFirstName(arrayOfParameters[IndexOfTextToSearch].Replace("\"", string.Empty));
                }
                else if (string.Equals("lastname", arrayOfParameters[IndexPropertyName], StringComparison.InvariantCultureIgnoreCase))
                {
                    result = Program.fileCabinetService.FindByLastName(arrayOfParameters[IndexOfTextToSearch].Replace("\"", string.Empty));
                }
                else if (string.Equals("dateofbirth", arrayOfParameters[IndexPropertyName], StringComparison.InvariantCultureIgnoreCase))
                {
                    if (DateTime.TryParse(arrayOfParameters[IndexOfTextToSearch].Replace("\"", string.Empty), out DateTime dateOfBirth))
                    {
                        result = Program.fileCabinetService.FindByDateOfBirth(dateOfBirth);
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

                Program.PrintRecords(result);
            }
            else
            {
                return base.Handle(request);
            }

            return request;
        }
    }
}
