/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using NUnit.Framework;
using System.Globalization;
//using System.Numerics;
using PeterO;
using System.IO;
namespace Test {
  static class TestCommon {
    
    private sealed class MutableBigInteger {
      int[] data;
      int length;
      public MutableBigInteger() {
        data = new int[2];
        length = 1;
        data[0] = 0;
      }
      private static byte[] ReverseBytes(byte[] bytes) {
        if ((bytes) == null) throw new ArgumentNullException("bytes");
        int half = bytes.Length >> 1;
        int right = bytes.Length - 1;
        for (int i = 0; i < half; i++, right--) {
          byte value = bytes[i];
          bytes[i] = bytes[right];
          bytes[right] = value;
        }
        return bytes;
      }
    /// <summary> </summary>
    /// <returns></returns>
    /// <remarks/>
      public BigInteger ToBigInteger() {
        byte[] bytes = new byte[length * 4 + 1];
        for (int i = 0; i < length; i++) {
          bytes[i * 4 + 0] = (byte)((data[i]) & 0xFF);
          bytes[i * 4 + 1] = (byte)((data[i] >> 8) & 0xFF);
          bytes[i * 4 + 2] = (byte)((data[i] >> 16) & 0xFF);
          bytes[i * 4 + 3] = (byte)((data[i] >> 24) & 0xFF);
        }
        bytes[bytes.Length - 1] = 0;
        return new BigInteger((byte[])bytes);
      }

    /// <summary> Converts this object to a text string.</summary>
    /// <returns> A string representation of this object.</returns>
    /// <remarks/>
      public override string ToString() {
        return ToBigInteger().ToString();
      }

    /// <summary> Multiplies this instance by the value of a Int32 object.</summary>
    /// <param name='multiplicand'> A 32-bit signed integer.</param>
    /// <returns> The product of the two objects.</returns>
    /// <remarks/>
      public MutableBigInteger Multiply(int multiplicand) {
        if (multiplicand < 0)
          throw new ArgumentException("Only positive multiplicands are supported");
        else if (multiplicand != 0) {
          int carry = 0;
          for (int i = 0; i < length; i++) {
            long subproduct = ((long)data[i]) & 0xFFFFFFFFL;
            subproduct *= multiplicand;
            subproduct += carry;
            carry = unchecked((int)((subproduct >> 32) & 0xFFFFFFFFL));
            data[i] = unchecked((int)((subproduct) & 0xFFFFFFFFL));
          }
          if (carry != 0) {
            if (length >= data.Length) {
              int[] newdata = new int[length + 20];
              Array.Copy(data, 0, newdata, 0, data.Length);
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
      
    /// <summary> </summary>
    /// <param name='augend'> A 32-bit signed integer.</param>
    /// <returns></returns>
    /// <remarks/>
      public MutableBigInteger Add(int augend) {
        if (augend < 0)
          throw new ArgumentException("Only positive augends are supported");
        else if (augend != 0) {
          int carry = 0;
          for (int i = 0; i < length; i++) {
            long subproduct = ((long)data[i]) & 0xFFFFFFFFL;
            subproduct += augend;
            subproduct += carry;
            carry = unchecked((int)((subproduct >> 32) & 0xFFFFFFFFL));
            data[i] = unchecked((int)((subproduct) & 0xFFFFFFFFL));
            if (carry == 0) return this;
            augend = 0;
          }
          if (carry != 0) {
            if (length >= data.Length) {
              int[] newdata = new int[length + 20];
              Array.Copy(data, 0, newdata, 0, data.Length);
              data = newdata;
            }
            data[length] = carry;
            length++;
          }
        }
        return this;
      }
    }

    public static void AssertDecFrac(DecimalFraction d3, string output){
      if(output==null && d3!=null)Assert.Fail("d3 must be null");
      if(output!=null && !d3.ToString().Equals(output)){
        DecimalFraction d4=DecimalFraction.FromString(output);
        Assert.AreEqual(output,d3.ToString(),(
          "expected: ["+(d4.Mantissa).ToString()+","+(d4.Exponent).ToString()+"]\\n"+
          "but was: ["+(d3.Mantissa).ToString()+","+(d3.Exponent).ToString()+"]"
         ));   }
    }
    
    public static BigInteger BigIntParse(string str){
      if(str==null || str.Length==0)throw new ArgumentNullException("str");
      int offset=0;
      bool negative=false;
      if(str[0]=='-'){
        offset++;
        negative=true;
      }
      // Assumes the string contains
      // only the digits '0' through '9'
      MutableBigInteger mbi = new MutableBigInteger();
      for (int i = offset; i < str.Length; i++) {
        int digit = (int)(str[i] - '0');
        mbi.Multiply(10).Add(digit);
      }
      BigInteger bi=mbi.ToBigInteger();
      if(negative)bi=-bi;
      return bi;
    }
    
    public static void AssertFlags(int expected, int actual){
      if(expected==actual)return;
      Assert.AreEqual((expected&PrecisionContext.FlagInexact)!=0,
                      (expected&PrecisionContext.FlagInexact)!=0,"Inexact");
      Assert.AreEqual((expected&PrecisionContext.FlagRounded)!=0,
                      (expected&PrecisionContext.FlagRounded)!=0,"Rounded");
      Assert.AreEqual((expected&PrecisionContext.FlagSubnormal)!=0,
                      (expected&PrecisionContext.FlagSubnormal)!=0,"Subnormal");
      Assert.AreEqual((expected&PrecisionContext.FlagOverflow)!=0,
                      (expected&PrecisionContext.FlagOverflow)!=0,"Overflow");
      Assert.AreEqual((expected&PrecisionContext.FlagUnderflow)!=0,
                      (expected&PrecisionContext.FlagUnderflow)!=0,"Underflow");
      Assert.AreEqual((expected&PrecisionContext.FlagClamped)!=0,
                      (expected&PrecisionContext.FlagClamped)!=0,"Clamped");
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
    public static void AssertEqualsHashCode(CBORObject o, CBORObject o2) {
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
        try { o.AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail("Object: "+o+", "+ex.ToString()); }
        try { o.AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail("Object: "+o+", "+ex.ToString()); }
        try { o.AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail("Object: "+o+", "+ex.ToString()); }
        try { o.AsInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail("Object: "+o+", "+ex.ToString()); }
        try { o.AsSingle(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail("Object: "+o+", "+ex.ToString()); }
        try { o.AsDouble(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail("Object: "+o+", "+ex.ToString()); }
        try { o.AsBigFloat(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail("Object: "+o+", "+ex.ToString()); }
        try { o.AsBigInteger(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail("Object: "+o+", "+ex.ToString()); }
        try { o.AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail("Object: "+o+", "+ex.ToString()); }
        return;
      }
      BigInteger df=o.AsDecimalFraction().ToBigInteger();
      try { o.AsBigInteger(); } catch(Exception ex){ Assert.Fail("Object: "+o+", int: "+df+", "+ex.ToString()); }
      try { o.AsBigFloat(); } catch(Exception ex){ Assert.Fail("Object: "+o+", int: "+df+", "+ex.ToString()); }
      try { o.AsSingle(); } catch(Exception ex){ Assert.Fail("Object: "+o+", int: "+df+", "+ex.ToString()); }
      try { o.AsDouble(); } catch(Exception ex){ Assert.Fail("Object: "+o+", int: "+df+", "+ex.ToString()); }
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