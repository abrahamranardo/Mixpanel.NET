using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Mixpanel.NET.Engage {
    public class MixpanelEngage : MixpanelClientBase, IEngage {
        private readonly EngageOptions _options;

        /// <summary>
        /// Creates a new Mixpanel Engage client for a given API token
        /// </summary>
        /// <param name="token">The API token for MixPanel</param>
        /// <param name="http">Optional: An implementation of IMixpanelHttp, <see cref="MixpanelHttp"/>
        /// Determines if class names and properties will be serialized to JSON literally.
        /// If false (the default) spaces will be inserted between camel-cased words for improved 
        /// readability on the reporting side.
        /// </param>
        /// <param name="options">Optional: Specific options for the API <see cref="EngageOptions"/></param>
        public MixpanelEngage(string token, IMixpanelHttp http = null, EngageOptions options = null)
        : base(token, http) 
        {
            _options = options ?? new EngageOptions();
        }

        private Dictionary<string, object> PrepareData(string distinctId,
            IDictionary<string, object> setProperties = null,
            IDictionary<string, object> setOnceProperties = null,
            IDictionary<string, object> incrementProperties = null,
            IDictionary<string, object> appendProperties = null,
            IDictionary<string, object> transactionProperties = null,
            bool delete = false, string aliasId = null,
            string ip = null, bool ignoreAlias = false, bool ignoreTime = false)
        {
            Dictionary<string, object> dictionary = null;

            if (aliasId == null)
            {
                dictionary = new Dictionary<string, object> { { "$token", token }, { "$distinct_id", distinctId } };

                if (!string.IsNullOrWhiteSpace(ip))
                    dictionary.Add("$ip", ip);

                if (setProperties != null)
                    dictionary.Add("$set", setProperties.FormatProperties());

                if (setOnceProperties != null)
                    dictionary.Add("$set_once", setOnceProperties.FormatProperties());

                if (incrementProperties != null)
                    dictionary.Add("$add", incrementProperties.FormatProperties());

                if (appendProperties != null)
                {
                    if (transactionProperties != null)
                        appendProperties.Add("$transactions", transactionProperties.FormatProperties());

                    dictionary.Add("$append", appendProperties.FormatProperties());
                }

                if (ignoreTime)
                    dictionary.Add("$ignore_time", "true");

                if (ignoreAlias)
                    dictionary.Add("$ignore_alias", "true");

                if (delete)
                    dictionary.Add("$delete", string.Empty);
            }
            else
            {
                dictionary = new Dictionary<string, object> { { "event", "$create_alias" } };

                dictionary.Add("properties", new Dictionary<string, object> { { "distinct_id", distinctId }, { "alias", aliasId }, { "$token", token } });
            }

            return dictionary;
        }

        private bool Engage(string distinctId, 
            IDictionary<string, object> setProperties = null,
            IDictionary<string, object> setOnceProperties = null,
            IDictionary<string, object> incrementProperties = null,
            IDictionary<string, object> appendProperties = null, 
            IDictionary<string, object> transactionProperties = null,
            bool delete = false, string aliasId = null,
            string ip = null, bool ignoreAlias = false, bool ignoreTime = false) 
        {
            // Standardize token and time values for Mixpanel
            var data = new JavaScriptSerializer().Serialize(
                PrepareData(distinctId, setProperties, setOnceProperties, 
                    incrementProperties, appendProperties, transactionProperties, 
                    delete, aliasId, ip, ignoreAlias, ignoreTime));

            var values = "data=" + data.Base64Encode();

            var contents = _options.UseGet
                ? http.Get(Resources.Engage(_options.ProxyUrl), values)
                : http.Post(Resources.Engage(_options.ProxyUrl), values);

            return contents == "1";
        }

        public bool Delete(string distinctId, bool ignoreAlias = false, bool addToBatch = false) 
        {
            if (addToBatch)
                return AddBatch(distinctId, ignoreAlias:ignoreAlias, delete: true);
            else
                return Engage(distinctId, ignoreAlias:ignoreAlias, delete: true);
        }

        public bool Set(string distinctId, IDictionary<string, object> setProperties,
            string ip = null, bool ignoreAlias = false, bool ignoreTime = false, bool addToBatch = false) 
        {
            if(addToBatch)
                return AddBatch(distinctId, setProperties, ip: ip, ignoreAlias:ignoreAlias, ignoreTime: ignoreTime);
            else
                return Engage(distinctId, setProperties, ip: ip, ignoreAlias:ignoreAlias, ignoreTime: ignoreTime);
        }

        public bool SetOnce(string distictId, IDictionary<string, object> setOnceProperties,
            string ip = null, bool ignoreAlias = false, bool ignoreTime = false, bool addToBatch = false)
        {
            if (addToBatch)
                return AddBatch(distictId, setOnceProperties: setOnceProperties, ip: ip, ignoreAlias: ignoreAlias, ignoreTime: ignoreTime);
            else
                return Engage(distictId, setOnceProperties: setOnceProperties, ip: ip, ignoreAlias: ignoreAlias, ignoreTime: ignoreTime);
        }

        public bool Increment(string distinctId, IDictionary<string, object> incrementProperties,
            string ip = null, bool ignoreAlias = false, bool ignoreTime = false, bool addToBatch = false)
        {
            if(addToBatch)
                return AddBatch(distinctId, incrementProperties: incrementProperties, ip: ip, ignoreAlias: ignoreAlias, ignoreTime: ignoreTime);
            else
                return Engage(distinctId, incrementProperties: incrementProperties, ip: ip, ignoreAlias: ignoreAlias, ignoreTime: ignoreTime);
        }

        public bool Append(string distinctId, IDictionary<string, object> appendProperties, IDictionary<string, object> transactionProperties,
            string ip = null, bool ignoreAlias = false, bool ignoreTime = false, bool addToBatch = false)
        {
            if(addToBatch)
                return AddBatch(distinctId, appendProperties: appendProperties, transactionProperties: transactionProperties, ip: ip, ignoreAlias:ignoreAlias, ignoreTime: ignoreTime);
            else
                return Engage(distinctId, appendProperties: appendProperties, transactionProperties: transactionProperties, ip: ip, ignoreAlias:ignoreAlias, ignoreTime: ignoreTime);
        }

        public bool SetAlias(string distinctId, string aliasId, bool ignoreTime = false, bool addToBatch = false)
        {
            if (addToBatch)
                return AddBatch(distinctId, aliasId: aliasId, ignoreTime: ignoreTime);
            else
                return Engage(distinctId, aliasId: aliasId, ignoreTime: ignoreTime);
        }

        private bool AddBatch(string distinctId,
            IDictionary<string, object> setProperties = null,
            IDictionary<string, object> setOnceProperties = null,
            IDictionary<string, object> incrementProperties = null,
            IDictionary<string, object> appendProperties = null,
            IDictionary<string, object> transactionProperties = null,
            bool delete = false, string aliasId = null,
            string ip = null, bool ignoreAlias = false, bool ignoreTime = false)
        {
            if (batch == null)
                batch = new List<Dictionary<string, object>>();

            batch.Add(PrepareData(distinctId, setProperties, setOnceProperties,
                incrementProperties, appendProperties, transactionProperties,
                delete, aliasId, ip, ignoreAlias, ignoreTime));

            // There's a limitation 50 events for each request
            if (batch.Count == 50)
            {
                return Flush();
            }

            return true;
        }

        public bool Flush()
        {
            var data = new JavaScriptSerializer().Serialize(batch);

            var values = "data=" + data.Base64Encode();

            // For send a batch, we need to use Post method
            var contents = http.Post(Resources.Engage(_options.ProxyUrl), values);

            batch.Clear();

            return contents == "1";
        }
    }
}