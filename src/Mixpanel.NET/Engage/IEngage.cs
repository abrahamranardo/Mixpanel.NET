using System.Collections.Generic;

namespace Mixpanel.NET.Engage {
  public interface IEngage {
      bool Delete(string distinctId, bool addToBatch);
      bool Set(string distinctId, IDictionary<string, object> setProperties, string ip, bool addToBatch, bool ignoreTime);
      bool SetOnce(string distinctId, IDictionary<string, object> setOnceProperties, string ip, bool addToBatch, bool ignoreTime);
      bool Increment(string distinctId, IDictionary<string, object> incrementProperties, string ip, bool addToBatch, bool ignoreTime);
      bool Append(string distinctId, IDictionary<string, object> appendProperties, IDictionary<string, object> transactionProperties,
          string ip, bool addToBatch, bool ignoreTime);
      bool SetAlias(string distinctId, string aliasId, bool addToBatch, bool ignoreTime);
  }
}