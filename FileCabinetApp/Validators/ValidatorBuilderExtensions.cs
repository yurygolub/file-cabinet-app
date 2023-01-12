using System;
using FileCabinetApp.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp.Validators
{
    public static class ValidatorBuilderExtensions
    {
        public static IRecordValidator CreateDefault(this ValidatorBuilder validatorBuilder, IConfiguration configuration)
        {
            return validatorBuilder
                .ValidateFirstName(int.Parse(configuration["default:firstname:min"]), int.Parse(configuration["default:firstname:max"]))
                .ValidateLastName(int.Parse(configuration["default:lastname:min"]), int.Parse(configuration["default:lastname:max"]))
                .ValidateDateOfBirth(DateTime.Parse(configuration["default:dateOfBirth:from"]), DateTime.Parse(configuration["default:dateOfBirth:to"]))
                .ValidateWeight(short.Parse(configuration["default:weight:min"]), short.Parse(configuration["default:weight:max"]))
                .ValidateAccount(decimal.Parse(configuration["default:account:min"]), decimal.Parse(configuration["default:account:max"]))
                .ValidateLetter()
                .Create();
        }

        public static IRecordValidator CreateCustom(this ValidatorBuilder validatorBuilder, IConfiguration configuration)
        {
            return validatorBuilder
                .ValidateFirstName(int.Parse(configuration["custom:firstname:min"]), int.Parse(configuration["custom:firstname:max"]))
                .ValidateLastName(int.Parse(configuration["custom:lastname:min"]), int.Parse(configuration["custom:lastname:max"]))
                .ValidateDateOfBirth(DateTime.Parse(configuration["custom:dateOfBirth:from"]), DateTime.Parse(configuration["custom:dateOfBirth:to"]))
                .ValidateWeight(short.Parse(configuration["custom:weight:min"]), short.Parse(configuration["custom:weight:max"]))
                .ValidateAccount(decimal.Parse(configuration["custom:account:min"]), decimal.Parse(configuration["custom:account:max"]))
                .ValidateLetter()
                .Create();
        }
    }
}
