using System;

namespace PeterO {
  /// <summary>A class that adapts a random byte generator to generate
  /// random numbers in a variety of statistical distributions.
  /// <para>The method descriptions in this class assume the underlying
  /// random byte generator generates uniformly distributed numbers that
  /// are independent of each other.</para>
  /// <para><b>Thread safety:</b> The methods in this class are safe for
  /// concurrent use by multiple threads, as long as the underlying
  /// random byte generator is as well.</para></summary>
  public sealed class RandomGenerator : IRandomGenExtended
  {
    private readonly IRandomGen valueIrg;
    private readonly object valueNormalLock = new();
    private bool valueHaveLastNormal;
    private double valueLastNormal;

    /// <summary>Initializes a new instance of the
    /// <see cref='RandomGenerator'/> class.Initializes a new instance of
    /// the RandomGenerator class.</summary>
    public RandomGenerator() : this(new XorShift128Plus()) {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='RandomGenerator'/> class.Initializes a new instance of
    /// the RandomGenerator class.</summary>
    /// <param name='valueIrg'>An IRandomGen object.</param>
    public RandomGenerator(IRandomGen valueIrg) {
      this.valueIrg = valueIrg;
    }

    /// <summary>Returns either true or false, depending on the given
    /// probability.</summary>
    /// <param name='p'>A probability from 0 through 1. 0 means always
    /// false, and 1 means always true.</param>
    /// <returns>A Boolean object.</returns>
    public bool Bernoulli(double p) {
      return p < 0 ?
        throw new ArgumentException("p(" + p + ") is less than 0") :
        p > 1 ? throw new ArgumentException("p(" + p + ") is more than 1") :
this.Uniform() < p;
    }

    /// <summary>Returns either true or false at a 50% chance
    /// each.</summary>
    /// <returns>A Boolean object.</returns>
    public bool Bernoulli() {
      return this.UniformInt(2) == 0;
    }

    /// <summary>Conceptually, generates either 1 or 0 the given number of
    /// times, where either number is equally likely, and counts the number
    /// of 1's generated.</summary>
    /// <param name='trials'>The number of times to generate a random
    /// number, conceptually.</param>
    /// <returns>A 32-bit signed integer.</returns>
    public int Binomial(int trials) {
      return this.Binomial(trials, 0.5);
    }

    /// <inheritdoc/>
    /// <returns/>
    /// <param name='bytes'>Not documented yet.</param>
    /// <param name='offset'>Not documented yet.</param>
    /// <param name='count'>Not documented yet.</param>
    public int GetBytes(byte[] bytes, int offset, int count) {
      return this.valueIrg.GetBytes(bytes, offset, count);
    }

    /// <summary>Conceptually, generates either 1 or 0 the given number of
    /// times, where a 1 is generated at the given probability, and counts
    /// the number of 1's generated.</summary>
    /// <param name='trials'>The number of times to generate a random
    /// number, conceptually.</param>
    /// <param name='p'>The probability for each trial to succeed, from 0
    /// (never) to 1 (always).</param>
    /// <returns>The number of successes in a given number of
    /// trials.</returns>
    public int Binomial(int trials, double p) {
      if (p < 0) {
        throw new ArgumentException("p(" + p + ") is less than 0");
      }
      if (p > 1) {
        throw new ArgumentException("p(" + p + ") is more than 1");
      }
      if (trials <= -1) {
        throw new ArgumentException("trials(" + trials +
          ") is not greater than " + (-1));
      }
      if (trials == 0 || p == 1.0) {
        return trials;
      }
      var count = 0;
      if (p == 0.5) {
        var bytes = new byte[1];
        for (int i = 0; i < trials && i >= 0;) {
          _ = this.valueIrg.GetBytes(bytes, 0, 1);
          int b = bytes[0];
          while (i < trials && i >= 0) {
            if ((b & 1) == 1) {
              ++count;
            }
            b >>= 1;
            ++i;
          }
        }
      } else {
        for (int i = 0; i < trials && i >= 0; ++i) {
          if (this.Uniform() < p) {
            ++count;
          }
        }
      }
      return count;
    }

    /// <summary>Generates a random number that is the sum of the squares
    /// of "df" normally-distributed random numbers with a mean of 0 and a
    /// standard deviation of 1.</summary>
    /// <param name='df'>Degrees of freedom (the number of independently
    /// chosen normally-distributed numbers).</param>
    /// <returns>A 64-bit floating-point number.</returns>
    public double ChiSquared(int df) {
      return df <= 0 ? throw new ArgumentException("df(" + df + ") is not" +
"\u0020greater than 0") : this.Gamma(df * 0.5, 2);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 64-bit floating-point number.</returns>
    public double Exponential() {
      return -Math.Log(1.0 - this.Uniform());
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='a'>Another 64-bit floating-point number.</param>
    /// <param name='b'>A 64-bit floating-point number. (3).</param>
    /// <returns>A 64-bit floating-point number.</returns>
    public double Gamma(double a, double b) {
      return b <= 0 ? throw new ArgumentException("b(" + b + ") is not" +
"\u0020greater than 0") : this.Gamma(a) * b;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='a'>Another 64-bit floating-point number.</param>
    /// <returns>A 64-bit floating-point number.</returns>
    public double Gamma(double a) {
      if (a <= 0) {
        throw new ArgumentException("a(" + a + ") is not greater than 0");
      }
      double v, x, u, x2, d, c;
      d = (a < 1 ? 1 + a : a) - (1.0 / 3.0);
      c = 1 / Math.Sqrt(9 * d);
      do {
        do {
          x = this.Normal();
          v = Math.Pow((c * x) + 1, 3);
        } while (v <= 0);
        u = 1.0 - this.Uniform();
        x2 = x * x;
      } while (u >= 1 - (0.0331 * x2 * x2) &&
        Math.Log(u) >= (0.5 * x2) + (d * (1 - v + Math.Log(v))));
      return a < 1 ? d * v * Math.Exp(this.Exponential() / -a) : d * v;
    }

    /// <summary>Conceptually, generates either 1 or 0 until a 1 is
    /// generated, and counts the number of 0's generated. Either number
    /// has an equal probability of being generated.</summary>
    /// <returns>The number of failures until a success happens.</returns>
    public int Geometric() {
      return this.NegativeBinomial(1, 0.5);
    }

    /// <summary>Conceptually, generates either 1 or 0 until a 1 is
    /// generated, and counts the number of 0's generated. A 1 is generated
    /// at the given probability.</summary>
    /// <param name='p'>A 64-bit floating-point number.</param>
    /// <returns>The number of failures until a success happens.</returns>
    public int Geometric(double p) {
      return this.NegativeBinomial(1, p);
    }

    /// <summary>Conceptually, given a set of tokens, some of which are
    /// labeled 1 and the others labeled 0, draws "trials" tokens at random
    /// without replacement and then counts the number of 1's
    /// drawn.</summary>
    /// <param name='trials'>The number of tokens drawn at random without
    /// replacement.</param>
    /// <param name='ones'>The number of tokens labeled 1.</param>
    /// <param name='count'>The number of tokens labeled 1 or 0.</param>
    /// <returns>A 32-bit signed integer.</returns>
    public int Hypergeometric(int trials, int ones, int count) {
      if (ones < 0) {
        throw new ArgumentException("ones(" + ones + ") is less than 0");
      }
      if (ones > count) {
        throw new ArgumentException("ones(" + ones + ") is more than " +
          count);
      }
      if (count < 0) {
        throw new ArgumentException("count(" + count +
          ") is less than 0");
      }
      if (trials < 0) {
        throw new ArgumentException("trials(" + trials +
          ") is less than 0");
      }
      if (trials > count) {
        throw new ArgumentException("trials(" + trials +
          ") is more than " + count);
      }
      var ret = 0;
      for (int i = 0; i < trials && ones > 0; ++i) {
        if (this.UniformInt(count) < ones) {
          --ones;
          ++ret;
        }
        --count;
      }
      return ret;
    }

    /// <summary>Generates a logarithmic normally-distributed number with
    /// the given mean and standard deviation.</summary>
    /// <param name='mean'>The desired mean.</param>
    /// <param name='sd'>Standard deviation.</param>
    /// <returns>A 64-bit floating-point number.</returns>
    public double LogNormal(double mean, double sd) {
      return Math.Exp(this.Normal(mean, sd));
    }

    /// <summary>Conceptually, generates either 1 or 0 until the given
    /// number of 1's are generated, and counts the number of 0's
    /// generated. A 1 is generated at the given probability.</summary>
    /// <param name='trials'>The number of 1's to generate before the
    /// process stops.</param>
    /// <param name='p'>The probability for each trial to succeed, from 0
    /// (never) to 1 (always).</param>
    /// <returns>The number of 0's generated. Returns Int32.MaxValue if
    /// <paramref name='p'/> is 0.</returns>
    public int NegativeBinomial(int trials, double p) {
      if (p < 0) {
        throw new ArgumentException("p(" + p + ") is less than 0");
      }
      if (p > 1) {
        throw new ArgumentException("p(" + p + ") is more than 1");
      }
      if (trials <= -1) {
        throw new ArgumentException("trials(" + trials +
          ") is not greater than " + (-1));
      }
      if (trials == 0 || p == 1.0) {
        return 0;
      }
      if (p == 0.0) {
        return Int32.MaxValue;
      }
      var count = 0;
      if (p == 0.5) {
        var bytes = new byte[1];
        while (true) {
          _ = this.valueIrg.GetBytes(bytes, 0, 1);
          int b = bytes[0];
          for (int i = 0; i < 8; ++i) {
            if ((b & 1) == 1) {
              --trials;
              if (trials <= 0) {
                return count;
              }
            } else {
              count = checked(count + 1);
            }
            b >>= 1;
            ++i;
          }
        }
      } else {
        while (true) {
          if (this.Uniform() < p) {
            --trials;
            if (trials <= 0) {
              return count;
            }
          } else {
            count = checked(count + 1);
          }
        }
      }
    }

    /// <summary>Conceptually, generates either 1 or 0 the given number of
    /// times until the given number of 1's are generated, and counts the
    /// number of 0's generated. Either number has an equal probability of
    /// being generated.</summary>
    /// <param name='trials'>The number of 1's to generate before the
    /// process stops.</param>
    /// <returns>The number of 0's generated. Returns Int32.MaxValue if "p"
    /// is 0.</returns>
    public int NegativeBinomial(int trials) {
      return this.NegativeBinomial(trials, 0.5);
    }
    // The Normal, Exponential, Poisson, and
    // single-argument Gamma methods were adapted
    // from a third-party public-domain JavaScript file.

    /// <summary>Generates a normally-distributed number with mean 0 and
    /// standard deviation 1.</summary>
    /// <returns>A 64-bit floating-point number.</returns>
    public double Normal() {
      lock (this.valueNormalLock) {
        if (this.valueHaveLastNormal) {
          this.valueHaveLastNormal = false;
          return this.valueLastNormal;
        }
      }
      double x = 1.0 - this.Uniform();
      double y = this.Uniform();
      double s = Math.Sqrt(-2 * Math.Log(x));
      double t = 2 * Math.PI * y;
      double otherNormal = s * Math.Sin(t);
      lock (this.valueNormalLock) {
        this.valueLastNormal = otherNormal;
        this.valueHaveLastNormal = true;
      }
      return s * Math.Cos(t);
    }

    /// <summary>Generates a normally-distributed number with the given
    /// mean and standard deviation.</summary>
    /// <param name='mean'>The desired mean.</param>
    /// <param name='sd'>Standard deviation.</param>
    /// <returns>A 64-bit floating-point number.</returns>
    public double Normal(double mean, double sd) {
      return (this.Normal() * sd) + mean;
    }

    /// <summary>Generates a random integer such that the average of random
    /// numbers approaches the given mean number when this method is called
    /// repeatedly with the same mean.</summary>
    /// <param name='mean'>The expected mean of the random numbers.</param>
    /// <returns>A 32-bit signed integer.</returns>
    public int Poisson(double mean) {
      if (mean < 0) {
        throw new ArgumentException("mean(" + mean +
          ") is less than 0");
      }
      double l = Math.Exp(-mean);
      var count = -1;
      double p = 1.0;
      while (true) {
        ++count;
        p *= this.Uniform();
        if (p <= l) {
          return count;
        }
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='min'>Smallest possible number that will be
    /// generated.</param>
    /// <param name='max'>Number that the randomly-generated number will be
    /// less than.</param>
    /// <returns>A 64-bit floating-point number.</returns>
    public double Uniform(double min, double max) {
      return min >= max ?
        throw new ArgumentException("min(" + min + ") is not less than " +
          max) : min + ((max - min) * this.Uniform());
    }

    /// <summary>Returns a uniformly-distributed 64-bit floating-point
    /// number from 0 and up, but less than the given number.</summary>
    /// <param name='max'>Number that the randomly-generated number will be
    /// less than.</param>
    /// <returns>A 64-bit floating-point number.</returns>
    public double Uniform(double max) {
      return this.Uniform(0.0, max);
    }

    /// <summary>Returns a uniformly-distributed 64-bit floating-point
    /// number from 0 and up, but less than 1.</summary>
    /// <returns>A 64-bit floating-point number.</returns>
    public double Uniform() {
      return this.UniformLong(9007199254740992L) / 9007199254740992.0;
    }

    /// <summary>Returns a uniformly-distributed 32-bit floating-point
    /// number from 0 and up, but less than 1.</summary>
    /// <returns>A 32-bit floating-point number.</returns>
    public double UniformSingle() {
      return this.UniformInt(16777216) / 16777216.0f;
    }

    /// <summary>Generates a random 32-bit signed integer within a given
    /// range.</summary>
    /// <param name='minInclusive'>Smallest possible value of the random
    /// number.</param>
    /// <param name='maxExclusive'>One plus the largest possible value of
    /// the random number.</param>
    /// <returns>A 32-bit signed integer.</returns>
    public int UniformInt(int minInclusive, int maxExclusive) {
      if (minInclusive > maxExclusive) {
        throw new ArgumentException("minInclusive(" + minInclusive +
          ") is more than " + maxExclusive);
      }
      if (minInclusive == maxExclusive) {
        return minInclusive;
      }
      if (minInclusive >= 0) {
        return minInclusive + this.UniformInt(maxExclusive - minInclusive);
      } else {
        long diff = maxExclusive - minInclusive;
        return diff <= Int32.MaxValue ? minInclusive +
this.UniformInt((int)diff) : (int)(minInclusive + this.UniformLong(diff));
      }
    }

    /// <summary>Generates a random 64-bit signed integer within a given
    /// range.</summary>
    /// <param name='minInclusive'>Smallest possible value of the random
    /// number.</param>
    /// <param name='maxExclusive'>One plus the largest possible value of
    /// the random number.</param>
    /// <returns>A 64-bit signed integer.</returns>
    public long UniformLong(long minInclusive, long maxExclusive) {
      if (minInclusive > maxExclusive) {
        throw new ArgumentException("minInclusive(" + minInclusive +
          ") is more than " + maxExclusive);
      }
      if (minInclusive == maxExclusive) {
        return minInclusive;
      }
      if (minInclusive >= 0) {
        return minInclusive + this.UniformLong(maxExclusive - minInclusive);
      } else {
        if ((maxExclusive < 0 && Int64.MaxValue + maxExclusive <
            minInclusive) ||
          (maxExclusive > 0 && Int64.MinValue + maxExclusive > minInclusive) ||
          minInclusive - maxExclusive < 0) {
          var b = new byte[8];
          while (true) {
            _ = this.valueIrg.GetBytes(b, 0, 8);
            // Difference is greater than MaxValue
            long lb = b[0] & 0xffL;
            lb |= (b[1] & 0xffL) << 8;
            lb |= (b[2] & 0xffL) << 16;
            lb |= (b[3] & 0xffL) << 24;
            lb |= (b[4] & 0xffL) << 32;
            lb |= (b[5] & 0xffL) << 40;
            lb |= (b[6] & 0xffL) << 48;
            lb |= (b[7] & 0x7fL) << 56;
            if (lb >= minInclusive && lb < maxExclusive) {
              return lb;
            }
          }
        } else {
          return minInclusive + this.UniformLong(maxExclusive - minInclusive);
        }
      }
    }

    /// <summary>Generates a random 32-bit signed integer 0 or greater and
    /// less than the given number.</summary>
    /// <param name='maxExclusive'>One plus the largest possible value of
    /// the random number.</param>
    /// <returns>A 32-bit signed integer.</returns>
    public int UniformInt(int maxExclusive) {
      if (maxExclusive < 0) {
        throw new ArgumentException("maxExclusive(" + maxExclusive +
          ") is less than 0");
      }
      if (maxExclusive <= 1) {
        return 0;
      }
      if (this.valueIrg is IRandomGenExtended rge) {
        return rge.GetInt32(maxExclusive);
      }
      var b = new byte[4];
      switch (maxExclusive) {
        case 2: {
            _ = this.valueIrg.GetBytes(b, 0, 1);
            return b[0] & 1;
          }
        case 256: {
            _ = this.valueIrg.GetBytes(b, 0, 1);
            return b[0] & 1;
          }
        default: {
            while (true) {
              int ib;
              if (maxExclusive == 0x1000000) {
                _ = this.valueIrg.GetBytes(b, 0, 3);
                ib = b[0] & 0xff;
                ib |= (b[1] & 0xff) << 8;
                ib |= (b[2] & 0xff) << 16;
                return ib;
              }
              if (maxExclusive == 0x10000) {
                _ = this.valueIrg.GetBytes(b, 0, 2);
                ib = b[0] & 0xff;
                ib |= (b[1] & 0xff) << 8;
                return ib;
              }
              int maxexc;
              maxexc = Int32.MaxValue / maxExclusive * maxExclusive;
              while (true) {
                _ = this.valueIrg.GetBytes(b, 0, 4);
                ib = b[0] & 0xff;
                ib |= (b[1] & 0xff) << 8;
                ib |= (b[2] & 0xff) << 16;
                ib |= (b[3] & 0x7f) << 24;
                if (ib < maxexc) {
                  return ib % maxExclusive;
                }
              }
            }
          }
      }
    }

    /// <inheritdoc/>
    /// <returns/>
    /// <param name='maxExclusive'>Not documented yet.</param>
    public long GetInt64(long maxExclusive) {
      return this.UniformLong(maxExclusive);
    }

    /// <inheritdoc/>
    /// <returns/>
    /// <param name='maxExclusive'>Not documented yet.</param>
    public int GetInt32(int maxExclusive) {
      return this.UniformInt(maxExclusive);
    }

    /// <summary>Generates a random 32-bit signed integer 0 or greater and
    /// less than the given number.</summary>
    /// <param name='maxExclusive'>One plus the largest possible value of
    /// the random number.</param>
    /// <returns>A 64-bit signed integer.</returns>
    public long UniformLong(long maxExclusive) {
      if (maxExclusive < 0) {
        throw new ArgumentException("maxExclusive(" + maxExclusive +
          ") is less than 0");
      }
      if (maxExclusive <= Int32.MaxValue) {
        return this.UniformInt((int)maxExclusive);
      }
      if (this.valueIrg is IRandomGenExtended rge) {
        return rge.GetInt64(maxExclusive);
      }

      long maxexc;
      var b = new byte[8];
      maxexc = Int64.MaxValue / maxExclusive * maxExclusive;
      while (true) {
        _ = this.valueIrg.GetBytes(b, 0, 8);
        long lb = b[0] & 0xffL;
        lb |= (b[1] & 0xffL) << 8;
        lb |= (b[2] & 0xffL) << 16;
        lb |= (b[3] & 0xffL) << 24;
        lb |= (b[4] & 0xffL) << 32;
        lb |= (b[5] & 0xffL) << 40;
        lb |= (b[6] & 0xffL) << 48;
        lb |= (b[7] & 0x7fL) << 56;
        if (lb < maxexc) {
          return lb % maxExclusive;
        }
      }
    }
  }
}
