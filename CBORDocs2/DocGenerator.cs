using System;
using System.IO;
using System.Reflection;

namespace PeterO.DocGen {
  /// <summary>A documentation generator.</summary>
  public static class DocGenerator {
    internal static void Generate(string assemblyFile, string docdir) {
      if (assemblyFile == null) {
        throw new ArgumentNullException(nameof(assemblyFile));
      }
      if (assemblyFile.Length == 0) {
        throw new ArgumentException("assemblyFile is empty.");
      }
      if (docdir == null) {
        throw new ArgumentNullException(nameof(docdir));
      }
      if (docdir.Length == 0) {
        throw new ArgumentException("docdir is empty.");
      }
      string directory = Path.GetFullPath(docdir);
      assemblyFile = Path.GetFullPath(assemblyFile);
      string assemblyXml = Path.ChangeExtension(assemblyFile, ".xml");
      Directory.SetCurrentDirectory(Path.GetDirectoryName(assemblyFile));
      if (!File.Exists(assemblyFile)) {
        // Exit early, not found
        throw new ArgumentException("Assembly file not found: " + assemblyFile);
      }
      if (!File.Exists(assemblyXml)) {
        // Exit early, not found
        throw new ArgumentException("XML documentation not found: " +
              assemblyXml);
      }
      var asm = Assembly.LoadFrom(assemblyFile);
      _ = Directory.CreateDirectory(directory);
      try {
        var xmldoc = new XmlDoc(assemblyXml);
        TextWriter oldWriter = Console.Out;
        var visitor = new TypeVisitor(directory);
        foreach (Type t in asm.GetTypes()) {
          visitor.HandleTypeAndMembers(t, xmldoc);
        }
        visitor.Finish();
        var visitor2 = new SummaryVisitor(
            Path.Combine(directory, "APIDocs.md"));
        foreach (Type t in asm.GetTypes()) {
          visitor2.HandleType(t, xmldoc);
        }
        visitor2.Finish();
      } catch (IOException ex) {
        Console.WriteLine(ex.Message);
        return;
      }
    }
  }
}
