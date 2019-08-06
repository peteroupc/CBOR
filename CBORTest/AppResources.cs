using System;
using System.Linq;
using System.Reflection;
using System.Resources;
using PeterO.Cbor;

namespace Test {
  internal sealed class AppResources {
    private readonly ResourceManager mgr;

    public AppResources(string name) {
      this.mgr = new ResourceManager(
           name,
           Assembly.GetExecutingAssembly());
    }

    public CBORObject GetJSON(string name) {
      return CBORObject.FromJSONString(this.mgr.GetString(name,
  System.Globalization.CultureInfo.InvariantCulture));
    }

    public string GetString(string name) {
      return this.mgr.GetString(name,
  System.Globalization.CultureInfo.InvariantCulture);
    }
  }
}
