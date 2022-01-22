using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace PeterO.DocGen {
  public static class TestGenerator {
    public static void GenerateTests(Type type, string directory) {
      if (type == null) {
        throw new ArgumentNullException(nameof(type));
      }
      var name = TypeNameUtil.UndecorateTypeName(type.Name);
      var builder = new StringBuilder();
      Directory.CreateDirectory(directory);
      builder.Append("using System;\n");
      builder.Append("using System.Collections.Generic;\n");
      builder.Append("using System.Text;\n");
      builder.Append("using " + type.Namespace + ";\n");
      builder.Append("using Microsoft.VisualStudio.TestTools.UnitTesting;\n");
      builder.Append("namespace Test {\n");
      builder.Append(" [TestClass]\n");
      builder.Append(" public partial class " + name + "Test {\n");
      var methods = new SortedSet<string>();
      var hasPublicConstructor = false;
      foreach (var method in type.GetConstructors()) {
        if (!method.IsPublic) {
          continue;
        }
        hasPublicConstructor = true;
        break;
      }
      if (hasPublicConstructor) {
        methods.Add("Constructor");
      }
      foreach (var method in type.GetMethods()) {
        if (!method.IsPublic) {
          continue;
        }
        if (!method.DeclaringType.Equals(method.ReflectedType)) {
          continue;
        }
        var methodName = method.Name;
        if (methodName.StartsWith("get_", StringComparison.Ordinal)) {
          methodName = methodName.Substring(4);
        } else if (methodName.StartsWith("set_", StringComparison.Ordinal)) {
          methodName = methodName.Substring(4);
        } else if (methodName.StartsWith("op_", StringComparison.Ordinal)) {
          methodName = "Operator" + methodName.Substring(3);
        }
        if (methodName.StartsWith(".ctor", StringComparison.Ordinal)) {
          methodName = "Constructor";
        }
        if (methodName.StartsWith(".cctor", StringComparison.Ordinal)) {
          methodName = "StaticConstructor";
        }
        if (methodName.Length == 0) {
          continue;
        }
        methodName = PeterO.DataUtilities.ToUpperCaseAscii(
          methodName.Substring(0, 1)) + methodName.Substring(1);
        methods.Add(methodName);
      }
      if (methods.Count == 0) {
       // no tests to write
        return;
      }
      if (methods.Contains("Constructor")) {
        builder.Append(" [TestMethod]\n");
        builder.Append(" public void TestConstructor() {\n");
        builder.Append(" // not implemented yet\n");
        builder.Append(" }\n");
      }
      foreach (var methodName in methods) {
        if (methodName.Equals("Constructor", StringComparison.Ordinal)) {
          continue;
        }
        builder.Append(" [TestMethod]\n");
        builder.Append(" public void Test" + methodName + "() {\n");
        builder.Append(" // not implemented yet\n");
        builder.Append(" }\n");
      }
      builder.Append(" }\n");
      builder.Append('}');
      var filename = Path.Combine(directory, name + "Test.cs");
      if (!File.Exists(filename)) {
        File.WriteAllText(filename, builder.ToString());
      }
    }

    public static void GenerateTests(Assembly assembly, string directory) {
      if (assembly == null) {
        throw new ArgumentNullException(nameof(assembly));
      }
      foreach (var type in assembly.GetTypes()) {
        if (!type.IsPublic || type.IsInterface) {
          continue;
        }
        GenerateTests(type, directory);
      }
    }
  }
}
