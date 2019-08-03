using System;
using System.Reflection;
using System.Resources;
using PeterO.Cbor;

namespace Test {
  internal sealed class AppResources {
    private readonly ResourceManager mgr;
#pragma warning disable CA1801 // name used in one circumstance
    public AppResources(string name) {
      // #if NET20 || NET40
      this.mgr = new ResourceManager(
          name,
          Assembly.GetExecutingAssembly());
      // #else
      // this.mgr = new ResourceManager(typeof(AppResources));
      // #endif
    }
#pragma warning restore CA1801

    public CBORObject GetJSON(string name) {
      return CBORObject.FromJSONString(
          this.GetString(name));
    }

    public string GetString(string name) {
      return this.mgr.GetString(name,
         System.Globalization.CultureInfo.InvariantCulture);
    }
  }
}
