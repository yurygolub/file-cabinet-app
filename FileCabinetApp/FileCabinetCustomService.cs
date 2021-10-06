namespace FileCabinetApp
{
    /// <summary>
    /// Provides methods for working with custom file cabinet.
    /// </summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>
        /// Creates a custom record validator object.
        /// </summary>
        /// <returns>Custom record validator object.</returns>
        public override IRecordValidator CreateValidator()
        {
            return new CustomValidator();
        }
    }
}
