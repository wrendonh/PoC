using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersistingPoC.Repository.Models.Sql
{
    [Table("Company", Schema = "dbo")]
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        public bool Enabled { get; set; }
    }
}
