/*
Written by Peter O.

Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
https://creativecommons.org/publicdomain/zero/1.0/

 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace PeterO.DocGen {
    /// <summary>A documentation visitor.</summary>
  internal class DocVisitor : IVisitor {
    private const string FourSpaces = " " + " " + " " + " ";

    private static readonly IDictionary<string, string> ValueOperators =
          OperatorList();

    private readonly StringBuilder buffer = new StringBuilder();
    private readonly StringBuilder exceptionStr = new StringBuilder();

    private readonly IDictionary<string, StringBuilder> members = new
      SortedDictionary<string, StringBuilder>();

    private readonly StringBuilder paramStr = new StringBuilder();
    private readonly StringBuilder returnStr = new StringBuilder();
    private StringBuilder currentBuffer;

    public static void AppendConstraints(
      Type[] genericArguments,
      StringBuilder builder) {
      foreach (var arg in genericArguments) {
        if (arg.IsGenericParameter) {
          var constraints = arg.GetGenericParameterConstraints();
          if (constraints.Length == 0 && (arg.GenericParameterAttributes &
  (GenericParameterAttributes.ReferenceTypeConstraint |
  GenericParameterAttributes.NotNullableValueTypeConstraint |
                   GenericParameterAttributes.DefaultConstructorConstraint))
                == GenericParameterAttributes.None) {
            continue;
          }
          builder.Append("\r\n" + FourSpaces + FourSpaces + "where ");
          builder.Append(TypeNameUtil.UndecorateTypeName(arg.Name));
          builder.Append(" : ");
          var first = true;
          if ((arg.GenericParameterAttributes &
               GenericParameterAttributes.ReferenceTypeConstraint) !=
              GenericParameterAttributes.None) {
            if (!first) {
              builder.Append(", ");
            }
            builder.Append("class");
            first = false;
          }
          if ((arg.GenericParameterAttributes &
               GenericParameterAttributes.NotNullableValueTypeConstraint) !=
              GenericParameterAttributes.None) {
            if (!first) {
              builder.Append(", ");
            }
            builder.Append("struct");
            first = false;
          }
          if ((arg.GenericParameterAttributes &
               GenericParameterAttributes.DefaultConstructorConstraint) !=
              GenericParameterAttributes.None) {
            if (!first) {
              builder.Append(", ");
            }
            builder.Append("new()");
            first = false;
          }
          foreach (var constr in constraints) {
            if (!first) {
              builder.Append(", ");
            }
            builder.Append(FormatType(constr));
            first = false;
          }
        }
        builder.Append(FormatType(arg));
      }
    }

    public static string FormatField(FieldInfo field) {
      var builder = new StringBuilder();
      builder.Append(FourSpaces);
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
      builder.Append((char)0x20); // space
      builder.Append(field.Name);
      if (field.IsLiteral) {
        try {
          var obj = field.GetRawConstantValue();
          if (obj is int) {
            builder.Append(" = " + (int)obj + ";");
          } else if (obj is long) {
            builder.Append(" = " + (long)obj + "L;");
          } else {
            builder.Append(';');
          }
        } catch (InvalidOperationException) {
          builder.Append(';');
        }
      } else {
        builder.Append(';');
      }
      return builder.ToString();
    }

    public static string FormatMethod(
      MethodBase method,
      bool shortform) {
      var builder = new StringBuilder();
      if (!shortform) {
        builder.Append(FourSpaces);
        if (!method.ReflectedType.IsInterface) {
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
          } else if (method is MethodInfo &&
IsMethodOverride((MethodInfo)method)) {
            builder.Append("override ");
          } else if (method.IsVirtual) {
            builder.Append("virtual ");
          }
        }
      }
      var methodInfo = method as MethodInfo;
      var isExtension = false;
      Attribute attr;
      if (methodInfo != null) {
        attr = methodInfo.GetCustomAttribute(
          typeof(System.Runtime.CompilerServices.ExtensionAttribute));
        isExtension = attr != null;
        if (method.Name.Equals("op_Explicit", StringComparison.Ordinal)) {
          builder.Append("explicit operator ");
          builder.Append(FormatType(methodInfo.ReturnType));
        } else if (method.Name.Equals("op_Implicit",
  StringComparison.Ordinal)) {
          builder.Append("implicit operator ");
          builder.Append(FormatType(methodInfo.ReturnType));
        } else if (ValueOperators.TryGetValue(method.Name, out string op)) {
          builder.Append(FormatType(methodInfo.ReturnType));
          builder.Append(" operator ");
          builder.Append(op);
        } else {
          if (!shortform) {
            builder.Append(FormatType(methodInfo.ReturnType));
          }
          builder.Append((char)0x20);
          builder.Append(method.Name);
        }
      } else {
  builder.Append(TypeNameUtil.UndecorateTypeName(method.ReflectedType.Name));
      }
      bool first;
      if (method is MethodInfo && method.GetGenericArguments().Length > 0) {
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
      builder.Append('(');
      first = true;
      foreach (var param in method.GetParameters()) {
        if (!first) {
          builder.Append(',');
        }
        if (!shortform) {
          builder.Append("\r\n" + FourSpaces + FourSpaces);
        } else if (!first) {
          builder.Append((char)0x20);
        }
        if (first && isExtension) {
          builder.Append("this ");
        }
        attr = param.GetCustomAttribute(typeof(ParamArrayAttribute));
        if (attr != null) {
          builder.Append("params ");
        }
        builder.Append(FormatType(param.ParameterType));
        if (!shortform) {
          builder.Append((char)0x20);
          builder.Append(param.Name);
        }
        first = false;
      }
      builder.Append(')');
      if (method is MethodInfo && method.GetGenericArguments().Length > 0) {
        AppendConstraints(method.GetGenericArguments(), builder);
      }
      if (!shortform) {
        builder.Append(';');
      }
      return builder.ToString();
    }

    public static string FormatProperty(PropertyInfo property) {
      return FormatProperty(property, false);
    }

    public static string FormatProperty(PropertyInfo property, bool shortform) {
      var builder = new StringBuilder();
      var getter = property.GetGetMethod();
      var setter = property.GetSetMethod();
      if (!shortform) {
        builder.Append(FourSpaces);
        if (!property.ReflectedType.IsInterface) {
          if ((getter != null && getter.IsPublic) ||
              (setter != null && setter.IsPublic)) {
            builder.Append("public ");
          } else if ((getter != null && getter.IsAssembly) ||
                    (setter != null && setter.IsAssembly)) {
            builder.Append("internal ");
          } else if ((getter != null && getter.IsFamily) ||
                    (setter != null && setter.IsFamily)) {
            builder.Append("protected ");
          }
          if ((getter != null && getter.IsStatic) ||
              (setter != null && setter.IsStatic)) {
            builder.Append("static ");
          }
          if ((getter != null && getter.IsAbstract) ||
              (setter != null && setter.IsAbstract)) {
            builder.Append("abstract ");
          }
          if ((getter != null && getter.IsFinal) ||
              (setter != null && setter.IsFinal)) {
            builder.Append("sealed ");
          } else if (IsMethodOverride(getter)) {
            builder.Append("override ");
          } else if ((getter != null && getter.IsVirtual) ||
                    (setter != null && setter.IsVirtual)) {
            builder.Append("virtual ");
          }
        }
        builder.Append(FormatType(property.PropertyType));
        builder.Append((char)0x20);
      }
      bool first;
      var indexParams = property.GetIndexParameters();
      if (indexParams.Length > 0) {
        builder.Append("this[");
      } else {
        builder.Append(property.Name);
      }
      first = true;
      foreach (var param in indexParams) {
        if (!first) {
          builder.Append(",\r\n" + FourSpaces + FourSpaces);
        } else {
          builder.Append(indexParams.Length == 1 ?
                    String.Empty : "\r\n" + FourSpaces + FourSpaces);
        }
        var attr = param.GetCustomAttribute(typeof(ParamArrayAttribute));
        if (attr != null) {
          builder.Append("params ");
        }
        builder.Append(FormatType(param.ParameterType));
        if (!shortform) {
          builder.Append((char)0x20);
          builder.Append(param.Name);
        }
        first = false;
      }
      if (indexParams.Length > 0) {
        builder.Append(']');
      }
      if (!shortform) {
        builder.Append(" { ");
        if (getter != null && !getter.IsPrivate) {
          builder.Append("get; ");
        }
        if (setter != null && !setter.IsPrivate) {
          builder.Append("set; ");
        }
        builder.Append('}');
      }
      return builder.ToString();
    }

    public static string FormatType(Type type) {
      var rawfmt = FormatTypeRaw(type);
      if (!type.IsArray && !type.IsGenericType) {
        return rawfmt;
      }
      var sb = new StringBuilder();
      sb.Append(rawfmt);
      if (type.ContainsGenericParameters) {
        sb.Append('<');
        var first = true;
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
        for (var i = 0; i < type.GetArrayRank(); ++i) {
          sb.Append("[]");
        }
      }
      return sb.ToString();
    }

    public static string FormatTypeRaw(Type type) {
      var name = TypeNameUtil.UndecorateTypeName(type.Name);
      if (type.IsGenericParameter) {
        return name;
      }
      name = TypeNameUtil.SimpleTypeName(type);
      if (name.Equals("System.Decimal", StringComparison.Ordinal)) {
        return "decimal";
      }
      return name.Equals("System.Int32", StringComparison.Ordinal) ? "int" :
        (name.Equals("System.Int64", StringComparison.Ordinal) ? "long" :
         (name.Equals("System.Int16", StringComparison.Ordinal) ? "short" :
          (name.Equals("System.UInt32", StringComparison.Ordinal) ? "uint" :
           (name.Equals("System.UInt64", StringComparison.Ordinal) ? "ulong" :
            (name.Equals("System.UInt16", StringComparison.Ordinal) ? "ushort" :
             (name.Equals("System.Char", StringComparison.Ordinal) ? "char" :
              (name.Equals("System.Object", StringComparison.Ordinal) ?
"object" :
           (name.Equals("System.Void", StringComparison.Ordinal) ? "void" :
(name.Equals("System.Byte", StringComparison.Ordinal) ?
                    "byte" :
  (name.Equals("System.SByte", StringComparison.Ordinal) ? "sbyte" :
(name.Equals("System.String", StringComparison.Ordinal) ?
                "string" : (name.Equals("System.Boolean",
  StringComparison.Ordinal) ?
  "bool" : (name.Equals("System.Single", StringComparison.Ordinal) ?
  "float" : (name.Equals("System.Double", StringComparison.Ordinal) ? "double" :
  name))))))))))))));
    }

    public static string FormatTypeSig(Type typeInfo) {
      var builder = new StringBuilder();
      builder.Append(FourSpaces);
      if (typeInfo.IsNested ? typeInfo.IsNestedPublic : typeInfo.IsPublic) {
        builder.Append("public ");
      } else {
        builder.Append("internal ");
      }
      if (typeInfo.IsAbstract && typeInfo.IsSealed) {
        builder.Append("static ");
      } else if (typeInfo.IsAbstract && !typeInfo.IsInterface) {
        builder.Append("abstract ");
      } else if (typeInfo.IsSealed) {
        builder.Append("sealed ");
      }
      if (typeInfo.IsValueType) {
        builder.Append("struct ");
      } else if (typeInfo.IsClass) {
        builder.Append("class ");
      } else {
        builder.Append("interface ");
      }
      builder.Append(TypeNameUtil.UndecorateTypeName(typeInfo.Name));
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
      if (typeInfo.BaseType != null &&
          typeInfo.BaseType.Equals(typeof(object))) {
        derived = null;
      }
      if (derived != null || ifaces.Length > 0) {
        builder.Append(" :\r\n" + FourSpaces);
        if (derived != null) {
          builder.Append(FourSpaces + FormatType(derived));
          first = false;
        }
        if (ifaces.Length > 0) {
          // Sort interface names to ensure they are
          // displayed in a consistent order. Apparently, GetInterfaces
          // can return such interfaces in an unspecified order.
          var ifacenames = new List<string>();
          foreach (var iface in ifaces) {
            ifacenames.Add(FormatType(iface));
          }
          ifacenames.Sort();
          foreach (var ifacename in ifacenames) {
            if (!first) {
              builder.Append(",\r\n" + FourSpaces);
            }
            builder.Append(FourSpaces + ifacename);
            first = false;
          }
        }
      }
      AppendConstraints(typeInfo.GetGenericArguments(), builder);
      return builder.ToString();
    }

    public static string GetTypeID(Type type) {
      var name = FormatType(type);
      name = name.Replace(", ", ",");
      var builder = new StringBuilder();
      for (var i = 0; i < name.Length; ++i) {
        var cat = CharUnicodeInfo.GetUnicodeCategory(name, i);
        var cp = DataUtilities.CodePointAt(name, i);
        if (cp >= 0x10000) {
          ++i;
        }
        if (cat == UnicodeCategory.UppercaseLetter ||
            cat == UnicodeCategory.LowercaseLetter ||
            cat == UnicodeCategory.TitlecaseLetter ||
            cat == UnicodeCategory.OtherLetter ||
            cat == UnicodeCategory.DecimalDigitNumber ||
            cp == '_' || cp == '.') {
          if (cp >= 0x10000) {
            builder.Append(name, i, 2);
          } else {
            builder.Append(name[i]);
          }
        } else {
          builder.Append(' ');
        }
      }
      name = builder.ToString();
      name = name.Trim();
      name = name.Replace(' ', '-');
      return name;
    }

    public static bool IsMethodOverride(MethodInfo method) {
      var type = method.DeclaringType;
      var baseMethod = method.GetBaseDefinition();
      return (baseMethod != null) && (!method.Equals(baseMethod));
    }

    public void Debug(string ln) {
      this.WriteLine(ln);
      this.WriteLine(String.Empty);
    }

    public override string ToString() {
      var b = new StringBuilder();
      b.Append(this.buffer.ToString());
      foreach (var b2 in this.members.Keys) {
        b.Append(this.members[b2].ToString());
      }
      return b.ToString();
    }

    public void VisitNode(INode node) {
      if (String.IsNullOrEmpty(node.LocalName)) {
        var t = node.GetContent();
       // Collapse multiple spaces into a single space
        t = Regex.Replace(t, @"\s+", " ");
        if (t.Length != 1 || t[0] != ' ') {
          // Don't write if result is a single space
          this.Write(t);
        }
        XmlDoc.VisitInnerNode(node, this);
      } else {
        var xmlName = PeterO.DataUtilities.ToLowerCaseAscii(node.LocalName);
        if (xmlName.Equals("c", StringComparison.Ordinal)) {
          this.VisitC(node);
        } else if (xmlName.Equals("code", StringComparison.Ordinal)) {
          this.VisitCode(node);
        } else if (xmlName.Equals("example", StringComparison.Ordinal)) {
          this.VisitExample(node);
        } else if (xmlName.Equals("exception", StringComparison.Ordinal)) {
          this.VisitException(node);
        } else if (xmlName.Equals("see", StringComparison.Ordinal)) {
          this.VisitSee(node);
        } else if (xmlName.Equals("item", StringComparison.Ordinal)) {
          this.VisitItem(node);
        } else if (xmlName.Equals("list", StringComparison.Ordinal)) {
          this.VisitList(node);
        } else if (xmlName.Equals("para", StringComparison.Ordinal)) {
          this.VisitPara(node);
        } else if (xmlName.Equals("param", StringComparison.Ordinal)) {
          this.VisitParam(node);
        } else if (xmlName.Equals("paramref", StringComparison.Ordinal)) {
          this.VisitParamRef(node);
        } else if (xmlName.Equals("remarks", StringComparison.Ordinal) ||
                  xmlName.Equals("summary", StringComparison.Ordinal)) {
          XmlDoc.VisitInnerNode(node, this);
          this.Write("\r\n\r\n");
        } else if (xmlName.Equals("returns", StringComparison.Ordinal)) {
          this.VisitReturns(node);
        } else if (xmlName.Equals("typeparam", StringComparison.Ordinal)) {
          this.VisitTypeParam(node);
        } else if (xmlName.Equals("value", StringComparison.Ordinal)) {
          this.VisitValue(node);
        } else if (xmlName.Equals("b", StringComparison.Ordinal) ||
xmlName.Equals("strong", StringComparison.Ordinal) ||
xmlName.Equals("i", StringComparison.Ordinal) ||
xmlName.Equals("a", StringComparison.Ordinal) ||
xmlName.Equals("sup", StringComparison.Ordinal) ||
xmlName.Equals("em", StringComparison.Ordinal)) {
          var sb = new StringBuilder();
          sb.Append("<" + xmlName);
          foreach (var attr in node.GetAttributes()) {
            sb.Append(" " + attr + "=");
            sb.Append("\"" + DocGenUtil.HtmlEscape(
              node.GetAttribute(attr)) + "\"");
          }
          sb.Append('>');
          this.Write(sb.ToString());
          XmlDoc.VisitInnerNode(node, this);
          this.Write("</" + xmlName + ">");
        } else {
          XmlDoc.VisitInnerNode(node, this);
        }
      }
    }

    public void VisitC(INode node) {
      this.Write(" `" + node.GetContent() + "` ");
    }

    public void VisitCode(INode node) {
      this.WriteLine("\r\n\r\n");
      foreach (var line in node.GetContent().Split('\n')) {
        this.WriteLine(FourSpaces + line.TrimEnd());
      }
      this.WriteLine("\r\n\r\n");
    }

    public void VisitExample(INode node) {
      XmlDoc.VisitInnerNode(node, this);
      this.WriteLine("\r\n\r\n");
    }

    public void VisitException(INode node) {
      using (var ch = this.Change(this.exceptionStr)) {
        var cref = node.GetAttribute("cref");
        if (cref == null) {
          cref = String.Empty;
          Console.WriteLine("Warning: cref attribute absent in <exception>");
        }
        if (cref.StartsWith("T:", StringComparison.Ordinal)) {
          cref = cref.Substring(2);
        }
        this.WriteLine(" * " + cref + ": ");
        XmlDoc.VisitInnerNode(node, this);
        this.WriteLine("\r\n\r\n");
      }
    }

    public void VisitSee(INode see) {
      string cref = see.GetAttribute("cref");
      if (cref == null) {
        cref = String.Empty;
        Console.WriteLine("Warning: cref attribute absent in <see>");
      }
      if (cref.Substring(0, 2).Equals("T:", StringComparison.Ordinal)) {
        string typeName = TypeNameUtil.UndecorateTypeName(cref.Substring(2));
        string content = DocGenUtil.HtmlEscape(see.GetContent());
        if (String.IsNullOrEmpty(content)) {
          content = typeName;
        }
        this.Write("[" + content + "]");
        this.Write("(" + typeName + ".md)");
        XmlDoc.VisitInnerNode(see, this);
      } else if (cref.Substring(0, 2).Equals("M:", StringComparison.Ordinal)) {
        string content = DocGenUtil.HtmlEscape(see.GetContent());
        if (String.IsNullOrEmpty(content)) {
          content = cref;
        }
        this.Write("**" + content + "**");
      } else {
        XmlDoc.VisitInnerNode(see, this);
      }
    }

    public void VisitItem(INode node) {
      this.Write(" * ");
      XmlDoc.VisitInnerNode(node, this);
      this.WriteLine("\r\n\r\n");
    }

    public void VisitList(INode node) {
      this.WriteLine("\r\n\r\n");
      XmlDoc.VisitInnerNode(node, this);
    }

    public void HandleMember(MemberInfo info, XmlDoc xmldoc) {
      var signature = String.Empty;
      var mnu = TypeNameUtil.XmlDocMemberName(info);
      var mnm = xmldoc.GetMemberNode(mnu);
      if (info is MethodBase) {
        var method = (MethodBase)info;
        if (!method.IsPublic && !method.IsFamily) {
         // Ignore methods other than public and protected
         // methods
          return;
        }
        if (mnm == null) {
          Console.WriteLine("member info not found: " + mnu);
          return;
        }
        using (var ch = this.AddMember(info)) {
          signature = FormatMethod(method, false);
          this.WriteLine("<a id=\"" +
                    MemberSummaryVisitor.MemberAnchor(info) + "\"></a>");
          this.WriteLine("### " + Heading(info) +
                    "\r\n\r\n" + signature + "\r\n\r\n");
          var attr = method.GetCustomAttribute(typeof(ObsoleteAttribute)) as
            ObsoleteAttribute;
          if (attr != null) {
            this.WriteLine("<b>Deprecated.</b> " +
DocGenUtil.HtmlEscape(attr.Message) + "\r\n\r\n");
          }
          var cattr =
method.GetCustomAttribute(typeof(CLSCompliantAttribute)) as
            CLSCompliantAttribute;
          if (cattr != null && !cattr.IsCompliant) {
            this.WriteLine("<b>This API is not CLS-compliant.</b>\r\n\r\n");
          }
          this.paramStr.Clear();
          this.returnStr.Clear();
          this.exceptionStr.Clear();
          XmlDoc.VisitInnerNode(mnm, this);
          if (this.paramStr.Length > 0) {
            this.Write("<b>Parameters:</b>\r\n\r\n");
            var paramString = this.paramStr.ToString();
           // Decrease spacing between list items
            paramString = paramString.Replace("\r\n * ", " * ");
            this.Write(paramString);
          }
          this.Write(this.returnStr.ToString());
          if (this.exceptionStr.Length > 0) {
            this.Write("<b>Exceptions:</b>\r\n\r\n");
            this.Write(this.exceptionStr.ToString());
          }
        }
      } else if (info is Type) {
        var type = (Type)info;
        if (!(type.IsNested ? type.IsNestedPublic : type.IsPublic)) {
         // Ignore nonpublic types
          return;
        }
        if (mnm == null) {
          Console.WriteLine("member info not found: " + mnu);
          return;
        }
        using (var ch = this.AddMember(info)) {
          this.WriteLine("## " + Heading(type) + "\r\n\r\n");
          this.WriteLine(FormatTypeSig(type) + "\r\n\r\n");
          var attr = type.GetCustomAttribute(typeof(ObsoleteAttribute)) as
            ObsoleteAttribute;
          if (attr != null) {
            this.WriteLine("<b>Deprecated.</b> " + attr.Message + "\r\n\r\n");
          }
          var cattr = type.GetCustomAttribute(typeof(CLSCompliantAttribute)) as
            CLSCompliantAttribute;
          if (cattr != null && !cattr.IsCompliant) {
            this.WriteLine("<b>This API is not CLS-compliant.</b>\r\n\r\n");
          }
          this.paramStr.Clear();
          XmlDoc.VisitInnerNode(mnm, this);
          this.Write("\r\n\r\n");
          this.WriteLine("<<<MEMBER_SUMMARY>>>");
          if (this.paramStr.Length > 0) {
            this.Write("<b>Parameters:</b>\r\n\r\n");
            var paramString = this.paramStr.ToString();
           // Decrease spacing between list items
            paramString = paramString.Replace("\r\n * ", " * ");
            this.Write(paramString);
          }
        }
      } else if (info is PropertyInfo) {
        var property = (PropertyInfo)info;
        if (!PropertyIsPublicOrFamily(property)) {
         // Ignore methods other than public and protected
         // methods
          return;
        }
        if (mnm == null) {
          Console.WriteLine("member info not found: " + mnu);
          return;
        }
        using (var ch = this.AddMember(info)) {
          signature = FormatProperty(property);
          this.WriteLine("<a id=\"" +
                    MemberSummaryVisitor.MemberAnchor(info) + "\"></a>");
          this.WriteLine("### " + property.Name + "\r\n\r\n" + signature +
                    "\r\n\r\n");
          var attr = property.GetCustomAttribute(typeof(ObsoleteAttribute)) as
            ObsoleteAttribute;
          if (attr != null) {
            this.WriteLine("<b>Deprecated.</b> " + attr.Message + "\r\n\r\n");
          }
          var cattr =
property.GetCustomAttribute(typeof(CLSCompliantAttribute)) as
            CLSCompliantAttribute;
          if (cattr != null && !cattr.IsCompliant) {
            this.WriteLine("<b>This API is not CLS-compliant.</b>\r\n\r\n");
          }
          this.paramStr.Clear();
          this.returnStr.Clear();
          this.exceptionStr.Clear();
          XmlDoc.VisitInnerNode(mnm, this);
          if (this.paramStr.Length > 0) {
            this.Write("<b>Parameters:</b>\r\n\r\n");
            this.Write(this.paramStr.ToString());
          }
          this.Write(this.returnStr.ToString());
          if (this.exceptionStr.Length > 0) {
            this.Write("<b>Exceptions:</b>\r\n\r\n");
            this.Write(this.exceptionStr.ToString());
          }
        }
      } else if (info is FieldInfo) {
        var field = (FieldInfo)info;
        if (!field.IsPublic && !field.IsFamily) {
         // Ignore nonpublic, nonprotected fields
          return;
        }
        if (mnm == null) {
          Console.WriteLine("member info not found: " + mnu);
          return;
        }
        using (var ch = this.AddMember(info)) {
          signature = FormatField(field);
          this.WriteLine("<a id=\"" +
                    MemberSummaryVisitor.MemberAnchor(info) + "\"></a>");
          this.WriteLine("### " + field.Name + "\r\n\r\n" + signature +
                    "\r\n\r\n");
          var attr = field.GetCustomAttribute(typeof(ObsoleteAttribute)) as
            ObsoleteAttribute;
          if (attr != null) {
            this.WriteLine("<b>Deprecated.</b> " + attr.Message + "\r\n\r\n");
          }
          var cattr = field.GetCustomAttribute(typeof(CLSCompliantAttribute)) as
            CLSCompliantAttribute;
          if (cattr != null && !cattr.IsCompliant) {
            this.WriteLine("<b>This API is not CLS-compliant.</b>\r\n\r\n");
          }
          XmlDoc.VisitInnerNode(mnm, this);
        }
      }
    }

    public void VisitPara(INode node) {
      XmlDoc.VisitInnerNode(node, this);
      this.WriteLine("\r\n\r\n");
    }

    public void VisitParam(INode node) {
      using (var ch = this.Change(this.paramStr)) {
        this.Write(" * <i>" + node.GetAttribute("name") + "</i>: ");
        XmlDoc.VisitInnerNode(node, this);
        this.WriteLine("\r\n\r\n");
      }
    }

    public void VisitParamRef(INode node) {
      this.WriteLine(" <i>" + node.GetAttribute("name") + "</i>");
      XmlDoc.VisitInnerNode(node, this);
    }

    public void VisitReturns(INode node) {
      using (var ch = this.Change(this.returnStr)) {
        this.WriteLine("<b>Return Value:</b>\r\n");
        XmlDoc.VisitInnerNode(node, this);
        this.WriteLine("\r\n\r\n");
      }
    }

    public void VisitTypeParam(INode node) {
      using (var ch = this.Change(this.paramStr)) {
        this.Write(" * &lt;" + node.GetAttribute("name") + "&gt;: ");
        XmlDoc.VisitInnerNode(node, this);
        this.WriteLine("\r\n\r\n");
      }
    }

    public void VisitValue(INode node) {
      using (var ch = this.Change(this.returnStr)) {
        this.WriteLine("<b>Returns:</b>\r\n");
        XmlDoc.VisitInnerNode(node, this);
        this.WriteLine("\r\n\r\n");
      }
    }

    private static string Heading(MemberInfo info) {
      var ret = String.Empty;
      if (info is MethodBase) {
        var method = (MethodBase)info;
        if (method is ConstructorInfo) {
          return TypeNameUtil.UndecorateTypeName(method.ReflectedType.Name) +
          " Constructor";
        }
        return MethodNameHeading(method.Name);
      }
      if (info is Type) {
        var type = (Type)info;
        return FormatType(type);
      } else if (info is PropertyInfo) {
        var property = (PropertyInfo)info;
        return property.Name;
      } else if (info is FieldInfo) {
        var field = (FieldInfo)info;
        return field.Name;
      }
      return ret;
    }

    private static string HeadingUnambiguous(MemberInfo info) {
      var ret = String.Empty;
      if (info is MethodBase) {
        var method = (MethodBase)info;
        return (method is ConstructorInfo) ? ("<1>" + " " +
          FormatMethod(method, false)) : ("<4>" + method.Name + " " +
          FormatMethod(method, false));
      }
      if (info is Type) {
        var type = (Type)info;
        return "<0>" + FormatType(type);
      } else if (info is PropertyInfo) {
        var property = (PropertyInfo)info;
        return "<3>" + property.Name;
      } else if (info is FieldInfo) {
        var field = (FieldInfo)info;
        return "<2>" + field.Name;
      }
      return ret;
    }

    private static string MethodNameHeading(string p) {
      return ValueOperators.TryGetValue(p, out string op) ? ("Operator `" +
        op + "`") : (p.Equals("op_Explicit", StringComparison.Ordinal) ?
          "Explicit Operator" :
         (p.Equals("op_Implicit", StringComparison.Ordinal) ?
          "Implicit Operator" : p));
    }

    private static IDictionary<string, string> OperatorList() {
      var ops = new Dictionary<string, string>();
      ops["op_Addition"] = "+";
      ops["op_UnaryPlus"] = "+";
      ops["op_Subtraction"] = "-";
      ops["op_UnaryNegation"] = "-";
      ops["op_Multiply"] = "*";
      ops["op_Division"] = "/";
      ops["op_LeftShift"] = "<<";
      ops["op_RightShift"] = ">>";
      ops["op_BitwiseAnd"] = "&";
      ops["op_BitwiseOr"] = "|";
      ops["op_ExclusiveOr"] = "^";
      ops["op_LogicalNot"] = "!";
      ops["op_OnesComplement"] = "~";
      ops["op_True"] = "true";
      ops["op_False"] = "false";
      ops["op_Modulus"] = "%";
      ops["op_Decrement"] = "--";
      ops["op_Increment"] = "++";
      ops["op_Equality"] = "==";
      ops["op_Inequality"] = "!=";
      ops["op_GreaterThan"] = ">";
      ops["op_GreaterThanOrEqual"] = ">=";
      ops["op_LessThan"] = "<";
      ops["op_LessThanOrEqual"] = "<=";
      return ops;
    }

    private static bool PropertyIsPublicOrFamily(PropertyInfo property) {
      var getter = property.GetGetMethod();
      var setter = property.GetSetMethod();
      return ((getter != null && getter.IsPublic) || (setter != null &&
        setter.IsPublic)) || ((getter != null && getter.IsFamily) || (setter !=
                null &&
  setter.IsFamily));
    }

    private IDisposable AddMember(MemberInfo member) {
      var builder = new StringBuilder();
      var heading = HeadingUnambiguous(member);
      this.members[heading] = builder;
      return new BufferChanger(this, builder);
    }

    private IDisposable Change(StringBuilder builder) {
      return new BufferChanger(this, builder);
    }

    private void Write(string ln) {
      if (this.currentBuffer != null) {
        this.currentBuffer.Append(ln);
      } else {
        this.buffer.Append(ln);
      }
    }

    private void WriteLine(string ln) {
      if (this.currentBuffer != null) {
        this.currentBuffer.Append(ln);
        this.currentBuffer.Append("\r\n");
      } else {
        this.buffer.Append(ln);
        this.buffer.Append("\r\n");
      }
    }

    private class BufferChanger : IDisposable {
      private readonly StringBuilder oldBuffer;
      private readonly DocVisitor vis;

      public BufferChanger(DocVisitor vis, StringBuilder buffer) {
        this.vis = vis;
        this.oldBuffer = vis.currentBuffer;
        vis.currentBuffer = buffer;
      }

      public void Dispose() {
        this.vis.currentBuffer = this.oldBuffer;
      }
    }
  }
}
