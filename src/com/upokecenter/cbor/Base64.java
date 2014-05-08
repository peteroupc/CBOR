package com.upokecenter.cbor;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import com.upokecenter.util.*;

    /**
     * Description of Base64.
     */
  final class Base64 {
private Base64() {
}
    private static final String Base64URL = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
    private static final String Base64Classic = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

    /**
     * Not documented yet.
     * @param str A StringBuilder object.
     * @param data A byte array.
     * @param padding A Boolean object.
     */
    public static void ToBase64(StringBuilder str, byte[] data, boolean padding) {
      if (data == null) {
        throw new NullPointerException("data");
      }
      ToBase64(str, data, 0, data.length, Base64Classic, padding);
    }

    public static void ToBase64URL(StringBuilder str, byte[] data, boolean padding) {
      if (data == null) {
        throw new NullPointerException("data");
      }
      ToBase64(str, data, 0, data.length, Base64URL, padding);
    }

    public static void ToBase64(StringBuilder str, byte[] data, int offset, int count, boolean padding) {
      ToBase64(str, data, offset, count, Base64Classic, padding);
    }

    public static void ToBase64URL(StringBuilder str, byte[] data, int offset, int count, boolean padding) {
      ToBase64(str, data, offset, count, Base64URL, padding);
    }

    public static String ToBase64String(byte[] data, boolean padding) {
      if (data == null) {
        throw new NullPointerException("data");
      }
      return ToBase64String(data, 0, data.length, padding);
    }

    public static String ToBase64URLString(byte[] data, boolean padding) {
      if (data == null) {
        throw new NullPointerException("data");
      }
      return ToBase64String(data, 0, data.length, padding);
    }

    public static String ToBase64String(byte[] data, int offset, int count, boolean padding) {
      StringBuilder builder = new StringBuilder();
      ToBase64(builder, data, offset, count, Base64Classic, padding);
      return builder.toString();
    }

    public static String ToBase64URLString(byte[] data, int offset, int count, boolean padding) {
      StringBuilder builder = new StringBuilder();
      ToBase64(builder, data, offset, count, Base64Classic, padding);
      return builder.toString();
    }

    private static void ToBase64(StringBuilder str, byte[] data, int offset, int count, String alphabet, boolean padding) {
      if (data == null) {
        throw new NullPointerException("data");
      }
      if (offset < 0) {
        throw new IllegalArgumentException("offset (" + Integer.toString((int)offset) + ") is less than " + "0");
      }
      if (offset > data.length) {
        throw new IllegalArgumentException("offset (" + Integer.toString((int)offset) + ") is more than " + Integer.toString((int)data.length));
      }
      if (count < 0) {
        throw new IllegalArgumentException("count (" + Integer.toString((int)count) + ") is less than " + "0");
      }
      if (count > data.length) {
        throw new IllegalArgumentException("count (" + Integer.toString((int)count) + ") is more than " + Integer.toString((int)data.length));
      }
      if (data.length - offset < count) {
        throw new IllegalArgumentException("data's length minus " + offset + " (" + Integer.toString((int)(data.length - offset)) + ") is less than " + Integer.toString((int)count));
      }
      int length = offset + count;
      int i = offset;
      for (i = offset; i < (length - 2); i += 3) {
        str.append(alphabet.charAt((data[i] >> 2) & 63));
        str.append(alphabet.charAt(((data[i] & 3) << 4) + ((data[i + 1] >> 4) & 15)));
        str.append(alphabet.charAt(((data[i + 1] & 15) << 2) + ((data[i + 2] >> 6) & 3)));
        str.append(alphabet.charAt(data[i + 2] & 63));
      }
      int lenmod3 = count % 3;
      if (lenmod3 != 0) {
        i = length - lenmod3;
        str.append(alphabet.charAt((data[i] >> 2) & 63));
        if (lenmod3 == 2) {
          str.append(alphabet.charAt(((data[i] & 3) << 4) + ((data[i + 1] >> 4) & 15)));
          str.append(alphabet.charAt((data[i + 1] & 15) << 2));
          if (padding) {
            str.append("=");
          }
        } else {
          str.append(alphabet.charAt((data[i] & 3) << 4));
          if (padding) {
            str.append("==");
          }
        }
      }
    }
  }
