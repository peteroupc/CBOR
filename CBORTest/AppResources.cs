using System;
using System.Linq;
using System.Reflection;
using System.Resources;
using PeterO.Cbor;

namespace Test {
  internal sealed class AppResources {
    private readonly ResourceManager mgr;

    public AppResources(string name) {
      this.mgr = new ResourceManager(this.GetType());
    }

    public CBORObject GetJSON(string name) {
      return CBORObject.FromJSONString(this.mgr.GetString(name));
    }

    public string GetString(string name) {
      return this.mgr.GetString(name);
    }
  }
}
