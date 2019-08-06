using System;
using System.Collections.Generic;
using System.Text;
using PeterO;

namespace PeterO.Cbor {
  internal sealed class OptionsParser {
    private readonly IDictionary<string, string> dict = new
Dictionary<string, string>();
    public OptionsParser(string options) {
      if (options == null) {
        throw new ArgumentNullException(nameof(options));
      }
      if (options.Length > 0) {
        string[] optionsArray = options.Split(';');
        foreach (string opt in optionsArray) {
          int index = opt.IndexOf('=');
          if (index < 0) {
            throw new ArgumentException("Invalid options string: " + options);
          }
          string key = DataUtilities.ToLowerCaseAscii(opt.Substring(0, index));
          string value = opt.Substring(index + 1);
          this.dict[key] = value;
        }
      }
    }

    public bool GetBoolean(string key, bool defaultValue) {
      string lckey = DataUtilities.ToLowerCaseAscii(key);
      if (this.dict.ContainsKey(lckey)) {
        string lcvalue = DataUtilities.ToLowerCaseAscii(this.dict[lckey]);
        return lcvalue.Equals("1", StringComparison.Ordinal) ||
lcvalue.Equals("yes", StringComparison.Ordinal) ||
            lcvalue.Equals("on", StringComparison.Ordinal) ||
lcvalue.Equals("true", StringComparison.Ordinal);
      }
      return defaultValue;
    }
  }
}
