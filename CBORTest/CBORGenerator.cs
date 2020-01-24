using System;
using System.Collections.Generic;
using PeterO;

namespace Test {
  public sealed class CBORGenerator {
    private sealed class ByteWriter {
      private byte[] bytes = new byte[64];
      private int pos = 0;
      public ByteWriter Write(int b) {
        if (this.pos < this.bytes.Length) {
          this.bytes[this.pos++] = (byte)b;
        } else {
          var newbytes = new byte[this.bytes.Length * 2];
          Array.Copy(this.bytes, 0, newbytes, 0, this.bytes.Length);
          this.bytes = newbytes;
          this.bytes[this.pos++] = (byte)b;
        }
        return this;
      }
      public int ByteLength {
        get {
          return this.pos;
        }
      }
      public byte[] ToBytes() {
        var newbytes = new byte[this.pos];
        Array.Copy(this.bytes, 0, newbytes, 0, pos);
        return newbytes;
      }
    }

    private void GenerateArgument(
      RandomGenerator r,
      int majorType,
      int len,
      ByteWriter bs) {
      var minArg = 0;
      var maxArg = 4;
      minArg = (len < 0x18) ? (0) : ((len <= 0xff) ? 1 : ((len <= 0xffff) ?
2 : (3)));
      var sh = 0;
      int arg = minArg + r.UniformInt(maxArg - minArg + 1);
      switch (arg) {
        case 0:
          bs.Write(majorType * 0x20 + len);
          break;
        case 1:
          bs.Write(majorType * 0x20 + 0x18);
          bs.Write(len & 0xff);
          break;
        case 2:
          bs.Write(majorType * 0x20 + 0x19);
          sh = 8;
          for (int i = 0; i < 2; ++i) {
            bs.Write((len >> sh) & 0xff);
            sh -= 8;
          }
          break;
        case 3:
          bs.Write(majorType * 0x20 + 0x1a);
          sh = 24;
          for (int i = 0; i < 4; ++i) {
            bs.Write((len >> sh) & 0xff);
            sh -= 8;
          }
          break;
        case 4:
          bs.Write(majorType * 0x20 + 0x1b);
          for (int i = 0; i < 4; ++i) {
            bs.Write(0);
          }
          sh = 24;
          for (int i = 0; i < 4; ++i) {
            bs.Write((len >> sh) & 0xff);
            sh -= 8;
          }
          break;
      }
    }

    private static int[]
    MajorTypes = {
      0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 4,
      4, 5, 6, 6, 7, 7, 7, 7, 7, 7,
    };
    private static int[] MajorTypesHighLength = {
      0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 6,
      6, 7, 7, 7, 7, 7, 7,
    };

    private static void GenerateUtf8(RandomGenerator ra, ByteWriter bs, int
      length) {
      for (int i = 0; i < length;) {
        int r = ra.UniformInt(10);
        if (r > 0) {
          bs.Write(ra.UniformInt(128));
          ++i;
        } else {
          r = ra.UniformInt(3);
          if (r == 0 && length - i >= 2) {
            r = 0xc2 + ra.UniformInt((0xdf - 0xc2) + 1);
            bs.Write(r);
            bs.Write(0x80 + ra.UniformInt(0x40));
            i += 2;
          } else if (r == 1 && length - i >= 3) {
            r = 0xe0 + ra.UniformInt(16);
            bs.Write(r);
            int lower = (r == 0xe0) ? 0xa0 : 0x80;
            int upper = (r == 0xed) ? 0x9f : 0xbf;
            r = lower + ra.UniformInt((upper - lower) + 1);
            bs.Write(r);
            bs.Write(0x80 + ra.UniformInt(0x40));
            i += 3;
          } else if (r == 2 && length - i >= 4) {
            r = 0xf0 + ra.UniformInt(5);
            bs.Write(r);
            int lower = (r == 0xf0) ? 0x90 : 0x80;
            int upper = (r == 0xf4) ? 0x8f : 0xbf;
            r = lower + ra.UniformInt((upper - lower) + 1);
            bs.Write(r);
            bs.Write(0x80 + ra.UniformInt(0x40));
            bs.Write(0x80 + ra.UniformInt(0x40));
            i += 4;
          }
        }
      }
    }

