namespace PersistingPoC.Entities
{
    public static class Enums
    {
        public enum TaskStatuses
        {
            Pending = 0,
            InProgress = 1,
            Finished = 2,
            Failed = 3
        }

        public enum TaskTypes
        {
            Backward = 1,
            Forward = 2
        }

        public enum IntegrationTypes
        {
            ConnectWise = 1
        }

        public enum StorageTypes
        {
            SqlServer = 1,
            MongoDb = 2,
            PostgreSql = 3,
            Redis = 4
        }

        public enum TicketStatuses
        {
            Created = 1,
            Updated = 2,
            Deleted = 3
        }
    }
}
