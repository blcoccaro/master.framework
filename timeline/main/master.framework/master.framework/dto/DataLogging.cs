namespace master.framework.dto
{
    /// <summary>
    /// Data Logging
    /// </summary>
    public class DataLogging
    {
        /// <summary>
        /// Message from Exception
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// StackTrace from Exception
        /// </summary>
        public string StackTrace { get; set; }
        /// <summary>
        /// Source from Exception
        /// </summary>
        public string Source { get; set; }
    }
}
