/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.IO;

namespace PeterO.Cbor {
  internal sealed class CharacterReader : ICharacterReader
  {
    private string str;
    private Stream stream;
    private int offset;

    private sealed class Utf16Reader : ICharacterReader {
      private readonly bool bigEndian;
      private readonly Stream stream;

      private int offset;
      private int savedC1;
      private int savedC2;

      public Utf16Reader(Stream stream, bool bigEndian) {
        this.stream = stream;
        this.bigEndian = bigEndian;
        this.savedC1 = -1;
      }

      public void Unget(int c1, int c2) {
        this.savedC1 = c1;
        this.savedC2 = c2;
      }

      public int NextChar() {
        try {
          int c1, c2;
          if (this.savedC1 >= 0) {
            c1 = this.savedC1;
            c2 = this.savedC2;
            this.savedC1 = -1;
          } else {
            c1 = this.stream.ReadByte();
            c2 = this.stream.ReadByte();
          }
          if (c1 < 0) {
            return -1;
          }
          if (c2 < 0) {
            throw CharacterReader.NewError("Invalid UTF-16", this.offset);
          }
          c1 = this.bigEndian ? ((c1 << 8) | c2) : ((c2 << 8) | c1);
          int surr = c1 & 0xfc00;
          if (surr == 0xd800) {
            surr = c1;
            c1 = this.stream.ReadByte();
            if (c1 < 0) {
              CharacterReader.NewError("Invalid UTF-16", this.offset);
            }
            c2 = this.stream.ReadByte();
            if (c2 < 0) {
              throw CharacterReader.NewError("Invalid UTF-16", this.offset);
            }
            c1 = this.bigEndian ? ((c1 << 8) | c2) : ((c2 << 8) | c1);
            if ((c1 & 0xfc00) == 0xdc00) {
              ++this.offset;
              return 0x10000 + ((surr - 0xd800) << 10) + (c1 - 0xdc00);
            }
            throw CharacterReader.NewError(
              "Unpaired surrogate code point",
              this.offset);
          }
          if (surr == 0xdc00) {
            throw CharacterReader.NewError(
"Unpaired surrogate code point",
this.offset);
          }
          ++this.offset;
          return c1;
        } catch (IOException ex) {
          throw new FormatException(
            "I/O error occurred (offset " + this.offset + ")",
            ex);
        }
      }
    }

    private sealed class Utf32Reader : ICharacterReader {
      private readonly bool bigEndian;
      private readonly Stream stream;

      private int offset;

      public Utf32Reader(Stream stream, bool bigEndian) {
        this.stream = stream;
        this.bigEndian = bigEndian;
      }

      public int NextChar() {
        try {
          int c1 = this.stream.ReadByte();
          if (c1 < 0) {
            return -1;
          }
          int c2 = this.stream.ReadByte();
          if (c2 < 0) {
            throw CharacterReader.NewError("Invalid UTF-32", this.offset);
          }
          int c3 = this.stream.ReadByte();
          if (c3 < 0) {
            throw CharacterReader.NewError("Invalid UTF-32", this.offset);
          }
          int c4 = this.stream.ReadByte();
          if (c4 < 0) {
            throw CharacterReader.NewError("Invalid UTF-32", this.offset);
          }
          c1 = this.bigEndian ? ((c1 << 24) | (c2 << 16) | (c3 << 8) | c4) :
            ((c4 << 24) | (c3 << 16) | (c2 << 8) | c1);
          int surr = c1 & 0xfffc00;
          if (c1 < 0 || c1 >= 0x110000 || (c1 & 0xfff800) == 0xd800) {
            throw CharacterReader.NewError("Invalid UTF-32", this.offset);
          }
          ++this.offset;
          return c1;
        } catch (IOException ex) {
          throw new FormatException(
            "I/O error occurred (offset " + this.offset + ")",
            ex);
        }
      }
    }

    private sealed class Utf8Reader : ICharacterReader {
      private readonly Stream stream;
      private int lastChar;
      private int offset;

      public Utf8Reader(Stream stream) {
        this.stream = stream;
        this.lastChar = -1;
      }

      public void Unget(int ch) {
        this.lastChar = ch;
      }

      public int NextChar() {
        int cp = 0;
        int bytesSeen = 0;
        int bytesNeeded = 0;
        int lower = 0;
        int upper = 0;
        try {
          while (true) {
            int b;
            if (this.lastChar != -1) {
              b = this.lastChar;
              this.lastChar = -1;
            } else {
              b = this.stream.ReadByte();
            }
            if (b < 0) {
              if (bytesNeeded != 0) {
                bytesNeeded = 0;
                throw CharacterReader.NewError("Invalid UTF-8", this.offset);
              }
              return -1;
            }
            if (bytesNeeded == 0) {
              if ((b & 0x7f) == b) {
                ++this.offset;
                return b;
              }
              if (b >= 0xc2 && b <= 0xdf) {
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
                throw CharacterReader.NewError("Invalid UTF-8", this.offset);
              }
              continue;
            }
            if (b < lower || b > upper) {
              cp = bytesNeeded = bytesSeen = 0;
              throw CharacterReader.NewError("Invalid UTF-8", this.offset);
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
          throw new FormatException(
            "I/O error occurred (offset " + this.offset + ")",
            ex);
        }
      }
    }

    public CharacterReader(string str) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      this.str = str;
    }

