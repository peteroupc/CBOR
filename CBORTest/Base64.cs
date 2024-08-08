/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;

namespace Test {
  internal static class Base64 {
    private const string Base64URL =
      "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";

    private const string Base64Classic =
      "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

    public static void WriteBase64(
      StringOutput writer,
      byte[] data,
      int offset,
      int count,
      bool padding) {
      WriteBase64(writer, data, offset, count, true, padding);
    }

    public static void WriteBase64URL(
      StringOutput writer,
      byte[] data,
      int offset,
      int count,
      bool padding) {
      WriteBase64(writer, data, offset, count, false, padding);
    }

    private static void WriteBase64(
      StringOutput writer,
      byte[] data,
      int offset,
      int count,
      bool classic,
      bool padding) {
      if (writer == null) {
        throw new ArgumentNullException(nameof(writer));
      }
      if (offset < 0) {
        throw new ArgumentException("offset(" + offset + ") is less than " +
          "0 ");
      }
      if (offset > data.Length) {
        throw new ArgumentException("offset(" + offset + ") is more than " +
          data.Length);
      }
      if (count < 0) {
        throw new ArgumentException("count(" + count + ") is less than " +
          "0 ");
      }
      if (count > data.Length) {
        throw new ArgumentException("count(" + count + ") is more than " +
          data.Length);
      }
      if (data.Length - offset < count) {
        throw new ArgumentException("data's length minus " + offset + "(" +
          (data.Length - offset) + ") is less than " + count);
      }
      string alphabet = classic ? Base64Classic : Base64URL;
      int length = offset + count;
      var buffer = new byte[32];
      var bufferOffset = 0;
      int i;
      for (i = offset; i < (length - 2); i += 3) {
        if (bufferOffset >= buffer.Length) {
          writer.WriteAscii(buffer, 0, bufferOffset);
          bufferOffset = 0;
        }
        buffer[bufferOffset++] = (byte)alphabet[(data[i] >> 2) & 63];
        buffer[bufferOffset++] = (byte)alphabet[((data[i] & 3) << 4) +
            ((data[i + 1] >> 4) & 15)];
        buffer[bufferOffset++] = (byte)alphabet[((data[i + 1] & 15) << 2) +
((data[i +
                  2] >> 6) & 3)];
        buffer[bufferOffset++] = (byte)alphabet[data[i + 2] & 63];
      }
      int lenmod3 = count % 3;
      if (lenmod3 != 0) {
        if (bufferOffset >= buffer.Length) {
          writer.WriteAscii(buffer, 0, bufferOffset);
          bufferOffset = 0;
        }
        i = length - lenmod3;
        buffer[bufferOffset++] = (byte)alphabet[(data[i] >> 2) & 63];
        if (lenmod3 == 2) {
          buffer[bufferOffset++] = (byte)alphabet[((data[i] & 3) << 4) +
((data[i + 1] >>
                  4) & 15)];
          buffer[bufferOffset++] = (byte)alphabet[(data[i + 1] & 15) << 2];
          if (padding) {
            buffer[bufferOffset++] = (byte)'=';
          }
        } else {
          buffer[bufferOffset++] = (byte)alphabet[(data[i] & 3) << 4];
          if (padding) {
            buffer[bufferOffset++] = (byte)'=';
            buffer[bufferOffset++] = (byte)'=';
          }
        }
      }
      if (bufferOffset >= 0) {
        writer.WriteAscii(buffer, 0, bufferOffset);
      }
    }
  }
}
