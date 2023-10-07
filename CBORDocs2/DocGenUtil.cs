using System;
using System.IO;
using System.Text.RegularExpressions;

namespace PeterO.DocGen {
  public static class DocGenUtil {
    public static string HtmlEscape(string str) {
      if (str == null) {
        return str;
      }
      str = str.Replace("&", "&amp;");
      str = str.Replace("<", "&lt;");
      str = str.Replace(">", "&gt;");
      str = str.Replace("*", "&#x2a;");
      str = str.Replace("\"", "&#x22;");
      return str;
    }

    public static string NormalizeLines(string x) {
      if (String.IsNullOrEmpty(x)) {
        return x;
      }
      x = Regex.Replace(x, @"[ \t]+(?=[\r\n]|$)", String.Empty);
      x = Regex.Replace(x, @"\r?\n(\r?\n)+", "\n\n");
      x = Regex.Replace(x, @"\r?\n", "\n");
      x = Regex.Replace(x, @"^\s*", String.Empty);
      x = Regex.Replace(x, @"\s+$", String.Empty);
      return x + "\n";
    }

    public static void FileEdit(string filename, string newString) {
      string oldString;
      try {
        oldString = File.ReadAllText(filename);
      } catch (IOException) {
        oldString = null;
      }
      if (oldString == null || !oldString.Equals(newString,
  StringComparison.Ordinal)) {
        File.WriteAllText(filename, newString);
      }
    }
  }
}
