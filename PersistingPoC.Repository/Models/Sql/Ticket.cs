using NpgsqlTypes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersistingPoC.Repository.Models.Sql
{
    [Table("Ticket", Schema="dbo")]
    public class Ticket
    {
        [PgName("id")]
        public int Id { get; set; }

        [Required]
        [PgName("externalticketid")]
        public int ExternalTicketId { get; set; }

        [Required]
        [PgName("companyid")]
        public int CompanyId { get; set; }

        [Required]
        [PgName("integrationtypeid")]
        public int IntegrationTypeId { get; set; }

        [Required]
        [PgName("initialdescription")]
        public string InitialDescription { get; set; }

        [Required]
        [MaxLength(200)]
        [PgName("createdby")]
        public string CreatedBy { get; set; }

        [Required]
        [PgName("createddate")]
        public DateTime CreatedDate { get; set; }

        [Required]
        [MaxLength(500)]
        [PgName("contenttitle")]
        public string ContentTitle { get; set; }

        [PgName("lastupdateddate")]
        public DateTime? LastUpdatedDate { get; set; }

        [MaxLength(200)]
        [PgName("lastupdatedby")]
        public string LastUpdatedBy { get; set; }
    }
}
