using System;
using System.Reflection;
using System.Resources;
using PeterO.Cbor;

namespace Test {
  internal sealed class AppResources {
    private readonly ResourceManager mgr;

    public AppResources(string name) {
#if NET20 || NET40
      this.mgr = new ResourceManager(
          "Resources",
          Assembly.GetExecutingAssembly());
#else
      this.mgr = new ResourceManager(typeof(AppResources));
#endif
    }

    public CBORObject GetJSON(string name) {
      return CBORObject.FromJSONString(GetString(name));
    }

    public string GetString(string name) {
      return this.mgr.GetString(name);
    }
  }
}
