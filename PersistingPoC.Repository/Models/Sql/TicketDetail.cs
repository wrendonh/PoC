using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersistingPoC.Repository.Models.Sql
{
    [Table("ticketdetail")]
    public class TicketDetail
    {
        [PgName("id")]
        public int Id { get; set; }

        [Required]
        [PgName("ticketid")]
        public int TicketId { get; set; }

        [Required]
        [MaxLength(500)]
        [PgName("title")]
        public string Title { get; set; }

        [Required]
        [MaxLength(100)]
        [PgName("time")]
        public string Time { get; set; }

        [Required]
        [PgName("description")]
        public string Description { get; set; }
    }
}
