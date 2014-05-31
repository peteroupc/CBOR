/*
Written in 2014 by Peter O.

Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using ClariusLabs.NuDoc;
using PeterO.Cbor;

namespace CBORDocs {
  public class DocVisitor : Visitor {
    private StringBuilder paramStr = new StringBuilder();
    private StringBuilder returnStr = new StringBuilder();
    private StringBuilder exceptionStr = new StringBuilder();
    private StringBuilder currentBuffer = null;
    private StringBuilder buffer = new StringBuilder();

    public override string ToString() {
      return buffer.ToString();
    }

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

    private static string valueFourSpaces = " " + " " + " " + " ";

    public static string FormatTypeSig(Type typeInfo) {
      StringBuilder builder = new StringBuilder();
      builder.Append(valueFourSpaces);
      if (typeInfo.IsPublic) {
        builder.Append("public ");
      } else {
        builder.Append("internal ");
      }
      if (typeInfo.IsAbstract && typeInfo.IsSealed) {
        builder.Append("static ");
      } else if (typeInfo.IsAbstract) {
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
      if (typeInfo.BaseType != null &&
        typeInfo.BaseType.Equals(typeof(object))) {
        derived = null;
      }
      if (derived != null || ifaces.Length > 0) {
        builder.Append(" :\r\n" + valueFourSpaces);
        if (derived != null) {
          builder.Append(valueFourSpaces + FormatType(derived));
          first = false;
        }
        if (ifaces.Length > 0) {
          foreach (var iface in ifaces) {
            if (!first) {
              builder.Append(",\r\n" + valueFourSpaces);
            }
            builder.Append(valueFourSpaces + FormatType(iface));
            first = false;
          }
        }
      }
      AppendConstraints(typeInfo.GetGenericArguments(), builder);
      return builder.ToString();
    }

    public static void AppendConstraints(Type[] genericArguments, StringBuilder builder) {
      foreach (var arg in genericArguments) {
        if (arg.IsGenericParameter) {
          var constraints = arg.GetGenericParameterConstraints();
          if (constraints.Length == 0 && (arg.GenericParameterAttributes &
            (GenericParameterAttributes.ReferenceTypeConstraint |
            GenericParameterAttributes.NotNullableValueTypeConstraint |
            GenericParameterAttributes.DefaultConstructorConstraint)) == GenericParameterAttributes.None) {
              continue;
          }
          builder.Append("\r\n" + valueFourSpaces + valueFourSpaces + "where ");
          builder.Append(UndecorateTypeName(arg.Name));
          builder.Append(" : ");
          bool first = true;
          if ((arg.GenericParameterAttributes &
            GenericParameterAttributes.ReferenceTypeConstraint) != GenericParameterAttributes.None) {
              if (!first) {
                builder.Append(", ");
              }
            builder.Append("class");
            first = false;
          }
          if ((arg.GenericParameterAttributes &
            GenericParameterAttributes.NotNullableValueTypeConstraint) != GenericParameterAttributes.None) {
            if (!first) {
              builder.Append(", ");
            }
            builder.Append("struct");
            first = false;
          }
          if ((arg.GenericParameterAttributes &
            GenericParameterAttributes.DefaultConstructorConstraint) != GenericParameterAttributes.None) {
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
      StringBuilder builder = new StringBuilder();
      builder.Append(valueFourSpaces);
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
      // TODO: Operator names
      if (method is MethodInfo) {
        builder.Append(FormatType(((MethodInfo)method).ReturnType));
        builder.Append(" ");
        builder.Append(method.Name);
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
          builder.Append(",\r\n" + valueFourSpaces);
        } else {
          builder.Append("\r\n" + valueFourSpaces);
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
      if ((getter == null ? false : getter.IsPublic) ||
         (setter == null ? false : setter.IsPublic)) {
        return true;
      } else if ((getter == null ? false : getter.IsAssembly) ||
        (setter == null ? false : setter.IsAssembly)) {
        return true;
      }
      return false;
    }

    public static string FormatProperty(PropertyInfo property) {
      StringBuilder builder = new StringBuilder();
      MethodInfo getter = property.GetGetMethod();
      MethodInfo setter = property.GetSetMethod();
      builder.Append(valueFourSpaces);
      if ((getter == null ? false : getter.IsPublic) ||
        (setter == null ? false : setter.IsPublic)) {
        builder.Append("public ");
      } else if ((getter == null ? false : getter.IsAssembly) ||
        (setter == null ? false : setter.IsAssembly)) {
        builder.Append("internal ");
      } else if ((getter == null ? false : getter.IsFamily) ||
        (setter == null ? false : setter.IsFamily)) {
        builder.Append("protected ");
      }
      if ((getter == null ? false : getter.IsStatic) ||
        (setter == null ? false : setter.IsStatic)) {
        builder.Append("static ");
      }
      if ((getter == null ? false : getter.IsAbstract) ||
        (setter == null ? false : setter.IsAbstract)) {
        builder.Append("abstract ");
      }
      if ((getter == null ? false : getter.IsFinal) ||
        (setter == null ? false : setter.IsFinal)) {
        builder.Append("sealed ");
      } else if (IsMethodOverride(getter ?? setter)) {
        builder.Append("override ");
      } else if ((getter == null ? false : getter.IsVirtual) ||
        (setter == null ? false : setter.IsVirtual)) {
        builder.Append("virtual ");
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
          builder.Append(",\r\n" + valueFourSpaces);
        } else {
          builder.Append(indexParams.Length == 1 ?
            String.Empty : "\r\n" + valueFourSpaces);
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
      StringBuilder builder = new StringBuilder();
      builder.Append(valueFourSpaces);
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
      if (name.Equals("System.Object")) {
        return "object";
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
      if (name.Equals("System.String")) {
        return "string";
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

    public static bool IsMethodOverride(MethodInfo method) {
      Type type = method.DeclaringType;
      MethodInfo baseMethod = method.GetBaseDefinition();
      if (baseMethod == null) {
        return false;
      }
      if (method.Equals(baseMethod)) {
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

    public override void VisitValue(Value param) {
      currentBuffer = returnStr;
      WriteLine("<b>Returns:</b>\r\n");
      base.VisitValue(param);
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
      WriteLine(" <i>" + param.Name + "</i>");
      base.VisitParamRef(param);
    }

    public override void VisitMember(Member member) {
      MemberInfo info = member.Info;
      string signature = String.Empty;
      if (info is MethodBase) {
        MethodBase method = (MethodBase)info;
        if (!method.IsPublic && !method.IsFamily) {
          // Ignore methods other than public and protected
          // methods
          return;
        }
        signature = FormatMethod(method);
        if (method is ConstructorInfo) {
          WriteLine("### " + UndecorateTypeName(method.ReflectedType.Name) +
            " Constructor\r\n\r\n" + signature + "\r\n\r\n");
        } else {
          WriteLine("### " + method.Name + "\r\n\r\n" + signature + "\r\n\r\n");
        }
        paramStr.Clear();
        returnStr.Clear();
        exceptionStr.Clear();
        base.VisitMember(member);
        if (paramStr.Length > 0) {
          Write("<b>Parameters:</b>\r\n\r\n");
          string paramString = paramStr.ToString();
          // Decrease spacing between list items
          paramString = paramString.Replace("\r\n * ", " * ");
          Write(paramString);
        }
        Write(returnStr.ToString());
        if (exceptionStr.Length > 0) {
          Write("<b>Exceptions:</b>\r\n\r\n");
          Write(exceptionStr.ToString());
        }
      } else if (info is Type) {
        Type type = (Type)info;
        if (!type.IsPublic) {
          // Ignore nonpublic types
          return;
        }
        WriteLine("## " + FormatType(type) + "\r\n\r\n");
        WriteLine(FormatTypeSig(type) + "\r\n\r\n");
        base.VisitMember(member);
      } else if (info is PropertyInfo) {
        PropertyInfo property = (PropertyInfo)info;
        if (!PropertyIsPublicOrFamily(property)) {
          // Ignore methods other than public and protected
          // methods
          return;
        }
        signature = FormatProperty(property);
        WriteLine("### " + property.Name + "\r\n\r\n" + signature + "\r\n\r\n");
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
        WriteLine("### " + field.Name + "\r\n\r\n" + signature + "\r\n\r\n");
        base.VisitMember(member);
      }
    }

    public override void VisitSummary(Summary summary) {
      // WriteLine("<b>Summary:</b>\r\n");
      base.VisitSummary(summary);
      WriteLine("\r\n\r\n");
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
        WriteLine(valueFourSpaces + line.TrimEnd());
      }
      WriteLine("\r\n\r\n");
      base.VisitCode(code);
    }

    private void Write(string ln) {
      if (currentBuffer != null) {
        currentBuffer.Append(ln);
      } else {
        buffer.Append(ln);
      }
    }

    private void WriteLine(string ln) {
      if (currentBuffer != null) {
        currentBuffer.Append(ln);
        currentBuffer.Append("\r\n");
      } else {
        buffer.Append(ln);
        buffer.Append("\r\n");
      }
    }

    public void Debug(string ln) {
      WriteLine(ln);
      WriteLine(String.Empty);
    }

    public override void VisitPara(Para para) {
      base.VisitPara(para);
      WriteLine("\r\n\r\n");
    }
  }

  public class Test {
    public new bool Equals(object o) {
      return false;
    }

    public override string ToString() {
      return base.ToString();
    }

    public virtual void Virtual() {
    }

    public void Ordinary() {
    }
  }

  public class TypeVisitor : Visitor, IComparer<Type> {
    private SortedDictionary<Type, DocVisitor> docs;
    private TextWriter writer;

    public TypeVisitor(TextWriter writer) {
      docs = new SortedDictionary<Type, DocVisitor>(this);
      this.writer = writer;
    }

    public void Finish() {
      foreach (var key in docs.Keys) {
        writer.WriteLine(docs[key].ToString());
      }
    }

    public override void VisitMember(Member member) {
      Type currentType;
      if (member.Info is Type) {
        currentType = (Type)member.Info;
      } else {
        if (member.Info == null) {
          return;
        }
        currentType = member.Info.ReflectedType;
      }
      if (currentType == null || !currentType.IsPublic) {
        return;
      }
      if (!docs.ContainsKey(currentType)) {
        var docVisitor = new DocVisitor();
        docs[currentType] = docVisitor;
      }
      docs[currentType].VisitMember(member);
      base.VisitMember(member);
    }

    public int Compare(Type x, Type y) {
      return x.FullName.CompareTo(y.FullName);
    }
  }

  internal class Program {
    internal static void Main(string[] args) {
      var members = DocReader.Read(typeof(CBORObject).Assembly);
      var oldWriter = Console.Out;
      using (var writer = new StreamWriter("../../../APIDocs.md")) {
        var visitor = new TypeVisitor(writer);
        members.Accept(visitor);
        visitor.Finish();
      }
    }
  }
}
