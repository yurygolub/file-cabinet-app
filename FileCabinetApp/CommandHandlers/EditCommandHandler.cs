﻿using System;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;

namespace FileCabinetApp.CommandHandlers
{
    public class EditCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        public EditCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Command == "edit")
            {
                if (!int.TryParse(request.Parameters, out int id))
                {
                    Console.WriteLine($"The '{request.Parameters}' is incorrect parameters");
                    return null;
                }

                try
                {
                    this.fileCabinetService.IsRecordExist(id);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }

                Program.InputRecord(out RecordParameterObject record);
                this.fileCabinetService.EditRecord(id, record);
                Console.WriteLine($"Record #{id} is updated.");
            }
            else
            {
                return base.Handle(request);
            }

            return request;
        }
    }
}
