/*
Written in 2013-2018 by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Text;
using NUnit.Framework;

namespace Test {
  public static class TestCommon {
    private const string Digits = "0123456789";

    public static int StringToInt(string str) {
      var neg = false;
      var i = 0;
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (str.Length > 0 && str[0] == '-') {
        neg = true;
        ++i;
      }
      if (i == str.Length) {
        throw new FormatException();
      }
      var ret = 0;
      while (i < str.Length) {
        int c = str[i];
        ++i;
        if (c is >= '0' and <= '9') {
          int x = c - '0';
          if (ret > 214748364) {
            throw new FormatException();
          }
          ret *= 10;
          if (ret == 2147483640) {
            if (neg && x == 8) {
              return i != str.Length ? throw new FormatException() :
Int32.MinValue;
            }
            if (x > 7) {
              throw new FormatException();
            }
          }
          ret += x;
        } else {
          throw new FormatException();
        }
      }
      return neg ? -ret : ret;
    }

    public static long StringToLong(string str) {
      var neg = false;
      var i = 0;
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (str.Length > 0 && str[0] == '-') {
        neg = true;
        ++i;
      }
      if (i == str.Length) {
        throw new FormatException();
      }
      long ret = 0;
      while (i < str.Length) {
        int c = str[i];
        ++i;
        if (c is >= '0' and <= '9') {
          int x = c - '0';
          if (ret > 922337203685477580L) {
            throw new FormatException();
          }
          ret *= 10;
          if (ret == 9223372036854775800L) {
            if (neg && x == 8) {
              return i != str.Length ? throw new FormatException() :
Int64.MinValue;
            }
            if (x > 7) {
              throw new FormatException();
            }
          }
          ret += x;
        } else {
          throw new FormatException();
        }
      }
      return neg ? -ret : ret;
    }

    public static void AssertByteArraysEqual(byte[] arr1, byte[] arr2) {
      if (!ByteArraysEqual(arr1, arr2)) {
        Assert.Fail("Expected " + ToByteArrayString(arr1) + ",\ngot..... " +
          ToByteArrayString(arr2));
      }
    }

    public static void AssertByteArraysEqual(
      byte[] arr1,
      int offset,
      int length,
      byte[] arr2) {
      if (!ByteArraysEqual(
         arr1,
         offset,
         length,
         arr2,
         0,
         arr2 == null ? 0 : arr2.Length)) {
        Assert.Fail("Expected " + ToByteArrayString(arr1) + ",\ngot..... " +
          ToByteArrayString(arr2));
      }
    }

    public static void AssertByteArraysEqual(
      byte[] arr1,
      byte[] arr2,
      int offset2,
      int length2) {
      if (!ByteArraysEqual(
        arr1,
        0,
        arr1 == null ? 0 : arr1.Length,
        arr2,
        offset2,
        length2)) {
        Assert.Fail("Expected " + ToByteArrayString(arr1) + ",\ngot..... " +
          ToByteArrayString(arr2));
      }
    }

    public static void AssertByteArraysEqual(
      byte[] arr1,
      int offset,
      int length,
      byte[] arr2,
      int offset2,
      int length2) {
      if (!ByteArraysEqual(arr1, offset, length, arr2, offset2, length2)) {
        Assert.Fail("Expected " + ToByteArrayString(arr1) + ",\ngot..... " +
          ToByteArrayString(arr2));
      }
    }

    public static void AssertNotEqual(object o, object o2, string msg) {
      if (o == null) {
        throw new ArgumentNullException(nameof(o));
      }
      if (o.Equals(o2)) {
        string str = msg + "\r\n" + ObjectMessages(
          o,
          o2,
          "Unexpectedly equal");
        Assert.Fail(str);
      }
    }

    public static void AssertEquals(object o, object o2, string msg) {
      if (o == null) {
        throw new ArgumentNullException(nameof(o));
      }
      if (!o.Equals(o2)) {
        Assert.AreEqual(o, o2, msg);
      }
    }

    public static void AssertNotEqual(object o, object o2) {
      if (o == null) {
        throw new ArgumentNullException(nameof(o));
      }
      if (o.Equals(o2)) {
        string str = ObjectMessages(
          o,
          o2,
          "Unexpectedly equal");
        Assert.Fail(str);
      }
    }

    public static void AssertEquals(object o, object o2) {
      if (o == null) {
        throw new ArgumentNullException(nameof(o));
      }
      if (!o.Equals(o2)) {
        Assert.AreEqual(o, o2);
      }
    }

    public static void AssertEqualsHashCode(object o, object o2) {
      if (o == null) {
        throw new ArgumentNullException(nameof(o));
      }
      if (o.Equals(o2)) {
        if (o2 == null) {
          throw new ArgumentNullException(nameof(o2));
        }
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
        if (o2 == null) {
          throw new ArgumentNullException(nameof(o2));
        }
        if (o2.Equals(o)) {
          Assert.Fail(String.Empty + o + " does not equal " + o2 +
            " but not vice versa");
        }
        // At least check that GetHashCode doesn't throw
        try {
          _ = o.GetHashCode();
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        try {
          _ = o2.GetHashCode();
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
    }

    public static void CompareTestConsistency<T>(T o1, T o2, T o3) where T :
      IComparable<T>
    {
      if (o1 == null) {
        throw new ArgumentNullException(nameof(o1));
      }
      if (o2 == null) {
        throw new ArgumentNullException(nameof(o2));
      }
      if (o3 == null) {
        throw new ArgumentNullException(nameof(o3));
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

    public static void CompareTestNotEqual<T>(T o1, T o2) where T :
      IComparable<T>
    {
      if (CompareTestReciprocal(o1, o2) == 0) {
        Assert.Fail(ObjectMessages(
            o1,
            o2,
            "Unexpectedly equal: " + CompareTestReciprocal(o1, o2)));
      }
    }

    public static void CompareTestNotEqual<T>(T o1, T o2, string msg) where T :
      IComparable<T>
    {
      if (CompareTestReciprocal(o1, o2) == 0) {
        string str = msg + "\r\n" + ObjectMessages(
          o1,
          o2,
          "Unexpectedly equal: " + CompareTestReciprocal(o1, o2));
        Assert.Fail(str);
      }
    }

    public static void CompareTestEqual<T>(T o1, T o2) where T :
      IComparable<T>
    {
      if (CompareTestReciprocal(o1, o2) != 0) {
        Assert.Fail(ObjectMessages(
            o1,
            o2,
            "Not equal: " + CompareTestReciprocal(o1, o2)));
      }
    }

    public static void CompareTestEqual<T>(T o1, T o2, string msg) where T :
      IComparable<T>
    {
      if (CompareTestReciprocal(o1, o2) != 0) {
        string str = msg + "\r\n" + ObjectMessages(
          o1,
          o2,
          "Not equal: " + CompareTestReciprocal(o1, o2));
        Assert.Fail(str);
      }
    }

    public static void CompareTestEqualAndConsistent<T>(T o1, T o2) where T :
      IComparable<T>
    {
      CompareTestEqualAndConsistent(o1, o2, null);
    }

    public static void CompareTestEqualAndConsistent<T>(
      T o1,
      T o2,
      string msg) where T : IComparable<T>
    {
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
      IComparable<T>
    {
      CompareTestLess(o2, o1);
    }

    public static void CompareTestLess<T>(T o1, T o2) where T :
      IComparable<T>
    {
      if (CompareTestReciprocal(o1, o2) >= 0) {
        Assert.Fail(ObjectMessages(
            o1,
            o2,
            "Not less: " + CompareTestReciprocal(o1, o2)));
      }
    }

    public static void CompareTestGreaterEqual<T>(T o1, T o2) where T :
      IComparable<T>
    {
      CompareTestLessEqual(o2, o1);
    }

    public static void CompareTestLessEqual<T>(T o1, T o2) where T :
      IComparable<T>
    {
      if (CompareTestReciprocal(o1, o2) > 0) {
        Assert.Fail(ObjectMessages(
            o1,
            o2,
            "Not less or equal: " + CompareTestReciprocal(o1, o2)));
      }
    }

    public static void CompareTestLess<T>(T o1, T o2, string msg) where T :
      IComparable<T>
    {
      if (CompareTestReciprocal(o1, o2) >= 0) {
        string str = msg + "\r\n" + ObjectMessages(
          o1,
          o2,
          "Not less: " + CompareTestReciprocal(o1, o2));
        Assert.Fail(str);
      }
    }

    public static void CompareTestLessEqual<T>(T o1, T o2, string msg)
    where T : IComparable<T>
    {
      if (CompareTestReciprocal(o1, o2) > 0) {
        string str = msg + "\r\n" + ObjectMessages(
          o1,
          o2,
          "Not less or equal: " + CompareTestReciprocal(o1, o2));
        Assert.Fail(str);
      }
    }

    public static void CompareTestGreater<T>(T o1, T o2, string msg) where T :
      IComparable<T>
    {
      if (CompareTestReciprocal(o1, o2) <= 0) {
        string str = msg + "\r\n" + ObjectMessages(
          o1,
          o2,
          "Not greater: " + CompareTestReciprocal(o1, o2));
        Assert.Fail(str);
      }
    }

    public static void CompareTestGreaterEqual<T>(T o1, T o2, string msg) where
    T : IComparable<T>
    {
      if (CompareTestReciprocal(o1, o2) < 0) {
        string str = msg + "\r\n" + ObjectMessages(
          o1,
          o2,
          "Not greater or equal: " + CompareTestReciprocal(o1, o2));
        Assert.Fail(str);
      }
    }

    public static int CompareTestReciprocal<T>(T o1, T o2) where T :
      IComparable<T>
    {
      if (o1 == null) {
        throw new ArgumentNullException(nameof(o1));
      }
      if (o2 == null) {
        throw new ArgumentNullException(nameof(o2));
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
      IComparable<T>
    {
      if (o1 == null) {
        throw new ArgumentNullException(nameof(o1));
      }
      if (o2 == null) {
        throw new ArgumentNullException(nameof(o2));
      }
      if (o3 == null) {
        throw new ArgumentNullException(nameof(o3));
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
      if (value == 0) {
        return "0";
      }
      if (value == Int32.MinValue) {
        return "-2147483648";
      }
      bool neg = value < 0;
      if (neg) {
        value = -value;
      }
      char[] chars;
      int count;
      if (value < 100000) {
        if (neg) {
          chars = new char[6];
          count = 5;
        } else {
          chars = new char[5];
          count = 4;
        }
        while (value > 9) {
          int intdivvalue = unchecked((((value >> 1) * 52429) >> 18) & 16383);
          char digit = Digits[value - (intdivvalue * 10)];
          chars[count--] = digit;
          value = intdivvalue;
        }
        if (value != 0) {
          chars[count--] = Digits[value];
        }
        if (neg) {
          chars[count] = '-';
        } else {
          ++count;
        }
        return new String(chars, count, chars.Length - count);
      }
      chars = new char[12];
      count = 11;
      while (value >= 163840) {
        int intdivvalue = value / 10;
        char digit = Digits[value - (intdivvalue * 10)];
        chars[count--] = digit;
        value = intdivvalue;
      }
      while (value > 9) {
        int intdivvalue = unchecked((((value >> 1) * 52429) >> 18) & 16383);
        char digit = Digits[value - (intdivvalue * 10)];
        chars[count--] = digit;
        value = intdivvalue;
      }
      if (value != 0) {
        chars[count--] = Digits[value];
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
      bool neg = longValue < 0;
      char[] chars;
      int intlongValue = unchecked((int)longValue);
      if (intlongValue == longValue) {
        return IntToString(intlongValue);
      } else {
        chars = new char[24];
        var count = 23;
        if (neg) {
          longValue = -longValue;
        }
        while (longValue >= 163840) {
          long divValue = longValue / 10;
          char digit = Digits[(int)(longValue - (divValue * 10))];
          chars[count--] = digit;
          longValue = divValue;
        }
        while (longValue > 9) {
          long divValue = unchecked((((longValue >> 1) * 52429) >> 18) & 16383);
          char digit = Digits[(int)(longValue - (divValue * 10))];
          chars[count--] = digit;
          longValue = divValue;
        }
        if (longValue != 0) {
          chars[count--] = Digits[(int)longValue];
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
      string s) {
      return s + ":\n" + o1 + " and\n" + o2;
    }

    public static string ObjectMessages(
      object o1,
      object o2,
      object o3,
      string s) {
      return s + ":\n" + o1 + " and\n" + o2 + " and\n" + o3;
    }

    private const int RepeatDivideThreshold = 10000;

    public static string Repeat(char c, int num) {
      if (num < 0) {
        throw new ArgumentException("num (" + num +
           ") is not greater or equal to 0");
      }
      var sb = new StringBuilder(num);
      if (num > RepeatDivideThreshold) {
        string sb2 = Repeat(c, RepeatDivideThreshold);
        int count = num / RepeatDivideThreshold;
        int rem = num % RepeatDivideThreshold;
        for (int i = 0; i < count; ++i) {
          _ = sb.Append(sb2);
        }
        for (int i = 0; i < rem; ++i) {
          _ = sb.Append(c);
        }
      } else {
        for (int i = 0; i < num; ++i) {
          _ = sb.Append(c);
        }
      }
      return sb.ToString();
    }

    public static string Repeat(string str, int num) {
      if (num < 0) {
        throw new ArgumentException("num (" + num +
           ") is not greater or equal to 0");
      }
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (str.Length == 1) {
        return Repeat(str[0], num);
      }
      var sb = new StringBuilder(num * str.Length);
      for (int i = 0; i < num; ++i) {
        _ = sb.Append(str);
      }
      return sb.ToString();
    }

    public static string ToByteArrayString(byte[] bytes) {
      return (bytes == null) ? "null" : ToByteArrayString(
         bytes,
         0,
         bytes.Length);
    }

    public static string ToByteArrayString(
       byte[] bytes,
       int offset,
       int length) {
      if (bytes == null) {
        return "null";
      }
      if (bytes == null) {
        throw new ArgumentNullException(nameof(bytes));
      }
      if (offset < 0) {
        throw new ArgumentException("\"offset\" (" + offset + ") is not" +
"\u0020greater or equal to 0");
      }
      if (offset > bytes.Length) {
        throw new ArgumentException("\"offset\" (" + offset + ") is not less" +
"\u0020or equal to " + bytes.Length);
      }
      if (length < 0) {
        throw new ArgumentException(" (" + length + ") is not greater or" +
"\u0020equal to 0");
      }
      if (length > bytes.Length) {
        throw new ArgumentException(" (" + length + ") is not less or equal" +
"\u0020to " + bytes.Length);
      }
      if (bytes.Length - offset < length) {
        throw new ArgumentException("\"bytes\" + \"'s length minus \" +" +
"\u0020offset (" + (bytes.Length - offset) + ") is not greater or equal to " +
length);
      }
      var sb = new System.Text.StringBuilder();
      const string ValueHex = "0123456789ABCDEF";
      _ = sb.Append("new byte[] { ");
      for (int i = 0; i < length; ++i) {
        if (i > 0) {
          _ = sb.Append(',');
        }
        if ((bytes[offset + i] & 0x80) != 0) {
          sb.Append("(byte)");
        }
        sb.Append("0x");
        sb.Append(ValueHex[(bytes[offset + i] >> 4) & 0xf]);
        sb.Append(ValueHex[bytes[offset + i] & 0xf]);
      }
      _ = sb.Append('}');
      return sb.ToString();
    }

    private static bool ByteArraysEqual(
      byte[] arr1,
      int offset,
      int length,
      byte[] arr2,
      int offset2,
      int length2) {
      if (arr1 == null) {
        return arr2 == null;
      }
      if (arr2 == null) {
        return false;
      }
      if (offset < 0) {
        throw new ArgumentException("\"offset\" (" + offset + ") is not" +
"\u0020greater or equal to 0");
      }
      if (offset > arr1.Length) {
        throw new ArgumentException("\"offset\" (" + offset + ") is not less" +
"\u0020or equal to " + arr1.Length);
      }
      if (length < 0) {
        throw new ArgumentException(" (" + length + ") is not greater or" +
"\u0020equal to 0");
      }
      if (length > arr1.Length) {
        throw new ArgumentException(" (" + length + ") is not less or equal" +
"\u0020to " + arr1.Length);
      }
      if (arr1.Length - offset < length) {
        throw new ArgumentException("\"arr1\" + \"'s length minus \" +" +
"\u0020offset (" + (arr1.Length - offset) + ") is not greater or equal to " +
length);
      }
      if (arr2 == null) {
        throw new ArgumentNullException(nameof(arr2));
      }
      if (offset2 < 0) {
        throw new ArgumentException("\"offset2\" (" + offset2 + ") is not" +
"\u0020greater or equal to 0");
      }
      if (offset2 > arr2.Length) {
        throw new ArgumentException("\"offset2\" (" + offset2 + ") is not" +
"\u0020less or equal to " + arr2.Length);
      }
      if (length2 < 0) {
        throw new ArgumentException(" (" + length2 + ") is not greater or" +
"\u0020equal to 0");
      }
      if (length2 > arr2.Length) {
        throw new ArgumentException(" (" + length2 + ") is not less or equal" +
"\u0020to " + arr2.Length);
      }
      if (arr2.Length - offset2 < length2) {
        throw new ArgumentException("\"arr2\"'s length minus " +
"\u0020offset2 (" + (arr2.Length - offset2) + ") is not greater or equal to " +
length2);
      }
      if (length != length2) {
        return false;
      }
      for (int i = 0; i < length; ++i) {
        if (arr1[offset + i] != arr2[offset2 + i]) {
          return false;
        }
      }
      return true;
    }

    public static bool ByteArraysEqual(byte[] arr1, byte[] arr2) {
      if (arr1 == null) {
        return arr2 == null;
      }
      if (arr2 == null) {
        return false;
      }
      if (arr1.Length != arr2.Length) {
        return false;
      }
      for (int i = 0; i < arr1.Length; ++i) {
        if (arr1[i] != arr2[i]) {
          return false;
        }
      }
      return true;
    }
  }
}
