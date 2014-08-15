using System.Collections.Generic;
namespace Mixpanel.NET
{
    public abstract class MixpanelClientBase
    {
        protected IMixpanelHttp http;
        protected string token;
        protected List<Dictionary<string, object>> batch;

        protected MixpanelClientBase(string token, IMixpanelHttp http = null)
        {
            this.http = http ?? new MixpanelHttp();
            this.token = token;
            this.batch = new List<Dictionary<string, object>>();
        }
    }
}