using System;

namespace PeterO {
    /// <summary>A class that implements a statistically-random byte
    /// generator, using Sebastiano Vigna's
    /// <a
    /// href='http://xorshift.di.unimi.it/xorshift128plus.c'>xorshift128+</a>
    /// RNG as the underlying implementation. This class is safe for
    /// concurrent use among multiple threads.</summary>
  public class XorShift128Plus : IRandomGen {
    private long[] s = new long[2];
    private object syncRoot = new Object();

    public XorShift128Plus() {
      this.Seed();
    }

    public int GetBytes(byte[] bytes, int offset, int length) {
      if (bytes == null) {
        throw new ArgumentNullException("bytes");
      }
      if (offset < 0) {
        throw new ArgumentException("offset (" + offset +
          ") is less than 0");
      }
      if (offset > bytes.Length) {
        throw new ArgumentException("offset (" + offset +
          ") is more than " + bytes.Length);
      }
      if (length < 0) {
        throw new ArgumentException("length (" + length +
          ") is less than 0");
      }
      if (length > bytes.Length) {
        throw new ArgumentException("length (" + length +
          ") is more than " + bytes.Length);
      }
      if (bytes.Length - offset < length) {
        throw new ArgumentException("bytes's length minus " + offset + " (" +
          (bytes.Length - offset) + ") is less than " + length);
      }
      int count = length;
      lock (this.syncRoot) {
        while (length >= 8) {
          long nv = this.NextValue();
          bytes[offset++] = unchecked((byte)nv);
          nv >>= 8;
          bytes[offset++] = unchecked((byte)nv);
          nv >>= 8;
          bytes[offset++] = unchecked((byte)nv);
          nv >>= 8;
          bytes[offset++] = unchecked((byte)nv);
          nv >>= 8;
          bytes[offset++] = unchecked((byte)nv);
          nv >>= 8;
          bytes[offset++] = unchecked((byte)nv);
          nv >>= 8;
          bytes[offset++] = unchecked((byte)nv);
          nv >>= 8;
          bytes[offset++] = unchecked((byte)nv);
          length -= 8;
        }
        if (length != 0) {
          long nv = this.NextValue();
          while (length > 0) {
            bytes[offset++] = unchecked((byte)nv);
            nv >>= 8;
            --length;
          }
        }
      }
      return count;
    }

    // xorshift128 + generator
    // http://xorshift.di.unimi.it/xorshift128plus.c
    private long NextValue() {
          long s1 = this.s[0];
          long s0 = this.s[1];
           this.s[0] = s0;
           s1 ^= s1 << 23;
           long t1 = (s1 >> 18) & 0x3fffffffffffL;
           long t0 = (s0 >> 5) & 0x7ffffffffffffffL;
           this.s[1] = s1 ^ s0 ^ t1 ^ t0;
           return unchecked(this.s[1] + s0);
    }

    private void Seed() {
      long lb = DateTime.Now.Ticks & 0xffffffffffL;
      this.s[0] =lb;
      lb = 0L;
      this.s[1] = lb;
      if ((this.s[0] | this.s[1]) == 0) {
        ++this.s[0];
      }
    }
  }
}
