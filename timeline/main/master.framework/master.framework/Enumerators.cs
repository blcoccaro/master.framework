namespace master.framework
{
    public class Enumerators
    {
        public enum TimeStampType
        {
            Native,
            Custom
        }
        public enum TimeStampAction
        {
            Start,
            End,
            Middle,
        }
        public enum HistoricalType
        {
            ByProperty,
            ByObject,
        }
        public enum CreatedOrUpdatedType
        {
            User,
            Date
        }
        public enum CreatedType
        {
            User,
            Date
        }
        public enum UpdatedType
        {
            User,
            Date
        }
        public enum WinServiceCommand
        {
            //StartTrace = 128,
            Beacon = 129,
            StartDebugMode = 130,
            StopDebugMode = 131,
        }
        public enum Database
        {
            Unknow,
            SQLServer,
            Oracle,
            PostgreSQL,
            DB2,
            MySQL
        }

        public enum EntityStatus
        {
            Add,
            Changed,
            Delete
        }

        public enum LogType
        {
            Debug,
            Info,
            Warn,
            Fatal,
            Error,
        }
    }
}
