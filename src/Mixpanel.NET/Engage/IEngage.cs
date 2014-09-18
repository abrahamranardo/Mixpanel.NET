using System.Collections.Generic;

namespace Mixpanel.NET.Engage {
  public interface IEngage {
      bool Delete(string distinctId, bool addToBatch);
      bool Set(string distinctId, IDictionary<string, object> setProperties, string ip, bool addToBatch);
      bool SetOnce(string distinctId, IDictionary<string, object> setOnceProperties, string ip, bool addToBatch);
      bool Increment(string distinctId, IDictionary<string, object> incrementProperties, string ip, bool addToBatch);
      bool Append(string distinctId, IDictionary<string, object> appendProperties, IDictionary<string, object> transactionProperties, string ip, bool addToBatch);
      bool SetAlias(string distinctId, string aliasId, bool addToBatch);
  }
}