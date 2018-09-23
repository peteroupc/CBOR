using System;
using System.Collections.Generic;
namespace PeterO.Cbor {
  public class CBORTypeMapper {
    private readonly IList<string> typePrefixes;
    private readonly IList<string> typeNames;
    private readonly IDictionary<Object, ConverterInfo>
      converters = new Dictionary<Object, ConverterInfo>();

    public void AddConverter<T>(Type type, ICBORConverter<T> converter) {
      if (type == null) {
        throw new ArgumentNullException(nameof(type));
      }
      if (converter == null) {
        throw new ArgumentNullException(nameof(converter));
      }
      ConverterInfo ci = new ConverterInfo();
      ci.Converter = converter;
      ci.ToObject = PropertyMap.FindOneArgumentMethod(
        converter,
        "ToCBORObject",
        type);
      if (ci.ToObject == null) {
        throw new ArgumentException(
          "Converter doesn't contain a proper ToCBORObject method");
      }
      converters[type] = ci;
    }

    internal CBORObject ConvertWithConverter(object obj) {
      Object type = obj.GetType();
      ConverterInfo convinfo = null;
        if (converters.ContainsKey(type)) {
          convinfo = converters[type];
        } else {
          return null;
        }
      if (convinfo == null) {
        return null;
      }
      return (CBORObject)PropertyMap.InvokeOneArgumentMethod(
        convinfo.ToObject,
        convinfo.Converter,
        obj);
    }


    public CBORTypeMapper AddTypePrefix(string prefix) {
      //ArgumentAssert.NotEmpty(prefix);
      return this;
    }
    public CBORTypeMapper AddTypeName(string name) {
      //ArgumentAssert.NotEmpty(name);
      return this;
    }

    internal sealed class ConverterInfo {
      private object toObject;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.ConverterInfo.ToObject"]/*'/>
      public object ToObject {
        get {
          return this.toObject;
        }

        set {
          this.toObject = value;
        }
      }

      private object converter;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.ConverterInfo.Converter"]/*'/>
      public object Converter {
        get {
          return this.converter;
        }

        set {
          this.converter = value;
        }
      }
    }
  }
}
