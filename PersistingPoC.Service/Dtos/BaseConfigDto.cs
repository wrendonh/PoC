using System;

namespace PersistingPoC.Service.Dtos
{
    public class BaseConfigDto
    {
        public int HistoricalDataCutoffValue { get; set; }

        public string HistoricalDataCutoffUnit { get; set; }

        public DateTime HistoricalDataCutoff
        {
            get
            {
                return HistoricalDataCutoffUnit switch
                {
                    "year" => DateTime.Now.AddYears(-1 * HistoricalDataCutoffValue),
                    null => DateTime.Now.AddYears(-3),
                    _ => DateTime.Now.AddMonths(-1 * HistoricalDataCutoffValue),
                };
            }
        }
    }
}
