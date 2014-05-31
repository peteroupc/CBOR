/*
Written in 2014 by Peter O.

Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using ClariusLabs.NuDoc;
using PeterO.Cbor;

namespace CBORDocs {
  public class DocVisitor : Visitor {
    StringBuilder paramStr = new StringBuilder();
    StringBuilder returnStr = new StringBuilder();
    StringBuilder exceptionStr = new StringBuilder();
    StringBuilder currentBuffer = null;

    public static string FormatType(Type type) {
      string rawfmt = FormatTypeRaw(type);
      if (!type.IsArray && !type.IsGenericType) {
        return rawfmt;
      }
      StringBuilder sb = new StringBuilder();
      sb.Append(rawfmt);
      if (type.ContainsGenericParameters) {
        sb.Append('<');
        bool first = true;
        foreach (var arg in type.GetGenericArguments()) {
          if (!first) {
            sb.Append(", ");
          }
          sb.Append(FormatType(arg));
          first = false;
        }
        sb.Append('>');
      }
      if (type.IsArray) {
        for (int i = 0; i < type.GetArrayRank(); ++i) {
          sb.Append("[]");
        }
      }
      return sb.ToString();
    }

    public static string FormatTypeSig(Type typeInfo) {
      StringBuilder builder = new StringBuilder();
      builder.Append("    ");
      if (typeInfo.IsPublic) {
        builder.Append("public ");
      } else {
        builder.Append("internal ");
      }
      if (typeInfo.IsAbstract) {
        builder.Append("abstract ");
      }
      if (typeInfo.IsSealed) {
        builder.Append("sealed ");
      }
      builder.Append(typeInfo.Name);
      bool first;
      if (typeInfo.GetGenericArguments().Length > 0) {
        builder.Append('<');
        first = true;
        foreach (var arg in typeInfo.GetGenericArguments()) {
          if (!first) {
            builder.Append(", ");
          }
          builder.Append(FormatType(arg));
          first = false;
        }
        builder.Append('>');
      }
      first = true;
      var ifaces = typeInfo.GetInterfaces();
      var derived = typeInfo.BaseType;
      if (derived != null || ifaces.Length > 0) {
        builder.Append(" :\r\n");
        if (derived != null) {
          builder.Append("    " + FormatType(derived));
          first = false;
        }
        if (ifaces.Length > 0) {
          foreach (var iface in ifaces) {
            if (!first) {
              builder.Append(";\r\n");
            }
            builder.Append("    " + FormatType(iface));
            first = false;
          }
        }
      }
      // TODO: Get type constraints
      return builder.ToString();
    }

    public static string FormatMethod(MethodInfo method) {
      StringBuilder builder = new StringBuilder();
      builder.Append("    ");
      if (method.IsPublic) {
        builder.Append("public ");
      }
      if (method.IsAssembly) {
        builder.Append("internal ");
      }
      if (method.IsFamily) {
        builder.Append("protected ");
      }
      if (method.IsStatic) {
        builder.Append("static ");
      }
      if (method.IsAbstract) {
        builder.Append("abstract ");
      }
      if (method.IsFinal) {
        builder.Append("sealed ");
      }
      if (IsMethodOverride(method)) {
        builder.Append("override ");
      } else if (method.IsVirtual) {
        builder.Append("virtual ");
      }
      if (!method.IsConstructor) {
        builder.Append(FormatType(method.ReturnType));
        builder.Append(" ");
      }
      builder.Append(method.Name);
      bool first;
      if (method.GetGenericArguments().Length > 0) {
        builder.Append('<');
        first = true;
        foreach (var arg in method.GetGenericArguments()) {
          if (!first) {
            builder.Append(", ");
          }
          builder.Append(FormatType(arg));
          first = false;
        }
        builder.Append('>');
      }
      builder.Append("(");
      first = true;
      foreach (var param in method.GetParameters()) {
        if (!first) {
          builder.Append(",\r\n        ");
        } else {
          builder.Append("\r\n        ");
        }
        Attribute attr = param.GetCustomAttribute(typeof(ParamArrayAttribute));
        if (attr != null) {
          builder.Append("params ");
        }
        builder.Append(FormatType(param.ParameterType));
        builder.Append(" ");
        builder.Append(param.Name);
        first = false;
      }
      // TODO: Get type constraints
      builder.Append(");");
      return builder.ToString();
    }

    public static string FormatField(FieldInfo field) {
      StringBuilder builder = new StringBuilder();
      builder.Append("    ");
      if (field.IsPublic) {
        builder.Append("public ");
      }
      if (field.IsAssembly) {
        builder.Append("internal ");
      }
      if (field.IsFamily) {
        builder.Append("protected ");
      }
      if (field.IsStatic) {
        builder.Append("static ");
      }
      if (field.IsInitOnly) {
        builder.Append("readonly ");
      }
      builder.Append(FormatType(field.FieldType));
      builder.Append(" ");
      builder.Append(field.Name);
      if (field.IsLiteral) {
        try {
          object obj = field.GetRawConstantValue();
          if (obj is int) {
            builder.Append(" = " + (int)obj + ";");
          } else if (obj is long) {
            builder.Append(" = " + (long)obj + "L;");
          } else {
            builder.Append(";");
          }
        } catch (InvalidOperationException) {
          builder.Append(";");
        }
      } else {
        builder.Append(";");
      }
      return builder.ToString();
    }

    public static string FormatTypeRaw(Type type) {
      string name = type.Name;
      int idx = name.IndexOf('`');
      if (idx >= 0) {
        name = name.Substring(0, idx);
      }
      idx = name.IndexOf('[');
      if (idx >= 0) {
        name = name.Substring(0, idx);
      }
      if (type.IsGenericParameter) {
        return name;
      }
      name = type.Namespace + "." + name;
      if (name.Equals("System.Int32")) {
        return "int";
      }
      if (name.Equals("System.Int64")) {
        return "long";
      }
      if (name.Equals("System.Int16")) {
        return "short";
      }
      if (name.Equals("System.UInt32")) {
        return "uint";
      }
      if (name.Equals("System.UInt64")) {
        return "ulong";
      }
      if (name.Equals("System.UInt16")) {
        return "ushort";
      }
      if (name.Equals("System.Char")) {
        return "char";
      }
      if (name.Equals("System.Void")) {
        return "void";
      }
      if (name.Equals("System.Byte")) {
        return "byte";
      }
      if (name.Equals("System.SByte")) {
        return "sbyte";
      }
      if (name.Equals("System.Boolean")) {
        return "bool";
      }
      if (name.Equals("System.Float")) {
        return "float";
      }
      if (name.Equals("System.Double")) {
        return "double";
      }
      return name;
    }

    private static bool IsMethodOverride(MethodInfo method) {
      Type type = method.DeclaringType;
      MethodInfo baseMethod = method.GetBaseDefinition();
      if (baseMethod == null) {
        return false;
      }
      // TODO: Doesn't work yet
      if (method.DeclaringType.Equals(baseMethod.DeclaringType)) {
        return false;
      }
      return true;
    }
    public override void VisitReturns(Returns param) {
      currentBuffer = returnStr;
      WriteLine("<b>Returns:</b>\r\n");
      base.VisitReturns(param);
      WriteLine("\r\n\r\n");
      currentBuffer = null;
    }

    public override void VisitException(ClariusLabs.NuDoc.Exception exception) {
      currentBuffer = exceptionStr;
      string cref = exception.Cref;
      if (cref.StartsWith("T:")) {
        cref = cref.Substring(2);
      }
      WriteLine(" * " + cref + ": ");
      base.VisitException(exception);
      WriteLine("\r\n\r\n");
      currentBuffer = null;
    }

    public override void VisitParam(Param param) {
      currentBuffer = paramStr;
      Write(" * <i>" + param.Name + "</i>: ");
      base.VisitParam(param);
      WriteLine("\r\n\r\n");
      currentBuffer = null;
    }

    public override void VisitList(List list) {
      WriteLine("\r\n\r\n");
      base.VisitList(list);
    }

    public override void VisitItem(Item item) {
      Write(" * ");
      base.VisitItem(item);
      WriteLine("\r\n\r\n");
    }

    public override void VisitTypeParam(TypeParam param) {
      currentBuffer = paramStr;
      Write(" * &lt;" + param.Name + "&gt;: ");
      base.VisitTypeParam(param);
      WriteLine("\r\n\r\n");
      currentBuffer = null;
    }

    public override void VisitParamRef(ParamRef param) {
      WriteLine("<i>" + param.Name + "</i>");
      base.VisitParamRef(param);
    }
    public override void VisitMember(Member member) {
      MemberInfo info = member.Info;
      string signature = String.Empty;
      if (info is MethodInfo) {
        MethodInfo method = (MethodInfo)info;
        if (!method.IsPublic && !method.IsFamily) {
          // Ignore methods other than public and protected
          // methods
          return;
        }
        signature = FormatMethod(method);
        WriteLine("### " + method.Name + "\r\n\r\n" + signature + "\r\n\r\n");
        paramStr.Clear();
        returnStr.Clear();
        exceptionStr.Clear();
        base.VisitMember(member);
        if (paramStr.Length > 0) {
          Write("<b>Parameters:</b>\r\n\r\n");
          Write(paramStr.ToString());
        }
        Write(returnStr.ToString());
        if (exceptionStr.Length > 0) {
          Write("<b>Exceptions:</b>\r\n\r\n");
          Write(exceptionStr.ToString());
        }
      } else if (info is FieldInfo) {
        FieldInfo field = (FieldInfo)info;
        if (!field.IsPublic && !field.IsFamily) {
          // Ignore nonpublic, nonprotected fields
          return;
        }
        signature = FormatField(field);
        WriteLine("## " + field.Name + "\r\n\r\n" + signature + "\r\n\r\n");
        base.VisitMember(member);
      }
      // TODO: Properties
    }

    public override void VisitSummary(Summary summary) {
      base.VisitSummary(summary);
      WriteLine("\r\n\r\n");
    }

    public override void VisitType(TypeDeclaration type) {
      Type typeinfo = (Type)type.Info;
      if (typeinfo.IsPublic) {
        // TODO: Write more detailed type info
        WriteLine("## " + FormatType(typeinfo) + "\r\n\r\n");
        base.VisitType(type);
      }
    }

    public override void VisitText(Text text) {
      string t = text.Content;
      // Collapse multiple spaces into a single space
      t = Regex.Replace(t, @"\s+", " ");
      Write(t);
      base.VisitText(text);
    }

    public override void VisitC(C code) {
      Write(" `" + code.Content + "` ");
      base.VisitC(code);
    }

    public override void VisitCode(Code code) {
      foreach (var line in code.Content.Split('\n')) {
        WriteLine("    " + line.TrimEnd());
      }
      WriteLine("\r\n\r\n");
      base.VisitCode(code);
    }

    private void Write(string ln) {
      if (currentBuffer != null) {
        currentBuffer.Append(ln);
      } else {
        Console.Write(ln);
      }
    }
    private void WriteLine(string ln) {
      if (currentBuffer != null) {
        currentBuffer.Append(ln);
        currentBuffer.Append("\r\n");
      } else {
        Console.WriteLine(ln);
      }
    }

    public override void VisitPara(Para para) {
      base.VisitPara(para);
      WriteLine("\r\n\r\n");
    }
  }

  class Program {
    // TODO: Currently a work in progress
    static void Main(string[] args) {
      var members = DocReader.Read(typeof(CBORObject).Assembly);
      var oldWriter = Console.Out;
      using (var writer = new StreamWriter("doc.md")) {
        Console.SetOut(writer);
        members.Accept(new DocVisitor());
        Console.SetOut(oldWriter);
      }
    }
  }
}
