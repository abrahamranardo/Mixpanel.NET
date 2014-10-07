using Mixpanel.NET.Events;

namespace Mixpanel.NET.Engage {
    public class EngageOptions : MixpanelClientOptions 
    {
        /// <summary>
        /// If true then the ignore_time flag is used when sending data to Mixpanel.
        /// default: true
        /// </summary>
        public bool IgnoreTime = true;

        /// <summary>
        /// If true then the ignore_alias flag is used when sending data to Mixpanel.
        /// default: true
        /// </summary>
        public bool IgnoreAlias = true;
    }
}