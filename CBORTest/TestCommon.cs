/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;
using NUnit.Framework;
using System.Globalization;
//using System.Numerics;
using PeterO;
using System.IO;
namespace Test {
  static class TestCommon {

    public static void AssertBigIntegersEqual(string a, BigInteger b){
      Assert.AreEqual(a,b.ToString());
      BigInteger a2=BigInteger.fromString(a);
      Assert.AreEqual(a2,b);
      AssertEqualsHashCode(a2,b);
    }
    public static void DoTestDivide(string dividend, string divisor, string result){
      BigInteger bigintA=BigInteger.fromString(dividend);
      BigInteger bigintB=BigInteger.fromString(divisor);
      if(bigintB.IsZero){
        try { bigintA.divide(bigintB); Assert.Fail("Expected divide by 0 error");
        } catch(Exception){ }
      } else {
        AssertBigIntegersEqual(result,bigintA.divide(bigintB));
      }
    }
    public static void DoTestRemainder(string dividend, string divisor, string result){
      BigInteger bigintA=BigInteger.fromString(dividend);
      BigInteger bigintB=BigInteger.fromString(divisor);
      if(bigintB.IsZero){
        try { bigintA.remainder(bigintB); Assert.Fail("Expected divide by 0 error");
        } catch(Exception){ }
      } else {
        AssertBigIntegersEqual(result,(bigintA.remainder(bigintB)));
      }
    }
    public static void DoTestDivideAndRemainder(string dividend, string divisor, string result, string rem){
      BigInteger bigintA=BigInteger.fromString(dividend);
      BigInteger bigintB=BigInteger.fromString(divisor);
      BigInteger rembi;
      if(bigintB.IsZero){
        try {
          BigInteger quo=BigInteger.DivRem(bigintA,bigintB,out rembi);
          Assert.Fail("Expected divide by 0 error");
        } catch(Exception){ }
      } else {
        BigInteger quo=BigInteger.DivRem(bigintA,bigintB,out rembi);
        AssertBigIntegersEqual(result,quo);
        AssertBigIntegersEqual(rem,rembi);
      }
    }
    public static void DoTestMultiply(string m1, string m2, string result){
      BigInteger bigintA=BigInteger.fromString(m1);
      BigInteger bigintB=BigInteger.fromString(m2);
      AssertBigIntegersEqual(result,(bigintA.multiply(bigintB)));
    }
    public static void DoTestAdd(string m1, string m2, string result){
      BigInteger bigintA=BigInteger.fromString(m1);
      BigInteger bigintB=BigInteger.fromString(m2);
      AssertBigIntegersEqual(result,(bigintA.add(bigintB)));
    }
    public static void DoTestSubtract(string m1, string m2, string result){
      BigInteger bigintA=BigInteger.fromString(m1);
      BigInteger bigintB=BigInteger.fromString(m2);
      AssertBigIntegersEqual(result,(bigintA.subtract(bigintB)));
    }
    public static void DoTestPow(string m1, int m2, string result){
      BigInteger bigintA=BigInteger.fromString(m1);
      AssertBigIntegersEqual(result,(bigintA.pow(m2)));
    }
    public static void DoTestShiftLeft(string m1, int m2, string result){
      BigInteger bigintA=BigInteger.fromString(m1);
      AssertBigIntegersEqual(result,(bigintA.shiftLeft(m2)));
      AssertBigIntegersEqual(result,(bigintA.shiftRight(-m2)));
    }
    public static void DoTestShiftRight(string m1, int m2, string result){
      BigInteger bigintA=BigInteger.fromString(m1);
      AssertBigIntegersEqual(result,(bigintA.shiftRight(m2)));
      AssertBigIntegersEqual(result,(bigintA.shiftLeft(-m2)));
    }

    public static void AssertDecFrac(ExtendedDecimal d3, string output){
      if(output==null && d3!=null)Assert.Fail("d3 must be null");
      if(output!=null && !d3.ToString().Equals(output)){
        ExtendedDecimal d4=ExtendedDecimal.FromString(output);
        Assert.AreEqual(output,d3.ToString(),(
          "expected: ["+(d4.UnsignedMantissa).ToString()+","+(d4.Exponent).ToString()+"]\\n"+
          "but was: ["+(d3.UnsignedMantissa).ToString()+","+(d3.Exponent).ToString()+"]"
         ));   }
    }

    public static BigInteger BigIntParse(string str){
      return BigInteger.fromString(str);
    }

