/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;
using System.Globalization;
using System.IO;
using System.Text;
using NUnit.Framework;
using PeterO;

namespace Test {
    /// <summary>Contains CBOR tests.</summary>
    /// <returns/><param name='r'> A FastRandom object.</param>
  [TestFixture]
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
      byte[] bytes = new byte[x];
      for (int i = 0; i < x; ++i) {
        bytes[i] = unchecked((byte)rand.NextValue(256));
      }
      return CBORObject.FromObject(bytes);
    }

    private static CBORObject RandomCBORTextString(FastRandom rand) {
      int length = rand.NextValue(0x2000);
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < length; ++i) {
        int x = rand.NextValue(100);
        if (x < 95) {
          // ASCII
          sb.Append((char)(0x20 + rand.NextValue(0x60)));
        } else if (x < 98) {
          // Supplementary character
          x = rand.NextValue(0x400) + 0xD800;
          sb.Append((char)x);
          x = rand.NextValue(0x400) + 0xDC00;
          sb.Append((char)x);
        } else {
          // BMP character
          x = 0x20 + rand.NextValue(0xFFE0);
          if (x >= 0xD800 && x < 0xE000) {
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
      if (x < 80) {
        count = 2;
      } else if (x < 93) {
        count = 1;
      } else if (x < 98) {
        count = 0;
      } else {
        count = 10;
      }
      CBORObject cborRet = CBORObject.NewMap();
      for (int i = 0; i < count; ++i) {
        CBORObject key = RandomCBORObject(rand, depth + 1);
        CBORObject value = RandomCBORObject(rand, depth + 1);
        cborRet[key] = value;
      }
      return cborRet;
    }

    private static CBORObject RandomCBORTaggedObject(FastRandom rand, int depth) {
      int tag = rand.NextValue(0x1000000);
      if (tag == 2 || tag == 3 || tag == 4 || tag == 5 || tag == 30) {
        tag = 0;
      }
      return CBORObject.FromObjectAndTag(RandomCBORObject(rand, depth + 1), tag);
    }

    private static CBORObject RandomCBORArray(FastRandom rand, int depth) {
      int x = rand.NextValue(100);
      int count = 0;
      if (x < 80) {
        count = 2;
      } else if (x < 93) {
        count = 1;
      } else if (x < 98) {
        count = 0;
      } else {
        count = 10;
      }
      CBORObject cborRet = CBORObject.NewArray();
      for (int i = 0; i < count; ++i) {
        cborRet.Add(RandomCBORObject(rand, depth + 1));
      }
      return cborRet;
    }

    private static ExtendedRational RandomRational(FastRandom rand) {
      BigInteger bigintA = RandomBigInteger(rand);
      BigInteger bigintB = RandomBigInteger(rand);
      if (bigintB.IsZero) {
        bigintB = BigInteger.One;
      }
      return new ExtendedRational(bigintA, bigintB);
    }

    private static CBORObject RandomCBORObject(FastRandom rand) {
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
          return rand.NextValue(2) == 0 ? CBORObject.Null : CBORObject.Undefined;
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
      r &= ~0x7F800000;  // clear exponent
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
      byte[] bytes = new byte[count];
      for (int i = 0; i < count; ++i) {
        bytes[i] = (byte)((int)r.NextValue(256));
      }
      return new BigInteger((byte[])bytes);
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
      return ExtendedFloat.Create(RandomBigInteger(r), (BigInteger)(r.NextValue(400) - 200));
    }

    public static String RandomBigIntString(FastRandom r) {
      int count = r.NextValue(50) + 1;
      StringBuilder sb = new StringBuilder();
      if (r.NextValue(2) == 0) {
        sb.Append('-');
      }
      for (int i = 0; i < count; ++i) {
        if (i == 0) {
          sb.Append((char)('1' + r.NextValue(9)));
        } else {
          sb.Append((char)('0' + r.NextValue(10)));
        }
      }
      return sb.ToString();
    }

    public static String RandomDecimalString(FastRandom r) {
      int count = r.NextValue(20) + 1;
      StringBuilder sb = new StringBuilder();
      if (r.NextValue(2) == 0) {
        sb.Append('-');
      }
      for (int i = 0; i < count; ++i) {
        if (i == 0) {
          sb.Append((char)('1' + r.NextValue(9)));
        } else {
          sb.Append((char)('0' + r.NextValue(10)));
        }
      }
      if (r.NextValue(2) == 0) {
        sb.Append('.');
        count = r.NextValue(20) + 1;
        for (int i = 0; i < count; ++i) {
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

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestAdd() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 3000; ++i) {
        CBORObject o1 = RandomNumber(r);
        CBORObject o2 = RandomNumber(r);
        ExtendedDecimal cmpDecFrac = o1.AsExtendedDecimal().Add(o2.AsExtendedDecimal());
        ExtendedDecimal cmpCobj = CBORObject.Addition(o1, o2).AsExtendedDecimal();
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

    [Test]
    public void TestMultiply() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 3000; ++i) {
        CBORObject o1 = RandomNumber(r);
        CBORObject o2 = RandomNumber(r);
        ExtendedDecimal cmpDecFrac = o1.AsExtendedDecimal().Multiply(o2.AsExtendedDecimal());
        ExtendedDecimal cmpCobj = CBORObject.Multiply(o1, o2).AsExtendedDecimal();
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

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestSubtract() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 3000; ++i) {
        CBORObject o1 = RandomNumber(r);
        CBORObject o2 = RandomNumber(r);
        ExtendedDecimal cmpDecFrac = o1.AsExtendedDecimal().Subtract(o2.AsExtendedDecimal());
        ExtendedDecimal cmpCobj = CBORObject.Subtract(o1, o2).AsExtendedDecimal();
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

    private static string ObjectMessages(CBORObject o1, CBORObject o2, String s) {
      if (o1.Type == CBORType.Number && o2.Type == CBORType.Number) {
        return s + ":\n" + o1.ToString() + " and\n" + o2.ToString() + "\nOR\n" +
          o1.AsExtendedDecimal().ToString() + " and\n" + o2.AsExtendedDecimal().ToString() + "\nOR\n" +
          "AddSubCompare(" + ToByteArrayString(o1) + ",\n" + ToByteArrayString(o2) + ");";
      } else {
        return s + ":\n" + o1.ToString() + " and\n" + o2.ToString() + "\nOR\n" +
          ToByteArrayString(o1) + " and\n" + ToByteArrayString(o2);
      }
    }

    private static void CompareTestEqual(CBORObject o1, CBORObject o2) {
      if (CompareTestReciprocal(o1, o2) != 0) {
        Assert.Fail(ObjectMessages(o1, o2, "Not equal: " + CompareTestReciprocal(o1, o2)));
      }
    }

    private static void CompareTestLess(CBORObject o1, CBORObject o2) {
      if (CompareTestReciprocal(o1, o2) >= 0) {
        Assert.Fail(ObjectMessages(o1, o2, "Not less: " + CompareTestReciprocal(o1, o2)));
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

    public static string ToByteArrayString(CBORObject obj) {
      byte[] bytes = obj.EncodeToBytes();
      StringBuilder sb = new StringBuilder();
      string hex = "0123456789ABCDEF";
      sb.Append("CBORObject.DecodeFromBytes(new byte[] { ");
      for (int i = 0; i < bytes.Length; ++i) {
        if (i > 0) {
          sb.Append(",");
        }
        if ((bytes[i] & 0x80) != 0) {
          sb.Append("(byte)0x");
        } else {
          sb.Append("0x");
        }
        sb.Append(hex[(bytes[i] >> 4) & 0xF]);
        sb.Append(hex[bytes[i] & 0xF]);
      }
      sb.Append("})");
      return sb.ToString();
    }

    [Test]
    public void TestDecFracCompareIntegerVsBigFraction() {
      ExtendedDecimal a = ExtendedDecimal.FromString("7.004689238424764477580371752455515117709288087566222056632084784688080253355047487262563521426272927783429622650146484375");
      ExtendedDecimal b = ExtendedDecimal.FromString("5");
      Assert.AreEqual(1, a.CompareTo(b));
      Assert.AreEqual(-1, b.CompareTo(a));
      CBORObject o1 = null;
      CBORObject o2 = null;
      o1 = CBORObject.DecodeFromBytes(new byte[] { (byte)0xFB, (byte)0x8B, 0x44, (byte)0xF2, (byte)0xA9, 0x0C, 0x27, 0x42, 0x28 });
      o2 = CBORObject.DecodeFromBytes(new byte[] { (byte)0xC5, (byte)0x82, 0x38, (byte)0xA4, (byte)0xC3, 0x50, 0x02, (byte)0x98,
                                        (byte)0xC5, (byte)0xA8, 0x02, (byte)0xC1, (byte)0xF6, (byte)0xC0, 0x1A, (byte)0xBE, 0x08,
                                        0x04, (byte)0x86, (byte)0x99, 0x3E, (byte)0xF1 });
      AddSubCompare(o1, o2);
    }

    private static void CompareDecimals(CBORObject o1, CBORObject o2) {
      int cmpDecFrac = o1.AsExtendedDecimal().CompareTo(o2.AsExtendedDecimal());
      int cmpCobj = CompareTestReciprocal(o1, o2);
      if (cmpDecFrac != cmpCobj) {
        Assert.AreEqual(cmpDecFrac, cmpCobj, ObjectMessages(o1, o2, "Compare: Results don't match"));
      }
      TestCommon.AssertRoundTrip(o1);
      TestCommon.AssertRoundTrip(o2);
    }

    private static void AddSubCompare(CBORObject o1, CBORObject o2) {
      ExtendedDecimal cmpDecFrac = o1.AsExtendedDecimal().Add(o2.AsExtendedDecimal());
      ExtendedDecimal cmpCobj = CBORObject.Addition(o1, o2).AsExtendedDecimal();
      if (cmpDecFrac.CompareTo(cmpCobj) != 0) {
        Assert.AreEqual(
          0,
          cmpDecFrac.CompareTo(cmpCobj),
          ObjectMessages(o1, o2, "Add: Results don't match:\n" + cmpDecFrac + " vs\n" + cmpCobj));
      }
      cmpDecFrac = o1.AsExtendedDecimal().Subtract(o2.AsExtendedDecimal());
      cmpCobj = CBORObject.Subtract(o1, o2).AsExtendedDecimal();
      if (cmpDecFrac.CompareTo(cmpCobj) != 0) {
        Assert.AreEqual(
          0,
          cmpDecFrac.CompareTo(cmpCobj),
          ObjectMessages(o1, o2, "Subtract: Results don't match:\n" + cmpDecFrac + " vs\n" + cmpCobj));
      }
      CompareDecimals(o1, o2);
    }

    [Test]
    public void TestCompareB() {
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xD8, 0x1E, (byte)0x82, (byte)0xC2, 0x58, 0x28, 0x77, 0x24, 0x73, (byte)0x84, (byte)0xBD, 0x72, (byte)0x82, 0x7C, (byte)0xD6, (byte)0x93, 0x18, 0x44, (byte)0x8A, (byte)0x88, 0x43, 0x67, (byte)0xA2, (byte)0xEB, 0x11, 0x00, 0x15, 0x1B, 0x1D, 0x5D, (byte)0xDC, (byte)0xEB, 0x39, 0x17, 0x72, 0x11, 0x5B, 0x03, (byte)0xFA, (byte)0xA8, 0x3F, (byte)0xD2, 0x75, (byte)0xF8, 0x36, (byte)0xC8, 0x1A, 0x00, 0x2E, (byte)0x8C, (byte)0x8D}),
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xFA, 0x7F, (byte)0x80, 0x00, 0x00}));
      CompareTestLess(
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xD8, 0x1E, (byte)0x82, (byte)0xC2, 0x58, 0x28, 0x77, 0x24, 0x73, (byte)0x84, (byte)0xBD, 0x72, (byte)0x82, 0x7C, (byte)0xD6, (byte)0x93, 0x18, 0x44, (byte)0x8A, (byte)0x88, 0x43, 0x67, (byte)0xA2, (byte)0xEB, 0x11, 0x00, 0x15, 0x1B, 0x1D, 0x5D, (byte)0xDC, (byte)0xEB, 0x39, 0x17, 0x72, 0x11, 0x5B, 0x03, (byte)0xFA, (byte)0xA8, 0x3F, (byte)0xD2, 0x75, (byte)0xF8, 0x36, (byte)0xC8, 0x1A, 0x00, 0x2E, (byte)0x8C, (byte)0x8D}),
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xFA, 0x7F, (byte)0x80, 0x00, 0x00}));
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xFB, 0x7F, (byte)0xF8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xFA, 0x7F, (byte)0x80, 0x00, 0x00}));
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] { 0x1A, (byte)0xFC, 0x1A, (byte)0xB0, 0x52 }),
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xC5, (byte)0x82, 0x38, 0x5F, (byte)0xC2, 0x50, 0x08, 0x70, (byte)0xF3, (byte)0xC4, (byte)0x90, 0x4C, 0x14, (byte)0xBA, 0x59, (byte)0xF0, (byte)0xC6, (byte)0xCB, (byte)0x8C, (byte)0x8D, 0x40, (byte)0x80 }));
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xC5, (byte)0x82, 0x38, (byte)0xC7, 0x3B, 0x00, 0x00, 0x08, (byte)0xBF, (byte)0xDA, (byte)0xAF, 0x73, 0x46 }),
        CBORObject.DecodeFromBytes(new byte[] { 0x3B, 0x5A, (byte)0x9B, (byte)0x9A, (byte)0x9C, (byte)0xB4, (byte)0x95, (byte)0xBF, 0x71 }));
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] { 0x1A, (byte)0xBB, 0x0C, (byte)0xF7, 0x52 }),
        CBORObject.DecodeFromBytes(new byte[] { 0x1A, (byte)0x82, 0x00, (byte)0xBF, (byte)0xF9 }));
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xFA, 0x1F, (byte)0x80, (byte)0xDB, (byte)0x9B }),
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xFB, 0x31, (byte)0x90, (byte)0xEA, 0x16, (byte)0xBE, (byte)0x80, 0x0B, 0x37 }));
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xFB, 0x3C, 0x00, (byte)0xCF, (byte)0xB6, (byte)0xBD, (byte)0xFF, 0x37, 0x38 }),
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xFA, 0x30, (byte)0x80, 0x75, 0x63 }));
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xC5, (byte)0x82, 0x38, 0x7D, 0x3A, 0x06, (byte)0xBC, (byte)0xD5, (byte)0xB8 }),
        CBORObject.DecodeFromBytes(new byte[] { 0x38, 0x5C }));
    }

    [Test]
    public void ExtraDecimalTests() {
      Assert.AreEqual(
        ExtendedDecimal.NegativeInfinity,
        ExtendedDecimal.FromString("-79228162514264337593543950336").RoundToBinaryPrecision(PrecisionContext.CliDecimal));
      Assert.AreEqual(
        ExtendedDecimal.PositiveInfinity,
        ExtendedDecimal.FromString("8.782580686213340724E+28").RoundToBinaryPrecision(PrecisionContext.CliDecimal));
      Assert.AreEqual(
        ExtendedDecimal.NegativeInfinity,
        ExtendedDecimal.FromString("-9.3168444507547E+28").RoundToBinaryPrecision(PrecisionContext.CliDecimal));
      Assert.AreEqual(
        "-9344285899206687626894794544",
        ExtendedDecimal.FromString("-9344285899206687626894794544.04982268810272216796875").RoundToBinaryPrecision(new PrecisionContext(96, Rounding.HalfEven, 0, 28, false)).ToPlainString());
      Assert.AreEqual(
        ExtendedDecimal.PositiveInfinity,
        ExtendedDecimal.FromString("96148154858060747311034406200").RoundToBinaryPrecision(PrecisionContext.CliDecimal));
      Assert.AreEqual(
        ExtendedDecimal.PositiveInfinity,
        ExtendedDecimal.FromString("90246605365627217170000000000").RoundToBinaryPrecision(PrecisionContext.CliDecimal));
    }

    [Test]
    public void TestFloatDecimalRoundTrip() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 5000; ++i) {
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

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestCompare() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 5000; ++i) {
        CBORObject o1 = RandomNumber(r);
        CBORObject o2 = RandomNumber(r);
        CompareDecimals(o1, o2);
      }
      for (int i = 0; i < 500; ++i) {
        CBORObject o1 = RandomCBORObject(r);
        CBORObject o2 = RandomCBORObject(r);
        CompareTestReciprocal(o1, o2);
      }
      for (int i = 0; i < 50; ++i) {
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
      CBORObject[] sortedObjects = new CBORObject[] {
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
        CBORObject.FromObject(new byte[] { 0, 1 }),
        CBORObject.FromObject(new byte[] { 0, 2 }),
        CBORObject.FromObject(new byte[] { 0, 2, 0 }),
        CBORObject.FromObject(new byte[] { 1, 1 }),
        CBORObject.FromObject(new byte[] { 1, 1, 4 }),
        CBORObject.FromObject(new byte[] { 1, 2 }),
        CBORObject.FromObject(new byte[] { 1, 2, 6 }),
        CBORObject.FromObject("aa"),
        CBORObject.FromObject("ab"),
        CBORObject.FromObject("abc"),
        CBORObject.FromObject("ba"),
        CBORObject.FromObject(CBORObject.NewArray()),
        CBORObject.FromObject(CBORObject.NewMap()),
      };
      for (int i = 0; i < sortedObjects.Length; ++i) {
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

    [Test]
    public void TestParseJSONNumber() {
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber(null, false, false, false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber("1e+99999999999999999999999999", false, false, true));
      if (CBORDataUtilities.ParseJSONNumber("1e+99999999999999999999999999", false, false, false) == null) {
        Assert.Fail();
      }
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber(String.Empty, false, false, false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber("xyz", false, false, false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber("0..1", false, false, false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber("0xyz", false, false, false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber("0.1xyz", false, false, false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber("0.xyz", false, false, false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber("0.5exyz", false, false, false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber("0.5q+88", false, false, false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber("0.5ee88", false, false, false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber("0.5e+xyz", false, false, false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber("0.5e+88xyz", false, false, false));
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestParseDecimalStrings() {
      FastRandom rand = new FastRandom();
      for (int i = 0; i < 3000; ++i) {
        string r = RandomDecimalString(rand);
        TestDecimalString(r);
      }
    }

    [Test]
    public void TestExtendedInfinity() {
      Assert.IsTrue(ExtendedDecimal.PositiveInfinity.IsInfinity());
      Assert.IsTrue(ExtendedDecimal.PositiveInfinity.IsPositiveInfinity());
      Assert.IsFalse(ExtendedDecimal.PositiveInfinity.IsNegativeInfinity());
      Assert.IsFalse(ExtendedDecimal.PositiveInfinity.IsNegative);
      Assert.IsTrue(ExtendedDecimal.NegativeInfinity.IsInfinity());
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
    }

    [Test]
    public void TestJSONBase64() {
      CBORObject o;
      o = CBORObject.FromObjectAndTag(new byte[] { 0x9a, 0xd6, 0xf0, 0xe8 }, 22);
      Assert.AreEqual("\"mtbw6A\"", o.ToJSONString());
      o = CBORObject.FromObject(new byte[] { 0x9a, 0xd6, 0xf0, 0xe8 });
      Assert.AreEqual("\"mtbw6A\"", o.ToJSONString());
      o = CBORObject.FromObjectAndTag(new byte[] { 0x9a, 0xd6, 0xf0, 0xe8 }, 23);
      Assert.AreEqual("\"9AD6F0E8\"", o.ToJSONString());
      o = CBORObject.FromObject(new byte[] { 0x9a, 0xd6, 0xff, 0xe8 });
      Assert.AreEqual("\"mtb_6A\"", o.ToJSONString());  // Encode with Base64URL by default
      o = CBORObject.FromObjectAndTag(new byte[] { 0x9a, 0xd6, 0xff, 0xe8 }, 22);
      Assert.AreEqual("\"mtb/6A\"", o.ToJSONString());  // Encode with Base64
    }

    [Test]
    public void TestCBORExceptions() {
      try {
        CBORObject.NewArray().AsExtendedDecimal();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsExtendedDecimal();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { });
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0x1C });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
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
        CBORObject.True.AsExtendedDecimal();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsExtendedDecimal();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsExtendedDecimal();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(String.Empty).AsExtendedDecimal();
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

    [Test]
    public void TestCBORInfinity() {
      TestCommon.AssertRoundTrip(CBORObject.FromObject(ExtendedDecimal.NegativeInfinity));
      TestCommon.AssertRoundTrip(CBORObject.FromObject(ExtendedFloat.NegativeInfinity));
      TestCommon.AssertRoundTrip(CBORObject.FromObject(Double.NegativeInfinity));
      TestCommon.AssertRoundTrip(CBORObject.FromObject(Single.NegativeInfinity));
      TestCommon.AssertRoundTrip(CBORObject.FromObject(ExtendedDecimal.PositiveInfinity));
      TestCommon.AssertRoundTrip(CBORObject.FromObject(ExtendedFloat.PositiveInfinity));
      TestCommon.AssertRoundTrip(CBORObject.FromObject(Double.PositiveInfinity));
      TestCommon.AssertRoundTrip(CBORObject.FromObject(Single.PositiveInfinity));
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestRandomData() {
      FastRandom rand = new FastRandom();
      String badString = null;
      CBORObject badobj = null;
      for (int i = 0; i < 2000; ++i) {
        CBORObject obj = RandomCBORObject(rand);
        TestCommon.AssertRoundTrip(obj);
      }
      if (badobj != null) {
        Console.WriteLine(badString);
        Console.WriteLine(ToByteArrayString(badobj));
      }
      // Test slightly modified objects
      for (int i = 0; i < 200; ++i) {
        byte[] array = RandomCBORObject(rand).EncodeToBytes();
        int count = rand.NextValue(10) + 1;
        for (int j = 0; j < count; ++j) {
          int index = rand.NextValue(array.Length);
          array[index] = unchecked((byte)rand.NextValue(256));
        }
        using (MemoryStream ms = new MemoryStream(array)) {
          while (ms.Position != ms.Length) {
            try {
              CBORObject o = CBORObject.Read(ms);
              if (o == null) {
                Assert.Fail("object read is null");
              }
              byte[] encodedBytes = o.EncodeToBytes();
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
                }
              } catch (Exception ex) {
                Assert.Fail(jsonString + "\n" + ex.ToString());
                throw new InvalidOperationException(String.Empty, ex);
              }
            } catch (CBORException) {
              // Expected exception
            }
          }
        }
      }
      // Test random nonsense data
      for (int i = 0; i < 200; ++i) {
        byte[] array = new byte[rand.NextValue(1000000) + 1];
        for (int j = 0; j < array.Length; ++j) {
          if (j + 3 <= array.Length) {
            int r = rand.NextValue(0x1000000);
            array[j] = (byte)(r & 0xFF);
            array[j + 1] = (byte)((r >> 8) & 0xFF);
            array[j + 2] = (byte)((r >> 16) & 0xFF);
            j += 2;
          } else {
            array[j] = (byte)rand.NextValue(256);
          }
        }
        using (MemoryStream ms = new MemoryStream(array)) {
          while (ms.Position != ms.Length) {
            try {
              CBORObject o = CBORObject.Read(ms);
              if (o == null) {
                Assert.Fail("object read is null");
              }
              try {
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
                }
              } catch (Exception ex) {
                Assert.Fail(jsonString + "\n" + ex.ToString());
                throw new InvalidOperationException(String.Empty, ex);
              }
            } catch (CBORException) {
              // Expected exception
            }
          }
        }
      }
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestExtendedFloatSingle() {
      FastRandom rand = new FastRandom();
      for (int i = 0; i < 255; ++i) {  // Try a random float with a given exponent
        TestExtendedFloatSingleCore(RandomSingle(rand, i), null);
        TestExtendedFloatSingleCore(RandomSingle(rand, i), null);
        TestExtendedFloatSingleCore(RandomSingle(rand, i), null);
        TestExtendedFloatSingleCore(RandomSingle(rand, i), null);
      }
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestExtendedFloatDouble() {
      TestExtendedFloatDoubleCore(3.5, "3.5");
      TestExtendedFloatDoubleCore(7, "7");
      TestExtendedFloatDoubleCore(1.75, "1.75");
      TestExtendedFloatDoubleCore(3.5, "3.5");
      TestExtendedFloatDoubleCore((double)Int32.MinValue, "-2147483648");
      TestExtendedFloatDoubleCore((double)Int64.MinValue, "-9223372036854775808");
      FastRandom rand = new FastRandom();
      for (int i = 0; i < 2047; ++i) {  // Try a random double with a given exponent
        TestExtendedFloatDoubleCore(RandomDouble(rand, i), null);
        TestExtendedFloatDoubleCore(RandomDouble(rand, i), null);
        TestExtendedFloatDoubleCore(RandomDouble(rand, i), null);
        TestExtendedFloatDoubleCore(RandomDouble(rand, i), null);
      }
    }
    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestTagThenBreak() {
      TestCommon.FromBytesTestAB(new byte[] { 0xD1, 0xFF });
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestJSONSurrogates() {
      try {
        CBORObject.FromJSONString("[\"\ud800\udc00\"]");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\\udc00\"]");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[\"\ud800\\udc00\"]");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\udc00\"]");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\udc00\ud800\udc00\"]");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\ud800\udc00\"]");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\\udc00\ud800\udc00\"]");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
    }

    /// <summary>Not documented yet.</summary>
    [Test]
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

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestCBORFromArray() {
      CBORObject o = CBORObject.FromObject(new int[] { 1, 2, 3 });
      Assert.AreEqual(3, o.Count);
      Assert.AreEqual(1, o[0].AsInt32());
      Assert.AreEqual(2, o[1].AsInt32());
      Assert.AreEqual(3, o[2].AsInt32());
      TestCommon.AssertRoundTrip(o);
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestJSON() {
      CBORObject o;
      try {
        CBORObject.FromJSONString("[\"\\ud800\"]"); Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[1,2,"); Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[1,2,3"); Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{,\"0\":0,\"1\":1}");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try { CBORObject.FromJSONString("{\"0\":0,,\"1\":1}"); Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try { CBORObject.FromJSONString("{\"0\":0,\"1\":1,}"); Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[,0,1,2]"); Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[0,,1,2]"); Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[0,1,,2]"); Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[0,1,2,]"); Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[0001]");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{a:true}");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\"://comment\ntrue}");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\":/*comment*/true}");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{'a':true}");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\":'b'}");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\t\":true}");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\r\":true}");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\n\":true}");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("['a']");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\":\"a\t\"}");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[\"a\\'\"]");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[NaN]");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[+Infinity]");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[-Infinity]");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[Infinity]");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\":\"a\r\"}");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\":\"a\n\"}");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[\"a\t\"]");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
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
      Assert.AreEqual("true", CBORObject.True.ToJSONString());
      Assert.AreEqual("false", CBORObject.False.ToJSONString());
      Assert.AreEqual("null", CBORObject.Null.ToJSONString());
    }

    [Test]
    public void TestBoolean() {
      TestCommon.AssertSer(CBORObject.True, "true");
      TestCommon.AssertSer(CBORObject.False, "false");
      Assert.AreEqual(CBORObject.True, CBORObject.FromObject(true));
      Assert.AreEqual(CBORObject.False, CBORObject.FromObject(false));
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestByte() {
      for (int i = 0; i <= 255; ++i) {
        TestCommon.AssertSer(
          CBORObject.FromObject((byte)i),
          String.Format(CultureInfo.InvariantCulture, "{0}", i));
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='bytes'>A byte array.</param>
    /// <param name='expectedRet'>A 32-bit signed integer.</param>
    /// <param name='expectedString'>A string object.</param>
    /// <param name='noReplaceRet'>A 32-bit signed integer. (2).</param>
    /// <param name='noReplaceString'>A string object. (2).</param>
    public void DoTestReadUtf8(
      byte[] bytes,
      int expectedRet,
      string expectedString,
      int noReplaceRet,
      string noReplaceString) {
      this.DoTestReadUtf8(
        bytes,
        bytes.Length,
        expectedRet,
        expectedString,
        noReplaceRet,
        noReplaceString);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='bytes'>A byte array.</param>
    /// <param name='length'>A 32-bit signed integer.</param>
    /// <param name='expectedRet'>A 32-bit signed integer. (2).</param>
    /// <param name='expectedString'>A string object.</param>
    /// <param name='noReplaceRet'>A 32-bit signed integer. (3).</param>
    /// <param name='noReplaceString'>A string object. (2).</param>
    public void DoTestReadUtf8(
      byte[] bytes,
      int length,
      int expectedRet,
      string expectedString,
      int noReplaceRet,
      string noReplaceString) {
      try {
        StringBuilder builder = new StringBuilder();
        int ret = 0;
        using (MemoryStream ms = new MemoryStream(bytes)) {
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
          ret = DataUtilities.ReadUtf8FromBytes(bytes, 0, length, builder, true);
          Assert.AreEqual(expectedRet, ret);
          if (expectedRet == 0) {
            Assert.AreEqual(expectedString, builder.ToString());
          }
          builder.Clear();
          ret = DataUtilities.ReadUtf8FromBytes(bytes, 0, length, builder, false);
          Assert.AreEqual(noReplaceRet, ret);
          if (noReplaceRet == 0) {
            Assert.AreEqual(noReplaceString, builder.ToString());
          }
        }
      } catch (IOException ex) {
        throw new CBORException(String.Empty, ex);
      }
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestDecFracOverflow() {
      Assert.AreEqual(ExtendedDecimal.PositiveInfinity, CBORObject.FromObject(Single.PositiveInfinity).AsExtendedDecimal());
      Assert.AreEqual(ExtendedDecimal.NegativeInfinity, CBORObject.FromObject(Single.NegativeInfinity).AsExtendedDecimal());
      Assert.IsTrue(CBORObject.FromObject(Single.NaN).AsExtendedDecimal().IsNaN());
      Assert.AreEqual(ExtendedDecimal.PositiveInfinity, CBORObject.FromObject(Double.PositiveInfinity).AsExtendedDecimal());
      Assert.AreEqual(ExtendedDecimal.NegativeInfinity, CBORObject.FromObject(Double.NegativeInfinity).AsExtendedDecimal());
      Assert.IsTrue(CBORObject.FromObject(Double.NaN).AsExtendedDecimal().IsNaN());
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestFPToBigInteger() {
      Assert.AreEqual("0", CBORObject.FromObject((float)0.75).AsBigInteger().ToString());
      Assert.AreEqual("0", CBORObject.FromObject((float)0.99).AsBigInteger().ToString());
      Assert.AreEqual("0", CBORObject.FromObject((float)0.0000000000000001).AsBigInteger().ToString());
      Assert.AreEqual("0", CBORObject.FromObject((float)0.5).AsBigInteger().ToString());
      Assert.AreEqual("1", CBORObject.FromObject((float)1.5).AsBigInteger().ToString());
      Assert.AreEqual("2", CBORObject.FromObject((float)2.5).AsBigInteger().ToString());
      Assert.AreEqual("328323", CBORObject.FromObject((float)328323f).AsBigInteger().ToString());
      Assert.AreEqual("0", CBORObject.FromObject((double)0.75).AsBigInteger().ToString());
      Assert.AreEqual("0", CBORObject.FromObject((double)0.99).AsBigInteger().ToString());
      Assert.AreEqual("0", CBORObject.FromObject((double)0.0000000000000001).AsBigInteger().ToString());
      Assert.AreEqual("0", CBORObject.FromObject((double)0.5).AsBigInteger().ToString());
      Assert.AreEqual("1", CBORObject.FromObject((double)1.5).AsBigInteger().ToString());
      Assert.AreEqual("2", CBORObject.FromObject((double)2.5).AsBigInteger().ToString());
      Assert.AreEqual("328323", CBORObject.FromObject((double)328323).AsBigInteger().ToString());
      try {
        CBORObject.FromObject(Single.PositiveInfinity).AsBigInteger(); Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(Single.NegativeInfinity).AsBigInteger(); Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(Single.NaN).AsBigInteger(); Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(Double.PositiveInfinity).AsBigInteger(); Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(Double.NegativeInfinity).AsBigInteger(); Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(Double.NaN).AsBigInteger(); Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestDecFracFP() {
      Assert.AreEqual("0.75", ExtendedDecimal.FromDouble(0.75).ToString());
      Assert.AreEqual("0.5", ExtendedDecimal.FromDouble(0.5).ToString());
      Assert.AreEqual("0.25", ExtendedDecimal.FromDouble(0.25).ToString());
      Assert.AreEqual("0.875", ExtendedDecimal.FromDouble(0.875).ToString());
      Assert.AreEqual("0.125", ExtendedDecimal.FromDouble(0.125).ToString());
      Assert.AreEqual("0.75", ExtendedDecimal.FromSingle(0.75f).ToString());
      Assert.AreEqual("0.5", ExtendedDecimal.FromSingle(0.5f).ToString());
      Assert.AreEqual("0.25", ExtendedDecimal.FromSingle(0.25f).ToString());
      Assert.AreEqual("0.875", ExtendedDecimal.FromSingle(0.875f).ToString());
      Assert.AreEqual("0.125", ExtendedDecimal.FromSingle(0.125f).ToString());
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void ScaleTest() {
      Assert.AreEqual(-(BigInteger)7, ExtendedDecimal.FromString("1.265e-4").Exponent);
      Assert.AreEqual(-(BigInteger)4, ExtendedDecimal.FromString("0.000E-1").Exponent);
      Assert.AreEqual(-(BigInteger)16, ExtendedDecimal.FromString("0.57484848535648e-2").Exponent);
      Assert.AreEqual(-(BigInteger)22, ExtendedDecimal.FromString("0.485448e-16").Exponent);
      Assert.AreEqual(-(BigInteger)20, ExtendedDecimal.FromString("0.5657575351495151495649565150e+8").Exponent);
      Assert.AreEqual(-(BigInteger)10, ExtendedDecimal.FromString("0e-10").Exponent);
      Assert.AreEqual(-(BigInteger)17, ExtendedDecimal.FromString("0.504952e-11").Exponent);
      Assert.AreEqual(-(BigInteger)13, ExtendedDecimal.FromString("0e-13").Exponent);
      Assert.AreEqual(-(BigInteger)43, ExtendedDecimal.FromString("0.49495052535648555757515648e-17").Exponent);
      Assert.AreEqual((BigInteger)7, ExtendedDecimal.FromString("0.485654575150e+19").Exponent);
      Assert.AreEqual(BigInteger.Zero, ExtendedDecimal.FromString("0.48515648e+8").Exponent);
      Assert.AreEqual(-(BigInteger)45, ExtendedDecimal.FromString("0.49485251485649535552535451544956e-13").Exponent);
      Assert.AreEqual(-(BigInteger)6, ExtendedDecimal.FromString("0.565754515152575448505257e+18").Exponent);
      Assert.AreEqual((BigInteger)16, ExtendedDecimal.FromString("0e+16").Exponent);
      Assert.AreEqual((BigInteger)6, ExtendedDecimal.FromString("0.5650e+10").Exponent);
      Assert.AreEqual(-(BigInteger)5, ExtendedDecimal.FromString("0.49555554575756575556e+15").Exponent);
      Assert.AreEqual(-(BigInteger)37, ExtendedDecimal.FromString("0.57494855545057534955e-17").Exponent);
      Assert.AreEqual(-(BigInteger)25, ExtendedDecimal.FromString("0.4956504855525748575456e-3").Exponent);
      Assert.AreEqual(-(BigInteger)26, ExtendedDecimal.FromString("0.55575355495654484948525354545053494854e+12").Exponent);
      Assert.AreEqual(-(BigInteger)22, ExtendedDecimal.FromString("0.484853575350494950575749545057e+8").Exponent);
      Assert.AreEqual((BigInteger)11, ExtendedDecimal.FromString("0.52545451e+19").Exponent);
      Assert.AreEqual(-(BigInteger)29, ExtendedDecimal.FromString("0.48485654495751485754e-9").Exponent);
      Assert.AreEqual(-(BigInteger)38, ExtendedDecimal.FromString("0.56525456555549545257535556495655574848e+0").Exponent);
      Assert.AreEqual(-(BigInteger)15, ExtendedDecimal.FromString("0.485456485657545752495450554857e+15").Exponent);
      Assert.AreEqual(-(BigInteger)37, ExtendedDecimal.FromString("0.485448525554495048e-19").Exponent);
      Assert.AreEqual(-(BigInteger)29, ExtendedDecimal.FromString("0.494952485550514953565655e-5").Exponent);
      Assert.AreEqual(-(BigInteger)8, ExtendedDecimal.FromString("0.50495454554854505051534950e+18").Exponent);
      Assert.AreEqual(-(BigInteger)37, ExtendedDecimal.FromString("0.5156524853575655535351554949525449e-3").Exponent);
      Assert.AreEqual((BigInteger)3, ExtendedDecimal.FromString("0e+3").Exponent);
      Assert.AreEqual(-(BigInteger)8, ExtendedDecimal.FromString("0.51505056554957575255555250e+18").Exponent);
      Assert.AreEqual(-(BigInteger)14, ExtendedDecimal.FromString("0.5456e-10").Exponent);
      Assert.AreEqual(-(BigInteger)36, ExtendedDecimal.FromString("0.494850515656505252555154e-12").Exponent);
      Assert.AreEqual(-(BigInteger)42, ExtendedDecimal.FromString("0.535155525253485757525253555749575749e-6").Exponent);
      Assert.AreEqual(-(BigInteger)29, ExtendedDecimal.FromString("0.56554952554850525552515549564948e+3").Exponent);
      Assert.AreEqual(-(BigInteger)40, ExtendedDecimal.FromString("0.494855545257545656515554495057e-10").Exponent);
      Assert.AreEqual(-(BigInteger)18, ExtendedDecimal.FromString("0.5656504948515252555456e+4").Exponent);
      Assert.AreEqual(-(BigInteger)17, ExtendedDecimal.FromString("0e-17").Exponent);
      Assert.AreEqual(-(BigInteger)32, ExtendedDecimal.FromString("0.55535551515249535049495256e-6").Exponent);
      Assert.AreEqual(-(BigInteger)31, ExtendedDecimal.FromString("0.4948534853564853565654514855e-3").Exponent);
      Assert.AreEqual(-(BigInteger)38, ExtendedDecimal.FromString("0.5048485057535249555455e-16").Exponent);
      Assert.AreEqual(-(BigInteger)16, ExtendedDecimal.FromString("0e-16").Exponent);
      Assert.AreEqual((BigInteger)5, ExtendedDecimal.FromString("0.5354e+9").Exponent);
      Assert.AreEqual(BigInteger.One, ExtendedDecimal.FromString("0.54e+3").Exponent);
      Assert.AreEqual(-(BigInteger)38, ExtendedDecimal.FromString("0.4849525755545751574853494948e-10").Exponent);
      Assert.AreEqual(-(BigInteger)33, ExtendedDecimal.FromString("0.52514853565252565251565548e-7").Exponent);
      Assert.AreEqual(-(BigInteger)13, ExtendedDecimal.FromString("0.575151545652e-1").Exponent);
      Assert.AreEqual(-(BigInteger)22, ExtendedDecimal.FromString("0.49515354514852e-8").Exponent);
      Assert.AreEqual(-(BigInteger)24, ExtendedDecimal.FromString("0.54535357515356545554e-4").Exponent);
      Assert.AreEqual(-(BigInteger)11, ExtendedDecimal.FromString("0.574848e-5").Exponent);
      Assert.AreEqual(-(BigInteger)3, ExtendedDecimal.FromString("0.565055e+3").Exponent);
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestReadUtf8() {
      this.DoTestReadUtf8(
        new byte[] { 0x21,
        0x21,
        0x21 },
        0,
        "!!!",
        0,
        "!!!");
      this.DoTestReadUtf8(
        new byte[] { 0x20,
        0xc2,
        0x80 },
        0,
        " \u0080",
        0,
        " \u0080");
      this.DoTestReadUtf8(
        new byte[] { 0x20,
        0xc2,
        0x80,
        0x20 },
        0,
        " \u0080 ",
        0,
        " \u0080 ");
      this.DoTestReadUtf8(
        new byte[] { 0x20,
        0xc2,
        0x80,
        0xc2 },
        0,
        " \u0080\ufffd",
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] { 0x20,
        0xc2,
        0x21,
        0x21 },
        0,
        " \ufffd!!",
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] { 0x20,
        0xc2,
        0xff,
        0x20 },
        0,
        " \ufffd\ufffd ",
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] { 0x20,
        0xe0,
        0xa0,
        0x80 },
        0,
        " \u0800",
        0,
        " \u0800");
      this.DoTestReadUtf8(
        new byte[] { 0x20,
        0xe0,
        0xa0,
        0x80,
        0x20 },
        0,
        " \u0800 ",
        0,
        " \u0800 ");
      this.DoTestReadUtf8(
        new byte[] { 0x20,
        0xf0,
        0x90,
        0x80,
        0x80 },
        0,
        " \ud800\udc00",
        0,
        " \ud800\udc00");
      this.DoTestReadUtf8(
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
      this.DoTestReadUtf8(
        new byte[] { 0x20,
        0xf0,
        0x90 },
        5,
        -2,
        null,
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] { 0x20,
        0x20,
        0x20 },
        5,
        -2,
        null,
        -2,
        null);
      this.DoTestReadUtf8(
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
      this.DoTestReadUtf8(
        new byte[] { 0x20,
        0xf0,
        0x90,
        0x80,
        0x20 },
        0,
        " \ufffd ",
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] { 0x20,
        0xf0,
        0x90,
        0x20 },
        0,
        " \ufffd ",
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] { 0x20,
        0xf0,
        0x90,
        0x80,
        0xff },
        0,
        " \ufffd\ufffd",
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] { 0x20,
        0xf0,
        0x90,
        0xff },
        0,
        " \ufffd\ufffd",
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] { 0x20,
        0xe0,
        0xa0,
        0x20 },
        0,
        " \ufffd ",
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] { 0x20,
        0xe0,
        0x20 },
        0,
        " \ufffd ",
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] { 0x20,
        0xe0,
        0xa0,
        0xff },
        0,
        " \ufffd\ufffd",
        -1,
        null);
      this.DoTestReadUtf8(
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
      for (int i = 0; i < arrayA.Length; ++i) {
        if (arrayA[i] != arrayB[i]) {
          return false;
        }
      }
      return true;
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestArray() {
      CBORObject cbor = CBORObject.FromJSONString("[]");
      cbor.Add(CBORObject.FromObject(3));
      cbor.Add(CBORObject.FromObject(4));
      byte[] bytes = cbor.EncodeToBytes();
      bool isequal = ByteArrayEquals(new byte[] { (byte)(0x80 | 2), 3, 4 }, bytes);
      Assert.IsTrue(isequal, "array not equal");
    }

    /// <summary>Not documented yet.</summary>
    [Test]
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
    }

    private static String Repeat(char c, int num) {
      System.Text.StringBuilder sb = new System.Text.StringBuilder();
      for (int i = 0; i < num; ++i) {
        sb.Append(c);
      }
      return sb.ToString();
    }

    private static String Repeat(String c, int num) {
      System.Text.StringBuilder sb = new System.Text.StringBuilder();
      for (int i = 0; i < num; ++i) {
        sb.Append(c);
      }
      return sb.ToString();
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestTextStringStream() {
      CBORObject cbor = TestCommon.FromBytesTestAB(
        new byte[] { 0x7F,
        0x61,
        0x2e,
        0x61,
        0x2e,
        0xFF });
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
    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestTextStringStreamNoTagsBeforeDefinite() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0x7F,
        0x61,
        0x20,
        0xC0,
        0x61,
        0x20,
        0xFF });
    }
    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestTextStringStreamNoIndefiniteWithinDefinite() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0x7F,
        0x61,
        0x20,
        0x7F,
        0x61,
        0x20,
        0xFF,
        0xFF });
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestByteStringStream() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0x5F,
        0x41,
        0x20,
        0x41,
        0x20,
        0xFF });
    }
    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestByteStringStreamNoTagsBeforeDefinite() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0x5F,
        0x41,
        0x20,
        0xC2,
        0x41,
        0x20,
        0xFF });
    }

    public static void AssertDecimalsEquivalent(string a, string b) {
      CBORObject ca = CBORDataUtilities.ParseJSONNumber(a);
      CBORObject cb = CBORDataUtilities.ParseJSONNumber(b);
      CompareTestEqual(ca, cb);
      TestCommon.AssertRoundTrip(ca);
      TestCommon.AssertRoundTrip(cb);
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void ZeroStringTests2() {
      Assert.AreEqual("0.0001265", ExtendedDecimal.FromString("1.265e-4").ToString());
      Assert.AreEqual("0.0001265", ExtendedDecimal.FromString("1.265e-4").ToEngineeringString());
      Assert.AreEqual("0.0001265", ExtendedDecimal.FromString("1.265e-4").ToPlainString());
      Assert.AreEqual("0.0000", ExtendedDecimal.FromString("0.000E-1").ToString());
      Assert.AreEqual("0.0000", ExtendedDecimal.FromString("0.000E-1").ToEngineeringString());
      Assert.AreEqual("0.0000", ExtendedDecimal.FromString("0.000E-1").ToPlainString());
      Assert.AreEqual("0E-16", ExtendedDecimal.FromString("0.0000000000000e-3").ToString());
      Assert.AreEqual("0.0E-15", ExtendedDecimal.FromString("0.0000000000000e-3").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000", ExtendedDecimal.FromString("0.0000000000000e-3").ToPlainString());
      Assert.AreEqual("0E-8", ExtendedDecimal.FromString("0.000000000e+1").ToString());
      Assert.AreEqual("0.00E-6", ExtendedDecimal.FromString("0.000000000e+1").ToEngineeringString());
      Assert.AreEqual("0.00000000", ExtendedDecimal.FromString("0.000000000e+1").ToPlainString());
      Assert.AreEqual("0.000", ExtendedDecimal.FromString("0.000000000000000e+12").ToString());
      Assert.AreEqual("0.000", ExtendedDecimal.FromString("0.000000000000000e+12").ToEngineeringString());
      Assert.AreEqual("0.000", ExtendedDecimal.FromString("0.000000000000000e+12").ToPlainString());
      Assert.AreEqual("0E-25", ExtendedDecimal.FromString("0.00000000000000e-11").ToString());
      Assert.AreEqual("0.0E-24", ExtendedDecimal.FromString("0.00000000000000e-11").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000000000", ExtendedDecimal.FromString("0.00000000000000e-11").ToPlainString());
      Assert.AreEqual("0E-7", ExtendedDecimal.FromString("0.000000000000e+5").ToString());
      Assert.AreEqual("0.0E-6", ExtendedDecimal.FromString("0.000000000000e+5").ToEngineeringString());
      Assert.AreEqual("0.0000000", ExtendedDecimal.FromString("0.000000000000e+5").ToPlainString());
      Assert.AreEqual("0E-8", ExtendedDecimal.FromString("0.0000e-4").ToString());
      Assert.AreEqual("0.00E-6", ExtendedDecimal.FromString("0.0000e-4").ToEngineeringString());
      Assert.AreEqual("0.00000000", ExtendedDecimal.FromString("0.0000e-4").ToPlainString());
      Assert.AreEqual("0.0000", ExtendedDecimal.FromString("0.000000e+2").ToString());
      Assert.AreEqual("0.0000", ExtendedDecimal.FromString("0.000000e+2").ToEngineeringString());
      Assert.AreEqual("0.0000", ExtendedDecimal.FromString("0.000000e+2").ToPlainString());
      Assert.AreEqual("0E+2", ExtendedDecimal.FromString("0.0e+3").ToString());
      Assert.AreEqual("0.0E+3", ExtendedDecimal.FromString("0.0e+3").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.0e+3").ToPlainString());
      Assert.AreEqual("0E-7", ExtendedDecimal.FromString("0.000000000000000e+8").ToString());
      Assert.AreEqual("0.0E-6", ExtendedDecimal.FromString("0.000000000000000e+8").ToEngineeringString());
      Assert.AreEqual("0.0000000", ExtendedDecimal.FromString("0.000000000000000e+8").ToPlainString());
      Assert.AreEqual("0E+7", ExtendedDecimal.FromString("0.000e+10").ToString());
      Assert.AreEqual("0.00E+9", ExtendedDecimal.FromString("0.000e+10").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.000e+10").ToPlainString());
      Assert.AreEqual("0E-31", ExtendedDecimal.FromString("0.0000000000000000000e-12").ToString());
      Assert.AreEqual("0.0E-30", ExtendedDecimal.FromString("0.0000000000000000000e-12").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000000000000000", ExtendedDecimal.FromString("0.0000000000000000000e-12").ToPlainString());
      Assert.AreEqual("0.00000", ExtendedDecimal.FromString("0.0000e-1").ToString());
      Assert.AreEqual("0.00000", ExtendedDecimal.FromString("0.0000e-1").ToEngineeringString());
      Assert.AreEqual("0.00000", ExtendedDecimal.FromString("0.0000e-1").ToPlainString());
      Assert.AreEqual("0E-22", ExtendedDecimal.FromString("0.00000000000e-11").ToString());
      Assert.AreEqual("0.0E-21", ExtendedDecimal.FromString("0.00000000000e-11").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000000", ExtendedDecimal.FromString("0.00000000000e-11").ToPlainString());
      Assert.AreEqual("0E-28", ExtendedDecimal.FromString("0.00000000000e-17").ToString());
      Assert.AreEqual("0.0E-27", ExtendedDecimal.FromString("0.00000000000e-17").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000000000000", ExtendedDecimal.FromString("0.00000000000e-17").ToPlainString());
      Assert.AreEqual("0.00000", ExtendedDecimal.FromString("0.00000000000000e+9").ToString());
      Assert.AreEqual("0.00000", ExtendedDecimal.FromString("0.00000000000000e+9").ToEngineeringString());
      Assert.AreEqual("0.00000", ExtendedDecimal.FromString("0.00000000000000e+9").ToPlainString());
      Assert.AreEqual("0E-28", ExtendedDecimal.FromString("0.0000000000e-18").ToString());
      Assert.AreEqual("0.0E-27", ExtendedDecimal.FromString("0.0000000000e-18").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000000000000", ExtendedDecimal.FromString("0.0000000000e-18").ToPlainString());
      Assert.AreEqual("0E-14", ExtendedDecimal.FromString("0.0e-13").ToString());
      Assert.AreEqual("0.00E-12", ExtendedDecimal.FromString("0.0e-13").ToEngineeringString());
      Assert.AreEqual("0.00000000000000", ExtendedDecimal.FromString("0.0e-13").ToPlainString());
      Assert.AreEqual("0E-8", ExtendedDecimal.FromString("0.000000000000000000e+10").ToString());
      Assert.AreEqual("0.00E-6", ExtendedDecimal.FromString("0.000000000000000000e+10").ToEngineeringString());
      Assert.AreEqual("0.00000000", ExtendedDecimal.FromString("0.000000000000000000e+10").ToPlainString());
      Assert.AreEqual("0E+15", ExtendedDecimal.FromString("0.0000e+19").ToString());
      Assert.AreEqual("0E+15", ExtendedDecimal.FromString("0.0000e+19").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.0000e+19").ToPlainString());
      Assert.AreEqual("0E-13", ExtendedDecimal.FromString("0.00000e-8").ToString());
      Assert.AreEqual("0.0E-12", ExtendedDecimal.FromString("0.00000e-8").ToEngineeringString());
      Assert.AreEqual("0.0000000000000", ExtendedDecimal.FromString("0.00000e-8").ToPlainString());
      Assert.AreEqual("0E+3", ExtendedDecimal.FromString("0.00000000000e+14").ToString());
      Assert.AreEqual("0E+3", ExtendedDecimal.FromString("0.00000000000e+14").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.00000000000e+14").ToPlainString());
      Assert.AreEqual("0E-17", ExtendedDecimal.FromString("0.000e-14").ToString());
      Assert.AreEqual("0.00E-15", ExtendedDecimal.FromString("0.000e-14").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000", ExtendedDecimal.FromString("0.000e-14").ToPlainString());
      Assert.AreEqual("0E-25", ExtendedDecimal.FromString("0.000000e-19").ToString());
      Assert.AreEqual("0.0E-24", ExtendedDecimal.FromString("0.000000e-19").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000000000", ExtendedDecimal.FromString("0.000000e-19").ToPlainString());
      Assert.AreEqual("0E+7", ExtendedDecimal.FromString("0.000000000000e+19").ToString());
      Assert.AreEqual("0.00E+9", ExtendedDecimal.FromString("0.000000000000e+19").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.000000000000e+19").ToPlainString());
      Assert.AreEqual("0E+5", ExtendedDecimal.FromString("0.0000000000000e+18").ToString());
      Assert.AreEqual("0.0E+6", ExtendedDecimal.FromString("0.0000000000000e+18").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.0000000000000e+18").ToPlainString());
      Assert.AreEqual("0E-16", ExtendedDecimal.FromString("0.00000000000000e-2").ToString());
      Assert.AreEqual("0.0E-15", ExtendedDecimal.FromString("0.00000000000000e-2").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000", ExtendedDecimal.FromString("0.00000000000000e-2").ToPlainString());
      Assert.AreEqual("0E-31", ExtendedDecimal.FromString("0.0000000000000e-18").ToString());
      Assert.AreEqual("0.0E-30", ExtendedDecimal.FromString("0.0000000000000e-18").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000000000000000", ExtendedDecimal.FromString("0.0000000000000e-18").ToPlainString());
      Assert.AreEqual("0E-17", ExtendedDecimal.FromString("0e-17").ToString());
      Assert.AreEqual("0.00E-15", ExtendedDecimal.FromString("0e-17").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000", ExtendedDecimal.FromString("0e-17").ToPlainString());
      Assert.AreEqual("0E+17", ExtendedDecimal.FromString("0e+17").ToString());
      Assert.AreEqual("0.0E+18", ExtendedDecimal.FromString("0e+17").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0e+17").ToPlainString());
      Assert.AreEqual("0E-17", ExtendedDecimal.FromString("0.00000000000000000e+0").ToString());
      Assert.AreEqual("0.00E-15", ExtendedDecimal.FromString("0.00000000000000000e+0").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000", ExtendedDecimal.FromString("0.00000000000000000e+0").ToPlainString());
      Assert.AreEqual("0E-13", ExtendedDecimal.FromString("0.0000000000000e+0").ToString());
      Assert.AreEqual("0.0E-12", ExtendedDecimal.FromString("0.0000000000000e+0").ToEngineeringString());
      Assert.AreEqual("0.0000000000000", ExtendedDecimal.FromString("0.0000000000000e+0").ToPlainString());
      Assert.AreEqual("0E-31", ExtendedDecimal.FromString("0.0000000000000000000e-12").ToString());
      Assert.AreEqual("0.0E-30", ExtendedDecimal.FromString("0.0000000000000000000e-12").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000000000000000", ExtendedDecimal.FromString("0.0000000000000000000e-12").ToPlainString());
      Assert.AreEqual("0E-9", ExtendedDecimal.FromString("0.0000000000000000000e+10").ToString());
      Assert.AreEqual("0E-9", ExtendedDecimal.FromString("0.0000000000000000000e+10").ToEngineeringString());
      Assert.AreEqual("0.000000000", ExtendedDecimal.FromString("0.0000000000000000000e+10").ToPlainString());
      Assert.AreEqual("0E-7", ExtendedDecimal.FromString("0.00000e-2").ToString());
      Assert.AreEqual("0.0E-6", ExtendedDecimal.FromString("0.00000e-2").ToEngineeringString());
      Assert.AreEqual("0.0000000", ExtendedDecimal.FromString("0.00000e-2").ToPlainString());
      Assert.AreEqual("0E+9", ExtendedDecimal.FromString("0.000000e+15").ToString());
      Assert.AreEqual("0E+9", ExtendedDecimal.FromString("0.000000e+15").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.000000e+15").ToPlainString());
      Assert.AreEqual("0E-19", ExtendedDecimal.FromString("0.000000000e-10").ToString());
      Assert.AreEqual("0.0E-18", ExtendedDecimal.FromString("0.000000000e-10").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000", ExtendedDecimal.FromString("0.000000000e-10").ToPlainString());
      Assert.AreEqual("0E-8", ExtendedDecimal.FromString("0.00000000000000e+6").ToString());
      Assert.AreEqual("0.00E-6", ExtendedDecimal.FromString("0.00000000000000e+6").ToEngineeringString());
      Assert.AreEqual("0.00000000", ExtendedDecimal.FromString("0.00000000000000e+6").ToPlainString());
      Assert.AreEqual("0E+12", ExtendedDecimal.FromString("0.00000e+17").ToString());
      Assert.AreEqual("0E+12", ExtendedDecimal.FromString("0.00000e+17").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.00000e+17").ToPlainString());
      Assert.AreEqual("0E-18", ExtendedDecimal.FromString("0.000000000000000000e-0").ToString());
      Assert.AreEqual("0E-18", ExtendedDecimal.FromString("0.000000000000000000e-0").ToEngineeringString());
      Assert.AreEqual("0.000000000000000000", ExtendedDecimal.FromString("0.000000000000000000e-0").ToPlainString());
      Assert.AreEqual("0.00000", ExtendedDecimal.FromString("0.0000000000000000e+11").ToString());
      Assert.AreEqual("0.00000", ExtendedDecimal.FromString("0.0000000000000000e+11").ToEngineeringString());
      Assert.AreEqual("0.00000", ExtendedDecimal.FromString("0.0000000000000000e+11").ToPlainString());
      Assert.AreEqual("0E+3", ExtendedDecimal.FromString("0.000000000000e+15").ToString());
      Assert.AreEqual("0E+3", ExtendedDecimal.FromString("0.000000000000e+15").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.000000000000e+15").ToPlainString());
      Assert.AreEqual("0E-27", ExtendedDecimal.FromString("0.00000000e-19").ToString());
      Assert.AreEqual("0E-27", ExtendedDecimal.FromString("0.00000000e-19").ToEngineeringString());
      Assert.AreEqual("0.000000000000000000000000000", ExtendedDecimal.FromString("0.00000000e-19").ToPlainString());
      Assert.AreEqual("0E-11", ExtendedDecimal.FromString("0.00000e-6").ToString());
      Assert.AreEqual("0.00E-9", ExtendedDecimal.FromString("0.00000e-6").ToEngineeringString());
      Assert.AreEqual("0.00000000000", ExtendedDecimal.FromString("0.00000e-6").ToPlainString());
      Assert.AreEqual("0E-14", ExtendedDecimal.FromString("0e-14").ToString());
      Assert.AreEqual("0.00E-12", ExtendedDecimal.FromString("0e-14").ToEngineeringString());
      Assert.AreEqual("0.00000000000000", ExtendedDecimal.FromString("0e-14").ToPlainString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.000000000e+9").ToString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.000000000e+9").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.000000000e+9").ToPlainString());
      Assert.AreEqual("0E+8", ExtendedDecimal.FromString("0.00000e+13").ToString());
      Assert.AreEqual("0.0E+9", ExtendedDecimal.FromString("0.00000e+13").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.00000e+13").ToPlainString());
      Assert.AreEqual("0.000", ExtendedDecimal.FromString("0.000e-0").ToString());
      Assert.AreEqual("0.000", ExtendedDecimal.FromString("0.000e-0").ToEngineeringString());
      Assert.AreEqual("0.000", ExtendedDecimal.FromString("0.000e-0").ToPlainString());
      Assert.AreEqual("0E-9", ExtendedDecimal.FromString("0.000000000000000e+6").ToString());
      Assert.AreEqual("0E-9", ExtendedDecimal.FromString("0.000000000000000e+6").ToEngineeringString());
      Assert.AreEqual("0.000000000", ExtendedDecimal.FromString("0.000000000000000e+6").ToPlainString());
      Assert.AreEqual("0E+8", ExtendedDecimal.FromString("0.000000000e+17").ToString());
      Assert.AreEqual("0.0E+9", ExtendedDecimal.FromString("0.000000000e+17").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.000000000e+17").ToPlainString());
      Assert.AreEqual("0.00000", ExtendedDecimal.FromString("0.00000000000e+6").ToString());
      Assert.AreEqual("0.00000", ExtendedDecimal.FromString("0.00000000000e+6").ToEngineeringString());
      Assert.AreEqual("0.00000", ExtendedDecimal.FromString("0.00000000000e+6").ToPlainString());
      Assert.AreEqual("0E-11", ExtendedDecimal.FromString("0.00000000000000e+3").ToString());
      Assert.AreEqual("0.00E-9", ExtendedDecimal.FromString("0.00000000000000e+3").ToEngineeringString());
      Assert.AreEqual("0.00000000000", ExtendedDecimal.FromString("0.00000000000000e+3").ToPlainString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0e+0").ToString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0e+0").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0e+0").ToPlainString());
      Assert.AreEqual("0E+9", ExtendedDecimal.FromString("0.000e+12").ToString());
      Assert.AreEqual("0E+9", ExtendedDecimal.FromString("0.000e+12").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.000e+12").ToPlainString());
      Assert.AreEqual("0.00", ExtendedDecimal.FromString("0.00000000000e+9").ToString());
      Assert.AreEqual("0.00", ExtendedDecimal.FromString("0.00000000000e+9").ToEngineeringString());
      Assert.AreEqual("0.00", ExtendedDecimal.FromString("0.00000000000e+9").ToPlainString());
      Assert.AreEqual("0E-23", ExtendedDecimal.FromString("0.00000000000000e-9").ToString());
      Assert.AreEqual("0.00E-21", ExtendedDecimal.FromString("0.00000000000000e-9").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000000000", ExtendedDecimal.FromString("0.00000000000000e-9").ToPlainString());
      Assert.AreEqual("0.0", ExtendedDecimal.FromString("0e-1").ToString());
      Assert.AreEqual("0.0", ExtendedDecimal.FromString("0e-1").ToEngineeringString());
      Assert.AreEqual("0.0", ExtendedDecimal.FromString("0e-1").ToPlainString());
      Assert.AreEqual("0E-17", ExtendedDecimal.FromString("0.0000e-13").ToString());
      Assert.AreEqual("0.00E-15", ExtendedDecimal.FromString("0.0000e-13").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000", ExtendedDecimal.FromString("0.0000e-13").ToPlainString());
      Assert.AreEqual("0E-18", ExtendedDecimal.FromString("0.00000000000e-7").ToString());
      Assert.AreEqual("0E-18", ExtendedDecimal.FromString("0.00000000000e-7").ToEngineeringString());
      Assert.AreEqual("0.000000000000000000", ExtendedDecimal.FromString("0.00000000000e-7").ToPlainString());
      Assert.AreEqual("0E-10", ExtendedDecimal.FromString("0.00000000000000e+4").ToString());
      Assert.AreEqual("0.0E-9", ExtendedDecimal.FromString("0.00000000000000e+4").ToEngineeringString());
      Assert.AreEqual("0.0000000000", ExtendedDecimal.FromString("0.00000000000000e+4").ToPlainString());
      Assert.AreEqual("0E-16", ExtendedDecimal.FromString("0.00000000e-8").ToString());
      Assert.AreEqual("0.0E-15", ExtendedDecimal.FromString("0.00000000e-8").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000", ExtendedDecimal.FromString("0.00000000e-8").ToPlainString());
      Assert.AreEqual("0E-8", ExtendedDecimal.FromString("0.00e-6").ToString());
      Assert.AreEqual("0.00E-6", ExtendedDecimal.FromString("0.00e-6").ToEngineeringString());
      Assert.AreEqual("0.00000000", ExtendedDecimal.FromString("0.00e-6").ToPlainString());
      Assert.AreEqual("0.00", ExtendedDecimal.FromString("0.0e-1").ToString());
      Assert.AreEqual("0.00", ExtendedDecimal.FromString("0.0e-1").ToEngineeringString());
      Assert.AreEqual("0.00", ExtendedDecimal.FromString("0.0e-1").ToPlainString());
      Assert.AreEqual("0E-26", ExtendedDecimal.FromString("0.0000000000000000e-10").ToString());
      Assert.AreEqual("0.00E-24", ExtendedDecimal.FromString("0.0000000000000000e-10").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000000000000", ExtendedDecimal.FromString("0.0000000000000000e-10").ToPlainString());
      Assert.AreEqual("0E+12", ExtendedDecimal.FromString("0.00e+14").ToString());
      Assert.AreEqual("0E+12", ExtendedDecimal.FromString("0.00e+14").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.00e+14").ToPlainString());
      Assert.AreEqual("0E-13", ExtendedDecimal.FromString("0.000000000000000000e+5").ToString());
      Assert.AreEqual("0.0E-12", ExtendedDecimal.FromString("0.000000000000000000e+5").ToEngineeringString());
      Assert.AreEqual("0.0000000000000", ExtendedDecimal.FromString("0.000000000000000000e+5").ToPlainString());
      Assert.AreEqual("0E+6", ExtendedDecimal.FromString("0.0e+7").ToString());
      Assert.AreEqual("0E+6", ExtendedDecimal.FromString("0.0e+7").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.0e+7").ToPlainString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.00000000e+8").ToString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.00000000e+8").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.00000000e+8").ToPlainString());
      Assert.AreEqual("0E-9", ExtendedDecimal.FromString("0.000000000e+0").ToString());
      Assert.AreEqual("0E-9", ExtendedDecimal.FromString("0.000000000e+0").ToEngineeringString());
      Assert.AreEqual("0.000000000", ExtendedDecimal.FromString("0.000000000e+0").ToPlainString());
      Assert.AreEqual("0E+10", ExtendedDecimal.FromString("0.000e+13").ToString());
      Assert.AreEqual("0.00E+12", ExtendedDecimal.FromString("0.000e+13").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.000e+13").ToPlainString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.0000000000000000e+16").ToString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.0000000000000000e+16").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.0000000000000000e+16").ToPlainString());
      Assert.AreEqual("0E-9", ExtendedDecimal.FromString("0.00000000e-1").ToString());
      Assert.AreEqual("0E-9", ExtendedDecimal.FromString("0.00000000e-1").ToEngineeringString());
      Assert.AreEqual("0.000000000", ExtendedDecimal.FromString("0.00000000e-1").ToPlainString());
      Assert.AreEqual("0E-26", ExtendedDecimal.FromString("0.00000000000e-15").ToString());
      Assert.AreEqual("0.00E-24", ExtendedDecimal.FromString("0.00000000000e-15").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000000000000", ExtendedDecimal.FromString("0.00000000000e-15").ToPlainString());
      Assert.AreEqual("0E+10", ExtendedDecimal.FromString("0.0e+11").ToString());
      Assert.AreEqual("0.00E+12", ExtendedDecimal.FromString("0.0e+11").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.0e+11").ToPlainString());
      Assert.AreEqual("0E+2", ExtendedDecimal.FromString("0.00000e+7").ToString());
      Assert.AreEqual("0.0E+3", ExtendedDecimal.FromString("0.00000e+7").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.00000e+7").ToPlainString());
      Assert.AreEqual("0E-38", ExtendedDecimal.FromString("0.0000000000000000000e-19").ToString());
      Assert.AreEqual("0.00E-36", ExtendedDecimal.FromString("0.0000000000000000000e-19").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000000000000000000000000", ExtendedDecimal.FromString("0.0000000000000000000e-19").ToPlainString());
      Assert.AreEqual("0E-16", ExtendedDecimal.FromString("0.0000000000e-6").ToString());
      Assert.AreEqual("0.0E-15", ExtendedDecimal.FromString("0.0000000000e-6").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000", ExtendedDecimal.FromString("0.0000000000e-6").ToPlainString());
      Assert.AreEqual("0E-32", ExtendedDecimal.FromString("0.00000000000000000e-15").ToString());
      Assert.AreEqual("0.00E-30", ExtendedDecimal.FromString("0.00000000000000000e-15").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000000000000000000", ExtendedDecimal.FromString("0.00000000000000000e-15").ToPlainString());
      Assert.AreEqual("0E-13", ExtendedDecimal.FromString("0.000000000000000e+2").ToString());
      Assert.AreEqual("0.0E-12", ExtendedDecimal.FromString("0.000000000000000e+2").ToEngineeringString());
      Assert.AreEqual("0.0000000000000", ExtendedDecimal.FromString("0.000000000000000e+2").ToPlainString());
      Assert.AreEqual("0E-19", ExtendedDecimal.FromString("0.0e-18").ToString());
      Assert.AreEqual("0.0E-18", ExtendedDecimal.FromString("0.0e-18").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000", ExtendedDecimal.FromString("0.0e-18").ToPlainString());
      Assert.AreEqual("0E-20", ExtendedDecimal.FromString("0.00000000000000e-6").ToString());
      Assert.AreEqual("0.00E-18", ExtendedDecimal.FromString("0.00000000000000e-6").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000000", ExtendedDecimal.FromString("0.00000000000000e-6").ToPlainString());
      Assert.AreEqual("0E-20", ExtendedDecimal.FromString("0.000e-17").ToString());
      Assert.AreEqual("0.00E-18", ExtendedDecimal.FromString("0.000e-17").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000000", ExtendedDecimal.FromString("0.000e-17").ToPlainString());
      Assert.AreEqual("0E-21", ExtendedDecimal.FromString("0.00000000000000e-7").ToString());
      Assert.AreEqual("0E-21", ExtendedDecimal.FromString("0.00000000000000e-7").ToEngineeringString());
      Assert.AreEqual("0.000000000000000000000", ExtendedDecimal.FromString("0.00000000000000e-7").ToPlainString());
      Assert.AreEqual("0E-15", ExtendedDecimal.FromString("0.000000e-9").ToString());
      Assert.AreEqual("0E-15", ExtendedDecimal.FromString("0.000000e-9").ToEngineeringString());
      Assert.AreEqual("0.000000000000000", ExtendedDecimal.FromString("0.000000e-9").ToPlainString());
      Assert.AreEqual("0E-11", ExtendedDecimal.FromString("0e-11").ToString());
      Assert.AreEqual("0.00E-9", ExtendedDecimal.FromString("0e-11").ToEngineeringString());
      Assert.AreEqual("0.00000000000", ExtendedDecimal.FromString("0e-11").ToPlainString());
      Assert.AreEqual("0E+2", ExtendedDecimal.FromString("0.000000000e+11").ToString());
      Assert.AreEqual("0.0E+3", ExtendedDecimal.FromString("0.000000000e+11").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.000000000e+11").ToPlainString());
      Assert.AreEqual("0.0", ExtendedDecimal.FromString("0.0000000000000000e+15").ToString());
      Assert.AreEqual("0.0", ExtendedDecimal.FromString("0.0000000000000000e+15").ToEngineeringString());
      Assert.AreEqual("0.0", ExtendedDecimal.FromString("0.0000000000000000e+15").ToPlainString());
      Assert.AreEqual("0.000000", ExtendedDecimal.FromString("0.0000000000000000e+10").ToString());
      Assert.AreEqual("0.000000", ExtendedDecimal.FromString("0.0000000000000000e+10").ToEngineeringString());
      Assert.AreEqual("0.000000", ExtendedDecimal.FromString("0.0000000000000000e+10").ToPlainString());
      Assert.AreEqual("0.00000", ExtendedDecimal.FromString("0.000000000e+4").ToString());
      Assert.AreEqual("0.00000", ExtendedDecimal.FromString("0.000000000e+4").ToEngineeringString());
      Assert.AreEqual("0.00000", ExtendedDecimal.FromString("0.000000000e+4").ToPlainString());
      Assert.AreEqual("0E-28", ExtendedDecimal.FromString("0.000000000000000e-13").ToString());
      Assert.AreEqual("0.0E-27", ExtendedDecimal.FromString("0.000000000000000e-13").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000000000000", ExtendedDecimal.FromString("0.000000000000000e-13").ToPlainString());
      Assert.AreEqual("0E-27", ExtendedDecimal.FromString("0.0000000000000000000e-8").ToString());
      Assert.AreEqual("0E-27", ExtendedDecimal.FromString("0.0000000000000000000e-8").ToEngineeringString());
      Assert.AreEqual("0.000000000000000000000000000", ExtendedDecimal.FromString("0.0000000000000000000e-8").ToPlainString());
      Assert.AreEqual("0E-26", ExtendedDecimal.FromString("0.00000000000e-15").ToString());
      Assert.AreEqual("0.00E-24", ExtendedDecimal.FromString("0.00000000000e-15").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000000000000", ExtendedDecimal.FromString("0.00000000000e-15").ToPlainString());
      Assert.AreEqual("0E+10", ExtendedDecimal.FromString("0.00e+12").ToString());
      Assert.AreEqual("0.00E+12", ExtendedDecimal.FromString("0.00e+12").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.00e+12").ToPlainString());
      Assert.AreEqual("0E+4", ExtendedDecimal.FromString("0.0e+5").ToString());
      Assert.AreEqual("0.00E+6", ExtendedDecimal.FromString("0.0e+5").ToEngineeringString());
      Assert.AreEqual("0", ExtendedDecimal.FromString("0.0e+5").ToPlainString());
      Assert.AreEqual("0E-9", ExtendedDecimal.FromString("0.0000000000000000e+7").ToString());
      Assert.AreEqual("0E-9", ExtendedDecimal.FromString("0.0000000000000000e+7").ToEngineeringString());
      Assert.AreEqual("0.000000000", ExtendedDecimal.FromString("0.0000000000000000e+7").ToPlainString());
      Assert.AreEqual("0E-16", ExtendedDecimal.FromString("0.0000000000000000e-0").ToString());
      Assert.AreEqual("0.0E-15", ExtendedDecimal.FromString("0.0000000000000000e-0").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000", ExtendedDecimal.FromString("0.0000000000000000e-0").ToPlainString());
      Assert.AreEqual("0.00", ExtendedDecimal.FromString("0.000000000000000e+13").ToString());
      Assert.AreEqual("0.00", ExtendedDecimal.FromString("0.000000000000000e+13").ToEngineeringString());
      Assert.AreEqual("0.00", ExtendedDecimal.FromString("0.000000000000000e+13").ToPlainString());
      Assert.AreEqual("0E-24", ExtendedDecimal.FromString("0.00000000000e-13").ToString());
      Assert.AreEqual("0E-24", ExtendedDecimal.FromString("0.00000000000e-13").ToEngineeringString());
      Assert.AreEqual("0.000000000000000000000000", ExtendedDecimal.FromString("0.00000000000e-13").ToPlainString());
      Assert.AreEqual("0E-13", ExtendedDecimal.FromString("0.000e-10").ToString());
      Assert.AreEqual("0.0E-12", ExtendedDecimal.FromString("0.000e-10").ToEngineeringString());
      Assert.AreEqual("0.0000000000000", ExtendedDecimal.FromString("0.000e-10").ToPlainString());
    }
    // Tests whether AsInt32/64/16/AsByte properly truncate floats
    // and doubles before bounds checking
    [Test]
    public void FloatingPointCloseToEdge() {
      try {
        CBORObject.FromObject(2.147483647E9d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.147483647E9d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.147483647E9d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.147483647E9d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836470000002E9d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836470000002E9d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836470000002E9d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836470000002E9d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836469999998E9d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836469999998E9d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836469999998E9d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836469999998E9d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.147483648E9d).AsInt32();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.147483648E9d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.147483648E9d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.147483648E9d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836480000005E9d).AsInt32();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836480000005E9d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836480000005E9d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836480000005E9d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836479999998E9d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836479999998E9d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836479999998E9d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836479999998E9d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.147483646E9d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.147483646E9d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.147483646E9d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.147483646E9d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836460000002E9d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836460000002E9d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836460000002E9d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836460000002E9d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836459999998E9d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836459999998E9d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836459999998E9d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474836459999998E9d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.147483648E9d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.147483648E9d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.147483648E9d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.147483648E9d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836479999998E9d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836479999998E9d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836479999998E9d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836479999998E9d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836480000005E9d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836480000005E9d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836480000005E9d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836480000005E9d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.147483647E9d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.147483647E9d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.147483647E9d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.147483647E9d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836469999998E9d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836469999998E9d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836469999998E9d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836469999998E9d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836470000002E9d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836470000002E9d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836470000002E9d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836470000002E9d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.147483649E9d).AsInt32();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.147483649E9d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.147483649E9d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.147483649E9d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836489999995E9d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836489999995E9d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836489999995E9d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836489999995E9d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836490000005E9d).AsInt32();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836490000005E9d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836490000005E9d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474836490000005E9d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.223372036854776E18d).AsInt32();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.223372036854776E18d).AsInt64();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.223372036854776E18d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.223372036854776E18d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.223372036854778E18d).AsInt32();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.223372036854778E18d).AsInt64();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.223372036854778E18d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.223372036854778E18d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.2233720368547748E18d).AsInt32();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.2233720368547748E18d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.2233720368547748E18d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.2233720368547748E18d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.223372036854776E18d).AsInt32();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.223372036854776E18d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.223372036854776E18d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.223372036854776E18d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.2233720368547748E18d).AsInt32();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.2233720368547748E18d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.2233720368547748E18d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.2233720368547748E18d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.223372036854778E18d).AsInt32();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.223372036854778E18d).AsInt64();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.223372036854778E18d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.223372036854778E18d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.0d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.0d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.0d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.0d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.000000000004d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.000000000004d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.000000000004d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.000000000004d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.999999999996d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.999999999996d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.999999999996d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.999999999996d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32768.0d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32768.0d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32768.0d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32768.0d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32768.00000000001d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32768.00000000001d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32768.00000000001d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32768.00000000001d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.999999999996d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.999999999996d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.999999999996d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.999999999996d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.0d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.0d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.0d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.0d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.000000000004d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.000000000004d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.000000000004d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.000000000004d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32765.999999999996d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32765.999999999996d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32765.999999999996d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32765.999999999996d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.0d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.0d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.0d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.0d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.999999999996d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.999999999996d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.999999999996d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.999999999996d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.00000000001d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.00000000001d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.00000000001d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.00000000001d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.0d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.0d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.0d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.0d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32766.999999999996d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32766.999999999996d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32766.999999999996d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32766.999999999996d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.000000000004d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.000000000004d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.000000000004d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.000000000004d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32769.0d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32769.0d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32769.0d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32769.0d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.99999999999d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.99999999999d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.99999999999d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.99999999999d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32769.00000000001d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32769.00000000001d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32769.00000000001d).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32769.00000000001d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(0.0d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(0.0d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(0.0d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(0.0d).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(4.9E-324d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(4.9E-324d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(4.9E-324d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(4.9E-324d).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-4.9E-324d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-4.9E-324d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-4.9E-324d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-4.9E-324d).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(1.0d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(1.0d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(1.0d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(1.0d).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(1.0000000000000002d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(1.0000000000000002d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(1.0000000000000002d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(1.0000000000000002d).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(0.9999999999999999d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(0.9999999999999999d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(0.9999999999999999d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(0.9999999999999999d).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-1.0d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-1.0d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-1.0d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-1.0d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-0.9999999999999999d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-0.9999999999999999d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-0.9999999999999999d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-0.9999999999999999d).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-1.0000000000000002d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-1.0000000000000002d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-1.0000000000000002d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-1.0000000000000002d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.0d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.0d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.0d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.0d).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.00000000000003d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.00000000000003d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.00000000000003d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.00000000000003d).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.99999999999997d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.99999999999997d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.99999999999997d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.99999999999997d).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(256.0d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(256.0d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(256.0d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(256.0d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(256.00000000000006d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(256.00000000000006d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(256.00000000000006d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(256.00000000000006d).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.99999999999997d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.99999999999997d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.99999999999997d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.99999999999997d).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.0d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.0d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.0d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.0d).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.00000000000003d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.00000000000003d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.00000000000003d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.00000000000003d).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(253.99999999999997d).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(253.99999999999997d).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(253.99999999999997d).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(253.99999999999997d).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.14748365E9f).AsInt32();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.14748365E9f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.14748365E9f).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.14748365E9f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474839E9f).AsInt32();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474839E9f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474839E9f).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.1474839E9f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.14748352E9f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.14748352E9f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.14748352E9f).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2.14748352E9f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.14748365E9f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.14748365E9f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.14748365E9f).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.14748365E9f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.14748352E9f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.14748352E9f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.14748352E9f).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.14748352E9f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474839E9f).AsInt32();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474839E9f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474839E9f).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-2.1474839E9f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.223372E18f).AsInt32();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.223372E18f).AsInt64();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.223372E18f).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.223372E18f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.223373E18f).AsInt32();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.223373E18f).AsInt64();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.223373E18f).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.223373E18f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.2233715E18f).AsInt32();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.2233715E18f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.2233715E18f).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(9.2233715E18f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.223372E18f).AsInt32();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.223372E18f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.223372E18f).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.223372E18f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.2233715E18f).AsInt32();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.2233715E18f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.2233715E18f).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.2233715E18f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.223373E18f).AsInt32();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.223373E18f).AsInt64();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.223373E18f).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-9.223373E18f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.0f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.0f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.0f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.0f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.002f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.002f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.002f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.002f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.998f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.998f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.998f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.998f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32768.0f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32768.0f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32768.0f).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32768.0f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32768.004f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32768.004f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32768.004f).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32768.004f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.998f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.998f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.998f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32767.998f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.0f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.0f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.0f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.0f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.002f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.002f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.002f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32766.002f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32765.998f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32765.998f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32765.998f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(32765.998f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.0f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.0f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.0f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.0f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.998f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.998f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.998f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.998f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.004f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.004f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.004f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.004f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.0f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.0f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.0f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.0f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32766.998f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32766.998f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32766.998f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32766.998f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.002f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.002f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.002f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32767.002f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32769.0f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32769.0f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32769.0f).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32769.0f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.996f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.996f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.996f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32768.996f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32769.004f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32769.004f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32769.004f).AsInt16();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-32769.004f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(0.0f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(0.0f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(0.0f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(0.0f).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(1.4E-45f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(1.4E-45f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(1.4E-45f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(1.4E-45f).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-1.4E-45f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-1.4E-45f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-1.4E-45f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-1.4E-45f).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(1.0f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(1.0f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(1.0f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(1.0f).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(1.0000001f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(1.0000001f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(1.0000001f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(1.0000001f).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(0.99999994f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(0.99999994f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(0.99999994f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(0.99999994f).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-1.0f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-1.0f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-1.0f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-1.0f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-0.99999994f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-0.99999994f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-0.99999994f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-0.99999994f).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-1.0000001f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-1.0000001f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-1.0000001f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(-1.0000001f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.0f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.0f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.0f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.0f).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.00002f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.00002f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.00002f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.00002f).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.99998f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.99998f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.99998f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.99998f).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(256.0f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(256.0f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(256.0f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(256.0f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(256.00003f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(256.00003f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(256.00003f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(256.00003f).AsByte();
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.99998f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.99998f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.99998f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(255.99998f).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.0f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.0f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.0f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.0f).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.00002f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.00002f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.00002f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(254.00002f).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(253.99998f).AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(253.99998f).AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(253.99998f).AsInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(253.99998f).AsByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void FromDoubleTest() {
      Assert.AreEqual("0.213299999999999989608312489508534781634807586669921875", ExtendedDecimal.FromDouble(0.2133).ToString());
      Assert.AreEqual("2.29360000000000010330982488752915582352898127282969653606414794921875E-7", ExtendedDecimal.FromDouble(2.2936E-7).ToString());
      Assert.AreEqual("3893200000", ExtendedDecimal.FromDouble(3.8932E9).ToString());
      Assert.AreEqual("128230", ExtendedDecimal.FromDouble(128230.0).ToString());
      Assert.AreEqual("127210", ExtendedDecimal.FromDouble(127210.0).ToString());
      Assert.AreEqual("0.267230000000000023074875343809253536164760589599609375", ExtendedDecimal.FromDouble(0.26723).ToString());
      Assert.AreEqual("0.302329999999999987636556397774256765842437744140625", ExtendedDecimal.FromDouble(0.30233).ToString());
      Assert.AreEqual("0.0000019512000000000000548530838806460252499164198525249958038330078125", ExtendedDecimal.FromDouble(1.9512E-6).ToString());
      Assert.AreEqual("199500", ExtendedDecimal.FromDouble(199500.0).ToString());
      Assert.AreEqual("36214000", ExtendedDecimal.FromDouble(3.6214E7).ToString());
      Assert.AreEqual("1913300000000", ExtendedDecimal.FromDouble(1.9133E12).ToString());
      Assert.AreEqual("0.0002173499999999999976289799530349000633577816188335418701171875", ExtendedDecimal.FromDouble(2.1735E-4).ToString());
      Assert.AreEqual("0.0000310349999999999967797807698399736864303122274577617645263671875", ExtendedDecimal.FromDouble(3.1035E-5).ToString());
      Assert.AreEqual("1.274999999999999911182158029987476766109466552734375", ExtendedDecimal.FromDouble(1.275).ToString());
      Assert.AreEqual("214190", ExtendedDecimal.FromDouble(214190.0).ToString());
      Assert.AreEqual("3981300000", ExtendedDecimal.FromDouble(3.9813E9).ToString());
      Assert.AreEqual("1092700", ExtendedDecimal.FromDouble(1092700.0).ToString());
      Assert.AreEqual("0.023609999999999999042987752773115062154829502105712890625", ExtendedDecimal.FromDouble(0.02361).ToString());
      Assert.AreEqual("12.321999999999999175770426518283784389495849609375", ExtendedDecimal.FromDouble(12.322).ToString());
      Assert.AreEqual("0.002586999999999999889921387108415729016996920108795166015625", ExtendedDecimal.FromDouble(0.002587).ToString());
      Assert.AreEqual("1322000000", ExtendedDecimal.FromDouble(1.322E9).ToString());
      Assert.AreEqual("95310000000", ExtendedDecimal.FromDouble(9.531E10).ToString());
      Assert.AreEqual("142.3799999999999954525264911353588104248046875", ExtendedDecimal.FromDouble(142.38).ToString());
      Assert.AreEqual("2252.5", ExtendedDecimal.FromDouble(2252.5).ToString());
      Assert.AreEqual("363600000000", ExtendedDecimal.FromDouble(3.636E11).ToString());
      Assert.AreEqual("0.00000323700000000000009386523676380154057596882921643555164337158203125", ExtendedDecimal.FromDouble(3.237E-6).ToString());
      Assert.AreEqual("728000", ExtendedDecimal.FromDouble(728000.0).ToString());
      Assert.AreEqual("25818000", ExtendedDecimal.FromDouble(2.5818E7).ToString());
      Assert.AreEqual("1090000", ExtendedDecimal.FromDouble(1090000.0).ToString());
      Assert.AreEqual("1.5509999999999999342747969421907328069210052490234375", ExtendedDecimal.FromDouble(1.551).ToString());
      Assert.AreEqual("26.035000000000000142108547152020037174224853515625", ExtendedDecimal.FromDouble(26.035).ToString());
      Assert.AreEqual("833000000", ExtendedDecimal.FromDouble(8.33E8).ToString());
      Assert.AreEqual("812300000000", ExtendedDecimal.FromDouble(8.123E11).ToString());
      Assert.AreEqual("2622.90000000000009094947017729282379150390625", ExtendedDecimal.FromDouble(2622.9).ToString());
      Assert.AreEqual("1.290999999999999925393012745189480483531951904296875", ExtendedDecimal.FromDouble(1.291).ToString());
      Assert.AreEqual("286140", ExtendedDecimal.FromDouble(286140.0).ToString());
      Assert.AreEqual("0.06733000000000000095923269327613525092601776123046875", ExtendedDecimal.FromDouble(0.06733).ToString());
      Assert.AreEqual("0.000325160000000000010654532811571471029310487210750579833984375", ExtendedDecimal.FromDouble(3.2516E-4).ToString());
      Assert.AreEqual("383230000", ExtendedDecimal.FromDouble(3.8323E8).ToString());
      Assert.AreEqual("0.02843299999999999994049204588009160943329334259033203125", ExtendedDecimal.FromDouble(0.028433).ToString());
      Assert.AreEqual("837000000", ExtendedDecimal.FromDouble(8.37E8).ToString());
      Assert.AreEqual("0.0160800000000000005428990590417015482671558856964111328125", ExtendedDecimal.FromDouble(0.01608).ToString());
      Assert.AreEqual("3621000000000", ExtendedDecimal.FromDouble(3.621E12).ToString());
      Assert.AreEqual("78.1200000000000045474735088646411895751953125", ExtendedDecimal.FromDouble(78.12).ToString());
      Assert.AreEqual("1308000000", ExtendedDecimal.FromDouble(1.308E9).ToString());
      Assert.AreEqual("0.031937000000000000110578213252665591426193714141845703125", ExtendedDecimal.FromDouble(0.031937).ToString());
      Assert.AreEqual("1581500", ExtendedDecimal.FromDouble(1581500.0).ToString());
      Assert.AreEqual("244200", ExtendedDecimal.FromDouble(244200.0).ToString());
      Assert.AreEqual("2.28179999999999995794237200343046456652018605382181704044342041015625E-7", ExtendedDecimal.FromDouble(2.2818E-7).ToString());
      Assert.AreEqual("39.73400000000000176214598468504846096038818359375", ExtendedDecimal.FromDouble(39.734).ToString());
      Assert.AreEqual("1614", ExtendedDecimal.FromDouble(1614.0).ToString());
      Assert.AreEqual("0.0003831899999999999954607143859419693399104289710521697998046875", ExtendedDecimal.FromDouble(3.8319E-4).ToString());
      Assert.AreEqual("543.3999999999999772626324556767940521240234375", ExtendedDecimal.FromDouble(543.4).ToString());
      Assert.AreEqual("319310000", ExtendedDecimal.FromDouble(3.1931E8).ToString());
      Assert.AreEqual("1429000", ExtendedDecimal.FromDouble(1429000.0).ToString());
      Assert.AreEqual("2653700000000", ExtendedDecimal.FromDouble(2.6537E12).ToString());
      Assert.AreEqual("722000000", ExtendedDecimal.FromDouble(7.22E8).ToString());
      Assert.AreEqual("27.199999999999999289457264239899814128875732421875", ExtendedDecimal.FromDouble(27.2).ToString());
      Assert.AreEqual("0.00000380250000000000001586513038998038638283105683512985706329345703125", ExtendedDecimal.FromDouble(3.8025E-6).ToString());
      Assert.AreEqual("0.0000364159999999999982843446044711299691698513925075531005859375", ExtendedDecimal.FromDouble(3.6416E-5).ToString());
      Assert.AreEqual("2006000", ExtendedDecimal.FromDouble(2006000.0).ToString());
      Assert.AreEqual("2681200000", ExtendedDecimal.FromDouble(2.6812E9).ToString());
      Assert.AreEqual("27534000000", ExtendedDecimal.FromDouble(2.7534E10).ToString());
      Assert.AreEqual("3.911600000000000165617541382501176627783934236504137516021728515625E-7", ExtendedDecimal.FromDouble(3.9116E-7).ToString());
      Assert.AreEqual("0.0028135000000000000286437540353290387429296970367431640625", ExtendedDecimal.FromDouble(0.0028135).ToString());
      Assert.AreEqual("0.91190000000000004387601393318618647754192352294921875", ExtendedDecimal.FromDouble(0.9119).ToString());
      Assert.AreEqual("2241200", ExtendedDecimal.FromDouble(2241200.0).ToString());
      Assert.AreEqual("32.4500000000000028421709430404007434844970703125", ExtendedDecimal.FromDouble(32.45).ToString());
      Assert.AreEqual("13800000000", ExtendedDecimal.FromDouble(1.38E10).ToString());
      Assert.AreEqual("0.047300000000000001765254609153998899273574352264404296875", ExtendedDecimal.FromDouble(0.0473).ToString());
      Assert.AreEqual("205.340000000000003410605131648480892181396484375", ExtendedDecimal.FromDouble(205.34).ToString());
      Assert.AreEqual("3.981899999999999995026200849679298698902130126953125", ExtendedDecimal.FromDouble(3.9819).ToString());
      Assert.AreEqual("1152.799999999999954525264911353588104248046875", ExtendedDecimal.FromDouble(1152.8).ToString());
      Assert.AreEqual("1322000", ExtendedDecimal.FromDouble(1322000.0).ToString());
      Assert.AreEqual("0.00013414000000000001334814203612921801322954706847667694091796875", ExtendedDecimal.FromDouble(1.3414E-4).ToString());
      Assert.AreEqual("3.4449999999999999446924077266263264363033158588223159313201904296875E-7", ExtendedDecimal.FromDouble(3.445E-7).ToString());
      Assert.AreEqual("1.3610000000000000771138253079228785935583800892345607280731201171875E-7", ExtendedDecimal.FromDouble(1.361E-7).ToString());
      Assert.AreEqual("26090000", ExtendedDecimal.FromDouble(2.609E7).ToString());
      Assert.AreEqual("9.93599999999999994315658113919198513031005859375", ExtendedDecimal.FromDouble(9.936).ToString());
      Assert.AreEqual("0.00000600000000000000015200514458246772164784488268196582794189453125", ExtendedDecimal.FromDouble(6.0E-6).ToString());
      Assert.AreEqual("260.31000000000000227373675443232059478759765625", ExtendedDecimal.FromDouble(260.31).ToString());
      Assert.AreEqual("344.6000000000000227373675443232059478759765625", ExtendedDecimal.FromDouble(344.6).ToString());
      Assert.AreEqual("3.423700000000000187583282240666449069976806640625", ExtendedDecimal.FromDouble(3.4237).ToString());
      Assert.AreEqual("2342100000", ExtendedDecimal.FromDouble(2.3421E9).ToString());
      Assert.AreEqual("0.00023310000000000000099260877295392901942250318825244903564453125", ExtendedDecimal.FromDouble(2.331E-4).ToString());
      Assert.AreEqual("0.7339999999999999857891452847979962825775146484375", ExtendedDecimal.FromDouble(0.734).ToString());
      Assert.AreEqual("0.01541499999999999988287147090204598498530685901641845703125", ExtendedDecimal.FromDouble(0.015415).ToString());
      Assert.AreEqual("0.0035311000000000001240729741169843691750429570674896240234375", ExtendedDecimal.FromDouble(0.0035311).ToString());
      Assert.AreEqual("1221700000000", ExtendedDecimal.FromDouble(1.2217E12).ToString());
      Assert.AreEqual("0.48299999999999998490096686509787105023860931396484375", ExtendedDecimal.FromDouble(0.483).ToString());
      Assert.AreEqual("0.0002871999999999999878506906636488338335766457021236419677734375", ExtendedDecimal.FromDouble(2.872E-4).ToString());
      Assert.AreEqual("96.1099999999999994315658113919198513031005859375", ExtendedDecimal.FromDouble(96.11).ToString());
      Assert.AreEqual("36570", ExtendedDecimal.FromDouble(36570.0).ToString());
      Assert.AreEqual("0.00001830000000000000097183545932910675446692039258778095245361328125", ExtendedDecimal.FromDouble(1.83E-5).ToString());
      Assert.AreEqual("301310000", ExtendedDecimal.FromDouble(3.0131E8).ToString());
      Assert.AreEqual("382200", ExtendedDecimal.FromDouble(382200.0).ToString());
      Assert.AreEqual("248350000", ExtendedDecimal.FromDouble(2.4835E8).ToString());
      Assert.AreEqual("0.0015839999999999999046040866090834242640994489192962646484375", ExtendedDecimal.FromDouble(0.001584).ToString());
      Assert.AreEqual("0.000761999999999999982035203682784185730270110070705413818359375", ExtendedDecimal.FromDouble(7.62E-4).ToString());
      Assert.AreEqual("313300000000", ExtendedDecimal.FromDouble(3.133E11).ToString());
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void ToPlainStringTest() {
      Assert.AreEqual("277220000000", ExtendedDecimal.FromString("277.22E9").ToPlainString());
      Assert.AreEqual("3911900", ExtendedDecimal.FromString("391.19E4").ToPlainString());
      Assert.AreEqual("0.00000038327", ExtendedDecimal.FromString("383.27E-9").ToPlainString());
      Assert.AreEqual("47330000000", ExtendedDecimal.FromString("47.33E9").ToPlainString());
      Assert.AreEqual("322210", ExtendedDecimal.FromString("322.21E3").ToPlainString());
      Assert.AreEqual("1.913", ExtendedDecimal.FromString("191.3E-2").ToPlainString());
      Assert.AreEqual("11917", ExtendedDecimal.FromString("119.17E2").ToPlainString());
      Assert.AreEqual("0.0001596", ExtendedDecimal.FromString("159.6E-6").ToPlainString());
      Assert.AreEqual("70160000000", ExtendedDecimal.FromString("70.16E9").ToPlainString());
      Assert.AreEqual("166240000000", ExtendedDecimal.FromString("166.24E9").ToPlainString());
      Assert.AreEqual("235250", ExtendedDecimal.FromString("235.25E3").ToPlainString());
      Assert.AreEqual("372200000", ExtendedDecimal.FromString("37.22E7").ToPlainString());
      Assert.AreEqual("32026000000", ExtendedDecimal.FromString("320.26E8").ToPlainString());
      Assert.AreEqual("0.00000012711", ExtendedDecimal.FromString("127.11E-9").ToPlainString());
      Assert.AreEqual("0.000009729", ExtendedDecimal.FromString("97.29E-7").ToPlainString());
      Assert.AreEqual("175130000000", ExtendedDecimal.FromString("175.13E9").ToPlainString());
      Assert.AreEqual("0.000003821", ExtendedDecimal.FromString("38.21E-7").ToPlainString());
      Assert.AreEqual("62.8", ExtendedDecimal.FromString("6.28E1").ToPlainString());
      Assert.AreEqual("138290000", ExtendedDecimal.FromString("138.29E6").ToPlainString());
      Assert.AreEqual("1601.9", ExtendedDecimal.FromString("160.19E1").ToPlainString());
      Assert.AreEqual("35812", ExtendedDecimal.FromString("358.12E2").ToPlainString());
      Assert.AreEqual("2492800000000", ExtendedDecimal.FromString("249.28E10").ToPlainString());
      Assert.AreEqual("0.00031123", ExtendedDecimal.FromString("311.23E-6").ToPlainString());
      Assert.AreEqual("0.16433", ExtendedDecimal.FromString("164.33E-3").ToPlainString());
      Assert.AreEqual("29.920", ExtendedDecimal.FromString("299.20E-1").ToPlainString());
      Assert.AreEqual("105390", ExtendedDecimal.FromString("105.39E3").ToPlainString());
      Assert.AreEqual("3825000", ExtendedDecimal.FromString("382.5E4").ToPlainString());
      Assert.AreEqual("909", ExtendedDecimal.FromString("90.9E1").ToPlainString());
      Assert.AreEqual("32915000000", ExtendedDecimal.FromString("329.15E8").ToPlainString());
      Assert.AreEqual("24523000000", ExtendedDecimal.FromString("245.23E8").ToPlainString());
      Assert.AreEqual("0.0000009719", ExtendedDecimal.FromString("97.19E-8").ToPlainString());
      Assert.AreEqual("551200000", ExtendedDecimal.FromString("55.12E7").ToPlainString());
      Assert.AreEqual("1238", ExtendedDecimal.FromString("12.38E2").ToPlainString());
      Assert.AreEqual("0.0025020", ExtendedDecimal.FromString("250.20E-5").ToPlainString());
      Assert.AreEqual("5320", ExtendedDecimal.FromString("53.20E2").ToPlainString());
      Assert.AreEqual("14150000000", ExtendedDecimal.FromString("141.5E8").ToPlainString());
      Assert.AreEqual("0.0033834", ExtendedDecimal.FromString("338.34E-5").ToPlainString());
      Assert.AreEqual("160390000000", ExtendedDecimal.FromString("160.39E9").ToPlainString());
      Assert.AreEqual("152170000", ExtendedDecimal.FromString("152.17E6").ToPlainString());
      Assert.AreEqual("13300000000", ExtendedDecimal.FromString("13.3E9").ToPlainString());
      Assert.AreEqual("13.8", ExtendedDecimal.FromString("1.38E1").ToPlainString());
      Assert.AreEqual("0.00000034821", ExtendedDecimal.FromString("348.21E-9").ToPlainString());
      Assert.AreEqual("525000000", ExtendedDecimal.FromString("52.5E7").ToPlainString());
      Assert.AreEqual("2152100000000", ExtendedDecimal.FromString("215.21E10").ToPlainString());
      Assert.AreEqual("234280000000", ExtendedDecimal.FromString("234.28E9").ToPlainString());
      Assert.AreEqual("310240000000", ExtendedDecimal.FromString("310.24E9").ToPlainString());
      Assert.AreEqual("345390000000", ExtendedDecimal.FromString("345.39E9").ToPlainString());
      Assert.AreEqual("0.00000011638", ExtendedDecimal.FromString("116.38E-9").ToPlainString());
      Assert.AreEqual("2762500000000", ExtendedDecimal.FromString("276.25E10").ToPlainString());
      Assert.AreEqual("0.0000015832", ExtendedDecimal.FromString("158.32E-8").ToPlainString());
      Assert.AreEqual("27250", ExtendedDecimal.FromString("272.5E2").ToPlainString());
      Assert.AreEqual("0.00000038933", ExtendedDecimal.FromString("389.33E-9").ToPlainString());
      Assert.AreEqual("3811500000", ExtendedDecimal.FromString("381.15E7").ToPlainString());
      Assert.AreEqual("280000", ExtendedDecimal.FromString("280.0E3").ToPlainString());
      Assert.AreEqual("0.0002742", ExtendedDecimal.FromString("274.2E-6").ToPlainString());
      Assert.AreEqual("0.000038714", ExtendedDecimal.FromString("387.14E-7").ToPlainString());
      Assert.AreEqual("0.00002277", ExtendedDecimal.FromString("227.7E-7").ToPlainString());
      Assert.AreEqual("20121", ExtendedDecimal.FromString("201.21E2").ToPlainString());
      Assert.AreEqual("255400", ExtendedDecimal.FromString("255.4E3").ToPlainString());
      Assert.AreEqual("0.000018727", ExtendedDecimal.FromString("187.27E-7").ToPlainString());
      Assert.AreEqual("0.01697", ExtendedDecimal.FromString("169.7E-4").ToPlainString());
      Assert.AreEqual("69900000000", ExtendedDecimal.FromString("69.9E9").ToPlainString());
      Assert.AreEqual("0.0320", ExtendedDecimal.FromString("3.20E-2").ToPlainString());
      Assert.AreEqual("23630", ExtendedDecimal.FromString("236.30E2").ToPlainString());
      Assert.AreEqual("0.00000022022", ExtendedDecimal.FromString("220.22E-9").ToPlainString());
      Assert.AreEqual("28.730", ExtendedDecimal.FromString("287.30E-1").ToPlainString());
      Assert.AreEqual("0.0000001563", ExtendedDecimal.FromString("156.3E-9").ToPlainString());
      Assert.AreEqual("13.623", ExtendedDecimal.FromString("136.23E-1").ToPlainString());
      Assert.AreEqual("12527000000", ExtendedDecimal.FromString("125.27E8").ToPlainString());
      Assert.AreEqual("0.000018030", ExtendedDecimal.FromString("180.30E-7").ToPlainString());
      Assert.AreEqual("3515000000", ExtendedDecimal.FromString("351.5E7").ToPlainString());
      Assert.AreEqual("28280000000", ExtendedDecimal.FromString("28.28E9").ToPlainString());
      Assert.AreEqual("0.2884", ExtendedDecimal.FromString("288.4E-3").ToPlainString());
      Assert.AreEqual("122200", ExtendedDecimal.FromString("12.22E4").ToPlainString());
      Assert.AreEqual("0.002575", ExtendedDecimal.FromString("257.5E-5").ToPlainString());
      Assert.AreEqual("389200", ExtendedDecimal.FromString("389.20E3").ToPlainString());
      Assert.AreEqual("0.03949", ExtendedDecimal.FromString("394.9E-4").ToPlainString());
      Assert.AreEqual("0.000013426", ExtendedDecimal.FromString("134.26E-7").ToPlainString());
      Assert.AreEqual("5829000", ExtendedDecimal.FromString("58.29E5").ToPlainString());
      Assert.AreEqual("0.000885", ExtendedDecimal.FromString("88.5E-5").ToPlainString());
      Assert.AreEqual("0.019329", ExtendedDecimal.FromString("193.29E-4").ToPlainString());
      Assert.AreEqual("713500000000", ExtendedDecimal.FromString("71.35E10").ToPlainString());
      Assert.AreEqual("2520", ExtendedDecimal.FromString("252.0E1").ToPlainString());
      Assert.AreEqual("0.000000532", ExtendedDecimal.FromString("53.2E-8").ToPlainString());
      Assert.AreEqual("18.120", ExtendedDecimal.FromString("181.20E-1").ToPlainString());
      Assert.AreEqual("0.00000005521", ExtendedDecimal.FromString("55.21E-9").ToPlainString());
      Assert.AreEqual("57.31", ExtendedDecimal.FromString("57.31E0").ToPlainString());
      Assert.AreEqual("0.00000011313", ExtendedDecimal.FromString("113.13E-9").ToPlainString());
      Assert.AreEqual("532.3", ExtendedDecimal.FromString("53.23E1").ToPlainString());
      Assert.AreEqual("0.000036837", ExtendedDecimal.FromString("368.37E-7").ToPlainString());
      Assert.AreEqual("0.01874", ExtendedDecimal.FromString("187.4E-4").ToPlainString());
      Assert.AreEqual("526000000", ExtendedDecimal.FromString("5.26E8").ToPlainString());
      Assert.AreEqual("3083200", ExtendedDecimal.FromString("308.32E4").ToPlainString());
      Assert.AreEqual("0.7615", ExtendedDecimal.FromString("76.15E-2").ToPlainString());
      Assert.AreEqual("1173800000", ExtendedDecimal.FromString("117.38E7").ToPlainString());
      Assert.AreEqual("0.001537", ExtendedDecimal.FromString("15.37E-4").ToPlainString());
      Assert.AreEqual("145.3", ExtendedDecimal.FromString("145.3E0").ToPlainString());
      Assert.AreEqual("22629000000", ExtendedDecimal.FromString("226.29E8").ToPlainString());
      Assert.AreEqual("2242600000000", ExtendedDecimal.FromString("224.26E10").ToPlainString());
      Assert.AreEqual("0.00000026818", ExtendedDecimal.FromString("268.18E-9").ToPlainString());
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void ToEngineeringStringTest() {
      Assert.AreEqual("8.912", ExtendedDecimal.FromString("89.12E-1").ToEngineeringString());
      Assert.AreEqual("0.024231", ExtendedDecimal.FromString("242.31E-4").ToEngineeringString());
      Assert.AreEqual("22.918E+6", ExtendedDecimal.FromString("229.18E5").ToEngineeringString());
      Assert.AreEqual("0.000032618", ExtendedDecimal.FromString("326.18E-7").ToEngineeringString());
      Assert.AreEqual("55.0E+6", ExtendedDecimal.FromString("55.0E6").ToEngineeringString());
      Assert.AreEqual("224.36E+3", ExtendedDecimal.FromString("224.36E3").ToEngineeringString());
      Assert.AreEqual("230.12E+9", ExtendedDecimal.FromString("230.12E9").ToEngineeringString());
      Assert.AreEqual("0.000011320", ExtendedDecimal.FromString("113.20E-7").ToEngineeringString());
      Assert.AreEqual("317.7E-9", ExtendedDecimal.FromString("317.7E-9").ToEngineeringString());
      Assert.AreEqual("3.393", ExtendedDecimal.FromString("339.3E-2").ToEngineeringString());
      Assert.AreEqual("27.135E+9", ExtendedDecimal.FromString("271.35E8").ToEngineeringString());
      Assert.AreEqual("377.19E-9", ExtendedDecimal.FromString("377.19E-9").ToEngineeringString());
      Assert.AreEqual("3.2127E+9", ExtendedDecimal.FromString("321.27E7").ToEngineeringString());
      Assert.AreEqual("2.9422", ExtendedDecimal.FromString("294.22E-2").ToEngineeringString());
      Assert.AreEqual("0.0000011031", ExtendedDecimal.FromString("110.31E-8").ToEngineeringString());
      Assert.AreEqual("2.4324", ExtendedDecimal.FromString("243.24E-2").ToEngineeringString());
      Assert.AreEqual("0.0006412", ExtendedDecimal.FromString("64.12E-5").ToEngineeringString());
      Assert.AreEqual("1422.3", ExtendedDecimal.FromString("142.23E1").ToEngineeringString());
      Assert.AreEqual("293.0", ExtendedDecimal.FromString("293.0E0").ToEngineeringString());
      Assert.AreEqual("0.0000025320", ExtendedDecimal.FromString("253.20E-8").ToEngineeringString());
      Assert.AreEqual("36.66E+9", ExtendedDecimal.FromString("366.6E8").ToEngineeringString());
      Assert.AreEqual("3.4526E+12", ExtendedDecimal.FromString("345.26E10").ToEngineeringString());
      Assert.AreEqual("2.704", ExtendedDecimal.FromString("270.4E-2").ToEngineeringString());
      Assert.AreEqual("432E+6", ExtendedDecimal.FromString("4.32E8").ToEngineeringString());
      Assert.AreEqual("224.22", ExtendedDecimal.FromString("224.22E0").ToEngineeringString());
      Assert.AreEqual("0.000031530", ExtendedDecimal.FromString("315.30E-7").ToEngineeringString());
      Assert.AreEqual("11.532E+6", ExtendedDecimal.FromString("115.32E5").ToEngineeringString());
      Assert.AreEqual("39420", ExtendedDecimal.FromString("394.20E2").ToEngineeringString());
      Assert.AreEqual("67.24E-9", ExtendedDecimal.FromString("67.24E-9").ToEngineeringString());
      Assert.AreEqual("34933", ExtendedDecimal.FromString("349.33E2").ToEngineeringString());
      Assert.AreEqual("67.8E-9", ExtendedDecimal.FromString("67.8E-9").ToEngineeringString());
      Assert.AreEqual("19.231E+6", ExtendedDecimal.FromString("192.31E5").ToEngineeringString());
      Assert.AreEqual("1.7317E+9", ExtendedDecimal.FromString("173.17E7").ToEngineeringString());
      Assert.AreEqual("43.9", ExtendedDecimal.FromString("43.9E0").ToEngineeringString());
      Assert.AreEqual("0.0000016812", ExtendedDecimal.FromString("168.12E-8").ToEngineeringString());
      Assert.AreEqual("3.715E+12", ExtendedDecimal.FromString("371.5E10").ToEngineeringString());
      Assert.AreEqual("424E-9", ExtendedDecimal.FromString("42.4E-8").ToEngineeringString());
      Assert.AreEqual("1.6123E+12", ExtendedDecimal.FromString("161.23E10").ToEngineeringString());
      Assert.AreEqual("302.8E+6", ExtendedDecimal.FromString("302.8E6").ToEngineeringString());
      Assert.AreEqual("175.13", ExtendedDecimal.FromString("175.13E0").ToEngineeringString());
      Assert.AreEqual("298.20E-9", ExtendedDecimal.FromString("298.20E-9").ToEngineeringString());
      Assert.AreEqual("36.223E+9", ExtendedDecimal.FromString("362.23E8").ToEngineeringString());
      Assert.AreEqual("27739", ExtendedDecimal.FromString("277.39E2").ToEngineeringString());
      Assert.AreEqual("0.011734", ExtendedDecimal.FromString("117.34E-4").ToEngineeringString());
      Assert.AreEqual("190.13E-9", ExtendedDecimal.FromString("190.13E-9").ToEngineeringString());
      Assert.AreEqual("3.5019", ExtendedDecimal.FromString("350.19E-2").ToEngineeringString());
      Assert.AreEqual("383.27E-9", ExtendedDecimal.FromString("383.27E-9").ToEngineeringString());
      Assert.AreEqual("24.217E+6", ExtendedDecimal.FromString("242.17E5").ToEngineeringString());
      Assert.AreEqual("2.9923E+9", ExtendedDecimal.FromString("299.23E7").ToEngineeringString());
      Assert.AreEqual("3.0222", ExtendedDecimal.FromString("302.22E-2").ToEngineeringString());
      Assert.AreEqual("0.04521", ExtendedDecimal.FromString("45.21E-3").ToEngineeringString());
      Assert.AreEqual("15.00", ExtendedDecimal.FromString("150.0E-1").ToEngineeringString());
      Assert.AreEqual("290E+3", ExtendedDecimal.FromString("29.0E4").ToEngineeringString());
      Assert.AreEqual("263.37E+3", ExtendedDecimal.FromString("263.37E3").ToEngineeringString());
      Assert.AreEqual("28.321", ExtendedDecimal.FromString("283.21E-1").ToEngineeringString());
      Assert.AreEqual("21.32", ExtendedDecimal.FromString("21.32E0").ToEngineeringString());
      Assert.AreEqual("0.00006920", ExtendedDecimal.FromString("69.20E-6").ToEngineeringString());
      Assert.AreEqual("0.0728", ExtendedDecimal.FromString("72.8E-3").ToEngineeringString());
      Assert.AreEqual("1.646E+9", ExtendedDecimal.FromString("164.6E7").ToEngineeringString());
      Assert.AreEqual("1.1817", ExtendedDecimal.FromString("118.17E-2").ToEngineeringString());
      Assert.AreEqual("0.000026235", ExtendedDecimal.FromString("262.35E-7").ToEngineeringString());
      Assert.AreEqual("23.37E+6", ExtendedDecimal.FromString("233.7E5").ToEngineeringString());
      Assert.AreEqual("391.24", ExtendedDecimal.FromString("391.24E0").ToEngineeringString());
      Assert.AreEqual("2213.6", ExtendedDecimal.FromString("221.36E1").ToEngineeringString());
      Assert.AreEqual("353.32", ExtendedDecimal.FromString("353.32E0").ToEngineeringString());
      Assert.AreEqual("0.012931", ExtendedDecimal.FromString("129.31E-4").ToEngineeringString());
      Assert.AreEqual("0.0017626", ExtendedDecimal.FromString("176.26E-5").ToEngineeringString());
      Assert.AreEqual("207.5E+3", ExtendedDecimal.FromString("207.5E3").ToEngineeringString());
      Assert.AreEqual("314.10", ExtendedDecimal.FromString("314.10E0").ToEngineeringString());
      Assert.AreEqual("379.20E+9", ExtendedDecimal.FromString("379.20E9").ToEngineeringString());
      Assert.AreEqual("0.00037912", ExtendedDecimal.FromString("379.12E-6").ToEngineeringString());
      Assert.AreEqual("743.8E-9", ExtendedDecimal.FromString("74.38E-8").ToEngineeringString());
      Assert.AreEqual("234.17E-9", ExtendedDecimal.FromString("234.17E-9").ToEngineeringString());
      Assert.AreEqual("132.6E+6", ExtendedDecimal.FromString("13.26E7").ToEngineeringString());
      Assert.AreEqual("25.15E+6", ExtendedDecimal.FromString("251.5E5").ToEngineeringString());
      Assert.AreEqual("87.32", ExtendedDecimal.FromString("87.32E0").ToEngineeringString());
      Assert.AreEqual("3.3116E+9", ExtendedDecimal.FromString("331.16E7").ToEngineeringString());
      Assert.AreEqual("6.14E+9", ExtendedDecimal.FromString("61.4E8").ToEngineeringString());
      Assert.AreEqual("0.0002097", ExtendedDecimal.FromString("209.7E-6").ToEngineeringString());
      Assert.AreEqual("5.4E+6", ExtendedDecimal.FromString("5.4E6").ToEngineeringString());
      Assert.AreEqual("219.9", ExtendedDecimal.FromString("219.9E0").ToEngineeringString());
      Assert.AreEqual("0.00002631", ExtendedDecimal.FromString("26.31E-6").ToEngineeringString());
      Assert.AreEqual("482.8E+6", ExtendedDecimal.FromString("48.28E7").ToEngineeringString());
      Assert.AreEqual("267.8", ExtendedDecimal.FromString("267.8E0").ToEngineeringString());
      Assert.AreEqual("0.3209", ExtendedDecimal.FromString("320.9E-3").ToEngineeringString());
      Assert.AreEqual("0.30015", ExtendedDecimal.FromString("300.15E-3").ToEngineeringString());
      Assert.AreEqual("2.6011E+6", ExtendedDecimal.FromString("260.11E4").ToEngineeringString());
      Assert.AreEqual("1.1429", ExtendedDecimal.FromString("114.29E-2").ToEngineeringString());
      Assert.AreEqual("0.0003060", ExtendedDecimal.FromString("306.0E-6").ToEngineeringString());
      Assert.AreEqual("97.7E+3", ExtendedDecimal.FromString("97.7E3").ToEngineeringString());
      Assert.AreEqual("12.229E+9", ExtendedDecimal.FromString("122.29E8").ToEngineeringString());
      Assert.AreEqual("6.94E+3", ExtendedDecimal.FromString("69.4E2").ToEngineeringString());
      Assert.AreEqual("383.5", ExtendedDecimal.FromString("383.5E0").ToEngineeringString());
      Assert.AreEqual("315.30E+3", ExtendedDecimal.FromString("315.30E3").ToEngineeringString());
      Assert.AreEqual("130.38E+9", ExtendedDecimal.FromString("130.38E9").ToEngineeringString());
      Assert.AreEqual("206.16E+9", ExtendedDecimal.FromString("206.16E9").ToEngineeringString());
      Assert.AreEqual("304.28E-9", ExtendedDecimal.FromString("304.28E-9").ToEngineeringString());
      Assert.AreEqual("661.3E+3", ExtendedDecimal.FromString("66.13E4").ToEngineeringString());
      Assert.AreEqual("1.8533", ExtendedDecimal.FromString("185.33E-2").ToEngineeringString());
      Assert.AreEqual("70.7E+6", ExtendedDecimal.FromString("70.7E6").ToEngineeringString());
    }

    /// <summary>Not documented yet.</summary>
    [Test]
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

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestAsByte() {
      for (int i = 0; i < 255; ++i) {
        Assert.AreEqual((byte)i, CBORObject.FromObject(i).AsByte());
      }
      for (int i = -200; i < 0; ++i) {
        try {
          CBORObject.FromObject(i).AsByte();
        } catch (OverflowException) {
        } catch (Exception ex) {
          Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
        }
      }
      for (int i = 256; i < 512; ++i) {
        try {
          CBORObject.FromObject(i).AsByte();
        } catch (OverflowException) {
        } catch (Exception ex) {
          Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
        }
      }
    }
    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestByteStringStreamNoIndefiniteWithinDefinite() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0x5F,
        0x41,
        0x20,
        0x5F,
        0x41,
        0x20,
        0xFF,
        0xFF });
    }

    [Test]
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

    [Test]
    public void TestExtendedDecimalExceptions() {
      try {
        ExtendedDecimal.Min(null, ExtendedDecimal.One);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.Min(ExtendedDecimal.One, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

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
    }

    /// <summary>Not documented yet.</summary>
    [Test]
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
    [Test]
    public void TestDecFracToSingleDoubleHighExponents() {
      if (-5.731800748367376E125d != ExtendedDecimal.FromString("-57318007483673759194E+106").ToDouble()) {
        Assert.Fail("otherValue double -57318007483673759194E+106\nExpected: -5.731800748367376E125d\nWas: " + ExtendedDecimal.FromString("-57318007483673759194E+106").ToDouble());
      }
      if (914323.0f != ExtendedDecimal.FromString("914323").ToSingle()) {
        Assert.Fail("otherValue single 914323\nExpected: 914323.0f\nWas: " + ExtendedDecimal.FromString("914323").ToSingle());
      }
      if (914323.0d != ExtendedDecimal.FromString("914323").ToDouble()) {
        Assert.Fail("otherValue double 914323\nExpected: 914323.0d\nWas: " + ExtendedDecimal.FromString("914323").ToDouble());
      }
      if (Single.NegativeInfinity != ExtendedDecimal.FromString("-57318007483673759194E+106").ToSingle()) {
        Assert.Fail("otherValue single -57318007483673759194E+106\nExpected: Single.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-57318007483673759194E+106").ToSingle());
      }
      if (0.0f != ExtendedDecimal.FromString("420685230629E-264").ToSingle()) {
        Assert.Fail("otherValue single 420685230629E-264\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("420685230629E-264").ToSingle());
      }
      if (4.20685230629E-253d != ExtendedDecimal.FromString("420685230629E-264").ToDouble()) {
        Assert.Fail("otherValue double 420685230629E-264\nExpected: 4.20685230629E-253d\nWas: " + ExtendedDecimal.FromString("420685230629E-264").ToDouble());
      }
      if (Single.PositiveInfinity != ExtendedDecimal.FromString("1089152800893419E+168").ToSingle()) {
        Assert.Fail("otherValue single 1089152800893419E+168\nExpected: Single.PositiveInfinity\nWas: " + ExtendedDecimal.FromString("1089152800893419E+168").ToSingle());
      }
      if (1.089152800893419E183d != ExtendedDecimal.FromString("1089152800893419E+168").ToDouble()) {
        Assert.Fail("otherValue double 1089152800893419E+168\nExpected: 1.089152800893419E183d\nWas: " + ExtendedDecimal.FromString("1089152800893419E+168").ToDouble());
      }
      if (1.5936804E7f != ExtendedDecimal.FromString("15936804").ToSingle()) {
        Assert.Fail("otherValue single 15936804\nExpected: 1.5936804E7f\nWas: " + ExtendedDecimal.FromString("15936804").ToSingle());
      }
      if (1.5936804E7d != ExtendedDecimal.FromString("15936804").ToDouble()) {
        Assert.Fail("otherValue double 15936804\nExpected: 1.5936804E7d\nWas: " + ExtendedDecimal.FromString("15936804").ToDouble());
      }
      if (Single.NegativeInfinity != ExtendedDecimal.FromString("-24681.2332E+61").ToSingle()) {
        Assert.Fail("otherValue single -24681.2332E+61\nExpected: Single.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-24681.2332E+61").ToSingle());
      }
      if (-2.46812332E65d != ExtendedDecimal.FromString("-24681.2332E+61").ToDouble()) {
        Assert.Fail("otherValue double -24681.2332E+61\nExpected: -2.46812332E65d\nWas: " + ExtendedDecimal.FromString("-24681.2332E+61").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-417509591569.6827833177512321E-93").ToSingle()) {
        Assert.Fail("otherValue single -417509591569.6827833177512321E-93\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-417509591569.6827833177512321E-93").ToSingle());
      }
      if (-4.175095915696828E-82d != ExtendedDecimal.FromString("-417509591569.6827833177512321E-93").ToDouble()) {
        Assert.Fail("otherValue double -417509591569.6827833177512321E-93\nExpected: -4.175095915696828E-82d\nWas: " + ExtendedDecimal.FromString("-417509591569.6827833177512321E-93").ToDouble());
      }
      if (5.38988331E17f != ExtendedDecimal.FromString("538988338119784732").ToSingle()) {
        Assert.Fail("otherValue single 538988338119784732\nExpected: 5.38988331E17f\nWas: " + ExtendedDecimal.FromString("538988338119784732").ToSingle());
      }
      if (5.389883381197847E17d != ExtendedDecimal.FromString("538988338119784732").ToDouble()) {
        Assert.Fail("otherValue double 538988338119784732\nExpected: 5.389883381197847E17d\nWas: " + ExtendedDecimal.FromString("538988338119784732").ToDouble());
      }
      if (260.14423f != ExtendedDecimal.FromString("260.1442248").ToSingle()) {
        Assert.Fail("otherValue single 260.1442248\nExpected: 260.14423f\nWas: " + ExtendedDecimal.FromString("260.1442248").ToSingle());
      }
      if (260.1442248d != ExtendedDecimal.FromString("260.1442248").ToDouble()) {
        Assert.Fail("otherValue double 260.1442248\nExpected: 260.1442248d\nWas: " + ExtendedDecimal.FromString("260.1442248").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-8457715957008143770.130850853640402959E-181").ToSingle()) {
        Assert.Fail("otherValue single -8457715957008143770.130850853640402959E-181\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-8457715957008143770.130850853640402959E-181").ToSingle());
      }
      if (-8.457715957008144E-163d != ExtendedDecimal.FromString("-8457715957008143770.130850853640402959E-181").ToDouble()) {
        Assert.Fail("otherValue double -8457715957008143770.130850853640402959E-181\nExpected: -8.457715957008144E-163d\nWas: " + ExtendedDecimal.FromString("-8457715957008143770.130850853640402959E-181").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("22.7178448747E-225").ToSingle()) {
        Assert.Fail("otherValue single 22.7178448747E-225\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("22.7178448747E-225").ToSingle());
      }
      if (2.27178448747E-224d != ExtendedDecimal.FromString("22.7178448747E-225").ToDouble()) {
        Assert.Fail("otherValue double 22.7178448747E-225\nExpected: 2.27178448747E-224d\nWas: " + ExtendedDecimal.FromString("22.7178448747E-225").ToDouble());
      }
      if (-790581.44f != ExtendedDecimal.FromString("-790581.4576317018014").ToSingle()) {
        Assert.Fail("otherValue single -790581.4576317018014\nExpected: -790581.44f\nWas: " + ExtendedDecimal.FromString("-790581.4576317018014").ToSingle());
      }
      if (-790581.4576317018d != ExtendedDecimal.FromString("-790581.4576317018014").ToDouble()) {
        Assert.Fail("otherValue double -790581.4576317018014\nExpected: -790581.4576317018d\nWas: " + ExtendedDecimal.FromString("-790581.4576317018014").ToDouble());
      }
      if (-1.80151695E16f != ExtendedDecimal.FromString("-18015168704168440").ToSingle()) {
        Assert.Fail("otherValue single -18015168704168440\nExpected: -1.80151695E16f\nWas: " + ExtendedDecimal.FromString("-18015168704168440").ToSingle());
      }
      if (-1.801516870416844E16d != ExtendedDecimal.FromString("-18015168704168440").ToDouble()) {
        Assert.Fail("otherValue double -18015168704168440\nExpected: -1.801516870416844E16d\nWas: " + ExtendedDecimal.FromString("-18015168704168440").ToDouble());
      }
      if (-36.0f != ExtendedDecimal.FromString("-36").ToSingle()) {
        Assert.Fail("otherValue single -36\nExpected: -36.0f\nWas: " + ExtendedDecimal.FromString("-36").ToSingle());
      }
      if (-36.0d != ExtendedDecimal.FromString("-36").ToDouble()) {
        Assert.Fail("otherValue double -36\nExpected: -36.0d\nWas: " + ExtendedDecimal.FromString("-36").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("653060307988076E-230").ToSingle()) {
        Assert.Fail("otherValue single 653060307988076E-230\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("653060307988076E-230").ToSingle());
      }
      if (6.53060307988076E-216d != ExtendedDecimal.FromString("653060307988076E-230").ToDouble()) {
        Assert.Fail("otherValue double 653060307988076E-230\nExpected: 6.53060307988076E-216d\nWas: " + ExtendedDecimal.FromString("653060307988076E-230").ToDouble());
      }
      if (Single.NegativeInfinity != ExtendedDecimal.FromString("-4446345.5911E+316").ToSingle()) {
        Assert.Fail("otherValue single -4446345.5911E+316\nExpected: Single.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-4446345.5911E+316").ToSingle());
      }
      if (Double.NegativeInfinity != ExtendedDecimal.FromString("-4446345.5911E+316").ToDouble()) {
        Assert.Fail("otherValue double -4446345.5911E+316\nExpected: Double.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-4446345.5911E+316").ToDouble());
      }
      if (-5.3940226E15f != ExtendedDecimal.FromString("-5394022706804125.84338479899885").ToSingle()) {
        Assert.Fail("otherValue single -5394022706804125.84338479899885\nExpected: -5.3940226E15f\nWas: " + ExtendedDecimal.FromString("-5394022706804125.84338479899885").ToSingle());
      }
      if (-5.394022706804126E15d != ExtendedDecimal.FromString("-5394022706804125.84338479899885").ToDouble()) {
        Assert.Fail("otherValue double -5394022706804125.84338479899885\nExpected: -5.394022706804126E15d\nWas: " + ExtendedDecimal.FromString("-5394022706804125.84338479899885").ToDouble());
      }
      if (Single.PositiveInfinity != ExtendedDecimal.FromString("310504020304E+181").ToSingle()) {
        Assert.Fail("otherValue single 310504020304E+181\nExpected: Single.PositiveInfinity\nWas: " + ExtendedDecimal.FromString("310504020304E+181").ToSingle());
      }
      if (3.10504020304E192d != ExtendedDecimal.FromString("310504020304E+181").ToDouble()) {
        Assert.Fail("otherValue double 310504020304E+181\nExpected: 3.10504020304E192d\nWas: " + ExtendedDecimal.FromString("310504020304E+181").ToDouble());
      }
      if (Single.NegativeInfinity != ExtendedDecimal.FromString("-164609450222646.21988340572652533E+317").ToSingle()) {
        Assert.Fail("otherValue single -164609450222646.21988340572652533E+317\nExpected: Single.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-164609450222646.21988340572652533E+317").ToSingle());
      }
      if (Double.NegativeInfinity != ExtendedDecimal.FromString("-164609450222646.21988340572652533E+317").ToDouble()) {
        Assert.Fail("otherValue double -164609450222646.21988340572652533E+317\nExpected: Double.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-164609450222646.21988340572652533E+317").ToDouble());
      }
      if (7.1524661E18f != ExtendedDecimal.FromString("7152466127871812565.075310").ToSingle()) {
        Assert.Fail("otherValue single 7152466127871812565.075310\nExpected: 7.1524661E18f\nWas: " + ExtendedDecimal.FromString("7152466127871812565.075310").ToSingle());
      }
      if (7.1524661278718126E18d != ExtendedDecimal.FromString("7152466127871812565.075310").ToDouble()) {
        Assert.Fail("otherValue double 7152466127871812565.075310\nExpected: 7.1524661278718126E18d\nWas: " + ExtendedDecimal.FromString("7152466127871812565.075310").ToDouble());
      }
      if (925.0f != ExtendedDecimal.FromString("925").ToSingle()) {
        Assert.Fail("otherValue single 925\nExpected: 925.0f\nWas: " + ExtendedDecimal.FromString("925").ToSingle());
      }
      if (925.0d != ExtendedDecimal.FromString("925").ToDouble()) {
        Assert.Fail("otherValue double 925\nExpected: 925.0d\nWas: " + ExtendedDecimal.FromString("925").ToDouble());
      }
      if (34794.0f != ExtendedDecimal.FromString("34794").ToSingle()) {
        Assert.Fail("otherValue single 34794\nExpected: 34794.0f\nWas: " + ExtendedDecimal.FromString("34794").ToSingle());
      }
      if (34794.0d != ExtendedDecimal.FromString("34794").ToDouble()) {
        Assert.Fail("otherValue double 34794\nExpected: 34794.0d\nWas: " + ExtendedDecimal.FromString("34794").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-337655705333269E-276").ToSingle()) {
        Assert.Fail("otherValue single -337655705333269E-276\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-337655705333269E-276").ToSingle());
      }
      if (-3.37655705333269E-262d != ExtendedDecimal.FromString("-337655705333269E-276").ToDouble()) {
        Assert.Fail("otherValue double -337655705333269E-276\nExpected: -3.37655705333269E-262d\nWas: " + ExtendedDecimal.FromString("-337655705333269E-276").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-564484627E-81").ToSingle()) {
        Assert.Fail("otherValue single -564484627E-81\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-564484627E-81").ToSingle());
      }
      if (-5.64484627E-73d != ExtendedDecimal.FromString("-564484627E-81").ToDouble()) {
        Assert.Fail("otherValue double -564484627E-81\nExpected: -5.64484627E-73d\nWas: " + ExtendedDecimal.FromString("-564484627E-81").ToDouble());
      }
      if (Single.NegativeInfinity != ExtendedDecimal.FromString("-249095219081.80985049618E+175").ToSingle()) {
        Assert.Fail("otherValue single -249095219081.80985049618E+175\nExpected: Single.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-249095219081.80985049618E+175").ToSingle());
      }
      if (-2.4909521908180986E186d != ExtendedDecimal.FromString("-249095219081.80985049618E+175").ToDouble()) {
        Assert.Fail("otherValue double -249095219081.80985049618E+175\nExpected: -2.4909521908180986E186d\nWas: " + ExtendedDecimal.FromString("-249095219081.80985049618E+175").ToDouble());
      }
      if (Single.NegativeInfinity != ExtendedDecimal.FromString("-1696361380616078392E+221").ToSingle()) {
        Assert.Fail("otherValue single -1696361380616078392E+221\nExpected: Single.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-1696361380616078392E+221").ToSingle());
      }
      if (-1.6963613806160784E239d != ExtendedDecimal.FromString("-1696361380616078392E+221").ToDouble()) {
        Assert.Fail("otherValue double -1696361380616078392E+221\nExpected: -1.6963613806160784E239d\nWas: " + ExtendedDecimal.FromString("-1696361380616078392E+221").ToDouble());
      }
      if (Single.PositiveInfinity != ExtendedDecimal.FromString("61520501993928105481.8536829047214988E+205").ToSingle()) {
        Assert.Fail("otherValue single 61520501993928105481.8536829047214988E+205\nExpected: Single.PositiveInfinity\nWas: " + ExtendedDecimal.FromString("61520501993928105481.8536829047214988E+205").ToSingle());
      }
      if (6.15205019939281E224d != ExtendedDecimal.FromString("61520501993928105481.8536829047214988E+205").ToDouble()) {
        Assert.Fail("otherValue double 61520501993928105481.8536829047214988E+205\nExpected: 6.15205019939281E224d\nWas: " + ExtendedDecimal.FromString("61520501993928105481.8536829047214988E+205").ToDouble());
      }
      if (2.08756651E14f != ExtendedDecimal.FromString("208756654290770").ToSingle()) {
        Assert.Fail("otherValue single 208756654290770\nExpected: 2.08756651E14f\nWas: " + ExtendedDecimal.FromString("208756654290770").ToSingle());
      }
      if (2.0875665429077E14d != ExtendedDecimal.FromString("208756654290770").ToDouble()) {
        Assert.Fail("otherValue double 208756654290770\nExpected: 2.0875665429077E14d\nWas: " + ExtendedDecimal.FromString("208756654290770").ToDouble());
      }
      if (-1.31098592E13f != ExtendedDecimal.FromString("-13109858687380").ToSingle()) {
        Assert.Fail("otherValue single -13109858687380\nExpected: -1.31098592E13f\nWas: " + ExtendedDecimal.FromString("-13109858687380").ToSingle());
      }
      if (-1.310985868738E13d != ExtendedDecimal.FromString("-13109858687380").ToDouble()) {
        Assert.Fail("otherValue double -13109858687380\nExpected: -1.310985868738E13d\nWas: " + ExtendedDecimal.FromString("-13109858687380").ToDouble());
      }
      if (Single.PositiveInfinity != ExtendedDecimal.FromString("6650596004E+280").ToSingle()) {
        Assert.Fail("otherValue single 6650596004E+280\nExpected: Single.PositiveInfinity\nWas: " + ExtendedDecimal.FromString("6650596004E+280").ToSingle());
      }
      if (6.650596004E289d != ExtendedDecimal.FromString("6650596004E+280").ToDouble()) {
        Assert.Fail("otherValue double 6650596004E+280\nExpected: 6.650596004E289d\nWas: " + ExtendedDecimal.FromString("6650596004E+280").ToDouble());
      }
      if (-9.2917935E13f != ExtendedDecimal.FromString("-92917937534357E0").ToSingle()) {
        Assert.Fail("otherValue single -92917937534357E0\nExpected: -9.2917935E13f\nWas: " + ExtendedDecimal.FromString("-92917937534357E0").ToSingle());
      }
      if (-9.2917937534357E13d != ExtendedDecimal.FromString("-92917937534357E0").ToDouble()) {
        Assert.Fail("otherValue double -92917937534357E0\nExpected: -9.2917937534357E13d\nWas: " + ExtendedDecimal.FromString("-92917937534357E0").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-46E-153").ToSingle()) {
        Assert.Fail("otherValue single -46E-153\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-46E-153").ToSingle());
      }
      if (-4.6E-152d != ExtendedDecimal.FromString("-46E-153").ToDouble()) {
        Assert.Fail("otherValue double -46E-153\nExpected: -4.6E-152d\nWas: " + ExtendedDecimal.FromString("-46E-153").ToDouble());
      }
      if (1.05161414E13f != ExtendedDecimal.FromString("10516141645281.77872161523035480").ToSingle()) {
        Assert.Fail("otherValue single 10516141645281.77872161523035480\nExpected: 1.05161414E13f\nWas: " + ExtendedDecimal.FromString("10516141645281.77872161523035480").ToSingle());
      }
      if (1.051614164528178E13d != ExtendedDecimal.FromString("10516141645281.77872161523035480").ToDouble()) {
        Assert.Fail("otherValue double 10516141645281.77872161523035480\nExpected: 1.051614164528178E13d\nWas: " + ExtendedDecimal.FromString("10516141645281.77872161523035480").ToDouble());
      }
      if (Single.NegativeInfinity != ExtendedDecimal.FromString("-8312147094254E+299").ToSingle()) {
        Assert.Fail("otherValue single -8312147094254E+299\nExpected: Single.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-8312147094254E+299").ToSingle());
      }
      if (Double.NegativeInfinity != ExtendedDecimal.FromString("-8312147094254E+299").ToDouble()) {
        Assert.Fail("otherValue double -8312147094254E+299\nExpected: Double.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-8312147094254E+299").ToDouble());
      }
      if (5.10270368E8f != ExtendedDecimal.FromString("510270376.1879").ToSingle()) {
        Assert.Fail("otherValue single 510270376.1879\nExpected: 5.10270368E8f\nWas: " + ExtendedDecimal.FromString("510270376.1879").ToSingle());
      }
      if (5.102703761879E8d != ExtendedDecimal.FromString("510270376.1879").ToDouble()) {
        Assert.Fail("otherValue double 510270376.1879\nExpected: 5.102703761879E8d\nWas: " + ExtendedDecimal.FromString("510270376.1879").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-693696E-143").ToSingle()) {
        Assert.Fail("otherValue single -693696E-143\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-693696E-143").ToSingle());
      }
      if (-6.93696E-138d != ExtendedDecimal.FromString("-693696E-143").ToDouble()) {
        Assert.Fail("otherValue double -693696E-143\nExpected: -6.93696E-138d\nWas: " + ExtendedDecimal.FromString("-693696E-143").ToDouble());
      }
      if (Single.NegativeInfinity != ExtendedDecimal.FromString("-91.43E+139").ToSingle()) {
        Assert.Fail("otherValue single -91.43E+139\nExpected: Single.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-91.43E+139").ToSingle());
      }
      if (-9.143E140d != ExtendedDecimal.FromString("-91.43E+139").ToDouble()) {
        Assert.Fail("otherValue double -91.43E+139\nExpected: -9.143E140d\nWas: " + ExtendedDecimal.FromString("-91.43E+139").ToDouble());
      }
      if (Single.NegativeInfinity != ExtendedDecimal.FromString("-4103819741762400.45807953367286162E+235").ToSingle()) {
        Assert.Fail("otherValue single -4103819741762400.45807953367286162E+235\nExpected: Single.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-4103819741762400.45807953367286162E+235").ToSingle());
      }
      if (-4.1038197417624E250d != ExtendedDecimal.FromString("-4103819741762400.45807953367286162E+235").ToDouble()) {
        Assert.Fail("otherValue double -4103819741762400.45807953367286162E+235\nExpected: -4.1038197417624E250d\nWas: " + ExtendedDecimal.FromString("-4103819741762400.45807953367286162E+235").ToDouble());
      }
      if (-1.44700998E11f != ExtendedDecimal.FromString("-144701002301.18954542331279957").ToSingle()) {
        Assert.Fail("otherValue single -144701002301.18954542331279957\nExpected: -1.44700998E11f\nWas: " + ExtendedDecimal.FromString("-144701002301.18954542331279957").ToSingle());
      }
      if (-1.4470100230118954E11d != ExtendedDecimal.FromString("-144701002301.18954542331279957").ToDouble()) {
        Assert.Fail("otherValue double -144701002301.18954542331279957\nExpected: -1.4470100230118954E11d\nWas: " + ExtendedDecimal.FromString("-144701002301.18954542331279957").ToDouble());
      }
      if (Single.PositiveInfinity != ExtendedDecimal.FromString("73.01E+211").ToSingle()) {
        Assert.Fail("otherValue single 73.01E+211\nExpected: Single.PositiveInfinity\nWas: " + ExtendedDecimal.FromString("73.01E+211").ToSingle());
      }
      if (7.301E212d != ExtendedDecimal.FromString("73.01E+211").ToDouble()) {
        Assert.Fail("otherValue double 73.01E+211\nExpected: 7.301E212d\nWas: " + ExtendedDecimal.FromString("73.01E+211").ToDouble());
      }
      if (-4.4030403E9f != ExtendedDecimal.FromString("-4403040441").ToSingle()) {
        Assert.Fail("otherValue single -4403040441\nExpected: -4.4030403E9f\nWas: " + ExtendedDecimal.FromString("-4403040441").ToSingle());
      }
      if (-4.403040441E9d != ExtendedDecimal.FromString("-4403040441").ToDouble()) {
        Assert.Fail("otherValue double -4403040441\nExpected: -4.403040441E9d\nWas: " + ExtendedDecimal.FromString("-4403040441").ToDouble());
      }
      if (Single.NegativeInfinity != ExtendedDecimal.FromString("-19E+64").ToSingle()) {
        Assert.Fail("otherValue single -19E+64\nExpected: Single.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-19E+64").ToSingle());
      }
      if (-1.9E65d != ExtendedDecimal.FromString("-19E+64").ToDouble()) {
        Assert.Fail("otherValue double -19E+64\nExpected: -1.9E65d\nWas: " + ExtendedDecimal.FromString("-19E+64").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("6454087684516815.5353496080253E-144").ToSingle()) {
        Assert.Fail("otherValue single 6454087684516815.5353496080253E-144\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("6454087684516815.5353496080253E-144").ToSingle());
      }
      if (6.454087684516816E-129d != ExtendedDecimal.FromString("6454087684516815.5353496080253E-144").ToDouble()) {
        Assert.Fail("otherValue double 6454087684516815.5353496080253E-144\nExpected: 6.454087684516816E-129d\nWas: " + ExtendedDecimal.FromString("6454087684516815.5353496080253E-144").ToDouble());
      }
      if (Single.PositiveInfinity != ExtendedDecimal.FromString("1051852710343668.522107559786846776E+278").ToSingle()) {
        Assert.Fail("otherValue single 1051852710343668.522107559786846776E+278\nExpected: Single.PositiveInfinity\nWas: " + ExtendedDecimal.FromString("1051852710343668.522107559786846776E+278").ToSingle());
      }
      if (1.0518527103436685E293d != ExtendedDecimal.FromString("1051852710343668.522107559786846776E+278").ToDouble()) {
        Assert.Fail("otherValue double 1051852710343668.522107559786846776E+278\nExpected: 1.0518527103436685E293d\nWas: " + ExtendedDecimal.FromString("1051852710343668.522107559786846776E+278").ToDouble());
      }
      if (Single.PositiveInfinity != ExtendedDecimal.FromString("86077128802.374518623891E+218").ToSingle()) {
        Assert.Fail("otherValue single 86077128802.374518623891E+218\nExpected: Single.PositiveInfinity\nWas: " + ExtendedDecimal.FromString("86077128802.374518623891E+218").ToSingle());
      }
      if (8.607712880237452E228d != ExtendedDecimal.FromString("86077128802.374518623891E+218").ToDouble()) {
        Assert.Fail("otherValue double 86077128802.374518623891E+218\nExpected: 8.607712880237452E228d\nWas: " + ExtendedDecimal.FromString("86077128802.374518623891E+218").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("367820230207102E-199").ToSingle()) {
        Assert.Fail("otherValue single 367820230207102E-199\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("367820230207102E-199").ToSingle());
      }
      if (3.67820230207102E-185d != ExtendedDecimal.FromString("367820230207102E-199").ToDouble()) {
        Assert.Fail("otherValue double 367820230207102E-199\nExpected: 3.67820230207102E-185d\nWas: " + ExtendedDecimal.FromString("367820230207102E-199").ToDouble());
      }
      if (9.105086E-27f != ExtendedDecimal.FromString("91050857573912688994E-46").ToSingle()) {
        Assert.Fail("otherValue single 91050857573912688994E-46\nExpected: 9.105086E-27f\nWas: " + ExtendedDecimal.FromString("91050857573912688994E-46").ToSingle());
      }
      if (9.105085757391269E-27d != ExtendedDecimal.FromString("91050857573912688994E-46").ToDouble()) {
        Assert.Fail("otherValue double 91050857573912688994E-46\nExpected: 9.105085757391269E-27d\nWas: " + ExtendedDecimal.FromString("91050857573912688994E-46").ToDouble());
      }
      if (Single.PositiveInfinity != ExtendedDecimal.FromString("73.895899E+102").ToSingle()) {
        Assert.Fail("otherValue single 73.895899E+102\nExpected: Single.PositiveInfinity\nWas: " + ExtendedDecimal.FromString("73.895899E+102").ToSingle());
      }
      if (7.3895899E103d != ExtendedDecimal.FromString("73.895899E+102").ToDouble()) {
        Assert.Fail("otherValue double 73.895899E+102\nExpected: 7.3895899E103d\nWas: " + ExtendedDecimal.FromString("73.895899E+102").ToDouble());
      }
      if (Single.NegativeInfinity != ExtendedDecimal.FromString("-796808893178.891470585829021E+330").ToSingle()) {
        Assert.Fail("otherValue single -796808893178.891470585829021E+330\nExpected: Single.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-796808893178.891470585829021E+330").ToSingle());
      }
      if (Double.NegativeInfinity != ExtendedDecimal.FromString("-796808893178.891470585829021E+330").ToDouble()) {
        Assert.Fail("otherValue double -796808893178.891470585829021E+330\nExpected: Double.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-796808893178.891470585829021E+330").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("275081E-206").ToSingle()) {
        Assert.Fail("otherValue single 275081E-206\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("275081E-206").ToSingle());
      }
      if (2.75081E-201d != ExtendedDecimal.FromString("275081E-206").ToDouble()) {
        Assert.Fail("otherValue double 275081E-206\nExpected: 2.75081E-201d\nWas: " + ExtendedDecimal.FromString("275081E-206").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-4322898910615499.82096E-95").ToSingle()) {
        Assert.Fail("otherValue single -4322898910615499.82096E-95\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-4322898910615499.82096E-95").ToSingle());
      }
      if (-4.3228989106155E-80d != ExtendedDecimal.FromString("-4322898910615499.82096E-95").ToDouble()) {
        Assert.Fail("otherValue double -4322898910615499.82096E-95\nExpected: -4.3228989106155E-80d\nWas: " + ExtendedDecimal.FromString("-4322898910615499.82096E-95").ToDouble());
      }
      if (Single.PositiveInfinity != ExtendedDecimal.FromString("143343913109764E+63").ToSingle()) {
        Assert.Fail("otherValue single 143343913109764E+63\nExpected: Single.PositiveInfinity\nWas: " + ExtendedDecimal.FromString("143343913109764E+63").ToSingle());
      }
      if (1.43343913109764E77d != ExtendedDecimal.FromString("143343913109764E+63").ToDouble()) {
        Assert.Fail("otherValue double 143343913109764E+63\nExpected: 1.43343913109764E77d\nWas: " + ExtendedDecimal.FromString("143343913109764E+63").ToDouble());
      }
      if (-7.9102981E16f != ExtendedDecimal.FromString("-79102983237104015").ToSingle()) {
        Assert.Fail("otherValue single -79102983237104015\nExpected: -7.9102981E16f\nWas: " + ExtendedDecimal.FromString("-79102983237104015").ToSingle());
      }
      if (-7.9102983237104016E16d != ExtendedDecimal.FromString("-79102983237104015").ToDouble()) {
        Assert.Fail("otherValue double -79102983237104015\nExpected: -7.9102983237104016E16d\nWas: " + ExtendedDecimal.FromString("-79102983237104015").ToDouble());
      }
      if (-9.07E-10f != ExtendedDecimal.FromString("-907E-12").ToSingle()) {
        Assert.Fail("otherValue single -907E-12\nExpected: -9.07E-10f\nWas: " + ExtendedDecimal.FromString("-907E-12").ToSingle());
      }
      if (-9.07E-10d != ExtendedDecimal.FromString("-907E-12").ToDouble()) {
        Assert.Fail("otherValue double -907E-12\nExpected: -9.07E-10d\nWas: " + ExtendedDecimal.FromString("-907E-12").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("191682103431.217475E-84").ToSingle()) {
        Assert.Fail("otherValue single 191682103431.217475E-84\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("191682103431.217475E-84").ToSingle());
      }
      if (1.9168210343121748E-73d != ExtendedDecimal.FromString("191682103431.217475E-84").ToDouble()) {
        Assert.Fail("otherValue double 191682103431.217475E-84\nExpected: 1.9168210343121748E-73d\nWas: " + ExtendedDecimal.FromString("191682103431.217475E-84").ToDouble());
      }
      if (-5.6E-45f != ExtendedDecimal.FromString("-492913.1840948615992120438E-50").ToSingle()) {
        Assert.Fail("otherValue single -492913.1840948615992120438E-50\nExpected: -5.6E-45f\nWas: " + ExtendedDecimal.FromString("-492913.1840948615992120438E-50").ToSingle());
      }
      if (-4.929131840948616E-45d != ExtendedDecimal.FromString("-492913.1840948615992120438E-50").ToDouble()) {
        Assert.Fail("otherValue double -492913.1840948615992120438E-50\nExpected: -4.929131840948616E-45d\nWas: " + ExtendedDecimal.FromString("-492913.1840948615992120438E-50").ToDouble());
      }
      if (Single.NegativeInfinity != ExtendedDecimal.FromString("-752873150058767E+272").ToSingle()) {
        Assert.Fail("otherValue single -752873150058767E+272\nExpected: Single.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-752873150058767E+272").ToSingle());
      }
      if (-7.52873150058767E286d != ExtendedDecimal.FromString("-752873150058767E+272").ToDouble()) {
        Assert.Fail("otherValue double -752873150058767E+272\nExpected: -7.52873150058767E286d\nWas: " + ExtendedDecimal.FromString("-752873150058767E+272").ToDouble());
      }
      if (27.311937f != ExtendedDecimal.FromString("27.311937404").ToSingle()) {
        Assert.Fail("otherValue single 27.311937404\nExpected: 27.311937f\nWas: " + ExtendedDecimal.FromString("27.311937404").ToSingle());
      }
      if (27.311937404d != ExtendedDecimal.FromString("27.311937404").ToDouble()) {
        Assert.Fail("otherValue double 27.311937404\nExpected: 27.311937404d\nWas: " + ExtendedDecimal.FromString("27.311937404").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("39147083343918E-143").ToSingle()) {
        Assert.Fail("otherValue single 39147083343918E-143\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("39147083343918E-143").ToSingle());
      }
      if (3.9147083343918E-130d != ExtendedDecimal.FromString("39147083343918E-143").ToDouble()) {
        Assert.Fail("otherValue double 39147083343918E-143\nExpected: 3.9147083343918E-130d\nWas: " + ExtendedDecimal.FromString("39147083343918E-143").ToDouble());
      }
      if (-1.97684019E11f != ExtendedDecimal.FromString("-197684018253").ToSingle()) {
        Assert.Fail("otherValue single -197684018253\nExpected: -1.97684019E11f\nWas: " + ExtendedDecimal.FromString("-197684018253").ToSingle());
      }
      if (-1.97684018253E11d != ExtendedDecimal.FromString("-197684018253").ToDouble()) {
        Assert.Fail("otherValue double -197684018253\nExpected: -1.97684018253E11d\nWas: " + ExtendedDecimal.FromString("-197684018253").ToDouble());
      }
      if (6.400822E14f != ExtendedDecimal.FromString("640082188903507").ToSingle()) {
        Assert.Fail("otherValue single 640082188903507\nExpected: 6.400822E14f\nWas: " + ExtendedDecimal.FromString("640082188903507").ToSingle());
      }
      if (6.40082188903507E14d != ExtendedDecimal.FromString("640082188903507").ToDouble()) {
        Assert.Fail("otherValue double 640082188903507\nExpected: 6.40082188903507E14d\nWas: " + ExtendedDecimal.FromString("640082188903507").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-913144352720144E-312").ToSingle()) {
        Assert.Fail("otherValue single -913144352720144E-312\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-913144352720144E-312").ToSingle());
      }
      if (-9.13144352720144E-298d != ExtendedDecimal.FromString("-913144352720144E-312").ToDouble()) {
        Assert.Fail("otherValue double -913144352720144E-312\nExpected: -9.13144352720144E-298d\nWas: " + ExtendedDecimal.FromString("-913144352720144E-312").ToDouble());
      }
      if (-3.68781005E15f != ExtendedDecimal.FromString("-3687809947210631").ToSingle()) {
        Assert.Fail("otherValue single -3687809947210631\nExpected: -3.68781005E15f\nWas: " + ExtendedDecimal.FromString("-3687809947210631").ToSingle());
      }
      if (-3.687809947210631E15d != ExtendedDecimal.FromString("-3687809947210631").ToDouble()) {
        Assert.Fail("otherValue double -3687809947210631\nExpected: -3.687809947210631E15d\nWas: " + ExtendedDecimal.FromString("-3687809947210631").ToDouble());
      }
      if (Single.PositiveInfinity != ExtendedDecimal.FromString("53083788630724917310.06236692262351E+169").ToSingle()) {
        Assert.Fail("otherValue single 53083788630724917310.06236692262351E+169\nExpected: Single.PositiveInfinity\nWas: " + ExtendedDecimal.FromString("53083788630724917310.06236692262351E+169").ToSingle());
      }
      if (5.3083788630724916E188d != ExtendedDecimal.FromString("53083788630724917310.06236692262351E+169").ToDouble()) {
        Assert.Fail("otherValue double 53083788630724917310.06236692262351E+169\nExpected: 5.3083788630724916E188d\nWas: " + ExtendedDecimal.FromString("53083788630724917310.06236692262351E+169").ToDouble());
      }
      if (-7.0943446E19f != ExtendedDecimal.FromString("-70943446332471357958").ToSingle()) {
        Assert.Fail("otherValue single -70943446332471357958\nExpected: -7.0943446E19f\nWas: " + ExtendedDecimal.FromString("-70943446332471357958").ToSingle());
      }
      if (-7.094344633247136E19d != ExtendedDecimal.FromString("-70943446332471357958").ToDouble()) {
        Assert.Fail("otherValue double -70943446332471357958\nExpected: -7.094344633247136E19d\nWas: " + ExtendedDecimal.FromString("-70943446332471357958").ToDouble());
      }
      if (63367.23f != ExtendedDecimal.FromString("63367.23157744207").ToSingle()) {
        Assert.Fail("otherValue single 63367.23157744207\nExpected: 63367.23f\nWas: " + ExtendedDecimal.FromString("63367.23157744207").ToSingle());
      }
      if (63367.23157744207d != ExtendedDecimal.FromString("63367.23157744207").ToDouble()) {
        Assert.Fail("otherValue double 63367.23157744207\nExpected: 63367.23157744207d\nWas: " + ExtendedDecimal.FromString("63367.23157744207").ToDouble());
      }
      if (Single.PositiveInfinity != ExtendedDecimal.FromString("2100535E+120").ToSingle()) {
        Assert.Fail("otherValue single 2100535E+120\nExpected: Single.PositiveInfinity\nWas: " + ExtendedDecimal.FromString("2100535E+120").ToSingle());
      }
      if (2.100535E126d != ExtendedDecimal.FromString("2100535E+120").ToDouble()) {
        Assert.Fail("otherValue double 2100535E+120\nExpected: 2.100535E126d\nWas: " + ExtendedDecimal.FromString("2100535E+120").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("914534543212037911E-174").ToSingle()) {
        Assert.Fail("otherValue single 914534543212037911E-174\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("914534543212037911E-174").ToSingle());
      }
      if (9.14534543212038E-157d != ExtendedDecimal.FromString("914534543212037911E-174").ToDouble()) {
        Assert.Fail("otherValue double 914534543212037911E-174\nExpected: 9.14534543212038E-157d\nWas: " + ExtendedDecimal.FromString("914534543212037911E-174").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-12437185743660570E-180").ToSingle()) {
        Assert.Fail("otherValue single -12437185743660570E-180\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-12437185743660570E-180").ToSingle());
      }
      if (-1.243718574366057E-164d != ExtendedDecimal.FromString("-12437185743660570E-180").ToDouble()) {
        Assert.Fail("otherValue double -12437185743660570E-180\nExpected: -1.243718574366057E-164d\nWas: " + ExtendedDecimal.FromString("-12437185743660570E-180").ToDouble());
      }
      if (-3.3723915E19f != ExtendedDecimal.FromString("-33723915695913879E+3").ToSingle()) {
        Assert.Fail("otherValue single -33723915695913879E+3\nExpected: -3.3723915E19f\nWas: " + ExtendedDecimal.FromString("-33723915695913879E+3").ToSingle());
      }
      if (-3.3723915695913878E19d != ExtendedDecimal.FromString("-33723915695913879E+3").ToDouble()) {
        Assert.Fail("otherValue double -33723915695913879E+3\nExpected: -3.3723915695913878E19d\nWas: " + ExtendedDecimal.FromString("-33723915695913879E+3").ToDouble());
      }
      if (6.3664833E10f != ExtendedDecimal.FromString("63664831787").ToSingle()) {
        Assert.Fail("otherValue single 63664831787\nExpected: 6.3664833E10f\nWas: " + ExtendedDecimal.FromString("63664831787").ToSingle());
      }
      if (6.3664831787E10d != ExtendedDecimal.FromString("63664831787").ToDouble()) {
        Assert.Fail("otherValue double 63664831787\nExpected: 6.3664831787E10d\nWas: " + ExtendedDecimal.FromString("63664831787").ToDouble());
      }
      if (Single.PositiveInfinity != ExtendedDecimal.FromString("432187105445201137.3321724908E+97").ToSingle()) {
        Assert.Fail("otherValue single 432187105445201137.3321724908E+97\nExpected: Single.PositiveInfinity\nWas: " + ExtendedDecimal.FromString("432187105445201137.3321724908E+97").ToSingle());
      }
      if (4.321871054452011E114d != ExtendedDecimal.FromString("432187105445201137.3321724908E+97").ToDouble()) {
        Assert.Fail("otherValue double 432187105445201137.3321724908E+97\nExpected: 4.321871054452011E114d\nWas: " + ExtendedDecimal.FromString("432187105445201137.3321724908E+97").ToDouble());
      }
      if (-5.1953271E13f != ExtendedDecimal.FromString("-51953270775979").ToSingle()) {
        Assert.Fail("otherValue single -51953270775979\nExpected: -5.1953271E13f\nWas: " + ExtendedDecimal.FromString("-51953270775979").ToSingle());
      }
      if (-5.1953270775979E13d != ExtendedDecimal.FromString("-51953270775979").ToDouble()) {
        Assert.Fail("otherValue double -51953270775979\nExpected: -5.1953270775979E13d\nWas: " + ExtendedDecimal.FromString("-51953270775979").ToDouble());
      }
      if (2.14953088E9f != ExtendedDecimal.FromString("2149530805").ToSingle()) {
        Assert.Fail("otherValue single 2149530805\nExpected: 2.14953088E9f\nWas: " + ExtendedDecimal.FromString("2149530805").ToSingle());
      }
      if (2.149530805E9d != ExtendedDecimal.FromString("2149530805").ToDouble()) {
        Assert.Fail("otherValue double 2149530805\nExpected: 2.149530805E9d\nWas: " + ExtendedDecimal.FromString("2149530805").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-4672759140.6362E-223").ToSingle()) {
        Assert.Fail("otherValue single -4672759140.6362E-223\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-4672759140.6362E-223").ToSingle());
      }
      if (-4.6727591406362E-214d != ExtendedDecimal.FromString("-4672759140.6362E-223").ToDouble()) {
        Assert.Fail("otherValue double -4672759140.6362E-223\nExpected: -4.6727591406362E-214d\nWas: " + ExtendedDecimal.FromString("-4672759140.6362E-223").ToDouble());
      }
      if (-9.0f != ExtendedDecimal.FromString("-9").ToSingle()) {
        Assert.Fail("otherValue single -9\nExpected: -9.0f\nWas: " + ExtendedDecimal.FromString("-9").ToSingle());
      }
      if (-9.0d != ExtendedDecimal.FromString("-9").ToDouble()) {
        Assert.Fail("otherValue double -9\nExpected: -9.0d\nWas: " + ExtendedDecimal.FromString("-9").ToDouble());
      }
      if (Single.NegativeInfinity != ExtendedDecimal.FromString("-1903960322936E+304").ToSingle()) {
        Assert.Fail("otherValue single -1903960322936E+304\nExpected: Single.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-1903960322936E+304").ToSingle());
      }
      if (Double.NegativeInfinity != ExtendedDecimal.FromString("-1903960322936E+304").ToDouble()) {
        Assert.Fail("otherValue double -1903960322936E+304\nExpected: Double.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-1903960322936E+304").ToDouble());
      }
      if (Single.PositiveInfinity != ExtendedDecimal.FromString("405766405417980707E+316").ToSingle()) {
        Assert.Fail("otherValue single 405766405417980707E+316\nExpected: Single.PositiveInfinity\nWas: " + ExtendedDecimal.FromString("405766405417980707E+316").ToSingle());
      }
      if (Double.PositiveInfinity != ExtendedDecimal.FromString("405766405417980707E+316").ToDouble()) {
        Assert.Fail("otherValue double 405766405417980707E+316\nExpected: Double.PositiveInfinity\nWas: " + ExtendedDecimal.FromString("405766405417980707E+316").ToDouble());
      }
      if (-166174.94f != ExtendedDecimal.FromString("-1661749343992047E-10").ToSingle()) {
        Assert.Fail("otherValue single -1661749343992047E-10\nExpected: -166174.94f\nWas: " + ExtendedDecimal.FromString("-1661749343992047E-10").ToSingle());
      }
      if (-166174.9343992047d != ExtendedDecimal.FromString("-1661749343992047E-10").ToDouble()) {
        Assert.Fail("otherValue double -1661749343992047E-10\nExpected: -166174.9343992047d\nWas: " + ExtendedDecimal.FromString("-1661749343992047E-10").ToDouble());
      }
      if (5893094.0f != ExtendedDecimal.FromString("5893094.099969899224047667").ToSingle()) {
        Assert.Fail("otherValue single 5893094.099969899224047667\nExpected: 5893094.0f\nWas: " + ExtendedDecimal.FromString("5893094.099969899224047667").ToSingle());
      }
      if (5893094.099969899d != ExtendedDecimal.FromString("5893094.099969899224047667").ToDouble()) {
        Assert.Fail("otherValue double 5893094.099969899224047667\nExpected: 5893094.099969899d\nWas: " + ExtendedDecimal.FromString("5893094.099969899224047667").ToDouble());
      }
      if (-3.4023195E17f != ExtendedDecimal.FromString("-340231946762317122").ToSingle()) {
        Assert.Fail("otherValue single -340231946762317122\nExpected: -3.4023195E17f\nWas: " + ExtendedDecimal.FromString("-340231946762317122").ToSingle());
      }
      if (-3.4023194676231712E17d != ExtendedDecimal.FromString("-340231946762317122").ToDouble()) {
        Assert.Fail("otherValue double -340231946762317122\nExpected: -3.4023194676231712E17d\nWas: " + ExtendedDecimal.FromString("-340231946762317122").ToDouble());
      }
      if (Single.PositiveInfinity != ExtendedDecimal.FromString("3.10041643978E+236").ToSingle()) {
        Assert.Fail("otherValue single 3.10041643978E+236\nExpected: Single.PositiveInfinity\nWas: " + ExtendedDecimal.FromString("3.10041643978E+236").ToSingle());
      }
      if (3.10041643978E236d != ExtendedDecimal.FromString("3.10041643978E+236").ToDouble()) {
        Assert.Fail("otherValue double 3.10041643978E+236\nExpected: 3.10041643978E236d\nWas: " + ExtendedDecimal.FromString("3.10041643978E+236").ToDouble());
      }
      if (1.43429217E13f != ExtendedDecimal.FromString("14342921940186").ToSingle()) {
        Assert.Fail("otherValue single 14342921940186\nExpected: 1.43429217E13f\nWas: " + ExtendedDecimal.FromString("14342921940186").ToSingle());
      }
      if (1.4342921940186E13d != ExtendedDecimal.FromString("14342921940186").ToDouble()) {
        Assert.Fail("otherValue double 14342921940186\nExpected: 1.4342921940186E13d\nWas: " + ExtendedDecimal.FromString("14342921940186").ToDouble());
      }
      if (1.97766234E9f != ExtendedDecimal.FromString("1977662368").ToSingle()) {
        Assert.Fail("otherValue single 1977662368\nExpected: 1.97766234E9f\nWas: " + ExtendedDecimal.FromString("1977662368").ToSingle());
      }
      if (1.977662368E9d != ExtendedDecimal.FromString("1977662368").ToDouble()) {
        Assert.Fail("otherValue double 1977662368\nExpected: 1.977662368E9d\nWas: " + ExtendedDecimal.FromString("1977662368").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("891.32009975058011674E-268").ToSingle()) {
        Assert.Fail("otherValue single 891.32009975058011674E-268\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("891.32009975058011674E-268").ToSingle());
      }
      if (8.913200997505801E-266d != ExtendedDecimal.FromString("891.32009975058011674E-268").ToDouble()) {
        Assert.Fail("otherValue double 891.32009975058011674E-268\nExpected: 8.913200997505801E-266d\nWas: " + ExtendedDecimal.FromString("891.32009975058011674E-268").ToDouble());
      }
      if (Single.NegativeInfinity != ExtendedDecimal.FromString("-895468936291.471679344983419E+316").ToSingle()) {
        Assert.Fail("otherValue single -895468936291.471679344983419E+316\nExpected: Single.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-895468936291.471679344983419E+316").ToSingle());
      }
      if (Double.NegativeInfinity != ExtendedDecimal.FromString("-895468936291.471679344983419E+316").ToDouble()) {
        Assert.Fail("otherValue double -895468936291.471679344983419E+316\nExpected: Double.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-895468936291.471679344983419E+316").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("61308E-104").ToSingle()) {
        Assert.Fail("otherValue single 61308E-104\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("61308E-104").ToSingle());
      }
      if (6.1308E-100d != ExtendedDecimal.FromString("61308E-104").ToDouble()) {
        Assert.Fail("otherValue double 61308E-104\nExpected: 6.1308E-100d\nWas: " + ExtendedDecimal.FromString("61308E-104").ToDouble());
      }
      if (-5362.791f != ExtendedDecimal.FromString("-5362.79122778669072").ToSingle()) {
        Assert.Fail("otherValue single -5362.79122778669072\nExpected: -5362.791f\nWas: " + ExtendedDecimal.FromString("-5362.79122778669072").ToSingle());
      }
      if (-5362.791227786691d != ExtendedDecimal.FromString("-5362.79122778669072").ToDouble()) {
        Assert.Fail("otherValue double -5362.79122778669072\nExpected: -5362.791227786691d\nWas: " + ExtendedDecimal.FromString("-5362.79122778669072").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("861664379590901308.23330613776542261919E-101").ToSingle()) {
        Assert.Fail("otherValue single 861664379590901308.23330613776542261919E-101\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("861664379590901308.23330613776542261919E-101").ToSingle());
      }
      if (8.616643795909013E-84d != ExtendedDecimal.FromString("861664379590901308.23330613776542261919E-101").ToDouble()) {
        Assert.Fail("otherValue double 861664379590901308.23330613776542261919E-101\nExpected: 8.616643795909013E-84d\nWas: " + ExtendedDecimal.FromString("861664379590901308.23330613776542261919E-101").ToDouble());
      }
      if (Single.NegativeInfinity != ExtendedDecimal.FromString("-1884773180.50192918329237967651E+204").ToSingle()) {
        Assert.Fail("otherValue single -1884773180.50192918329237967651E+204\nExpected: Single.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-1884773180.50192918329237967651E+204").ToSingle());
      }
      if (-1.884773180501929E213d != ExtendedDecimal.FromString("-1884773180.50192918329237967651E+204").ToDouble()) {
        Assert.Fail("otherValue double -1884773180.50192918329237967651E+204\nExpected: -1.884773180501929E213d\nWas: " + ExtendedDecimal.FromString("-1884773180.50192918329237967651E+204").ToDouble());
      }
      if (1.89187207E13f != ExtendedDecimal.FromString("18918720095123.6152").ToSingle()) {
        Assert.Fail("otherValue single 18918720095123.6152\nExpected: 1.89187207E13f\nWas: " + ExtendedDecimal.FromString("18918720095123.6152").ToSingle());
      }
      if (1.8918720095123613E13d != ExtendedDecimal.FromString("18918720095123.6152").ToDouble()) {
        Assert.Fail("otherValue double 18918720095123.6152\nExpected: 1.8918720095123613E13d\nWas: " + ExtendedDecimal.FromString("18918720095123.6152").ToDouble());
      }
      if (94667.95f != ExtendedDecimal.FromString("94667.95264211741602").ToSingle()) {
        Assert.Fail("otherValue single 94667.95264211741602\nExpected: 94667.95f\nWas: " + ExtendedDecimal.FromString("94667.95264211741602").ToSingle());
      }
      if (94667.95264211742d != ExtendedDecimal.FromString("94667.95264211741602").ToDouble()) {
        Assert.Fail("otherValue double 94667.95264211741602\nExpected: 94667.95264211742d\nWas: " + ExtendedDecimal.FromString("94667.95264211741602").ToDouble());
      }
      if (Single.PositiveInfinity != ExtendedDecimal.FromString("1230618521424E+134").ToSingle()) {
        Assert.Fail("otherValue single 1230618521424E+134\nExpected: Single.PositiveInfinity\nWas: " + ExtendedDecimal.FromString("1230618521424E+134").ToSingle());
      }
      if (1.230618521424E146d != ExtendedDecimal.FromString("1230618521424E+134").ToDouble()) {
        Assert.Fail("otherValue double 1230618521424E+134\nExpected: 1.230618521424E146d\nWas: " + ExtendedDecimal.FromString("1230618521424E+134").ToDouble());
      }
      if (Single.PositiveInfinity != ExtendedDecimal.FromString("3022403935588782E+85").ToSingle()) {
        Assert.Fail("otherValue single 3022403935588782E+85\nExpected: Single.PositiveInfinity\nWas: " + ExtendedDecimal.FromString("3022403935588782E+85").ToSingle());
      }
      if (3.022403935588782E100d != ExtendedDecimal.FromString("3022403935588782E+85").ToDouble()) {
        Assert.Fail("otherValue double 3022403935588782E+85\nExpected: 3.022403935588782E100d\nWas: " + ExtendedDecimal.FromString("3022403935588782E+85").ToDouble());
      }
      if (Single.PositiveInfinity != ExtendedDecimal.FromString("64543E+274").ToSingle()) {
        Assert.Fail("otherValue single 64543E+274\nExpected: Single.PositiveInfinity\nWas: " + ExtendedDecimal.FromString("64543E+274").ToSingle());
      }
      if (6.4543E278d != ExtendedDecimal.FromString("64543E+274").ToDouble()) {
        Assert.Fail("otherValue double 64543E+274\nExpected: 6.4543E278d\nWas: " + ExtendedDecimal.FromString("64543E+274").ToDouble());
      }
      if (6.7181355E10f != ExtendedDecimal.FromString("67181356837.903551518080873954").ToSingle()) {
        Assert.Fail("otherValue single 67181356837.903551518080873954\nExpected: 6.7181355E10f\nWas: " + ExtendedDecimal.FromString("67181356837.903551518080873954").ToSingle());
      }
      if (6.718135683790355E10d != ExtendedDecimal.FromString("67181356837.903551518080873954").ToDouble()) {
        Assert.Fail("otherValue double 67181356837.903551518080873954\nExpected: 6.718135683790355E10d\nWas: " + ExtendedDecimal.FromString("67181356837.903551518080873954").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-4508016E-321").ToSingle()) {
        Assert.Fail("otherValue single -4508016E-321\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-4508016E-321").ToSingle());
      }
      if (-4.508016E-315d != ExtendedDecimal.FromString("-4508016E-321").ToDouble()) {
        Assert.Fail("otherValue double -4508016E-321\nExpected: -4.508016E-315d\nWas: " + ExtendedDecimal.FromString("-4508016E-321").ToDouble());
      }
      if (Single.NegativeInfinity != ExtendedDecimal.FromString("-62855032520.512452348497E+39").ToSingle()) {
        Assert.Fail("otherValue single -62855032520.512452348497E+39\nExpected: Single.NegativeInfinity\nWas: " + ExtendedDecimal.FromString("-62855032520.512452348497E+39").ToSingle());
      }
      if (-6.285503252051245E49d != ExtendedDecimal.FromString("-62855032520.512452348497E+39").ToDouble()) {
        Assert.Fail("otherValue double -62855032520.512452348497E+39\nExpected: -6.285503252051245E49d\nWas: " + ExtendedDecimal.FromString("-62855032520.512452348497E+39").ToDouble());
      }
      if (3177.2236f != ExtendedDecimal.FromString("3177.2237286").ToSingle()) {
        Assert.Fail("otherValue single 3177.2237286\nExpected: 3177.2236f\nWas: " + ExtendedDecimal.FromString("3177.2237286").ToSingle());
      }
      if (3177.2237286d != ExtendedDecimal.FromString("3177.2237286").ToDouble()) {
        Assert.Fail("otherValue double 3177.2237286\nExpected: 3177.2237286d\nWas: " + ExtendedDecimal.FromString("3177.2237286").ToDouble());
      }
      if (-7.950583E8f != ExtendedDecimal.FromString("-795058316.9186492185346968").ToSingle()) {
        Assert.Fail("otherValue single -795058316.9186492185346968\nExpected: -7.950583E8f\nWas: " + ExtendedDecimal.FromString("-795058316.9186492185346968").ToSingle());
      }
      if (-7.950583169186492E8d != ExtendedDecimal.FromString("-795058316.9186492185346968").ToDouble()) {
        Assert.Fail("otherValue double -795058316.9186492185346968\nExpected: -7.950583169186492E8d\nWas: " + ExtendedDecimal.FromString("-795058316.9186492185346968").ToDouble());
      }
    }

    [Test]
    public void TestCanFitInSpecificCases() {
      Assert.IsFalse(CBORObject.FromObject(2554895343).CanFitInSingle());
    }

    [Test]
    public void TestCanFitIn() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 1000; ++i) {
        CBORObject ed = RandomNumber(r);
        ExtendedDecimal ed2;
        ed2 = ExtendedDecimal.FromDouble(ed.AsExtendedDecimal().ToDouble());
        if ((ed.AsExtendedDecimal().CompareTo(ed2) == 0) != ed.CanFitInDouble()) {
          Assert.Fail(ToByteArrayString(ed) + "; /" + "/ " + ed.ToJSONString());
        }
        ed2 = ExtendedDecimal.FromSingle(ed.AsExtendedDecimal().ToSingle());
        if ((ed.AsExtendedDecimal().CompareTo(ed2) == 0) != ed.CanFitInSingle()) {
          Assert.Fail(ToByteArrayString(ed) + "; /" + "/ " + ed.ToJSONString());
        }
        if (!ed.IsInfinity() && !ed.IsNaN()) {
          ed2 = ExtendedDecimal.FromBigInteger(ed.AsExtendedDecimal().ToBigInteger());
          if ((ed.AsExtendedDecimal().CompareTo(ed2) == 0) != ed.IsIntegral) {
            Assert.Fail(ToByteArrayString(ed) + "; /" + "/ " + ed.ToJSONString());
          }
        }
        if (!ed.IsInfinity() && !ed.IsNaN()) {
          BigInteger bi = ed.AsBigInteger();
          if (ed.IsIntegral) {
            if (bi.canFitInInt() != ed.CanFitInInt32()) {
              Assert.Fail(ToByteArrayString(ed) + "; /" + "/ " + ed.ToJSONString());
            }
          }
          if (bi.canFitInInt() != ed.CanTruncatedIntFitInInt32()) {
            Assert.Fail(ToByteArrayString(ed) + "; /" + "/ " + ed.ToJSONString());
          }
          if (ed.IsIntegral) {
            if ((bi.bitLength() <= 63) != ed.CanFitInInt64()) {
              Assert.Fail(ToByteArrayString(ed) + "; /" + "/ " + ed.ToJSONString());
            }
          }
          if ((bi.bitLength() <= 63) != ed.CanTruncatedIntFitInInt64()) {
            Assert.Fail(ToByteArrayString(ed) + "; /" + "/ " + ed.ToJSONString());
          }
        }
      }
    }

    /// <summary>Not documented yet.</summary>
    [Test]
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
    [Test]
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

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestDoubleToOther() {
      CBORObject dbl1 = CBORObject.FromObject((double)Int32.MinValue);
      CBORObject dbl2 = CBORObject.FromObject((double)Int32.MaxValue);
      try {
        dbl1.AsInt16(); Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        dbl1.AsByte(); Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.IsTrue(dbl1.CanFitInInt32());
        dbl1.AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.IsTrue(dbl1.CanFitInInt64());
        dbl1.AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        dbl1.AsBigInteger();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        dbl2.AsInt16(); Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        dbl2.AsByte(); Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        dbl2.AsInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        dbl2.AsInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        dbl2.AsBigInteger();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestBigTag() {
      CBORObject.FromObjectAndTag(CBORObject.Null, (BigInteger.One << 64) - BigInteger.One);
    }
    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestDecimalFracExactlyTwoElements() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0xc4,
        0x82,
        0xc2,
        0x41,
        1 });
    }

    /// <summary>Not documented yet.</summary>
    [Test]
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

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestShort() {
      for (int i = Int16.MinValue; i <= Int16.MaxValue; ++i) {
        TestCommon.AssertSer(
          CBORObject.FromObject((short)i),
          String.Format(CultureInfo.InvariantCulture, "{0}", i));
      }
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestByteArray() {
      TestCommon.AssertSer(
        CBORObject.FromObject(new byte[] { 0x20, 0x78 }),
        "h'2078'");
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestBigNumBytes() {
      CBORObject o = null;
      o = TestCommon.FromBytesTestAB(new byte[] { 0xc2, 0x41, 0x88 });
      Assert.AreEqual((BigInteger)0x88L, o.AsBigInteger());
      o = TestCommon.FromBytesTestAB(new byte[] { 0xc2, 0x42, 0x88, 0x77 });
      Assert.AreEqual((BigInteger)0x8877L, o.AsBigInteger());
      o = TestCommon.FromBytesTestAB(new byte[] { 0xc2, 0x44, 0x88, 0x77, 0x66, 0x55 });
      Assert.AreEqual((BigInteger)0x88776655L, o.AsBigInteger());
      o = TestCommon.FromBytesTestAB(new byte[] { 0xc2, 0x47, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22 });
      Assert.AreEqual((BigInteger)0x88776655443322L, o.AsBigInteger());
    }

    [Test]
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

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestTaggedUntagged() {
      for (int i = 200; i < 1000; ++i) {
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

    public void AssertBigIntString(string s, BigInteger bi) {
      Assert.AreEqual(s, bi.ToString());
    }

    [Test]
    public void TestCBORBigInteger() {
      CBORObject o = CBORObject.DecodeFromBytes(new byte[] { 0x3B, (byte)0xCE, (byte)0xE2, 0x5A, 0x57, (byte)0xD8, 0x21, (byte)0xB9, (byte)0xA7 });
      Assert.AreEqual(BigInteger.fromString("-14907577049884506536"), o.AsBigInteger());
    }

    public void AssertAdd(BigInteger bi, BigInteger bi2, string s) {
      this.AssertBigIntString(s, bi + (BigInteger)bi2);
      this.AssertBigIntString(s, bi2 + (BigInteger)bi);
      BigInteger negbi = BigInteger.Zero - (BigInteger)bi;
      BigInteger negbi2 = BigInteger.Zero - (BigInteger)bi2;
      this.AssertBigIntString(s, bi - (BigInteger)negbi2);
      this.AssertBigIntString(s, bi2 - (BigInteger)negbi);
    }

    [Test]
    public void TestBigIntAddSub() {
      BigInteger posSmall = (BigInteger)5;
      BigInteger negSmall = -(BigInteger)5;
      BigInteger posLarge = (BigInteger)5555555;
      BigInteger negLarge = -(BigInteger)5555555;
      this.AssertAdd(posSmall, posSmall, "10");
      this.AssertAdd(posSmall, negSmall, "0");
      this.AssertAdd(posSmall, posLarge, "5555560");
      this.AssertAdd(posSmall, negLarge, "-5555550");
      this.AssertAdd(negSmall, negSmall, "-10");
      this.AssertAdd(negSmall, posLarge, "5555550");
      this.AssertAdd(negSmall, negLarge, "-5555560");
      this.AssertAdd(posLarge, posLarge, "11111110");
      this.AssertAdd(posLarge, negLarge, "0");
      this.AssertAdd(negLarge, negLarge, "-11111110");
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestBigInteger() {
      BigInteger bi = (BigInteger)3;
      this.AssertBigIntString("3", bi);
      BigInteger negseven = (BigInteger)(-7);
      this.AssertBigIntString("-7", negseven);
      BigInteger other = (BigInteger)(-898989);
      this.AssertBigIntString("-898989", other);
      other = (BigInteger)898989;
      this.AssertBigIntString("898989", other);
      for (int i = 0; i < 500; ++i) {
        TestCommon.AssertSer(
          CBORObject.FromObject(bi),
          String.Format(CultureInfo.InvariantCulture, "{0}", bi));
        Assert.IsTrue(CBORObject.FromObject(bi).IsIntegral);
        TestCommon.AssertRoundTrip(CBORObject.FromObject(bi));
        TestCommon.AssertRoundTrip(CBORObject.FromObject(ExtendedDecimal.Create(bi, BigInteger.One)));
        bi *= (BigInteger)negseven;
      }
      BigInteger[] ranges = new BigInteger[] {
        (BigInteger)Int64.MinValue - (BigInteger)512,
        (BigInteger)Int64.MinValue + (BigInteger)512,
        BigInteger.Zero - (BigInteger)512,
        BigInteger.Zero + (BigInteger)512,
        (BigInteger)Int64.MaxValue - (BigInteger)512,
        (BigInteger)Int64.MaxValue + (BigInteger)512,
        ((BigInteger.One << 64) - BigInteger.One) - (BigInteger)512,
        ((BigInteger.One << 64) - BigInteger.One) + (BigInteger)512,
      };
      for (int i = 0; i < ranges.Length; i += 2) {
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

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestLong() {
      long[] ranges = new long[] {
        -65539, 65539,
        0xFFFFF000L, 0x100000400L,
        Int64.MaxValue - 1000, Int64.MaxValue,
        Int64.MinValue, Int64.MinValue + 1000
      };
      for (int i = 0; i < ranges.Length; i += 2) {
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

    /// <summary>Not documented yet.</summary>
    [Test]
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
          String.Format(CultureInfo.InvariantCulture, "{0}", i));
      }
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestCodePointCompare() {
      Assert.AreEqual(0, Math.Sign(DataUtilities.CodePointCompare("abc", "abc")));
      Assert.AreEqual(0, Math.Sign(DataUtilities.CodePointCompare("\ud800\udc00", "\ud800\udc00")));
      Assert.AreEqual(-1, Math.Sign(DataUtilities.CodePointCompare("abc", "\ud800\udc00")));
      Assert.AreEqual(-1, Math.Sign(DataUtilities.CodePointCompare("\uf000", "\ud800\udc00")));
      Assert.AreEqual(1, Math.Sign(DataUtilities.CodePointCompare("\uf000", "\ud800")));
    }

    /// <summary>Not documented yet.</summary>
    [Test]
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

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestGetUtf8Length() {
      try {
        DataUtilities.GetUtf8Length(null, true);
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Length(null, false);
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(3, DataUtilities.GetUtf8Length("abc", true));
      Assert.AreEqual(4, DataUtilities.GetUtf8Length("\u0300\u0300", true));
      Assert.AreEqual(6, DataUtilities.GetUtf8Length("\u3000\u3000", true));
      Assert.AreEqual(6, DataUtilities.GetUtf8Length("\ud800\ud800", true));
      Assert.AreEqual(-1, DataUtilities.GetUtf8Length("\ud800\ud800", false));
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestDouble() {
      if (!CBORObject.FromObject(Double.PositiveInfinity).IsPositiveInfinity()) {
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
          String.Format(CultureInfo.InvariantCulture, "{0}", i));
        if (oldobj != null) {
          CompareTestLess(oldobj, o);
        }
        oldobj = o;
      }
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void TestTags() {
      BigInteger maxuint = (BigInteger.One << 64) - BigInteger.One;
      BigInteger[] ranges = new BigInteger[] {
        (BigInteger)6,
        (BigInteger)65539,
        (BigInteger)Int32.MaxValue - (BigInteger)500,
        (BigInteger)Int32.MaxValue + (BigInteger)500,
        (BigInteger)Int64.MaxValue - (BigInteger)500,
        (BigInteger)Int64.MaxValue + (BigInteger)500,
        ((BigInteger.One << 64) - BigInteger.One) - (BigInteger)500,
        maxuint,
      };
      Assert.IsFalse(CBORObject.True.IsTagged);
      Assert.AreEqual(BigInteger.Zero, CBORObject.True.InnermostTag);
      BigInteger[] tagstmp = CBORObject.True.GetTags();
      Assert.AreEqual(0, tagstmp.Length);
      for (int i = 0; i < ranges.Length; i += 2) {
        BigInteger bigintTemp = ranges[i];
        while (true) {
          if (bigintTemp.Equals((BigInteger)30) ||
              bigintTemp.Equals((BigInteger)29)) {
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
              String.Format(CultureInfo.InvariantCulture, "obj tag doesn't match: {0}", obj));
          }
          TestCommon.AssertSer(
            obj,
            String.Format(CultureInfo.InvariantCulture, "{0}(0)", bigintTemp));
          if (!bigintTemp.Equals(maxuint)) {
            // Test multiple tags
            CBORObject obj2 = CBORObject.FromObjectAndTag(obj, bigintTemp + BigInteger.One);
            BigInteger[] bi = obj2.GetTags();
            if (bi.Length != 2) {
              Assert.AreEqual(
                2,
                bi.Length,
                String.Format(CultureInfo.InvariantCulture, "Expected 2 tags: {0}", obj2));
            }
            if (!bi[0].Equals((BigInteger)bigintTemp + BigInteger.One)) {
              Assert.AreEqual(
                bigintTemp + BigInteger.One,
                bi[0],
                String.Format(CultureInfo.InvariantCulture, "Outer tag doesn't match: {0}", obj2));
            }
            if (!bi[1].Equals((BigInteger)bigintTemp)) {
              Assert.AreEqual(
                bigintTemp,
                bi[1],
                String.Format(CultureInfo.InvariantCulture, "Inner tag doesn't match: {0}", obj2));
            }
            if (!obj2.InnermostTag.Equals((BigInteger)bigintTemp)) {
              Assert.AreEqual(
                bigintTemp,
                obj2.InnermostTag,
                String.Format(CultureInfo.InvariantCulture, "Innermost tag doesn't match: {0}", obj2));
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
