using System;
using System.Collections.Generic;

namespace PeterO.Cbor {
  internal sealed class OptionsParser {
    private readonly IDictionary<string, string> dict = new
    Dictionary<string, string>();

    private static string[] SplitAt(string str, string delimiter) {
      if (delimiter == null) {
        throw new ArgumentNullException(nameof(delimiter));
      }
      if (delimiter.Length == 0) {
        throw new ArgumentException("delimiter is empty.");
      }
      if (String.IsNullOrEmpty(str)) {
        return new[] { String.Empty };
      }
      var index = 0;
      var first = true;
      List<string> strings = null;
      int delimLength = delimiter.Length;
      while (true) {
        int index2 = str.IndexOf(
          delimiter,
          index,
          StringComparison.Ordinal);
        if (index2 < 0) {
          if (first) {
            var strret = new string[1];
            strret[0] = str;
            return strret;
          }
          strings = strings ?? new List<string>();
          strings.Add(str.Substring(index));
          break;
        } else {
          first = false;
          string newstr = str.Substring(index, index2 - index);
          strings = strings ?? new List<string>();
          strings.Add(newstr);
          index = index2 + delimLength;
        }
      }
      return strings.ToArray();
    }

    public OptionsParser(string options) {
      if (options == null) {
        throw new ArgumentNullException(nameof(options));
      }
      if (options.Length > 0) {
        string[] optionsArray = SplitAt(options, ";");
        foreach (string opt in optionsArray) {
          int index = opt.IndexOf('=');
          if (index < 0) {
            throw new ArgumentException("Invalid options string: " +
              options);
          }
          string key = DataUtilities.ToLowerCaseAscii(opt.Substring(0,
                index));
          string value = opt.Substring(index + 1);
          this.dict[key] = value;
        }
      }
    }

    public string GetLCString(string key, string defaultValue) {
      string lckey = DataUtilities.ToLowerCaseAscii(key);
      if (this.dict.TryGetValue(lckey, out string val)) {
        string lcvalue = DataUtilities.ToLowerCaseAscii(val);
        return lcvalue;
      }
      return defaultValue;
    }

    public bool GetBoolean(string key, bool defaultValue) {
      string lckey = DataUtilities.ToLowerCaseAscii(key);
      if (this.dict.TryGetValue(lckey, out string val)) {
        string lcvalue = DataUtilities.ToLowerCaseAscii(val);
        return lcvalue.Equals("1", StringComparison.Ordinal) ||
          lcvalue.Equals("yes", StringComparison.Ordinal) ||
          lcvalue.Equals("on", StringComparison.Ordinal) ||
          lcvalue.Equals("true", StringComparison.Ordinal);
      }
      return defaultValue;
    }
  }
}
