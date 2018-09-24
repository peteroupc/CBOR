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
      Assert.IsNull(CBORObject.Null.ToObject(typeof(EInteger)));
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
        var numberString = (string)numberinfo["number"].ToObject(typeof(string));
        CBORObject cbornumber =
  ToObjectTest.TestToFromObjectRoundTrip(EDecimal.FromString(numberString));
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
ToObjectTest.TestToFromObjectRoundTrip((float)0.75)
            .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
        "0",
        stringTemp);
      }
      {
        string stringTemp =
ToObjectTest.TestToFromObjectRoundTrip((float)0.99)
            .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
        "0",
        stringTemp);
      }
      {
        string stringTemp =
ToObjectTest.TestToFromObjectRoundTrip((float)0.0000000000000001)
            .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
        "0",
        stringTemp);
      }
      {
        string stringTemp =
ToObjectTest.TestToFromObjectRoundTrip((float)0.5)
            .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
        "0",
        stringTemp);
      }
      {
        string stringTemp =
ToObjectTest.TestToFromObjectRoundTrip((float)1.5)
            .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
        "1",
        stringTemp);
      }
      {
        string stringTemp =
ToObjectTest.TestToFromObjectRoundTrip((float)2.5)
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
        string stringTemp =
ToObjectTest.TestToFromObjectRoundTrip((double)0.75)
            .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
        "0",
        stringTemp);
      }
      {
        string stringTemp =
ToObjectTest.TestToFromObjectRoundTrip((double)0.99)
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
        string stringTemp =
ToObjectTest.TestToFromObjectRoundTrip((double)0.5)
            .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
        "0",
        stringTemp);
      }
      {
        string stringTemp =
ToObjectTest.TestToFromObjectRoundTrip((double)1.5)
            .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
        "1",
        stringTemp);
      }
      {
        string stringTemp =
ToObjectTest.TestToFromObjectRoundTrip((double)2.5)
            .ToObject(typeof(EInteger)).ToString();
        Assert.AreEqual(
        "2",
        stringTemp);
      }
      {
        string stringTemp =
ToObjectTest.TestToFromObjectRoundTrip((double)328323)
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
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(Single.NegativeInfinity)
                  .ToObject(typeof(EInteger));
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(Single.NaN)
                  .ToObject(typeof(EInteger));
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(Double.PositiveInfinity)
                  .ToObject(typeof(EInteger));
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(Double.NegativeInfinity)
                  .ToObject(typeof(EInteger));
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(Double.NaN)
                  .ToObject(typeof(EInteger));
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestAsBoolean() {
      Assert.IsTrue((bool)CBORObject.True.ToObject(typeof(bool)));
      Assert.IsTrue((bool)ToObjectTest.TestToFromObjectRoundTrip(0)
              .ToObject(typeof(bool)));
      Assert.IsTrue((bool)ToObjectTest.TestToFromObjectRoundTrip(String.Empty)
              .ToObject(typeof(bool)));
      Assert.IsFalse((bool)CBORObject.False.ToObject(typeof(bool)));
      Assert.IsFalse((bool)CBORObject.Undefined.ToObject(typeof(bool)));
      Assert.IsTrue((bool)CBORObject.NewArray().ToObject(typeof(bool)));
      Assert.IsTrue((bool)CBORObject.NewMap().ToObject(typeof(bool)));
    }

    [Test]
    public void TestNullBoolean() {
      Assert.IsNull(CBORObject.Null.ToObject(typeof(bool)));
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
        if ((bool)numberinfo["byte"].ToObject(typeof(bool)) == true) {
          Assert.AreEqual(
  TestCommon.StringToInt((string)numberinfo["integer"].ToObject(typeof(string))),
            ((int)(byte)cbornumber.ToObject(typeof(byte))) & 0xff);
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
        Assert.AreEqual(
  (byte)i,
  ToObjectTest.TestToFromObjectRoundTrip(i).ToObject(typeof(byte)));
      }
      for (int i = -200; i < 0; ++i) {
        try {
          ToObjectTest.TestToFromObjectRoundTrip(i).ToObject(typeof(byte));
        } catch (OverflowException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString()); throw new
            InvalidOperationException(String.Empty, ex);
        }
      }
      for (int i = 256; i < 512; ++i) {
        try {
          ToObjectTest.TestToFromObjectRoundTrip(i).ToObject(typeof(byte));
        } catch (OverflowException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString()); throw new
            InvalidOperationException(String.Empty, ex);
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
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(EDecimal.FromString(
  (string)numberinfo["number"].ToObject(typeof(string))));
        CBORObjectTest.AreEqualExact(
  (double)EDecimal.FromString((string)numberinfo["number"].ToObject(typeof(string))).ToDouble(),
          (double)cbornumber.ToObject(typeof(double)));
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
        string stringTemp =
ToObjectTest.TestToFromObjectRoundTrip(Single.NaN)
            .ToObject(typeof(EDecimal)).ToString();
        Assert.AreEqual(
        "NaN",
        stringTemp);
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
        object objectTemp = "NaN";
        object objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(Double.NaN)
          .ToObject(typeof(EDecimal)).ToString();
        Assert.AreEqual(objectTemp, objectTemp2);
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

      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(ToObjectTest.TestToFromObjectRoundTrip(Single.NaN)
            .ToObject(typeof(ERational))).IsNaN());
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
  ToObjectTest.TestToFromObjectRoundTrip(ToObjectTest.TestToFromObjectRoundTrip(Double.NaN)
          .ToObject(typeof(ERational))).IsNaN());
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
   EDecimal.FromString((string)numberinfo["number"].ToObject(typeof(string))));
        if ((bool)numberinfo["int16"].ToObject(typeof(bool)) == true) {
          Assert.AreEqual(
  TestCommon.StringToInt((string)numberinfo["integer"].ToObject(typeof(string))),
    cbornumber.ToObject(typeof(short)));
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
        ToObjectTest.TestToFromObjectRoundTrip(String.Empty).ToObject(typeof(int));
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
        var isdouble = (bool)numberinfo["double"].ToObject(typeof(bool));
        CBORObject cbornumberdouble =
          ToObjectTest.TestToFromObjectRoundTrip(edec.ToDouble());
        var issingle = (bool)numberinfo["single"].ToObject(typeof(bool));
        CBORObject cbornumbersingle =
          ToObjectTest.TestToFromObjectRoundTrip(edec.ToSingle());
        if ((bool)numberinfo["int32"].ToObject(typeof(bool)) == true) {
          Assert.AreEqual(
  TestCommon.StringToInt((string)numberinfo["integer"].ToObject(typeof(string))),
    cbornumber.ToObject(typeof(int)));
          if (isdouble) {
            Assert.AreEqual(
  TestCommon.StringToInt((string)numberinfo["integer"].ToObject(typeof(string))),
    cbornumberdouble.ToObject(typeof(int)));
          }
          if (issingle) {
            Assert.AreEqual(
  TestCommon.StringToInt((string)numberinfo["integer"].ToObject(typeof(string))),
    cbornumbersingle.ToObject(typeof(int)));
          }
        } else {
          try {
            cbornumber.ToObject(typeof(int));
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
        var isdouble = (bool)numberinfo["double"].ToObject(typeof(bool));
        CBORObject cbornumberdouble =
          ToObjectTest.TestToFromObjectRoundTrip(edec.ToDouble());
        var issingle = (bool)numberinfo["single"].ToObject(typeof(bool));
        CBORObject cbornumbersingle =
          ToObjectTest.TestToFromObjectRoundTrip(edec.ToSingle());
        if ((bool)numberinfo["int64"].ToObject(typeof(bool)) == true) {
          Assert.AreEqual(
   TestCommon.StringToLong((string)numberinfo["integer"].ToObject(typeof(string))),
   cbornumber.ToObject(typeof(long)));
          if (isdouble) {
            Assert.AreEqual(
   TestCommon.StringToLong((string)numberinfo["integer"].ToObject(typeof(string))),
   cbornumberdouble.ToObject(typeof(long)));
          }
          if (issingle) {
            Assert.AreEqual(
   TestCommon.StringToLong((string)numberinfo["integer"].ToObject(typeof(string))),
   cbornumbersingle.ToObject(typeof(long)));
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
    public void TestAsSByte() {
      // not implemented yet
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
        CBORObjectTest.AreEqualExact(
  (float)EDecimal.FromString((string)numberinfo["number"].ToObject(typeof(string))).ToSingle(),
          (float)cbornumber.ToObject(typeof(float)));
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

      if ((ToObjectTest.TestToFromObjectRoundTrip(CBORObject.Null).ToObject(typeof(string))) !=
            null) {
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
    [Timeout(5000)]
    public void TestToObject() {
      var ao = new PODClass();
      CBORObject co = ToObjectTest.TestToFromObjectRoundTrip(ao);
      Assert.IsFalse(co.ContainsKey("PrivatePropA"));
      Assert.IsFalse(co.ContainsKey("privatePropA"));
      Assert.IsFalse(co.ContainsKey("staticPropA"));
      Assert.IsFalse(co.ContainsKey("StaticPropA"));
      co["propA"] = ToObjectTest.TestToFromObjectRoundTrip(999);
      co["floatProp"] = ToObjectTest.TestToFromObjectRoundTrip(3.5);
      co["doubleProp"] = ToObjectTest.TestToFromObjectRoundTrip(4.5);
      co["stringProp"] = ToObjectTest.TestToFromObjectRoundTrip("stringProp");
      co["stringArray"] = CBORObject.NewArray().Add("a").Add("b");
      ao = (PODClass)co.ToObject(typeof(PODClass));
      Assert.AreEqual(999, ao.PropA);
      Assert.AreEqual((float)3.5, ao.FloatProp);
      Assert.AreEqual(4.5, ao.DoubleProp);
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
      List<string> stringList = (List<string>)
        co.ToObject(typeof(List<string>));
      Assert.AreEqual(2, stringList.Count);
      Assert.AreEqual("hello", stringList[0]);
      Assert.AreEqual("world", stringList[1]);
      IList<string> istringList = (IList<string>)
        co.ToObject(typeof(IList<string>));
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
        {
          Assert.Fail();
        }
      }
      if (intDict["b"] != 2) {
        {
          Assert.Fail();
        }
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
    public static void TestCharRoundTrip() {
      for (var i = 0; i < 0x10000; ++i) {
        if ((i & 0xf800) == 0xd800) {
          continue;
        }
        var c = (char)i;
        TestToFromObjectRoundTrip(c);
      }
    }

    [Test]
    public void TestUriRoundTrip() {
      Uri uri = new Uri("http://example.com/path/path2?query#fragment");
      TestToFromObjectRoundTrip(uri);
    }

    [Test]
    public void TestUUIDRoundTrip() {
      var rng = new RandomGenerator();
      for (var i = 0; i < 500; i++) {
        TestToFromObjectRoundTrip(RandomUUID(rng));
      }
    }

    public static object RandomUUID(RandomGenerator rand) {
      string hex = "0123456789ABCDEF";
      StringBuilder sb = new StringBuilder();
      for (var i = 0; i < 8; i++) { sb.Append(hex[rand.UniformInt(16)]); }
      sb.Append('-');
      for (var i = 0; i < 4; i++) { sb.Append(hex[rand.UniformInt(16)]); }
      sb.Append('-');
      for (var i = 0; i < 4; i++) { sb.Append(hex[rand.UniformInt(16)]); }
      sb.Append('-');
      for (var i = 0; i < 4; i++) { sb.Append(hex[rand.UniformInt(16)]); }
      sb.Append('-');
      for (var i = 0; i < 12; i++) { sb.Append(hex[rand.UniformInt(16)]); }
      return new Guid(sb.ToString());
    }

    public static CBORObject TestToFromObjectRoundTrip(object obj) {
      CBORObject cbor = CBORObject.FromObject(obj);
      if (obj != null) {
        object obj2 = null;
        try {
          obj2 = cbor.ToObject(obj.GetType());
        } catch (Exception ex) {
          Assert.Fail(ex.ToString() + "\n" + cbor);
          throw new InvalidOperationException(String.Empty, ex);
        }
        if (!obj.Equals(obj2)) {
          Assert.AreEqual(obj, obj2, cbor.ToString());
        }
      }
      return cbor;
    }
  }
}
