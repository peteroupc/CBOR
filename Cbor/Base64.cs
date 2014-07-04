/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.IO;
using System.Text;

namespace PeterO.Cbor {
  internal static class Base64
  {
    private const string Base64URL =
      "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_ ";

    private const string Base64Classic =
      "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/ ";

    public static void ToBase64(StringBuilder str, byte[] data, bool padding) {
      if (data == null) {
        throw new ArgumentNullException("data");
      }
      ToBase64(str, data, 0, data.Length, Base64Classic, padding);
    }

    public static void ToBase64URL(
      StringBuilder str,
      byte[] data,
      bool padding) {
      if (data == null) {
        throw new ArgumentNullException("data");
      }
      ToBase64(str, data, 0, data.Length, Base64URL, padding);
    }

    public static void ToBase64(
      StringBuilder str,
      byte[] data,
      int offset,
      int count,
      bool padding) {
      ToBase64(str, data, offset, count, Base64Classic, padding);
    }

    public static void ToBase64URL(
      StringBuilder str,
      byte[] data,
      int offset,
      int count,
      bool padding) {
      ToBase64(str, data, offset, count, Base64URL, padding);
    }

    public static void WriteBase64(
      Stream outputStream,
      byte[] data,
      int offset,
      int count,
      bool padding) {
      WriteBase64(outputStream, data, offset, count, Base64Classic, padding);
    }

    public static void WriteBase64URL(
      Stream outputStream,
      byte[] data,
      int offset,
      int count,
      bool padding) {
      WriteBase64(outputStream, data, offset, count, Base64URL, padding);
    }

    public static string ToBase64String(byte[] data, bool padding) {
      if (data == null) {
        throw new ArgumentNullException("data");
      }
      return ToBase64String(data, 0, data.Length, padding);
    }

    public static string ToBase64URLString(byte[] data, bool padding) {
      if (data == null) {
        throw new ArgumentNullException("data");
      }
      return ToBase64String(data, 0, data.Length, padding);
    }

    public static string ToBase64String(
      byte[] data,
      int offset,
      int count,
      bool padding) {
      var builder = new StringBuilder();
      ToBase64(builder, data, offset, count, Base64Classic, padding);
      return builder.ToString();
    }

    public static string ToBase64URLString(
      byte[] data,
      int offset,
      int count,
      bool padding) {
      var builder = new StringBuilder();
      ToBase64(builder, data, offset, count, Base64Classic, padding);
      return builder.ToString();
    }

    private static void ToBase64(
      StringBuilder str,
      byte[] data,
      int offset,
      int count,
      string alphabet,
      bool padding) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (data == null) {
        throw new ArgumentNullException("data");
      }
      if (offset < 0) {
        throw new ArgumentException(
          "offset (" + offset + ") is less than " + "0 ");
      }
      if (offset > data.Length) {
        throw new ArgumentException(
          "offset (" + offset + ") is more than " + data.Length);
      }
      if (count < 0) {
     throw new ArgumentException("count (" + count + ") is less than " +
          "0 ");
      }
      if (count > data.Length) {
        throw new ArgumentException(
          "count (" + count + ") is more than " + data.Length);
      }
      if (data.Length - offset < count) {
        throw new ArgumentException("data's length minus " + offset + " (" +
          (data.Length - offset) +
                                    ") is less than " + count);
      }
      int length = offset + count;
      int i = offset;
      for (i = offset; i < (length - 2); i += 3) {
        str.Append(alphabet[(data[i] >> 2) & 63]);
        str.Append(alphabet[((data[i] & 3) << 4) + ((data[i + 1] >> 4) & 15)]);
        str.Append(alphabet[((data[i + 1] & 15) << 2) + ((data[i + 2] >> 6) &
                                                         3)]);
        str.Append(alphabet[data[i + 2] & 63]);
      }
      int lenmod3 = count % 3;
      if (lenmod3 != 0) {
        i = length - lenmod3;
        str.Append(alphabet[(data[i] >> 2) & 63]);
        if (lenmod3 == 2) {
          str.Append(alphabet[((data[i] & 3) << 4) + ((data[i + 1] >> 4) &
                                                      15)]);
          str.Append(alphabet[(data[i + 1] & 15) << 2]);
          if (padding) {
            str.Append("=");
          }
        } else {
          str.Append(alphabet[(data[i] & 3) << 4]);
          if (padding) {
            str.Append("==");
          }
        }
      }
    }

    private static void WriteBase64(
      Stream outputStream,
      byte[] data,
      int offset,
      int count,
      string alphabet,
      bool padding) {
      if (outputStream == null) {
        throw new ArgumentNullException("outputStream");
      }
      if (offset < 0) {
   throw new ArgumentException("offset (" + offset + ") is less than " +
          "0 ");
      }
      if (offset > data.Length) {
        throw new ArgumentException("offset (" + offset + ") is more than " +
          data.Length);
      }
      if (count < 0) {
     throw new ArgumentException("count (" + count + ") is less than " +
          "0 ");
      }
      if (count > data.Length) {
        throw new ArgumentException("count (" + count + ") is more than " +
          data.Length);
      }
      if (data.Length - offset < count) {
        throw new ArgumentException("data's length minus " + offset + " (" +
          (data.Length - offset) +
                                    ") is less than " + count);
      }
      int length = offset + count;
      int i = offset;
      var buffer = new byte[4];
      for (i = offset; i < (length - 2); i += 3) {
        buffer[0] = (byte)alphabet[(data[i] >> 2) & 63];
        buffer[1] = (byte)alphabet[((data[i] & 3) << 4) +
                                   ((data[i + 1] >> 4) &
                                    15)];
        buffer[2] = (byte)alphabet[((data[i + 1] & 15) << 2) + ((data[i +
                                                                      2] >> 6) &
                                                                3)];
        buffer[3] = (byte)alphabet[data[i + 2] & 63];
        outputStream.Write(buffer, 0, 4);
      }
      int lenmod3 = count % 3;
      if (lenmod3 != 0) {
        i = length - lenmod3;
        buffer[0] = (byte)alphabet[(data[i] >> 2) & 63];
        if (lenmod3 == 2) {
          buffer[1] = (byte)alphabet[((data[i] & 3) << 4) + ((data[i + 1] >>
                                                              4) &
                                                             15)];
          buffer[2] = (byte)alphabet[(data[i + 1] & 15) << 2];
          if (padding) {
            buffer[3] = (byte)'=';
            outputStream.Write(buffer, 0, 4);
          } else {
            outputStream.Write(buffer, 0, 3);
          }
        } else {
          buffer[1] = (byte)alphabet[(data[i] & 3) << 4];
          if (padding) {
            buffer[2] = (byte)'=';
            buffer[3] = (byte)'=';
            outputStream.Write(buffer, 0, 4);
          } else {
            outputStream.Write(buffer, 0, 2);
          }
        }
      }
    }
  }
}
