using ConnectWiseDotNetSDK.ConnectWise.Client;
using Newtonsoft.Json;
using PersistingPoC.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersistingPoC.Service.Services.ConnectWise
{
    public class BaseService
    {
        public ApiClient Client { get; set; }
        public string Url { get; set; }
        protected string ClientId { get; set; }
        protected string ApiKey { get; set; }

        protected static T FormatResponse<T>(Response resp)
        {
            var content = resp.GetRawResponseBody();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                Formatting = Formatting.None,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                FloatParseHandling = FloatParseHandling.Decimal,
                Converters = new List<JsonConverter> { new CustomIntConverter() },
                Error = (sender, args) =>
                {
                    //anything missing we can figurout when it becomes important
                    args.ErrorContext.Handled = true;
                }
            };
            var d = JsonConvert.DeserializeObject<T>(content, settings);
            return d;
        }

        public virtual void InitializeClient(string apiKey, string clientId, string site)
        {
            var info = Encoding.ASCII.GetString(Convert.FromBase64String(apiKey)).Split('+', ':');

            Client = new ApiClient(clientId, null, site, info[0]);
            //need to add this header so that we always receive a certain version of the API and new, breaking changes will not be implemented without our knowledge
            //https://developer.connectwise.com/Manage/Developer_Guide
            Client.DefaultHeaders.TryAdd("Accept", "application/vnd.connectwise.com+json; version=3.0.0");

            Client.SetPublicPrivateKey(info[1], info[2], string.Empty);
            Url = site;
            ApiKey = apiKey;
            ClientId = clientId;

            if (!Url.Contains("http"))
            {
                Url = $"https://{Url}";
            }
        }
    }
}
