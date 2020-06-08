using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersistingPoC.Repository.Models.Sql
{
    [Table("TaskStatus", Schema = "dbo")]
    public class TaskStatus
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
