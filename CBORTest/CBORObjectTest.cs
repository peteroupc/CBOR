using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeterO;
using PeterO.Cbor;

namespace Test {
  [TestClass]
  public class CBORObjectTest {
    [TestMethod]
    public void TestAbs() {
      // not implemented yet
    }
    [TestMethod]
    public void TestAdd() {
      // not implemented yet
    }
    [TestMethod]
    public void TestAddConverter() {
      // not implemented yet
    }
    [TestMethod]
    public void TestAddition() {
      // not implemented yet
    }
    [TestMethod]
    public void TestAddTagHandler() {
      // not implemented yet
    }
    [TestMethod]
    public void TestAsBigInteger() {
      try {
        CBORObject.FromObject((object)null).AsBigInteger();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Null.AsBigInteger();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsBigInteger();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsBigInteger();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsBigInteger();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewArray().AsBigInteger();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsBigInteger();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"].AsString()));
        if (!numberinfo["integer"].Equals(CBORObject.Null)) {
          Assert.AreEqual(numberinfo["integer"].AsString(), cbornumber.AsBigInteger().ToString());
        } else {
          try {
            cbornumber.AsBigInteger();
            Assert.Fail("Should have failed");
          } catch (OverflowException) {
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
      }
    }
    [TestMethod]
    public void TestAsBoolean() {
      Assert.IsTrue(CBORObject.True.AsBoolean());
      Assert.IsTrue(CBORObject.FromObject(0).AsBoolean());
      Assert.IsTrue(CBORObject.FromObject(String.Empty).AsBoolean());
      Assert.IsFalse(CBORObject.False.AsBoolean());
      Assert.IsFalse(CBORObject.Null.AsBoolean());
      Assert.IsFalse(CBORObject.Undefined.AsBoolean());
      Assert.IsTrue(CBORObject.NewArray().AsBoolean());
      Assert.IsTrue(CBORObject.NewMap().AsBoolean());
    }
    [TestMethod]
    public void TestAsByte() {
      try {
        CBORObject.NewArray().AsByte();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsByte();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsByte();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsByte();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsByte();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(String.Empty).AsByte();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestAsDouble() {
      try {
        CBORObject.NewArray().AsDouble();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsDouble();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsDouble();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsDouble();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsDouble();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(String.Empty).AsDouble();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestAsExtendedDecimal() {
      Assert.AreEqual(ExtendedDecimal.PositiveInfinity, CBORObject.FromObject(Single.PositiveInfinity).AsExtendedDecimal());
      Assert.AreEqual(ExtendedDecimal.NegativeInfinity, CBORObject.FromObject(Single.NegativeInfinity).AsExtendedDecimal());
      Assert.IsTrue(CBORObject.FromObject(Single.NaN).AsExtendedDecimal().IsNaN());
      Assert.AreEqual(ExtendedDecimal.PositiveInfinity, CBORObject.FromObject(Double.PositiveInfinity).AsExtendedDecimal());
      Assert.AreEqual(ExtendedDecimal.NegativeInfinity, CBORObject.FromObject(Double.NegativeInfinity).AsExtendedDecimal());
      Assert.IsTrue(CBORObject.FromObject(Double.NaN).AsExtendedDecimal().IsNaN());
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
    }
    [TestMethod]
    public void TestAsExtendedFloat() {
      Assert.AreEqual(ExtendedFloat.PositiveInfinity, CBORObject.FromObject(Single.PositiveInfinity).AsExtendedFloat());
      Assert.AreEqual(ExtendedFloat.NegativeInfinity, CBORObject.FromObject(Single.NegativeInfinity).AsExtendedFloat());
      Assert.IsTrue(CBORObject.FromObject(Single.NaN).AsExtendedFloat().IsNaN());
      Assert.AreEqual(ExtendedFloat.PositiveInfinity, CBORObject.FromObject(Double.PositiveInfinity).AsExtendedFloat());
      Assert.AreEqual(ExtendedFloat.NegativeInfinity, CBORObject.FromObject(Double.NegativeInfinity).AsExtendedFloat());
      Assert.IsTrue(CBORObject.FromObject(Double.NaN).AsExtendedFloat().IsNaN());
    }
    [TestMethod]
    public void TestAsExtendedRational() {
      Assert.AreEqual(ExtendedRational.PositiveInfinity, CBORObject.FromObject(Single.PositiveInfinity).AsExtendedRational());
      Assert.AreEqual(ExtendedRational.NegativeInfinity, CBORObject.FromObject(Single.NegativeInfinity).AsExtendedRational());
      Assert.IsTrue(CBORObject.FromObject(Single.NaN).AsExtendedRational().IsNaN());
      Assert.AreEqual(ExtendedRational.PositiveInfinity, CBORObject.FromObject(Double.PositiveInfinity).AsExtendedRational());
      Assert.AreEqual(ExtendedRational.NegativeInfinity, CBORObject.FromObject(Double.NegativeInfinity).AsExtendedRational());
      Assert.IsTrue(CBORObject.FromObject(Double.NaN).AsExtendedRational().IsNaN());
    }
    [TestMethod]
    public void TestAsInt16() {
      try {
        CBORObject.NewArray().AsInt16();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsInt16();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsInt16();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsInt16();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsInt16();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(String.Empty).AsInt16();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [TestMethod]
    public void TestAsInt32() {
      try {
        CBORObject.NewArray().AsInt32();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsInt32();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsInt32();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsInt32();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsInt32();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(String.Empty).AsInt32();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"].AsString()));
        if (numberinfo["int32"].AsBoolean()) {
          Assert.AreEqual(BigInteger.fromString(numberinfo["integer"].AsString()).intValue(), cbornumber.AsInt32());
        } else {
          try {
            cbornumber.AsInt32();
            Assert.Fail("Should have failed " + cbornumber);
          } catch (OverflowException) {
          } catch (Exception ex) {
            Assert.Fail(ex.ToString() + cbornumber);
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
      }
    }
    [TestMethod]
    public void TestAsInt64() {
      try {
        CBORObject.NewArray().AsInt64();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsInt64();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsInt64();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsInt64();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsInt64();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(String.Empty).AsInt64();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"].AsString()));
        if (numberinfo["int64"].AsBoolean()) {
          Assert.AreEqual(BigInteger.fromString(numberinfo["integer"].AsString()).longValue(), cbornumber.AsInt64());
        } else {
          try {
            cbornumber.AsInt64();
            Assert.Fail("Should have failed " + cbornumber);
          } catch (OverflowException) {
          } catch (Exception ex) {
            Assert.Fail(ex.ToString() + cbornumber);
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
      }
    }
    [TestMethod]
    public void TestAsSByte() {
      // not implemented yet
    }
    [TestMethod]
    public void TestAsSingle() {
      // not implemented yet
    }
    [TestMethod]
    public void TestAsString() {
      // not implemented yet
    }
    [TestMethod]
    public void TestAsUInt16() {
      // not implemented yet
    }
    [TestMethod]
    public void TestAsUInt32() {
      // not implemented yet
    }
    [TestMethod]
    public void TestAsUInt64() {
      // not implemented yet
    }
    [TestMethod]
    public void TestCanFitInDouble() {
      Assert.IsTrue(CBORObject.FromObject(0).CanFitInDouble());
      Assert.IsFalse(CBORObject.True.CanFitInDouble());
      Assert.IsFalse(CBORObject.FromObject(String.Empty).CanFitInDouble());
      Assert.IsFalse(CBORObject.NewArray().CanFitInDouble());
      Assert.IsFalse(CBORObject.NewMap().CanFitInDouble());
      Assert.IsFalse(CBORObject.False.CanFitInDouble());
      Assert.IsFalse(CBORObject.Null.CanFitInDouble());
      Assert.IsFalse(CBORObject.Undefined.CanFitInDouble());
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"].AsString()));
        if (numberinfo["double"].AsBoolean()) {
          Assert.IsTrue(cbornumber.CanFitInDouble());
        } else {
          Assert.IsFalse(cbornumber.CanFitInDouble());
        }
      }
    }
    [TestMethod]
    public void TestCanFitInInt32() {
      Assert.IsTrue(CBORObject.FromObject(0).CanFitInInt32());
      Assert.IsFalse(CBORObject.True.CanFitInInt32());
      Assert.IsFalse(CBORObject.FromObject(String.Empty).CanFitInInt32());
      Assert.IsFalse(CBORObject.NewArray().CanFitInInt32());
      Assert.IsFalse(CBORObject.NewMap().CanFitInInt32());
      Assert.IsFalse(CBORObject.False.CanFitInInt32());
      Assert.IsFalse(CBORObject.Null.CanFitInInt32());
      Assert.IsFalse(CBORObject.Undefined.CanFitInInt32());
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"].AsString()));
        if (numberinfo["int32"].AsBoolean() && numberinfo["isintegral"].AsBoolean()) {
          Assert.IsTrue(cbornumber.CanFitInInt32());
        } else {
          Assert.IsFalse(cbornumber.CanFitInInt32());
        }
      }
    }
    [TestMethod]
    public void TestCanFitInInt64() {
      Assert.IsTrue(CBORObject.FromObject(0).CanFitInSingle());
      Assert.IsFalse(CBORObject.True.CanFitInSingle());
      Assert.IsFalse(CBORObject.FromObject(String.Empty).CanFitInSingle());
      Assert.IsFalse(CBORObject.NewArray().CanFitInSingle());
      Assert.IsFalse(CBORObject.NewMap().CanFitInSingle());
      Assert.IsFalse(CBORObject.False.CanFitInSingle());
      Assert.IsFalse(CBORObject.Null.CanFitInSingle());
      Assert.IsFalse(CBORObject.Undefined.CanFitInSingle());
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"].AsString()));
        if (numberinfo["int64"].AsBoolean() && numberinfo["isintegral"].AsBoolean()) {
          Assert.IsTrue(cbornumber.CanFitInInt64());
        } else {
          Assert.IsFalse(cbornumber.CanFitInInt64());
        }
      }
    }
    [TestMethod]
    public void TestCanFitInSingle() {
      Assert.IsTrue(CBORObject.FromObject(0).CanFitInSingle());
      Assert.IsFalse(CBORObject.True.CanFitInSingle());
      Assert.IsFalse(CBORObject.FromObject(String.Empty).CanFitInSingle());
      Assert.IsFalse(CBORObject.NewArray().CanFitInSingle());
      Assert.IsFalse(CBORObject.NewMap().CanFitInSingle());
      Assert.IsFalse(CBORObject.False.CanFitInSingle());
      Assert.IsFalse(CBORObject.Null.CanFitInSingle());
      Assert.IsFalse(CBORObject.Undefined.CanFitInSingle());
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"].AsString()));
        if (numberinfo["single"].AsBoolean()) {
          Assert.IsTrue(cbornumber.CanFitInSingle());
        } else {
          Assert.IsFalse(cbornumber.CanFitInSingle());
        }
      }
    }
    [TestMethod]
    public void TestCanTruncatedIntFitInInt32() {
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(-2, 11)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(-2, 12)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(-2, 13)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(-2, 14)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(-2, 15)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(-2, 16)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(-2, 17)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(-2, 18)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(-2, 19)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(0).CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.True.CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.FromObject(String.Empty).CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.NewArray().CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.NewMap().CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.False.CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.Null.CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.Undefined.CanTruncatedIntFitInInt32());
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"].AsString()));
        if (numberinfo["int32"].AsBoolean()) {
          Assert.IsTrue(cbornumber.CanTruncatedIntFitInInt32());
        } else {
          Assert.IsFalse(cbornumber.CanTruncatedIntFitInInt32());
        }
      }
    }
    [TestMethod]
    public void TestCanTruncatedIntFitInInt64() {
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(-2, 11)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(-2, 12)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(-2, 13)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(-2, 14)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(-2, 15)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(-2, 16)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(-2, 17)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(-2, 18)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(-2, 19)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(CBORObject.FromObject(0).CanTruncatedIntFitInInt64());
      Assert.IsFalse(CBORObject.True.CanTruncatedIntFitInInt64());
      Assert.IsFalse(CBORObject.FromObject(String.Empty).CanTruncatedIntFitInInt64());
      Assert.IsFalse(CBORObject.NewArray().CanTruncatedIntFitInInt64());
      Assert.IsFalse(CBORObject.NewMap().CanTruncatedIntFitInInt64());
      Assert.IsFalse(CBORObject.False.CanTruncatedIntFitInInt64());
      Assert.IsFalse(CBORObject.Null.CanTruncatedIntFitInInt64());
      Assert.IsFalse(CBORObject.Undefined.CanTruncatedIntFitInInt64());
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"].AsString()));
        if (numberinfo["int64"].AsBoolean()) {
          Assert.IsTrue(cbornumber.CanTruncatedIntFitInInt64());
        } else {
          Assert.IsFalse(cbornumber.CanTruncatedIntFitInInt64());
        }
      }
    }
    [TestMethod]
    public void TestCompareTo() {
      // not implemented yet
    }
    [TestMethod]
    public void TestContainsKey() {
      // not implemented yet
    }
    [TestMethod]
    public void TestCount() {
      Assert.AreEqual(0, CBORObject.True.Count);
      Assert.AreEqual(0, CBORObject.False.Count);
      Assert.AreEqual(0, CBORObject.NewArray().Count);
      Assert.AreEqual(0, CBORObject.NewMap().Count);
    }
    [TestMethod]
    public void TestDecodeFromBytes() {
      // not implemented yet
    }
    [TestMethod]
    public void TestDivide() {
      // not implemented yet
    }
    [TestMethod]
    public void TestEncodeToBytes() {
      // not implemented yet
    }
    [TestMethod]
    public void TestEquals() {
      // not implemented yet
    }
    [TestMethod]
    public void TestFromJSONString() {
      try {
        CBORObject.FromJSONString("\"\\uxxxx\"");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\"\\ud800\udc00\"");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\"\ud800\\udc00\"");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\"\\U0023\"");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\"\\u002x\"");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\"\\u00xx\"");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\"\\u0xxx\"");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\"\\u0\"");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\"\\u00\"");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\"\\u000\"");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("trbb");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("trub");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("falsb");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("nulb");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[true");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[true,");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[true]!");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
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
      try {
        CBORObject.FromJSONString("{\"0\":0,,\"1\":1}"); Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"0\":0,\"1\":1,}"); Assert.Fail("Should have failed");
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
      Assert.AreEqual(CBORObject.True, CBORObject.FromJSONString("true"));
      Assert.AreEqual(CBORObject.False, CBORObject.FromJSONString("false"));
      Assert.AreEqual(CBORObject.Null, CBORObject.FromJSONString("null"));
      Assert.AreEqual(5, CBORObject.FromJSONString(" 5 ").AsInt32());
      Assert.AreEqual("/\b", CBORObject.FromJSONString("\"\\/\\b\"").AsString());
    }
    [TestMethod]
    public void TestFromObject() {
      // not implemented yet
    }
    [TestMethod]
    public void TestFromObjectAndTag() {
      CBORObject.FromObjectAndTag(CBORObject.Null, (BigInteger.One << 64) - BigInteger.One);
    }
    [TestMethod]
    public void TestFromSimpleValue() {
      // not implemented yet
    }
    [TestMethod]
    public void TestGetByteString() {
      // not implemented yet
    }
    [TestMethod]
    public void TestGetHashCode() {
      // not implemented yet
    }
    [TestMethod]
    public void TestGetTags() {
      // not implemented yet
    }
    [TestMethod]
    public void TestHasTag() {
      try {
        CBORObject.True.HasTag(-1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger bigintNull = null;
        CBORObject.True.HasTag(bigintNull);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.HasTag(BigInteger.One.negate());
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.IsFalse(CBORObject.True.HasTag(0));
      Assert.IsFalse(CBORObject.True.HasTag(BigInteger.Zero));
    }
    [TestMethod]
    public void TestInnermostTag() {
      // not implemented yet
    }
    [TestMethod]
    public void TestInsert() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsFalse() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsFinite() {
      Assert.IsTrue(CBORObject.FromObject(0).IsFinite);
      Assert.IsFalse(CBORObject.True.IsFinite);
      Assert.IsFalse(CBORObject.FromObject(String.Empty).IsFinite);
      Assert.IsFalse(CBORObject.NewArray().IsFinite);
      Assert.IsFalse(CBORObject.NewMap().IsFinite);
      Assert.IsFalse(CBORObject.False.IsFinite);
      Assert.IsFalse(CBORObject.Null.IsFinite);
      Assert.IsFalse(CBORObject.Undefined.IsFinite);
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"].AsString()));
        if (!numberinfo["integer"].Equals(CBORObject.Null)) {
          Assert.IsTrue(cbornumber.IsFinite);
        } else {
          Assert.IsFalse(cbornumber.IsFinite);
        }
      }
    }
    [TestMethod]
    public void TestIsInfinity() {
      // not implemented yet
    }

    public static CBORObject GetNumberData() {
      return new AppResources("Resources").GetJSON("numbers");
    }

    [TestMethod]
    public void TestIsIntegral() {
      Assert.IsTrue(CBORObject.FromObject(0).IsIntegral);
      Assert.IsFalse(CBORObject.True.IsIntegral);
      Assert.IsFalse(CBORObject.FromObject(String.Empty).IsIntegral);
      Assert.IsFalse(CBORObject.NewArray().IsIntegral);
      Assert.IsFalse(CBORObject.NewMap().IsIntegral);
      Assert.IsFalse(CBORObject.False.IsIntegral);
      Assert.IsFalse(CBORObject.Null.IsIntegral);
      Assert.IsFalse(CBORObject.Undefined.IsIntegral);
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"].AsString()));
        if (numberinfo["isintegral"].AsBoolean()) {
          Assert.IsTrue(cbornumber.IsIntegral);
        } else {
          Assert.IsFalse(cbornumber.IsIntegral);
        }
      }
    }
    [TestMethod]
    public void TestIsNaN() {
      Assert.IsFalse(CBORObject.True.IsNaN());
      Assert.IsFalse(CBORObject.FromObject(String.Empty).IsNaN());
      Assert.IsFalse(CBORObject.NewArray().IsNaN());
      Assert.IsFalse(CBORObject.NewMap().IsNaN());
      Assert.IsFalse(CBORObject.False.IsNaN());
      Assert.IsFalse(CBORObject.Null.IsNaN());
      Assert.IsFalse(CBORObject.Undefined.IsNaN());
    }
    [TestMethod]
    public void TestIsNegativeInfinity() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsNull() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsPositiveInfinity() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsTagged() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsTrue() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsUndefined() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsZero() {
      // not implemented yet
    }
    [TestMethod]
    public void TestItem() {
      // not implemented yet
    }
    [TestMethod]
    public void TestKeys() {
      // not implemented yet
    }
    [TestMethod]
    public void TestMultiply() {
      // not implemented yet
    }
    [TestMethod]
    public void TestNegate() {
      // not implemented yet
    }
    [TestMethod]
    public void TestNewArray() {
      // not implemented yet
    }
    [TestMethod]
    public void TestNewMap() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOperatorAddition() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOperatorDivision() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOperatorModulus() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOperatorMultiply() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOperatorSubtraction() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOutermostTag() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRead() {
      // not implemented yet
    }
    [TestMethod]
    public void TestReadJSON() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRemainder() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRemove() {
      // not implemented yet
    }
    [TestMethod]
    public void TestSet() {
      // not implemented yet
    }
    [TestMethod]
    public void TestSign() {
      // not implemented yet
    }
    [TestMethod]
    public void TestSimpleValue() {
      // not implemented yet
    }
    [TestMethod]
    public void TestSubtract() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToJSONString() {
      Assert.AreEqual("true", CBORObject.True.ToJSONString());
      Assert.AreEqual("false", CBORObject.False.ToJSONString());
      Assert.AreEqual("null", CBORObject.Null.ToJSONString());
      Assert.AreEqual("null", CBORObject.FromObject(Single.PositiveInfinity).ToJSONString());
      Assert.AreEqual("null", CBORObject.FromObject(Single.NegativeInfinity).ToJSONString());
      Assert.AreEqual("null", CBORObject.FromObject(Single.NaN).ToJSONString());
      Assert.AreEqual("null", CBORObject.FromObject(Double.PositiveInfinity).ToJSONString());
      Assert.AreEqual("null", CBORObject.FromObject(Double.NegativeInfinity).ToJSONString());
      Assert.AreEqual("null", CBORObject.FromObject(Double.NaN).ToJSONString());
    }
    [TestMethod]
    public void TestToString() {
      // not implemented yet
    }
    [TestMethod]
    public void TestType() {
      // not implemented yet
    }
    [TestMethod]
    public void TestUntag() {
      // not implemented yet
    }
    [TestMethod]
    public void TestUntagOne() {
      // not implemented yet
    }
    [TestMethod]
    public void TestValues() {
      // not implemented yet
    }
    [TestMethod]
    public void TestWrite() {
      // not implemented yet
    }
    [TestMethod]
    public void TestWriteJSON() {
      // not implemented yet
    }
    [TestMethod]
    public void TestWriteJSONTo() {
      // not implemented yet
    }
    [TestMethod]
    public void TestWriteTo() {
      // not implemented yet
    }
  }
}
