/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;

namespace PeterO
{
  internal static class Extras
  {
    public static int[] DoubleToIntegers(double dbl) {
      long value = BitConverter.ToInt64(
        BitConverter.GetBytes((double)dbl), 0);
      return new int[]{
        unchecked((int)(value & 0xFFFFFFFFL)),
        unchecked((int)((value >> 32) & 0xFFFFFFFFL))
      };
    }

    public static double IntegersToDouble(int[] integers) {
      long value = ((long)integers[0]) & 0xFFFFFFFFL;
      value |= (((long)integers[1]) & 0xFFFFFFFFL) << 32;
      return BitConverter.ToDouble(BitConverter.GetBytes((long)value), 0);
    }

  }
}
