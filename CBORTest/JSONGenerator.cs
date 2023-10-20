using System;
using System.Collections.Generic;
using System.Text;
using PeterO;

namespace Test {
  public sealed class JSONGenerator {
    private sealed class ByteWriter {
      private byte[] bytes = new byte[64];

      public ByteWriter Write(int b) {
        if (this.ByteLength < this.bytes.Length) {
          this.bytes[this.ByteLength] = (byte)b;
          ++this.ByteLength;
        } else {
          var newbytes = new byte[this.bytes.Length * 2];
          Array.Copy(this.bytes, 0, newbytes, 0, this.bytes.Length);
          this.bytes = newbytes;
          this.bytes[this.ByteLength] = (byte)b;
          ++this.ByteLength;
        }
        return this;
      }

      public int ByteLength { get; set; }

      public byte[] ToBytes() {
        var newbytes = new byte[this.ByteLength];
        Array.Copy(this.bytes, 0, newbytes, 0, this.ByteLength);
        return newbytes;
      }
    }

    private static readonly int[] ValueMajorTypes = {
      0, 1, 3, 4, 5,
    };

    private static readonly int[] ValueMajorTypesTop = {
      0, 1, 3, 4, 4, 4, 4, 4, 5, 5, 5,
      5, 5, 5, 5, 5, 5, 5, 5, 5,
    };

    private static readonly int[] ValueEscapes = {
      '\\', '/', '\"',
      'b', 'f', 'n', 'r', 't', 'u',
    };

    private static readonly char[] ValueEscapeChars = {
      '\\', '/', '\"',
      (char)8, (char)12, '\n', '\r', '\t', (char)0,
    };

    private static void GenerateCodeUnit(
      IRandomGenExtended ra,
      ByteWriter bs,
      int cu) {
      int c;
      var shift = 12;
      for (int i = 0; i < 4; ++i) {
        c = (cu >> shift) & 0xf;
        int bw = c < 10 ? (0x30 + c) : (0x41 + (c - 10) +
          (ra.GetInt32(2) * 0x20));
        bs.Write(bw);
        shift -= 4;
      }
    }

    private static void GenerateUtf16(
      IRandomGenExtended ra,
      ByteWriter bs,
      StringBuilder sb) {
      int r = ra.GetInt32(0x110000 - 0x800);
      if (r >= 0xd800) {
        r += 0x800;
      }
      if (r >= 0x10000) {
        int rc = (((r - 0x10000) >> 10) & 0x3ff) | 0xd800;
        GenerateCodeUnit(ra, bs, rc);
        if (sb != null) {
          _ = sb.Append((char)rc);
        }
        _ = bs.Write('\\');
        _ = bs.Write('u');
        rc = ((r - 0x10000) & 0x3ff) | 0xdc00;
        GenerateCodeUnit(ra, bs, rc);
        if (sb != null) {
          _ = sb.Append((char)rc);
        }
      } else {
        GenerateCodeUnit(ra, bs, r);
        if (sb != null) {
          _ = sb.Append((char)r);
        }
      }
    }

    private static void GenerateWhitespace(
      IRandomGenExtended ra,
      ByteWriter bs) {
      if (ra.GetInt32(10) == 0) {
        int len = ra.GetInt32(20);
        int[] ws = { 0x09, 0x0d, 0x0a, 0x20 };
        if (ra.GetInt32(100) == 0) {
          len = ra.GetInt32(100);
        }
        for (int i = 0; i < len; ++i) {
          _ = bs.Write(ws[ra.GetInt32(ws.Length)]);
        }
      }
    }

