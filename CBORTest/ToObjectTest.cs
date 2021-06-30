using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using PeterO;
using PeterO.Cbor;
using PeterO.Numbers;

namespace Test {
  [TestFixture]
  public class ToObjectTest {
    [Test]
    public void TestAsEInteger() {
      if (CBORObject.Null.ToObject(typeof(EInteger)) != null) {
        Assert.Fail();
      }
      try {
        CBORObject.True.ToObject(typeof(EInteger));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.ToObject(typeof(EInteger));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.ToObject(typeof(EInteger));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewArray().ToObject(typeof(EInteger));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().ToObject(typeof(EInteger));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = CBORObjectTest.GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        var numberString = (string)numberinfo["number"]
          .ToObject(typeof(string));
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(
            EDecimal.FromString(numberString));
        if (!numberinfo["integer"].Equals(CBORObject.Null)) {
          Assert.AreEqual(
            numberinfo["integer"].ToObject(typeof(string)),
            cbornumber.ToObject(typeof(EInteger)).ToString());
        } else {
          try {
            cbornumber.ToObject(typeof(EInteger));
            Assert.Fail("Should have failed");
          } catch (OverflowException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
      }

      {
        string stringTemp =
          ToObjectTest.TestToFromObjectRoundTrip(0.75f)
          .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
          "0",
          stringTemp);
      }
      {
        string stringTemp = ToObjectTest.TestToFromObjectRoundTrip(0.99f)
          .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
          "0",
          stringTemp);
      }
      {
        string stringTemp =
          ToObjectTest.TestToFromObjectRoundTrip(0.0000000000000001f)
          .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
          "0",
          stringTemp);
      }
      {
        string stringTemp = ToObjectTest.TestToFromObjectRoundTrip(0.5f)
          .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
          "0",
          stringTemp);
      }
      {
        string stringTemp = ToObjectTest.TestToFromObjectRoundTrip(1.5f)
          .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
          "1",
          stringTemp);
      }
      {
        string stringTemp = ToObjectTest.TestToFromObjectRoundTrip(2.5f)
          .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
          "2",
          stringTemp);
      }
      {
        string stringTemp =
          ToObjectTest.TestToFromObjectRoundTrip((float)328323f)
          .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
          "328323",
          stringTemp);
      }
      {
        string stringTemp = ToObjectTest.TestToFromObjectRoundTrip((double)0.75)
          .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
          "0",
          stringTemp);
      }
      {
        string stringTemp = ToObjectTest.TestToFromObjectRoundTrip((double)0.99)
          .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
          "0",
          stringTemp);
      }
      {
        string stringTemp =
          ToObjectTest.TestToFromObjectRoundTrip((double)0.0000000000000001)
          .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
          "0",
          stringTemp);
      }
      {
        string stringTemp = ToObjectTest.TestToFromObjectRoundTrip((double)0.5)
          .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
          "0",
          stringTemp);
      }
      {
        string stringTemp = ToObjectTest.TestToFromObjectRoundTrip((double)1.5)
          .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
          "1",
          stringTemp);
      }
      {
        string stringTemp = ToObjectTest.TestToFromObjectRoundTrip((double)2.5)
          .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
          "2",
          stringTemp);
      }
      {
        double dvalue = 328323;
        string stringTemp = ToObjectTest.TestToFromObjectRoundTrip(dvalue)
          .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
          "328323",
          stringTemp);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(Single.PositiveInfinity)
        .ToObject(typeof(EInteger));
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(Single.NegativeInfinity)
        .ToObject(typeof(EInteger));
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(Single.NaN)
        .ToObject(typeof(EInteger));
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(Double.PositiveInfinity)
        .ToObject(typeof(EInteger));
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(Double.NegativeInfinity)
        .ToObject(typeof(EInteger));
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(Double.NaN)
        .ToObject(typeof(EInteger));
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestAsBoolean() {
      Assert.AreEqual(true, CBORObject.True.ToObject(typeof(bool)));
      {
        object objectTemp = true;
        var objectTemp2 = (object)ToObjectTest.TestToFromObjectRoundTrip(0)
          .ToObject(typeof(bool));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = true;
        var objectTemp2 =
          (object)ToObjectTest.TestToFromObjectRoundTrip(String.Empty)

          .ToObject(typeof(bool));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      Assert.AreEqual(false, CBORObject.False.ToObject(typeof(bool)));
      Assert.AreEqual(false, CBORObject.Undefined.ToObject(typeof(bool)));
      Assert.AreEqual(true, CBORObject.NewArray().ToObject(typeof(bool)));
      Assert.AreEqual(true, CBORObject.NewMap().ToObject(typeof(bool)));
    }

    [Test]
    public void TestNullBoolean() {
      if (CBORObject.Null.ToObject(typeof(bool)) != null) {
        Assert.Fail();
      }
    }

    [Test]
    public void TestAsByte() {
      try {
        CBORObject.NewArray().ToObject(typeof(byte));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().ToObject(typeof(byte));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.ToObject(typeof(byte));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.ToObject(typeof(byte));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.ToObject(typeof(byte));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(String.Empty)
        .ToObject(typeof(byte));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = CBORObjectTest.GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(EDecimal.FromString(
              (string)numberinfo["number"].ToObject(typeof(string))));

        if ((bool)numberinfo["byte"].AsBoolean()) {
          int i1 = TestCommon.StringToInt((string)numberinfo["integer"]
              .ToObject(typeof(string)));
          int i2 = ((int)(Byte)cbornumber.ToObject(typeof(byte))) & 0xff;
          Assert.AreEqual(i1, i2);
        } else {
          try {
            cbornumber.ToObject(typeof(byte));
            Assert.Fail("Should have failed " + cbornumber);
          } catch (OverflowException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString() + cbornumber);
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
      }
      for (var i = 0; i < 255; ++i) {
        object
        o = ToObjectTest.TestToFromObjectRoundTrip(i).ToObject(typeof(byte));
        Assert.AreEqual((byte)i, (byte)o);
      }
      for (int i = -200; i < 0; ++i) {
        try {
          ToObjectTest.TestToFromObjectRoundTrip(i).ToObject(typeof(byte));
        } catch (OverflowException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      for (int i = 256; i < 512; ++i) {
        try {
          ToObjectTest.TestToFromObjectRoundTrip(i).ToObject(typeof(byte));
        } catch (OverflowException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
    }

    [Test]
    public void TestAsDouble() {
      try {
        CBORObject.NewArray().ToObject(typeof(double));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().ToObject(typeof(double));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.ToObject(typeof(double));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.ToObject(typeof(double));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.ToObject(typeof(double));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(String.Empty)
        .ToObject(typeof(double));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = CBORObjectTest.GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        double dbl;
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(EDecimal.FromString(
              (string)numberinfo["number"].ToObject(typeof(string))));
        dbl = (double)EDecimal.FromString(
            (string)numberinfo["number"].ToObject(typeof(string)))
          .ToDouble();
        object dblobj = cbornumber.ToObject(typeof(double));
        CBORObjectTest.AreEqualExact(
          dbl,
          (double)dblobj);
      }
    }

    [Test]
    public void TestAsEDecimal() {
      {
        object objectTemp = CBORTestCommon.DecPosInf;
        object objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(Single.PositiveInfinity)
          .ToObject(typeof(EDecimal));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = CBORTestCommon.DecNegInf;
        object objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(Single.NegativeInfinity)
          .ToObject(typeof(EDecimal));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = CBORTestCommon.DecPosInf;
        object objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(Double.PositiveInfinity)
          .ToObject(typeof(EDecimal));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = CBORTestCommon.DecNegInf;
        object objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(Double.NegativeInfinity)
          .ToObject(typeof(EDecimal));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        bool bo = ((EDecimal)ToObjectTest.TestToFromObjectRoundTrip(Single.NaN)
          .ToObject(typeof(EDecimal))).IsNaN();
        Assert.IsTrue(bo);
      }
      {
        bool bo = ((EDecimal)ToObjectTest.TestToFromObjectRoundTrip(Double.NaN)
          .ToObject(typeof(EDecimal))).IsNaN();
        Assert.IsTrue(bo);
      }
      try {
        CBORObject.NewArray().ToObject(typeof(EDecimal));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().ToObject(typeof(EDecimal));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.ToObject(typeof(EDecimal));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.ToObject(typeof(EDecimal));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.ToObject(typeof(EDecimal));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(String.Empty)
        .ToObject(typeof(EDecimal));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestAsEFloat() {
      {
        object objectTemp = CBORTestCommon.FloatPosInf;
        object objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(Single.PositiveInfinity)
          .ToObject(typeof(EFloat));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = CBORTestCommon.FloatNegInf;
        object objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(Single.NegativeInfinity)
          .ToObject(typeof(EFloat));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      var ef = (EFloat)ToObjectTest.TestToFromObjectRoundTrip(Single.NaN)
        .ToObject(typeof(EFloat));
      Assert.IsTrue(ef.IsNaN());
      {
        object objectTemp = CBORTestCommon.FloatPosInf;
        object objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(Double.PositiveInfinity)
          .ToObject(typeof(EFloat));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = CBORTestCommon.FloatNegInf;
        object objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(Double.NegativeInfinity)
          .ToObject(typeof(EFloat));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      ef = (EFloat)ToObjectTest.TestToFromObjectRoundTrip(Double.NaN)
        .ToObject(typeof(EFloat));
      Assert.IsTrue(ef.IsNaN());
    }

    [Test]
    public void TestAsERational() {
      {
        object objectTemp = CBORTestCommon.RatPosInf;
        object objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(Single.PositiveInfinity)
          .ToObject(typeof(ERational));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = CBORTestCommon.RatNegInf;
        object objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(Single.NegativeInfinity)
          .ToObject(typeof(ERational));
        Assert.AreEqual(objectTemp, objectTemp2);
      }

      bool bnan = ToObjectTest.TestToFromObjectRoundTrip(
          ToObjectTest.TestToFromObjectRoundTrip(Single.NaN)
          .ToObject(typeof(ERational))).AsNumber().IsNaN();
      Assert.IsTrue(bnan);
      {
        object objectTemp = CBORTestCommon.RatPosInf;
        object objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(Double.PositiveInfinity)
          .ToObject(typeof(ERational));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = CBORTestCommon.RatNegInf;
        object objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(Double.NegativeInfinity)
          .ToObject(typeof(ERational));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      Assert.IsTrue(
        ToObjectTest.TestToFromObjectRoundTrip(
          ToObjectTest.TestToFromObjectRoundTrip(Double.NaN)
          .ToObject(typeof(ERational))).AsNumber().IsNaN());
    }

    [Test]
    public void TestAsInt16() {
      try {
        CBORObject.NewArray().ToObject(typeof(short));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().ToObject(typeof(short));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.ToObject(typeof(short));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.ToObject(typeof(short));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.ToObject(typeof(short));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(String.Empty)
        .ToObject(typeof(short));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = CBORObjectTest.GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(
            EDecimal.FromString((string)numberinfo["number"].ToObject(
                typeof(string))));
        if ((bool)numberinfo["int16"].AsBoolean()) {
          var sh = (short)TestCommon.StringToInt(
              (string)numberinfo["integer"].ToObject(typeof(string)));
          object o = cbornumber.ToObject(typeof(short));
          Assert.AreEqual(sh, (short)o);
        } else {
          try {
            cbornumber.ToObject(typeof(short));
            Assert.Fail("Should have failed " + cbornumber);
          } catch (OverflowException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString() + cbornumber);
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
      }
    }

    [Test]
    public void TestAsInt32() {
      try {
        CBORObject.NewArray().ToObject(typeof(int));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().ToObject(typeof(int));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.ToObject(typeof(int));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.ToObject(typeof(int));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.ToObject(typeof(int));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject secbor =
          ToObjectTest.TestToFromObjectRoundTrip(String.Empty);
        secbor.ToObject(typeof(int));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = CBORObjectTest.GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        EDecimal edec =

  EDecimal.FromString((string)numberinfo["number"].ToObject(typeof(string)));
        CBORObject cbornumber = ToObjectTest.TestToFromObjectRoundTrip(edec);
        bool isdouble;
        isdouble = (bool)numberinfo["double"].AsBoolean();
        CBORObject cbornumberdouble =
          ToObjectTest.TestToFromObjectRoundTrip(edec.ToDouble());
        bool issingle;
        issingle = (bool)numberinfo["single"].AsBoolean();
        CBORObject cbornumbersingle =
          ToObjectTest.TestToFromObjectRoundTrip(edec.ToSingle());
        if ((bool)numberinfo["int32"].AsBoolean()) {
          object o = cbornumber.ToObject(typeof(int));
          Assert.AreEqual(
            TestCommon.StringToInt((string)numberinfo["integer"].ToObject(
                typeof(string))),
            (int)o);
          if (isdouble) {
            o = cbornumberdouble.ToObject(typeof(int));
            Assert.AreEqual(
              TestCommon.StringToInt((string)numberinfo["integer"].ToObject(
                  typeof(string))),
              (int)o);
          }
          if (issingle) {
            o = cbornumbersingle.ToObject(typeof(int));
            Assert.AreEqual(
              TestCommon.StringToInt((string)numberinfo["integer"].ToObject(
                  typeof(string))),
              (int)o);
          }
        } else {
          try {
            Console.WriteLine(String.Empty + cbornumber.ToObject(typeof(int)));
            Assert.Fail("Should have failed " + cbornumber);
          } catch (OverflowException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString() + cbornumber);
            throw new InvalidOperationException(String.Empty, ex);
          }
          if (isdouble) {
            try {
              cbornumberdouble.ToObject(typeof(int));
              Assert.Fail("Should have failed");
            } catch (OverflowException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
          }
          if (issingle) {
            try {
              cbornumbersingle.ToObject(typeof(int));
              Assert.Fail("Should have failed");
            } catch (OverflowException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
          }
        }
      }
    }

    [Test]
    public void TestAsInt64() {
      try {
        CBORObject.NewArray().ToObject(typeof(long));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().ToObject(typeof(long));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.ToObject(typeof(long));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.ToObject(typeof(long));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.ToObject(typeof(long));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(String.Empty)
        .ToObject(typeof(long));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = CBORObjectTest.GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        EDecimal edec =

  EDecimal.FromString((string)numberinfo["number"].ToObject(typeof(string)));
        CBORObject cbornumber = ToObjectTest.TestToFromObjectRoundTrip(edec);
        bool isdouble;
        isdouble = (bool)numberinfo["double"].AsBoolean();
        CBORObject cbornumberdouble =
          ToObjectTest.TestToFromObjectRoundTrip(edec.ToDouble());
        bool issingle;
        issingle = (bool)numberinfo["single"].AsBoolean();
        CBORObject cbornumbersingle =
          ToObjectTest.TestToFromObjectRoundTrip(edec.ToSingle());
        if ((bool)numberinfo["int64"].AsBoolean()) {
          object o = cbornumber.ToObject(typeof(long));
          Assert.AreEqual(
            TestCommon.StringToLong((string)numberinfo["integer"].ToObject(
                typeof(string))),
            (long)o);
          if (isdouble) {
            long strlong = TestCommon.StringToLong(
                (string)numberinfo["integer"].ToObject(typeof(string)));
            o = cbornumberdouble.ToObject(typeof(long));
            Assert.AreEqual(
              strlong,
              (long)o);
          }
          if (issingle) {
            long strlong = TestCommon.StringToLong(
                (string)numberinfo["integer"].ToObject(typeof(string)));
            o = cbornumberdouble.ToObject(typeof(long));
            Assert.AreEqual(
              strlong,
              (long)o);
          }
        } else {
          try {
            cbornumber.ToObject(typeof(long));
            Assert.Fail("Should have failed " + cbornumber);
          } catch (OverflowException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString() + cbornumber);
            throw new InvalidOperationException(String.Empty, ex);
          }
          if (isdouble) {
            try {
              cbornumberdouble.ToObject(typeof(long));
              Assert.Fail("Should have failed");
            } catch (OverflowException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
          }
          if (issingle) {
            try {
              cbornumbersingle.ToObject(typeof(long));
              Assert.Fail("Should have failed");
            } catch (OverflowException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
          }
        }
      }
    }

    [Test]
    public void TestAsSingle() {
      try {
        CBORObject.NewArray().ToObject(typeof(float));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().ToObject(typeof(float));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.ToObject(typeof(float));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.ToObject(typeof(float));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.ToObject(typeof(float));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(String.Empty)
        .ToObject(typeof(float));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = CBORObjectTest.GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(EDecimal.FromString(
              (string)numberinfo["number"].ToObject(typeof(string))));

        var f1 =
          (float)EDecimal.FromString((string)numberinfo["number"].ToObject(
              typeof(string))).ToSingle();
        Object f2 = cbornumber.ToObject(typeof(float));
        if (!((object)f1).Equals(f2)) {
          Assert.Fail();
        }
      }
    }

    [Test]
    public void TestAsString() {
      {
        var stringTemp = (string)ToObjectTest.TestToFromObjectRoundTrip("test")
          .ToObject(typeof(string));
        Assert.AreEqual(
          "test",
          stringTemp);
      }
      var sb = (string)ToObjectTest.TestToFromObjectRoundTrip(CBORObject.Null)
        .ToObject(typeof(string));
      if (sb != null) {
        Assert.Fail();
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(true).ToObject(typeof(string));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(false).ToObject(typeof(string));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(5).ToObject(typeof(string));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewArray().ToObject(typeof(string));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().ToObject(typeof(string));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestToObjectFieldClass() {
      var fc = new FieldClass();
      CBORObject co = CBORObject.FromObject(fc);
      Assert.IsFalse(co.ContainsKey("PrivateFieldA"));
      Assert.IsFalse(co.ContainsKey("privateFieldA"));
      Assert.IsFalse(co.ContainsKey("PrivateFieldB"));
      Assert.IsFalse(co.ContainsKey("privateFieldB"));
      Assert.IsFalse(co.ContainsKey("staticFieldA"));
      Assert.IsFalse(co.ContainsKey("StaticFieldA"));
      Assert.IsFalse(co.ContainsKey("constFieldA"));
      Assert.IsFalse(co.ContainsKey("ConstFieldA"));
      Assert.IsFalse(co.ContainsKey("readonlyFieldA"));
      Assert.IsFalse(co.ContainsKey("ReadonlyFieldA"));
      Assert.IsTrue(co.ContainsKey("publicFieldA"));
      co["privateFieldA"] = ToObjectTest.TestToFromObjectRoundTrip(999);
      co["publicFieldA"] = ToObjectTest.TestToFromObjectRoundTrip(999);
      fc = (FieldClass)co.ToObject(typeof(FieldClass));
      Assert.AreEqual(999, fc.publicFieldA);
    }

    [Test]
    public void TestToObjectDictStringString() {
      CBORObject cbor = CBORObject.NewMap().Add("a", "b").Add("c", "d");
      Dictionary<string, string> stringDict =
        (Dictionary<string, string>)cbor.ToObject(
          typeof(Dictionary<string, string>));
      Assert.AreEqual(2, stringDict.Count);
      Assert.IsTrue(stringDict.ContainsKey("a"));
      Assert.IsTrue(stringDict.ContainsKey("c"));
      Assert.AreEqual("b", stringDict["a"]);
      Assert.AreEqual("d", stringDict["c"]);
    }
    [Test]
    public void TestToObjectIDictStringString() {
      CBORObject cbor = CBORObject.NewMap().Add("a", "b").Add("c", "d");
      IDictionary<string, string> stringDict2 =
        (IDictionary<string, string>)cbor.ToObject(
          typeof(IDictionary<string, string>));
      Assert.AreEqual(2, stringDict2.Count);
      Assert.IsTrue(stringDict2.ContainsKey("a"));
      Assert.IsTrue(stringDict2.ContainsKey("c"));
      Assert.AreEqual("b", stringDict2["a"]);
      Assert.AreEqual("d", stringDict2["c"]);
    }

    [Test]
    [Timeout(5000)]
    public void TestToObject() {
      var ao = new PODClass();
      CBORObject co = CBORObject.FromObject(ao);
      Assert.IsFalse(co.ContainsKey("PrivatePropA"));
      Assert.IsFalse(co.ContainsKey("privatePropA"));
      Assert.IsFalse(co.ContainsKey("staticPropA"));
      Assert.IsFalse(co.ContainsKey("StaticPropA"));
      co["privatePropA"] = ToObjectTest.TestToFromObjectRoundTrip(999);
      co["propA"] = ToObjectTest.TestToFromObjectRoundTrip(999);
      co["floatProp"] = ToObjectTest.TestToFromObjectRoundTrip(3.5);
      co["doubleProp"] = ToObjectTest.TestToFromObjectRoundTrip(4.5);
      co["stringProp"] = ToObjectTest.TestToFromObjectRoundTrip("stringProp");
      co["stringArray"] = CBORObject.NewArray().Add("a").Add("b");
      ao = (PODClass)co.ToObject(typeof(PODClass));
      // Check whether ToObject ignores private setters
      Assert.IsTrue(ao.HasGoodPrivateProp());
      Assert.AreEqual(999, ao.PropA);
      if (ao.FloatProp != 3.5f) {
        Assert.Fail();
      }
      if (ao.DoubleProp != 4.5) {
        Assert.Fail();
      }
      Assert.AreEqual("stringProp", ao.StringProp);
      string[] stringArray = ao.StringArray;
      Assert.AreEqual(2, stringArray.Length);
      Assert.AreEqual("a", stringArray[0]);
      Assert.AreEqual("b", stringArray[1]);
      Assert.IsFalse(ao.IsPropC);
      co["propC"] = CBORObject.True;
      ao = (PODClass)co.ToObject(typeof(PODClass));
      Assert.IsTrue(ao.IsPropC);
      co = CBORObject.True;
      Assert.AreEqual(true, co.ToObject(typeof(bool)));
      co = CBORObject.False;
      Assert.AreEqual(false, co.ToObject(typeof(bool)));
      co = ToObjectTest.TestToFromObjectRoundTrip("hello world");
      var stringTemp = (string)co.ToObject(typeof(string));
      Assert.AreEqual(
        "hello world",
        stringTemp);
      co = CBORObject.NewArray();
      co.Add("hello");
      co.Add("world");
      List<string> stringList = (List<string>)co.ToObject(typeof(List<string>));
      Assert.AreEqual(2, stringList.Count);
      Assert.AreEqual("hello", stringList[0]);
      Assert.AreEqual("world", stringList[1]);
      IList<string> istringList = (IList<string>)co.ToObject(
          typeof(IList<string>));

      Assert.AreEqual(2, istringList.Count);
      Assert.AreEqual("hello", istringList[0]);
      Assert.AreEqual("world", istringList[1]);
      co = CBORObject.NewMap();
      co.Add("a", 1);
      co.Add("b", 2);
      Dictionary<string, int> intDict =
        (Dictionary<string, int>)co.ToObject(
          typeof(Dictionary<string, int>));
      Assert.AreEqual(2, intDict.Count);
      Assert.IsTrue(intDict.ContainsKey("a"));
      Assert.IsTrue(intDict.ContainsKey("b"));
      if (intDict["a"] != 1) {
        Assert.Fail();
      }
      if (intDict["b"] != 2) {
        Assert.Fail();
      }
      IDictionary<string, int> iintDict = (IDictionary<string, int>)co.ToObject(
          typeof(IDictionary<string, int>));
      Assert.AreEqual(2, iintDict.Count);
      Assert.IsTrue(iintDict.ContainsKey("a"));
      Assert.IsTrue(iintDict.ContainsKey("b"));
      if (iintDict["a"] != 1) {
        Assert.Fail();
      }
      if (iintDict["b"] != 2) {
        Assert.Fail();
      }
      co = CBORObject.FromObjectAndTag(
          "2000-01-01T00:00:00Z",
          0);
      try {
        co.ToObject(typeof(DateTime));
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestShortRoundTrip() {
      for (var i = -32768; i < 32768; ++i) {
        var c = (short)i;
        TestToFromObjectRoundTrip(c);
      }
    }

    [Test]
    public void TestCharRoundTrip() {
      for (var i = 0; i < 0x10000; ++i) {
        var c = (char)i;
        TestToFromObjectRoundTrip(c);
      }
    }

    private static string RandomDate(RandomGenerator rand) {
      int year = rand.UniformInt(1, 10000);
      int month = rand.UniformInt(1, 13);
      int day = rand.UniformInt(1, 29);
      int hour = rand.UniformInt(1, 24);
      int min = rand.UniformInt(1, 60);
      int sec = rand.UniformInt(1, 60);
      var dt = new char[20];
      dt[0] = (char)(0x30 + ((year / 1000) % 10));
      dt[1] = (char)(0x30 + ((year / 100) % 10));
      dt[2] = (char)(0x30 + ((year / 10) % 10));
      dt[3] = (char)(0x30 + (year % 10));
      dt[4] = '-';
      dt[5] = (char)(0x30 + ((month / 10) % 10));
      dt[6] = (char)(0x30 + (month % 10));
      dt[7] = '-';
      dt[8] = (char)(0x30 + ((day / 10) % 10));
      dt[9] = (char)(0x30 + (day % 10));
      dt[10] = 'T';
      dt[11] = (char)(0x30 + ((hour / 10) % 10));
      dt[12] = (char)(0x30 + (hour % 10));
      dt[13] = ':';
      dt[14] = (char)(0x30 + ((min / 10) % 10));
      dt[15] = (char)(0x30 + (min % 10));
      dt[16] = ':';
      dt[17] = (char)(0x30 + ((sec / 10) % 10));
      dt[18] = (char)(0x30 + (sec % 10));
      dt[19] = 'Z';
      return new String(dt);
    }

    [Test]
    public void TestDateRoundTrip() {
      var rand = new RandomGenerator();
      for (var i = 0; i < 5000; ++i) {
        string s = RandomDate(rand);
        CBORObject cbor = CBORObject.FromObjectAndTag(s, 0);
        var dtime = (DateTime)cbor.ToObject(typeof(DateTime));
        CBORObject cbor2 = CBORObject.FromObject(dtime);
        Assert.AreEqual(s, cbor2.AsString());
        TestToFromObjectRoundTrip(dtime);
      }
    }

    [Test]
    public void TestDateRoundTripNumber() {
      var rand = new RandomGenerator();
      var typemapper = new CBORTypeMapper().AddConverter(
         typeof(DateTime),
         CBORDateConverter.TaggedNumber);
      for (var i = 0; i < 5000; ++i) {
        string s = RandomDate(rand);
        CBORObject cbor = CBORObject.FromObjectAndTag(s, 0);
        var dtime = (DateTime)cbor.ToObject(typeof(DateTime));
        CBORObject cbor2 = CBORObject.FromObject(dtime);
        Assert.AreEqual(s, cbor2.AsString());
        CBORObject cborNumber = CBORObject.FromObject(dtime, typemapper);
        Assert.IsTrue(cborNumber.Type == CBORType.Integer ||
           cborNumber.Type == CBORType.FloatingPoint);
        var dtime2 = (DateTime)cborNumber.ToObject(typeof(DateTime),
  typemapper);
        cbor2 = CBORObject.FromObject(dtime2, typemapper);
        Assert.IsTrue(cbor2.Type == CBORType.Integer ||
           cbor2.Type == CBORType.FloatingPoint);
        Assert.AreEqual(cbor2, cborNumber, s);
        TestToFromObjectRoundTrip(dtime);
      }
    }

    [Test]
    public void TestDateRoundTripUntaggedNumber() {
      var rand = new RandomGenerator();
      var typemapper = new CBORTypeMapper().AddConverter(
         typeof(DateTime),
         CBORDateConverter.UntaggedNumber);
      for (var i = 0; i < 5000; ++i) {
        string s = RandomDate(rand);
        CBORObject cbor = CBORObject.FromObjectAndTag(s, 0);
        var dtime = (DateTime)cbor.ToObject(typeof(DateTime));
        CBORObject cbor2 = CBORObject.FromObject(dtime);
        Assert.AreEqual(s, cbor2.AsString());
        CBORObject cborNumber = CBORObject.FromObject(dtime, typemapper);
        Assert.IsTrue(cborNumber.Type == CBORType.Integer ||
           cborNumber.Type == CBORType.FloatingPoint);
        var dtime2 = (DateTime)cborNumber.ToObject(typeof(DateTime),
  typemapper);
        cbor2 = CBORObject.FromObject(dtime2, typemapper);
        Assert.IsTrue(cbor2.Type == CBORType.Integer ||
           cbor2.Type == CBORType.FloatingPoint);
        Assert.AreEqual(cbor2, cborNumber, s);
        TestToFromObjectRoundTrip(dtime);
      }
    }

    [Test]
    public void TestBadDate() {
      CBORObject cbor = CBORObject.FromObjectAndTag(
          "2000-1-01T00:00:00Z",
          0);
      try {
        cbor.ToObject(typeof(DateTime));
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.FromObjectAndTag(
          "2000-01-1T00:00:00Z",
          0);
      try {
        cbor.ToObject(typeof(DateTime));
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.FromObjectAndTag(
          "2000-01-01T0:00:00Z",
          0);
      try {
        cbor.ToObject(typeof(DateTime));
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.FromObjectAndTag(
          "2000-01-01T00:0:00Z",
          0);
      try {
        cbor.ToObject(typeof(DateTime));
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.FromObjectAndTag(
          "2000-01-01T00:00:0Z",
          0);
      try {
        cbor.ToObject(typeof(DateTime));
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.FromObjectAndTag(
          "T01:01:01Z",
          0);
      try {
        cbor.ToObject(typeof(DateTime));
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestUriRoundTrip() {
      try {
        var uri = new Uri("http://example.com/path/path2?query#fragment");
        TestToFromObjectRoundTrip(uri);
      } catch (Exception ex) {
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    private class CPOD3Converter : ICBORToFromConverter<CPOD3> {
      public CBORObject ToCBORObject(CPOD3 cpod) {
        return CBORObject.NewMap().Add(0, cpod.Aa)
          .Add(1, cpod.Bb).Add(2, cpod.Cc);
      }
      public CPOD3 FromCBORObject(CBORObject obj) {
        if (obj.Type != CBORType.Map) {
          throw new CBORException();
        }
        var ret = new CPOD3();
        ret.Aa = obj[0].AsString();
        ret.Bb = obj[1].AsString();
        ret.Cc = obj[2].AsString();
        return ret;
      }
    }

    [Test]
    public void TestCBORTypeMapper() {
      var cp = new CPOD3();
      cp.Aa = "aa";
      cp.Bb = "bb";
      cp.Cc = "cc";
      var cp2 = new CPOD3();
      cp2.Aa = "AA";
      cp2.Bb = "BB";
      cp2.Cc = "CC";
      var tm = new CBORTypeMapper().AddConverter(
        typeof(CPOD3),
        new CPOD3Converter());
      CBORObject cbor;
      CBORObject cbor2;
      cbor = CBORObject.FromObject(cp, tm);
      Assert.AreEqual(CBORType.Map, cbor.Type);
      Assert.AreEqual(3, cbor.Count);
      {
        string stringTemp = cbor[0].AsString();
        Assert.AreEqual(
          "aa",
          stringTemp);
      }
      Assert.IsFalse(cbor.ContainsKey("aa"));
      Assert.IsFalse(cbor.ContainsKey("Aa"));
      {
        string stringTemp = cbor[1].AsString();
        Assert.AreEqual(
          "bb",
          stringTemp);
      }
      {
        string stringTemp = cbor[2].AsString();
        Assert.AreEqual(
          "cc",
          stringTemp);
      }
      var cpx = (CPOD3)cbor.ToObject(typeof(CPOD3), tm);
      Assert.AreEqual("aa", cpx.Aa);
      Assert.AreEqual("bb", cpx.Bb);
      Assert.AreEqual("cc", cpx.Cc);
      var cpodArray = new CPOD3[] { cp, cp2 };
      cbor = CBORObject.FromObject(cpodArray, tm);
      Assert.AreEqual(CBORType.Array, cbor.Type);
      Assert.AreEqual(2, cbor.Count);
      cbor2 = cbor[0];
      {
        string stringTemp = cbor2[0].AsString();
        Assert.AreEqual(
          "aa",
          stringTemp);
      }
      {
        string stringTemp = cbor2[1].AsString();
        Assert.AreEqual(
          "bb",
          stringTemp);
      }
      {
        string stringTemp = cbor2[2].AsString();
        Assert.AreEqual(
          "cc",
          stringTemp);
      }
      cbor2 = cbor[1];
      {
        string stringTemp = cbor2[0].AsString();
        Assert.AreEqual(
          "AA",
          stringTemp);
      }
      {
        string stringTemp = cbor2[1].AsString();
        Assert.AreEqual(
          "BB",
          stringTemp);
      }
      {
        string stringTemp = cbor2[2].AsString();
        Assert.AreEqual(
          "CC",
          stringTemp);
      }
      CPOD3[] cpa = (CPOD3[])cbor.ToObject(typeof(CPOD3[]), tm);
      cpx = cpa[0];
      Assert.AreEqual("aa", cpx.Aa);
      Assert.AreEqual("bb", cpx.Bb);
      Assert.AreEqual("cc", cpx.Cc);
      cpx = cpa[1];
      Assert.AreEqual("AA", cpx.Aa);
      Assert.AreEqual("BB", cpx.Bb);
      Assert.AreEqual("CC", cpx.Cc);
    }

    [Test]
    public void TestUUIDRoundTrip() {
      var rng = new RandomGenerator();
      for (var i = 0; i < 500; ++i) {
        TestToFromObjectRoundTrip(RandomUUID(rng));
      }
    }

    public static object RandomUUID(RandomGenerator rand) {
      if (rand == null) {
        throw new ArgumentNullException(nameof(rand));
      }
      string hex = "0123456789ABCDEF";
      var sb = new StringBuilder();
      if (rand == null) {
        throw new ArgumentNullException(nameof(rand));
      }
      for (var i = 0; i < 8; ++i) {
        sb.Append(hex[rand.UniformInt(16)]);
      }
      sb.Append('-');
      for (var i = 0; i < 4; ++i) {
        sb.Append(hex[rand.UniformInt(16)]);
      }
      sb.Append('-');
      for (var i = 0; i < 4; ++i) {
        sb.Append(hex[rand.UniformInt(16)]);
      }
      sb.Append('-');
      for (var i = 0; i < 4; ++i) {
        sb.Append(hex[rand.UniformInt(16)]);
      }
      sb.Append('-');
      for (var i = 0; i < 12; ++i) {
        sb.Append(hex[rand.UniformInt(16)]);
      }
      return new Guid(sb.ToString());
    }

    public static CBORObject TestToFromObjectRoundTrip(object obj) {
      CBORObject cbor = CBORObject.FromObject(obj);
      if (obj != null) {
        object obj2 = null;
        try {
          obj2 = cbor.ToObject(obj.GetType());
        } catch (Exception ex) {
          Assert.Fail(ex.ToString() + "\n" + cbor + "\n" + obj.GetType());
          throw new InvalidOperationException(String.Empty, ex);
        }
        if (!obj.Equals(obj2)) {
          if (obj is byte[]) {
            TestCommon.AssertByteArraysEqual(
              (byte[])obj,
              (byte[])obj2);
          } else if (obj is string[]) {
            Assert.AreEqual((string[])obj, (string[])obj2);
          } else {
            Assert.AreEqual(obj, obj2, cbor + "\n" + obj.GetType());
          }
        }
        // Tests for DecodeObjectFromBytes
        byte[] encdata = cbor.EncodeToBytes();
        object obj3 =
          CBORObject.DecodeFromBytes(encdata).ToObject(obj.GetType());
        object obj4 = CBORObject.DecodeObjectFromBytes(encdata, obj.GetType());
        TestCommon.AssertEqualsHashCode(obj, obj2);
        TestCommon.AssertEqualsHashCode(obj, obj3);
        TestCommon.AssertEqualsHashCode(obj, obj4);
      }
      return cbor;
    }
  }
}
