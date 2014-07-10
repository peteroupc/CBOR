/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeterO;
using PeterO.Cbor;

namespace Test {
    /// <summary>Contains CBOR tests.</summary>
    /// <returns/>
  [TestClass]
  public class CBORTest {
    private static void TestExtendedFloatDoubleCore(double d, string s) {
      double oldd = d;
      ExtendedFloat bf = ExtendedFloat.FromDouble(d);
      if (s != null) {
        Assert.AreEqual(s, bf.ToString());
      }
      d = bf.ToDouble();
      Assert.AreEqual((double)oldd, d);
      Assert.IsTrue(CBORObject.FromObject(bf).CanFitInDouble());
      TestCommon.AssertRoundTrip(CBORObject.FromObject(bf));
      TestCommon.AssertRoundTrip(CBORObject.FromObject(d));
    }

    private static void TestExtendedFloatSingleCore(float d, string s) {
      float oldd = d;
      ExtendedFloat bf = ExtendedFloat.FromSingle(d);
      if (s != null) {
        Assert.AreEqual(s, bf.ToString());
      }
      d = bf.ToSingle();
      Assert.AreEqual((float)oldd, d);
      Assert.IsTrue(CBORObject.FromObject(bf).CanFitInSingle());
      TestCommon.AssertRoundTrip(CBORObject.FromObject(bf));
      TestCommon.AssertRoundTrip(CBORObject.FromObject(d));
    }

    public static CBORObject RandomNumber(FastRandom rand) {
      switch (rand.NextValue(6)) {
        case 0:
          return CBORObject.FromObject(RandomDouble(rand, Int32.MaxValue));
        case 1:
          return CBORObject.FromObject(RandomSingle(rand, Int32.MaxValue));
        case 2:
          return CBORObject.FromObject(RandomBigInteger(rand));
        case 3:
          return CBORObject.FromObject(RandomExtendedFloat(rand));
        case 4:
          return CBORObject.FromObject(RandomExtendedDecimal(rand));
        case 5:
          return CBORObject.FromObject(RandomInt64(rand));
        default:
          throw new ArgumentException();
      }
    }

    public static CBORObject RandomNumberOrRational(FastRandom rand) {
      switch (rand.NextValue(7)) {
        case 0:
          return CBORObject.FromObject(RandomDouble(rand, Int32.MaxValue));
        case 1:
          return CBORObject.FromObject(RandomSingle(rand, Int32.MaxValue));
        case 2:
          return CBORObject.FromObject(RandomBigInteger(rand));
        case 3:
          return CBORObject.FromObject(RandomExtendedFloat(rand));
        case 4:
          return CBORObject.FromObject(RandomExtendedDecimal(rand));
        case 5:
          return CBORObject.FromObject(RandomInt64(rand));
        case 6:
          return CBORObject.FromObject(RandomRational(rand));
        default:
          throw new ArgumentException();
      }
    }

    private static CBORObject RandomCBORByteString(FastRandom rand) {
      int x = rand.NextValue(0x2000);
      var bytes = new byte[x];
      for (var i = 0; i < x; ++i) {
        bytes[i] = unchecked((byte)rand.NextValue(256));
      }
      return CBORObject.FromObject(bytes);
    }

    private static CBORObject RandomCBORByteStringShort(FastRandom rand) {
      int x = rand.NextValue(50);
      var bytes = new byte[x];
      for (var i = 0; i < x; ++i) {
        bytes[i] = unchecked((byte)rand.NextValue(256));
      }
      return CBORObject.FromObject(bytes);
    }

    private static CBORObject RandomCBORTextString(FastRandom rand) {
      int length = rand.NextValue(0x2000);
      var sb = new StringBuilder();
      for (var i = 0; i < length; ++i) {
        int x = rand.NextValue(100);
        if (x < 95) {
          // ASCII
          sb.Append((char)(0x20 + rand.NextValue(0x60)));
        } else if (x < 98) {
          // Supplementary character
          x = rand.NextValue(0x400) + 0xd800;
          sb.Append((char)x);
          x = rand.NextValue(0x400) + 0xdc00;
          sb.Append((char)x);
        } else {
          // BMP character
          x = 0x20 + rand.NextValue(0xffe0);
          if (x >= 0xd800 && x < 0xe000) {
            // surrogate code unit, generate ASCII instead
            x = 0x20 + rand.NextValue(0x60);
          }
          sb.Append((char)x);
        }
      }
      return CBORObject.FromObject(sb.ToString());
    }

    private static CBORObject RandomCBORMap(FastRandom rand, int depth) {
      int x = rand.NextValue(100);
      int count = 0;
      count = (x < 80) ? 2 : ((x < 93) ? 1 : ((x < 98) ? 0 : 10));
      CBORObject cborRet = CBORObject.NewMap();
      for (var i = 0; i < count; ++i) {
        CBORObject key = RandomCBORObject(rand, depth + 1);
        CBORObject value = RandomCBORObject(rand, depth + 1);
        cborRet[key] = value;
      }
      return cborRet;
    }

    private static CBORObject RandomCBORTaggedObject(
      FastRandom rand,
      int depth) {
      int tag = 0;
      if (rand.NextValue(2) == 0) {
        int[] tagselection = { 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 30, 30,
          30, 0, 1, 25, 26, 27 };
        tag = tagselection[rand.NextValue(tagselection.Length)];
      } else {
        tag = rand.NextValue(0x1000000);
      }
      if (tag == 25) {
        tag = 0;
      }
      if (tag == 30) {
        return RandomCBORByteString(rand);
      }
      for (var i = 0; i < 15; ++i) {
        CBORObject o;
        // Console.WriteLine("tag "+tag+" "+i);
        if (tag == 0 || tag == 1 || tag == 28 || tag == 29) {
          tag = 999;
        }
        if (tag == 2 || tag == 3) {
          o = RandomCBORByteStringShort(rand);
        } else if (tag == 4 || tag == 5) {
          o = CBORObject.NewArray();
          o.Add(RandomSmallIntegral(rand));
          o.Add(CBORObject.FromObject(RandomBigInteger(rand)));
        } else if (tag == 30) {
          o = CBORObject.NewArray();
          o.Add(RandomSmallIntegral(rand));
          o.Add(CBORObject.FromObject(RandomBigInteger(rand)));
        } else {
          o = RandomCBORObject(rand, depth + 1);
        }
        try {
          o = CBORObject.FromObjectAndTag(o, tag);
          // Console.WriteLine("done");
          return o;
        } catch (Exception) {
          continue;
        }
      }
      // Console.WriteLine("Failed "+tag);
      return CBORObject.Null;
    }

    private static CBORObject RandomCBORArray(FastRandom rand, int depth) {
      int x = rand.NextValue(100);
      int count = 0;
      count = (x < 80) ? 2 : ((x < 93) ? 1 : ((x < 98) ? 0 : 10));
      CBORObject cborRet = CBORObject.NewArray();
      for (var i = 0; i < count; ++i) {
        cborRet.Add(RandomCBORObject(rand, depth + 1));
      }
      return cborRet;
    }

    public static ExtendedRational RandomRational(FastRandom rand) {
      BigInteger bigintA = RandomBigInteger(rand);
      BigInteger bigintB = RandomBigInteger(rand);
      if (bigintB.IsZero) {
        bigintB = BigInteger.One;
      }
      return new ExtendedRational(bigintA, bigintB);
    }

    public static CBORObject RandomCBORObject(FastRandom rand) {
      return RandomCBORObject(rand, 0);
    }

    private static CBORObject RandomCBORObject(FastRandom rand, int depth) {
      int nextval = rand.NextValue(11);
      switch (nextval) {
        case 0:
        case 1:
        case 2:
        case 3:
          return RandomNumberOrRational(rand);
        case 4:
          return rand.NextValue(2) == 0 ? CBORObject.True : CBORObject.False;
        case 5:
          return rand.NextValue(2) == 0 ? CBORObject.Null :
            CBORObject.Undefined;
        case 6:
          return RandomCBORTextString(rand);
        case 7:
          return RandomCBORByteString(rand);
        case 8:
          return RandomCBORArray(rand, depth);
        case 9:
          return RandomCBORMap(rand, depth);
        case 10:
          return RandomCBORTaggedObject(rand, depth);
        default:
          return RandomNumber(rand);
      }
    }

    private static long RandomInt64(FastRandom rand) {
      long r = rand.NextValue(0x10000);
      r |= ((long)rand.NextValue(0x10000)) << 16;
      if (rand.NextValue(2) == 0) {
        r |= ((long)rand.NextValue(0x10000)) << 32;
        if (rand.NextValue(2) == 0) {
          r |= ((long)rand.NextValue(0x10000)) << 48;
        }
      }
      return r;
    }

    private static double RandomDouble(FastRandom rand, int exponent) {
      if (exponent == Int32.MaxValue) {
        exponent = rand.NextValue(2047);
      }
      long r = rand.NextValue(0x10000);
      r |= ((long)rand.NextValue(0x10000)) << 16;
      if (rand.NextValue(2) == 0) {
        r |= ((long)rand.NextValue(0x10000)) << 32;
        if (rand.NextValue(2) == 0) {
          r |= ((long)rand.NextValue(0x10000)) << 48;
        }
      }
      r &= ~0x7FF0000000000000L;  // clear exponent
      r |= ((long)exponent) << 52;  // set exponent
      return BitConverter.ToDouble(BitConverter.GetBytes((long)r), 0);
    }

    private static float RandomSingle(FastRandom rand, int exponent) {
      if (exponent == Int32.MaxValue) {
        exponent = rand.NextValue(255);
      }
      int r = rand.NextValue(0x10000);
      if (rand.NextValue(2) == 0) {
        r |= ((int)rand.NextValue(0x10000)) << 16;
      }
      r &= ~0x7f800000;  // clear exponent
      r |= ((int)exponent) << 23;  // set exponent
      return BitConverter.ToSingle(BitConverter.GetBytes((int)r), 0);
    }

    public static ExtendedDecimal RandomExtendedDecimal(FastRandom r) {
      if (r.NextValue(100) == 0) {
        int x = r.NextValue(3);
        if (x == 0) {
          return ExtendedDecimal.PositiveInfinity;
        }
        if (x == 1) {
          return ExtendedDecimal.NegativeInfinity;
        }
        if (x == 2) {
          return ExtendedDecimal.NaN;
        }
        // Signaling NaN currently not generated because
        // it doesn't round-trip as well
      }
      return ExtendedDecimal.FromString(RandomDecimalString(r));
    }

    public static BigInteger RandomBigInteger(FastRandom r) {
      int count = r.NextValue(60) + 1;
      var bytes = new byte[count];
      for (var i = 0; i < count; ++i) {
        bytes[i] = (byte)((int)r.NextValue(256));
      }
      return BigInteger.fromByteArray(bytes, true);
    }

    public static ExtendedFloat RandomExtendedFloat(FastRandom r) {
      if (r.NextValue(100) == 0) {
        int x = r.NextValue(3);
        if (x == 0) {
          return ExtendedFloat.PositiveInfinity;
        }
        if (x == 1) {
          return ExtendedFloat.NegativeInfinity;
        }
        if (x == 2) {
          return ExtendedFloat.NaN;
        }
      }
      return ExtendedFloat.Create(
        RandomBigInteger(r),
        (BigInteger)(r.NextValue(400) - 200));
    }

    public static String RandomBigIntString(FastRandom r) {
      int count = r.NextValue(50) + 1;
      var sb = new StringBuilder();
      if (r.NextValue(2) == 0) {
        sb.Append('-');
      }
      for (var i = 0; i < count; ++i) {
        if (i == 0) {
          sb.Append((char)('1' + r.NextValue(9)));
        } else {
          sb.Append((char)('0' + r.NextValue(10)));
        }
      }
      return sb.ToString();
    }

    public static CBORObject RandomSmallIntegral(FastRandom r) {
      int count = r.NextValue(20) + 1;
      var sb = new StringBuilder();
      if (r.NextValue(2) == 0) {
        sb.Append('-');
      }
      for (var i = 0; i < count; ++i) {
        if (i == 0) {
          sb.Append((char)('1' + r.NextValue(9)));
        } else {
          sb.Append((char)('0' + r.NextValue(10)));
        }
      }
      return CBORObject.FromObject(BigInteger.fromString(sb.ToString()));
    }

    public static String RandomDecimalString(FastRandom r) {
      int count = r.NextValue(20) + 1;
      var sb = new StringBuilder();
      if (r.NextValue(2) == 0) {
        sb.Append('-');
      }
      for (var i = 0; i < count; ++i) {
        if (i == 0) {
          sb.Append((char)('1' + r.NextValue(9)));
        } else {
          sb.Append((char)('0' + r.NextValue(10)));
        }
      }
      if (r.NextValue(2) == 0) {
        sb.Append('.');
        count = r.NextValue(20) + 1;
        for (var i = 0; i < count; ++i) {
          sb.Append((char)('0' + r.NextValue(10)));
        }
      }
      if (r.NextValue(2) == 0) {
        sb.Append('E');
        count = r.NextValue(20);
        if (count != 0) {
          sb.Append(r.NextValue(2) == 0 ? '+' : '-');
        }
        sb.Append(Convert.ToString(
          (int)count,
          CultureInfo.InvariantCulture));
      }
      return sb.ToString();
    }

    private static void TestDecimalString(String r) {
      CBORObject o = CBORObject.FromObject(ExtendedDecimal.FromString(r));
      CBORObject o2 = CBORDataUtilities.ParseJSONNumber(r);
      CompareTestEqual(o, o2);
    }

    [TestMethod]
    public void TestAdd() {
      var r = new FastRandom();
      for (var i = 0; i < 3000; ++i) {
        CBORObject o1 = RandomNumber(r);
        CBORObject o2 = RandomNumber(r);
        ExtendedDecimal cmpDecFrac =
          o1.AsExtendedDecimal().Add(o2.AsExtendedDecimal());
        ExtendedDecimal cmpCobj = CBORObject.Addition(
          o1,
          o2).AsExtendedDecimal();
        if (cmpDecFrac.CompareTo(cmpCobj) != 0) {
          Assert.AreEqual(
            0,
            cmpDecFrac.CompareTo(cmpCobj),
            ObjectMessages(o1, o2, "Results don't match"));
        }
        TestCommon.AssertRoundTrip(o1);
        TestCommon.AssertRoundTrip(o2);
      }
    }

    [TestMethod]
    public void TestDivide() {
      var r = new FastRandom();
      for (var i = 0; i < 3000; ++i) {
        CBORObject o1 = CBORObject.FromObject(RandomBigInteger(r));
        CBORObject o2 = CBORObject.FromObject(RandomBigInteger(r));
        if (o2.IsZero) {
          continue;
        }
        var er = new ExtendedRational(o1.AsBigInteger(), o2.AsBigInteger());
        if (er.CompareTo(CBORObject.Divide(o1, o2).AsExtendedRational()) != 0) {
          Assert.Fail(ObjectMessages(o1, o2, "Results don't match"));
        }
      }
      for (var i = 0; i < 3000; ++i) {
        CBORObject o1 = RandomNumber(r);
        CBORObject o2 = RandomNumber(r);
        ExtendedRational er =
          o1.AsExtendedRational().Divide(o2.AsExtendedRational());
        if (er.CompareTo(CBORObject.Divide(o1, o2).AsExtendedRational()) != 0) {
          Assert.Fail(ObjectMessages(o1, o2, "Results don't match"));
        }
      }
    }
    [TestMethod]
    public void TestMultiply() {
      var r = new FastRandom();
      for (var i = 0; i < 3000; ++i) {
        CBORObject o1 = RandomNumber(r);
        CBORObject o2 = RandomNumber(r);
        ExtendedDecimal cmpDecFrac =
          o1.AsExtendedDecimal().Multiply(o2.AsExtendedDecimal());
        ExtendedDecimal cmpCobj = CBORObject.Multiply(
          o1,
          o2).AsExtendedDecimal();
        if (cmpDecFrac.CompareTo(cmpCobj) != 0) {
          Assert.AreEqual(
            0,
            cmpDecFrac.CompareTo(cmpCobj),
            ObjectMessages(o1, o2, "Results don't match"));
        }
        TestCommon.AssertRoundTrip(o1);
        TestCommon.AssertRoundTrip(o2);
      }
    }

    [TestMethod]
    public void TestSubtract() {
      var r = new FastRandom();
      for (var i = 0; i < 3000; ++i) {
        CBORObject o1 = RandomNumber(r);
        CBORObject o2 = RandomNumber(r);
        ExtendedDecimal cmpDecFrac =
          o1.AsExtendedDecimal().Subtract(o2.AsExtendedDecimal());
        ExtendedDecimal cmpCobj = CBORObject.Subtract(
          o1,
          o2).AsExtendedDecimal();
        if (cmpDecFrac.CompareTo(cmpCobj) != 0) {
          Assert.AreEqual(
            0,
            cmpDecFrac.CompareTo(cmpCobj),
            ObjectMessages(o1, o2, "Results don't match"));
        }
        TestCommon.AssertRoundTrip(o1);
        TestCommon.AssertRoundTrip(o2);
      }
    }

    private static string ObjectMessages(
      CBORObject o1,
      CBORObject o2,
      String s) {
      if (o1.Type == CBORType.Number && o2.Type == CBORType.Number) {
        return s + ":\n" + o1 + " and\n" + o2 + "\nOR\n" +
          o1.AsExtendedDecimal() + " and\n" + o2.AsExtendedDecimal() +
          "\nOR\n" +
          "AddSubCompare(" + ToByteArrayString(o1) + ",\n" +
          ToByteArrayString(o2) + ");";
      }
      return s + ":\n" + o1 + " and\n" + o2 + "\nOR\n" +
        ToByteArrayString(o1) + " and\n" + ToByteArrayString(o2);
    }

    public static void CompareTestEqual(CBORObject o1, CBORObject o2) {
      if (CompareTestReciprocal(o1, o2) != 0) {
        Assert.Fail(
          ObjectMessages(
o1,
o2,
"Not equal: " + CompareTestReciprocal(o1, o2)));
      }
    }

    public static void CompareTestLess(CBORObject o1, CBORObject o2) {
      if (CompareTestReciprocal(o1, o2) >= 0) {
        Assert.Fail(
          ObjectMessages(
o1,
o2,
"Not less: " + CompareTestReciprocal(o1, o2)));
      }
    }

    private static int CompareTestReciprocal(CBORObject o1, CBORObject o2) {
      if (o1 == null) {
        throw new ArgumentNullException("o1");
      }
      if (o2 == null) {
        throw new ArgumentNullException("o2");
      }
      int cmp = o1.CompareTo(o2);
      int cmp2 = o2.CompareTo(o1);
      if (-cmp2 != cmp) {
        Assert.AreEqual(cmp, -cmp2, ObjectMessages(o1, o2, "Not reciprocal"));
      }
      return cmp;
    }

    public static string ToByteArrayString(byte[] bytes) {
      var sb = new StringBuilder();
      string hex = "0123456789ABCDEF";
      sb.Append("new byte[] { ");
      for (var i = 0; i < bytes.Length; ++i) {
        if (i > 0) {
          sb.Append(","); }
        if ((bytes[i] & 0x80) != 0) {
          sb.Append("(byte)0x");
        } else {
          sb.Append("0x");
        }
        sb.Append(hex[(bytes[i] >> 4) & 0xf]);
        sb.Append(hex[bytes[i] & 0xf]);
      }
      sb.Append("}");
      return sb.ToString();
    }

    public static string ToByteArrayString(CBORObject obj) {
      return new StringBuilder()
        .Append("CBORObject.DecodeFromBytes(")
        .Append(ToByteArrayString(obj.EncodeToBytes()))
        .Append(")")
        .ToString();
    }

    [TestMethod]
    public void TestExtendedCompare() {
      Assert.AreEqual(
        -1,
        ExtendedRational.Zero.CompareTo(ExtendedRational.NaN));
      Assert.AreEqual(-1, ExtendedFloat.Zero.CompareTo(ExtendedFloat.NaN));
      Assert.AreEqual(-1, ExtendedDecimal.Zero.CompareTo(ExtendedDecimal.NaN));
    }

    [TestMethod]
    public void TestDecFracCompareIntegerVsBigFraction() {
      ExtendedDecimal a =
        ExtendedDecimal.FromString(
          "7.00468923842476447758037175245551511770928808756622205663208" + "4784688080253355047487262563521426272927783429622650146484375" );
      ExtendedDecimal b = ExtendedDecimal.FromString("5");
      Assert.AreEqual(1, a.CompareTo(b));
      Assert.AreEqual(-1, b.CompareTo(a));
      CBORObject o1 = null;
      CBORObject o2 = null;
      o1 = CBORObject.DecodeFromBytes(new byte[] { (byte)0xfb, (byte)0x8b,
                       0x44, (byte)0xf2, (byte)0xa9, 0x0c, 0x27, 0x42, 0x28 });
      o2 = CBORObject.DecodeFromBytes(new byte[] { (byte)0xc5, (byte)0x82,
                                        0x38, (byte)0xa4, (byte)0xc3, 0x50,
                                        0x02,
                                        (byte)0x98,
                                        (byte)0xc5, (byte)0xa8, 0x02,
                                        (byte)0xc1, (byte)0xf6,
                                        (byte)0xc0, 0x1a, (byte)0xbe,
                                        0x08,
                             0x04, (byte)0x86, (byte)0x99, 0x3e, (byte)0xf1 });
      AddSubCompare(o1, o2);
    }

    private static void CompareDecimals(CBORObject o1, CBORObject o2) {
      int cmpDecFrac = o1.AsExtendedDecimal().CompareTo(o2.AsExtendedDecimal());
      int cmpCobj = CompareTestReciprocal(o1, o2);
      if (cmpDecFrac != cmpCobj) {
        Assert.AreEqual(
          cmpDecFrac,
          cmpCobj,
          ObjectMessages(o1, o2, "Compare: Results don't match"));
      }
      TestCommon.AssertRoundTrip(o1);
      TestCommon.AssertRoundTrip(o2);
    }

    private static void AddSubCompare(CBORObject o1, CBORObject o2) {
      ExtendedDecimal cmpDecFrac =
        o1.AsExtendedDecimal().Add(o2.AsExtendedDecimal());
      ExtendedDecimal cmpCobj = CBORObject.Addition(o1, o2).AsExtendedDecimal();
      if (cmpDecFrac.CompareTo(cmpCobj) != 0) {
        Assert.AreEqual(
          0,
          cmpDecFrac.CompareTo(cmpCobj),
          ObjectMessages(
            o1,
            o2,
            "Add: Results don't match:\n" + cmpDecFrac + " vs\n" + cmpCobj));
      }
      cmpDecFrac = o1.AsExtendedDecimal().Subtract(o2.AsExtendedDecimal());
      cmpCobj = CBORObject.Subtract(o1, o2).AsExtendedDecimal();
      if (cmpDecFrac.CompareTo(cmpCobj) != 0) {
        Assert.AreEqual(
          0,
          cmpDecFrac.CompareTo(cmpCobj),
          ObjectMessages(
            o1,
            o2,
            "Subtract: Results don't match:\n" + cmpDecFrac + " vs\n" + cmpCobj));
      }
      CompareDecimals(o1, o2);
    }