    private static void GenerateJsonNumber(
        IRandomGenExtended ra,
        ByteWriter bs) {
      if (ra.GetInt32(2) == 0) {
        _ = bs.Write('-');
      }
      bool shortLen = ra.GetInt32(100) < 75;
      int len;
      if (ra.GetInt32(100) < 2) {
        // Integer part is zero
        _ = bs.Write(0x30);
      } else {
        // Integer part
        len = shortLen ? ra.GetInt32(10) + 1 :
           (ra.GetInt32(2000) * ra.GetInt32(2000) / 2000) + 1;
        _ = bs.Write(0x31 + ra.GetInt32(9));
        for (int i = 0; i < len; ++i) {
          _ = bs.Write(0x30 + ra.GetInt32(10));
        }
      }
      // Fractional part
      if (ra.GetInt32(2) == 0) {
        _ = bs.Write(0x2e);
        len = shortLen ? ra.GetInt32(10) + 1 :
         (ra.GetInt32(2000) * ra.GetInt32(2000) / 2000) + 1;
        for (int i = 0; i < len; ++i) {
          _ = bs.Write(0x30 + ra.GetInt32(10));
        }
      }
      if (ra.GetInt32(2) == 0) {
        int rr = ra.GetInt32(3);
        if (rr == 0) {
          _ = bs.Write('E');
        } else if (rr == 1) {
          _ = bs.Write('E').Write('+');
        } else if (rr == 2) {
          _ = bs.Write('E').Write('-');
        }
        len = 1 + ra.GetInt32(5);
        if (ra.GetInt32(10) == 0) {
          len = 1 + (ra.GetInt32(2000) * ra.GetInt32(2000) / 2000);
        }
        for (int i = 0; i < len; ++i) {
          _ = bs.Write(0x30 + ra.GetInt32(10));
        }
      }
    }

    private static int GenerateUtf8(
      IRandomGenExtended ra,
      ByteWriter bs,
      StringBuilder sb,
      int len) {
      int r = ra.GetInt32(3);
      int r2, r3, r4;
      if (r == 0 && len >= 2) {
        r = 0xc2 + ra.GetInt32(0xdf - 0xc2 + 1);
        _ = bs.Write(r);
        r2 = 0x80 + ra.GetInt32(0x40);
        _ = bs.Write(r2);
        if (sb != null) {
          _ = sb.Append((char)(((r - 0x80) << 6) | r2));
        }
        return 2;
      } else if (r == 1 && len >= 3) {
        r = 0xe0 + ra.GetInt32(16);
        _ = bs.Write(r);
        int lower = (r == 0xe0) ? 0xa0 : 0x80;
        int upper = (r == 0xed) ? 0x9f : 0xbf;
        r2 = lower + ra.GetInt32(upper - lower + 1);
        _ = bs.Write(r2);
        r3 = 0x80 + ra.GetInt32(0x40);
        _ = bs.Write(r3);
        if (sb != null) {
          _ = sb.Append((char)(((r - 0x80) << 12) | ((r2 - 0x80) << 6) | r3));
        }
        return 3;
      } else if (r == 2 && len >= 4) {
        r = 0xf0 + ra.GetInt32(5);
        _ = bs.Write(r);
        int lower = (r == 0xf0) ? 0x90 : 0x80;
        int upper = (r == 0xf4) ? 0x8f : 0xbf;
        r2 = lower + ra.GetInt32(upper - lower + 1);
        _ = bs.Write(r2);
        r3 = 0x80 + ra.GetInt32(0x40);
        _ = bs.Write(r3);
        r4 = 0x80 + ra.GetInt32(0x40);
        _ = bs.Write(r4);
        r = ((r - 0x80) << 18) | ((r2 - 0x80) << 12) | ((r3 - 0x80) << 6) | r4;
        if (sb != null) {
          _ = sb.Append((char)(((r - 0x10000) >> 10) | 0xd800));
          _ = sb.Append((char)(((r - 0x10000) & 0x3ff) | 0xdc00));
        }
        return 4;
      }
      return 0;
    }

    private static void GenerateJsonKey(
      IRandomGenExtended ra,
      ByteWriter bskey,
      int depth,
      IDictionary<string, string> keys) {
      while (true) {
        var sb = new StringBuilder();
        var bs = new ByteWriter();
        int len = ra.GetInt32(1000) * ra.GetInt32(1000);
        len /= 1000;
        if (ra.GetInt32(50) == 0 && depth < 2) {
          // Exponential curve that strongly favors small numbers
          var v = (long)ra.GetInt32(1000000) * ra.GetInt32(1000000);
          len = (int)(v / 1000000);
        }
        _ = bs.Write(0x22);
        for (int i = 0; i < len;) {
          int r = ra.GetInt32(10);
          if (r > 2) {
            int x = 0x20 + ra.GetInt32(60);
            if (x == '\"') {
              _ = bs.Write('\\').Write(x);
              _ = sb.Append('\"');
            } else if (x == '\\') {
              _ = bs.Write('\\').Write(x);
              _ = sb.Append('\\');
            } else {
              _ = bs.Write(x);
              _ = sb.Append((char)x);
            }
            ++i;
          } else if (r == 1) {
            _ = bs.Write('\\');
            int escindex = ra.GetInt32(ValueEscapes.Length);
            int esc = ValueEscapes[escindex];
            _ = bs.Write(esc);
            if (esc == 'u') {
              GenerateUtf16(ra, bs, sb);
            } else {
              _ = sb.Append(ValueEscapeChars[escindex]);
            }
          } else {
            _ = GenerateUtf8(ra, bs, sb, len - i);
          }
        }
        _ = bs.Write(0x22);
        string key = sb.ToString();
        if (!keys.ContainsKey(key)) {
          keys[key] = String.Empty;
          byte[] bytes = bs.ToBytes();
          for (int i = 0; i < bytes.Length; ++i) {
            _ = bskey.Write(bytes[i] & 0xff);
          }
          return;
        }
      }
    }

