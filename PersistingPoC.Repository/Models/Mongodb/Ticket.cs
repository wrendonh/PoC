using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace PersistingPoC.Repository.Models.Mongodb
{
    public class Ticket
    {
        [BsonId]
        public string Id { get; set; }

        public int ExternalTicketId { get; set; }              
        public int CompanyId { get; set; }
        public int IntegrationType { get; set; }
        public string InitialDescription { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ContentTitle { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }        

        public List<TicketDetail> Details { get; set; }
    }
}