    [TestMethod]
    public void TestCompareB() {
      Assert.IsTrue(CBORObject.DecodeFromBytes(new byte[] { (byte)0xfa,
                                 0x7f, (byte)0x80, 0x00, 0x00 }).IsInfinity());
      Assert.IsTrue(CBORObject.DecodeFromBytes(new byte[] { (byte)0xfa,
            0x7f, (byte)0x80, 0x00, 0x00 }).AsExtendedRational().IsInfinity());
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8, 0x1e,
                                (byte)0x82, (byte)0xc2, 0x58, 0x28, 0x77,
                                       0x24,
                                     0x73,
                                     (byte)0x84,
                                (byte)0xbd, 0x72, (byte)0x82, 0x7c,
                                       (byte)0xd6,
                                     (byte)0x93,
                                     0x18,
                                     0x44, (byte)0x8a, (byte)0x88, 0x43, 0x67,
                                     (byte)0xa2,
                                     (byte)0xeb,
                                0x11, 0x00, 0x15, 0x1b, 0x1d, 0x5d,
                                       (byte)0xdc,
                                     (byte)0xeb,
                                     0x39,
                                     0x17, 0x72, 0x11, 0x5b, 0x03, (byte)0xfa,
                                     (byte)0xa8,
                                     0x3f,
                          (byte)0xd2, 0x75, (byte)0xf8, 0x36, (byte)0xc8,
                                       0x1a,
                                     0x00,
                                     0x2e,
                                     (byte)0x8c, (byte)0x8d }),
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xfa, 0x7f,
                                     (byte)0x80, 0x00, 0x00 }));
      CompareTestLess(
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8, 0x1e,
                                (byte)0x82, (byte)0xc2, 0x58, 0x28, 0x77,
                                       0x24,
                                     0x73,
                                     (byte)0x84,
                                (byte)0xbd, 0x72, (byte)0x82, 0x7c,
                                       (byte)0xd6,
                                     (byte)0x93,
                                     0x18,
                                     0x44, (byte)0x8a, (byte)0x88, 0x43, 0x67,
                                     (byte)0xa2,
                                     (byte)0xeb,
                                0x11, 0x00, 0x15, 0x1b, 0x1d, 0x5d,
                                       (byte)0xdc,
                                     (byte)0xeb,
                                     0x39,
                                     0x17, 0x72, 0x11, 0x5b, 0x03, (byte)0xfa,
                                     (byte)0xa8,
                                     0x3f,
                          (byte)0xd2, 0x75, (byte)0xf8, 0x36, (byte)0xc8,
                                       0x1a,
                                     0x00,
                                     0x2e,
                                     (byte)0x8c, (byte)0x8d }),
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xfa, 0x7f,
                                     (byte)0x80, 0x00, 0x00 }));
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xfb, 0x7f,
                             (byte)0xf8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xfa, 0x7f,
                                     (byte)0x80, 0x00, 0x00 }));
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] { 0x1a, (byte)0xfc, 0x1a,
                                     (byte)0xb0, 0x52 }),
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xc5, (byte)0x82,
                                     0x38, 0x5f, (byte)0xc2, 0x50, 0x08, 0x70,
                                     (byte)0xf3,
                                     (byte)0xc4,
                                     (byte)0x90, 0x4c, 0x14, (byte)0xba, 0x59,
                                     (byte)0xf0,
                                     (byte)0xc6,
                      (byte)0xcb, (byte)0x8c, (byte)0x8d, 0x40, (byte)0x80 }));
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xc5, (byte)0x82,
                                     0x38, (byte)0xc7, 0x3b, 0x00, 0x00, 0x08,
                                     (byte)0xbf,
                                     (byte)0xda,
                                     (byte)0xaf, 0x73, 0x46 }),
        CBORObject.DecodeFromBytes(new byte[] { 0x3b, 0x5a, (byte)0x9b,
          (byte)0x9a, (byte)0x9c, (byte)0xb4, (byte)0x95, (byte)0xbf, 0x71 }));
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] { 0x1a, (byte)0xbb, 0x0c,
                                     (byte)0xf7, 0x52 }),
        CBORObject.DecodeFromBytes(new byte[] { 0x1a, (byte)0x82, 0x00,
                                     (byte)0xbf, (byte)0xf9 }));
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xfa, 0x1f,
                                     (byte)0x80, (byte)0xdb, (byte)0x9b }),
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xfb, 0x31,
          (byte)0x90, (byte)0xea, 0x16, (byte)0xbe, (byte)0x80, 0x0b, 0x37 }));
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xfb, 0x3c, 0x00,
                 (byte)0xcf, (byte)0xb6, (byte)0xbd, (byte)0xff, 0x37, 0x38 }),
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xfa, 0x30,
                                     (byte)0x80, 0x75, 0x63 }));
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xc5, (byte)0x82,
          0x38, 0x7d, 0x3a, 0x06, (byte)0xbc, (byte)0xd5, (byte)0xb8 }),
        CBORObject.DecodeFromBytes(new byte[] { 0x38, 0x5c }));
      TestCommon.AssertRoundTrip(
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xda, 0x00, 0x1d,
                          (byte)0xdb, 0x03, (byte)0xfb, (byte)0xff,
                                       (byte)0xf0,
                                     0x00,
                                     0x00,
                                     0x00, 0x00, 0x00, 0x00 }));
      CBORObject cbor = CBORObject.FromObjectAndTag(
        Double.NegativeInfinity,
        1956611);
      TestCommon.AssertRoundTrip(cbor);
      cbor =

        CBORObject.FromObjectAndTag(
          CBORObject.FromObject(Double.NegativeInfinity),
          1956611);
      TestCommon.AssertRoundTrip(cbor);
      cbor =

        CBORObject.FromObjectAndTag(
          CBORObject.FromObject(ExtendedFloat.NegativeInfinity),
          1956611);
      TestCommon.AssertRoundTrip(cbor);
      cbor =

        CBORObject.FromObjectAndTag(
          CBORObject.FromObject(ExtendedDecimal.NegativeInfinity),
          1956611);
      TestCommon.AssertRoundTrip(cbor);
      cbor =

        CBORObject.FromObjectAndTag(
          CBORObject.FromObject(ExtendedRational.NegativeInfinity),
          1956611);
      TestCommon.AssertRoundTrip(cbor);
    }

    [TestMethod]
    public void ExtraDecimalTests() {
      Assert.AreEqual(
        ExtendedDecimal.NegativeInfinity,
        ExtendedDecimal.FromString("-79228162514264337593543950336"
)
        .RoundToPrecision(PrecisionContext.CliDecimal));
      Assert.AreEqual(
        ExtendedDecimal.PositiveInfinity,
        ExtendedDecimal.FromString("8.782580686213340724E+28"
)
        .RoundToPrecision(PrecisionContext.CliDecimal));
      Assert.AreEqual(
        ExtendedDecimal.NegativeInfinity,
        ExtendedDecimal.FromString("-9.3168444507547E+28"
)
        .RoundToPrecision(PrecisionContext.CliDecimal));
      Assert.AreEqual(
        "-9344285899206687626894794544",

  ExtendedDecimal.FromString("-9344285899206687626894794544.04982268810272216796875"
).RoundToPrecision(PrecisionContext.CliDecimal)
        .ToPlainString());
      Assert.AreEqual(
        ExtendedDecimal.PositiveInfinity,
        ExtendedDecimal.FromString("96148154858060747311034406200"
)
        .RoundToPrecision(PrecisionContext.CliDecimal));
      Assert.AreEqual(
        ExtendedDecimal.PositiveInfinity,
        ExtendedDecimal.FromString("90246605365627217170000000000"
)
        .RoundToPrecision(PrecisionContext.CliDecimal));
    }

    [TestMethod]
    public void TestFloatDecimalRoundTrip() {
      var r = new FastRandom();
      for (var i = 0; i < 5000; ++i) {
        ExtendedFloat ef = RandomExtendedFloat(r);
        ExtendedDecimal ed = ef.ToExtendedDecimal();
        ExtendedFloat ef2 = ed.ToExtendedFloat();
        // Tests that values converted from float to decimal and
        // back have the same numerical value
        if (ef.CompareTo(ef2) != 0) {
          Assert.AreEqual(
            0,
            ef.CompareTo(ef2),
            "TestFloatDecimalRoundTrip " + ef + "; " + ef2);
        }
      }
    }

    [TestMethod]
    // [Timeout(10000)]
    public void TestCompare() {
      var r = new FastRandom();
      // string badstr = null;
      const int CompareCount = 500;
      for (var i = 0; i < CompareCount; ++i) {
        CBORObject o1 = RandomCBORObject(r);
        CBORObject o2 = RandomCBORObject(r);
        CompareTestReciprocal(o1, o2);
      }
      for (var i = 0; i < 5000; ++i) {
        CBORObject o1 = RandomNumber(r);
        CBORObject o2 = RandomNumber(r);
        CompareDecimals(o1, o2);
      }
      for (var i = 0; i < 50; ++i) {
        CBORObject o1 = CBORObject.FromObject(Single.NegativeInfinity);
        CBORObject o2 = RandomNumberOrRational(r);
        if (o2.IsInfinity() || o2.IsNaN()) {
          continue;
        }
        CompareTestLess(o1, o2);
        o1 = CBORObject.FromObject(Double.NegativeInfinity);
        CompareTestLess(o1, o2);
        o1 = CBORObject.FromObject(Single.PositiveInfinity);
        CompareTestLess(o2, o1);
        o1 = CBORObject.FromObject(Double.PositiveInfinity);
        CompareTestLess(o2, o1);
        o1 = CBORObject.FromObject(Single.NaN);
        CompareTestLess(o2, o1);
        o1 = CBORObject.FromObject(Double.NaN);
        CompareTestLess(o2, o1);
      }
      byte[] bytes1 = { 0, 1 };
      byte[] bytes2 = { 0, 2 };
      byte[] bytes3 = { 0, 2, 0 };
      byte[] bytes4 = { 1, 1 };
      byte[] bytes5 = { 1, 1, 4 };
      byte[] bytes6 = { 1, 2 };
      byte[] bytes7 = { 1, 2, 6 };
      CBORObject[] sortedObjects = {
        CBORObject.Undefined,
        CBORObject.Null,
        CBORObject.False,
        CBORObject.True,
        CBORObject.FromObject(Double.NegativeInfinity),
        CBORObject.FromObject(ExtendedDecimal.FromString("-1E+5000")),
        CBORObject.FromObject(Int64.MinValue),
        CBORObject.FromObject(Int32.MinValue),
        CBORObject.FromObject(-2),
        CBORObject.FromObject(-1),
        CBORObject.FromObject(0),
        CBORObject.FromObject(1),
        CBORObject.FromObject(2),
        CBORObject.FromObject(Int64.MaxValue),
        CBORObject.FromObject(ExtendedDecimal.FromString("1E+5000")),
        CBORObject.FromObject(Double.PositiveInfinity),
        CBORObject.FromObject(Double.NaN),
        CBORObject.FromSimpleValue(0),
        CBORObject.FromSimpleValue(19),
        CBORObject.FromSimpleValue(32),
        CBORObject.FromSimpleValue(255),
        CBORObject.FromObject(bytes1),
        CBORObject.FromObject(bytes2),
        CBORObject.FromObject(bytes3),
        CBORObject.FromObject(bytes4),
        CBORObject.FromObject(bytes5),
        CBORObject.FromObject(bytes6),
        CBORObject.FromObject(bytes7),
        CBORObject.FromObject("aa"),
        CBORObject.FromObject("ab"),
        CBORObject.FromObject("abc"),
        CBORObject.FromObject("ba"),
        CBORObject.FromObject(CBORObject.NewArray()),
        CBORObject.FromObject(CBORObject.NewMap()),
      };
      for (var i = 0; i < sortedObjects.Length; ++i) {
        for (int j = i; j < sortedObjects.Length; ++j) {
          if (i == j) {
            CompareTestEqual(sortedObjects[i], sortedObjects[j]);
          } else {
            CompareTestLess(sortedObjects[i], sortedObjects[j]);
          }
        }
        Assert.AreEqual(1, sortedObjects[i].CompareTo(null));
      }
      CBORObject sp = CBORObject.FromObject(Single.PositiveInfinity);
      CBORObject sn = CBORObject.FromObject(Single.NegativeInfinity);
      CBORObject snan = CBORObject.FromObject(Single.NaN);
      CBORObject dp = CBORObject.FromObject(Double.PositiveInfinity);
      CBORObject dn = CBORObject.FromObject(Double.NegativeInfinity);
      CBORObject dnan = CBORObject.FromObject(Double.NaN);
      CompareTestEqual(sp, sp);
      CompareTestEqual(sp, dp);
      CompareTestEqual(dp, dp);
      CompareTestEqual(sn, sn);
      CompareTestEqual(sn, dn);
      CompareTestEqual(dn, dn);
      CompareTestEqual(snan, snan);
      CompareTestEqual(snan, dnan);
      CompareTestEqual(dnan, dnan);
      CompareTestLess(sn, sp);
      CompareTestLess(sn, dp);
      CompareTestLess(sn, snan);
      CompareTestLess(sn, dnan);
      CompareTestLess(sp, snan);
      CompareTestLess(sp, dnan);
      CompareTestLess(dn, dp);
      CompareTestLess(dp, dnan);
    }

    [TestMethod]
    public void TestTags264And265() {
      CBORObject cbor;
      // Tag 264
      cbor = CBORObject.DecodeFromBytes(new byte[] { 0xd9, 0x01, 0x08, 0x82,
                                          0xc2, 0x42, 2, 2, 0xc2, 0x42, 2, 2 });
      TestCommon.AssertRoundTrip(cbor);
      // Tag 265
      cbor = CBORObject.DecodeFromBytes(new byte[] { 0xd9, 0x01, 0x09, 0x82,
                                          0xc2, 0x42, 2, 2, 0xc2, 0x42, 2, 2 });
      TestCommon.AssertRoundTrip(cbor);
    }

    [TestMethod]
    public void TestParseDecimalStrings() {
      var rand = new FastRandom();
      for (var i = 0; i < 3000; ++i) {
        string r = RandomDecimalString(rand);
        TestDecimalString(r);
      }
    }

    [TestMethod]
    public void TestExtendedInfinity() {
      Assert.IsTrue(ExtendedDecimal.PositiveInfinity.IsPositiveInfinity());
      Assert.IsFalse(ExtendedDecimal.PositiveInfinity.IsNegativeInfinity());
      Assert.IsFalse(ExtendedDecimal.PositiveInfinity.IsNegative);
      Assert.IsFalse(ExtendedDecimal.NegativeInfinity.IsPositiveInfinity());
      Assert.IsTrue(ExtendedDecimal.NegativeInfinity.IsNegativeInfinity());
      Assert.IsTrue(ExtendedDecimal.NegativeInfinity.IsNegative);
      Assert.IsTrue(ExtendedFloat.PositiveInfinity.IsInfinity());
      Assert.IsTrue(ExtendedFloat.PositiveInfinity.IsPositiveInfinity());
      Assert.IsFalse(ExtendedFloat.PositiveInfinity.IsNegativeInfinity());
      Assert.IsFalse(ExtendedFloat.PositiveInfinity.IsNegative);
      Assert.IsTrue(ExtendedFloat.NegativeInfinity.IsInfinity());
      Assert.IsFalse(ExtendedFloat.NegativeInfinity.IsPositiveInfinity());
      Assert.IsTrue(ExtendedFloat.NegativeInfinity.IsNegativeInfinity());
      Assert.IsTrue(ExtendedFloat.NegativeInfinity.IsNegative);
      Assert.IsTrue(ExtendedRational.PositiveInfinity.IsInfinity());
      Assert.IsTrue(ExtendedRational.PositiveInfinity.IsPositiveInfinity());
      Assert.IsFalse(ExtendedRational.PositiveInfinity.IsNegativeInfinity());
      Assert.IsFalse(ExtendedRational.PositiveInfinity.IsNegative);
      Assert.IsTrue(ExtendedRational.NegativeInfinity.IsInfinity());
      Assert.IsFalse(ExtendedRational.NegativeInfinity.IsPositiveInfinity());
      Assert.IsTrue(ExtendedRational.NegativeInfinity.IsNegativeInfinity());
      Assert.IsTrue(ExtendedRational.NegativeInfinity.IsNegative);

      Assert.AreEqual(
        ExtendedDecimal.PositiveInfinity,
        ExtendedDecimal.FromDouble(Double.PositiveInfinity));
      Assert.AreEqual(
        ExtendedDecimal.NegativeInfinity,
        ExtendedDecimal.FromDouble(Double.NegativeInfinity));
      Assert.AreEqual(
        ExtendedDecimal.PositiveInfinity,
        ExtendedDecimal.FromSingle(Single.PositiveInfinity));
      Assert.AreEqual(
        ExtendedDecimal.NegativeInfinity,
        ExtendedDecimal.FromSingle(Single.NegativeInfinity));

      Assert.AreEqual(
        ExtendedFloat.PositiveInfinity,
        ExtendedFloat.FromDouble(Double.PositiveInfinity));
      Assert.AreEqual(
        ExtendedFloat.NegativeInfinity,
        ExtendedFloat.FromDouble(Double.NegativeInfinity));
      Assert.AreEqual(
        ExtendedFloat.PositiveInfinity,
        ExtendedFloat.FromSingle(Single.PositiveInfinity));
      Assert.AreEqual(
        ExtendedFloat.NegativeInfinity,
        ExtendedFloat.FromSingle(Single.NegativeInfinity));

      Assert.AreEqual(
        ExtendedRational.PositiveInfinity,
        ExtendedRational.FromDouble(Double.PositiveInfinity));
      Assert.AreEqual(
        ExtendedRational.NegativeInfinity,
        ExtendedRational.FromDouble(Double.NegativeInfinity));
      Assert.AreEqual(
        ExtendedRational.PositiveInfinity,
        ExtendedRational.FromSingle(Single.PositiveInfinity));
      Assert.AreEqual(
        ExtendedRational.NegativeInfinity,
        ExtendedRational.FromSingle(Single.NegativeInfinity));

      Assert.AreEqual(
        ExtendedRational.PositiveInfinity,
        ExtendedRational.FromExtendedDecimal(ExtendedDecimal.PositiveInfinity));
      Assert.AreEqual(
        ExtendedRational.NegativeInfinity,
        ExtendedRational.FromExtendedDecimal(ExtendedDecimal.NegativeInfinity));
      Assert.AreEqual(
        ExtendedRational.PositiveInfinity,
        ExtendedRational.FromExtendedFloat(ExtendedFloat.PositiveInfinity));
      Assert.AreEqual(
        ExtendedRational.NegativeInfinity,
        ExtendedRational.FromExtendedFloat(ExtendedFloat.NegativeInfinity));

  Assert.IsTrue(Double.IsPositiveInfinity(ExtendedRational.PositiveInfinity.ToDouble()));

  Assert.IsTrue(Double.IsNegativeInfinity(ExtendedRational.NegativeInfinity.ToDouble()));

  Assert.IsTrue(Single.IsPositiveInfinity(ExtendedRational.PositiveInfinity.ToSingle()));

  Assert.IsTrue(Single.IsNegativeInfinity(ExtendedRational.NegativeInfinity.ToSingle()));
      try {
        ExtendedDecimal.PositiveInfinity.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.NegativeInfinity.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.PositiveInfinity.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.NegativeInfinity.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedRational.PositiveInfinity.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedRational.NegativeInfinity.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [TestMethod]
    public void TestCBORExceptions() {
      try {
        CBORObject.DecodeFromBytes(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewArray().Remove(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().Remove(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewArray().Add(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewArray().Add(CBORObject.Null);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().Add(CBORObject.True);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.Remove(CBORObject.True);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(0).Remove(CBORObject.True);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(String.Empty).Remove(CBORObject.True);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewArray().AsExtendedFloat();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsExtendedFloat();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsExtendedFloat();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsExtendedFloat();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsExtendedFloat();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(String.Empty).AsExtendedFloat();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [TestMethod]
    public void TestReadWriteInt() {
      var r = new FastRandom();
      try {
        for (var i = 0; i < 1000; ++i) {
          int val = unchecked((int)RandomInt64(r));
          using (var ms = new MemoryStream()) {
            MiniCBOR.WriteInt32(val, ms);
            using (var ms2 = new MemoryStream(ms.ToArray())) {
              Assert.AreEqual(val, MiniCBOR.ReadInt32(ms2));
            }
          }
          using (var ms3 = new MemoryStream()) {
            CBORObject.Write(val, ms3);
            using (var ms2 = new MemoryStream(ms3.ToArray())) {
              Assert.AreEqual(val, MiniCBOR.ReadInt32(ms2));
            }
          }
        }
      } catch (IOException ioex) {
        Assert.Fail(ioex.Message);
      }
    }

    [TestMethod]
    public void TestCBORInfinity() {
      Assert.AreEqual(
        "-Infinity" ,
        CBORObject.FromObject(ExtendedRational.NegativeInfinity).ToString());
      Assert.AreEqual(
        "Infinity" ,
        CBORObject.FromObject(ExtendedRational.PositiveInfinity).ToString());

  TestCommon.AssertRoundTrip(CBORObject.FromObject(ExtendedRational.NegativeInfinity));

  TestCommon.AssertRoundTrip(CBORObject.FromObject(ExtendedRational.PositiveInfinity));
      Assert.IsTrue(CBORObject.FromObject(ExtendedRational.NegativeInfinity)
                    .IsInfinity());
      Assert.IsTrue(CBORObject.FromObject(ExtendedRational.PositiveInfinity)
                    .IsInfinity());
      Assert.IsTrue(CBORObject.FromObject(ExtendedRational.NegativeInfinity)
                    .IsNegativeInfinity());
      Assert.IsTrue(CBORObject.FromObject(ExtendedRational.PositiveInfinity)
                    .IsPositiveInfinity());
      Assert.IsTrue(CBORObject.PositiveInfinity.IsInfinity());
      Assert.IsTrue(CBORObject.PositiveInfinity.IsPositiveInfinity());
      Assert.IsTrue(CBORObject.NegativeInfinity.IsInfinity());
      Assert.IsTrue(CBORObject.NegativeInfinity.IsNegativeInfinity());
      Assert.IsTrue(CBORObject.NaN.IsNaN());

  TestCommon.AssertRoundTrip(CBORObject.FromObject(ExtendedDecimal.NegativeInfinity));

  TestCommon.AssertRoundTrip(CBORObject.FromObject(ExtendedFloat.NegativeInfinity));

  TestCommon.AssertRoundTrip(CBORObject.FromObject(Double.NegativeInfinity));

  TestCommon.AssertRoundTrip(CBORObject.FromObject(Single.NegativeInfinity));

  TestCommon.AssertRoundTrip(CBORObject.FromObject(ExtendedDecimal.PositiveInfinity));

  TestCommon.AssertRoundTrip(CBORObject.FromObject(ExtendedFloat.PositiveInfinity));

  TestCommon.AssertRoundTrip(CBORObject.FromObject(Double.PositiveInfinity));

  TestCommon.AssertRoundTrip(CBORObject.FromObject(Single.PositiveInfinity));
    }

    [TestMethod]
    [Timeout(5000)]
    public void TestExtendedExtremeExponent() {
      // Values with extremely high or extremely low exponents;
      // we just check whether this test method runs reasonably fast
      // for all these test cases
      CBORObject obj;
      obj = CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4, (byte)0x82,
                                      0x3a, 0x00, 0x1c, 0x2d, 0x0d, 0x1a,
                                           0x13,
                                         0x6c,
                                         (byte)0xa1,
                                         (byte)0x97 });
      TestCommon.AssertRoundTrip(obj);
      obj = CBORObject.DecodeFromBytes(new byte[] { (byte)0xda, 0x00, 0x14,
                          0x57, (byte)0xce, (byte)0xc5, (byte)0x82, 0x1a,
                                           0x46,
                                         0x5a,
                                         0x37,
                                (byte)0x87, (byte)0xc3, 0x50, 0x5e,
                                           (byte)0xec,
                                         (byte)0xfd,
                                         0x73,
                                         0x50, 0x64, (byte)0xa1, 0x1f, 0x10,
                                         (byte)0xc4,
                                         (byte)0xff,
                             (byte)0xf2, (byte)0xc4, (byte)0xc9, 0x65, 0x12 });
      TestCommon.AssertRoundTrip(obj);
      int actual = CBORObject.FromObject(
        ExtendedDecimal.Create((BigInteger)333333, (BigInteger)(-2)))
        .CompareTo(
          CBORObject.FromObject(
            ExtendedFloat.Create(
(BigInteger)5234222,
(BigInteger)(-24936668661488L))));
      Assert.AreEqual(1, actual);
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4, (byte)0x82, 0x31,
                                   0x19, 0x03, 0x43
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xda, 0x00,
                                                  (byte)0xa3, 0x35, (byte)0xc8,
                                                  (byte)0xc5,
                                      (byte)0x82, 0x1b, 0x00, 0x01,
                                                    (byte)0xe0,
                                                  (byte)0xb2,
                                                  (byte)0x83,
                          0x32, 0x0f, (byte)0x8b, (byte)0xc2, 0x58, 0x27,
                                                    0x2a,
                                                  0x65,
                                                  0x4a,
                                      (byte)0xbd, 0x67, 0x00, 0x15,
                                                    (byte)0x94,
                                                  (byte)0xb3,
                                                  (byte)0xdd,
                                      (byte)0x80, 0x49, 0x7c, 0x16,
                                                    (byte)0x9f,
                                                  (byte)0x83,
                                                  0x05,
                                (byte)0xd0, (byte)0x80, (byte)0xf8,
                                                    (byte)0x8d,
                                                  (byte)0xe3,
                                                  0x26,
                                      0x14, (byte)0xd6, 0x2d, (byte)0xab,
                                                    0x53,
                                                  (byte)0xd1,
                                                  0x79,
                                      (byte)0xe7, (byte)0xb5, (byte)0xc0,
                                                    0x73,
                                                  (byte)0xf0,
                                                  0x1d,
                                              (byte)0xbd, 0x45, (byte)0xfa }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xc5, (byte)0x82, 0x1b,
                                0x01, 0x58, 0x0a, (byte)0xc0, (byte)0xc8,
                                     0x66,
                                   0x47,
                                   (byte)0xc0,
                                (byte)0xc3, 0x58, 0x19, 0x50, 0x4d,
                                     (byte)0x89,
                                   0x04,
                                   (byte)0x8a,
                          (byte)0xc4, (byte)0xb7, 0x3a, 0x49, (byte)0xcc,
                                     0x13,
                                   0x4c,
                                   0x33,
                                (byte)0x80, 0x0c, 0x60, (byte)0xe7,
                                     (byte)0xd4,
                                   0x5b,
                                   (byte)0x89,
                                   (byte)0xdb, (byte)0xc8, (byte)0x81, 0x0a,
    (byte)0x85 }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8,
                                0x1e, (byte)0x82, (byte)0xc3, 0x58, 0x28,
                                                             0x3b,
                                                           0x1f,
                                                           0x60,
                                      (byte)0xa0, 0x4a, (byte)0xd3,
                                                             (byte)0x94,
                                                           0x20,
                                                           (byte)0xe9,
                          (byte)0xfa, (byte)0xd2, 0x03, (byte)0xb5,
                                                             (byte)0xd2,
                                                           0x0f,
                                                           0x7b,
                                0x7c, (byte)0x8d, 0x50, 0x4b, (byte)0x93,
                                                             0x5d,
                                                           0x6a,
                                                           (byte)0xc6,
                                      (byte)0xdf, 0x01, (byte)0xa9,
                                                             (byte)0xa6,
                                                           0x3c,
                                                           (byte)0xf4,
                                      (byte)0xf8, (byte)0xb2, 0x41,
                                                             (byte)0xc3,
                                                           (byte)0xfd,
                                                           0x5d,
                          (byte)0xc8, (byte)0x86, 0x2b, (byte)0xf3,
                                                             (byte)0xc2,
                                                           0x52,
                                                           0x58,
                                            0x3a, (byte)0xaf, 0x69,
                                                             (byte)0x89,
                                                           (byte)0xc0,
                                                           (byte)0xa4,
                                (byte)0xe1, 0x51, (byte)0x9f, 0x09,
                                                             (byte)0xcb,
                                                           (byte)0xbb,
                                                           0x15,
                                              0x35, (byte)0xcf, 0x2b, 0x52 }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4, (byte)0x82, 0x23,
                                0x3b, 0x00, 0x1b, (byte)0xda, (byte)0xb3,
                                     0x03,
                                   0x15,
                                   0x28,
    (byte)0xd8 }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xc5,
                                                  (byte)0x82, 0x1b, 0x00,
                                                             0x6f,
                                                           0x25,
                                                           0x52, (byte)0xc2,
                                0x11, (byte)0xe1, (byte)0xe7, (byte)0xc3,
                                                             0x58,
                                                           0x3a,
                                                           0x64,
                                (byte)0xc7, 0x29, (byte)0xdd, (byte)0x94,
                                                             0x6c,
                                                           0x4b,
                                                           0x09,
                                      (byte)0xa3, (byte)0xdf, 0x28,
                                                             (byte)0xaf,
                                                           0x0f,
                                                           (byte)0xbf,
                                (byte)0xdf, (byte)0xd7, 0x73, (byte)0xac,
                                                             0x20,
                                                           0x40,
                                                           (byte)0xaf,
                                (byte)0x94, 0x6d, (byte)0xd7, (byte)0xd2,
                                                             0x38,
                                                           (byte)0xd6,
                                                           0x14,
                                0x0a, 0x58, (byte)0xa2, 0x18, 0x12, 0x19,
                                                             0x2d,
                                                           0x40,
                                                           (byte)0x99,
                          (byte)0xca, (byte)0xb6, (byte)0x98, 0x61,
                                                             (byte)0x91,
                                                           0x5d,
                                                           0x49,
                                0x68, (byte)0xac, 0x1b, 0x32, 0x57,
                                                             (byte)0xca,
                                                           (byte)0x85,
                                                           0x0a,
                      (byte)0xea, 0x48, (byte)0xf8, 0x09, (byte)0xc2, 0x7e }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xc5, (byte)0x82, 0x1b,
                                0x00, 0x00, 0x00, 0x01, (byte)0xec,
                                     (byte)0xb5,
                                   0x38,
                                   (byte)0xdf,
                                   (byte)0xc2, 0x58, 0x37, 0x58, (byte)0xd6,
                                   0x14,
                                   (byte)0xc8,
                                (byte)0x95, 0x03, 0x44, (byte)0xf3,
                                     (byte)0xd4,
                                   0x34,
                                   (byte)0x9a,
                                (byte)0xdd, (byte)0xf9, (byte)0xca,
                                     (byte)0xfb,
                                   (byte)0xa3,
                                   0x6d,
                                   0x19, (byte)0xe7, 0x2a, 0x41, (byte)0xf8,
                                   (byte)0xad,
                                   (byte)0x9f,
                                   (byte)0xee, 0x5b, 0x4b, (byte)0xd7, 0x12,
                                   0x16,
                                   (byte)0xeb,
                                (byte)0x80, (byte)0x83, 0x6e, 0x20,
                                     (byte)0xe1,
                                   0x68,
                                   0x4e,
                          (byte)0x8d, (byte)0x83, (byte)0x9d, (byte)0xaf,
                                     0x4c,
                                   0x04,
                                   0x6c,
                                (byte)0xf4, (byte)0x96, 0x35, (byte)0xa4,
                                     0x75,
                                   (byte)0x81,
                                   0x45,
                                   (byte)0x88, (byte)0xf4, (byte)0xeb
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4,
                          (byte)0x82, 0x3b, 0x0d, (byte)0xd0, 0x71,
                                                    (byte)0xbc,
                                                  0x37,
                                                  0x65,
                                0x0d, (byte)0xa4, (byte)0xc2, 0x58, 0x21,
                                                    0x15,
                                                  0x67,
                                                  (byte)0xce,
                                (byte)0xb0, 0x03, 0x10, (byte)0xc2,
                                                    (byte)0xf6,
                                                  0x0d,
                                                  (byte)0x86,
                                      0x6d, 0x19, 0x29, (byte)0xa3, 0x41,
                                                    0x77,
                                                  0x0e,
                                                  (byte)0xe7,
                                (byte)0xe7, 0x3d, 0x42, 0x67, 0x2d,
                                                    (byte)0xe4,
                                                  0x0e,
                                                  (byte)0xfd,
          (byte)0x95, (byte)0xdc, (byte)0xb1, (byte)0xc7, 0x6c, 0x08, 0x40 }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8, 0x1e, (byte)0x82,
                                (byte)0xc3, 0x58, 0x1d, 0x20, 0x04,
                                     (byte)0x9d,
                                   (byte)0xbf,
                                   0x72,
                                   0x2b, 0x43, 0x1c, (byte)0x8d, 0x19,
                                   (byte)0x83,
                                   (byte)0xfd,
                                   (byte)0xf3, (byte)0xef, (byte)0xb3,
                                   (byte)0xaf,
                                   (byte)0x93,
                                   (byte)0xe2, (byte)0xc5, (byte)0xb6,
                                   (byte)0x95,
                                   (byte)0xed,
                                   (byte)0xcc, 0x68, (byte)0xd8, 0x01, 0x22,
                                   (byte)0xbe,
                                   0x11,
                                   (byte)0xc2, 0x58, 0x18, 0x44, (byte)0x99,
                                   (byte)0xfe,
                                   (byte)0xb7,
                                0x23, 0x36, (byte)0xe6, (byte)0xca, 0x36,
                                     0x36,
                                   (byte)0xe3,
                                   0x17,
                                (byte)0xbe, 0x44, (byte)0xb1, 0x14, 0x51,
                                     0x22,
                                   0x56,
                                   (byte)0x90,
                                   0x57, (byte)0xa3, (byte)0xba, (byte)0xeb
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xc5,
                                      (byte)0x82, 0x3a, 0x30, (byte)0xa2,
                                                    0x34,
                                                  (byte)0xe6,
                                                  (byte)0xc3,
                                      0x58, 0x39, 0x6a, 0x07, 0x25,
                                                    (byte)0x81,
                                                  (byte)0xe5,
                                                  0x29,
                                      (byte)0xf0, 0x42, (byte)0x95,
                                                    (byte)0xfd,
                                                  0x18,
                                                  (byte)0x95,
                                (byte)0xc5, 0x25, 0x56, (byte)0xd4,
                                                    (byte)0x89,
                                                  0x0b,
                                                  (byte)0x8c,
                                (byte)0xad, 0x45, 0x3e, (byte)0xdb,
                                                    (byte)0xc9,
                                                  0x39,
                                                  (byte)0xc8,
                                (byte)0xfd, 0x41, 0x02, (byte)0xad,
                                                    (byte)0xdf,
                                                  0x21,
                                                  (byte)0xd6,
                                0x04, 0x24, (byte)0xf6, 0x55, (byte)0x8d,
                                                    0x79,
                                                  (byte)0xde,
                                                  0x08,
                                      (byte)0x9b, (byte)0xce, 0x26,
                                                    (byte)0xb3,
                                                  (byte)0xf3,
                                                  0x47,
                                      (byte)0x8f, 0x4b, 0x38, 0x51, 0x20,
                                                    0x66,
                                                  (byte)0x82,
                                                  (byte)0xd6,
                                                  (byte)0x94, (byte)0xa8 }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4, (byte)0x82, 0x1b,
                                0x00, 0x00, 0x27, 0x37, (byte)0xf6,
                                     (byte)0x91,
                                   0x48,
                                   (byte)0xe5,
                                (byte)0xc3, 0x58, 0x18, 0x58, (byte)0xfb,
                                     0x1d,
                                   0x37,
                                   (byte)0xfb,
                                (byte)0x95, 0x13, (byte)0xdc, 0x11, 0x57,
                                     0x55,
                                   0x46,
                                   0x58,
                                (byte)0xc6, 0x01, 0x2a, (byte)0xef,
                                     (byte)0x9c,
                                   0x4c,
                                   (byte)0xab,
                                   0x23, 0x72, (byte)0x95, 0x5b
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8,
                                      0x1e, (byte)0x82, (byte)0xc3, 0x58,
                                                    0x25,
                                                  0x52,
                                                  (byte)0x82,
                                            (byte)0xf2, (byte)0xe2,
                                                    (byte)0xb2,
                                                  (byte)0xad,
                                                  (byte)0x81,
                          (byte)0xb1, (byte)0xe7, (byte)0x86, 0x21,
                                                    (byte)0xc3,
                                                  0x0d,
                                                  0x23,
                                (byte)0x92, (byte)0x91, 0x0d, 0x15,
                                                    (byte)0xc6,
                                                  (byte)0xcf,
                                                  0x6b,
                                      (byte)0xdf, 0x2d, (byte)0xcc,
                                                    (byte)0x8f,
                                                  (byte)0x94,
                                                  (byte)0xab,
                          (byte)0xfb, (byte)0xf1, (byte)0xae, 0x7d,
                                                    (byte)0x99,
                                                  0x5e,
                                                  0x6a,
                                0x6a, (byte)0xd7, (byte)0xbe, (byte)0xc2,
                                                    0x4f,
                                                  0x16,
                                                  0x0a,
                          (byte)0x9d, 0x47, 0x34, 0x4b, (byte)0xfb, 0x62,
                                                    0x57,
                                                  0x02,
                                                  0x07,
                                              (byte)0x84, 0x77, 0x5c, 0x33 }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8, 0x1e, (byte)0x82,
                                   (byte)0xc2, 0x51, 0x42, 0x63, (byte)0x9c,
                                   (byte)0xa0,
                                   (byte)0xcc,
                                   (byte)0xd0, 0x7d, (byte)0xfd, (byte)0xab,
                                   (byte)0x98,
                                   0x07,
                                (byte)0xf3, (byte)0xac, (byte)0xd1,
                                     (byte)0xb4,
                                   0x54,
                                   (byte)0x8a,
                                (byte)0xc2, 0x58, 0x20, 0x14, (byte)0xb6,
                                     0x42,
                                   0x55,
                                   (byte)0xed,
                                (byte)0xe3, 0x4b, 0x0c, 0x4e, (byte)0xf4,
                                     0x3d,
                                   0x55,
                                   0x60,
                                   (byte)0xac, (byte)0xf6, (byte)0xdb, 0x3b,
                                   (byte)0xe3,
                                   (byte)0xec,
                                   (byte)0x81, (byte)0x93, 0x6d, (byte)0xa8,
                                   (byte)0x9f,
                                   0x58,
                                   (byte)0xc2, 0x4f, 0x4e, 0x1c, (byte)0xda,
                                   0x68, (byte)0x8a
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4,
                                (byte)0x82, 0x1a, 0x00, 0x4f, 0x01, 0x53,
                                                    0x1a,
                                                  0x14,
                                                  (byte)0xe4,
                                                  0x07, (byte)0x88 }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4, (byte)0x82, 0x1a,
                          0x06, (byte)0x93, 0x34, 0x2c, (byte)0xc2, 0x58,
                                     0x31,
                                   0x42,
                                   0x0e,
                                   (byte)0xfa, 0x5c, (byte)0xb4, (byte)0xe6,
                                   (byte)0xed,
                                   (byte)0x8c,
                          (byte)0xf4, 0x23, 0x76, (byte)0xe6, 0x46,
                                     (byte)0xfe,
                                   0x4f,
                                   0x6f,
                                   (byte)0xed, 0x0c, 0x54, (byte)0xce, 0x28,
                                   0x2d,
                                   (byte)0x93,
                          (byte)0xd9, (byte)0x85, (byte)0x91, 0x04,
                                     (byte)0x90,
                                   0x48,
                                   0x69,
                                (byte)0xb1, (byte)0xea, 0x00, (byte)0x9f,
                                     0x1e,
                                   (byte)0xf4,
                                   0x7d,
                                0x0b, 0x5d, (byte)0xf6, 0x2e, (byte)0xef,
                                     0x0b,
                                   0x35,
                                   0x37,
                                   (byte)0xf5, 0x5f, 0x4b, (byte)0xa8
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8,
                          0x1e, (byte)0x82, (byte)0xc2, 0x58, 0x3b, 0x45,
                                                    0x4f,
                                                  0x0a,
                                                  0x18,
                          (byte)0xca, 0x6f, (byte)0xa3, 0x01, 0x38, 0x01,
                                                    0x63,
                                                  0x7b,
                                                  0x50,
                                (byte)0xf6, 0x12, (byte)0x8b, (byte)0xbd,
                                                    0x5d,
                                                  (byte)0xac,
                                                  0x58,
                                (byte)0x9d, (byte)0xde, 0x27, 0x59,
                                                    (byte)0xea,
                                                  0x11,
                                                  0x12,
                                            (byte)0x88, (byte)0x81,
                                                    (byte)0xe0,
                                                  (byte)0xd3,
                                                  (byte)0xe5,
                                            (byte)0xfc, (byte)0xb4,
                                                    (byte)0x8b,
                                                  (byte)0x9b,
                                                  (byte)0x9f,
                                      (byte)0xa5, 0x65, 0x32, 0x75, 0x2a,
                                                    0x2f,
                                                  (byte)0xd2,
                                                  0x04,
                          (byte)0x9d, (byte)0xf6, 0x4d, 0x75, 0x77,
                                                    (byte)0xf2,
                                                  0x21,
                                                  0x3e,
                                      0x19, (byte)0xb2, (byte)0x94,
                                                    (byte)0xa5,
                                                  (byte)0xa7,
                                                  (byte)0x94,
                                      (byte)0xc2, 0x58, 0x2e, 0x19,
                                                    (byte)0x8e,
                                                  (byte)0xa7,
                                                  (byte)0xb2,
                                (byte)0x98, (byte)0xb3, (byte)0xbc,
                                                    (byte)0xa5,
                                                  (byte)0xc4,
                                                  0x50,
                                      (byte)0xed, 0x49, (byte)0x9a, 0x27,
                                                    0x03,
                                                  (byte)0xfc,
                                                  0x0a,
                                      (byte)0xf3, 0x70, (byte)0x8e, 0x2e,
                                                    0x61,
                                                  0x18,
                                                  (byte)0xcd,
                                (byte)0xd5, (byte)0xc8, (byte)0xfd,
                                                    (byte)0xa6,
                                                  (byte)0x8d,
                                                  0x3b,
                                (byte)0xc5, (byte)0xa7, 0x40, (byte)0xd7,
                                                    0x5c,
                                                  (byte)0xd6,
                                                  0x1a,
                                            (byte)0xf6, (byte)0xee, 0x10,
                                                    0x72,
                                                  (byte)0xf7,
                                                  (byte)0x8e,
                                        (byte)0xc0, (byte)0x80, (byte)0x94 }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8, 0x1e, (byte)0x82,
                                   (byte)0xc3, 0x58, 0x3a, 0x2f, (byte)0xae,
                                   (byte)0x80,
                                   (byte)0x9f,
                                   0x14, (byte)0xcd, (byte)0xca, (byte)0xf7,
                                   (byte)0xd6,
                                   (byte)0xc9,
                          (byte)0xaa, 0x02, (byte)0x85, 0x2f, 0x28, 0x14,
                                     0x7b,
                                   0x1e,
                                   0x68,
                                   0x79, 0x17, 0x40, 0x6b, (byte)0xde, 0x4a,
                                   (byte)0xe2,
                                   (byte)0x83,
                                   (byte)0xab, (byte)0xb1, (byte)0x84,
                                   (byte)0xe0,
                                   (byte)0x85,
                                   (byte)0xd0, (byte)0xd7, 0x72, 0x58, 0x5c,
                                   (byte)0x8c,
                                   (byte)0xef,
                          (byte)0xd5, (byte)0xef, 0x28, 0x4a, (byte)0xe9,
                                     0x13,
                                   0x40,
                                   0x73,
                                (byte)0xe5, 0x2a, 0x70, 0x00, 0x7f,
                                     (byte)0xc7,
                                   0x70,
                                   (byte)0xb0,
                                (byte)0xac, 0x13, 0x14, (byte)0xc2, 0x58,
                                     0x19,
                                   0x14,
                                   0x15,
                                   (byte)0xbb, (byte)0xbf, 0x06, 0x67, 0x46,
                                   0x1e,
                                   (byte)0x98,
                                (byte)0xa4, (byte)0xb6, 0x27, (byte)0xd8,
                                     0x4a,
                                   0x3f,
                                   0x69,
                                   (byte)0xe2, 0x79, (byte)0xd9, (byte)0xd7,
                                   (byte)0xed,
                                   (byte)0xe7,
                                   (byte)0xc9, (byte)0xe2, 0x34
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4,
                                (byte)0x82, 0x3b, 0x00, 0x00, 0x00, 0x01,
                                                    0x1c,
                                                  (byte)0x8f,
                                                  0x5d,
                                0x3d, (byte)0xc3, 0x4c, 0x6c, 0x77, 0x44,
                                                    0x6f,
                                                  (byte)0xcc,
                                                  0x57,
          (byte)0xad, (byte)0x99, 0x1c, (byte)0xbc, (byte)0xca, (byte)0x8a }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8, 0x1e, (byte)0x82,
                          0x3b, 0x00, 0x2a, 0x4e, (byte)0xf7, (byte)0x87,
                                     0x4c,
                                   0x14,
                                   0x50,
                                (byte)0xc2, 0x58, 0x3a, 0x48, (byte)0xed,
                                     0x45,
                                   0x49,
                                   (byte)0xa1,
                                   0x4d, 0x48, (byte)0x80, (byte)0xfc,
                                   (byte)0xa4,
                                   (byte)0x96,
                                   (byte)0xce, (byte)0xc0, (byte)0xfb, 0x23,
                                   (byte)0x81,
                                   (byte)0xc4,
                          (byte)0xfe, 0x56, (byte)0x9b, 0x55, (byte)0xac,
                                     0x74,
                                   0x77,
                                   0x39,
                                0x00, 0x1a, 0x37, (byte)0xe5, (byte)0xfe,
                                     0x42,
                                   0x63,
                                   (byte)0x9b,
                                   0x6f, 0x15, 0x21, (byte)0x98, (byte)0xb8,
                                   0x29,
                                   (byte)0xf5,
                                (byte)0x85, (byte)0xda, 0x20, (byte)0xe5,
                                     0x3b,
                                   0x0f,
                                   (byte)0xa9,
                                   0x3d, 0x10, 0x3c, (byte)0xe9, (byte)0xce,
                                   (byte)0x9c,
                                   (byte)0xd6,
                                   0x5e, (byte)0xa6, 0x16, 0x55
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4,
                                      (byte)0x82, 0x3a, 0x00, (byte)0x8d,
                                                    0x14,
                                                  (byte)0x9b,
                                                  (byte)0xc3,
                                0x58, 0x25, 0x43, 0x65, 0x68, 0x79,
                                                    (byte)0x9c,
                                                  0x24,
                                                  (byte)0x95,
                                0x56, 0x37, (byte)0xaa, (byte)0xd2, 0x3e,
                                                    0x46,
                                                  0x18,
                                                  (byte)0xf4,
                                      (byte)0xef, 0x31, 0x1b, 0x3e,
                                                    (byte)0xa7,
                                                  (byte)0xce,
                                                  0x18,
                                      (byte)0xbe, (byte)0xdf, (byte)0xd4,
                                                    0x12,
                                                  (byte)0x94,
                                                  (byte)0x97,
                                0x47, (byte)0xb7, 0x14, (byte)0xc0,
                                                    (byte)0x8e,
                                                  0x07,
                                                  (byte)0xc3,
                                                  0x00, (byte)0xae }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8, 0x1e, (byte)0x82,
                                (byte)0xc3, 0x54, 0x4e, 0x49, 0x4b,
                                     (byte)0xeb,
                                   0x09,
                                   (byte)0xcc,
                                   (byte)0xd6, 0x7d, (byte)0x95, (byte)0x8c,
                                   (byte)0xc8,
                                   0x34,
                                   (byte)0xc4, 0x69, (byte)0x9b, (byte)0xc5,
                                   (byte)0x9a,
                                   0x5a,
                                (byte)0xa4, 0x72, (byte)0xc2, 0x58, 0x34,
                                     0x6f,
                                   (byte)0xb1,
                                   0x4c,
                          (byte)0x9b, 0x74, (byte)0x8f, (byte)0xf0,
                                     (byte)0x9a,
                                   0x56,
                                   0x39,
                                (byte)0x91, (byte)0xe6, (byte)0xbd,
                                     (byte)0xca,
                                   (byte)0x91,
                                   0x38,
                                   0x4f, 0x2f, (byte)0xf9, (byte)0x92,
                                   (byte)0xfe,
                                   (byte)0x85,
                                (byte)0xe4, 0x06, 0x59, (byte)0xb8,
                                     (byte)0x84,
                                   0x1a,
                                   (byte)0x83,
                          (byte)0x9b, 0x0e, 0x73, 0x30, (byte)0xfe,
                                     (byte)0xdf,
                                   0x2d,
                                   0x6c,
                                0x3b, (byte)0xfd, 0x0a, 0x64, 0x56,
                                     (byte)0xab,
                                   0x6f,
                                   (byte)0xd6,
                                   (byte)0x8c, 0x60, (byte)0x90, 0x1b, 0x7d,
                                   (byte)0xc7, (byte)0xef
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4,
                                      (byte)0x82, 0x1b, 0x00, 0x00, 0x00,
                                                    0x24,
                                                  (byte)0x86,
                                                  (byte)0xe1,
                                (byte)0x8e, (byte)0xfd, (byte)0xc3, 0x4f,
                                                    0x50,
                                                  0x71,
                                                  (byte)0xea,
                                      (byte)0xb9, 0x16, 0x4f, 0x3e, 0x0a,
                                                    0x66,
                                                  (byte)0xda,
                                                  0x12,
                                  (byte)0xf5, (byte)0xbd, (byte)0xf0, 0x14 }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8, 0x1e, (byte)0x82,
                                   (byte)0xc2, 0x58, 0x3a, 0x6b, 0x72, 0x30,
                                   (byte)0xe4,
                                   (byte)0xeb,
                                   (byte)0xbd, 0x3d, (byte)0xa9, (byte)0xca,
                                   (byte)0xee,
                                   0x03,
                                   (byte)0xbb, (byte)0xb1, (byte)0xcb,
                                   (byte)0xe8,
                                   (byte)0xd8,
                                (byte)0xc5, (byte)0xbc, (byte)0xfe,
                                     (byte)0xa2,
                                   (byte)0xa1,
                                   0x58,
                                (byte)0xce, (byte)0xfd, (byte)0xd2,
                                     (byte)0xf9,
                                   0x7f,
                                   (byte)0xc2,
                                0x39, 0x49, (byte)0xd8, 0x52, 0x41, 0x06,
                                     0x61,
                                   0x41,
                                   (byte)0xbc,
                                0x7e, (byte)0x9b, 0x68, (byte)0xa7,
                                     (byte)0xf4,
                                   (byte)0xc3,
                                   0x58,
                                   (byte)0xf0, 0x7e, 0x73, 0x77, (byte)0xf8,
                                   (byte)0x81,
                                   0x09,
                                (byte)0x88, 0x48, (byte)0x80, (byte)0xa5,
                                     0x79,
                                   0x22,
                                   0x23,
                                (byte)0xc2, 0x58, 0x1e, 0x6c, (byte)0xc7,
                                     0x0a,
                                   0x54,
                                   0x26,
                          (byte)0xe1, (byte)0x84, 0x6a, 0x6a, 0x5b, 0x0a,
                                     0x5f,
                                   0x41,
                                   0x3b,
                                0x6d, 0x66, (byte)0xf5, 0x47, 0x19,
                                     (byte)0xe5,
                                   0x71,
                                   0x0f,
                                   (byte)0xcb, 0x1b, (byte)0xf0, (byte)0xb4,
                                   (byte)0xbe,
                                   0x3b,
                                   (byte)0x9a, 0x03
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4,
                                                  (byte)0x82, 0x1a, 0x00, 0x07,
                                                  (byte)0xbb, 0x62,
                                                  0x1b,
                      0x00, 0x00, 0x00, 0x29, 0x43, 0x5d, 0x7b, (byte)0xe8 }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4, (byte)0x82, 0x3b,
                                0x00, 0x00, 0x00, 0x0f, (byte)0x93,
                                     (byte)0xd8,
                                   (byte)0xb1,
                                   0x6a,
                          (byte)0xc3, 0x58, 0x25, 0x25, 0x54, 0x48,
                                     (byte)0x8f,
                                   0x4c,
                                   0x2a,
                                   0x2e, 0x09, 0x65, 0x52, 0x1b, (byte)0x8b,
                                   (byte)0xcf,
                                   (byte)0xbb,
                                   0x13, (byte)0xf3, (byte)0xc7, (byte)0xf1,
                                   (byte)0x93,
                                   (byte)0xc6,
                                (byte)0xc2, 0x4f, 0x6c, 0x54, 0x2b, 0x00,
                                     0x5c,
                                   (byte)0xab,
                                   0x35,
                                   0x75, 0x2f, (byte)0x98, 0x71, 0x51, 0x75,
                                   0x4b, (byte)0xf5
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8,
                                0x1e, (byte)0x82, (byte)0xc3, 0x58, 0x1d,
                                                    0x7b,
                                                  (byte)0xaf,
                                                  0x6d,
                                            (byte)0xd7, (byte)0xb5,
                                                    (byte)0xa1,
                                                  (byte)0xb8,
                                                  (byte)0xba,
                          (byte)0xef, (byte)0xd7, (byte)0x92, (byte)0xba,
                                                    0x6d,
                                                  0x0a,
                                                  0x62,
                                (byte)0xd5, (byte)0x98, 0x7d, 0x3f,
                                                    (byte)0xcb,
                                                  (byte)0xab,
                                                  0x6c,
                                0x1d, 0x35, 0x59, 0x24, 0x46, 0x40,
                                                    (byte)0x90,
                                                  (byte)0xc2,
                                                  0x58,
                                      0x20, 0x16, (byte)0xd0, 0x18,
                                                    (byte)0xc6,
                                                  (byte)0xd7,
                                                  (byte)0xb1,
                          (byte)0xce, (byte)0xdd, (byte)0xf3, (byte)0xc1,
                                                    0x48,
                                                  0x75,
                                                  0x0c,
                                0x1d, 0x0c, (byte)0x9a, 0x2f, 0x05,
                                                    (byte)0x8b,
                                                  0x0f,
                                                  (byte)0xde,
                                      0x23, (byte)0x88, 0x59, (byte)0x8c,
                                                    0x42,
                                                  (byte)0xb5,
                                                  0x72,
                                  (byte)0x97, 0x44, (byte)0xfb, (byte)0x86 }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8, 0x1a, (byte)0xd8,
                                0x1e, (byte)0x82, (byte)0xc2, 0x58, 0x1f,
                                     0x03,
                                   (byte)0x9f,
                                   0x5d,
                                (byte)0x81, (byte)0x96, (byte)0xff,
                                     (byte)0xcb,
                                   0x19,
                                   (byte)0x8f,
                                0x07, 0x25, (byte)0xe9, (byte)0xf1,
                                     (byte)0xe9,
                                   0x6b,
                                   0x0a,
                                   (byte)0xb3, 0x67, (byte)0xfc, (byte)0xb1,
                                   (byte)0xe4,
                                   0x2c,
                          (byte)0xd6, (byte)0xef, (byte)0xce, (byte)0xfb,
                                     0x0d,
                                   0x25,
                                   0x78,
                                0x1d, (byte)0xf0, (byte)0xc2, 0x58, 0x31,
                                     0x77,
                                   0x40,
                                   (byte)0xea,
                                0x07, (byte)0x8e, (byte)0x8d, 0x02,
                                     (byte)0xda,
                                   0x24,
                                   (byte)0xb7,
                                (byte)0xb3, 0x14, (byte)0xa0, (byte)0x8f,
                                     0x07,
                                   0x7a,
                                   0x5f,
                                (byte)0xe2, 0x1d, 0x4d, 0x4f, 0x5c, 0x24,
                                     0x37,
                                   (byte)0xc7,
                                   0x64,
                                   (byte)0xb0, 0x36, 0x22, (byte)0xa3, 0x66,
                                   (byte)0xec,
                                   (byte)0xe2,
                                (byte)0xb0, 0x3a, 0x58, (byte)0xbc, 0x56,
                                     0x58,
                                   0x74,
                                   (byte)0xca,
                                   (byte)0xb2, 0x09, 0x28, (byte)0xcb, 0x57,
                                   (byte)0xd0,
                                   (byte)0xf0,
    (byte)0xfc }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4,
                                                  (byte)0x82, 0x1b, 0x00,
                                                             0x1b,
                                                        (byte)0xd3, 0x64,
                                                             0x45,
                                (byte)0xce, 0x21, 0x46, (byte)0xc2, 0x58,
                                                             0x1d,
                                                           0x47,
                                                           0x3d,
                                      (byte)0xdb, (byte)0xb3, 0x46, 0x57,
                                                             0x1f,
                                                           (byte)0xee,
                                                           (byte)0xa3,
                                (byte)0x84, 0x5c, 0x01, (byte)0xd6,
                                                             (byte)0xa0,
                                                           0x5a,
                                                           (byte)0xaa,
                                      0x71, 0x21, 0x65, 0x48, (byte)0xbe,
                                                             0x26,
                                                           0x07,
                                                           (byte)0x86,
                                  (byte)0xae, 0x29, 0x2a, (byte)0xd5, 0x37 }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4, (byte)0x82, 0x1b,
                                0x00, 0x00, 0x08, (byte)0xd6, 0x39,
                                     (byte)0xec,
                                   0x14,
                                   0x07,
                                   (byte)0xc3, 0x58, 0x2d, 0x38, 0x68, 0x32,
                                   (byte)0xe5,
                                   (byte)0xdf,
                                (byte)0xf3, (byte)0xb4, (byte)0x84,
                                     (byte)0xbe,
                                   (byte)0xf8,
                                   0x72,
                                (byte)0xa9, 0x68, (byte)0xcd, 0x0c, 0x13,
                                     0x62,
                                   0x43,
                                   0x19,
                                (byte)0xff, 0x77, (byte)0xe7, 0x70,
                                     (byte)0xf5,
                                   (byte)0x85,
                                   0x22,
                                   0x23, 0x1c, 0x72, 0x0b, (byte)0x9e, 0x43,
                                   (byte)0xa6,
                                   (byte)0xee,
                                (byte)0x81, 0x18, 0x76, 0x0b, (byte)0xb4,
                                     0x4f,
                                   (byte)0x97,
                                   0x27,
                                   0x39, 0x13, 0x78
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xda, 0x00,
                                                  (byte)0xef, 0x7f, 0x16,
                                                  (byte)0xd8,
                                                  0x1e,
                                      (byte)0x82, (byte)0xc3, 0x58, 0x2a,
                                                    0x49,
                                                  0x3c,
                                                  (byte)0xb2,
                                (byte)0x89, (byte)0xa2, 0x37, (byte)0xc4,
                                                    0x5d,
                                                  (byte)0xbf,
                                                  0x0b,
                                      0x4c, 0x77, 0x2c, 0x05, 0x32,
                                                    (byte)0xa7,
                                                  (byte)0x85,
                                                  (byte)0xe2,
                                0x31, 0x20, 0x61, (byte)0xa8, 0x06,
                                                    (byte)0xd3,
                                                  (byte)0xe2,
                                                  0x71,
                                0x06, 0x05, (byte)0xe6, (byte)0x8a,
                                                    (byte)0xa1,
                                                  0x03,
                                                  (byte)0xce,
                                0x2c, (byte)0xb6, (byte)0xba, 0x28,
                                                    (byte)0xfb,
                                                  (byte)0xb1,
                                                  0x5c,
                          (byte)0x97, (byte)0x85, (byte)0xc2, 0x58, 0x29,
                                                    0x5b,
                                                  0x73,
                                                  0x34,
                          0x0f, (byte)0x9f, (byte)0xa4, (byte)0x81,
                                                    (byte)0x82,
                                                  0x63,
                                                  0x6b,
                          0x16, 0x4e, 0x62, (byte)0x9d, (byte)0xcc,
                                                    (byte)0xe4,
                                                  0x06,
                                                  0x17,
                                0x35, (byte)0xc7, 0x52, (byte)0xf4,
                                                    (byte)0xe2,
                                                  0x28,
                                                  (byte)0xe7,
                                      0x12, 0x4b, 0x7b, 0x06, 0x77,
                                                    (byte)0xa3,
                                                  (byte)0xdd,
                                                  (byte)0xeb,
          0x70, 0x76, (byte)0xe6, (byte)0xa9, 0x16, 0x74, 0x29, (byte)0x86 }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4, (byte)0x82, 0x1b,
                                   0x00, 0x00, 0x00, 0x01, (byte)0x84,
                                   (byte)0xfa,
                                   (byte)0xb7,
                                (byte)0xe1, (byte)0xc3, 0x57, 0x54,
                                     (byte)0xe6,
                                   0x76,
                                   0x1b,
                                   (byte)0xe8, 0x78, (byte)0x92, 0x28, 0x39,
                                   0x57,
                                   (byte)0x8f,
                                (byte)0xbb, (byte)0xab, (byte)0xb8,
                                     (byte)0x8e,
                                   0x42,
                                   0x56,
                                   (byte)0xe1, (byte)0x82, (byte)0x82, 0x51,
                                   0x07, (byte)0xa9
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xc5,
                                (byte)0x82, 0x1b, (byte)0x80, (byte)0xbc,
                                                    0x30,
                                                  (byte)0xc1,
                                                  0x2b,
                                      0x01, 0x37, (byte)0x90, (byte)0xc3,
                                                    0x51,
                                                  0x21,
                                                  (byte)0x8b,
                                      (byte)0xa8, (byte)0xda, (byte)0xf7,
                                                    0x15,
                                                  0x4a,
                                                  (byte)0x95,
                                (byte)0x87, 0x79, 0x1e, 0x49, (byte)0xad,
                                                    0x3c,
                                                  (byte)0xba,
                                                  0x41,
                                                  0x2d }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4, (byte)0x82, 0x3a,
                          0x00, 0x0e, 0x31, (byte)0xb4, 0x3b, 0x00, 0x00,
                                     0x00,
                                   0x0e,
                                   0x2d,
                                   (byte)0xbf, (byte)0xb4, (byte)0xcb
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8,
                                      0x1e, (byte)0x82, (byte)0xc3, 0x58,
                                                    0x38,
                                                  0x77,
                                                  (byte)0x9d,
                                (byte)0x81, 0x19, 0x13, 0x4a, 0x2e, 0x58,
                                                    0x71,
                                                  0x70,
                                                  (byte)0x90,
                                      (byte)0xc2, (byte)0xb9, (byte)0xd1,
                                                    0x0a,
                                                  (byte)0xd6,
                                                  (byte)0xfb,
                          0x6b, 0x3d, 0x68, (byte)0xf2, 0x68, (byte)0x84,
                                                    0x06,
                                                  0x0b,
                                                  0x5a,
                                (byte)0xdf, (byte)0xe6, (byte)0xac, 0x51,
                                                    0x7e,
                                                  (byte)0xf4,
                                                  0x29,
                                (byte)0xe9, 0x17, 0x05, 0x79, (byte)0xe7,
                                                    0x4c,
                                                  0x72,
                                                  0x04,
                                      (byte)0xcc, 0x71, (byte)0xa6,
                                                    (byte)0xad,
                                                  (byte)0xc5,
                                                  (byte)0xc7,
                          0x05, (byte)0x91, (byte)0xa3, 0x3c, (byte)0x98,
                                                    0x18,
                                                  0x72,
                                                  0x49,
                                (byte)0xa1, (byte)0xc2, 0x58, 0x22, 0x65,
                                                    0x60,
                                                  0x0a,
                                                  (byte)0xe6,
                          (byte)0x9a, (byte)0xdb, 0x00, (byte)0xb0,
                                                    (byte)0xce,
                                                  0x65,
                                                  0x72,
                                0x11, (byte)0x89, 0x0b, (byte)0xbd,
                                                    (byte)0xbb,
                                                  0x33,
                                                  (byte)0xab,
                                      (byte)0x9b, (byte)0xa9, 0x48,
                                                    (byte)0xe3,
                                                  0x60,
                                                  (byte)0xad,
                                      (byte)0xaa, (byte)0x99, (byte)0xe5,
                                                    0x72,
                                                  (byte)0xd2,
                                                  (byte)0xfd,
                                  0x41, (byte)0x96, (byte)0xa7, (byte)0x82 }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4, (byte)0x82, 0x1a,
                          0x01, (byte)0xea, 0x05, (byte)0x8b, (byte)0xc2,
                                     0x58,
                                   0x2b,
                                   0x47,
                                (byte)0x81, 0x70, 0x43, (byte)0x97,
                                     (byte)0xf4,
                                   (byte)0x91,
                                   0x2f,
                                   0x67, 0x3e, 0x7a, (byte)0xa6, 0x15,
                                   (byte)0x9e,
                                   (byte)0x9f,
                                   (byte)0x97, 0x40, (byte)0xea, (byte)0xd1,
                                   (byte)0xc0,
                                   (byte)0xda,
                                   0x25, (byte)0x8d, (byte)0xa2, 0x2e,
                                   (byte)0xc3,
                                   (byte)0xe1,
                                   (byte)0xc9, 0x15, 0x43, 0x71, (byte)0xd3,
                                   (byte)0xa5,
                                   0x55,
                                   (byte)0x86, 0x7d, 0x05, 0x5d, (byte)0xdc,
                                   0x48,
                                   (byte)0xdb,
                                   (byte)0xc1, 0x6c
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8, 0x1e,
                                                  (byte)0x82, (byte)0xc2, 0x58,
                                                  0x2e,
                                                  0x3c,
                                      (byte)0xc5, 0x71, (byte)0xe2, 0x4c,
                                                    0x69,
                                                  0x25,
                                                  (byte)0xce,
                                (byte)0xb0, 0x70, 0x77, 0x2e, (byte)0xa3,
                                                    0x1f,
                                                  0x55,
                                                  (byte)0xe3,
                                0x1f, (byte)0xb1, (byte)0xb9, 0x4e,
                                                    (byte)0xa9,
                                                  (byte)0xe5,
                                                  0x35,
                                            (byte)0xa3, (byte)0xb6, 0x3a,
                                                    0x7c,
                                                  (byte)0x94,
                                                  (byte)0xd7,
                                (byte)0xe4, 0x0c, 0x57, (byte)0xe0,
                                                    (byte)0xf4,
                                                  0x03,
                                                  (byte)0x93,
                                0x5a, (byte)0x80, 0x25, 0x28, (byte)0x84,
                                                    0x10,
                                                  (byte)0x83,
                                                  0x0a,
                                (byte)0xf0, (byte)0xc9, (byte)0xc2, 0x58,
                                                    0x2a,
                                                  0x67,
                                                  (byte)0xd6,
                          0x33, (byte)0xd6, 0x79, 0x38, (byte)0xbd, 0x6d,
                                                    0x71,
                                                  0x53,
                                                  0x3f,
                                      (byte)0xc3, 0x31, 0x6e, (byte)0xa6,
                                                    0x4c,
                                                  (byte)0xe7,
                                                  0x1a,
                                      (byte)0xbe, (byte)0xaf, (byte)0xeb,
                                                    0x7e,
                                                  (byte)0xcc,
                                                  (byte)0xe7,
                                            0x40, 0x59, (byte)0xbc,
                                                    (byte)0xd7,
                                                  (byte)0xf8,
                                                  (byte)0x93,
                                (byte)0xab, (byte)0xce, 0x2e, 0x58,
                                                    (byte)0x9c,
                                                  (byte)0xf2,
                                                  0x10,
                                        0x4e, 0x59, (byte)0xe0, 0x26, 0x74 }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4, (byte)0x82, 0x3b,
                                   0x00, 0x00, 0x00, 0x58, 0x30, (byte)0xd2,
                                   0x37,
                                   (byte)0xd7,
                                (byte)0xc2, 0x58, 0x1c, 0x2d, 0x10,
                                     (byte)0xe5,
                                   0x04,
                                   0x75,
                                   (byte)0x8a, 0x75, (byte)0xf1, 0x2c, 0x28,
                                   (byte)0xab,
                                   (byte)0xeb,
                                   (byte)0xcd, 0x47, (byte)0xf1, (byte)0x8c,
                                   0x3b,
                                   (byte)0xf8,
                                   (byte)0x93, 0x2b, (byte)0xee, (byte)0xd9,
                                   (byte)0x9b,
                                   (byte)0xe6,
                                   (byte)0xba, (byte)0xde, (byte)0xc4,
    (byte)0x99 }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8,
                                0x1e, (byte)0x82, (byte)0xc2, 0x58, 0x3c,
                                                             0x71,
                                                           (byte)0xb3,
                                                           0x06,
                          (byte)0xa3, 0x1c, 0x29, (byte)0xcd, 0x4c, 0x52,
                                                             0x0c,
                                                           0x0c,
                                                           0x3a,
                                (byte)0xe8, 0x35, 0x08, (byte)0xcc, 0x46,
                                                             0x77,
                                                           0x78,
                                                           (byte)0x94,
                          (byte)0xe6, (byte)0x83, 0x73, 0x74, 0x1a,
                                                             (byte)0xc5,
                                                           0x34,
                                                           0x70,
                                      (byte)0x83, 0x5c, 0x48, 0x65,
                                                             (byte)0xe0,
                                                           0x68,
                                                           (byte)0xb6,
                                      (byte)0xab, 0x12, 0x29, 0x11, 0x03,
                                                             0x4c,
                                                           (byte)0xdd,
                                                           (byte)0x99,
                          (byte)0xe7, (byte)0x97, (byte)0xa0, (byte)0x88,
                                                             0x19,
                                                           0x6a,
                                                           0x00,
                                (byte)0xb1, 0x0e, (byte)0xa8, 0x09,
                                                             (byte)0xfd,
                                                           (byte)0x93,
                                                           0x16,
                                0x60, 0x28, (byte)0xce, (byte)0xc2, 0x58,
                                                             0x2c,
                                                           0x71,
                                                           0x11,
                                      (byte)0x95, (byte)0xf9, (byte)0xfe,
                                                             0x24,
                                                           (byte)0xc7,
                                                           (byte)0xab,
                                      0x36, 0x4e, (byte)0x82, 0x32,
                                                             (byte)0xfc,
                                                           (byte)0x8b,
                                                           (byte)0xd2,
                                      (byte)0xc7, 0x45, 0x58, 0x36, 0x0a,
                                                             0x1b,
                                                           (byte)0x82,
                                                           (byte)0xe5,
                          (byte)0xba, (byte)0xba, (byte)0xc7, 0x0d,
                                                             (byte)0xc6,
                                                           0x53,
                                                           0x0b,
                                      0x6c, (byte)0xdf, (byte)0xf2,
                                                             (byte)0x8e,
                                                           (byte)0xd9,
                                                           (byte)0x94,
                      0x3c, 0x08, 0x15, 0x07, (byte)0xac, 0x5e, 0x56, 0x16 }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8, 0x1e, (byte)0x82,
                                   (byte)0xc3, 0x58, 0x33, 0x58, 0x36, 0x69,
                                   (byte)0xa1,
                                   0x1c,
                                   (byte)0xd4, (byte)0xa2, 0x66, 0x7e,
                                   (byte)0xed,
                                   (byte)0xcf,
                                   (byte)0xe1, 0x19, 0x11, 0x47, 0x5e, 0x38,
                                   0x35,
                                   (byte)0xc4,
                                (byte)0xf6, 0x65, (byte)0xff, 0x53, 0x1f,
                                     0x14,
                                   0x25,
                                   0x7c,
                                   (byte)0x84, (byte)0xb4, 0x32, 0x72,
                                   (byte)0xe6,
                                   (byte)0xa1,
                          (byte)0xba, 0x63, 0x2f, 0x5f, 0x26, 0x20,
                                     (byte)0xd4,
                                   0x4b,
                                   0x2e,
                                (byte)0xfe, 0x59, 0x09, 0x2a, 0x21, 0x49,
                                     0x3d,
                                   0x32,
                                   (byte)0xc5,
                          (byte)0xc2, 0x58, 0x1e, 0x4e, 0x69, 0x29,
                                     (byte)0xe0,
                                   0x27,
                                   0x09,
                                0x36, 0x50, 0x61, 0x72, 0x57, 0x15, 0x6a,
                                     0x1f,
                                   0x70,
                                   0x54,
                          (byte)0xdf, 0x14, 0x3f, 0x04, 0x51, 0x48,
                                     (byte)0xba,
                                   0x5c,
                                   0x09,
                                   0x32, (byte)0xf4, 0x54, (byte)0xee, 0x4c
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4,
                                      (byte)0x82, 0x1a, 0x03, 0x48,
                                                    (byte)0xc6,
                                                  (byte)0x86,
                                                  (byte)0xc3,
                                0x58, 0x32, 0x6c, 0x7f, (byte)0xcb,
                                                    (byte)0xcc,
                                                  (byte)0xfb,
                                                  0x42,
                                (byte)0xb3, 0x5e, 0x4f, (byte)0x90,
                                                    (byte)0xc7,
                                                  0x2c,
                                                  (byte)0xa5,
                                      (byte)0xd1, (byte)0xa9, (byte)0xcc,
                                                    0x34,
                                                  0x1b,
                                                  (byte)0xa4,
                                (byte)0xab, 0x01, (byte)0xe1, (byte)0xb4,
                                                    0x1a,
                                                  0x1b,
                                                  0x20,
                                      (byte)0xc2, 0x60, (byte)0xe2,
                                                    (byte)0xb1,
                                                  (byte)0xd0,
                                                  (byte)0xd8,
                                      0x09, (byte)0xe6, 0x06, 0x7e, 0x03,
                                                    0x1b,
                                                  0x63,
                                                  (byte)0x99,
                                      (byte)0x96, 0x4e, 0x29, 0x2a, 0x41,
                                                    0x24,
                                                  (byte)0x99,
                                                  0x29,
                                                  (byte)0xdd, 0x11 }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4, (byte)0x82, 0x3b,
                                0x00, 0x01, (byte)0x97, (byte)0xcf, 0x67,
                                     0x3d,
                                   (byte)0xeb,
                                   0x15,
                                   (byte)0xc2, 0x58, 0x25, 0x26, (byte)0xe2,
                                   (byte)0x87,
                                   0x03,
                                   (byte)0xe4, (byte)0xb2, (byte)0xaa, 0x68,
                                   (byte)0x91,
                                   0x2f,
                                (byte)0xbf, (byte)0xc6, (byte)0xf5,
                                     (byte)0xf6,
                                   0x24,
                                   (byte)0xc6,
                                0x5b, (byte)0xaa, 0x29, (byte)0xdb,
                                     (byte)0xda,
                                   0x2e,
                                   (byte)0x93,
                                (byte)0x96, 0x49, (byte)0xfd, 0x3e, 0x2d,
                                     0x47,
                                   0x46,
                                   (byte)0xb6,
                                   (byte)0xe9, (byte)0xb9, 0x0b, (byte)0x9b,
                                   (byte)0x83, (byte)0xce
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8,
                                0x1e, (byte)0x82, (byte)0xc2, 0x49, 0x18,
                                                    0x6f,
                                                  0x19,
                                                  (byte)0xf9,
                          0x72, 0x4d, (byte)0x82, 0x4b, (byte)0xf0,
                                                    (byte)0xc2,
                                                  0x4d,
                                                  0x18,
                                (byte)0xc6, 0x07, (byte)0x81, 0x5c,
                                                    (byte)0xe7,
                                                  (byte)0xc6,
                                                  0x41,
                            0x1b, (byte)0xc9, (byte)0xba, (byte)0xf6, 0x75 }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4, (byte)0x82, 0x1b,
                                0x00, 0x00, 0x00, 0x05, 0x59, 0x47,
                                     (byte)0xdc,
                                   0x6c,
                                   (byte)0xc2,
                                0x58, 0x22, 0x7e, (byte)0xd8, 0x2d, 0x59,
                                     0x0b,
                                   (byte)0x8e,
                                   0x0b,
                          0x33, 0x4f, (byte)0xae, 0x6c, (byte)0xbc, 0x23,
                                     0x43,
                                   0x49,
                                   0x18,
                          (byte)0xca, 0x53, (byte)0x85, (byte)0xc8,
                                     (byte)0xc0,
                                   0x5a,
                                   0x39,
                                   0x01, 0x01, 0x73, (byte)0xcc, 0x57, 0x51,
                                   (byte)0x88,
                                   (byte)0xa1,
                                   0x74, 0x29, 0x10
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8, 0x1e,
                                                  (byte)0x82, 0x1a, 0x00, 0x31,
                                                  (byte)0x8d,
                                                  0x53,
                                                  0x19, 0x24, 0x1d }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8, 0x1e, (byte)0x82,
                                   (byte)0xc2, 0x58, 0x34, 0x47, 0x21, 0x59,
                                   (byte)0x88,
                                   (byte)0xb3,
                                   (byte)0xb9, (byte)0x85, 0x6b, 0x0d, 0x39,
                                   (byte)0x97,
                                   0x17,
                                (byte)0xd0, 0x10, (byte)0xd9, 0x62,
                                     (byte)0xd8,
                                   (byte)0xf4,
                                   0x3a,
                          (byte)0xfa, 0x3f, (byte)0x97, (byte)0xf5,
                                     (byte)0xaf,
                                   0x47,
                                   0x72,
                                   (byte)0xf8, (byte)0xd3, 0x36, (byte)0x9a,
                                   0x79,
                                   (byte)0xdd,
                                   (byte)0x8f, 0x5b, (byte)0xfe, 0x19,
                                   (byte)0xee,
                                   (byte)0x9e,
                                   (byte)0xe4, (byte)0x8a, 0x74, 0x3e,
                                   (byte)0x90,
                                   (byte)0xe7,
                          (byte)0x94, 0x66, 0x36, 0x7a, (byte)0xca,
                                     (byte)0xea,
                                   0x3d,
                                   0x61,
                                   (byte)0xc2, 0x58, 0x19, 0x73, (byte)0xe4,
                                   (byte)0xa8,
                                   0x56,
                          (byte)0xd5, 0x30, 0x4f, (byte)0xc0, 0x4e,
                                     (byte)0xd1,
                                   0x35,
                                   0x69,
                                   (byte)0x9a, (byte)0xb0, (byte)0x91, 0x01,
                                   (byte)0x9f,
                                   0x56,
                                   (byte)0xb8, 0x6f, 0x2d, (byte)0xda, 0x5b,
                                   (byte)0xa0, 0x38
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4,
                                (byte)0x82, 0x1b, 0x00, 0x00, 0x08,
                                                    (byte)0xed,
                                                  0x21,
                                                  0x0e,
                          (byte)0x83, (byte)0x9e, (byte)0xc2, 0x58, 0x1c,
                                                    0x46,
                                                  0x67,
                                                  0x31,
                          (byte)0xb3, (byte)0xd1, 0x4e, (byte)0xf9, 0x54,
                                                    0x08,
                                                  0x34,
                                                  0x7e,
                          0x43, (byte)0xce, 0x13, 0x3c, (byte)0xc0,
                                                    (byte)0xb5,
                                                  0x54,
                                                  0x1a,
                                      (byte)0xd9, (byte)0xb2, (byte)0x8f,
                                                    0x2f,
                                                  (byte)0xfe,
                                                  0x54,
                                              (byte)0x8c, (byte)0xd3, 0x73 }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4, (byte)0x82, 0x1b,
                                0x00, 0x00, 0x00, 0x01, 0x60, 0x1e,
                                     (byte)0xc1,
                                   (byte)0xcd,
                                   0x39,
                                   0x58, 0x73
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8, 0x1e,
                                                  (byte)0x82, (byte)0xc3, 0x4e,
                                                  0x70,
                                                  0x08,
                                      (byte)0x91, (byte)0xcc, 0x08,
                                                    (byte)0x90,
                                                  (byte)0x8e,
                                                  (byte)0xc9,
                                (byte)0xe0, (byte)0xaf, (byte)0xae,
                                                    (byte)0xbb,
                                                  0x77,
                                                  (byte)0x83,
                                (byte)0xc2, 0x58, 0x2d, 0x19, 0x7e,
                                                    (byte)0x9a,
                                                  (byte)0xe6,
                                                  0x65,
                                      0x7d, (byte)0xe7, 0x01, 0x7a,
                                                    (byte)0xae,
                                                  (byte)0x9f,
                                                  (byte)0x92,
                          0x19, (byte)0xe6, (byte)0xc3, (byte)0xed,
                                                    (byte)0xb8,
                                                  0x1f,
                                                  0x7b,
                                0x7a, (byte)0x90, (byte)0xe9, 0x1a, 0x3d,
                                                    0x6a,
                                                  (byte)0x82,
                                                  0x1c,
                                      (byte)0xe4, (byte)0x8f, 0x1e,
                                                    (byte)0xc9,
                                                  (byte)0x87,
                                                  0x2f,
                                (byte)0xbf, 0x3f, 0x47, (byte)0xaa,
                                                    (byte)0xe4,
                                                  (byte)0xc8,
                                                  0x20,
                                        0x1e, 0x03, (byte)0xa5, 0x3c, 0x23 }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4, (byte)0x82, 0x3b,
                                0x00, 0x63, (byte)0x93, (byte)0xb7, 0x75,
                                     0x43,
                                   (byte)0xf2,
                                   0x68,
                          (byte)0xc3, 0x58, 0x1f, 0x02, 0x17, 0x38,
                                     (byte)0xee,
                                   0x0e,
                                   0x2a,
                                   0x45, 0x58, 0x5c, 0x79, 0x75, (byte)0x88,
                                   0x18,
                                   (byte)0xa6,
                                   (byte)0xc5, (byte)0xcf, 0x02, 0x08, 0x29,
                                   0x76,
                                   (byte)0x89,
                                   (byte)0xe8, (byte)0xfb, 0x40, (byte)0xf3,
                                   (byte)0x84,
                                   (byte)0xc4,
                                   0x11, (byte)0xbe, 0x57, (byte)0xaf
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8,
                                0x1e, (byte)0x82, (byte)0xc3, 0x58, 0x24,
                                                    0x3a,
                                                  (byte)0xb3,
                                                  0x22,
                                      (byte)0x92, 0x6a, (byte)0xde,
                                                    (byte)0xe2,
                                                  0x2d,
                                                  (byte)0x98,
                                      (byte)0xe4, 0x04, 0x5f, (byte)0xb7,
                                                    0x19,
                                                  (byte)0xab,
                                                  0x4e,
                                (byte)0xc1, 0x28, (byte)0xad, (byte)0xe6,
                                                    0x2a,
                                                  (byte)0xda,
                                                  0x43,
                                            0x40, 0x46, 0x03, 0x63, 0x20,
                                                    0x44,
                                                  (byte)0xdc,
                                                  (byte)0xee,
                          (byte)0xc1, 0x06, 0x3b, (byte)0x87, 0x04,
                                                    (byte)0xc2,
                                                  0x58,
                                                  0x30,
                                      0x5f, 0x2d, 0x43, 0x03, (byte)0x88,
                                                    0x45,
                                                  (byte)0xc6,
                                                  0x23,
                          (byte)0xd8, 0x04, 0x68, 0x35, (byte)0xef,
                                                    (byte)0xd0,
                                                  0x5a,
                                                  0x78,
                                      (byte)0xac, 0x23, 0x29, (byte)0xf2,
                                                    0x78,
                                                  (byte)0xf1,
                                                  0x7d,
                                      (byte)0xa6, 0x4f, 0x4c, (byte)0xf3,
                                                    0x03,
                                                  0x44,
                                                  (byte)0xf7,
                                (byte)0xe4, 0x77, 0x21, 0x08, 0x38,
                                                    (byte)0x9a,
                                                  0x70,
                                                  (byte)0xa2,
                                0x60, 0x53, (byte)0xc7, (byte)0x80,
                                                    (byte)0xef,
                                                  (byte)0x89,
                                                  0x09,
                                        (byte)0xc2, (byte)0x9e, (byte)0xb6 }));
      CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4, (byte)0x82, 0x1a,
                          0x37, (byte)0xe1, 0x17, (byte)0xbe, (byte)0xc2,
                                     0x58,
                                   0x25,
                                   0x1f,
                                (byte)0xa9, (byte)0xe2, 0x1e, 0x77,
                                     (byte)0xd1,
                                   0x70,
                                   (byte)0xea,
                          0x7e, (byte)0xcc, 0x31, 0x76, (byte)0x8b,
                                     (byte)0xe0,
                                   0x3f,
                                   0x02,
                          (byte)0xaa, (byte)0xac, (byte)0xc7, (byte)0xe1,
                                     0x43,
                                   0x43,
                                   0x73,
                                0x60, (byte)0x87, (byte)0xfc, 0x7f,
                                     (byte)0xfd,
                                   0x4c,
                                   (byte)0xba,
                                   (byte)0x94, 0x7e, 0x17, (byte)0xec,
                                   (byte)0xd1, (byte)0xae, 0x5b
  }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8,
                                0x1e, (byte)0x82, 0x1b, 0x00, 0x00, 0x4a,
                                                    0x32,
                                                  (byte)0x84,
                                                  0x37,
                          (byte)0x90, (byte)0x8a, (byte)0xc2, 0x58, 0x28,
                                                    0x35,
                                                  0x12,
                                                  0x3f,
                                      0x2b, (byte)0xf4, 0x29, (byte)0xbd,
                                                    0x12,
                                                  (byte)0xc9,
                                                  (byte)0xfa,
                                (byte)0x89, 0x7b, (byte)0x91, (byte)0x9e,
                                                    0x4f,
                                                  0x13,
                                                  (byte)0xdb,
                          (byte)0xd7, (byte)0xdb, (byte)0x9a, (byte)0xe7,
                                                    0x10,
                                                  0x5d,
                                                  0x47,
                                      0x5d, (byte)0xad, 0x15, 0x5c,
                                                    (byte)0xbe,
                                                  0x30,
                                                  (byte)0xf7,
                                      (byte)0xef, (byte)0xe8, (byte)0xe0,
                                                    0x4a,
                                                  (byte)0xe5,
                                                  (byte)0xca,
                                        (byte)0xea, (byte)0xb9, (byte)0x89 }));
      Assert.AreEqual(
        -1,
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xd8, 0x1e,
                                (byte)0x82, (byte)0xc2, 0x58, 0x1e, 0x0e,
                                       0x53,
                                     0x4f,
                                     (byte)0xfe,
                                0x4d, 0x54, (byte)0xbb, 0x21, 0x3f,
                                       (byte)0xd5,
                                     (byte)0xea,
                                     0x61,
                                (byte)0x90, 0x68, (byte)0x8a, 0x14,
                                       (byte)0xfd,
                                     (byte)0x8d,
                                     0x19,
                          (byte)0xba, (byte)0xaf, (byte)0xbf, 0x3a, 0x67,
                                       0x5e,
                                     0x2d,
                                     0x52,
                                     0x41, (byte)0x93, (byte)0xa7, 0x18,
          0x41 }).CompareTo(CBORObject.DecodeFromBytes(new byte[] { (byte)0xc5,
                                      (byte)0x82, 0x1b, 0x00, 0x00, 0x4b,
                                                         0x3e,
                                                       (byte)0xcb,
                                                       (byte)0xe8,
                          (byte)0xc4, (byte)0xa3, (byte)0xc2, 0x58, 0x2a,
                                                         0x17,
                                                       0x0a,
                                                       0x4d,
                                      (byte)0x88, 0x40, (byte)0xe7,
                                                         (byte)0xe9,
                                                       (byte)0xe1,
                                                       (byte)0x95,
                                (byte)0xdc, (byte)0xad, (byte)0x97,
                                                         (byte)0x87,
                                                       0x66,
                                                       (byte)0x8c,
                                      0x77, 0x4b, (byte)0xd6, 0x46, 0x52,
                                                         0x00,
                                                       (byte)0xf0,
                                                       (byte)0xdd,
                                0x77, 0x16, (byte)0xa5, (byte)0xca, 0x71,
                                                         0x5d,
                                                       (byte)0xf5,
                                                       0x7c,
                                      0x6b, (byte)0x82, (byte)0x85, 0x47,
                                                         0x2d,
                                                       (byte)0x90,
                                                       (byte)0x89,
                                             0x12, (byte)0x93, 0x0b, 0x1e })));
    }

    [TestMethod]
    public void TestExample() {
      // The following creates a CBOR map and adds
      // several kinds of objects to it
      CBORObject cbor = CBORObject.NewMap()
        .Add("item", "any string")
        .Add("number", 42)
        .Add("map", CBORObject.NewMap().Add("number", 42))
        .Add("array", CBORObject.NewArray().Add(999f).Add("xyz"))
        .Add("bytes", new byte[] { 0, 1, 2 });
      // The following converts the map to CBOR
      byte[] bytes = cbor.EncodeToBytes();
      // The following converts the map to JSON
      string json = cbor.ToJSONString();
    }

    private void TestWriteToJSON(CBORObject obj) {
      CBORObject objA = null;
      using (var ms = new MemoryStream()) {
        try {
          obj.WriteJSONTo(ms);
          objA =
            CBORObject.FromJSONString(
              DataUtilities.GetUtf8String(
                ms.ToArray(),
                true));
        } catch (IOException ex) {
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      CBORObject objB = CBORObject.FromJSONString(obj.ToJSONString());
      if (!objA.Equals(objB)) {
        Console.WriteLine(objA);
        Console.WriteLine(objB);
        Assert.Fail("WriteJSONTo gives different results from ToJSONString");
      }
    }

    [TestMethod]
    public void TestRandomNonsense() {
      var rand = new FastRandom();
      for (var i = 0; i < 200; ++i) {
        var array = new byte[rand.NextValue(1000000) + 1];
        for (int j = 0; j < array.Length; ++j) {
          if (j + 3 <= array.Length) {
            int r = rand.NextValue(0x1000000);
            array[j] = (byte)(r & 0xff);
            array[j + 1] = (byte)((r >> 8) & 0xff);
            array[j + 2] = (byte)((r >> 16) & 0xff);
            j += 2;
          } else {
            array[j] = (byte)rand.NextValue(256);
          }
        }
        using (var ms = new MemoryStream(array)) {
          while (ms.Position != ms.Length) {
            try {
              CBORObject o = CBORObject.Read(ms);
              try {
                if (o == null) {
                  Assert.Fail("object read is null");
                }
                CBORObject.DecodeFromBytes(o.EncodeToBytes());
              } catch (Exception ex) {
                Assert.Fail(ex.ToString());
                throw new InvalidOperationException(String.Empty, ex);
              }
              String jsonString = String.Empty;
              try {
                if (o.Type == CBORType.Array || o.Type == CBORType.Map) {
                  jsonString = o.ToJSONString();
                  CBORObject.FromJSONString(jsonString);
                  this.TestWriteToJSON(o);
                }
              } catch (Exception ex) {
                Assert.Fail(jsonString + "\n" + ex);
                throw new InvalidOperationException(String.Empty, ex);
              }
            } catch (CBORException) {
              // Expected exception
            }
          }
        }
      }
    }

    public static void TestCBORMapAdd() {
      CBORObject cbor = CBORObject.NewMap();
      cbor.Add(1, 2);
      Assert.IsTrue(cbor.ContainsKey(CBORObject.FromObject(1)));
      Assert.AreEqual((int)2, cbor[CBORObject.FromObject(1)]);
      Assert.AreEqual("{\"1\":2}", cbor.ToJSONString());
      cbor.Add("hello", 2);
      Assert.IsTrue(cbor.ContainsKey("hello"));
      Assert.IsTrue(cbor.ContainsKey(CBORObject.FromObject("hello")));
      Assert.AreEqual((int)2, cbor["hello"]);
      cbor.Set(1, 3);
      Assert.IsTrue(cbor.ContainsKey(CBORObject.FromObject(1)));
      Assert.AreEqual((int)3, cbor[CBORObject.FromObject(1)]);
    }

    [TestMethod]
    [Timeout(20000)]
    public void TestRandomSlightlyModified() {
      var rand = new FastRandom();
      // Test slightly modified objects
      for (var i = 0; i < 200; ++i) {
        CBORObject originalObject = RandomCBORObject(rand);
        byte[] array = originalObject.EncodeToBytes();
        // Console.WriteLine(originalObject);
        int count2 = rand.NextValue(10) + 1;
        for (int j = 0; j < count2; ++j) {
          int index = rand.NextValue(array.Length);
          array[index] = unchecked((byte)rand.NextValue(256));
        }
        using (var ms = new MemoryStream(array)) {
          while (ms.Position != ms.Length) {
            try {
              CBORObject o = CBORObject.Read(ms);
              byte[] encodedBytes = (o == null) ? null : o.EncodeToBytes();
              try {
                CBORObject.DecodeFromBytes(encodedBytes);
              } catch (Exception ex) {
                Assert.Fail(ex.ToString());
                throw new InvalidOperationException(String.Empty, ex);
              }
              String jsonString = String.Empty;
              try {
                if (o.Type == CBORType.Array || o.Type == CBORType.Map) {
                  jsonString = o.ToJSONString();
                  // reread JSON string to test validity
                  CBORObject.FromJSONString(jsonString);
                  this.TestWriteToJSON(o);
                }
              } catch (Exception ex) {
                Assert.Fail(jsonString + "\n" + ex);
                throw new InvalidOperationException(String.Empty, ex);
              }
            } catch (CBORException) {
              // Expected exception
            }
          }
        }
      }
    }

    [TestMethod]
    [Timeout(50000)]
    public void TestRandomData() {
      var rand = new FastRandom();
      CBORObject obj;
      for (var i = 0; i < 1000; ++i) {
        obj = RandomCBORObject(rand);
        TestCommon.AssertRoundTrip(obj);
        this.TestWriteToJSON(obj);
      }
    }

    [TestMethod]
    public void TestExtendedFloatSingle() {
      var rand = new FastRandom();
      for (var i = 0; i < 255; ++i) {
        // Try a random float with a given
        // exponent
        TestExtendedFloatSingleCore(RandomSingle(rand, i), null);
        TestExtendedFloatSingleCore(RandomSingle(rand, i), null);
        TestExtendedFloatSingleCore(RandomSingle(rand, i), null);
        TestExtendedFloatSingleCore(RandomSingle(rand, i), null);
      }
    }

    [TestMethod]
    public void TestExtendedFloatDouble() {
      TestExtendedFloatDoubleCore(3.5, "3.5");
      TestExtendedFloatDoubleCore(7, "7");
      TestExtendedFloatDoubleCore(1.75, "1.75");
      TestExtendedFloatDoubleCore(3.5, "3.5");
      TestExtendedFloatDoubleCore((double)Int32.MinValue, "-2147483648");
      TestExtendedFloatDoubleCore(
        (double)Int64.MinValue,
        "-9223372036854775808");
      var rand = new FastRandom();
      for (var i = 0; i < 2047; ++i) {
        // Try a random double with a given
        // exponent
        TestExtendedFloatDoubleCore(RandomDouble(rand, i), null);
        TestExtendedFloatDoubleCore(RandomDouble(rand, i), null);
        TestExtendedFloatDoubleCore(RandomDouble(rand, i), null);
        TestExtendedFloatDoubleCore(RandomDouble(rand, i), null);
      }
    }
    [TestMethod]
    [ExpectedException(typeof(CBORException))]
    public void TestTagThenBreak() {
      TestCommon.FromBytesTestAB(new byte[] { 0xd1, 0xff });
    }

    [TestMethod]
    public void TestJSONEscapedChars() {
      CBORObject o = CBORObject.FromJSONString(
        "[\"\\r\\n\\u0006\\u000E\\u001A\\\\\\\"\"]");
      Assert.AreEqual(1, o.Count);
      Assert.AreEqual("\r\n\u0006\u000E\u001A\\\"", o[0].AsString());
      Assert.AreEqual(
        "[\"\\r\\n\\u0006\\u000E\\u001A\\\\\\\"\"]",
        o.ToJSONString());
      TestCommon.AssertRoundTrip(o);
    }

    [TestMethod]
    public void TestCBORFromArray() {
      CBORObject o = CBORObject.FromObject(new[] { 1, 2, 3 });
      Assert.AreEqual(3, o.Count);
      Assert.AreEqual(1, o[0].AsInt32());
      Assert.AreEqual(2, o[1].AsInt32());
      Assert.AreEqual(3, o[2].AsInt32());
      TestCommon.AssertRoundTrip(o);
    }

    [TestMethod]
    public void TestJSON() {
      CBORObject o;
      o = CBORObject.FromJSONString("[1,2,null,true,false,\"\"]");
      Assert.AreEqual(6, o.Count);
      Assert.AreEqual(1, o[0].AsInt32());
      Assert.AreEqual(2, o[1].AsInt32());
      Assert.AreEqual(CBORObject.Null, o[2]);
      Assert.AreEqual(CBORObject.True, o[3]);
      Assert.AreEqual(CBORObject.False, o[4]);
      Assert.AreEqual(String.Empty, o[5].AsString());
      o = CBORObject.FromJSONString("[1.5,2.6,3.7,4.0,222.22]");
      double actual = o[0].AsDouble();
      Assert.AreEqual((double)1.5, actual);
      try {
        CBORObject.FromJSONString("[0]");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[0.1]");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[0.1001]");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[0.0]");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[0.00]");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[0.000]");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[0.01]");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[0.001]");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[0.5]");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[0E5]");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[0E+6]");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\ufeff\u0020 {}");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      using (var ms2a = new MemoryStream(new byte[] { })) {
        try {
          CBORObject.ReadJSON(ms2a);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var ms2b = new MemoryStream(new byte[] { 0x20 })) {
        try {
          CBORObject.ReadJSON(ms2b);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      try {
        CBORObject.FromJSONString(String.Empty);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[.1]");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[-.1]");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\u0020");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual("true", CBORObject.FromJSONString("true").ToJSONString());
      Assert.AreEqual(
        "true",
        CBORObject.FromJSONString(" true ").ToJSONString());
      Assert.AreEqual(
        "false",
        CBORObject.FromJSONString("false").ToJSONString());
      Assert.AreEqual("null", CBORObject.FromJSONString("null").ToJSONString());
      Assert.AreEqual("5", CBORObject.FromJSONString("5").ToJSONString());
    }

    [TestMethod]
    public void TestBoolean() {
      TestCommon.AssertSer(CBORObject.True, "true");
      TestCommon.AssertSer(CBORObject.False, "false");
      Assert.AreEqual(CBORObject.True, CBORObject.FromObject(true));
      Assert.AreEqual(CBORObject.False, CBORObject.FromObject(false));
    }

    [TestMethod]
    public void TestByte() {
      for (var i = 0; i <= 255; ++i) {
        TestCommon.AssertSer(
          CBORObject.FromObject((byte)i),
          String.Empty + i);
      }
    }

    public void DoTestReadUtf8(
      byte[] bytes,
      int expectedRet,
      string expectedString,
      int noReplaceRet,
      string noReplaceString) {
      DoTestReadUtf8(
        bytes,
        bytes.Length,
        expectedRet,
        expectedString,
        noReplaceRet,
        noReplaceString);
    }

    public static void DoTestReadUtf8(
      byte[] bytes,
      int length,
      int expectedRet,
      string expectedString,
      int noReplaceRet,
      string noReplaceString) {
      try {
        var builder = new StringBuilder();
        int ret = 0;
        using (var ms = new MemoryStream(bytes)) {
          ret = DataUtilities.ReadUtf8(ms, length, builder, true);
          Assert.AreEqual(expectedRet, ret);
          if (expectedRet == 0) {
            Assert.AreEqual(expectedString, builder.ToString());
          }
          ms.Position = 0;
          builder.Clear();
          ret = DataUtilities.ReadUtf8(ms, length, builder, false);
          Assert.AreEqual(noReplaceRet, ret);
          if (noReplaceRet == 0) {
            Assert.AreEqual(noReplaceString, builder.ToString());
          }
        }
        if (bytes.Length >= length) {
          builder.Clear();
          ret = DataUtilities.ReadUtf8FromBytes(
            bytes,
            0,
            length,
            builder,
            true);
          Assert.AreEqual(expectedRet, ret);
          if (expectedRet == 0) {
            Assert.AreEqual(expectedString, builder.ToString());
          }
          builder.Clear();
          ret = DataUtilities.ReadUtf8FromBytes(
            bytes,
            0,
            length,
            builder,
            false);
          Assert.AreEqual(noReplaceRet, ret);
          if (noReplaceRet == 0) {
            Assert.AreEqual(noReplaceString, builder.ToString());
          }
        }
      } catch (IOException ex) {
        throw new CBORException(String.Empty, ex);
      }
    }

    [TestMethod]
    public void TestFPToBigInteger() {
      Assert.AreEqual(
        "0" ,
        CBORObject.FromObject((float)0.75).AsBigInteger().ToString());
      Assert.AreEqual(
        "0" ,
        CBORObject.FromObject((float)0.99).AsBigInteger().ToString());
      Assert.AreEqual(
        "0" ,
 CBORObject.FromObject((float)0.0000000000000001).AsBigInteger()
          .ToString());
      Assert.AreEqual(
        "0" ,
        CBORObject.FromObject((float)0.5).AsBigInteger().ToString());
      Assert.AreEqual(
        "1" ,
        CBORObject.FromObject((float)1.5).AsBigInteger().ToString());
      Assert.AreEqual(
        "2" ,
        CBORObject.FromObject((float)2.5).AsBigInteger().ToString());
      Assert.AreEqual(
        "328323" ,
        CBORObject.FromObject((float)328323f).AsBigInteger().ToString());
      Assert.AreEqual(
        "0" ,
        CBORObject.FromObject((double)0.75).AsBigInteger().ToString());
      Assert.AreEqual(
        "0" ,
        CBORObject.FromObject((double)0.99).AsBigInteger().ToString());
      Assert.AreEqual(
        "0" ,
CBORObject.FromObject((double)0.0000000000000001).AsBigInteger()
          .ToString());
      Assert.AreEqual(
        "0" ,
        CBORObject.FromObject((double)0.5).AsBigInteger().ToString());
      Assert.AreEqual(
        "1" ,
        CBORObject.FromObject((double)1.5).AsBigInteger().ToString());
      Assert.AreEqual(
        "2" ,
        CBORObject.FromObject((double)2.5).AsBigInteger().ToString());
      Assert.AreEqual(
        "328323" ,
        CBORObject.FromObject((double)328323).AsBigInteger().ToString());
      try {
        CBORObject.FromObject(Single.PositiveInfinity).AsBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(Single.NegativeInfinity).AsBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(Single.NaN).AsBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(Double.PositiveInfinity).AsBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(Double.NegativeInfinity).AsBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(Double.NaN).AsBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
    }

    [TestMethod]
    public void TestReadUtf8() {
      DoTestReadUtf8(
        new byte[] { 0x21,
        0x21,
        0x21 },
        0,
        "!!!",
        0,
        "!!!");
      DoTestReadUtf8(
        new byte[] { 0x20,
        0xc2,
        0x80 },
        0,
        " \u0080",
        0,
        " \u0080");
      DoTestReadUtf8(
        new byte[] { 0x20,
        0xc2,
        0x80,
        0x20 },
        0,
        " \u0080 ",
        0,
        " \u0080 ");
      DoTestReadUtf8(
        new byte[] { 0x20,
        0xc2,
        0x80,
        0xc2 },
        0,
        " \u0080\ufffd",
        -1,
        null);
      DoTestReadUtf8(
        new byte[] { 0x20,
        0xc2,
        0x21,
        0x21 },
        0,
        " \ufffd!!",
        -1,
        null);
      DoTestReadUtf8(
        new byte[] { 0x20,
        0xc2,
        0xff,
        0x20 },
        0,
        " \ufffd\ufffd ",
        -1,
        null);
      DoTestReadUtf8(
        new byte[] { 0x20,
        0xe0,
        0xa0,
        0x80 },
        0,
        " \u0800",
        0,
        " \u0800");
      DoTestReadUtf8(
        new byte[] { 0x20,
        0xe0,
        0xa0,
        0x80,
        0x20 },
        0,
        " \u0800 ",
        0,
        " \u0800 ");
      DoTestReadUtf8(
        new byte[] { 0x20,
        0xf0,
        0x90,
        0x80,
        0x80 },
        0,
        " \ud800\udc00",
        0,
        " \ud800\udc00");
      DoTestReadUtf8(
        new byte[] { 0x20,
        0xf0,
        0x90,
        0x80,
        0x80 },
        3,
        0,
        " \ufffd",
        -1,
        null);
      DoTestReadUtf8(
        new byte[] { 0x20,
        0xf0,
        0x90 },
        5,
        -2,
        null,
        -1,
        null);
      DoTestReadUtf8(
        new byte[] { 0x20,
        0x20,
        0x20 },
        5,
        -2,
        null,
        -2,
        null);
      DoTestReadUtf8(
        new byte[] { 0x20,
        0xf0,
        0x90,
        0x80,
        0x80,
        0x20 },
        0,
        " \ud800\udc00 ",
        0,
        " \ud800\udc00 ");
      DoTestReadUtf8(
        new byte[] { 0x20,
        0xf0,
        0x90,
        0x80,
        0x20 },
        0,
        " \ufffd ",
        -1,
        null);
      DoTestReadUtf8(
        new byte[] { 0x20,
        0xf0,
        0x90,
        0x20 },
        0,
        " \ufffd ",
        -1,
        null);
      DoTestReadUtf8(
        new byte[] { 0x20,
        0xf0,
        0x90,
        0x80,
        0xff },
        0,
        " \ufffd\ufffd",
        -1,
        null);
      DoTestReadUtf8(
        new byte[] { 0x20,
        0xf0,
        0x90,
        0xff },
        0,
        " \ufffd\ufffd",
        -1,
        null);
      DoTestReadUtf8(
        new byte[] { 0x20,
        0xe0,
        0xa0,
        0x20 },
        0,
        " \ufffd ",
        -1,
        null);
      DoTestReadUtf8(
        new byte[] { 0x20,
        0xe0,
        0x20 },
        0,
        " \ufffd ",
        -1,
        null);
      DoTestReadUtf8(
        new byte[] { 0x20,
        0xe0,
        0xa0,
        0xff },
        0,
        " \ufffd\ufffd",
        -1,
        null);
      DoTestReadUtf8(
        new byte[] { 0x20,
        0xe0,
        0xff },
        0,
        " \ufffd\ufffd",
        -1,
        null);
    }

    private static bool ByteArrayEquals(byte[] arrayA, byte[] arrayB) {
      if (arrayA == null) {
        return arrayB == null;
      }
      if (arrayB == null) {
        return false;
      }
      if (arrayA.Length != arrayB.Length) {
        return false;
      }
      for (var i = 0; i < arrayA.Length; ++i) {
        if (arrayA[i] != arrayB[i]) {
          return false;
        }
      }
      return true;
    }

    [TestMethod]
    public void TestArray() {
      CBORObject cbor = CBORObject.FromJSONString("[]");
      cbor.Add(CBORObject.FromObject(3));
      cbor.Add(CBORObject.FromObject(4));
      byte[] bytes = cbor.EncodeToBytes();
      bool isequal = ByteArrayEquals(
        new byte[] { (byte)(0x80 | 2),
        3,
        4 },
        bytes);
      Assert.IsTrue(isequal, "array not equal");
      cbor = CBORObject.FromObject(new[] { "a", "b", "c", "d", "e" });
      Assert.AreEqual("[\"a\",\"b\",\"c\",\"d\",\"e\"]", cbor.ToJSONString());
      TestCommon.AssertRoundTrip(cbor);
      cbor = CBORObject.DecodeFromBytes(new byte[] { 0x9f, 0, 1, 2, 3, 4, 5,
                                          6, 7, 0xff });
      Assert.AreEqual("[0,1,2,3,4,5,6,7]", cbor.ToJSONString());
    }

    [TestMethod]
    public void TestMap() {
      CBORObject cbor = CBORObject.FromJSONString("{\"a\":2,\"b\":4}");
      Assert.AreEqual(2, cbor.Count);
      TestCommon.AssertEqualsHashCode(
        CBORObject.FromObject(2),
        cbor[CBORObject.FromObject("a")]);
      TestCommon.AssertEqualsHashCode(
        CBORObject.FromObject(4),
        cbor[CBORObject.FromObject("b")]);
      Assert.AreEqual(2, cbor[CBORObject.FromObject("a")].AsInt32());
      Assert.AreEqual(4, cbor[CBORObject.FromObject("b")].AsInt32());
      Assert.AreEqual(0, CBORObject.True.Count);
      cbor = CBORObject.DecodeFromBytes(new byte[] { 0xbf, 0x61, 0x61, 2,
                                          0x61, 0x62, 4, 0xff });
      Assert.AreEqual(2, cbor.Count);
      TestCommon.AssertEqualsHashCode(
        CBORObject.FromObject(2),
        cbor[CBORObject.FromObject("a")]);
      TestCommon.AssertEqualsHashCode(
        CBORObject.FromObject(4),
        cbor[CBORObject.FromObject("b")]);
      Assert.AreEqual(2, cbor[CBORObject.FromObject("a")].AsInt32());
      Assert.AreEqual(4, cbor[CBORObject.FromObject("b")].AsInt32());
    }

    private static String Repeat(char c, int num) {
      var sb = new StringBuilder();
      for (var i = 0; i < num; ++i) {
        sb.Append(c);
      }
      return sb.ToString();
    }

    private static String Repeat(String c, int num) {
      var sb = new StringBuilder();
      for (var i = 0; i < num; ++i) {
        sb.Append(c);
      }
      return sb.ToString();
    }

    [TestMethod]
    public void TestTextStringStream() {
      CBORObject cbor = TestCommon.FromBytesTestAB(
        new byte[] { 0x7f,
        0x61,
        0x2e,
        0x61,
        0x2e,
        0xff });
      Assert.AreEqual("..", cbor.AsString());
      // Test streaming of long strings
      string longString = Repeat('x', 200000);
      CBORObject cbor2;
      cbor = CBORObject.FromObject(longString);
      cbor2 = TestCommon.FromBytesTestAB(cbor.EncodeToBytes());
      TestCommon.AssertEqualsHashCode(cbor, cbor2);
      Assert.AreEqual(longString, cbor2.AsString());
      longString = Repeat('\u00e0', 200000);
      cbor = CBORObject.FromObject(longString);
      cbor2 = TestCommon.FromBytesTestAB(cbor.EncodeToBytes());
      TestCommon.AssertEqualsHashCode(cbor, cbor2);
      Assert.AreEqual(longString, cbor2.AsString());
      longString = Repeat('\u3000', 200000);
      cbor = CBORObject.FromObject(longString);
      cbor2 = TestCommon.FromBytesTestAB(cbor.EncodeToBytes());
      TestCommon.AssertEqualsHashCode(cbor, cbor2);
      Assert.AreEqual(longString, cbor2.AsString());
      longString = Repeat("\ud800\udc00", 200000);
      cbor = CBORObject.FromObject(longString);
      cbor2 = TestCommon.FromBytesTestAB(cbor.EncodeToBytes());
      TestCommon.AssertEqualsHashCode(cbor, cbor2);
      Assert.AreEqual(longString, cbor2.AsString());
    }
    [TestMethod]
    [ExpectedException(typeof(CBORException))]
    public void TestTextStringStreamNoTagsBeforeDefinite() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0x7f,
        0x61,
        0x20,
        0xc0,
        0x61,
        0x20,
        0xff });
    }
    [TestMethod]
    [ExpectedException(typeof(CBORException))]
    public void TestTextStringStreamNoIndefiniteWithinDefinite() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0x7f,
        0x61,
        0x20,
        0x7f,
        0x61,
        0x20,
        0xff,
        0xff });
    }

    [TestMethod]
    public void TestByteStringStream() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0x5f,
        0x41,
        0x20,
        0x41,
        0x20,
        0xff });
    }
    [TestMethod]
    [ExpectedException(typeof(CBORException))]
    public void TestByteStringStreamNoTagsBeforeDefinite() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0x5f,
        0x41,
        0x20,
        0xc2,
        0x41,
        0x20,
        0xff });
    }

    public static void AssertDecimalsEquivalent(string a, string b) {
      CBORObject ca = CBORDataUtilities.ParseJSONNumber(a);
      CBORObject cb = CBORDataUtilities.ParseJSONNumber(b);
      CompareTestEqual(ca, cb);
      TestCommon.AssertRoundTrip(ca);
      TestCommon.AssertRoundTrip(cb);
    }

    [TestMethod]
    public void ZeroStringTests2() {
      Assert.AreEqual(
        "0.0001265",
        ExtendedDecimal.FromString("1.265e-4").ToString());
      Assert.AreEqual(
        "0.0001265",
        ExtendedDecimal.FromString("1.265e-4").ToEngineeringString());
      Assert.AreEqual(
        "0.0001265",
        ExtendedDecimal.FromString("1.265e-4").ToPlainString());
      Assert.AreEqual(
        "0.0000",
        ExtendedDecimal.FromString("0.000E-1").ToString());
      Assert.AreEqual(
        "0.0000",
        ExtendedDecimal.FromString("0.000E-1").ToEngineeringString());
      Assert.AreEqual(
        "0.0000",
        ExtendedDecimal.FromString("0.000E-1").ToPlainString());
      Assert.AreEqual(
        "0E-16" ,
        ExtendedDecimal.FromString("0.0000000000000e-3").ToString());
      Assert.AreEqual(
        "0.0E-15" ,
        ExtendedDecimal.FromString("0.0000000000000e-3" ).ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000000" ,
        ExtendedDecimal.FromString("0.0000000000000e-3").ToPlainString());
      Assert.AreEqual(
        "0E-8",
        ExtendedDecimal.FromString("0.000000000e+1").ToString());
      Assert.AreEqual(
        "0.00E-6" ,
        ExtendedDecimal.FromString("0.000000000e+1").ToEngineeringString());
      Assert.AreEqual(
        "0.00000000" ,
        ExtendedDecimal.FromString("0.000000000e+1").ToPlainString());
      Assert.AreEqual(
        "0.000" ,
        ExtendedDecimal.FromString("0.000000000000000e+12").ToString());
      Assert.AreEqual(
        "0.000" ,
   ExtendedDecimal.FromString("0.000000000000000e+12"
).ToEngineeringString());
      Assert.AreEqual(
        "0.000" ,
        ExtendedDecimal.FromString("0.000000000000000e+12").ToPlainString());
      Assert.AreEqual(
        "0E-25" ,
        ExtendedDecimal.FromString("0.00000000000000e-11").ToString());
      Assert.AreEqual(
        "0.0E-24" ,
    ExtendedDecimal.FromString("0.00000000000000e-11"
).ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000000000000000" ,
        ExtendedDecimal.FromString("0.00000000000000e-11").ToPlainString());
      Assert.AreEqual(
        "0E-7" ,
        ExtendedDecimal.FromString("0.000000000000e+5").ToString());
      Assert.AreEqual(
        "0.0E-6" ,
        ExtendedDecimal.FromString("0.000000000000e+5").ToEngineeringString());
      Assert.AreEqual(
        "0.0000000" ,
        ExtendedDecimal.FromString("0.000000000000e+5").ToPlainString());
      Assert.AreEqual(
        "0E-8",
        ExtendedDecimal.FromString("0.0000e-4").ToString());
      Assert.AreEqual(
        "0.00E-6",
        ExtendedDecimal.FromString("0.0000e-4").ToEngineeringString());
      Assert.AreEqual(
        "0.00000000",
        ExtendedDecimal.FromString("0.0000e-4").ToPlainString());
      Assert.AreEqual(
        "0.0000",
        ExtendedDecimal.FromString("0.000000e+2").ToString());
      Assert.AreEqual(
        "0.0000",
        ExtendedDecimal.FromString("0.000000e+2").ToEngineeringString());
      Assert.AreEqual(
        "0.0000",
        ExtendedDecimal.FromString("0.000000e+2").ToPlainString());
      Assert.AreEqual("0E+2", ExtendedDecimal.FromString("0.0e+3").ToString());
      Assert.AreEqual(
        "0.0E+3",
        ExtendedDecimal.FromString("0.0e+3").ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.0e+3").ToPlainString());
      Assert.AreEqual(
        "0E-7" ,
        ExtendedDecimal.FromString("0.000000000000000e+8").ToString());
      Assert.AreEqual(
        "0.0E-6" ,
    ExtendedDecimal.FromString("0.000000000000000e+8"
).ToEngineeringString());
      Assert.AreEqual(
        "0.0000000" ,
        ExtendedDecimal.FromString("0.000000000000000e+8").ToPlainString());
      Assert.AreEqual(
        "0E+7",
        ExtendedDecimal.FromString("0.000e+10").ToString());
      Assert.AreEqual(
        "0.00E+9",
        ExtendedDecimal.FromString("0.000e+10").ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.000e+10").ToPlainString());
      Assert.AreEqual(
        "0E-31" ,
        ExtendedDecimal.FromString("0.0000000000000000000e-12").ToString());
      Assert.AreEqual(
        "0.0E-30" ,
        ExtendedDecimal.FromString("0.0000000000000000000e-12"
).ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000000000000000000000" ,
     ExtendedDecimal.FromString("0.0000000000000000000e-12"
).ToPlainString());
      Assert.AreEqual(
        "0.00000",
        ExtendedDecimal.FromString("0.0000e-1").ToString());
      Assert.AreEqual(
        "0.00000",
        ExtendedDecimal.FromString("0.0000e-1").ToEngineeringString());
      Assert.AreEqual(
        "0.00000",
        ExtendedDecimal.FromString("0.0000e-1").ToPlainString());
      Assert.AreEqual(
        "0E-22" ,
        ExtendedDecimal.FromString("0.00000000000e-11").ToString());
      Assert.AreEqual(
        "0.0E-21" ,
        ExtendedDecimal.FromString("0.00000000000e-11").ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000000000000" ,
        ExtendedDecimal.FromString("0.00000000000e-11").ToPlainString());
      Assert.AreEqual(
        "0E-28" ,
        ExtendedDecimal.FromString("0.00000000000e-17").ToString());
      Assert.AreEqual(
        "0.0E-27" ,
        ExtendedDecimal.FromString("0.00000000000e-17").ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000000000000000000" ,
        ExtendedDecimal.FromString("0.00000000000e-17").ToPlainString());
      Assert.AreEqual(
        "0.00000" ,
        ExtendedDecimal.FromString("0.00000000000000e+9").ToString());
      Assert.AreEqual(
        "0.00000" ,
     ExtendedDecimal.FromString("0.00000000000000e+9"
).ToEngineeringString());
      Assert.AreEqual(
        "0.00000" ,
        ExtendedDecimal.FromString("0.00000000000000e+9").ToPlainString());
      Assert.AreEqual(
        "0E-28" ,
        ExtendedDecimal.FromString("0.0000000000e-18").ToString());
      Assert.AreEqual(
        "0.0E-27" ,
        ExtendedDecimal.FromString("0.0000000000e-18").ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000000000000000000" ,
        ExtendedDecimal.FromString("0.0000000000e-18").ToPlainString());
      Assert.AreEqual(
        "0E-14",
        ExtendedDecimal.FromString("0.0e-13").ToString());
      Assert.AreEqual(
        "0.00E-12",
        ExtendedDecimal.FromString("0.0e-13").ToEngineeringString());
      Assert.AreEqual(
        "0.00000000000000" ,
        ExtendedDecimal.FromString("0.0e-13").ToPlainString());
      Assert.AreEqual(
        "0E-8" ,
        ExtendedDecimal.FromString("0.000000000000000000e+10").ToString());
      Assert.AreEqual(
        "0.00E-6" ,
ExtendedDecimal.FromString("0.000000000000000000e+10"
).ToEngineeringString());
      Assert.AreEqual(
        "0.00000000" ,
        ExtendedDecimal.FromString("0.000000000000000000e+10" ).ToPlainString());
      Assert.AreEqual(
        "0E+15",
        ExtendedDecimal.FromString("0.0000e+19").ToString());
      Assert.AreEqual(
        "0E+15",
        ExtendedDecimal.FromString("0.0000e+19").ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.0000e+19").ToPlainString());
      Assert.AreEqual(
        "0E-13",
        ExtendedDecimal.FromString("0.00000e-8").ToString());
      Assert.AreEqual(
        "0.0E-12",
        ExtendedDecimal.FromString("0.00000e-8").ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000" ,
        ExtendedDecimal.FromString("0.00000e-8").ToPlainString());
      Assert.AreEqual(
        "0E+3" ,
        ExtendedDecimal.FromString("0.00000000000e+14").ToString());
      Assert.AreEqual(
        "0E+3" ,
        ExtendedDecimal.FromString("0.00000000000e+14").ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.00000000000e+14").ToPlainString());
      Assert.AreEqual(
        "0E-17",
        ExtendedDecimal.FromString("0.000e-14").ToString());
      Assert.AreEqual(
        "0.00E-15",
        ExtendedDecimal.FromString("0.000e-14").ToEngineeringString());
      Assert.AreEqual(
        "0.00000000000000000" ,
        ExtendedDecimal.FromString("0.000e-14").ToPlainString());
      Assert.AreEqual(
        "0E-25",
        ExtendedDecimal.FromString("0.000000e-19").ToString());
      Assert.AreEqual(
        "0.0E-24",
        ExtendedDecimal.FromString("0.000000e-19").ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000000000000000" ,
        ExtendedDecimal.FromString("0.000000e-19").ToPlainString());
      Assert.AreEqual(
        "0E+7" ,
        ExtendedDecimal.FromString("0.000000000000e+19").ToString());
      Assert.AreEqual(
        "0.00E+9" ,
        ExtendedDecimal.FromString("0.000000000000e+19" ).ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.000000000000e+19").ToPlainString());
      Assert.AreEqual(
        "0E+5" ,
        ExtendedDecimal.FromString("0.0000000000000e+18").ToString());
      Assert.AreEqual(
        "0.0E+6" ,
     ExtendedDecimal.FromString("0.0000000000000e+18"
).ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.0000000000000e+18").ToPlainString());
      Assert.AreEqual(
        "0E-16" ,
        ExtendedDecimal.FromString("0.00000000000000e-2").ToString());
      Assert.AreEqual(
        "0.0E-15" ,
     ExtendedDecimal.FromString("0.00000000000000e-2"
).ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000000" ,
        ExtendedDecimal.FromString("0.00000000000000e-2").ToPlainString());
      Assert.AreEqual(
        "0E-31" ,
        ExtendedDecimal.FromString("0.0000000000000e-18").ToString());
      Assert.AreEqual(
        "0.0E-30" ,
     ExtendedDecimal.FromString("0.0000000000000e-18"
).ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000000000000000000000" ,
        ExtendedDecimal.FromString("0.0000000000000e-18").ToPlainString());
      Assert.AreEqual("0E-17", ExtendedDecimal.FromString("0e-17").ToString());
      Assert.AreEqual(
        "0.00E-15",
        ExtendedDecimal.FromString("0e-17").ToEngineeringString());
      Assert.AreEqual(
        "0.00000000000000000" ,
        ExtendedDecimal.FromString("0e-17").ToPlainString());
      Assert.AreEqual("0E+17", ExtendedDecimal.FromString("0e+17").ToString());
      Assert.AreEqual(
        "0.0E+18",
        ExtendedDecimal.FromString("0e+17").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0e+17").ToPlainString());
      Assert.AreEqual(
        "0E-17" ,
        ExtendedDecimal.FromString("0.00000000000000000e+0").ToString());
      Assert.AreEqual(
        "0.00E-15" ,
  ExtendedDecimal.FromString("0.00000000000000000e+0"
).ToEngineeringString());
      Assert.AreEqual(
        "0.00000000000000000" ,
        ExtendedDecimal.FromString("0.00000000000000000e+0").ToPlainString());
      Assert.AreEqual(
        "0E-13" ,
        ExtendedDecimal.FromString("0.0000000000000e+0").ToString());
      Assert.AreEqual(
        "0.0E-12" ,
        ExtendedDecimal.FromString("0.0000000000000e+0" ).ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000" ,
        ExtendedDecimal.FromString("0.0000000000000e+0").ToPlainString());
      Assert.AreEqual(
        "0E-31" ,
        ExtendedDecimal.FromString("0.0000000000000000000e-12").ToString());
      Assert.AreEqual(
        "0.0E-30" ,
        ExtendedDecimal.FromString("0.0000000000000000000e-12"
).ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000000000000000000000" ,
     ExtendedDecimal.FromString("0.0000000000000000000e-12"
).ToPlainString());
      Assert.AreEqual(
        "0E-9" ,
        ExtendedDecimal.FromString("0.0000000000000000000e+10").ToString());
      Assert.AreEqual(
        "0E-9" ,
        ExtendedDecimal.FromString("0.0000000000000000000e+10"
).ToEngineeringString());
      Assert.AreEqual(
        "0.000000000" ,
     ExtendedDecimal.FromString("0.0000000000000000000e+10"
).ToPlainString());
      Assert.AreEqual(
        "0E-7",
        ExtendedDecimal.FromString("0.00000e-2").ToString());
      Assert.AreEqual(
        "0.0E-6",
        ExtendedDecimal.FromString("0.00000e-2").ToEngineeringString());
      Assert.AreEqual(
        "0.0000000",
        ExtendedDecimal.FromString("0.00000e-2").ToPlainString());
      Assert.AreEqual(
        "0E+9",
        ExtendedDecimal.FromString("0.000000e+15").ToString());
      Assert.AreEqual(
        "0E+9",
        ExtendedDecimal.FromString("0.000000e+15").ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.000000e+15").ToPlainString());
      Assert.AreEqual(
        "0E-19",
        ExtendedDecimal.FromString("0.000000000e-10").ToString());
      Assert.AreEqual(
        "0.0E-18" ,
        ExtendedDecimal.FromString("0.000000000e-10").ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000000000" ,
        ExtendedDecimal.FromString("0.000000000e-10").ToPlainString());
      Assert.AreEqual(
        "0E-8" ,
        ExtendedDecimal.FromString("0.00000000000000e+6").ToString());
      Assert.AreEqual(
        "0.00E-6" ,
     ExtendedDecimal.FromString("0.00000000000000e+6"
).ToEngineeringString());
      Assert.AreEqual(
        "0.00000000" ,
        ExtendedDecimal.FromString("0.00000000000000e+6").ToPlainString());
      Assert.AreEqual(
        "0E+12",
        ExtendedDecimal.FromString("0.00000e+17").ToString());
      Assert.AreEqual(
        "0E+12",
        ExtendedDecimal.FromString("0.00000e+17").ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.00000e+17").ToPlainString());
      Assert.AreEqual(
        "0E-18" ,
        ExtendedDecimal.FromString("0.000000000000000000e-0").ToString());
      Assert.AreEqual(
        "0E-18" ,
 ExtendedDecimal.FromString("0.000000000000000000e-0"
).ToEngineeringString());
      Assert.AreEqual(
        "0.000000000000000000" ,
        ExtendedDecimal.FromString("0.000000000000000000e-0").ToPlainString());
      Assert.AreEqual(
        "0.00000" ,
        ExtendedDecimal.FromString("0.0000000000000000e+11").ToString());
      Assert.AreEqual(
        "0.00000" ,
  ExtendedDecimal.FromString("0.0000000000000000e+11"
).ToEngineeringString());
      Assert.AreEqual(
        "0.00000" ,
        ExtendedDecimal.FromString("0.0000000000000000e+11").ToPlainString());
      Assert.AreEqual(
        "0E+3" ,
        ExtendedDecimal.FromString("0.000000000000e+15").ToString());
      Assert.AreEqual(
        "0E+3" ,
        ExtendedDecimal.FromString("0.000000000000e+15" ).ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.000000000000e+15").ToPlainString());
      Assert.AreEqual(
        "0E-27",
        ExtendedDecimal.FromString("0.00000000e-19").ToString());
      Assert.AreEqual(
        "0E-27",
        ExtendedDecimal.FromString("0.00000000e-19").ToEngineeringString());
      Assert.AreEqual(
        "0.000000000000000000000000000" ,
        ExtendedDecimal.FromString("0.00000000e-19").ToPlainString());
      Assert.AreEqual(
        "0E-11",
        ExtendedDecimal.FromString("0.00000e-6").ToString());
      Assert.AreEqual(
        "0.00E-9",
        ExtendedDecimal.FromString("0.00000e-6").ToEngineeringString());
      Assert.AreEqual(
        "0.00000000000" ,
        ExtendedDecimal.FromString("0.00000e-6").ToPlainString());
      Assert.AreEqual("0E-14", ExtendedDecimal.FromString("0e-14").ToString());
      Assert.AreEqual(
        "0.00E-12",
        ExtendedDecimal.FromString("0e-14").ToEngineeringString());
      Assert.AreEqual(
        "0.00000000000000" ,
        ExtendedDecimal.FromString("0e-14").ToPlainString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.000000000e+9").ToString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.000000000e+9").ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.000000000e+9").ToPlainString());
      Assert.AreEqual(
        "0E+8",
        ExtendedDecimal.FromString("0.00000e+13").ToString());
      Assert.AreEqual(
        "0.0E+9",
        ExtendedDecimal.FromString("0.00000e+13").ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.00000e+13").ToPlainString());
      Assert.AreEqual(
        "0.000",
        ExtendedDecimal.FromString("0.000e-0").ToString());
      Assert.AreEqual(
        "0.000",
        ExtendedDecimal.FromString("0.000e-0").ToEngineeringString());
      Assert.AreEqual(
        "0.000",
        ExtendedDecimal.FromString("0.000e-0").ToPlainString());
      Assert.AreEqual(
        "0E-9" ,
        ExtendedDecimal.FromString("0.000000000000000e+6").ToString());
      Assert.AreEqual(
        "0E-9" ,
    ExtendedDecimal.FromString("0.000000000000000e+6"
).ToEngineeringString());
      Assert.AreEqual(
        "0.000000000" ,
        ExtendedDecimal.FromString("0.000000000000000e+6").ToPlainString());
      Assert.AreEqual(
        "0E+8",
        ExtendedDecimal.FromString("0.000000000e+17").ToString());
      Assert.AreEqual(
        "0.0E+9" ,
        ExtendedDecimal.FromString("0.000000000e+17").ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.000000000e+17").ToPlainString());
      Assert.AreEqual(
        "0.00000" ,
        ExtendedDecimal.FromString("0.00000000000e+6").ToString());
      Assert.AreEqual(
        "0.00000" ,
        ExtendedDecimal.FromString("0.00000000000e+6").ToEngineeringString());
      Assert.AreEqual(
        "0.00000" ,
        ExtendedDecimal.FromString("0.00000000000e+6").ToPlainString());
      Assert.AreEqual(
        "0E-11" ,
        ExtendedDecimal.FromString("0.00000000000000e+3").ToString());
      Assert.AreEqual(
        "0.00E-9" ,
     ExtendedDecimal.FromString("0.00000000000000e+3"
).ToEngineeringString());
      Assert.AreEqual(
        "0.00000000000" ,
        ExtendedDecimal.FromString("0.00000000000000e+3").ToPlainString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0e+0").ToString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0e+0").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0e+0").ToPlainString());
      Assert.AreEqual(
        "0E+9",
        ExtendedDecimal.FromString("0.000e+12").ToString());
      Assert.AreEqual(
        "0E+9",
        ExtendedDecimal.FromString("0.000e+12").ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.000e+12").ToPlainString());
      Assert.AreEqual(
        "0.00",
        ExtendedDecimal.FromString("0.00000000000e+9").ToString());
      Assert.AreEqual(
        "0.00",
        ExtendedDecimal.FromString("0.00000000000e+9").ToEngineeringString());
      Assert.AreEqual(
        "0.00",
        ExtendedDecimal.FromString("0.00000000000e+9").ToPlainString());
      Assert.AreEqual(
        "0E-23" ,
        ExtendedDecimal.FromString("0.00000000000000e-9").ToString());
      Assert.AreEqual(
        "0.00E-21" ,
     ExtendedDecimal.FromString("0.00000000000000e-9"
).ToEngineeringString());
      Assert.AreEqual(
        "0.00000000000000000000000" ,
        ExtendedDecimal.FromString("0.00000000000000e-9").ToPlainString());
      Assert.AreEqual("0.0", ExtendedDecimal.FromString("0e-1").ToString());
      Assert.AreEqual(
        "0.0",
        ExtendedDecimal.FromString("0e-1").ToEngineeringString());
      Assert.AreEqual(
        "0.0",
        ExtendedDecimal.FromString("0e-1").ToPlainString());
      Assert.AreEqual(
        "0E-17",
        ExtendedDecimal.FromString("0.0000e-13").ToString());
      Assert.AreEqual(
        "0.00E-15",
        ExtendedDecimal.FromString("0.0000e-13").ToEngineeringString());
      Assert.AreEqual(
        "0.00000000000000000" ,
        ExtendedDecimal.FromString("0.0000e-13").ToPlainString());
      Assert.AreEqual(
        "0E-18" ,
        ExtendedDecimal.FromString("0.00000000000e-7").ToString());
      Assert.AreEqual(
        "0E-18" ,
        ExtendedDecimal.FromString("0.00000000000e-7").ToEngineeringString());
      Assert.AreEqual(
        "0.000000000000000000" ,
        ExtendedDecimal.FromString("0.00000000000e-7").ToPlainString());
      Assert.AreEqual(
        "0E-10" ,
        ExtendedDecimal.FromString("0.00000000000000e+4").ToString());
      Assert.AreEqual(
        "0.0E-9" ,
     ExtendedDecimal.FromString("0.00000000000000e+4"
).ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000" ,
        ExtendedDecimal.FromString("0.00000000000000e+4").ToPlainString());
      Assert.AreEqual(
        "0E-16",
        ExtendedDecimal.FromString("0.00000000e-8").ToString());
      Assert.AreEqual(
        "0.0E-15",
        ExtendedDecimal.FromString("0.00000000e-8").ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000000" ,
        ExtendedDecimal.FromString("0.00000000e-8").ToPlainString());
      Assert.AreEqual("0E-8", ExtendedDecimal.FromString("0.00e-6").ToString());
      Assert.AreEqual(
        "0.00E-6",
        ExtendedDecimal.FromString("0.00e-6").ToEngineeringString());
      Assert.AreEqual(
        "0.00000000",
        ExtendedDecimal.FromString("0.00e-6").ToPlainString());
      Assert.AreEqual("0.00", ExtendedDecimal.FromString("0.0e-1").ToString());
      Assert.AreEqual(
        "0.00",
        ExtendedDecimal.FromString("0.0e-1").ToEngineeringString());
      Assert.AreEqual(
        "0.00",
        ExtendedDecimal.FromString("0.0e-1").ToPlainString());
      Assert.AreEqual(
        "0E-26" ,
        ExtendedDecimal.FromString("0.0000000000000000e-10").ToString());
      Assert.AreEqual(
        "0.00E-24" ,
  ExtendedDecimal.FromString("0.0000000000000000e-10"
).ToEngineeringString());
      Assert.AreEqual(
        "0.00000000000000000000000000" ,
        ExtendedDecimal.FromString("0.0000000000000000e-10").ToPlainString());
      Assert.AreEqual(
        "0E+12",
        ExtendedDecimal.FromString("0.00e+14").ToString());
      Assert.AreEqual(
        "0E+12",
        ExtendedDecimal.FromString("0.00e+14").ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.00e+14").ToPlainString());
      Assert.AreEqual(
        "0E-13" ,
        ExtendedDecimal.FromString("0.000000000000000000e+5").ToString());
      Assert.AreEqual(
        "0.0E-12" ,
 ExtendedDecimal.FromString("0.000000000000000000e+5"
).ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000" ,
        ExtendedDecimal.FromString("0.000000000000000000e+5").ToPlainString());
      Assert.AreEqual("0E+6", ExtendedDecimal.FromString("0.0e+7").ToString());
      Assert.AreEqual(
        "0E+6",
        ExtendedDecimal.FromString("0.0e+7").ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.0e+7").ToPlainString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.00000000e+8").ToString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.00000000e+8").ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.00000000e+8").ToPlainString());
      Assert.AreEqual(
        "0E-9",
        ExtendedDecimal.FromString("0.000000000e+0").ToString());
      Assert.AreEqual(
        "0E-9",
        ExtendedDecimal.FromString("0.000000000e+0").ToEngineeringString());
      Assert.AreEqual(
        "0.000000000" ,
        ExtendedDecimal.FromString("0.000000000e+0").ToPlainString());
      Assert.AreEqual(
        "0E+10",
        ExtendedDecimal.FromString("0.000e+13").ToString());
      Assert.AreEqual(
        "0.00E+12",
        ExtendedDecimal.FromString("0.000e+13").ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.000e+13").ToPlainString());
      Assert.AreEqual(
        "0" ,
        ExtendedDecimal.FromString("0.0000000000000000e+16").ToString());
      Assert.AreEqual(
        "0" ,
  ExtendedDecimal.FromString("0.0000000000000000e+16"
).ToEngineeringString());
      Assert.AreEqual(
        "0" ,
        ExtendedDecimal.FromString("0.0000000000000000e+16").ToPlainString());
      Assert.AreEqual(
        "0E-9",
        ExtendedDecimal.FromString("0.00000000e-1").ToString());
      Assert.AreEqual(
        "0E-9",
        ExtendedDecimal.FromString("0.00000000e-1").ToEngineeringString());
      Assert.AreEqual(
        "0.000000000" ,
        ExtendedDecimal.FromString("0.00000000e-1").ToPlainString());
      Assert.AreEqual(
        "0E-26" ,
        ExtendedDecimal.FromString("0.00000000000e-15").ToString());
      Assert.AreEqual(
        "0.00E-24" ,
        ExtendedDecimal.FromString("0.00000000000e-15").ToEngineeringString());
      Assert.AreEqual(
        "0.00000000000000000000000000" ,
        ExtendedDecimal.FromString("0.00000000000e-15").ToPlainString());
      Assert.AreEqual(
        "0E+10",
        ExtendedDecimal.FromString("0.0e+11").ToString());
      Assert.AreEqual(
        "0.00E+12",
        ExtendedDecimal.FromString("0.0e+11").ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.0e+11").ToPlainString());
      Assert.AreEqual(
        "0E+2",
        ExtendedDecimal.FromString("0.00000e+7").ToString());
      Assert.AreEqual(
        "0.0E+3",
        ExtendedDecimal.FromString("0.00000e+7").ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.00000e+7").ToPlainString());
      Assert.AreEqual(
        "0E-38" ,
        ExtendedDecimal.FromString("0.0000000000000000000e-19").ToString());
      Assert.AreEqual(
        "0.00E-36" ,
        ExtendedDecimal.FromString("0.0000000000000000000e-19"
).ToEngineeringString());
      Assert.AreEqual(
        "0.00000000000000000000000000000000000000" ,
     ExtendedDecimal.FromString("0.0000000000000000000e-19"
).ToPlainString());
      Assert.AreEqual(
        "0E-16",
        ExtendedDecimal.FromString("0.0000000000e-6").ToString());
      Assert.AreEqual(
        "0.0E-15" ,
        ExtendedDecimal.FromString("0.0000000000e-6").ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000000" ,
        ExtendedDecimal.FromString("0.0000000000e-6").ToPlainString());
      Assert.AreEqual(
        "0E-32" ,
        ExtendedDecimal.FromString("0.00000000000000000e-15").ToString());
      Assert.AreEqual(
        "0.00E-30" ,
 ExtendedDecimal.FromString("0.00000000000000000e-15"
).ToEngineeringString());
      Assert.AreEqual(
        "0.00000000000000000000000000000000" ,
        ExtendedDecimal.FromString("0.00000000000000000e-15").ToPlainString());
      Assert.AreEqual(
        "0E-13" ,
        ExtendedDecimal.FromString("0.000000000000000e+2").ToString());
      Assert.AreEqual(
        "0.0E-12" ,
    ExtendedDecimal.FromString("0.000000000000000e+2"
).ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000" ,
        ExtendedDecimal.FromString("0.000000000000000e+2").ToPlainString());
      Assert.AreEqual(
        "0E-19",
        ExtendedDecimal.FromString("0.0e-18").ToString());
      Assert.AreEqual(
        "0.0E-18",
        ExtendedDecimal.FromString("0.0e-18").ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000000000" ,
        ExtendedDecimal.FromString("0.0e-18").ToPlainString());
      Assert.AreEqual(
        "0E-20" ,
        ExtendedDecimal.FromString("0.00000000000000e-6").ToString());
      Assert.AreEqual(
        "0.00E-18" ,
     ExtendedDecimal.FromString("0.00000000000000e-6"
).ToEngineeringString());
      Assert.AreEqual(
        "0.00000000000000000000" ,
        ExtendedDecimal.FromString("0.00000000000000e-6").ToPlainString());
      Assert.AreEqual(
        "0E-20",
        ExtendedDecimal.FromString("0.000e-17").ToString());
      Assert.AreEqual(
        "0.00E-18",
        ExtendedDecimal.FromString("0.000e-17").ToEngineeringString());
      Assert.AreEqual(
        "0.00000000000000000000" ,
        ExtendedDecimal.FromString("0.000e-17").ToPlainString());
      Assert.AreEqual(
        "0E-21" ,
        ExtendedDecimal.FromString("0.00000000000000e-7").ToString());
      Assert.AreEqual(
        "0E-21" ,
     ExtendedDecimal.FromString("0.00000000000000e-7"
).ToEngineeringString());
      Assert.AreEqual(
        "0.000000000000000000000" ,
        ExtendedDecimal.FromString("0.00000000000000e-7").ToPlainString());
      Assert.AreEqual(
        "0E-15",
        ExtendedDecimal.FromString("0.000000e-9").ToString());
      Assert.AreEqual(
        "0E-15",
        ExtendedDecimal.FromString("0.000000e-9").ToEngineeringString());
      Assert.AreEqual(
        "0.000000000000000" ,
        ExtendedDecimal.FromString("0.000000e-9").ToPlainString());
      Assert.AreEqual("0E-11", ExtendedDecimal.FromString("0e-11").ToString());
      Assert.AreEqual(
        "0.00E-9",
        ExtendedDecimal.FromString("0e-11").ToEngineeringString());
      Assert.AreEqual(
        "0.00000000000",
        ExtendedDecimal.FromString("0e-11").ToPlainString());
      Assert.AreEqual(
        "0E+2",
        ExtendedDecimal.FromString("0.000000000e+11").ToString());
      Assert.AreEqual(
        "0.0E+3" ,
        ExtendedDecimal.FromString("0.000000000e+11").ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.000000000e+11").ToPlainString());
      Assert.AreEqual(
        "0.0" ,
        ExtendedDecimal.FromString("0.0000000000000000e+15").ToString());
      Assert.AreEqual(
        "0.0" ,
  ExtendedDecimal.FromString("0.0000000000000000e+15"
).ToEngineeringString());
      Assert.AreEqual(
        "0.0" ,
        ExtendedDecimal.FromString("0.0000000000000000e+15").ToPlainString());
      Assert.AreEqual(
        "0.000000" ,
        ExtendedDecimal.FromString("0.0000000000000000e+10").ToString());
      Assert.AreEqual(
        "0.000000" ,
  ExtendedDecimal.FromString("0.0000000000000000e+10"
).ToEngineeringString());
      Assert.AreEqual(
        "0.000000" ,
        ExtendedDecimal.FromString("0.0000000000000000e+10").ToPlainString());
      Assert.AreEqual(
        "0.00000" ,
        ExtendedDecimal.FromString("0.000000000e+4").ToString());
      Assert.AreEqual(
        "0.00000" ,
        ExtendedDecimal.FromString("0.000000000e+4").ToEngineeringString());
      Assert.AreEqual(
        "0.00000" ,
        ExtendedDecimal.FromString("0.000000000e+4").ToPlainString());
      Assert.AreEqual(
        "0E-28" ,
        ExtendedDecimal.FromString("0.000000000000000e-13").ToString());
      Assert.AreEqual(
        "0.0E-27" ,
   ExtendedDecimal.FromString("0.000000000000000e-13"
).ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000000000000000000" ,
        ExtendedDecimal.FromString("0.000000000000000e-13").ToPlainString());
      Assert.AreEqual(
        "0E-27" ,
        ExtendedDecimal.FromString("0.0000000000000000000e-8").ToString());
      Assert.AreEqual(
        "0E-27" ,
ExtendedDecimal.FromString("0.0000000000000000000e-8"
).ToEngineeringString());
      Assert.AreEqual(
        "0.000000000000000000000000000" ,
        ExtendedDecimal.FromString("0.0000000000000000000e-8" ).ToPlainString());
      Assert.AreEqual(
        "0E-26" ,
        ExtendedDecimal.FromString("0.00000000000e-15").ToString());
      Assert.AreEqual(
        "0.00E-24" ,
        ExtendedDecimal.FromString("0.00000000000e-15").ToEngineeringString());
      Assert.AreEqual(
        "0.00000000000000000000000000" ,
        ExtendedDecimal.FromString("0.00000000000e-15").ToPlainString());
      Assert.AreEqual(
        "0E+10",
        ExtendedDecimal.FromString("0.00e+12").ToString());
      Assert.AreEqual(
        "0.00E+12",
        ExtendedDecimal.FromString("0.00e+12").ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.00e+12").ToPlainString());
      Assert.AreEqual("0E+4", ExtendedDecimal.FromString("0.0e+5").ToString());
      Assert.AreEqual(
        "0.00E+6",
        ExtendedDecimal.FromString("0.0e+5").ToEngineeringString());
      Assert.AreEqual(
        "0",
        ExtendedDecimal.FromString("0.0e+5").ToPlainString());
      Assert.AreEqual(
        "0E-9" ,
        ExtendedDecimal.FromString("0.0000000000000000e+7").ToString());
      Assert.AreEqual(
        "0E-9" ,
   ExtendedDecimal.FromString("0.0000000000000000e+7"
).ToEngineeringString());
      Assert.AreEqual(
        "0.000000000" ,
        ExtendedDecimal.FromString("0.0000000000000000e+7").ToPlainString());
      Assert.AreEqual(
        "0E-16" ,
        ExtendedDecimal.FromString("0.0000000000000000e-0").ToString());
      Assert.AreEqual(
        "0.0E-15" ,
   ExtendedDecimal.FromString("0.0000000000000000e-0"
).ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000000" ,
        ExtendedDecimal.FromString("0.0000000000000000e-0").ToPlainString());
      Assert.AreEqual(
        "0.00" ,
        ExtendedDecimal.FromString("0.000000000000000e+13").ToString());
      Assert.AreEqual(
        "0.00" ,
   ExtendedDecimal.FromString("0.000000000000000e+13"
).ToEngineeringString());
      Assert.AreEqual(
        "0.00" ,
        ExtendedDecimal.FromString("0.000000000000000e+13").ToPlainString());
      Assert.AreEqual(
        "0E-24" ,
        ExtendedDecimal.FromString("0.00000000000e-13").ToString());
      Assert.AreEqual(
        "0E-24" ,
        ExtendedDecimal.FromString("0.00000000000e-13").ToEngineeringString());
      Assert.AreEqual(
        "0.000000000000000000000000" ,
        ExtendedDecimal.FromString("0.00000000000e-13").ToPlainString());
      Assert.AreEqual(
        "0E-13",
        ExtendedDecimal.FromString("0.000e-10").ToString());
      Assert.AreEqual(
        "0.0E-12",
        ExtendedDecimal.FromString("0.000e-10").ToEngineeringString());
      Assert.AreEqual(
        "0.0000000000000" ,
        ExtendedDecimal.FromString("0.000e-10").ToPlainString());
    }

    [TestMethod]
    public void TestDecimalsEquivalent() {
      AssertDecimalsEquivalent("1.310E-7", "131.0E-9");
      AssertDecimalsEquivalent("0.001231", "123.1E-5");
      AssertDecimalsEquivalent("3.0324E+6", "303.24E4");
      AssertDecimalsEquivalent("3.726E+8", "372.6E6");
      AssertDecimalsEquivalent("2663.6", "266.36E1");
      AssertDecimalsEquivalent("34.24", "342.4E-1");
      AssertDecimalsEquivalent("3492.5", "349.25E1");
      AssertDecimalsEquivalent("0.31919", "319.19E-3");
      AssertDecimalsEquivalent("2.936E-7", "293.6E-9");
      AssertDecimalsEquivalent("6.735E+10", "67.35E9");
      AssertDecimalsEquivalent("7.39E+10", "7.39E10");
      AssertDecimalsEquivalent("0.0020239", "202.39E-5");
      AssertDecimalsEquivalent("1.6717E+6", "167.17E4");
      AssertDecimalsEquivalent("1.7632E+9", "176.32E7");
      AssertDecimalsEquivalent("39.526", "395.26E-1");
      AssertDecimalsEquivalent("0.002939", "29.39E-4");
      AssertDecimalsEquivalent("0.3165", "316.5E-3");
      AssertDecimalsEquivalent("3.7910E-7", "379.10E-9");
      AssertDecimalsEquivalent("0.000016035", "160.35E-7");
      AssertDecimalsEquivalent("0.001417", "141.7E-5");
      AssertDecimalsEquivalent("7.337E+5", "73.37E4");
      AssertDecimalsEquivalent("3.4232E+12", "342.32E10");
      AssertDecimalsEquivalent("2.828E+8", "282.8E6");
      AssertDecimalsEquivalent("4.822E-7", "48.22E-8");
      AssertDecimalsEquivalent("2.6328E+9", "263.28E7");
      AssertDecimalsEquivalent("2.9911E+8", "299.11E6");
      AssertDecimalsEquivalent("3.636E+9", "36.36E8");
      AssertDecimalsEquivalent("0.20031", "200.31E-3");
      AssertDecimalsEquivalent("1.922E+7", "19.22E6");
      AssertDecimalsEquivalent("3.0924E+8", "309.24E6");
      AssertDecimalsEquivalent("2.7236E+7", "272.36E5");
      AssertDecimalsEquivalent("0.01645", "164.5E-4");
      AssertDecimalsEquivalent("0.000292", "29.2E-5");
      AssertDecimalsEquivalent("1.9939", "199.39E-2");
      AssertDecimalsEquivalent("2.7929E+9", "279.29E7");
      AssertDecimalsEquivalent("1.213E+7", "121.3E5");
      AssertDecimalsEquivalent("2.765E+6", "276.5E4");
      AssertDecimalsEquivalent("270.11", "270.11E0");
      AssertDecimalsEquivalent("0.017718", "177.18E-4");
      AssertDecimalsEquivalent("0.003607", "360.7E-5");
      AssertDecimalsEquivalent("0.00038618", "386.18E-6");
      AssertDecimalsEquivalent("0.0004230", "42.30E-5");
      AssertDecimalsEquivalent("1.8410E+5", "184.10E3");
      AssertDecimalsEquivalent("0.00030427", "304.27E-6");
      AssertDecimalsEquivalent("6.513E+6", "65.13E5");
      AssertDecimalsEquivalent("0.06717", "67.17E-3");
      AssertDecimalsEquivalent("0.00031123", "311.23E-6");
      AssertDecimalsEquivalent("0.0031639", "316.39E-5");
      AssertDecimalsEquivalent("1.146E+5", "114.6E3");
      AssertDecimalsEquivalent("0.00039937", "399.37E-6");
      AssertDecimalsEquivalent("3.3817", "338.17E-2");
      AssertDecimalsEquivalent("0.00011128", "111.28E-6");
      AssertDecimalsEquivalent("7.818E+7", "78.18E6");
      AssertDecimalsEquivalent("2.6417E-7", "264.17E-9");
      AssertDecimalsEquivalent("1.852E+9", "185.2E7");
      AssertDecimalsEquivalent("0.0016216", "162.16E-5");
      AssertDecimalsEquivalent("2.2813E+6", "228.13E4");
      AssertDecimalsEquivalent("3.078E+12", "307.8E10");
      AssertDecimalsEquivalent("0.00002235", "22.35E-6");
      AssertDecimalsEquivalent("0.0032827", "328.27E-5");
      AssertDecimalsEquivalent("1.334E+9", "133.4E7");
      AssertDecimalsEquivalent("34.022", "340.22E-1");
      AssertDecimalsEquivalent("7.19E+6", "7.19E6");
      AssertDecimalsEquivalent("35.311", "353.11E-1");
      AssertDecimalsEquivalent("3.4330E+6", "343.30E4");
      AssertDecimalsEquivalent("0.000022923", "229.23E-7");
      AssertDecimalsEquivalent("2.899E+4", "289.9E2");
      AssertDecimalsEquivalent("0.00031", "3.1E-4");
      AssertDecimalsEquivalent("2.0418E+5", "204.18E3");
      AssertDecimalsEquivalent("3.3412E+11", "334.12E9");
      AssertDecimalsEquivalent("1.717E+10", "171.7E8");
      AssertDecimalsEquivalent("2.7024E+10", "270.24E8");
      AssertDecimalsEquivalent("1.0219E+9", "102.19E7");
      AssertDecimalsEquivalent("15.13", "151.3E-1");
      AssertDecimalsEquivalent("91.23", "91.23E0");
      AssertDecimalsEquivalent("3.4114E+6", "341.14E4");
      AssertDecimalsEquivalent("33.832", "338.32E-1");
      AssertDecimalsEquivalent("0.19234", "192.34E-3");
      AssertDecimalsEquivalent("16835", "168.35E2");
      AssertDecimalsEquivalent("0.00038610", "386.10E-6");
      AssertDecimalsEquivalent("1.6624E+9", "166.24E7");
      AssertDecimalsEquivalent("2.351E+9", "235.1E7");
      AssertDecimalsEquivalent("0.03084", "308.4E-4");
      AssertDecimalsEquivalent("0.00429", "42.9E-4");
      AssertDecimalsEquivalent("9.718E-8", "97.18E-9");
      AssertDecimalsEquivalent("0.00003121", "312.1E-7");
      AssertDecimalsEquivalent("3.175E+4", "317.5E2");
      AssertDecimalsEquivalent("376.6", "376.6E0");
      AssertDecimalsEquivalent("0.0000026110", "261.10E-8");
      AssertDecimalsEquivalent("7.020E+11", "70.20E10");
      AssertDecimalsEquivalent("2.1533E+9", "215.33E7");
      AssertDecimalsEquivalent("3.8113E+7", "381.13E5");
      AssertDecimalsEquivalent("7.531", "75.31E-1");
      AssertDecimalsEquivalent("991.0", "99.10E1");
      AssertDecimalsEquivalent("2.897E+8", "289.7E6");
      AssertDecimalsEquivalent("0.0000033211", "332.11E-8");
      AssertDecimalsEquivalent("0.03169", "316.9E-4");
      AssertDecimalsEquivalent("2.7321E+12", "273.21E10");
      AssertDecimalsEquivalent("394.38", "394.38E0");
      AssertDecimalsEquivalent("5.912E+7", "59.12E6");
    }

    [TestMethod]
    public void TestAsByte() {
      for (var i = 0; i < 255; ++i) {
        Assert.AreEqual((byte)i, CBORObject.FromObject(i).AsByte());
      }
      for (int i = -200; i < 0; ++i) {
        try {
          CBORObject.FromObject(i).AsByte();
        } catch (OverflowException) {
        } catch (Exception ex) {
          Assert.Fail(ex.ToString()); throw new
            InvalidOperationException(String.Empty, ex);
        }
      }
      for (int i = 256; i < 512; ++i) {
        try {
          CBORObject.FromObject(i).AsByte();
        } catch (OverflowException) {
        } catch (Exception ex) {
          Assert.Fail(ex.ToString()); throw new
            InvalidOperationException(String.Empty, ex);
        }
      }
    }
    [TestMethod]
    [ExpectedException(typeof(CBORException))]
    public void TestByteStringStreamNoIndefiniteWithinDefinite() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0x5f,
        0x41,
        0x20,
        0x5f,
        0x41,
        0x20,
        0xff,
        0xff });
    }

    [TestMethod]
    public void TestExceptions() {
      try {
        PrecisionContext.Unlimited.WithBigPrecision(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8String(null, false);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes(null, false);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.WriteUtf8(null, 0, 1, null, false);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.WriteUtf8("xyz", 0, 1, null, false);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.WriteUtf8(null, null, false);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.WriteUtf8("xyz", null, false);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\ud800", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\udc00", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\ud800\ud800", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\udc00\udc00", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\udc00\ud800", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8String(null, 0, 1, false);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [TestMethod]
    public void TestExtendedMiscellaneous() {
      Assert.AreEqual(
        ExtendedDecimal.Zero,
        ExtendedDecimal.FromExtendedFloat(ExtendedFloat.Zero));
      Assert.AreEqual(
        ExtendedDecimal.NegativeZero,
        ExtendedDecimal.FromExtendedFloat(ExtendedFloat.NegativeZero));
      Assert.AreEqual(ExtendedDecimal.Zero, ExtendedDecimal.FromInt32(0));
      Assert.AreEqual(ExtendedDecimal.One, ExtendedDecimal.FromInt32(1));
      Assert.AreEqual("sNaN", ExtendedDecimal.SignalingNaN.ToString());
      Assert.AreEqual(
        "sNaN" ,
        ExtendedDecimal.SignalingNaN.ToEngineeringString());
      Assert.AreEqual("sNaN", ExtendedDecimal.SignalingNaN.ToPlainString());
      Assert.AreEqual(
        ExtendedFloat.Zero,
        ExtendedDecimal.Zero.ToExtendedFloat());
      Assert.AreEqual(
        ExtendedFloat.NegativeZero,
        ExtendedDecimal.NegativeZero.ToExtendedFloat());
      if (0.0 != ExtendedDecimal.Zero.ToSingle()) {
        Assert.Fail("Failed " + ExtendedDecimal.Zero.ToSingle());
      }
      if (0.0 != ExtendedDecimal.Zero.ToDouble()) {
        Assert.Fail("Failed " + ExtendedDecimal.Zero.ToDouble());
      }
      if (0.0f != ExtendedFloat.Zero.ToSingle()) {
        Assert.Fail("Failed " + ExtendedFloat.Zero.ToDouble());
      }
      if (0.0f != ExtendedFloat.Zero.ToDouble()) {
        Assert.Fail("Failed " + ExtendedFloat.Zero.ToDouble());
      }
      try {
        CBORObject.FromObject(CBORObject.FromObject(Double.NaN).Sign);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject cbor = CBORObject.True;
      try {
        CBORObject.FromObject(cbor[0]);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor[0] = CBORObject.False;
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor = CBORObject.False;
        CBORObject.FromObject(cbor.Keys);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(CBORObject.NewArray().Keys);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(CBORObject.NewArray().Sign);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(CBORObject.NewMap().Sign);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(
        CBORObject.FromObject(0.1),
        CBORObject.FromObjectAndTag(0.1, 999999).Untag());
      Assert.AreEqual(-1, CBORObject.NewArray().SimpleValue);
      Assert.AreEqual(
        0,
        CBORObject.FromObject(0.1).CompareTo(CBORObject.FromObject(0.1)));
      Assert.AreEqual(
        0,
        CBORObject.FromObject(0.1f).CompareTo(CBORObject.FromObject(0.1f)));
      Assert.AreEqual(
        CBORObject.FromObject(2),
        CBORObject.FromObject(-2).Negate());
      Assert.AreEqual(
        CBORObject.FromObject(-2),
        CBORObject.FromObject(2).Negate());
      Assert.AreEqual(
        CBORObject.FromObject(2),
        CBORObject.FromObject(-2).Abs());
      Assert.AreEqual(CBORObject.FromObject(2), CBORObject.FromObject(2).Abs());
    }

    [TestMethod]
    public void TestExtendedDecimalExceptions() {
      try {
        ExtendedDecimal.Max(null, ExtendedDecimal.One);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.Max(ExtendedDecimal.One, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        ExtendedDecimal.MinMagnitude(null, ExtendedDecimal.One);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.MinMagnitude(ExtendedDecimal.One, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        ExtendedDecimal.MaxMagnitude(null, ExtendedDecimal.One);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.MaxMagnitude(ExtendedDecimal.One, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        ExtendedFloat.Min(null, ExtendedFloat.One);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.Min(ExtendedFloat.One, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        ExtendedFloat.Max(null, ExtendedFloat.One);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.Max(ExtendedFloat.One, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        ExtendedFloat.MinMagnitude(null, ExtendedFloat.One);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.MinMagnitude(ExtendedFloat.One, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        ExtendedFloat.MaxMagnitude(null, ExtendedFloat.One);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.MaxMagnitude(ExtendedFloat.One, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.Zero.Subtract(null, PrecisionContext.Unlimited);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.Zero.Subtract(null, PrecisionContext.Unlimited);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.Zero.Add(null, PrecisionContext.Unlimited);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.Zero.Add(null, PrecisionContext.Unlimited);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.FromExtendedFloat(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.FromString(String.Empty);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString(String.Empty);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [TestMethod]
    public void TestExtendedFloatDecFrac() {
      ExtendedFloat bf;
      bf = ExtendedFloat.FromInt64(20);
      Assert.AreEqual("20", ExtendedDecimal.FromExtendedFloat(bf).ToString());
      bf = ExtendedFloat.Create((BigInteger)3, (BigInteger)(-1));
      Assert.AreEqual("1.5", ExtendedDecimal.FromExtendedFloat(bf).ToString());
      bf = ExtendedFloat.Create((BigInteger)(-3), (BigInteger)(-1));
      Assert.AreEqual("-1.5", ExtendedDecimal.FromExtendedFloat(bf).ToString());
      ExtendedDecimal df;
      df = ExtendedDecimal.FromInt64(20);
      Assert.AreEqual("20", df.ToExtendedFloat().ToString());
      df = ExtendedDecimal.FromInt64(-20);
      Assert.AreEqual("-20", df.ToExtendedFloat().ToString());
      df = ExtendedDecimal.Create((BigInteger)15, (BigInteger)(-1));
      Assert.AreEqual("1.5", df.ToExtendedFloat().ToString());
      df = ExtendedDecimal.Create((BigInteger)(-15), (BigInteger)(-1));
      Assert.AreEqual("-1.5", df.ToExtendedFloat().ToString());
    }

    [TestMethod]
    public void TestCanFitInSpecificCases() {
      CBORObject cbor = CBORObject.DecodeFromBytes(new byte[] { (byte)0xfb,
                                                     0x41, (byte)0xe0,
                                                     (byte)0x85, 0x48,
                                                     0x2d, 0x14, 0x47,
                                                  0x7a });  // 2217361768.63373
      Assert.AreEqual(BigInteger.fromString("2217361768"), cbor.AsBigInteger());
      Assert.IsFalse(cbor.AsBigInteger().canFitInInt());
      Assert.IsFalse(cbor.CanTruncatedIntFitInInt32());
      cbor = CBORObject.DecodeFromBytes(new byte[] { (byte)0xc5, (byte)0x82,
                                     0x18, 0x2f, 0x32 });  // -2674012278751232
      Assert.AreEqual(52, cbor.AsBigInteger().bitLength());
      Assert.IsTrue(cbor.CanFitInInt64());
      Assert.IsFalse(CBORObject.FromObject(2554895343L).CanFitInSingle());
      cbor = CBORObject.DecodeFromBytes(new byte[] { (byte)0xc5, (byte)0x82,
                                          0x10, 0x38, 0x64 });  // -6619136
      Assert.AreEqual((BigInteger)(-6619136), cbor.AsBigInteger());
      Assert.AreEqual(-6619136, cbor.AsBigInteger().intValue());
      Assert.AreEqual(-6619136, cbor.AsInt32());
      Assert.IsTrue(cbor.AsBigInteger().canFitInInt());
      Assert.IsTrue(cbor.CanTruncatedIntFitInInt32());
    }

    [TestMethod]
    public void TestCanFitIn() {
      var r = new FastRandom();
      for (var i = 0; i < 5000; ++i) {
        CBORObject ed = RandomNumber(r);
        ExtendedDecimal ed2;
        ed2 = ExtendedDecimal.FromDouble(ed.AsExtendedDecimal().ToDouble());
        if ((ed.AsExtendedDecimal().CompareTo(ed2) == 0) !=
            ed.CanFitInDouble()) {
          Assert.Fail(ToByteArrayString(ed) + "; /" + "/ " + ed.ToJSONString());
        }
        ed2 = ExtendedDecimal.FromSingle(ed.AsExtendedDecimal().ToSingle());
        if ((ed.AsExtendedDecimal().CompareTo(ed2) == 0) !=
            ed.CanFitInSingle()) {
          Assert.Fail(ToByteArrayString(ed) + "; /" + "/ " + ed.ToJSONString());
        }
        if (!ed.IsInfinity() && !ed.IsNaN()) {
          ed2 =
            ExtendedDecimal.FromBigInteger(ed.AsExtendedDecimal()
                                           .ToBigInteger());
          if ((ed.AsExtendedDecimal().CompareTo(ed2) == 0) != ed.IsIntegral) {
            Assert.Fail(ToByteArrayString(ed) + "; /" + "/ " +
                        ed.ToJSONString());
          }
        }
        if (!ed.IsInfinity() && !ed.IsNaN()) {
          BigInteger bi = ed.AsBigInteger();
          if (ed.IsIntegral) {
            if (bi.canFitInInt() != ed.CanFitInInt32()) {
              Assert.Fail(ToByteArrayString(ed) + "; /" + "/ " +
                          ed.ToJSONString());
            }
          }
          if (bi.canFitInInt() != ed.CanTruncatedIntFitInInt32()) {
            Assert.Fail(ToByteArrayString(ed) + "; /" + "/ " +
                        ed.ToJSONString());
          }
          if (ed.IsIntegral) {
            if ((bi.bitLength() <= 63) != ed.CanFitInInt64()) {
              Assert.Fail(ToByteArrayString(ed) + "; /" + "/ " +
                          ed.ToJSONString());
            }
          }
          if ((bi.bitLength() <= 63) != ed.CanTruncatedIntFitInInt64()) {
            Assert.Fail(ToByteArrayString(ed) + "; /" + "/ " +
                        ed.ToJSONString());
          }
        }
      }
    }

    [TestMethod]
    public void TestDecimalFrac() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0xc4,
        0x82,
        0x3,
        0x1a,
        1,
        2,
        3,
        4 });
    }
    [TestMethod]
    [ExpectedException(typeof(CBORException))]
    public void TestDecimalFracExponentMustNotBeBignum() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0xc4,
        0x82,
        0xc2,
        0x41,
        1,
        0x1a,
        1,
        2,
        3,
        4 });
    }
    [TestMethod]
    [ExpectedException(typeof(CBORException))]
    public void TestDecimalFracExactlyTwoElements() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0xc4,
        0x82,
        0xc2,
        0x41,
        1 });
    }

    [TestMethod]
    public void TestDecimalFracMantissaMayBeBignum() {
      CBORObject o = TestCommon.FromBytesTestAB(
        new byte[] { 0xc4,
        0x82,
        0x3,
        0xc2,
        0x41,
        1 });
      Assert.AreEqual(
        ExtendedDecimal.Create(BigInteger.One, (BigInteger)3),
        o.AsExtendedDecimal());
    }

    [TestMethod]
    public void TestShort() {
      for (int i = Int16.MinValue; i <= Int16.MaxValue; ++i) {
        TestCommon.AssertSer(
          CBORObject.FromObject((short)i),
          String.Empty + i);
      }
    }

    [TestMethod]
    public void TestByteArray() {
      TestCommon.AssertSer(
        CBORObject.FromObject(new byte[] { 0x20, 0x78 }),
        "h'2078'");
    }

    [TestMethod]
    public void TestBigNumBytes() {
      CBORObject o = null;
      o = TestCommon.FromBytesTestAB(new byte[] { 0xc2, 0x41, 0x88 });
      Assert.AreEqual((BigInteger)0x88L, o.AsBigInteger());
      o = TestCommon.FromBytesTestAB(new byte[] { 0xc2, 0x42, 0x88, 0x77 });
      Assert.AreEqual((BigInteger)0x8877L, o.AsBigInteger());
      o = TestCommon.FromBytesTestAB(new byte[] { 0xc2, 0x44, 0x88, 0x77,
                                       0x66, 0x55 });
      Assert.AreEqual((BigInteger)0x88776655L, o.AsBigInteger());
      o = TestCommon.FromBytesTestAB(new byte[] { 0xc2, 0x47, 0x88, 0x77,
                                       0x66, 0x55, 0x44, 0x33, 0x22 });
      Assert.AreEqual((BigInteger)0x88776655443322L, o.AsBigInteger());
    }

    [TestMethod]
    public void TestMapInMap() {
      CBORObject oo;
      oo = CBORObject.NewArray()
        .Add(CBORObject.NewMap()
             .Add(
               new ExtendedRational(BigInteger.One, (BigInteger)2),
               3)
             .Add(4, false))
        .Add(true);
      TestCommon.AssertRoundTrip(oo);
      oo = CBORObject.NewArray();
      oo.Add(CBORObject.FromObject(0));
      CBORObject oo2 = CBORObject.NewMap();
      oo2.Add(CBORObject.FromObject(1), CBORObject.FromObject(1368));
      CBORObject oo3 = CBORObject.NewMap();
      oo3.Add(CBORObject.FromObject(2), CBORObject.FromObject(1625));
      CBORObject oo4 = CBORObject.NewMap();
      oo4.Add(oo2, CBORObject.True);
      oo4.Add(oo3, CBORObject.True);
      oo.Add(oo4);
      TestCommon.AssertRoundTrip(oo);
    }

    [TestMethod]
    public void TestTaggedUntagged() {
      for (int i = 200; i < 1000; ++i) {
        if (i == 264 || i == 265 || i + 1 == 264 || i + 1 == 265) {
          // Skip since they're being used as
          // arbitrary-precision numbers
          continue;
        }
        CBORObject o, o2;
        o = CBORObject.FromObject(0);
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObject(BigInteger.One << 100);
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObject(new byte[] { 1, 2, 3 });
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.NewArray();
        o.Add(CBORObject.FromObject(0));
        o.Add(CBORObject.FromObject(1));
        o.Add(CBORObject.FromObject(2));
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.NewMap();
        o.Add("a", CBORObject.FromObject(0));
        o.Add("b", CBORObject.FromObject(1));
        o.Add("c", CBORObject.FromObject(2));
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObject("a");
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.False;
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.True;
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.Null;
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.Undefined;
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
      }
    }

    public static void AssertBigIntString(string s, BigInteger bi) {
      Assert.AreEqual(s, bi.ToString());
    }

    [TestMethod]
    public void TestCBORBigInteger() {
      CBORObject o = CBORObject.DecodeFromBytes(new byte[] { 0x3b,
                                (byte)0xce, (byte)0xe2, 0x5a, 0x57,
                                                    (byte)0xd8,
                                                  0x21,
                                                  (byte)0xb9,
                                                  (byte)0xa7 });
      Assert.AreEqual(
        BigInteger.fromString("-14907577049884506536"),
        o.AsBigInteger());
    }

    public static void AssertAdd(BigInteger bi, BigInteger bi2, string s) {
      AssertBigIntString(s, bi + (BigInteger)bi2);
      AssertBigIntString(s, bi2 + (BigInteger)bi);
      BigInteger negbi = BigInteger.Zero - (BigInteger)bi;
      BigInteger negbi2 = BigInteger.Zero - (BigInteger)bi2;
      AssertBigIntString(s, bi - (BigInteger)negbi2);
      AssertBigIntString(s, bi2 - (BigInteger)negbi);
    }

    [TestMethod]
    public void TestBigIntAddSub() {
      var posSmall = (BigInteger)5;
      BigInteger negSmall = -(BigInteger)5;
      var posLarge = (BigInteger)5555555;
      BigInteger negLarge = -(BigInteger)5555555;
      AssertAdd(posSmall, posSmall, "10");
      AssertAdd(posSmall, negSmall, "0");
      AssertAdd(posSmall, posLarge, "5555560");
      AssertAdd(posSmall, negLarge, "-5555550");
      AssertAdd(negSmall, negSmall, "-10");
      AssertAdd(negSmall, posLarge, "5555550");
      AssertAdd(negSmall, negLarge, "-5555560");
      AssertAdd(posLarge, posLarge, "11111110");
      AssertAdd(posLarge, negLarge, "0");
      AssertAdd(negLarge, negLarge, "-11111110");
    }

    [TestMethod]
    public void TestBigInteger() {
      var bi = (BigInteger)3;
      AssertBigIntString("3", bi);
      var negseven = (BigInteger)(-7);
      AssertBigIntString("-7", negseven);
      var other = (BigInteger)(-898989);
      AssertBigIntString("-898989", other);
      other = (BigInteger)898989;
      AssertBigIntString("898989", other);
      for (var i = 0; i < 500; ++i) {
        TestCommon.AssertSer(
          CBORObject.FromObject(bi),
          String.Format(CultureInfo.InvariantCulture, "{0}", bi));
        Assert.IsTrue(CBORObject.FromObject(bi).IsIntegral);
        TestCommon.AssertRoundTrip(CBORObject.FromObject(bi));

        TestCommon.AssertRoundTrip(
          CBORObject.FromObject(
            ExtendedDecimal.Create(
bi,
BigInteger.One)));
        bi *= (BigInteger)negseven;
      }
      BigInteger[] ranges = {
        (BigInteger)Int64.MinValue - (BigInteger)512,
        (BigInteger)Int64.MinValue + (BigInteger)512,
        BigInteger.Zero - (BigInteger)512,
        BigInteger.Zero + (BigInteger)512,
        (BigInteger)Int64.MaxValue - (BigInteger)512,
        (BigInteger)Int64.MaxValue + (BigInteger)512,
        ((BigInteger.One << 64) - BigInteger.One) - (BigInteger)512,
        ((BigInteger.One << 64) - BigInteger.One) + (BigInteger)512,
      };
      for (var i = 0; i < ranges.Length; i += 2) {
        BigInteger bigintTemp = ranges[i];
        while (true) {
          TestCommon.AssertSer(
            CBORObject.FromObject(bigintTemp),
            String.Format(CultureInfo.InvariantCulture, "{0}", bigintTemp));
          if (bigintTemp.Equals(ranges[i + 1])) {
            break;
          }
          bigintTemp += BigInteger.One;
        }
      }
    }

    [TestMethod]
    public void TestLong() {
      long[] ranges = {
        -65539, 65539,
        0xFFFFF000L, 0x100000400L,
        Int64.MaxValue - 1000, Int64.MaxValue,
        Int64.MinValue, Int64.MinValue + 1000
      };
      for (var i = 0; i < ranges.Length; i += 2) {
        long j = ranges[i];
        while (true) {
          Assert.IsTrue(CBORObject.FromObject(j).IsIntegral);
          Assert.IsTrue(CBORObject.FromObject(j).CanFitInInt64());
          Assert.IsTrue(CBORObject.FromObject(j).CanTruncatedIntFitInInt64());
          TestCommon.AssertSer(
            CBORObject.FromObject(j),
            String.Format(CultureInfo.InvariantCulture, "{0}", j));
          Assert.AreEqual(
            CBORObject.FromObject(j),
            CBORObject.FromObject((BigInteger)j));
          CBORObject obj = CBORObject.FromJSONString(
            String.Format(CultureInfo.InvariantCulture, "[{0}]", j));
          TestCommon.AssertSer(
            obj,
            String.Format(CultureInfo.InvariantCulture, "[{0}]", j));
          if (j == ranges[i + 1]) {
            break;
          }
          ++j;
        }
      }
    }

    [TestMethod]
    public void TestFloat() {
      TestCommon.AssertSer(
        CBORObject.FromObject(Single.PositiveInfinity),
        "Infinity");
      TestCommon.AssertSer(
        CBORObject.FromObject(Single.NegativeInfinity),
        "-Infinity");
      TestCommon.AssertSer(
        CBORObject.FromObject(Single.NaN),
        "NaN");
      for (int i = -65539; i <= 65539; ++i) {
        TestCommon.AssertSer(
          CBORObject.FromObject((float)i),
          String.Empty + i);
      }
    }

    [TestMethod]
    public void TestSimpleValues() {
      TestCommon.AssertSer(
        CBORObject.FromObject(true),
        "true");
      TestCommon.AssertSer(
        CBORObject.FromObject(false),
        "false");
      TestCommon.AssertSer(
        CBORObject.FromObject((object)null),
        "null");
    }

    [TestMethod]
    public void TestDouble() {
      if
        (!CBORObject.FromObject(Double.PositiveInfinity).IsPositiveInfinity()) {
        Assert.Fail("Not positive infinity");
      }
      TestCommon.AssertSer(
        CBORObject.FromObject(Double.PositiveInfinity),
        "Infinity");
      TestCommon.AssertSer(
        CBORObject.FromObject(Double.NegativeInfinity),
        "-Infinity");
      TestCommon.AssertSer(
        CBORObject.FromObject(Double.NaN),
        "NaN");
      CBORObject oldobj = null;
      for (int i = -65539; i <= 65539; ++i) {
        CBORObject o = CBORObject.FromObject((double)i);
        Assert.IsTrue(o.CanFitInDouble());
        Assert.IsTrue(o.CanFitInInt32());
        Assert.IsTrue(o.IsIntegral);
        TestCommon.AssertSer(
          o,
          String.Empty + i);
        if (oldobj != null) {
          CompareTestLess(oldobj, o);
        }
        oldobj = o;
      }
    }

    [TestMethod]
    public void TestTags() {
      BigInteger maxuint = (BigInteger.One << 64) - BigInteger.One;
      BigInteger[] ranges = {
        (BigInteger)37,
        (BigInteger)65539,
        (BigInteger)Int32.MaxValue - (BigInteger)500,
        (BigInteger)Int32.MaxValue + (BigInteger)500,
        (BigInteger)Int64.MaxValue - (BigInteger)500,
        (BigInteger)Int64.MaxValue + (BigInteger)500,
        ((BigInteger.One << 64) - BigInteger.One) - (BigInteger)500,
        maxuint,
      };
      Assert.IsFalse(CBORObject.True.IsTagged);
      Assert.AreEqual(
        BigInteger.Zero - BigInteger.One,
        CBORObject.True.InnermostTag);
      BigInteger[] tagstmp = CBORObject.True.GetTags();
      Assert.AreEqual(0, tagstmp.Length);
      for (var i = 0; i < ranges.Length; i += 2) {
        BigInteger bigintTemp = ranges[i];
        while (true) {
          if (bigintTemp.CompareTo((BigInteger)(-1)) >= 0 &&
              bigintTemp.CompareTo((BigInteger)37) <= 0) {
            bigintTemp += BigInteger.One;
            continue;
          }
          if (bigintTemp.CompareTo((BigInteger)264) == 0 ||
              bigintTemp.CompareTo((BigInteger)265) == 0) {
            bigintTemp += BigInteger.One;
            continue;
          }
          CBORObject obj = CBORObject.FromObjectAndTag(0, bigintTemp);
          Assert.IsTrue(obj.IsTagged, "obj not tagged");
          BigInteger[] tags = obj.GetTags();
          Assert.AreEqual(1, tags.Length);
          Assert.AreEqual(bigintTemp, tags[0]);
          if (!obj.InnermostTag.Equals(bigintTemp)) {
            Assert.AreEqual(
              bigintTemp,
              obj.InnermostTag,
              String.Format(
                CultureInfo.InvariantCulture,
                "obj tag doesn't match: {0}",
                obj));
          }
          TestCommon.AssertSer(
            obj,
            String.Format(CultureInfo.InvariantCulture, "{0}(0)", bigintTemp));
          if (!bigintTemp.Equals(maxuint)) {
            BigInteger bigintNew = bigintTemp + BigInteger.One;
            if (bigintNew.Equals((BigInteger)264) ||
                bigintNew.Equals((BigInteger)265)) {
              bigintTemp += BigInteger.One;
              continue;
            }
            // Test multiple tags
            CBORObject obj2 = CBORObject.FromObjectAndTag(obj, bigintNew);
            BigInteger[] bi = obj2.GetTags();
            if (bi.Length != 2) {
              Assert.AreEqual(
                2,
                bi.Length,
                String.Format(
                  CultureInfo.InvariantCulture,
                  "Expected 2 tags: {0}",
                  obj2));
            }
            if (!bi[0].Equals((BigInteger)bigintTemp + BigInteger.One)) {
              Assert.AreEqual(
                bigintTemp + BigInteger.One,
                bi[0],
                String.Format(
                  CultureInfo.InvariantCulture,
                  "Outer tag doesn't match: {0}",
                  obj2));
            }
            if (!bi[1].Equals((BigInteger)bigintTemp)) {
              Assert.AreEqual(
                bigintTemp,
                bi[1],
                String.Format(
                  CultureInfo.InvariantCulture,
                  "Inner tag doesn't match: {0}",
                  obj2));
            }
            if (!obj2.InnermostTag.Equals((BigInteger)bigintTemp)) {
              Assert.AreEqual(
                bigintTemp,
                obj2.InnermostTag,
                String.Format(
                  CultureInfo.InvariantCulture,
                  "Innermost tag doesn't match: {0}",
                  obj2));
            }
            String str = String.Format(
              CultureInfo.InvariantCulture,
              "{0}({1}(0))",
              bigintTemp + BigInteger.One,
              bigintTemp);
            TestCommon.AssertSer(
              obj2,
              str);
          }
          if (bigintTemp.Equals(ranges[i + 1])) {
            break;
          }
          bigintTemp += BigInteger.One;
        }
      }
    }
  }
}
