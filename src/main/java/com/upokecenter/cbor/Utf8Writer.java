package com.upokecenter.cbor;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.io.*;

import com.upokecenter.util.*;

  final class Utf8Writer {

    private final StringBuilder builder;
    private final OutputStream outputStream;

    public Utf8Writer (StringBuilder builder) {
      this.builder = builder;
    }

    public Utf8Writer (OutputStream outputStream) {
      this.outputStream = outputStream;
    }

    public void WriteString(String str) {
      if (this.outputStream != null) {
        if (str.length() == 1) {
          this.WriteChar(str.charAt(0));
        } else {
     if (DataUtilities.WriteUtf8(str, 0, str.length(), this.outputStream, false) <
            0) {
            throw new IllegalArgumentException("str has an unpaired surrogate");
          }
        }
      } else {
        this.builder.append(str);
      }
    }

    public void WriteString(String str, int index, int length) {
      if (this.outputStream != null) {
        if (length == 1) {
          this.WriteChar(str.charAt(index));
        } else {
     if (DataUtilities.WriteUtf8(str, index, length, this.outputStream, false) <
            0) {
            throw new IllegalArgumentException("str has an unpaired surrogate");
          }
        }
      } else {
        this.builder.append(str);
      }
    }

    public void WriteChar(char ch) {
      if (this.outputStream != null) {
        if (ch < 0x80) {
          this.outputStream.write((byte)ch);
        } else if (ch <= 0x7ff) {
          this.outputStream.write((byte)(0xc0 | ((ch >> 6) & 0x1f)));
          this.outputStream.write((byte)(0x80 | (ch & 0x3f)));
        } else {
          if ((ch & 0xf800) == 0xd800) {
            throw new IllegalArgumentException("ch is a surrogate");
          }
          this.outputStream.write((byte)(0xe0 | ((ch >> 12) & 0x0f)));
          this.outputStream.write((byte)(0x80 | ((ch >> 6) & 0x3f)));
          this.outputStream.write((byte)(0x80 | (ch & 0x3f)));
        }
      } else {
        if ((ch & 0xf800) == 0xd800) {
          throw new IllegalArgumentException("ch is a surrogate");
        }
        this.builder.append(ch);
      }
    }
  }
