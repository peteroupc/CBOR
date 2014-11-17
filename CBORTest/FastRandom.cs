/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;

namespace Test {
    /// <summary>The system&apos;s random number generator will be called many
    /// times
    /// during testing. Unfortunately it can be very slow. So we use this
    /// wrapper
    /// class.</summary>
  public class FastRandom
  {
    private const int ReseedCount = 500;

    private Random rand;
    private Random rand2;
    private int count;

    private int w = 521288629;
    private int z = 362436069;
    private static int[] seeds = new int[32];

    private static void AddSeed(int seed) {
      lock (seeds) {
        if (seedIndex == -1) {
          seedIndex = 0;
        }
        seeds[seedIndex ]^=seed;
        seedCount = Math.Max(seedCount, seedIndex + 1);
        ++seedIndex;
        seedIndex %= seeds.Length;
      }
    }

    private static int GetSeed() {
      lock (seeds) {
        if (seedCount == 0) {
          return 0;
        }
        if (seedReadIndex >= seedCount) {
          seedReadIndex = 0;
        }
        return seeds[seedReadIndex++];
      }
    }

    private static int seedIndex = 0;
    private static int seedCount = 0;
    private static int seedReadIndex = 0;

    public FastRandom() {
      int randseed = GetSeed();
      this.rand = (randseed == 0) ? (new Random()) : (new Random(randseed));
      int randseed2 = unchecked(GetSeed() ^SysRandNext(this.rand, this.rand));
      this.rand2 = (randseed2 == 0) ? (new Random()) : (new Random(randseed2));
      this.count = ReseedCount;
    }

    private static int SysRandNext(Random randA, Random randB) {
      int ret = randA.Next(0x10000);
      ret |= randB.Next(0x10000) << 16;
      return ret;
    }

    private int NextValueInternal() {
      int w = this.w, z = this.z;
      // Use George Marsaglia's multiply-with-carry
      // algorithm.
      this.z = z = unchecked((36969 * (z & 65535)) + ((z >> 16) & 0xffff));
      this.w = w = unchecked((18000 * (w & 65535)) + ((z >> 16) & 0xffff));
      return ((z << 16) | (w & 65535)) & 0x7fffffff;
    }

    /// <summary>Generates a random number.</summary>
    /// <param name='v' >The return value will be 0 or greater, and less than
    /// this
    /// number.</param>
    /// <returns>A 32-bit signed integer.</returns>
    public int NextValue(int v) {
      if (v <= 0) {
        throw new ArgumentException(
          "v (" + v + ") is not greater than 0");
      }
      if (v <= 1) {
        return 0;
      }
      if (this.count >= ReseedCount) {
        // Call the default random number generator
        // every once in a while, to reseed
        this.count = 0;
        if (this.rand != null) {
          int seed = SysRandNext(this.rand, this.rand2);
          this.z ^= seed;
          seed = SysRandNext(this.rand2, this.rand);
          this.w ^= seed;
          if (this.z == 0) {
            this.z = 362436069;
          }
          if (this.w == 0) {
            this.w = 521288629;
          }
          seed = SysRandNext(this.rand, this.rand2);
          AddSeed(seed);
          return this.rand.Next(v);
        }
      }
      ++this.count;
      if (v == 0x1000000) {
        return this.NextValueInternal() & 0xffffff;
      }
      if (v == 0x10000) {
        return this.NextValueInternal() & 0xffff;
      }
      if (v == 0x100) {
        return this.NextValueInternal() & 0xff;
      }
      int maxExclusive = (Int32.MaxValue / v) * v;
      while (true) {
        int vi = this.NextValueInternal();
        if (vi < maxExclusive) {
          return vi % v;
        }
      }
    }
  }
}
