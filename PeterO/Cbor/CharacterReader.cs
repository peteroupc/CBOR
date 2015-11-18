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
  internal sealed class CharacterReader : ICharacterInput {
    private interface IByteReader {
      int ReadByte();
    }

    private sealed class WrappedStream : IByteReader {
      private Stream stream;

      public WrappedStream(Stream stream) {
        this.stream = stream;
      }

      public int ReadByte() {
        try {
          return this.stream.ReadByte();
        } catch (IOException ex) {
          throw new InvalidOperationException(ex.Message, ex);
        }
      }
    }

    private string str;
    private IByteReader stream;
    private int mode;
    private int offset;

    private sealed class SavedState {
      private int[] saved;
      private int savedOffset;
      private int savedLength;

      private void Ensure(int size) {
        this.saved = this.saved ?? (new int[this.savedLength + size]);
        if (this.savedOffset + size < this.saved.Length) {
          var newsaved = new int[this.savedOffset + size + 4];
          Array.Copy(this.saved, 0, newsaved, 0, this.savedLength);
          this.saved = newsaved;
        }
      }

      public void AddOne(int a) {
        this.Ensure(1);
        this.saved[this.savedLength++] = a;
      }

      public void AddTwo(int a, int b) {
        this.Ensure(2);
        this.saved[this.savedLength++] = a;
        this.saved[this.savedLength++] = b;
      }

      public void AddThree(int a, int b, int c) {
        this.Ensure(4);
        this.saved[this.savedLength++] = a;
        this.saved[this.savedLength++] = b;
        this.saved[this.savedLength++] = c;
      }

      public int Read(IByteReader input) {
        if (this.savedOffset < this.savedLength) {
          int ret = this.saved[this.savedOffset++];
          if (this.savedOffset >= this.savedLength) {
            this.savedOffset = this.savedLength = 0;
          }
          return ret;
        }
        return input.ReadByte();
      }
    }

    private sealed class Utf16Reader : ICharacterInput {
      private readonly bool bigEndian;
      private readonly IByteReader stream;
      private SavedState state;

      public Utf16Reader(IByteReader stream, bool bigEndian) {
        this.stream = stream;
        this.bigEndian = bigEndian;
        this.state = new SavedState();
      }

      public void Unget(int c1, int c2) {
        this.state.AddTwo(c1, c2);
      }

      public int ReadChar() {
        int c1 = this.state.Read(this.stream);
        int c2 = this.state.Read(this.stream);
        if (c1 < 0) {
          return -1;
        }
        if (c2 < 0) {
          throw new InvalidOperationException("Invalid UTF-16");
        }
        c1 = this.bigEndian ? ((c1 << 8) | c2) : ((c2 << 8) | c1);
        int surr = c1 & 0xfc00;
        if (surr == 0xd800) {
          surr = c1;
          c1 = this.state.Read(this.stream);
          if (c1 < 0) {
            new InvalidOperationException("Invalid UTF-16");
          }
          c2 = this.state.Read(this.stream);
          if (c2 < 0) {
            throw new InvalidOperationException("Invalid UTF-16");
          }
          c1 = this.bigEndian ? ((c1 << 8) | c2) : ((c2 << 8) | c1);
          if ((c1 & 0xfc00) == 0xdc00) {
            return 0x10000 + ((surr - 0xd800) << 10) + (c1 - 0xdc00);
          }
          throw new InvalidOperationException(
            "Unpaired surrogate code point");
        }
        if (surr == 0xdc00) {
          throw new InvalidOperationException(
"Unpaired surrogate code point");
        }

        return c1;
      }

      public int Read(int[] chars, int index, int length) {
        int count = 0;
        for (int i = 0; i < length; ++i) {
          int c = this.ReadChar();
          if (c < 0) {
 return count;
}
          chars[index + i] = c;
        }
        return count;
      }
    }

    private sealed class Utf32Reader : ICharacterInput {
      private readonly bool bigEndian;
      private readonly IByteReader stream;

      private SavedState state;

      public Utf32Reader(IByteReader stream, bool bigEndian) {
        this.stream = stream;
        this.bigEndian = bigEndian;
        this.state = new SavedState();
      }

      public int ReadChar() {
        int c1 = this.state.Read(this.stream);
        if (c1 < 0) {
          return -1;
        }
        int c2 = this.state.Read(this.stream);
        if (c2 < 0) {
          throw new InvalidOperationException("Invalid UTF-32");
        }
        int c3 = this.state.Read(this.stream);
        if (c3 < 0) {
          throw new InvalidOperationException("Invalid UTF-32");
        }
        int c4 = this.state.Read(this.stream);
        if (c4 < 0) {
          throw new InvalidOperationException("Invalid UTF-32");
        }
        c1 = this.bigEndian ? ((c1 << 24) | (c2 << 16) | (c3 << 8) | c4) :
          ((c4 << 24) | (c3 << 16) | (c2 << 8) | c1);
        int surr = c1 & 0xfffc00;
        if (c1 < 0 || c1 >= 0x110000 || (c1 & 0xfff800) == 0xd800) {
          throw new InvalidOperationException("Invalid UTF-32");
        }

        return c1;
      }

      public int Read(int[] chars, int index, int length) {
        int count = 0;
        for (int i = 0; i < length; ++i) {
          int c = this.ReadChar();
          if (c < 0) {
 return count;
}
          chars[index + i] = c;
        }
        return count;
      }
    }

    private sealed class Utf8Reader : ICharacterInput {
      private readonly IByteReader stream;
      private int lastChar;
      private SavedState state;

      public Utf8Reader(IByteReader stream) {
        this.stream = stream;
        this.lastChar = -1;
        this.state = new SavedState();
      }

      public void Unget(int ch) {
        this.state.AddOne(ch);
      }

      public void UngetThree(int a, int b, int c) {
        this.state.AddThree(a, b, c);
      }

      public int ReadChar() {
        int cp = 0;
        int bytesSeen = 0;
        int bytesNeeded = 0;
        int lower = 0;
        int upper = 0;
          while (true) {
            int b;
            if (this.lastChar != -1) {
              b = this.lastChar;
              this.lastChar = -1;
            } else {
              b = this.state.Read(this.stream);
            }
            if (b < 0) {
              if (bytesNeeded != 0) {
                bytesNeeded = 0;
                throw new InvalidOperationException("Invalid UTF-8");
              }
              return -1;
            }
            if (bytesNeeded == 0) {
              if ((b & 0x7f) == b) {
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
                throw new InvalidOperationException("Invalid UTF-8");
              }
              continue;
            }
            if (b < lower || b > upper) {
              cp = bytesNeeded = bytesSeen = 0;
              throw new InvalidOperationException("Invalid UTF-8");
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

            return ret;
          }
      }

      public int Read(int[] chars, int index, int length) {
        int count = 0;
        for (int i = 0; i < length; ++i) {
          int c = this.ReadChar();
          if (c < 0) {
 return count;
}
          chars[index + i] = c;
        }
        return count;
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

    public CharacterReader(Stream stream) : this(stream, 2) {
    }

    // Mode can be:
    // 0 - UTF-8 only
    // 1 - UTF-8 and UTF-16
    // 2 - UTF-8, UTF-16, and UTF-32
    // All three modes ignore the starting byte order mark
    public CharacterReader(Stream stream, int mode) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      this.stream = new WrappedStream(stream);
      this.mode = mode;
    }

    private ICharacterInput reader;

    // Detects a Unicode encoding assuming
    // the first character read will be ASCII
    // unless a byte order mark appears
    private int DetectUnicodeEncoding() {
      int mode = this.mode;
      int c1 = this.stream.ReadByte();
      if (c1 < 0) {
        return -1;
      }
      if (mode == 0) {  // UTF-8 only
        var utf8reader = new Utf8Reader(this.stream);
        this.reader = utf8reader;
        c1 = utf8reader.ReadChar();
        if (c1 == 0xfeff) {
          // Skip BOM
          c1 = utf8reader.ReadChar();
        }
        return c1;
      }
      // The rest of this method handles modes 1 and 2
      if (c1 == 0xff) {
        if (this.stream.ReadByte() == 0xfe) {
          if (mode == 1) {
            // UTF-8 or UTF-16 only, so this is little endian UTF-16
            var newReader = new Utf16Reader(this.stream, false);
            this.reader = newReader;
            return newReader.ReadChar();
          }
          // Little endian UTF-16 or UTF-32
          int c3 = this.stream.ReadByte();
          int c4 = this.stream.ReadByte();
          if (c3 == 0 && c4 == 0) {
            this.reader = new Utf32Reader(this.stream, false);
            return this.reader.ReadChar();
          } else {
            var newReader = new Utf16Reader(this.stream, false);
            this.reader = newReader;
            newReader.Unget(c3, c4);
            return newReader.ReadChar();
          }
        }
        throw new InvalidOperationException("Invalid Unicode stream");
      }
      if (c1 == 0xfe) {
        if (this.stream.ReadByte() == 0xff) {
          if (mode == 1) {
            // UTF-8 or UTF-16 only, so this is big endian UTF-16
            var newReader = new Utf16Reader(this.stream, true);
            this.reader = newReader;
            return newReader.ReadChar();
          }
          // Big endian UTF-16 or UTF-32
          int c3 = this.stream.ReadByte();
          int c4 = this.stream.ReadByte();
          if (c3 == 0 && c4 == 0) {
            this.reader = new Utf32Reader(this.stream, true);
            return this.reader.ReadChar();
          } else {
            var newReader = new Utf16Reader(this.stream, true);
            this.reader = newReader;
            newReader.Unget(c3, c4);
            return newReader.ReadChar();
          }
        }
        throw new InvalidOperationException("Invalid Unicode stream");
      }
      if (c1 == 0 && mode > 0) {
        int c2 = this.stream.ReadByte();
        if (c2 < 0) {
          // 0 EOF
          this.reader = new Utf8Reader(this.stream);
          return 0;
        }
        if (mode == 1) {
          // UTF-8 or UTF-16 only
          if ((c2 & 0x80) == 0 && c2 != 0) {
            this.reader = new Utf16Reader(this.stream, true);
            return c2;
          } else {
            var newreader = new Utf8Reader(this.stream);
            newreader.Unget(c2);
            this.reader = newreader;
            return 0;
          }
        }
        if (c2 == 0) {
          // 0 0
          int c3 = this.stream.ReadByte();
          int c4 = this.stream.ReadByte();
          if (c3 == 0xfe && c4 == 0xff) {
            // 0 0 FE FF
            this.reader = new Utf32Reader(this.stream, true);
            return this.reader.ReadChar();
          }
          if (c3 == 0 && c4 >= 0 && (c4 & 0x80) == 0) {
            // 0 0 0 ASCII
            this.reader = new Utf32Reader(this.stream, true);
            return c4;
          } else {
            // Other cases of "0 0 Any Any"
            var newReader = new Utf8Reader(this.stream);
            this.reader = newReader;
            newReader.UngetThree(c2, c3, c4);
            return 0;
          }
        }
        if ((c2 & 0x80) == 0) {
          // 0 ASCII
          // UTF-16BE
          this.reader = new Utf16Reader(this.stream, true);
          return c2;
        } else {
          // 0 NonAscii
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
          // ASCII 0
          if (mode == 1) {
            // UTF-8 and UTF-16 only, assume UTF-16 little-endian
            var newReader = new Utf16Reader(this.stream, false);
            this.reader = newReader;
            return c1;
          }
          int c3 = this.stream.ReadByte();
          int c4 = this.stream.ReadByte();
          if (c3 == 0 && c4 == 0) {
            // ASCII 0 0 0
            this.reader = new Utf32Reader(this.stream, false);
            return c1;
          } else {
            // Other cases of "ASCII 0 Any Any"
            var newReader = new Utf16Reader(this.stream, false);
            this.reader = newReader;
            newReader.Unget(c3, c4);
            return c1;
          }
        } else {
          // ASCII NonZero
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
        c1 = utf8reader.ReadChar();
        if (c1 == 0xfeff) {
          // Skip BOM
          c1 = utf8reader.ReadChar();
        }
        return c1;
      }
    }

    /// <summary>Reads the next character from a Unicode stream or a
    /// string.</summary>
    /// <returns>The next character, or -1 if the end of the string or
    /// stream was reached.</returns>
    public int ReadChar() {
      if (this.reader != null) {
        return this.reader.ReadChar();
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
          throw new InvalidOperationException("Unpaired surrogate code point");
        }
        ++this.offset;
        return c;
      }
    }

    public int Read(int[] chars, int index, int length) {
      if (chars == null) {
  throw new ArgumentNullException("chars");
}
if (index < 0) {
  throw new ArgumentException("index (" + index +
    ") is less than " + 0);
}
if (index > chars.Length) {
  throw new ArgumentException("index (" + index +
    ") is more than " + chars.Length);
}
if (length < 0) {
  throw new ArgumentException("length (" + length +
    ") is less than " + 0);
}
if (length > chars.Length) {
  throw new ArgumentException("length (" + length +
    ") is more than " + chars.Length);
}
if (chars.Length - index < length) {
  throw new ArgumentException("chars's length minus " + index + " (" +
    (chars.Length - index) + ") is less than " + length);
}
      int count = 0;
      for (int i = 0; i < length; ++i) {
        int c = this.ReadChar();
        if (c < 0) {
 return count;
}
        chars[index + i] = c;
      }
      return count;
    }
  }
}
