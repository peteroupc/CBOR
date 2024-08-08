/*
Written in 2013 by Peter Occil.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PeterO.Cbor;
using PeterO.Numbers;

namespace PeterO {
  /// <summary>Contains methods for reading and writing objects
  /// represented in BEncode, a serialization format used in the
  /// BitTorrent protocol. For more information, see:
  /// http://wiki.theory.org/BitTorrentSpecification This class accepts
  /// BEncoded strings in UTF-8, and outputs BEncoded strings in UTF-8.
  /// This class also demonstrates how CBORObject supports predefined
  /// serialization formats.</summary>
  public static class BEncoding {
    private static void WriteUtf8(string s, Stream stream) {
      if (DataUtilities.WriteUtf8(s, stream, false) != 0) {
        throw new CBORException("invalid surrogate");
      }
    }

    private static CBORObject ReadDictionary(Stream stream) {
      var obj = CBORObject.NewMap();
      while (true) {
        int c = stream.ReadByte();
        if (c == 'e') {
          break;
        }
        CBORObject s = ReadString(stream, (char)c);
        CBORObject o = ReadObject(stream, false);
        obj[s] = o;
      }
      return obj;
    }

    private static CBORObject ReadInteger(Stream stream) {
      var builder = new StringBuilder();
      var start = true;
      while (true) {
        int c = stream.ReadByte();
        if (c < 0) {
          throw new CBORException("Premature end of data");
        }
        if (c is >= '0' and <= '9') {
          _ = builder.Append((char)c);
          start = false;
        } else if (c == 'e') {
          break;
        } else if (start && c == '-') {
          start = false;
          _ = builder.Append((char)c);
        } else {
          throw new CBORException("Invalid integer encoding");
        }
      }
      string s = builder.ToString();
      return s.Length >= 2 && s[0] == '0' && s[1] == '0' ?
        throw new CBORException("Invalid integer encoding") :
        s.Length >= 3 && s[0] == '-' && s[1] == '0' && s[2] == '0' ?
        throw new CBORException("Invalid integer encoding") :
        CBORObject.FromEInteger(
          EInteger.FromString(s));
    }

    private static CBORObject ReadList(Stream stream) {
      var obj = CBORObject.NewArray();
      while (true) {
        CBORObject o = ReadObject(stream, true);
        if (o == null) {
          break; // 'e' was read
        }
        _ = obj.Add(o);
      }
      return obj;
    }

    public static CBORObject Read(Stream stream) {
      return stream == null ? throw new
ArgumentNullException(nameof(stream)) : ReadObject(stream, false);
    }

    private static CBORObject ReadObject(Stream stream, bool allowEnd) {
      int c = stream.ReadByte();
      return c switch {
        'd' => ReadDictionary(stream),
        'l' => ReadList(stream),
        'i' => ReadInteger(stream),
        'e' => allowEnd ? null : throw new CBORException("Object expected"),
        '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9'
=> ReadString(stream, (char)c),
        _ => throw new CBORException("Object expected"),
      };
    }

    private const string ValueDigits = "0123456789";

    public static string LongToString(long longValue) {
      return longValue == Int64.MinValue ?
        "-9223372036854775808" : longValue == 0L ?
        "0" : (longValue == Int32.MinValue) ? "-2147483648" :
EInteger.FromInt64(longValue).ToString();
    }

    private static CBORObject ReadString(Stream stream, char firstChar) {
      var builder = new StringBuilder();
      if (firstChar is < '0' or > '9') {
        throw new CBORException("Invalid integer encoding");
      }
      _ = builder.Append(firstChar);
      while (true) {
        int c = stream.ReadByte();
        if (c < 0) {
          throw new CBORException("Premature end of data");
        }
        if (c is >= '0' and <= '9') {
          _ = builder.Append((char)c);
        } else if (c == ':') {
          break;
        } else {
          throw new CBORException("Invalid integer encoding");
        }
      }
      string s = builder.ToString();
      if (s.Length >= 2 && s[0] == '0' && s[1] == '0') {
        throw new CBORException("Invalid integer encoding");
      }
      var numlength = EInteger.FromString(s);
      if (!numlength.CanFitInInt32()) {
        throw new CBORException("Length too long");
      }
      builder = new StringBuilder();
      return DataUtilities.ReadUtf8(
        stream,
        numlength.ToInt32Checked(),
        builder,
        false) switch {
        -2 => throw new CBORException("Premature end of data"),
        -1 => throw new CBORException("Invalid UTF-8"),
        _ => CBORObject.FromString(builder.ToString()),
      };
    }

    public static void Write(CBORObject obj, Stream stream) {
      if (obj == null) {
        throw new ArgumentNullException(nameof(obj));
      }
      if (obj.IsNumber) {
        if (stream == null) {
          throw new ArgumentNullException(nameof(stream));
        }
        stream.WriteByte(unchecked(0x69));
        WriteUtf8(obj.ToObject(typeof(EInteger)).ToString(), stream);
        stream.WriteByte(unchecked(0x65));
      } else if (obj.Type == CBORType.TextString) {
        string s = obj.AsString();
        long length = DataUtilities.GetUtf8Length(s, false);
        if (length < 0) {
          throw new CBORException("invalid string");
        }
        WriteUtf8(LongToString(length), stream);
        if (stream == null) {
          throw new ArgumentNullException(nameof(stream));
        }
        stream.WriteByte(unchecked((byte)':'));
        WriteUtf8(s, stream);
      } else if (obj.Type == CBORType.Map) {
        var hasNonStringKeys = false;
        foreach (CBORObject key in obj.Keys) {
          if (key.Type != CBORType.TextString) {
            hasNonStringKeys = true;
            break;
          }
        }
        if (hasNonStringKeys) {
          var valueSMap = new Dictionary<string, CBORObject>();
          // Copy to a map with String keys, since
          // some keys could be duplicates
          // when serialized to strings
          foreach (CBORObject key in obj.Keys) {
            CBORObject value = obj[key];
            string str = (key.Type == CBORType.TextString) ?
              key.AsString() : key.ToJSONString();
            valueSMap[str] = value;
          }
          if (stream == null) {
            throw new ArgumentNullException(nameof(stream));
          }
          stream.WriteByte(unchecked(0x64));
          foreach (KeyValuePair<string, CBORObject> entry in valueSMap) {
            string key = entry.Key;
            CBORObject value = entry.Value;
            long length = DataUtilities.GetUtf8Length(key, false);
            if (length < 0) {
              throw new CBORException("invalid string");
            }
            WriteUtf8(
              LongToString(length),
              stream);
            stream.WriteByte(unchecked((byte)':'));
            WriteUtf8(key, stream);
            Write(value, stream);
          }
          stream.WriteByte(unchecked(0x65));
        } else {
          if (stream == null) {
            throw new ArgumentNullException(nameof(stream));
          }
          stream.WriteByte(unchecked(0x64));
          foreach (CBORObject key in obj.Keys) {
            string str = key.AsString();
            long length = DataUtilities.GetUtf8Length(str, false);
            if (length < 0) {
              throw new CBORException("invalid string");
            }
            WriteUtf8(LongToString(length), stream);
            stream.WriteByte(unchecked((byte)':'));
            WriteUtf8(str, stream);
            Write(obj[key], stream);
          }
          stream.WriteByte(unchecked(0x65));
        }
      } else if (obj.Type == CBORType.Array) {
        if (stream == null) {
          throw new ArgumentNullException(nameof(stream));
        }
        stream.WriteByte(unchecked(0x6c));
        for (int i = 0; i < obj.Count; ++i) {
          Write(obj[i], stream);
        }
        stream.WriteByte(unchecked(0x65));
      } else {
        string str = obj.ToJSONString();
        long length = DataUtilities.GetUtf8Length(str, false);
        if (length < 0) {
          throw new CBORException("invalid string");
        }
        WriteUtf8(LongToString(length), stream);
        if (stream == null) {
          throw new ArgumentNullException(nameof(stream));
        }
        stream.WriteByte(unchecked((byte)':'));
        WriteUtf8(str, stream);
      }
    }
  }
}
