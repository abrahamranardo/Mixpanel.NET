using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Linq;

namespace Mixpanel.NET.Events
{
    public class MixpanelTracker : MixpanelClientBase, IEventTracker
    {
        private readonly TrackerOptions _options;

        /// <summary>
        /// Creates a new Mixpanel tracker for a given API token
        /// </summary>
        /// <param name="token">The API token for MixPanel</param>
        /// <param name="http">An implementation of IMixpanelHttp, <see cref="MixpanelHttp"/>
        /// Determines if class names and properties will be serialized to JSON literally.
        /// If false (the default) spaces will be inserted between camel-cased words for improved 
        /// readability on the reporting side.
        /// </param>
        /// <param name="options">Optional: Specific options for the API <see cref="TrackerOptions"/></param>
        public MixpanelTracker(string token, IMixpanelHttp http = null, TrackerOptions options = null)
            : base(token, http)
        {
            _options = options ?? new TrackerOptions();
        }

        public bool Flush()
        {
            var data = new JavaScriptSerializer().Serialize(batch);

            var values = "data=" + data.Base64Encode();
            if (_options.Test) values += "&test=1";

            // For send a batch, we need to use Post method
            var contents = http.Post(Resources.Track(_options.ProxyUrl), values);

            batch.Clear();

            return contents == "1";
        }

        public Dictionary<string, object> PrepareData(string @event, IDictionary<string, object> properties)
        {
            var propertyBag = properties.FormatProperties();
            // Standardize token and time values for Mixpanel
            propertyBag["token"] = token;

            if (_options.SetEventTime && !properties.Keys.Any(x => x.ToLower() == "time"))
                propertyBag["time"] = DateTime.UtcNow.FormatDate();

            return new Dictionary<string, object>
            {
                { "event", @event },
                { "properties", propertyBag }
            };
        }

        public bool Track(string @event, IDictionary<string, object> properties)
        {
            var data = new JavaScriptSerializer().Serialize(PrepareData(@event, properties));

            var values = "data=" + data.Base64Encode();

            if (_options.Test) values += "&test=1";

            var contents = _options.UseGet
              ? http.Get(Resources.Track(_options.ProxyUrl), values)
              : http.Post(Resources.Track(_options.ProxyUrl), values);

            return contents == "1";
        }

        public bool Track(MixpanelEvent @event)
        {
            return Track(@event.Event, @event.Properties);
        }

        public bool Track<T>(T @event)
        {
            return Track(@event.ToMixpanelEvent(_options.LiteralSerialization));
        }

        public bool AddBatch(string @event, IDictionary<string, object> properties)
        {
            if (batch == null)
                batch = new List<Dictionary<string, object>>();

            batch.Add(PrepareData(@event, properties));

            // There's a limitation 50 events for each request
            if (batch.Count() == 50)
            {
                return Flush();
            }

            return true;
        }
    }
}