using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersistingPoC.Repository.Models.Sql
{
    [Table("TaskTypeIntegration", Schema = "dbo")]
    public class TaskTypeIntegration
    {
        public int Id { get; set; }

        [Required]
        public int CompanyIntegrationId { get; set; }

        [Required]
        public int TaskTypeId { get; set; }

        [Required]
        public int TimeToProcess { get; set; }

        [ForeignKey(nameof(CompanyIntegrationId))]
        public CompanyIntegration CompanyIntegration { get; set; }

        [ForeignKey(nameof(TaskTypeId))]
        public TaskType TaskType { get; set; }
    }
}
