/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.IO;
using System.Text;

namespace PeterO.Cbor {
  internal sealed class StringOutput {
    private readonly StringBuilder builder;
    private readonly Stream outputStream;

    public StringOutput(StringBuilder builder) {
      this.builder = builder;
      this.outputStream = null;
    }

    public StringOutput(Stream outputStream) {
      this.outputStream = outputStream;
      this.builder = null;
    }

    public void WriteString(string str) {
      if (this.outputStream != null) {
        if (str.Length == 1) {
          this.WriteCodePoint(str[0]);
        } else {
          if (DataUtilities.WriteUtf8(
            str,
            0,
            str.Length,
            this.outputStream,
            false) < 0) {
            throw new ArgumentException("str has an unpaired surrogate");
          }
        }
      } else {
        this.builder.Append(str);
      }
    }

    public void WriteString(string str, int index, int length) {
      if (this.outputStream == null) {
        this.builder.Append(str, index, length);
      } else {
        if (length == 1) {
          this.WriteCodePoint(str[index]);
        } else {
          if (
            DataUtilities.WriteUtf8(
              str,
              index,
              length,
              this.outputStream,
              false) < 0) {
            throw new ArgumentException("str has an unpaired surrogate");
          }
        }
      }
    }

    public void WriteAscii(byte[] bytes, int index, int length) {
      if (bytes == null) {
        throw new ArgumentNullException(nameof(bytes));
      }
      if (index < 0) {
        throw new ArgumentException("\"index\" (" + index + ") is not" +
"\u0020greater or equal to 0");
      }
      if (index > bytes.Length) {
        throw new ArgumentException("\"index\" (" + index + ") is not less" +
"\u0020or equal to " + bytes.Length);
      }
      if (length < 0) {
        throw new ArgumentException(" (" + length + ") is not greater or" +
"\u0020equal to 0");
      }
      if (length > bytes.Length) {
        throw new ArgumentException(" (" + length + ") is not less or equal" +
"\u0020to " + bytes.Length);
      }
      if (bytes.Length - index < length) {
        throw new ArgumentException("\"bytes\" + \"'s length minus \" +" +
"\u0020index (" + (bytes.Length - index) + ") is not greater or equal to " +
length);
      }
      if (this.outputStream == null) {
        _ = DataUtilities.ReadUtf8FromBytes(
          bytes,
          index,
          length,
          this.builder,
          false);
      } else {
        for (int i = 0; i < length; ++i) {
          byte b = bytes[i + index];
          if ((b & 0x7f) != b) {
            throw new ArgumentException("str is non-ASCII");
          }
        }
        this.outputStream.Write(bytes, index, length);
      }
    }

    public void WriteCodePoint(int codePoint) {
      if ((codePoint >> 7) == 0) {
        // Code point is in the Basic Latin range (U+0000 to U+007F)
        if (this.outputStream == null) {
          this.builder.Append((char)codePoint);
        } else {
          this.outputStream.WriteByte((byte)codePoint);
        }
        return;
      }
      if (codePoint < 0) {
        throw new ArgumentException("codePoint(" + codePoint +
          ") is less than 0");
      }
      if (codePoint > 0x10ffff) {
        throw new ArgumentException("codePoint(" + codePoint +
          ") is more than " + 0x10ffff);
      }
      if (this.outputStream != null) {
        if (codePoint < 0x80) {
          this.outputStream.WriteByte((byte)codePoint);
        } else if (codePoint <= 0x7ff) {
          this.outputStream.WriteByte((byte)(0xc0 | ((codePoint >> 6) &
                0x1f)));
          this.outputStream.WriteByte((byte)(0x80 | (codePoint & 0x3f)));
        } else if (codePoint <= 0xffff) {
          if ((codePoint & 0xf800) == 0xd800) {
            throw new ArgumentException("ch is a surrogate");
          }
          this.outputStream.WriteByte((byte)(0xe0 | ((codePoint >> 12) &
                0x0f)));
          this.outputStream.WriteByte((byte)(0x80 | ((codePoint >> 6) &
                0x3f)));
          this.outputStream.WriteByte((byte)(0x80 | (codePoint & 0x3f)));
        } else {
          this.outputStream.WriteByte((byte)(0xf0 | ((codePoint >> 18) &
                0x07)));
          this.outputStream.WriteByte((byte)(0x80 | ((codePoint >> 12) &
                0x3f)));
          this.outputStream.WriteByte((byte)(0x80 | ((codePoint >> 6) &
                0x3f)));
          this.outputStream.WriteByte((byte)(0x80 | (codePoint & 0x3f)));
        }
      } else {
        if ((codePoint & 0xfff800) == 0xd800) {
          throw new ArgumentException("ch is a surrogate");
        }
        if (codePoint <= 0xffff) {
          {
            this.builder.Append((char)codePoint);
          }
        } else if (codePoint <= 0x10ffff) {
          this.builder.Append((char)((((codePoint - 0x10000) >> 10) &
0x3ff) |
              0xd800));
          this.builder.Append((char)(((codePoint - 0x10000) & 0x3ff) |
              0xdc00));
        }
      }
    }
  }
}
