namespace Mixpanel.NET {
    public static class Resources {
        public static string MixpanelAPIUrl { get { return "http://api.mixpanel.com"; } }
        public static string MixpanelWebUrl { get { return "http://mixpanel.com"; } }

        public static string Track(string proxy = null) {
            proxy = proxy ?? MixpanelAPIUrl;
            return proxy + "/track";
        }

        public static string Engage(string proxy = null) {
            proxy = proxy ?? MixpanelAPIUrl;
            return proxy + "/engage";
        }

        public static string Export(string method, string proxy = null)
        {
            proxy = proxy ?? MixpanelWebUrl;
            return proxy + "/api/2.0/" + method +"/";
        }
    }
}