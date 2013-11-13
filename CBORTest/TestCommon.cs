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
using System.Numerics;
using PeterO;
using System.IO;

namespace Test {
  static class TestCommon {
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
