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
      : base(token, http) {
      _options = options ?? new EngageOptions();
    }

    private bool Engage(string distinctId, IDictionary<string, object> setProperties = null,
        IDictionary<string,object> setOnceProperties = null, IDictionary<string, object> incrementProperties = null, 
        IDictionary<string, object> appendProperties = null, IDictionary<string, object> transactionProperties = null,
        bool delete = false, string ip = null) {
      // Standardize token and time values for Mixpanel
      var dictionary = 
        new Dictionary<string, object> {{"$token", token}, {"$distinct_id", distinctId}};

      if (!string.IsNullOrWhiteSpace(ip)) dictionary.Add("$ip",ip);

      if (setProperties != null) dictionary.Add("$set", setProperties.FormatProperties());

      if (setOnceProperties != null) dictionary.Add("$set_once", setOnceProperties);

      if (incrementProperties != null) dictionary.Add("$add", incrementProperties);

      if (appendProperties != null)
      {
          if (transactionProperties != null) appendProperties.Add("$transactions", transactionProperties);
          dictionary.Add("$append", appendProperties);
      }

      if (delete) dictionary.Add("$delete", string.Empty);

      var data = new JavaScriptSerializer().Serialize(dictionary);

      var values = "data=" + data.Base64Encode();

      var contents = _options.UseGet
        ? http.Get(Resources.Engage(_options.ProxyUrl), values)
        : http.Post(Resources.Engage(_options.ProxyUrl), values);

      return contents == "1";
    }

    public bool Delete(string distinctId) {
      return Engage(distinctId, delete: true);
    }

    public bool Set(string distinctId, IDictionary<string, object> setProperties, string ip = null) {
      return Engage(distinctId, setProperties, ip: ip);
    }

    public bool SetOnce(string distictId, IDictionary<string, object> setOnceProperties, string ip = null)
    {
      return Engage(distictId, setOnceProperties, ip: ip);
    }

    public bool Increment(string distinctId, IDictionary<string, object> incrementProperties, string ip = null)
    {
      return Engage(distinctId, incrementProperties: incrementProperties, ip: ip);
    }

    public bool Append(string distinctId, IDictionary<string, object> appendProperties, IDictionary<string, object> transactionProperties, string ip = null)
    {
      return Engage(distinctId, appendProperties: appendProperties, transactionProperties: transactionProperties, ip: ip);
    }

    private bool SetAlias(string distinctId, string aliasId)
    {
        // Standardize token and time values for Mixpanel
        var dictionary =
          new Dictionary<string, object> { { "event", "$create_alias" } };

        dictionary.Add("properties", new Dictionary<string, object> { { "distinct_id", distinctId }, { "alias", aliasId }, { "$token", token } });

        var data = new JavaScriptSerializer().Serialize(dictionary);

        var values = "data=" + data.Base64Encode();

        var contents = _options.UseGet
          ? http.Get(Resources.Engage(_options.ProxyUrl), values)
          : http.Post(Resources.Engage(_options.ProxyUrl), values);

        return contents == "1";
    }
  }
}