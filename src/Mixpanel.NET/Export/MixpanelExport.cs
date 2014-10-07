using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mixpanel.NET.Export
{
    public class MixpanelExport : IExport
    {
        private ExportOptions _options;
        private IMixpanelHttp _http;
        private string _apiKey;
        private string _apiSecret;

        public MixpanelExport(string apiKey, string apiSecret, IMixpanelHttp http = null, ExportOptions options = null)
        {
            this._http = http ?? new MixpanelHttp();
            this._apiKey = apiKey;
            this._apiSecret = apiSecret;
            this._options = options ?? new ExportOptions();
        }

        public string Export(string method, long expire, IDictionary<string, object> properties, string format = "json")
        {
            properties.Add("api_key", _apiKey);
            properties.Add("expire", expire);
            properties.Add("format", format);

            var signature = properties.ComputeSignature(_apiSecret);
            properties.Add("sig", signature);

            var query = "";
            foreach (var value in properties.OrderBy(v => v.Key))
            {
                if (!string.IsNullOrWhiteSpace(query))
                    query += "&";

                query += value.Key + "=" + Uri.EscapeUriString(value.Value.ToString());
            }

            return _http.Get(Resources.Export(method, _options.ProxyUrl), query);
        }
    }
}
