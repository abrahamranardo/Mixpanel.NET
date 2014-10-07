using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mixpanel.NET.Export
{
    public interface IExport
    {
        string Export(string method, long expire, IDictionary<string, object> properties, string format);
    }
}
