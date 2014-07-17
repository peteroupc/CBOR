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
      this.outputStream = null;
    }

    public Utf8Writer (OutputStream outputStream) {
      this.outputStream = outputStream;
      this.builder = null;
    }

    public void WriteString(String str) throws IOException {
      if (this.outputStream != null) {
        if (str.length() == 1) {
          this.WriteChar(str.charAt(0));
        } else {
          if (DataUtilities.WriteUtf8(
            str,
            0,
            str.length(),
            this.outputStream,
            false) < 0) {
            throw new IllegalArgumentException("str has an unpaired surrogate");
          }
        }
      } else {
        this.builder.append(str);
      }
    }

    public void WriteString(String str, int index, int length) throws IOException {
      if (this.outputStream != null) {
        if (length == 1) {
          this.WriteChar(str.charAt(index));
        } else {
          if (DataUtilities.WriteUtf8(
            str,
            index,
            length,
            this.outputStream,
            false) < 0) {
            throw new IllegalArgumentException("str has an unpaired surrogate");
          }
        }
      } else {
        this.builder.append(str, index, (index) + (length));
      }
    }

    public void WriteCodePoint(int codePoint) throws IOException {
      if (codePoint < 0) {
        throw new IllegalArgumentException("codePoint (" + codePoint +
                                    ") is less than " +
                                    0);
      }
      if (codePoint > 0x10ffff) {
        throw new IllegalArgumentException("codePoint (" + codePoint +
                                    ") is more than " +
                                    0x10ffff);
      }
      if (this.outputStream != null) {
        if (codePoint < 0x80) {
          this.outputStream.write((byte)codePoint);
        } else if (codePoint <= 0x7ff) {
          this.outputStream.write((byte)(0xc0 | ((codePoint >> 6) & 0x1f)));
          this.outputStream.write((byte)(0x80 | (codePoint & 0x3f)));
        } else if (codePoint <= 0xffff) {
          if ((codePoint & 0xf800) == 0xd800) {
            throw new IllegalArgumentException("ch is a surrogate");
          }
          this.outputStream.write((byte)(0xe0 | ((codePoint >> 12) &
                                                     0x0f)));
          this.outputStream.write((byte)(0x80 | ((codePoint >> 6) & 0x3f)));
          this.outputStream.write((byte)(0x80 | (codePoint & 0x3f)));
        } else {
          this.outputStream.write((byte)(0xf0 | ((codePoint >> 18) &
                                                     0x08)));
          this.outputStream.write((byte)(0x80 | ((codePoint >> 12) &
                                                     0x3f)));
          this.outputStream.write((byte)(0x80 | ((codePoint >> 6) & 0x3f)));
          this.outputStream.write((byte)(0x80 | (codePoint & 0x3f)));
        }
      } else {
        if ((codePoint & 0xfff800) == 0xd800) {
          throw new IllegalArgumentException("ch is a surrogate");
        }
        if (codePoint <= 0xffff) {
          { this.builder.append((char)codePoint);
          }
        } else if (codePoint <= 0x10ffff) {
          this.builder.append((char)((((codePoint - 0x10000) >> 10) &
                                      0x3ff) +0xd800));
          this.builder.append((char)(((codePoint - 0x10000) & 0x3ff) +0xdc00));
        }
      }
    }

    public void WriteChar(char ch) throws IOException {
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
