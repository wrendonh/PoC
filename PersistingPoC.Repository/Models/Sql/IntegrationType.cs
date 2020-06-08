using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersistingPoC.Repository.Models.Sql
{
    [Table("IntegrationType", Schema = "dbo")]
    public class IntegrationType
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
    }
}
