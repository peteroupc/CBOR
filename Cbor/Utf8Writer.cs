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
using PeterO;

namespace PeterO.Cbor {
  internal sealed class Utf8Writer
  {
    private readonly StringBuilder builder;
    private readonly Stream outputStream;

    public Utf8Writer(StringBuilder builder) {
      this.builder = builder;
    }

    public Utf8Writer(Stream outputStream) {
      this.outputStream = outputStream;
    }

    public void WriteString(string str) {
      if (this.outputStream != null) {
        if (str.Length == 1) {
          this.WriteChar(str[0]);
        } else {
     if (DataUtilities.WriteUtf8(str, 0, str.Length, this.outputStream, false) <
            0) {
            throw new ArgumentException("str has an unpaired surrogate");
          }
        }
      } else {
        this.builder.Append(str);
      }
    }

    public void WriteString(string str, int index, int length) {
      if (this.outputStream != null) {
        if (length == 1) {
          this.WriteChar(str[index]);
        } else {
     if (DataUtilities.WriteUtf8(str, index, length, this.outputStream, false) <
            0) {
            throw new ArgumentException("str has an unpaired surrogate");
          }
        }
      } else {
        this.builder.Append(str);
      }
    }

    public void WriteChar(char ch) {
      if (this.outputStream != null) {
        if (ch < 0x80) {
          this.outputStream.WriteByte((byte)ch);
        } else if (ch <= 0x7ff) {
          this.outputStream.WriteByte((byte)(0xc0 | ((ch >> 6) & 0x1f)));
          this.outputStream.WriteByte((byte)(0x80 | (ch & 0x3f)));
        } else {
          if ((ch & 0xf800) == 0xd800) {
            throw new ArgumentException("ch is a surrogate");
          }
          this.outputStream.WriteByte((byte)(0xe0 | ((ch >> 12) & 0x0f)));
          this.outputStream.WriteByte((byte)(0x80 | ((ch >> 6) & 0x3f)));
          this.outputStream.WriteByte((byte)(0x80 | (ch & 0x3f)));
        }
      } else {
        if ((ch & 0xf800) == 0xd800) {
          throw new ArgumentException("ch is a surrogate");
        }
        this.builder.Append(ch);
      }
    }
  }
}
