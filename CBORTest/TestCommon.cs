/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using PeterO;
using PeterO.Cbor;

namespace Test {
  public static class TestCommon {
    private static readonly string ValueDigits = "0123456789";

    public static void AssertByteArraysEqual(byte[] arr1, byte[] arr2) {
      if (!ByteArraysEqual(arr1, arr2)) {
        Assert.Fail("Expected " + ToByteArrayString(arr1) + ", got " +
          ToByteArrayString(arr2));
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

    public static void AssertRoundTrip(CBORObject o) {
      CBORObject o2 = FromBytesTestAB(o.EncodeToBytes());
      CompareTestEqual(o, o2);
      TestNumber(o);
      AssertEqualsHashCode(o, o2);
    }

    public static void AssertSer(CBORObject o, String s) {
      if (!s.Equals(o.ToString())) {
        Assert.AreEqual(s, o.ToString(), "o is not equal to s");
      }
      // Test round-tripping
      CBORObject o2 = FromBytesTestAB(o.EncodeToBytes());
      if (!s.Equals(o2.ToString())) {
        Assert.AreEqual(s, o2.ToString(), "o2 is not equal to s");
      }
      TestNumber(o);
      AssertEqualsHashCode(o, o2);
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

    public static int CompareTestReciprocal<T>(T o1, T o2) where T :
      IComparable<T> {
      if (o1 == null) {
        throw new ArgumentNullException("o1");
      }
      if (o2 == null) {
        throw new ArgumentNullException("o2");
      }
      int cmp = Math.Sign(o1.CompareTo(o2));
      int cmp2 = Math.Sign(o2.CompareTo(o1));
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
    // Tests the equivalence of the FromBytes and Read methods.
    public static CBORObject FromBytesTestAB(byte[] b) {
      CBORObject oa = FromBytesA(b);
      CBORObject ob = FromBytesB(b);
      if (!oa.Equals(ob)) {
        Assert.AreEqual(oa, ob);
      }
      return oa;
    }

    public static string IntToString(int value) {
      if (value == Int32.MinValue) {
        return "-2147483648";
      }
      if (value == 0) {
        return "0";
      }
      bool neg = value < 0;
      var chars = new char[24];
      var count = 0;
      if (neg) {
        chars[0] = '-';
        ++count;
        value = -value;
      }
      while (value != 0) {
        int intdivvalue = value / 10;
        char digit = ValueDigits[(int)(value - (intdivvalue * 10))];
        chars[count++] = digit;
        value = intdivvalue;
      }
      if (neg) {
        ReverseChars(chars, 1, count - 1);
      } else {
        ReverseChars(chars, 0, count);
      }
      return new String(chars, 0, count);
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
        if (neg) {
          chars[0] = '-';
          ++count;
          intlongValue = -intlongValue;
        }
        while (intlongValue != 0) {
          int intdivlongValue = intlongValue / 10;
        char digit = ValueDigits[(int)(intlongValue - (intdivlongValue *
            10))];
          chars[count++] = digit;
          intlongValue = intdivlongValue;
        }
      } else {
        chars = new char[24];
        if (neg) {
          chars[0] = '-';
          ++count;
          longValue = -longValue;
        }
        while (longValue != 0) {
          long divlongValue = longValue / 10;
          char digit = ValueDigits[(int)(longValue - (divlongValue * 10))];
          chars[count++] = digit;
          longValue = divlongValue;
        }
      }
      if (neg) {
        ReverseChars(chars, 1, count - 1);
      } else {
        ReverseChars(chars, 0, count);
      }
      return new String(chars, 0, count);
    }

    public static string ObjectMessages(
      object o1,
      object o2,
      String s) {
      var co1 = o1 as CBORObject;
      var co2 = o2 as CBORObject;
      return (co1 != null) ? TestCommon.ObjectMessages(co1, co2, s) : (s +
        ":\n" + o1 + " and\n" + o2);
    }

    public static string ObjectMessages(
      CBORObject o1,
      CBORObject o2,
      String s) {
      if (o1.Type == CBORType.Number && o2.Type == CBORType.Number) {
        return s + ":\n" + o1 + " and\n" + o2 + "\nOR\n" +
          o1.AsExtendedDecimal() + " and\n" + o2.AsExtendedDecimal() +
       "\nOR\n" + "AddSubCompare(" + TestCommon.ToByteArrayString(o1) + ",\n" +
          TestCommon.ToByteArrayString(o2) + ");";
      }
      return s + ":\n" + o1 + " and\n" + o2 + "\nOR\n" +
TestCommon.ToByteArrayString(o1) + " and\n" + TestCommon.ToByteArrayString(o2);
    }

    public static string ObjectMessages(
      object o1,
      object o2,
      object o3,
      String s) {
      var co1 = o1 as CBORObject;
      var co2 = o2 as CBORObject;
      var co3 = o3 as CBORObject;
      return (co1 != null) ? TestCommon.ObjectMessages(co1, co2, co3, s) :
        (s + ":\n" + o1 + " and\n" + o2 + " and\n" + o3);
    }

    public static string ObjectMessages(
      CBORObject o1,
      CBORObject o2,
      CBORObject o3,
      String s) {
      return s + ":\n" + o1 + " and\n" + o2 + " and\n" + o3 + "\nOR\n" +
TestCommon.ToByteArrayString(o1) + " and\n" + TestCommon.ToByteArrayString(o2) +
 " and\n" + TestCommon.ToByteArrayString(o3);
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

    public static void TestNumber(CBORObject o) {
      if (o.Type != CBORType.Number) {
        return;
      }
      if (o.IsPositiveInfinity() || o.IsNegativeInfinity() ||
          o.IsNaN()) {
        try {
          o.AsByte();
          Assert.Fail("Should have failed");
        } catch (OverflowException) {
          Console.Write(String.Empty);
        } catch (Exception ex) {
          Assert.Fail("Object: " + o + ", " + ex); throw new
            InvalidOperationException(String.Empty, ex);
        }
        try {
          o.AsInt16();
          Assert.Fail("Should have failed");
        } catch (OverflowException) {
          Console.Write(String.Empty);
        } catch (Exception ex) {
          Assert.Fail("Object: " + o + ", " + ex); throw new
            InvalidOperationException(String.Empty, ex);
        }
        try {
          o.AsInt32();
          Assert.Fail("Should have failed");
        } catch (OverflowException) {
          Console.Write(String.Empty);
        } catch (Exception ex) {
          Assert.Fail("Object: " + o + ", " + ex); throw new
            InvalidOperationException(String.Empty, ex);
        }
        try {
          o.AsInt64();
          Assert.Fail("Should have failed");
        } catch (OverflowException) {
          Console.Write(String.Empty);
        } catch (Exception ex) {
          Assert.Fail("Object: " + o + ", " + ex); throw new
            InvalidOperationException(String.Empty, ex);
        }
        try {
          o.AsSingle();
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        try {
          o.AsDouble();
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        try {
          o.AsBigInteger();
          Assert.Fail("Should have failed");
        } catch (OverflowException) {
          Console.Write(String.Empty);
        } catch (Exception ex) {
          Assert.Fail("Object: " + o + ", " + ex); throw new
            InvalidOperationException(String.Empty, ex);
        }
        return;
      }
      try {
        o.AsSingle();
      } catch (Exception ex) {
        Assert.Fail("Object: " + o + ", " + ex); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        o.AsDouble();
      } catch (Exception ex) {
        Assert.Fail("Object: " + o + ", " + ex); throw new
          InvalidOperationException(String.Empty, ex);
      }
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

    public static string ToByteArrayString(CBORObject obj) {
      return new System.Text.StringBuilder()
        .Append("CBORObject.DecodeFromBytes(")
           .Append(ToByteArrayString(obj.EncodeToBytes()))
           .Append(")").ToString();
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

    private static CBORObject FromBytesA(byte[] b) {
      return CBORObject.DecodeFromBytes(b);
    }

    private static CBORObject FromBytesB(byte[] b) {
      using (var ms = new System.IO.MemoryStream(b)) {
        CBORObject o = CBORObject.Read(ms);
        if (ms.Position != ms.Length) {
          throw new CBORException("not at EOF");
        }
        return o;
      }
    }

    private static void ReverseChars(char[] chars, int offset, int length) {
      int half = length >> 1;
      int right = offset + length - 1;
      for (var i = 0; i < half; i++, right--) {
        char value = chars[offset + i];
        chars[offset + i] = chars[right];
        chars[right] = value;
      }
    }
  }
}
