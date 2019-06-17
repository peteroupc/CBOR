using System;
using System.IO;
using System.Text.RegularExpressions;
namespace PeterO.DocGen {
  public static class DocGenUtil {

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
      string oldString = null;
      try {
        oldString = File.ReadAllText(filename);
      } catch (IOException) {
        oldString = null;
      }
      if (oldString == null || !oldString.Equals(newString)) {
        File.WriteAllText(filename, newString);
      }
    }
  }
}
