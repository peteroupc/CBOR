using System;
using System.Linq;
using System.Reflection;
using System.Resources;
using PeterO.Cbor;

namespace Test {
  internal sealed class AppResources {
    private readonly ResourceManager mgr;

    public AppResources(string name) {
      Assembly assembly = typeof(AppResources).Assembly;
      string assemblyName = assembly.FullName;
      int comma = assemblyName.IndexOf(",", StringComparison.Ordinal);
      // Get just the name of the assembly without
      // the parameters
      if (comma >= 0) {
        assemblyName = assemblyName.Substring(0, comma);
      }
      this.mgr = new ResourceManager(assemblyName + "." + name, assembly);
    }

    public CBORObject GetJSON(string name) {
      return CBORObject.FromJSONString(this.mgr.GetString(name));
    }

    public string GetString(string name) {
      return this.mgr.GetString(name);
    }
  }
}
