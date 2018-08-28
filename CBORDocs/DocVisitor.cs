/*
Written by Peter O. in 2014.

Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using NuDoq;

namespace PeterO.DocGen {
    /// <summary>A documentation visitor.</summary>
  internal class DocVisitor : Visitor {
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
      builder.Append(" ");
      builder.Append(field.Name);
      if (field.IsLiteral) {
        try {
          var obj = field.GetRawConstantValue();
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

    public static string FormatMethod(MethodBase method) {
      var builder = new StringBuilder();
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
     } else if (method is MethodInfo && IsMethodOverride((MethodInfo)method)) {
          builder.Append("override ");
        } else if (method.IsVirtual) {
          builder.Append("virtual ");
        }
      }
      var methodInfo = method as MethodInfo;
      var isExtension = false;
      Attribute attr;
      if (methodInfo != null) {
        attr = methodInfo.GetCustomAttribute(
          typeof(System.Runtime.CompilerServices.ExtensionAttribute));
        isExtension = attr != null;
        if (method.Name.Equals("op_Explicit")) {
          builder.Append("explicit operator ");
          builder.Append(FormatType(methodInfo.ReturnType));
        } else if (method.Name.Equals("op_Implicit")) {
          builder.Append("implicit operator ");
          builder.Append(FormatType(methodInfo.ReturnType));
        } else if (ValueOperators.ContainsKey(method.Name)) {
          builder.Append(FormatType(methodInfo.ReturnType));
          builder.Append(" operator ");
          builder.Append(ValueOperators[method.Name]);
        } else {
          builder.Append(FormatType(methodInfo.ReturnType));
          builder.Append(" ");
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
      builder.Append("(");
      first = true;
      foreach (var param in method.GetParameters()) {
        if (!first) {
          builder.Append(",\r\n" + FourSpaces + FourSpaces);
        } else {
          builder.Append("\r\n" + FourSpaces + FourSpaces);
        }
        if (first && isExtension) {
          builder.Append("this ");
        }
        attr = param.GetCustomAttribute(typeof(ParamArrayAttribute));
        if (attr != null) {
          builder.Append("params ");
        }
        builder.Append(FormatType(param.ParameterType));
        builder.Append(" ");
        builder.Append(param.Name);
        first = false;
      }
      builder.Append(")");
      if (method is MethodInfo && method.GetGenericArguments().Length > 0) {
        AppendConstraints(method.GetGenericArguments(), builder);
      }
      builder.Append(";");
      return builder.ToString();
    }

    public static string FormatProperty(PropertyInfo property) {
      var builder = new StringBuilder();
      var getter = property.GetGetMethod();
      var setter = property.GetSetMethod();
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
      builder.Append(" ");
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
        builder.Append(" ");
        builder.Append(param.Name);
        first = false;
      }
      if (indexParams.Length > 0) {
        builder.Append(")");
      }
      builder.Append(" { ");
      if (getter != null) {
        if (getter.IsPrivate && setter != null && !setter.IsPrivate) {
          builder.Append("private ");
        }
        builder.Append("get; ");
      }
      if (setter != null) {
        if (setter.IsPrivate && getter != null && !getter.IsPrivate) {
          builder.Append("private ");
        }
        builder.Append("set;");
      }
      builder.Append("}");
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
      name = type.Namespace + "." + name;
      return name.Equals("System.Int32") ? "int" :
        (name.Equals("System.Int64") ? "long" :
         (name.Equals("System.Int16") ? "short" :
          (name.Equals("System.UInt32") ? "uint" :
           (name.Equals("System.UInt64") ? "ulong" :
            (name.Equals("System.UInt16") ? "ushort" :
             (name.Equals("System.Char") ? "char" :
              (name.Equals("System.Object") ? "object" :
           (name.Equals("System.Void") ? "void" : (name.Equals("System.Byte") ?
                    "byte" :
  (name.Equals("System.SByte") ? "sbyte" : (name.Equals("System.String") ?
                "string" : (name.Equals("System.Boolean") ?
  "bool" : (name.Equals("System.Single") ?
  "float" : (name.Equals("System.Double") ? "double" :
  name))))))))))))));
    }

    public static string FormatTypeSig(Type typeInfo) {
      var builder = new StringBuilder();
      builder.Append(FourSpaces);
      if (typeInfo.IsPublic) {
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
          foreach (var iface in ifaces) {
            if (!first) {
              builder.Append(",\r\n" + FourSpaces);
            }
            builder.Append(FourSpaces + FormatType(iface));
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

    public override void VisitC(C code) {
      this.Write(" `" + code.Content + "` ");
      base.VisitC(code);
    }

    public override void VisitCode(Code code) {
      this.WriteLine("\r\n\r\n");
      foreach (var line in code.Content.Split('\n')) {
        this.WriteLine(FourSpaces + line.TrimEnd());
      }
      this.WriteLine("\r\n\r\n");
      base.VisitCode(code);
    }

    public override void VisitExample(Example example) {
      base.VisitExample(example);
      this.WriteLine("\r\n\r\n");
    }

    public override void VisitException(NuDoq.Exception exception) {
      using (var ch = this.Change(this.exceptionStr)) {
        var cref = exception.Cref;
        if (cref.StartsWith("T:", StringComparison.Ordinal)) {
          cref = cref.Substring(2);
        }
        this.WriteLine(" * " + cref + ": ");
        base.VisitException(exception);
        this.WriteLine("\r\n\r\n");
      }
    }

    private static string HtmlEscape(string str) {
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

    public override void VisitUnknownElement(UnknownElement element) {
      string xmlName = element.Xml.Name.ToString()
        .ToLowerInvariant();
      if (xmlName.Equals("b") ||
        xmlName.Equals("i") || xmlName.Equals("a") ||
        xmlName.Equals("sup") || xmlName.Equals("i")) {
        var sb = new StringBuilder();
        sb.Append("<" + xmlName);
        foreach (var attr in element.Xml.Attributes()) {
          sb.Append(" " + attr.Name.ToString() + "=");
          sb.Append("\"" + HtmlEscape(attr.Value) + "\"");
        }
        sb.Append(">");
        this.Write(sb.ToString());
        base.VisitUnknownElement(element);
        this.Write("</" + xmlName + ">");
      } else {
        base.VisitUnknownElement(element);
      }
    }

    public override void VisitSee(See see) {
      string cref = see.Cref;
      if (cref.Substring(0, 2).Equals("T:")) {
        string typeName = TypeNameUtil.UndecorateTypeName(cref.Substring(2));
        string content = HtmlEscape(see.Content);
        if (String.IsNullOrEmpty(content)) {
          content = HtmlEscape(see.ToText());
        }
        this.Write("[" + content + "]");
        this.Write("(" + typeName + ".md)");
        base.VisitSee(see);
      } else if (cref.Substring(0, 2).Equals("M:")) {
        string content = HtmlEscape(see.Content);
        if (String.IsNullOrEmpty(content)) {
          content = HtmlEscape(see.ToText());
        }
        this.Write("**" + content + "**");
      } else {
        base.VisitSee(see);
      }
    }

    public override void VisitItem(Item item) {
      this.Write(" * ");
      base.VisitItem(item);
      this.WriteLine("\r\n\r\n");
    }

    public override void VisitList(List list) {
      this.WriteLine("\r\n\r\n");
      base.VisitList(list);
    }

    public override void VisitMember(Member member) {
      var info = member.Info;
      var signature = String.Empty;
      if (info is MethodBase) {
        var method = (MethodBase)info;
        if (!method.IsPublic && !method.IsFamily) {
          // Ignore methods other than public and protected
          // methods
          return;
        }
        using (var ch = this.AddMember(info)) {
          signature = FormatMethod(method);
          this.WriteLine("### " + Heading(info) +
                    "\r\n\r\n" + signature + "\r\n\r\n");
          var attr = method.GetCustomAttribute(typeof(ObsoleteAttribute)) as
            ObsoleteAttribute;
          if (attr != null) {
            this.WriteLine("<b>Deprecated.</b> " + attr.Message + "\r\n\r\n");
          }
          this.paramStr.Clear();
          this.returnStr.Clear();
          this.exceptionStr.Clear();
          base.VisitMember(member);
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
        if (!type.IsPublic) {
          // Ignore nonpublic types
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
          this.paramStr.Clear();
          base.VisitMember(member);
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
        using (var ch = this.AddMember(info)) {
          signature = FormatProperty(property);
          this.WriteLine("### " + property.Name + "\r\n\r\n" + signature +
                    "\r\n\r\n");
          var attr = property.GetCustomAttribute(typeof(ObsoleteAttribute)) as
            ObsoleteAttribute;
          if (attr != null) {
            this.WriteLine("<b>Deprecated.</b> " + attr.Message + "\r\n\r\n");
          }
          this.paramStr.Clear();
          this.returnStr.Clear();
          this.exceptionStr.Clear();
          base.VisitMember(member);
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
        using (var ch = this.AddMember(info)) {
          signature = FormatField(field);
          this.WriteLine("### " + field.Name + "\r\n\r\n" + signature +
                    "\r\n\r\n");
          base.VisitMember(member);
        }
      }
    }

    public override void VisitPara(Para para) {
      base.VisitPara(para);
      this.WriteLine("\r\n\r\n");
    }

    public override void VisitParam(Param param) {
      using (var ch = this.Change(this.paramStr)) {
        this.Write(" * <i>" + param.Name + "</i>: ");
        base.VisitParam(param);
        this.WriteLine("\r\n\r\n");
      }
    }

    public override void VisitParamRef(ParamRef paramRef) {
      this.WriteLine(" <i>" + paramRef.Name + "</i>");
      base.VisitParamRef(paramRef);
    }

    public override void VisitRemarks(Remarks remarks) {
      base.VisitRemarks(remarks);
      this.WriteLine("\r\n\r\n");
    }

    public override void VisitReturns(Returns returns) {
      using (var ch = this.Change(this.returnStr)) {
        this.WriteLine("<b>Return Value:</b>\r\n");
        base.VisitReturns(returns);
        this.WriteLine("\r\n\r\n");
      }
    }

    public override void VisitSummary(Summary summary) {
      base.VisitSummary(summary);
      this.WriteLine("\r\n\r\n");
    }

    public override void VisitText(Text text) {
      var t = text.Content;
      // Collapse multiple spaces into a single space
      t = Regex.Replace(t, @"\s+", " ");
      this.Write(t);
      base.VisitText(text);
    }

    public override void VisitTypeParam(TypeParam typeParam) {
      using (var ch = this.Change(this.paramStr)) {
        this.Write(" * &lt;" + typeParam.Name + "&gt;: ");
        base.VisitTypeParam(typeParam);
        this.WriteLine("\r\n\r\n");
      }
    }

    public override void VisitValue(Value value) {
      using (var ch = this.Change(this.returnStr)) {
        this.WriteLine("<b>Returns:</b>\r\n");
        base.VisitValue(value);
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
          FormatMethod(method)) : ("<4>" + method.Name + " " +
          FormatMethod(method));
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
      return ValueOperators.ContainsKey(p) ? ("Operator `" +
        ValueOperators[p] + "`") :
        (p.Equals("op_Explicit") ? "Explicit Operator" :
         (p.Equals("op_Implicit") ? "Implicit Operator" : p));
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
      var buffer = new StringBuilder();
      var heading = HeadingUnambiguous(member);
      this.members[heading] = buffer;
      return new BufferChanger(this, buffer);
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
