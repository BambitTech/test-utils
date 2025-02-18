namespace Bambit.TestUtility.DatabaseTools.Reqnroll
{
    /// <summary>
    /// Represents an object that has been mocked in the database
    /// </summary>
    public class MockedObject
    {
        /// <summary>
        /// The name of the connection the mocked object belongs to
        /// </summary>
        public string ConnectionName { get; set; } = null!;

        /// <summary>
        /// The original schema of the mocked object
        /// </summary>
        public string OriginalSchema { get; set; } = null!;

        /// <summary>
        /// The original name of the mocked object
        /// </summary>
        public string OriginalName { get; set; } = null!;

        /// <summary>
        /// what the object has been renamed to
        /// </summary>
        public string NewName { get; set; } = null!;

        /// <summary>
        ///  Scripts needed to restore the mocked object
        /// </summary>
        public List<string> RestoreScripts { get; set; } = null!;
        /// <summary>
        /// The type of object that is mocked
        /// </summary>
        public DatabseObjectType DatabaseObjectType { get; set; }
    }
}
