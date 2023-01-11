using System;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    public static class ValidatorBuilderExtensions
    {
        public static IRecordValidator CreateDefault(this ValidatorBuilder validatorBuilder)
        {
            return new ValidatorBuilder()
                .ValidateFirstName(2, 30)
                .ValidateLastName(2, 30)
                .ValidateDateOfBirth(new DateTime(1930, 1, 1), DateTime.Now)
                .ValidateWeight(10, 400)
                .ValidateAccount(0, 1_000)
                .ValidateLetter()
                .Create();
        }

        public static IRecordValidator CreateCustom(this ValidatorBuilder validatorBuilder)
        {
            return new ValidatorBuilder()
                .ValidateFirstName(2, 60)
                .ValidateLastName(2, 60)
                .ValidateDateOfBirth(new DateTime(1950, 1, 1), DateTime.Now)
                .ValidateWeight(10, 500)
                .ValidateAccount(0, 10_000)
                .ValidateLetter()
                .Create();
        }
    }
}
