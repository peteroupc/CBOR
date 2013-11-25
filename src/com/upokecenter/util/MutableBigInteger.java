package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.math.*;

  final class MutableBigInteger {
    int[] data;
    int length;
    public MutableBigInteger() {
      data = new int[2];
      length = 1;
      data[0] = 0;
    }
    public MutableBigInteger(long val) {
      if (val < 0)
        throw new IllegalArgumentException("Only positive integers are supported");
      data = new int[4];
      length = 2;
      data[0] = ((int)((val) & 0xFFFFFFFFL));
      data[1] = ((int)((val >> 32) & 0xFFFFFFFFL));
    }
    private static byte[] ReverseBytes(byte[] bytes) {
      if ((bytes) == null) throw new NullPointerException("bytes");
      int half = bytes.length >> 1;
      int right = bytes.length - 1;
      for (int i = 0; i < half; i++, right--) {
        byte value = bytes[i];
        bytes[i] = bytes[right];
        bytes[right] = value;
      }
      return bytes;
    }
    /**
     * 
     */
    public BigInteger ToBigInteger() {
      byte[] bytes = new byte[length * 4 + 1];
      for (int i = 0; i < length; i++) {
        bytes[i * 4 + 0] = (byte)((data[i]) & 0xFF);
        bytes[i * 4 + 1] = (byte)((data[i] >> 8) & 0xFF);
        bytes[i * 4 + 2] = (byte)((data[i] >> 16) & 0xFF);
        bytes[i * 4 + 3] = (byte)((data[i] >> 24) & 0xFF);
      }
      bytes[bytes.length - 1] = 0;
      return new BigInteger(ReverseBytes((byte[])bytes));
    }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      return ToBigInteger().toString();
    }

    /**
     * Multiplies this instance by the value of a Int32 object.
     * @param multiplicand A 32-bit signed integer.
     * @return The product of the two objects.
     */
    public MutableBigInteger Multiply(int multiplicand) {
      if (multiplicand < 0)
        throw new IllegalArgumentException("Only positive multiplicands are supported");
      else if (multiplicand != 0) {
        int carry = 0;
        for (int i = 0; i < length; i++) {
          long subproduct = ((long)data[i]) & 0xFFFFFFFFL;
          subproduct *= multiplicand;
          subproduct += carry;
          carry = ((int)((subproduct >> 32) & 0xFFFFFFFFL));
          data[i] = ((int)((subproduct) & 0xFFFFFFFFL));
        }
        if (carry != 0) {
          if (length >= data.length) {
            int[] newdata = new int[length + 20];
            System.arraycopy(data, 0, newdata, 0, data.length);
            data = newdata;
          }
          data[length] = carry;
          length++;
        }
      } else {
        data[0] = 0;
        length = 1;
      }
      return this;
    }
    /**
     * 
     * @param augend A 32-bit signed integer.
     */
    public MutableBigInteger Add(int augend) {
      if (augend < 0)
        throw new IllegalArgumentException("Only positive augends are supported");
      else if (augend != 0) {
        int carry = 0;
        for (int i = 0; i < length; i++) {
          long subproduct = ((long)data[i]) & 0xFFFFFFFFL;
          subproduct += augend;
          subproduct += carry;
          carry = ((int)((subproduct >> 32) & 0xFFFFFFFFL));
          data[i] = ((int)((subproduct) & 0xFFFFFFFFL));
          if (carry == 0) return this;
          augend = 0;
        }
        if (carry != 0) {
          if (length >= data.length) {
            int[] newdata = new int[length + 20];
            System.arraycopy(data, 0, newdata, 0, data.length);
            data = newdata;
          }
          data[length] = carry;
          length++;
        }
      }
      return this;
    }
  }
