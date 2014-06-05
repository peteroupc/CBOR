using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using ClariusLabs.NuDoc;

namespace PeterO.DocGen {
  public class DocGenerator {
    public static void Generate(Assembly assembly, string docdir) {
      var directory = Path.GetFullPath(docdir);
      Directory.CreateDirectory(directory);
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
