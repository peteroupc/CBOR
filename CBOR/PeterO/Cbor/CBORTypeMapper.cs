using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PeterO.Cbor {
  /// <summary>Holds converters to customize the serialization and
  /// deserialization behavior of <c>CBORObject.FromObject</c> and
  /// <c>CBORObject#ToObject</c>, as well as type filters for
  /// <c>ToObject</c>.</summary>
  public sealed class CBORTypeMapper {
    private readonly IList<string> typePrefixes;
    private readonly IList<string> typeNames;
    private readonly IDictionary<object, ConverterInfo>
    converters;

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.CBORTypeMapper'/> class.</summary>
    public CBORTypeMapper() {
      this.typePrefixes = new List<string>();
      this.typeNames = new List<string>();
      this.converters = new Dictionary<object, ConverterInfo>();
    }

    /// <summary>Registers an object that converts objects of a given type
    /// to CBOR objects (called a CBOR converter). If the CBOR converter
    /// converts to and from CBOR objects, it should implement the
    /// ICBORToFromConverter interface and provide ToCBORObject and
    /// FromCBORObject methods. If the CBOR converter only supports
    /// converting to (not from) CBOR objects, it should implement the
    /// ICBORConverter interface and provide a ToCBORObject
    /// method.</summary>
    /// <param name='type'>A Type object specifying the type that the
    /// converter converts to CBOR objects.</param>
    /// <param name='converter'>The parameter <paramref name='converter'/>
    /// is an ICBORConverter object.</param>
    /// <typeparam name='T'>Must be the same as the "type"
    /// parameter.</typeparam>
    /// <returns>This object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='type'/> or <paramref name='converter'/> is null.</exception>
    /// <exception cref='ArgumentException'>Converter doesn't contain a
    /// proper ToCBORObject method".</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public CBORTypeMapper AddConverter<T>(
      Type type,
      ICBORConverter<T> converter) {
      if (type == null) {
        throw new ArgumentNullException(nameof(type));
      }
      if (converter == null) {
        throw new ArgumentNullException(nameof(converter));
      }
      var ci = new ConverterInfo
      {
        Converter = converter,
        ToObject = PropertyMap.FindOneArgumentMethod(
          converter.GetType(),
          "ToCBORObject",
          type),
      };
      if (ci.ToObject == null) {
        throw new ArgumentException(
          "Converter doesn't contain a proper ToCBORObject method");
      }
      ci.FromObject = PropertyMap.FindOneArgumentMethod(
          converter.GetType(),
          "FromCBORObject",
          typeof(CBORObject));
      this.converters[type] = ci;
      return this;
    }

    internal object ConvertBackWithConverter(
      CBORObject cbor,
      Type type) {
      ConverterInfo convinfo = PropertyMap.GetOrDefault(
        this.converters,
        type,
        null);
      return convinfo == null ? null : (convinfo.FromObject == null) ? null :
        PropertyMap.CallFromObject(convinfo, cbor);
    }

    internal CBORObject ConvertWithConverter(object obj) {
      object type = obj.GetType();
      ConverterInfo convinfo = PropertyMap.GetOrDefault(
        this.converters,
        type,
        null);
      return (convinfo == null) ? null :
        PropertyMap.CallToObject(convinfo, obj);
    }

    /// <summary>Returns whether the given Java or.NET type name fits the
    /// filters given in this mapper.</summary>
    /// <param name='typeName'>The fully qualified name of a Java or.NET
    /// class (e.g., <c>java.math.BigInteger</c> or
    /// <c>System.Globalization.CultureInfo</c> ).</param>
    /// <returns>Either <c>true</c> if the given Java or.NET type name fits
    /// the filters given in this mapper, or <c>false</c>
    /// otherwise.</returns>
    public bool FilterTypeName(string typeName) {
      if (String.IsNullOrEmpty(typeName)) {
        return false;
      }
      foreach (string prefix in this.typePrefixes) {
        if (typeName.Length >= prefix.Length &&
          typeName.Substring(0, prefix.Length).Equals(prefix,
            StringComparison.Ordinal)) {
          return true;
        }
      }
      foreach (string name in this.typeNames) {
        if (typeName.Equals(name, StringComparison.Ordinal)) {
          return true;
        }
      }
      return false;
    }

    /// <summary>Adds a prefix of a Java or.NET type for use in type
    /// matching. A type matches a prefix if its fully qualified name is or
    /// begins with that prefix, using codepoint-by-codepoint
    /// (case-sensitive) matching.</summary>
    /// <param name='prefix'>The prefix of a Java or.NET type (e.g.,
    /// `java.math.` or `System.Globalization`).</param>
    /// <returns>This object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='prefix'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='prefix'/> is empty.</exception>
    public CBORTypeMapper AddTypePrefix(string prefix) {
      if (prefix == null) {
        throw new ArgumentNullException(nameof(prefix));
      }
      if (prefix.Length == 0) {
        throw new ArgumentException("prefix" + " is empty.");
      }
      this.typePrefixes.Add(prefix);
      return this;
    }

    /// <summary>Adds the fully qualified name of a Java or.NET type for
    /// use in type matching.</summary>
    /// <param name='name'>The fully qualified name of a Java or.NET class
    /// (e.g., <c>java.math.BigInteger</c> or
    /// <c>System.Globalization.CultureInfo</c> ).</param>
    /// <returns>This object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='name'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='name'/> is empty.</exception>
    public CBORTypeMapper AddTypeName(string name) {
      if (name == null) {
        throw new ArgumentNullException(nameof(name));
      }
      if (name.Length == 0) {
        throw new ArgumentException("name" + " is empty.");
      }
      this.typeNames.Add(name);
      return this;
    }

    internal sealed class ConverterInfo {
      public object ToObject
      {
        get;
        set;
      }

      public object FromObject {
        get;
        set;
      }

      public object Converter {
        get;
        set;
      }
    }
  }
}
