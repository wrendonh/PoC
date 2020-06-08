using ConnectWiseDotNetSDK.ConnectWise.Client.Company.Model;
using ConnectWiseDotNetSDK.ConnectWise.Client.Service.Model;
using ConnectWiseDotNetSDK.ConnectWise.Client.Time.Model;
using PersistingPoC.Service.Dtos.ConnectWise;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersistingPoC.Service.Interfaces.ConnectWise
{
    public interface IUtilityService
    {
        string GetTicketQueryFactory(string direction, DateTime IndexingStartDate, DateTime HistoricalDataCutoff, ConfigurationDto CWConfig);
        Dictionary<string, string> GetProperties(Ticket ticket, ConfigurationDto connectWiseConfigDto, Dictionary<string, string> properties);
        Task<List<Company>> GetCompanies(ConfigurationDto cwConfig, int integrationId);
        DateTime GetTicketDateEntered(Ticket ticket);
        void InitializeClient(string apiKey, string clientId, string site);
        Task<IList<TimeEntry>> GetTimeEntriesFilteredAsync(string conditions, string orderBy, int? page, int? take);
        Task<IList<ServiceNote>> GetServiceNotesFilteredAsync(int? id, string conditions, string orderBy, int? page, int? take);
    }
}
