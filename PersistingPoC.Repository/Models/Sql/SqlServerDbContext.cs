using Microsoft.EntityFrameworkCore;

namespace PersistingPoC.Repository.Models.Sql
{
    public class SqlServerDbContext : DbContext 
    {
        public SqlServerDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyIntegration> CompanyIntegrations { get; set; }
        public DbSet<IntegrationType> IntegrationTypes { get; set; }
        public DbSet<TaskProcess> TaskProcesses { get; set; }
        public DbSet<TaskStatus> TaskStatuses { get; set; }
        public DbSet<TaskType> TaskTypes { get; set; }
        public DbSet<TaskTypeIntegration> TaskTypeIntegrations { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketDetail> TicketDetails { get; set; }
    }
}
