namespace FileCabinetApp
{
    /// <summary>
    /// Provides methods for working with default file cabinet.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        /// <summary>
        /// Creates a default record validator object.
        /// </summary>
        /// <returns>Default record validator object.</returns>
        public override IRecordValidator CreateValidator()
        {
            return new DefaultValidator();
        }
    }
}
