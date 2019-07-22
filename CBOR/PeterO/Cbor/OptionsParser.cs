using System;
using System.Collections.Generic;
using System.Text;
using PeterO;

namespace PeterO.Cbor {
  internal sealed class OptionsParser {
    private readonly IDictionary<string, string> dict = new
Dictionary<string, string>();
    public OptionsParser(string options) {
      string[] optionsArray = options.Split(';');
      foreach (string opt in optionsArray) {
        int index=opt.IndexOf('=');
if (index< 0) {
          throw new ArgumentException("Invalid options string: " + options);
        }
        string key = DataUtilities.ToLowerCaseAscii(opt.Substring(0, index));
        string value = opt.Substring(index + 1);
        dict[key]=value;
      }
    }

    public bool GetBoolean(string key, bool defaultValue) {
      string lckey = DataUtilities.ToLowerCaseAscii(key);
      if (dict.ContainsKey(lckey)) {
        string lcvalue = DataUtilities.ToLowerCaseAscii(dict[lckey]);
        return lcvalue.Equals("1") || lcvalue.Equals("yes") ||
            lcvalue.Equals("on") || lcvalue.Equals("true");
      }
      return defaultValue;
    }
  }
}
