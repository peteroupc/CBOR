/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;
using System.IO;

namespace PeterO.Cbor
{
    /// <summary>Description of CharacterReader.</summary>
  internal class CharacterReader
  {
    private string str;
    private Stream stream;
    private int offset;

    public CharacterReader(string str) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      this.str = str;
    }

    public CharacterReader(Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      this.stream = stream;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <returns>A CBORException object.</returns>
    public CBORException NewError(string str) {
      return new CBORException(str + " (offset " + this.offset + ")");
    }

    /// <summary>Reads the next character from a UTF-8 stream or a string.</summary>
    /// <returns>The next character, or -1 if the end of the string or stream
    /// was reached.</returns>
    public int NextChar() {
      if (this.stream != null) {
        int cp = 0;
        int bytesSeen = 0;
        int bytesNeeded = 0;
        int lower = 0;
        int upper = 0;
        try {
          while (true) {
            int b = this.stream.ReadByte();
            if (b < 0) {
              if (bytesNeeded != 0) {
                bytesNeeded = 0;
                throw this.NewError("Invalid UTF-8");
              } else {
                return -1;
              }
            }
            if (bytesNeeded == 0) {
              if ((b & 0x7f) == b) {
                ++this.offset;
                return b;
              } else if (b >= 0xc2 && b <= 0xdf) {
                bytesNeeded = 1;
                lower = 0x80;
                upper = 0xbf;
                cp = (b - 0xc0) << 6;
              } else if (b >= 0xe0 && b <= 0xef) {
                lower = (b == 0xe0) ? 0xa0 : 0x80;
                upper = (b == 0xed) ? 0x9f : 0xbf;
                bytesNeeded = 2;
                cp = (b - 0xe0) << 12;
              } else if (b >= 0xf0 && b <= 0xf4) {
                lower = (b == 0xf0) ? 0x90 : 0x80;
                upper = (b == 0xf4) ? 0x8f : 0xbf;
                bytesNeeded = 3;
                cp = (b - 0xf0) << 18;
              } else {
                throw this.NewError("Invalid UTF-8");
              }
              continue;
            }
            if (b < lower || b > upper) {
              cp = bytesNeeded = bytesSeen = 0;
              throw this.NewError("Invalid UTF-8");
            }
            lower = 0x80;
            upper = 0xbf;
            ++bytesSeen;
            cp += (b - 0x80) << (6 * (bytesNeeded - bytesSeen));
            if (bytesSeen != bytesNeeded) {
              continue;
            }
            int ret = cp;
            cp = 0;
            bytesSeen = 0;
            bytesNeeded = 0;
            ++this.offset;
            return ret;
          }
        } catch (IOException ex) {
          throw new CBORException("I/O error occurred (offset " + this.offset + ")", ex);
        }
      } else {
        int c = (this.offset < this.str.Length) ? this.str[this.offset] : -1;
        if (c >= 0xd800 && c <= 0xdbff && this.offset + 1 < this.str.Length &&
            this.str[this.offset + 1] >= 0xdc00 && this.str[this.offset + 1] <= 0xdfff) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c - 0xd800) * 0x400) + (this.str[this.offset + 1] - 0xdc00);
          ++this.offset;
        } else if (c >= 0xd800 && c <= 0xdfff) {
          // unpaired surrogate
          throw this.NewError("Unpaired surrogate code point");
        }
        ++this.offset;
        return c;
      }
    }
  }
}
