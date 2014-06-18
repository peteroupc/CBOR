using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ClariusLabs.NuDoc;

namespace PeterO.DocGen {
/// <summary>A documentation generator.</summary>
public static class DocGenerator {
    public static void Generate(string assemblyFile, string docdir) {
      var directory = Path.GetFullPath(docdir);
      Directory.CreateDirectory(directory);
      assemblyFile = Path.GetFullPath(assemblyFile);
      var appdomain = AppDomain.CreateDomain("docgen");
      Directory.SetCurrentDirectory(Path.GetDirectoryName(assemblyFile));
      var assembly = Assembly.LoadFrom(assemblyFile);
      var members = DocReader.Read(assembly);
      var oldWriter = Console.Out;
      var visitor = new TypeVisitor(directory);
      members.Accept(visitor);
      visitor.Finish();
      using (var writer = new StreamWriter(Path.Combine(directory, "APIDocs.md"), false, Encoding.UTF8)) {
        var visitor2 = new SummaryVisitor(writer);
        members.Accept(visitor2);
        visitor2.Finish();
      }
    }
  }
}
