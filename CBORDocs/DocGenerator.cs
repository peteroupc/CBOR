using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using ClariusLabs.NuDoc;

namespace PeterO.DocGen {
/// <summary>A documentation generator.</summary>
public class DocGenerator {
    public static void Generate(string assemblyFile, string docdir) {
      var directory = Path.GetFullPath(docdir);
      Directory.CreateDirectory(directory);
      assemblyFile = Path.GetFullPath(assemblyFile);
      var appdomain = AppDomain.CreateDomain("docgen");
      Directory.SetCurrentDirectory(Path.GetDirectoryName(assemblyFile));
      var assembly = Assembly.LoadFrom(assemblyFile);
      var members = DocReader.Read(assembly);
      var oldWriter = Console.Out;
      TypeVisitor visitor = new TypeVisitor(directory);
      members.Accept(visitor);
      visitor.Finish();
      using (var writer = new StreamWriter(Path.Combine(directory, "APIDocs.md"), false, Encoding.UTF8)) {
        SummaryVisitor visitor2 = new SummaryVisitor(writer);
        members.Accept(visitor2);
        visitor2.Finish();
      }
    }
  }
}
