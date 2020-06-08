using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static PersistingPoC.Entities.Enums;

namespace PersistingPoC.Repository.Models.Sql
{
    [Table("TaskProcess", Schema = "dbo")]
    public class TaskProcess
    {
        public int Id { get; set; }

        [Required]
        public int TaskTypeIntegrationId { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }

        [Required]
        public DateTime? EndDate { get; set; }

        [Required]
        public TaskStatuses Status { get; set; }

        [Required]
        public int TotalProcessed { get; set; }

        [ForeignKey(nameof(TaskTypeIntegrationId))]
        public TaskTypeIntegration TaskTypeIntegration { get; set; }
    }
}