    public CharacterReader(string str, bool skipByteOrderMark) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      this.offset = (skipByteOrderMark && str.Length > 0 && str[0] ==
        0xfeff) ? 1 : 0;
      this.str = str;
    }

    public CharacterReader(Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      this.stream = stream;
    }

    private ICharacterReader reader;

    // Detects a Unicode encoding assuming
    // the first character read will be ASCII
    // unless a byte order mark appears
    private int DetectUnicodeEncoding() {
      try {
        int c1 = this.stream.ReadByte();
        if (c1 < 0) {
          return -1;
        }
        if (c1 == 0xff) {
          if (this.stream.ReadByte() == 0xfe) {
            // Little endian UTF-16 or UTF-32
            int c3 = this.stream.ReadByte();
            int c4 = this.stream.ReadByte();
            if (c3 == 0 && c4 == 0) {
              this.reader = new Utf32Reader(this.stream, false);
              return this.reader.NextChar();
            } else {
              var newReader = new Utf16Reader(this.stream, false);
              this.reader = newReader;
              newReader.Unget(c3, c4);
              return newReader.NextChar();
            }
          }
          throw NewError("Invalid Unicode stream", 0);
        }
        if (c1 == 0xfe) {
          if (this.stream.ReadByte() == 0xff) {
            // Big endian UTF-16 or UTF-32
            int c3 = this.stream.ReadByte();
            int c4 = this.stream.ReadByte();
            if (c3 == 0 && c4 == 0) {
              this.reader = new Utf32Reader(this.stream, true);
              return this.reader.NextChar();
            } else {
              var newReader = new Utf16Reader(this.stream, true);
              this.reader = newReader;
              newReader.Unget(c3, c4);
              return newReader.NextChar();
            }
          }
          throw NewError("Invalid Unicode stream", 0);
        }
        if (c1 == 0) {
          int c2 = this.stream.ReadByte();
          if (c2 < 0) {
            // 0 EOF
            this.reader = new Utf8Reader(this.stream);
            return 0;
          }
          if (c2 == 0) {
            // 0 0
            int c3 = this.stream.ReadByte();
            int c4 = this.stream.ReadByte();
            if (c3 == 0xfe && c4 == 0xff) {
              // 0 0 FE FF
              this.reader = new Utf32Reader(this.stream, true);
              return this.reader.NextChar();
            }
            if (c3 == 0 && c4 >= 0 && (c4 & 0x80) == 0) {
              // 0 0 0 ASCII
              this.reader = new Utf32Reader(this.stream, true);
              return c4;
            } else {
              var newReader = new Utf16Reader(this.stream, true);
              this.reader = newReader;
              newReader.Unget(c3, c4);
              return newReader.NextChar();
            }
          }
          if ((c2 & 0x80) == 0) {
            // UTF-16BE
            this.reader = new Utf16Reader(this.stream, true);
            return c2;
          } else {
            var utf8reader = new Utf8Reader(this.stream);
            this.reader = utf8reader;
            utf8reader.Unget(c2);
            return 0;
          }
        }
        if ((c1 & 0x80) == 0) {
          int c2 = this.stream.ReadByte();
          if (c2 < 0) {
            this.reader = new Utf8Reader(this.stream);
            return c1;
          }
          if (c2 == 0) {
            int c3 = this.stream.ReadByte();
            int c4 = this.stream.ReadByte();
            if (c3 == 0 && c4 == 0) {
              this.reader = new Utf32Reader(this.stream, false);
              return c1;
            } else {
              var newReader = new Utf16Reader(this.stream, false);
              this.reader = newReader;
              newReader.Unget(c3, c4);
              return c1;
            }
          } else {
            var utf8reader = new Utf8Reader(this.stream);
            this.reader = utf8reader;
            utf8reader.Unget(c2);
            return c1;
          }
        } else {
          // Default case: assume UTF-8
          var utf8reader = new Utf8Reader(this.stream);
          this.reader = utf8reader;
          utf8reader.Unget(c1);
          c1 = utf8reader.NextChar();
          if (c1 == 0xfeff) {
            // Skip BOM
            c1 = utf8reader.NextChar();
          }
          return c1;
        }
      } catch (IOException ex) {
        throw new FormatException(
          "I/O error occurred (offset " + this.offset + ")",
          ex);
      }
    }

    public static FormatException NewError(string str, int offset) {
      return new FormatException(str + " (offset " + offset + ")");
    }

    public FormatException NewError(string str) {
      return NewError(str, this.offset);
    }

    /// <summary>Reads the next character from a Unicode stream or a
    /// string.</summary>
    /// <returns>The next character, or -1 if the end of the string or stream was
    /// reached.</returns>
    public int NextChar() {
      if (this.reader != null) {
        return this.reader.NextChar();
      }
      if (this.stream != null) {
        return this.DetectUnicodeEncoding();
      } else {
        int c = (this.offset < this.str.Length) ? this.str[this.offset] : -1;
        if ((c & 0xfc00) == 0xd800 && this.offset + 1 < this.str.Length &&
                this.str[this.offset + 1] >= 0xdc00 && this.str[this.offset + 1]
                <= 0xdfff) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c - 0xd800) << 10) + (this.str[this.offset + 1] -
          0xdc00);
          ++this.offset;
        } else if ((c & 0xf800) == 0xd800) {
          // unpaired surrogate
          throw this.NewError("Unpaired surrogate code point");
        }
        ++this.offset;
        return c;
      }
    }
  }
}