    public static void AssertFlags(int expected, int actual){
      if(expected==actual)return;
      Assert.AreEqual((expected&PrecisionContext.FlagInexact)!=0,
                      (actual&PrecisionContext.FlagInexact)!=0,"Inexact");
      Assert.AreEqual((expected&PrecisionContext.FlagRounded)!=0,
                      (actual&PrecisionContext.FlagRounded)!=0,"Rounded");
      Assert.AreEqual((expected&PrecisionContext.FlagSubnormal)!=0,
                      (actual&PrecisionContext.FlagSubnormal)!=0,"Subnormal");
      Assert.AreEqual((expected&PrecisionContext.FlagOverflow)!=0,
                      (actual&PrecisionContext.FlagOverflow)!=0,"Overflow");
      Assert.AreEqual((expected&PrecisionContext.FlagUnderflow)!=0,
                      (actual&PrecisionContext.FlagUnderflow)!=0,"Underflow");
      Assert.AreEqual((expected&PrecisionContext.FlagClamped)!=0,
                      (actual&PrecisionContext.FlagClamped)!=0,"Clamped");
      Assert.AreEqual((expected&PrecisionContext.FlagInvalid)!=0,
                      (actual&PrecisionContext.FlagInvalid)!=0,"Invalid");
      Assert.AreEqual((expected&PrecisionContext.FlagDivideByZero)!=0,
                      (actual&PrecisionContext.FlagDivideByZero)!=0,"DivideByZero");
    }

    private static CBORObject FromBytesA(byte[] b) {
      return CBORObject.DecodeFromBytes(b);
    }
    private static CBORObject FromBytesB(byte[] b) {
      using (MemoryStream ms = new MemoryStream(b)) {
        CBORObject o = CBORObject.Read(ms);
        if (ms.Position != ms.Length)
          throw new CBORException("not at EOF");
        return o;
      }
    }
    //
    //  Tests the equivalence of the FromBytes and Read methods.
    //
    public static CBORObject FromBytesTestAB(byte[] b) {
      CBORObject oa = FromBytesA(b);
      CBORObject ob = FromBytesB(b);
      if (!oa.Equals(ob))
        Assert.AreEqual(oa, ob);
      return oa;
    }
    public static void AssertEqualsHashCode(Object o, Object o2) {
      if (o.Equals(o2)) {
        if (!o2.Equals(o))
          Assert.Fail(
            String.Format(CultureInfo.InvariantCulture,
                          "{0} equals {1}, but not vice versa", o, o2));
        // Test for the guarantee that equal objects
        // must have equal hash codes
        if (o2.GetHashCode() != o.GetHashCode()) {
          // Don't use Assert.AreEqual directly because it has
          // quite a lot of overhead
          Assert.Fail(
            String.Format(CultureInfo.InvariantCulture,
                          "{0} and {1} don't have equal hash codes", o, o2));
        }
      } else {
        if (o2.Equals(o))
          Assert.Fail(
            String.Format(CultureInfo.InvariantCulture,
                          "{0} does not equal {1}, but not vice versa", o, o2));
      }
    }

    public static void TestNumber(CBORObject o){
      if(o.Type!= CBORType.Number){
        return;
      }
      if(o.IsPositiveInfinity() || o.IsNegativeInfinity() ||
         o.IsNaN()){
        try { o.AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail("Object: "+o+", "+ex.ToString()); throw; }
        try { o.AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail("Object: "+o+", "+ex.ToString()); throw; }
        try { o.AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail("Object: "+o+", "+ex.ToString()); throw; }
        try { o.AsInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail("Object: "+o+", "+ex.ToString()); throw; }
        try { o.AsSingle(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail("Object: "+o+", "+ex.ToString()); throw; }
        try { o.AsDouble(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail("Object: "+o+", "+ex.ToString()); throw; }
        try { o.AsBigInteger(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail("Object: "+o+", "+ex.ToString()); throw; }
        return;
      }
      BigInteger df=o.AsExtendedDecimal().ToBigInteger();
      try { o.AsBigInteger(); } catch(Exception ex){ Assert.Fail("Object: "+o+", int: "+df+", "+ex.ToString()); throw; }
      try { o.AsSingle(); } catch(Exception ex){ Assert.Fail("Object: "+o+", int: "+df+", "+ex.ToString()); throw; }
      try { o.AsDouble(); } catch(Exception ex){ Assert.Fail("Object: "+o+", int: "+df+", "+ex.ToString()); throw; }
    }

    public static void AssertRoundTrip(CBORObject o) {
      CBORObject o2 = FromBytesTestAB(o.EncodeToBytes());
      if (o2.Type == CBORType.Map && o.Type == CBORType.Map) {
        // Skip because key order may be different
      } else {
        if (!o.ToString().Equals(o2.ToString()))
          Assert.AreEqual(o.ToString(), o2.ToString(), "o2 is not equal to o");
      }
      TestNumber(o);
      AssertEqualsHashCode(o, o2);
    }
    public static void AssertSer(CBORObject o, String s) {
      if (!s.Equals(o.ToString()))
        Assert.AreEqual(s, o.ToString(), "o is not equal to s");
      // Test round-tripping
      CBORObject o2 = FromBytesTestAB(o.EncodeToBytes());
      if (!s.Equals(o2.ToString()))
        Assert.AreEqual(s, o2.ToString(), "o2 is not equal to s");
      TestNumber(o);
      AssertEqualsHashCode(o, o2);
    }
  }
}
