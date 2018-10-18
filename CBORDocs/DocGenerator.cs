using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NuDoq;

namespace PeterO.DocGen {
/// <summary>A documentation generator.</summary>
public static class DocGenerator {
// TODO: Make indexers ("this[]") show in documentation
    public static void Generate(string assemblyFile, string docdir) {
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
      var directory = Path.GetFullPath(docdir);
      assemblyFile = Path.GetFullPath(assemblyFile);
      Directory.SetCurrentDirectory(Path.GetDirectoryName(assemblyFile));
      if (!File.Exists(assemblyFile)) {
        // Exit early, not found
        throw new ArgumentException("Assembly file not found: " + assemblyFile);
      }
      var asm = Assembly.LoadFrom(assemblyFile);
      Directory.CreateDirectory(directory);
      try {
        var members = DocReader.Read(asm);
        var oldWriter = Console.Out;
      var visitor = new TypeVisitor(directory);
      members.Accept(visitor);
      visitor.Finish();
      var visitor2 = new SummaryVisitor(Path.Combine(directory, "APIDocs.md"
));
        members.Accept(visitor2);
        visitor2.Finish();
    } catch (IOException ex) {
      Console.WriteLine(ex.Message);
      return;
    }
    }
  }
}