    private static void GenerateJsonString(
      IRandomGenExtended ra,
      ByteWriter bs,
      int depth) {
      int len = ra.GetInt32(1000) * ra.GetInt32(1000);
      len /= 1000;
      if (ra.GetInt32(50) == 0 && depth < 2) {
        // Exponential curve that strongly favors small numbers
        var v = (long)ra.GetInt32(1000000) * ra.GetInt32(1000000);
        len = (int)(v / 1000000);
      }
      _ = bs.Write(0x22);
      for (int i = 0; i < len;) {
        int r = ra.GetInt32(10);
        if (r > 2) {
          int x = 0x20 + ra.GetInt32(60);
          if (x is '\"' or '\\') {
            bs.Write('\\');
          }
          bs.Write(x);
          ++i;
        } else if (r == 1) {
          _ = bs.Write('\\');
          int esc = ValueEscapes[ra.GetInt32(ValueEscapes.Length)];
          _ = bs.Write(esc);
          if (esc == 'u') {
            GenerateUtf16(ra, bs, null);
          }
        } else {
          _ = GenerateUtf8(ra, bs, null, len - i);
        }
      }
      _ = bs.Write(0x22);
    }

    private void Generate(IRandomGenExtended r, int depth, ByteWriter bs) {
      int majorType;
      majorType = ValueMajorTypes[r.GetInt32(ValueMajorTypes.Length)];
      if (depth == 0) {
        majorType = ValueMajorTypesTop[r.GetInt32(ValueMajorTypes.Length)];
      }
      GenerateWhitespace(r, bs);
      if (bs.ByteLength > 2000000) {
        majorType = r.GetInt32(2); // either 0 or 1
      }
      if (majorType == 0) {
        GenerateJsonNumber(r, bs);
      } else if (majorType == 1) {
        switch (r.GetInt32(3)) {
          case 0:
            _ = bs.Write('t').Write('r').Write('u').Write(
              'e');
            break;
          case 1:
            _ = bs.Write('n').Write('u').Write('l').Write(
              'l');
            break;
          case 2:

            _ = bs.Write('f').Write('a').Write('l').Write(
  's').Write(
                    'e');
            break;
        }
      } else if (majorType == 3) {
        GenerateJsonString(r, bs, depth);
      } else if (majorType is 4 or 5) {
        int len = r.GetInt32(8);
        if (r.GetInt32(50) == 0 && depth < 2) {
          var v = (long)r.GetInt32(1000) * r.GetInt32(1000);
          len = (int)(v / 1000);
        }
        if (depth > 6) {
          len = r.GetInt32(100) == 0 ? 1 : 0;
        }
        if (majorType == 4) {
          _ = bs.Write('[');
          for (int i = 0; i < len; ++i) {
            if (i > 0) {
              _ = bs.Write(',');
            }
            this.Generate(r, depth + 1, bs);
          }
          _ = bs.Write(']');
        }
        if (majorType == 5) {
          _ = bs.Write('{');
          var keys = new Dictionary<string, string>();
          for (int i = 0; i < len; ++i) {
            if (i > 0) {
              _ = bs.Write(',');
            }
            GenerateWhitespace(r, bs);
            GenerateJsonKey(r, bs, depth, keys);
            GenerateWhitespace(r, bs);
            _ = bs.Write(':');
            GenerateWhitespace(r, bs);
            this.Generate(r, depth + 1, bs);
          }
          _ = bs.Write('}');
        }
      }
      GenerateWhitespace(r, bs);
    }

    public byte[] Generate(IRandomGenExtended random) {
      var bs = new ByteWriter();
      if (random == null) {
        throw new ArgumentNullException(nameof(random));
      }
      this.Generate(random, 0, bs);
      byte[] ret = bs.ToBytes();
      return ret;
    }
  }
}
