using System.Collections.Generic;

namespace PersistingPoC.Service.Dtos.ConnectWise
{
    public class ConfigurationDto : BaseConfigDto
    {
        public string ApiKey { get; set; }
        public string ClientId { get; set; }
        public string Site { get; set; }
        public bool ImportTickets { get; set; }
        public bool ImportConfigs { get; set; }
        public bool ImportConfigDocuments { get; set; }
        public string VersionNumber { get; set; }

        public List<ServiceBoardDto> ServiceBoards { get; set; }
        public List<int> SelectedServiceBoards { get; set; }
        public List<RestrictedConfigTypeDto> RestrictedConfigTypes { get; set; }
        public Dictionary<string, bool> TicketDetails { get; set; }
        public List<string> SelectedTicketDetails { get; set; }
        public List<CompanyTypeDto> CompanyTypes { get; set; }
        public List<string> SelectedCompanyTypes { get; set; }
        public List<CompanyStatusDto> CompanyStatuses { get; set; }
        public List<string> SelectedCompanyStatuses { get; set; }
        public Dictionary<string, bool> TicketExclusions { get; set; }
        public List<string> SelectedTicketExclusions { get; set; }
    }
}