    private void Generate(RandomGenerator r, int depth, ByteWriter bs) {
      int majorType = MajorTypes[r.UniformInt(MajorTypes.Length)];
      if (bs.ByteLength > 2000000) {
        majorType = MajorTypesHighLength[r.UniformInt(
  MajorTypesHighLength.Length)];
      }
      if (majorType == 3 || majorType == 2) {
        int len = r.UniformInt(1000);
        if (r.UniformInt(50) == 0 && depth < 2) {
          var v = (long)r.UniformInt(100000) * r.UniformInt(100000);
          len = (int)(v / 100000);
        }
        if (depth > 6) {
          len = r.UniformInt(100) == 0 ? 1 : 0;
        }
        // TODO: Ensure key uniqueness
        if (r.UniformInt(2) == 0) {
          // Indefinite length
          bs.Write(0x1f + majorType * 0x20);
          while (len > 0) {
            int sublen = r.UniformInt(len + 1);
            this.GenerateArgument(r, majorType, sublen, bs);
            if (majorType == 3) {
              GenerateUtf8(r, bs, sublen);
            } else {
              for (int i = 0; i < sublen; ++i) {
                bs.Write(r.UniformInt(256));
              }
            }
            len -= sublen;
          }
          bs.Write(0xff);
        } else {
          // Definite length
          this.GenerateArgument(r, majorType, len, bs);
          if (majorType == 3) {
            GenerateUtf8(r, bs, len);
          } else {
            for (int i = 0; i < len; ++i) {
              bs.Write(r.UniformInt(256));
            }
          }
        }
        return;
      } else if (majorType == 4 || majorType == 5) {
        int len = r.UniformInt(8);
        if (r.UniformInt(50) == 0 && depth < 2) {
          var v = (long)r.UniformInt(1000) * r.UniformInt(1000);
          len = (int)(v / 1000);
        }
        bool indefiniteLength = r.UniformInt(2) == 0;
        if (indefiniteLength) {
          bs.Write(0x1f + majorType * 0x20);
        } else {
          this.GenerateArgument(r, majorType, len, bs);
        }
        for (int i = 0; i < len; ++i) {
          this.Generate(r, depth + 1, bs);
          if (majorType == 5) {
            this.Generate(r, depth + 1, bs);
          }
        }
        if (indefiniteLength) {
          bs.Write(0xff);
        }
        return;
      }
      int arg = r.UniformInt(5);
      switch (arg) {
        case 0:
          bs.Write(majorType * 0x20 + r.UniformInt(0x18));
          break;
        case 1:
          bs.Write(majorType * 0x20 + 0x18);
          if (majorType == 7) {
            bs.Write(32 + r.UniformInt(224));
          } else {
            bs.Write(r.UniformInt(256));
          }
          break;
        case 2:
          bs.Write(majorType * 0x20 + 0x19);
          for (int i = 0; i < 2; ++i) {
            bs.Write(r.UniformInt(256));
          }
          break;
        case 3:
          bs.Write(majorType * 0x20 + 0x1a);
          for (int i = 0; i < 4; ++i) {
            bs.Write(r.UniformInt(256));
          }
          break;
        case 4:
          bs.Write(majorType * 0x20 + 0x1b);
          for (int i = 0; i < 8; ++i) {
            bs.Write(r.UniformInt(256));
          }
          break;
      }
      if (majorType == 6) {
        this.Generate(r, depth + 1, bs);
      }
    }

    public byte[] Generate(IRandomGen random) {
      var bs = new ByteWriter();
      this.Generate(new RandomGenerator(random), 0, bs);
      byte[] ret = bs.ToBytes();
      return ret;
    }
  }
}
