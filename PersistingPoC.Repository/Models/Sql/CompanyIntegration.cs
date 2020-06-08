using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersistingPoC.Repository.Models.Sql
{
    [Table("CompanyIntegration", Schema = "dbo")]
    public class CompanyIntegration
    {
        public int Id { get; set; }

        [Required]
        public int IntegrationTypeId { get; set; }

        [Required]
        public string IntegrationConfig { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        public bool Enabled { get; set; }

        [ForeignKey(nameof(IntegrationTypeId))]
        public IntegrationType IntegrationType { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }
    }
}
