using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PersistingPoC.Repository.Models.Mongodb
{
    public class TicketDetail
    {
        [BsonId]
        public string Id { get; set; }

        public string Title { get; set; }
        public string Time { get; set; }
        public string Description { get; set; }
    }
}
