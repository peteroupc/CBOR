/*
Written in 2013-2018 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Text;
using NUnit.Framework;

namespace Test {
  public static class TestCommon {
    private static readonly string ValueDigits = "0123456789";

    public static void AssertByteArraysEqual(byte[] arr1, byte[] arr2) {
      if (!ByteArraysEqual(arr1, arr2)) {
        Assert.Fail("Expected " + ToByteArrayString(arr1) + ",\ngot..... " +
          ToByteArrayString(arr2));
      }
    }

    public static void AssertEquals(Object o, Object o2) {
      if (!o.Equals(o2)) {
         Assert.AreEqual(o, o2);
      }
    }

    public static void AssertEqualsHashCode(Object o, Object o2) {
      if (o.Equals(o2)) {
        if (!o2.Equals(o)) {
          Assert.Fail(
  String.Empty + o + " equals " + o2 + " but not vice versa");
        }
        // Test for the guarantee that equal objects
        // must have equal hash codes
        if (o2.GetHashCode() != o.GetHashCode()) {
          // Don't use Assert.AreEqual directly because it has
          // quite a lot of overhead
          Assert.Fail(
  String.Empty + o + " and " + o2 + " don't have equal hash codes");
        }
      } else {
        if (o2.Equals(o)) {
          Assert.Fail(String.Empty + o + " does not equal " + o2 +
            " but not vice versa");
        }
        // At least check that GetHashCode doesn't throw
        try {
          o.GetHashCode();
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        try {
          o2.GetHashCode();
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
    }

    public static void CompareTestConsistency<T>(T o1, T o2, T o3) where T :
      IComparable<T> {
      if (o1 == null) {
        throw new ArgumentNullException("o1");
      }
      if (o2 == null) {
        throw new ArgumentNullException("o2");
      }
      if (o3 == null) {
        throw new ArgumentNullException("o3");
      }
      int cmp = CompareTestReciprocal(o1, o2);
      int cmp2 = CompareTestReciprocal(o2, o3);
      int cmp3 = CompareTestReciprocal(o1, o3);
      Assert.AreEqual(cmp == 0, o1.Equals(o2));
      Assert.AreEqual(cmp == 0, o2.Equals(o1));
      Assert.AreEqual(cmp2 == 0, o2.Equals(o3));
      Assert.AreEqual(cmp2 == 0, o3.Equals(o2));
      Assert.AreEqual(cmp3 == 0, o1.Equals(o3));
      Assert.AreEqual(cmp3 == 0, o3.Equals(o1));
    }

    public static void CompareTestEqual<T>(T o1, T o2) where T :
        IComparable<T> {
      if (CompareTestReciprocal(o1, o2) != 0) {
        Assert.Fail(ObjectMessages(
          o1,
          o2,
          "Not equal: " + CompareTestReciprocal(o1, o2)));
      }
    }

    public static void CompareTestEqual<T>(T o1, T o2, string msg) where T :
        IComparable<T> {
      if (CompareTestReciprocal(o1, o2) != 0) {
        string str = msg + "\r\n" + ObjectMessages(
          o1,
          o2,
          "Not equal: " + CompareTestReciprocal(o1, o2));
        Assert.Fail(str);
      }
    }

    public static void CompareTestEqualAndConsistent<T>(T o1, T o2) where T :
    IComparable<T> {
      CompareTestEqualAndConsistent(o1, o2, null);
    }

    public static void CompareTestEqualAndConsistent<T>(
  T o1,
  T o2,
  string msg) where T :
    IComparable<T> {
      if (CompareTestReciprocal(o1, o2) != 0) {
        msg = (msg == null ? String.Empty : (msg + "\r\n")) +
          "Not equal: " + CompareTestReciprocal(o1, o2);
        Assert.Fail(ObjectMessages(
          o1,
          o2,
          msg));
      }
      if (!o1.Equals(o2)) {
        msg = (msg == null ? String.Empty : (msg + "\r\n")) +
          "Not equal: " + CompareTestReciprocal(o1, o2);
        Assert.Fail(ObjectMessages(
          o1,
          o2,
          msg));
      }
    }

    public static void CompareTestGreater<T>(T o1, T o2) where T :
          IComparable<T> {
      CompareTestLess(o2, o1);
    }

    public static void CompareTestLess<T>(T o1, T o2) where T : IComparable<T> {
      if (CompareTestReciprocal(o1, o2) >= 0) {
        Assert.Fail(ObjectMessages(
          o1,
          o2,
          "Not less: " + CompareTestReciprocal(o1, o2)));
      }
    }

    public static void CompareTestGreaterEqual<T>(T o1, T o2) where T :
          IComparable<T> {
      CompareTestLessEqual(o2, o1);
    }

    public static void CompareTestLessEqual<T>(T o1, T o2) where T :
      IComparable<T> {
      if (CompareTestReciprocal(o1, o2) > 0) {
        Assert.Fail(ObjectMessages(
          o1,
          o2,
          "Not less or equal: " + CompareTestReciprocal(o1, o2)));
      }
    }

    public static int CompareTestReciprocal<T>(T o1, T o2) where T :
      IComparable<T> {
      if (o1 == null) {
        throw new ArgumentNullException("o1");
      }
      if (o2 == null) {
        throw new ArgumentNullException("o2");
      }
      int cmp, cmp2;
      cmp = Math.Sign(o1.CompareTo(o2));
      cmp2 = Math.Sign(o2.CompareTo(o1));
      if (-cmp2 != cmp) {
        Assert.AreEqual(cmp, -cmp2, ObjectMessages(o1, o2, "Not reciprocal"));
      }
      return cmp;
    }

    public static void CompareTestRelations<T>(T o1, T o2, T o3) where T :
      IComparable<T> {
      if (o1 == null) {
        throw new ArgumentNullException("o1");
      }
      if (o2 == null) {
        throw new ArgumentNullException("o2");
      }
      if (o3 == null) {
        throw new ArgumentNullException("o3");
      }
      if (o1.CompareTo(o1) != 0) {
        Assert.Fail(o1.ToString());
      }
      if (o2.CompareTo(o2) != 0) {
        Assert.Fail(o2.ToString());
      }
      if (o3.CompareTo(o3) != 0) {
        Assert.Fail(o3.ToString());
      }
      int cmp12 = CompareTestReciprocal(o1, o2);
      int cmp23 = CompareTestReciprocal(o2, o3);
      int cmp13 = CompareTestReciprocal(o1, o3);
      // CompareTestReciprocal tests CompareTo both
      // ways, so shortcutting by negating the values
      // is allowed here
      int cmp21 = -cmp12;
      int cmp32 = -cmp23;
      int cmp31 = -cmp13;
      // Transitivity checks
      for (int i = -1; i <= 1; ++i) {
        if (cmp12 == i) {
          if (cmp23 == i && cmp13 != i) {
            Assert.Fail(ObjectMessages(o1, o2, o3, "Not transitive"));
          }
        }
        if (cmp23 == i) {
          if (cmp31 == i && cmp21 != i) {
            Assert.Fail(ObjectMessages(o1, o2, o3, "Not transitive"));
          }
        }
        if (cmp31 == i) {
          if (cmp12 == i && cmp32 != i) {
            Assert.Fail(ObjectMessages(o1, o2, o3, "Not transitive"));
          }
        }
      }
    }

    public static string IntToString(int value) {
      if (value == Int32.MinValue) {
        return "-2147483648";
      }
      if (value == 0) {
        return "0";
      }
      bool neg = value < 0;
      var chars = new char[12];
      var count = 11;
      if (neg) {
        value = -value;
      }
      while (value > 43698) {
        int intdivvalue = value / 10;
        char digit = ValueDigits[(int)(value - (intdivvalue * 10))];
        chars[count--] = digit;
        value = intdivvalue;
      }
      while (value > 9) {
        int intdivvalue = (value * 26215) >> 18;
        char digit = ValueDigits[(int)(value - (intdivvalue * 10))];
        chars[count--] = digit;
        value = intdivvalue;
      }
      if (value != 0) {
        chars[count--] = ValueDigits[(int)value];
      }
      if (neg) {
        chars[count] = '-';
      } else {
        ++count;
      }
      return new String(chars, count, 12 - count);
    }

    public static string LongToString(long longValue) {
      if (longValue == Int64.MinValue) {
        return "-9223372036854775808";
      }
      if (longValue == 0L) {
        return "0";
      }
      if (longValue == (long)Int32.MinValue) {
        return "-2147483648";
      }
      bool neg = longValue < 0;
      var count = 0;
      char[] chars;
      int intlongValue = unchecked((int)longValue);
      if ((long)intlongValue == longValue) {
        chars = new char[12];
        count = 11;
        if (neg) {
          intlongValue = -intlongValue;
        }
        while (intlongValue > 43698) {
          int intdivValue = intlongValue / 10;
        char digit = ValueDigits[(int)(intlongValue - (intdivValue * 10))];
        chars[count--] = digit;
        intlongValue = intdivValue;
      }
      while (intlongValue > 9) {
        int intdivValue = (intlongValue * 26215) >> 18;
        char digit = ValueDigits[(int)(intlongValue - (intdivValue * 10))];
        chars[count--] = digit;
        intlongValue = intdivValue;
      }
      if (intlongValue != 0) {
        chars[count--] = ValueDigits[(int)intlongValue];
      }
      if (neg) {
        chars[count] = '-';
      } else {
        ++count;
      }
      return new String(chars, count, 12 - count);
      } else {
        chars = new char[24];
        count = 23;
        if (neg) {
          longValue = -longValue;
        }
        while (longValue > 43698) {
          long divValue = longValue / 10;
        char digit = ValueDigits[(int)(longValue - (divValue * 10))];
        chars[count--] = digit;
        longValue = divValue;
      }
      while (longValue > 9) {
        long divValue = (longValue * 26215) >> 18;
        char digit = ValueDigits[(int)(longValue - (divValue * 10))];
        chars[count--] = digit;
        longValue = divValue;
      }
      if (longValue != 0) {
        chars[count--] = ValueDigits[(int)longValue];
      }
      if (neg) {
        chars[count] = '-';
      } else {
        ++count;
      }
      return new String(chars, count, 24 - count);
      }
    }

    public static string ObjectMessages(
      object o1,
      object o2,
      String s) {
      return s + ":\n" + o1 + " and\n" + o2;
    }

    public static string ObjectMessages(
      object o1,
      object o2,
      object o3,
      String s) {
      return s + ":\n" + o1 + " and\n" + o2 + " and\n" + o3;
    }

    public static String Repeat(char c, int num) {
      var sb = new StringBuilder();
      for (var i = 0; i < num; ++i) {
        sb.Append(c);
      }
      return sb.ToString();
    }

    public static String Repeat(String c, int num) {
      var sb = new StringBuilder();
      for (var i = 0; i < num; ++i) {
        sb.Append(c);
      }
      return sb.ToString();
    }

    public static string ToByteArrayString(byte[] bytes) {
      if (bytes == null) {
        return "null";
      }
      var sb = new System.Text.StringBuilder();
      const string ValueHex = "0123456789ABCDEF";
      sb.Append("new byte[] { ");
      for (var i = 0; i < bytes.Length; ++i) {
        if (i > 0) {
          sb.Append(","); }
        if ((bytes[i] & 0x80) != 0) {
          sb.Append("(byte)0x");
        } else {
          sb.Append("0x");
        }
        sb.Append(ValueHex[(bytes[i] >> 4) & 0xf]);
        sb.Append(ValueHex[bytes[i] & 0xf]);
      }
      sb.Append("}");
      return sb.ToString();
    }

    private static bool ByteArraysEqual(byte[] arr1, byte[] arr2) {
      if (arr1 == null) {
        return arr2 == null;
      }
      if (arr2 == null) {
        return false;
      }
      if (arr1.Length != arr2.Length) {
        return false;
      }
      for (var i = 0; i < arr1.Length; ++i) {
        if (arr1[i] != arr2[i]) {
          return false;
        }
      }
      return true;
    }
  }
}
