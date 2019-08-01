/*
Written in 2013 by Peter Occil.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Collections.Generic;
using System.Globalization;
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
    private static void writeUtf8(string s, Stream stream) {
      if (DataUtilities.WriteUtf8(s, stream, false) != 0) {
        throw new CBORException("invalid surrogate");
      }
    }

    private static CBORObject readDictionary(Stream stream) {
      CBORObject obj = CBORObject.NewMap();
      while (true) {
        int c = stream.ReadByte();
        if (c == 'e') {
          break;
        }
        CBORObject s = readString(stream, (char)c);
        CBORObject o = readObject(stream, false);
        obj[s] = o;
      }
      return obj;
    }

    private static CBORObject readInteger(Stream stream) {
      var builder = new StringBuilder();
      var start = true;
      while (true) {
        int c = stream.ReadByte();
        if (c < 0) {
          throw new CBORException("Premature end of data");
        }
        if (c >= (int)'0' && c <= (int)'9') {
          builder.Append((char)c);
          start = false;
        } else if (c == (int)'e') {
          break;
        } else if (start && c == '-') {
          start = false;
          builder.Append((char)c);
        } else {
          throw new CBORException("Invalid integer encoding");
        }
      }
      return CBORDataUtilities.ParseJSONNumber(
        builder.ToString(),
        true,
        false);
    }

    private static CBORObject readList(Stream stream) {
      CBORObject obj = CBORObject.NewArray();
      while (true) {
        CBORObject o = readObject(stream, true);
        if (o == null) {
          break; // 'e' was read
        }
        obj.Add(o);
      }
      return obj;
    }

    public static CBORObject Read(Stream stream) {
      return readObject(stream, false);
    }

    private static CBORObject readObject(Stream stream, bool allowEnd) {
      int c = stream.ReadByte();
      if (c == 'd') {
        return readDictionary(stream);
      }
      if (c == 'l') {
        return readList(stream);
      }
      if (allowEnd && c == 'e') {
        return null;
      }
      if (c == 'i') {
        return readInteger(stream);
      }
      if (c >= '0' && c <= '9') {
        return readString(stream, (char)c);
      }
      throw new CBORException("Object expected");
    }

    private static readonly string ValueDigits = "0123456789";

    public static string LongToString(long longValue) {
      if (longValue == Int64.MinValue) {
        return "-9223372036854775808";
      }
      if (longValue == 0L) {
        return "0";
      }
      if (longValue == (long)Int32.MinValue) {
        return "-2147483648";
      }
      bool neg = longValue < 0;
      var count = 0;
      char[] chars;
      int intlongValue = unchecked((int)longValue);
      if ((long)intlongValue == longValue) {
        chars = new char[12];
        count = 11;
        if (neg) {
          intlongValue = -intlongValue;
        }
        while (intlongValue > 43698) {
          int intdivValue = intlongValue / 10;
        char digit = ValueDigits[(int)(intlongValue - (intdivValue * 10))];
        chars[count--] = digit;
        intlongValue = intdivValue;
      }
      while (intlongValue > 9) {
        int intdivValue = (intlongValue * 26215) >> 18;
        char digit = ValueDigits[(int)(intlongValue - (intdivValue * 10))];
        chars[count--] = digit;
        intlongValue = intdivValue;
      }
      if (intlongValue != 0) {
        chars[count--] = ValueDigits[(int)intlongValue];
      }
      if (neg) {
        chars[count] = '-';
      } else {
        ++count;
      }
      return new String(chars, count, 12 - count);
    } else {
        chars = new char[24];
        count = 23;
        if (neg) {
          longValue = -longValue;
        }
        while (longValue > 43698) {
          long divValue = longValue / 10;
        char digit = ValueDigits[(int)(longValue - (divValue * 10))];
        chars[count--] = digit;
        longValue = divValue;
      }
      while (longValue > 9) {
        long divValue = (longValue * 26215) >> 18;
        char digit = ValueDigits[(int)(longValue - (divValue * 10))];
        chars[count--] = digit;
        longValue = divValue;
      }
      if (longValue != 0) {
        chars[count--] = ValueDigits[(int)longValue];
      }
      if (neg) {
        chars[count] = '-';
      } else {
        ++count;
      }
      return new String(chars, count, 24 - count);
      }
    }

    private static CBORObject readString(Stream stream, char firstChar) {
      var builder = new StringBuilder();
      if (firstChar < (int)'0' && firstChar > (int)'9') {
        throw new CBORException("Invalid integer encoding");
      }
      builder.Append(firstChar);
      while (true) {
        int c = stream.ReadByte();
        if (c < 0) {
          throw new CBORException("Premature end of data");
        }
        if (c >= (int)'0' && c <= (int)'9') {
          builder.Append((char)c);
        } else if (c == (int)':') {
          break;
        } else {
          throw new CBORException("Invalid integer encoding");
        }
      }
      CBORObject number = CBORDataUtilities.ParseJSONNumber(
        builder.ToString(),
        true,
        true);
      var length = 0;
      try {
        length = number.AsInt32();
      } catch (ArithmeticException ex) {
        throw new CBORException("Length too long", ex);
      }
      builder = new StringBuilder();
      switch (DataUtilities.ReadUtf8(stream, length, builder, false)) {
        case -2:
          throw new CBORException("Premature end of data");
        case -1:
          throw new CBORException("Invalid UTF-8");
      }
      return CBORObject.FromObject(builder.ToString());
    }

    public static void Write(CBORObject obj, Stream stream) {
      if (obj.Type == CBORType.Number) {
        stream.WriteByte(unchecked((byte)((byte)0x69)));
        writeUtf8(obj.AsEInteger().ToString(), stream);
        stream.WriteByte(unchecked((byte)((byte)0x65)));
      } else if (obj.Type == CBORType.TextString) {
        string s = obj.AsString();
        long length = DataUtilities.GetUtf8Length(s, false);
        if (length < 0) {
          throw new CBORException("invalid string");
        }
        writeUtf8(LongToString(length), stream);
        stream.WriteByte(unchecked((byte)((byte)':')));
        writeUtf8(s, stream);
      } else if (obj.Type == CBORType.Map) {
        var hasNonStringKeys = false;
        foreach (CBORObject key in obj.Keys) {
          if (key.Type != CBORType.TextString) {
            hasNonStringKeys = true;
            break;
          }
        }
        if (hasNonStringKeys) {
          var valueSMap = new Dictionary<String, CBORObject>();
          // Copy to a map with String keys, since
          // some keys could be duplicates
          // when serialized to strings
          foreach (CBORObject key in obj.Keys) {
            CBORObject value = obj[key];
            string str = (key.Type == CBORType.TextString) ?
              key.AsString() : key.ToJSONString();
            valueSMap[str] = value;
          }
          stream.WriteByte(unchecked((byte)((byte)0x64)));
          foreach (KeyValuePair<string, CBORObject> entry in valueSMap) {
            string key = entry.Key;
            CBORObject value = entry.Value;
            long length = DataUtilities.GetUtf8Length(key, false);
            if (length < 0) {
              throw new CBORException("invalid string");
            }
            writeUtf8(
  LongToString(length),
  stream);
            stream.WriteByte(unchecked((byte)((byte)':')));
            writeUtf8(key, stream);
            Write(value, stream);
          }
          stream.WriteByte(unchecked((byte)((byte)0x65)));
        } else {
          stream.WriteByte(unchecked((byte)((byte)0x64)));
          foreach (CBORObject key in obj.Keys) {
            string str = key.AsString();
            long length = DataUtilities.GetUtf8Length(str, false);
            if (length < 0) {
              throw new CBORException("invalid string");
            }
            writeUtf8(LongToString(length), stream);
            stream.WriteByte(unchecked((byte)((byte)':')));
            writeUtf8(str, stream);
            Write(obj[key], stream);
          }
          stream.WriteByte(unchecked((byte)((byte)0x65)));
        }
      } else if (obj.Type == CBORType.Array) {
        stream.WriteByte(unchecked((byte)((byte)0x6C)));
        for (var i = 0; i < obj.Count; ++i) {
          Write(obj[i], stream);
        }
        stream.WriteByte(unchecked((byte)((byte)0x65)));
      } else {
        string str = obj.ToJSONString();
        long length = DataUtilities.GetUtf8Length(str, false);
        if (length < 0) {
          throw new CBORException("invalid string");
        }
        writeUtf8(LongToString(length), stream);
        stream.WriteByte(unchecked((byte)((byte)':')));
        writeUtf8(str, stream);
      }
    }
  }
}
