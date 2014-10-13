/*
Written in 2014 by Peter O.

Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using ClariusLabs.NuDoc;

namespace PeterO.DocGen {
    /// <summary>A documentation visitor.</summary>
  internal class DocVisitor : Visitor {
    private const string FourSpaces = " " + " " + " " + " ";

    private StringBuilder paramStr = new StringBuilder();
    private StringBuilder returnStr = new StringBuilder();
    private StringBuilder exceptionStr = new StringBuilder();
    private StringBuilder currentBuffer;
    private StringBuilder buffer = new StringBuilder();

    public override string ToString() {
      return this.buffer.ToString();
    }

    public static string GetTypeID(Type type) {
      string name = FormatType(type);
      var builder = new StringBuilder();
      for (var i = 0; i < name.Length; ++i) {
        UnicodeCategory cat = CharUnicodeInfo.GetUnicodeCategory(name, i);
        int cp = DataUtilities.CodePointAt(name, i);
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

    public static string FormatType(Type type) {
      string rawfmt = FormatTypeRaw(type);
      if (!type.IsArray && !type.IsGenericType) {
        return rawfmt;
      }
      var sb = new StringBuilder();
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
        for (var i = 0; i < type.GetArrayRank(); ++i) {
          sb.Append("[]");
        }
      }
      return sb.ToString();
    }

    private static IDictionary<string, string> operators = OperatorList();

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
      builder.Append(UndecorateTypeName(typeInfo.Name));
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
          builder.Append(UndecorateTypeName(arg.Name));
          builder.Append(" : ");
          bool first = true;
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
      if (methodInfo != null) {
        if (method.Name.Equals("op_Explicit")) {
          builder.Append("explicit operator ");
          builder.Append(FormatType(methodInfo.ReturnType));
        } else if (method.Name.Equals("op_Implicit")) {
          builder.Append("implicit operator ");
          builder.Append(FormatType(methodInfo.ReturnType));
        } else if (operators.ContainsKey(method.Name)) {
          builder.Append(FormatType(methodInfo.ReturnType));
          builder.Append(" operator ");
          builder.Append(operators[method.Name]);
        } else {
          builder.Append(FormatType(methodInfo.ReturnType));
          builder.Append(" ");
          builder.Append(method.Name);
        }
      } else {
        builder.Append(UndecorateTypeName(method.ReflectedType.Name));
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
        Attribute attr = param.GetCustomAttribute(typeof(ParamArrayAttribute));
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

    private static bool PropertyIsPublicOrFamily(PropertyInfo property) {
      MethodInfo getter = property.GetGetMethod();
      MethodInfo setter = property.GetSetMethod();
      return ((getter != null && getter.IsPublic) || (setter != null &&
        setter.IsPublic)) || ((getter != null && getter.IsFamily) || (setter !=
                null &&
  setter.IsFamily));
    }

    public static string FormatProperty(PropertyInfo property) {
      var builder = new StringBuilder();
      MethodInfo getter = property.GetGetMethod();
      MethodInfo setter = property.GetSetMethod();
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
        Attribute attr = param.GetCustomAttribute(typeof(ParamArrayAttribute));
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

    public static string UndecorateTypeName(string name) {
      int idx = name.IndexOf('`');
      if (idx >= 0) {
        name = name.Substring(0, idx);
      }
      idx = name.IndexOf('[');
      if (idx >= 0) {
        name = name.Substring(0, idx);
      }
      return name;
    }

    public static string FormatTypeRaw(Type type) {
      string name = UndecorateTypeName(type.Name);
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

    public static bool IsMethodOverride(MethodInfo method) {
      Type type = method.DeclaringType;
      MethodInfo baseMethod = method.GetBaseDefinition();
      return (baseMethod != null) && (!method.Equals(baseMethod));
    }

    public override void VisitReturns(Returns returns) {
      this.currentBuffer = this.returnStr;
      this.WriteLine("<b>Returns:</b>\r\n");
      base.VisitReturns(returns);
      this.WriteLine("\r\n\r\n");
      this.currentBuffer = null;
    }

    public override void VisitValue(Value value) {
      this.currentBuffer = this.returnStr;
      this.WriteLine("<b>Returns:</b>\r\n");
      base.VisitValue(value);
      this.WriteLine("\r\n\r\n");
      this.currentBuffer = null;
    }

    public override void VisitException(ClariusLabs.NuDoc.Exception exception) {
      this.currentBuffer = this.exceptionStr;
      string cref = exception.Cref;
      if (cref.StartsWith("T:", StringComparison.Ordinal)) {
        cref = cref.Substring(2);
      }
      this.WriteLine(" * " + cref + ": ");
      base.VisitException(exception);
      this.WriteLine("\r\n\r\n");
      this.currentBuffer = null;
    }

    public override void VisitParam(Param param) {
      this.currentBuffer = this.paramStr;
      this.Write(" * <i>" + param.Name + "</i>: ");
      base.VisitParam(param);
      this.WriteLine("\r\n\r\n");
      this.currentBuffer = null;
    }

    public override void VisitList(List list) {
      this.WriteLine("\r\n\r\n");
      base.VisitList(list);
    }

    public override void VisitItem(Item item) {
      this.Write(" * ");
      base.VisitItem(item);
      this.WriteLine("\r\n\r\n");
    }

    public override void VisitTypeParam(TypeParam typeParam) {
      this.currentBuffer = this.paramStr;
      this.Write(" * &lt;" + typeParam.Name + "&gt;: ");
      base.VisitTypeParam(typeParam);
      this.WriteLine("\r\n\r\n");
      this.currentBuffer = null;
    }

    public override void VisitParamRef(ParamRef paramRef) {
      this.WriteLine(" <i>" + paramRef.Name + "</i>");
      base.VisitParamRef(paramRef);
    }

    public override void VisitMember(Member member) {
      MemberInfo info = member.Info;
      string signature = String.Empty;
      if (info is MethodBase) {
        var method = (MethodBase)info;
        if (!method.IsPublic && !method.IsFamily) {
          // Ignore methods other than public and protected
          // methods
          return;
        }
        signature = FormatMethod(method);
        if (method is ConstructorInfo) {
        this.WriteLine("### " + UndecorateTypeName(method.ReflectedType.Name) +
                         " Constructor\r\n\r\n" + signature + "\r\n\r\n");
        } else {
          this.WriteLine("### " + MethodNameHeading(method.Name) +
                         "\r\n\r\n" + signature + "\r\n\r\n");
        }
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
          string paramString = this.paramStr.ToString();
          // Decrease spacing between list items
          paramString = paramString.Replace("\r\n * ", " * ");
          this.Write(paramString);
        }
        this.Write(this.returnStr.ToString());
        if (this.exceptionStr.Length > 0) {
          this.Write("<b>Exceptions:</b>\r\n\r\n");
          this.Write(this.exceptionStr.ToString());
        }
      } else if (info is Type) {
        var type = (Type)info;
        if (!type.IsPublic) {
          // Ignore nonpublic types
          return;
        }
        this.WriteLine("## " + FormatType(type) + "\r\n\r\n");
        this.WriteLine(FormatTypeSig(type) + "\r\n\r\n");
        var attr = type.GetCustomAttribute(typeof(ObsoleteAttribute)) as
          ObsoleteAttribute;
        if (attr != null) {
          this.WriteLine("<b>Deprecated.</b> " + attr.Message + "\r\n\r\n");
        }
        base.VisitMember(member);
      } else if (info is PropertyInfo) {
        var property = (PropertyInfo)info;
        if (!PropertyIsPublicOrFamily(property)) {
          // Ignore methods other than public and protected
          // methods
          return;
        }
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
      } else if (info is FieldInfo) {
        var field = (FieldInfo)info;
        if (!field.IsPublic && !field.IsFamily) {
          // Ignore nonpublic, nonprotected fields
          return;
        }
        signature = FormatField(field);
        this.WriteLine("### " + field.Name + "\r\n\r\n" + signature +
                       "\r\n\r\n");
        base.VisitMember(member);
      }
    }

    private static string MethodNameHeading(string p) {
      return operators.ContainsKey(p) ? ("Operator `" + operators[p] + "`") :
        (p.Equals("op_Explicit") ? "Explicit Operator" :
         (p.Equals("op_Implicit") ? "Implicit Operator" : p));
    }

    public override void VisitSummary(Summary summary) {
      base.VisitSummary(summary);
      this.WriteLine("\r\n\r\n");
    }

    public override void VisitText(Text text) {
      string t = text.Content;
      // Collapse multiple spaces into a single space
      t = Regex.Replace(t, @"\s+", " ");
      this.Write(t);
      base.VisitText(text);
    }

    public override void VisitC(C code) {
      this.Write(" `" + code.Content + "` ");
      base.VisitC(code);
    }

    public override void VisitCode(Code code) {
      foreach (var line in code.Content.Split('\n')) {
        this.WriteLine(FourSpaces + line.TrimEnd());
      }
      this.WriteLine("\r\n\r\n");
      base.VisitCode(code);
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

    public void Debug(string ln) {
      this.WriteLine(ln);
      this.WriteLine(String.Empty);
    }

    public override void VisitPara(Para para) {
      base.VisitPara(para);
      this.WriteLine("\r\n\r\n");
    }
  }
}
