using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeterO;
using PeterO.Cbor;

namespace Test {
  [TestClass]
  public class CBORObjectTest {
    [TestMethod]
    public void TestAbs() {
      Assert.AreEqual(
        CBORObject.FromObject(2),
        CBORObject.FromObject(-2).Abs());
      Assert.AreEqual(
        CBORObject.FromObject(2),
        CBORObject.FromObject(2).Abs());
      Assert.AreEqual(
        CBORObject.FromObject(2.5),
        CBORObject.FromObject(-2.5).Abs());
      Assert.AreEqual(
        CBORObject.FromObject(ExtendedDecimal.FromString("6.63")),
        CBORObject.FromObject(ExtendedDecimal.FromString("-6.63")).Abs());
      Assert.AreEqual(
        CBORObject.FromObject(ExtendedFloat.FromString("2.75")),
        CBORObject.FromObject(ExtendedFloat.FromString("-2.75")).Abs());
      Assert.AreEqual(
        CBORObject.FromObject(ExtendedRational.FromDouble(2.5)),
        CBORObject.FromObject(ExtendedRational.FromDouble(-2.5)).Abs());
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
      try {
        CBORObject.Addition(null, CBORObject.FromObject(2));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Addition(CBORObject.FromObject(2), null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Addition(CBORObject.Null, CBORObject.FromObject(2));
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Addition(CBORObject.FromObject(2), CBORObject.Null);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
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
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Null.AsBigInteger();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsBigInteger();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsBigInteger();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsBigInteger();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewArray().AsBigInteger();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsBigInteger();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        string numberString = numberinfo["number"].AsString();
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberString));
        if (!numberinfo["integer"].Equals(CBORObject.Null)) {
          Assert.AreEqual(
            numberinfo["integer"].AsString(),
            cbornumber.AsBigInteger().ToString());
        } else {
          try {
            cbornumber.AsBigInteger();
            Assert.Fail("Should have failed");
          } catch (OverflowException ex) {
            Console.Write(String.Empty);
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
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsByte();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsByte();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsByte();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsByte();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(String.Empty).AsByte();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"
                     ].AsString()));
        if (numberinfo["byte"].AsBoolean()) {
          Assert.AreEqual(
    BigInteger.fromString(numberinfo["integer"].AsString()).intValueChecked(),
    ((int)cbornumber.AsByte()) & 0xff);
        } else {
          try {
            cbornumber.AsByte();
            Assert.Fail("Should have failed " + cbornumber);
          } catch (OverflowException ex) {
            Console.Write(String.Empty);
} catch (Exception ex) {
            Assert.Fail(ex.ToString() + cbornumber);
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
      }
    }

    private static void AreEqualExact(double a, double b) {
      if (Double.IsNaN(a)) {
        Assert.IsTrue(Double.IsNaN(b));
      } else if (a != b) {
        Assert.Fail("expected " + a + ", got " + b);
      }
    }

    private static void AreEqualExact(float a, float b) {
      if (Single.IsNaN(a)) {
        Assert.IsTrue(Single.IsNaN(b));
      } else if (a != b) {
        Assert.Fail("expected " + a + ", got " + b);
      }
    }

    [TestMethod]
    public void TestAsDouble() {
      try {
        CBORObject.NewArray().AsDouble();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsDouble();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsDouble();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsDouble();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsDouble();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(String.Empty).AsDouble();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"
                     ].AsString()));
        AreEqualExact(
(double)ExtendedDecimal.FromString(numberinfo["number"].AsString()).ToDouble(),
cbornumber.AsDouble());
      }
    }
    [TestMethod]
    public void TestAsExtendedDecimal() {
      Assert.AreEqual(
        ExtendedDecimal.PositiveInfinity,
        CBORObject.FromObject(Single.PositiveInfinity).AsExtendedDecimal());
      Assert.AreEqual(
        ExtendedDecimal.NegativeInfinity,
        CBORObject.FromObject(Single.NegativeInfinity).AsExtendedDecimal());
      Assert.IsTrue(CBORObject.FromObject(Single.NaN).AsExtendedDecimal()
                    .IsNaN());
      Assert.AreEqual(
        ExtendedDecimal.PositiveInfinity,
        CBORObject.FromObject(Double.PositiveInfinity).AsExtendedDecimal());
      Assert.AreEqual(
        ExtendedDecimal.NegativeInfinity,
        CBORObject.FromObject(Double.NegativeInfinity).AsExtendedDecimal());
      Assert.IsTrue(CBORObject.FromObject(Double.NaN).AsExtendedDecimal()
                    .IsNaN());
      try {
        CBORObject.NewArray().AsExtendedDecimal();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsExtendedDecimal();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsExtendedDecimal();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsExtendedDecimal();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsExtendedDecimal();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(String.Empty).AsExtendedDecimal();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestAsExtendedFloat() {
      Assert.AreEqual(
        ExtendedFloat.PositiveInfinity,
        CBORObject.FromObject(Single.PositiveInfinity).AsExtendedFloat());
      Assert.AreEqual(
        ExtendedFloat.NegativeInfinity,
        CBORObject.FromObject(Single.NegativeInfinity).AsExtendedFloat());
      Assert.IsTrue(CBORObject.FromObject(Single.NaN).AsExtendedFloat()
                    .IsNaN());
      Assert.AreEqual(
        ExtendedFloat.PositiveInfinity,
        CBORObject.FromObject(Double.PositiveInfinity).AsExtendedFloat());
      Assert.AreEqual(
        ExtendedFloat.NegativeInfinity,
        CBORObject.FromObject(Double.NegativeInfinity).AsExtendedFloat());
      Assert.IsTrue(CBORObject.FromObject(Double.NaN).AsExtendedFloat()
                    .IsNaN());
    }
    [TestMethod]
    public void TestAsExtendedRational() {
      Assert.AreEqual(
        ExtendedRational.PositiveInfinity,
        CBORObject.FromObject(Single.PositiveInfinity).AsExtendedRational());
      Assert.AreEqual(
        ExtendedRational.NegativeInfinity,
        CBORObject.FromObject(Single.NegativeInfinity).AsExtendedRational());
      Assert.IsTrue(CBORObject.FromObject(Single.NaN).AsExtendedRational()
                    .IsNaN());
      Assert.AreEqual(
        ExtendedRational.PositiveInfinity,
        CBORObject.FromObject(Double.PositiveInfinity).AsExtendedRational());
      Assert.AreEqual(
        ExtendedRational.NegativeInfinity,
        CBORObject.FromObject(Double.NegativeInfinity).AsExtendedRational());
      Assert.IsTrue(CBORObject.FromObject(Double.NaN).AsExtendedRational()
                    .IsNaN());
    }
    [TestMethod]
    public void TestAsInt16() {
      try {
        CBORObject.NewArray().AsInt16();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsInt16();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsInt16();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsInt16();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsInt16();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(String.Empty).AsInt16();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          CBORObject.FromObject(
            ExtendedDecimal.FromString(numberinfo["number"].AsString()));
        if (numberinfo["int16"].AsBoolean()) {
          Assert.AreEqual(
    BigInteger.fromString(numberinfo["integer"].AsString()).intValueChecked(),
    cbornumber.AsInt16());
        } else {
          try {
            cbornumber.AsInt16();
            Assert.Fail("Should have failed " + cbornumber);
          } catch (OverflowException ex) {
            Console.Write(String.Empty);
} catch (Exception ex) {
            Assert.Fail(ex.ToString() + cbornumber);
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
      }
    }

    [TestMethod]
    public void TestAsInt32() {
      try {
        CBORObject.NewArray().AsInt32();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsInt32();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsInt32();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsInt32();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsInt32();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(String.Empty).AsInt32();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        ExtendedDecimal edec =
          ExtendedDecimal.FromString(numberinfo["number"].AsString());
        CBORObject cbornumber = CBORObject.FromObject(edec);
        bool isdouble = numberinfo["double"].AsBoolean();
        CBORObject cbornumberdouble = CBORObject.FromObject(edec.ToDouble());
        bool issingle = numberinfo["single"].AsBoolean();
        CBORObject cbornumbersingle = CBORObject.FromObject(edec.ToSingle());
        if (numberinfo["int32"].AsBoolean()) {
          Assert.AreEqual(
    BigInteger.fromString(numberinfo["integer"].AsString()).intValueChecked(),
    cbornumber.AsInt32());
          if (isdouble) {
            Assert.AreEqual(
    BigInteger.fromString(numberinfo["integer"].AsString()).intValueChecked(),
    cbornumberdouble.AsInt32());
          }
          if (issingle) {
            Assert.AreEqual(
    BigInteger.fromString(numberinfo["integer"].AsString()).intValueChecked(),
    cbornumbersingle.AsInt32());
          }
        } else {
          try {
            cbornumber.AsInt32();
            Assert.Fail("Should have failed " + cbornumber);
          } catch (OverflowException ex) {
            Console.Write(String.Empty);
} catch (Exception ex) {
            Assert.Fail(ex.ToString() + cbornumber);
            throw new InvalidOperationException(String.Empty, ex);
          }
          if (isdouble) {
            try {
              cbornumberdouble.AsInt32();
              Assert.Fail("Should have failed");
            } catch (OverflowException ex) {
              Console.Write(String.Empty);
} catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
          }
          if (issingle) {
            try {
              cbornumbersingle.AsInt32();
              Assert.Fail("Should have failed");
            } catch (OverflowException ex) {
              Console.Write(String.Empty);
} catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
          }
        }
      }
    }
    [TestMethod]
    public void TestAsInt64() {
      try {
        CBORObject.NewArray().AsInt64();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsInt64();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsInt64();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsInt64();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsInt64();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(String.Empty).AsInt64();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        ExtendedDecimal edec =
          ExtendedDecimal.FromString(numberinfo["number"].AsString());
        CBORObject cbornumber = CBORObject.FromObject(edec);
        bool isdouble = numberinfo["double"].AsBoolean();
        CBORObject cbornumberdouble = CBORObject.FromObject(edec.ToDouble());
        bool issingle = numberinfo["single"].AsBoolean();
        CBORObject cbornumbersingle = CBORObject.FromObject(edec.ToSingle());
        if (numberinfo["int64"].AsBoolean()) {
          Assert.AreEqual(
   BigInteger.fromString(numberinfo["integer"].AsString()).longValueChecked(),
   cbornumber.AsInt64());
          if (isdouble) {
            Assert.AreEqual(
   BigInteger.fromString(numberinfo["integer"].AsString()).longValueChecked(),
   cbornumberdouble.AsInt64());
          }
          if (issingle) {
            Assert.AreEqual(
   BigInteger.fromString(numberinfo["integer"].AsString()).longValueChecked(),
   cbornumbersingle.AsInt64());
          }
        } else {
          try {
            cbornumber.AsInt64();
            Assert.Fail("Should have failed " + cbornumber);
          } catch (OverflowException ex) {
            Console.Write(String.Empty);
} catch (Exception ex) {
            Assert.Fail(ex.ToString() + cbornumber);
            throw new InvalidOperationException(String.Empty, ex);
          }
          if (isdouble) {
            try {
              cbornumberdouble.AsInt64();
              Assert.Fail("Should have failed");
            } catch (OverflowException ex) {
              Console.Write(String.Empty);
} catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
          }
          if (issingle) {
            try {
              cbornumbersingle.AsInt64();
              Assert.Fail("Should have failed");
            } catch (OverflowException ex) {
              Console.Write(String.Empty);
} catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
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
      try {
        CBORObject.NewArray().AsSingle();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsSingle();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsSingle();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsSingle();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsSingle();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(String.Empty).AsSingle();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"
                     ].AsString()));
        AreEqualExact(
(float)ExtendedDecimal.FromString(numberinfo["number"].AsString()).ToSingle(),
cbornumber.AsSingle());
      }
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
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"
                     ].AsString()));
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
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"
                     ].AsString()));
        if (numberinfo["int32"].AsBoolean() && numberinfo["isintegral"
                     ].AsBoolean()) {
          Assert.IsTrue(cbornumber.CanFitInInt32());
          Assert.IsTrue(CBORObject.FromObject(cbornumber.AsInt32())
                    .CanFitInInt32());
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
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"
                     ].AsString()));
        if (numberinfo["int64"].AsBoolean() && numberinfo["isintegral"
                     ].AsBoolean()) {
          Assert.IsTrue(cbornumber.CanFitInInt64());
          Assert.IsTrue(CBORObject.FromObject(cbornumber.AsInt64())
                    .CanFitInInt64());
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
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"
                     ].AsString()));
        if (numberinfo["single"].AsBoolean()) {
          Assert.IsTrue(cbornumber.CanFitInSingle());
        } else {
          Assert.IsFalse(cbornumber.CanFitInSingle());
        }
      }
    }
    [TestMethod]
    public void TestCanTruncatedIntFitInInt32() {
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(
        -2,
        11)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(
        -2,
        12)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(
        -2,
        13)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(
        -2,
        14)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(
        -2,
        15)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(
        -2,
        16)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(
        -2,
        17)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(
        -2,
        18)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(
        -2,
        19)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(0).CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.True.CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.FromObject(String.Empty)
                    .CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.NewArray().CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.NewMap().CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.False.CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.Null.CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.Undefined.CanTruncatedIntFitInInt32());
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          CBORObject.FromObject(
            ExtendedDecimal.FromString(numberinfo["number"].AsString()));
        if (numberinfo["int32"].AsBoolean()) {
          Assert.IsTrue(cbornumber.CanTruncatedIntFitInInt32());
        } else {
          Assert.IsFalse(cbornumber.CanTruncatedIntFitInInt32());
        }
      }
    }
    [TestMethod]
    public void TestCanTruncatedIntFitInInt64() {
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(
        -2,
        11)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(
        -2,
        12)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(
        -2,
        13)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(
        -2,
        14)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(
        -2,
        15)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(
        -2,
        16)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(
        -2,
        17)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(
        -2,
        18)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(CBORObject.FromObject(ExtendedFloat.Create(
        -2,
        19)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(CBORObject.FromObject(0).CanTruncatedIntFitInInt64());
      Assert.IsFalse(CBORObject.True.CanTruncatedIntFitInInt64());
      Assert.IsFalse(CBORObject.FromObject(String.Empty)
                    .CanTruncatedIntFitInInt64());
      Assert.IsFalse(CBORObject.NewArray().CanTruncatedIntFitInInt64());
      Assert.IsFalse(CBORObject.NewMap().CanTruncatedIntFitInInt64());
      Assert.IsFalse(CBORObject.False.CanTruncatedIntFitInInt64());
      Assert.IsFalse(CBORObject.Null.CanTruncatedIntFitInInt64());
      Assert.IsFalse(CBORObject.Undefined.CanTruncatedIntFitInInt64());
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"
                     ].AsString()));
        if (numberinfo["int64"].AsBoolean()) {
          Assert.IsTrue(cbornumber.CanTruncatedIntFitInInt64());
        } else {
          Assert.IsFalse(cbornumber.CanTruncatedIntFitInInt64());
        }
      }
    }
    [TestMethod]
    public void TestCompareTo() {
      Assert.AreEqual(1, CBORObject.True.CompareTo(null));
      Assert.AreEqual(1, CBORObject.False.CompareTo(null));
      Assert.AreEqual(1, CBORObject.Null.CompareTo(null));
      Assert.AreEqual(1, CBORObject.NewArray().CompareTo(null));
      Assert.AreEqual(1, CBORObject.NewMap().CompareTo(null));
      Assert.AreEqual(1, CBORObject.FromObject(100).CompareTo(null));
      Assert.AreEqual(1, CBORObject.FromObject(Double.NaN).CompareTo(null));
      CBORTest.CompareTestLess(CBORObject.Undefined, CBORObject.Null);
      CBORTest.CompareTestLess(CBORObject.Null, CBORObject.False);
      CBORTest.CompareTestLess(CBORObject.False, CBORObject.True);
      CBORTest.CompareTestLess(CBORObject.False, CBORObject.FromObject(0));
      CBORTest.CompareTestLess(CBORObject.False, CBORObject.FromSimpleValue(0));
      CBORTest.CompareTestLess(
        CBORObject.FromSimpleValue(0),
        CBORObject.FromSimpleValue(1));
      CBORTest.CompareTestLess(
        CBORObject.FromObject(0),
        CBORObject.FromObject(1));
      CBORTest.CompareTestLess(
        CBORObject.FromObject(0.0f),
        CBORObject.FromObject(1.0f));
      CBORTest.CompareTestLess(
        CBORObject.FromObject(0.0),
        CBORObject.FromObject(1.0));
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
      try {
        CBORObject.DecodeFromBytes(new byte[] { });
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0x1c });
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0x1e });
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xfe });
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestDivide() {
      try {
        CBORObject.Divide(null, CBORObject.FromObject(2));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Divide(CBORObject.FromObject(2), null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Divide(CBORObject.Null, CBORObject.FromObject(2));
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Divide(CBORObject.FromObject(2), CBORObject.Null);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
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
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\"\\ud800\udc00\"");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\"\ud800\\udc00\"");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\"\\U0023\"");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\"\\u002x\"");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\"\\u00xx\"");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\"\\u0xxx\"");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\"\\u0\"");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\"\\u00\"");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\"\\u000\"");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("trbb");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("trub");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("falsb");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("nulb");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[true");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[true,");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[true]!");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[\"\ud800\udc00\"]");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\\udc00\"]");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[\"\ud800\\udc00\"]");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\udc00\"]");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\udc00\ud800\udc00\"]");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\ud800\udc00\"]");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\\udc00\ud800\udc00\"]");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\"]");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[1,2,"); Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[1,2,3"); Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{,\"0\":0,\"1\":1}");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"0\":0,,\"1\":1}");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"0\":0,\"1\":1,}");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[,0,1,2]");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[0,,1,2]");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[0,1,,2]");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[0,1,2,]");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[0001]");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{a:true}");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\"://comment\ntrue}");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\":/*comment*/true}");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{'a':true}");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\":'b'}");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\t\":true}");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\r\":true}");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\n\":true}");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("['a']");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\":\"a\t\"}");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[\"a\\'\"]");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[NaN]");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[+Infinity]");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[-Infinity]");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[Infinity]");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\":\"a\r\"}");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\":\"a\n\"}");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[\"a\t\"]");
        Assert.Fail("Should have failed");
      } catch (CBORException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(CBORObject.True, CBORObject.FromJSONString("true"));
      Assert.AreEqual(CBORObject.False, CBORObject.FromJSONString("false"));
      Assert.AreEqual(CBORObject.Null, CBORObject.FromJSONString("null"));
      Assert.AreEqual(5, CBORObject.FromJSONString(" 5 ").AsInt32());
      {
        string stringTemp = CBORObject.FromJSONString("\"\\/\\b\"").AsString();
        Assert.AreEqual(
        "/\b",
        stringTemp);
      }
    }
    [TestMethod]
    public void TestFromObject() {
      var cborarray = new CBORObject[2];
      cborarray[0] = CBORObject.False;
      cborarray[1] = CBORObject.True;
      CBORObject cbor = CBORObject.FromObject(cborarray);
      Assert.AreEqual(2, cbor.Count);
      Assert.AreEqual(CBORObject.False, cbor[0]);
      Assert.AreEqual(CBORObject.True, cbor[1]);
      TestCommon.AssertRoundTrip(cbor);
      Assert.AreEqual(CBORObject.Null, CBORObject.FromObject((int[])null));
      long[] longarray = { 2, 3 };
      cbor = CBORObject.FromObject(longarray);
      Assert.AreEqual(2, cbor.Count);
      Assert.IsTrue(CBORObject.FromObject(2).CompareTo(cbor[0]) == 0);
      Assert.IsTrue(CBORObject.FromObject(3).CompareTo(cbor[1]) == 0);
      TestCommon.AssertRoundTrip(cbor);
      Assert.AreEqual(
        CBORObject.Null,
        CBORObject.FromObject((ExtendedRational)null));
      Assert.AreEqual(
        CBORObject.Null,
        CBORObject.FromObject((ExtendedDecimal)null));
      Assert.AreEqual(
        CBORObject.FromObject(10),
        CBORObject.FromObject(ExtendedRational.Create(10, 1)));
      try {
        CBORObject.FromObject(ExtendedRational.Create(10, 2));
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestFromObjectAndTag() {
      BigInteger bigvalue = BigInteger.One << 100;
      try {
        CBORObject.FromObjectAndTag(2, bigvalue);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObjectAndTag(2, -1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObjectAndTag(CBORObject.Null, -1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObjectAndTag(CBORObject.Null, 999999);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObjectAndTag(2, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObjectAndTag(2, BigInteger.Zero - BigInteger.One);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestFromSimpleValue() {
      try {
        CBORObject.FromSimpleValue(-1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromSimpleValue(256);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      for (int i = 0; i < 256; ++i) {
        if (i >= 24 && i < 32) {
          try {
            CBORObject.FromSimpleValue(i);
            Assert.Fail("Should have failed");
          } catch (ArgumentException ex) {
            Console.Write(String.Empty);
} catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        } else {
          CBORObject cbor = CBORObject.FromSimpleValue(i);
          Assert.AreEqual(i, cbor.SimpleValue);
        }
      }
    }
    [TestMethod]
    public void TestGetByteString() {
      try {
        CBORObject.True.GetByteString();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(0).GetByteString();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject("test").GetByteString();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.GetByteString();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewArray().GetByteString();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().GetByteString();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
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
      } catch (ArgumentException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        const BigInteger ValueBigintNull = null;
        CBORObject.True.HasTag(ValueBigintNull);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.HasTag(BigInteger.One.negate());
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
        Console.Write(String.Empty);
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
      CBORObject cbor;
      Assert.IsTrue(CBORObject.FromObject(0).IsFinite);
      Assert.IsFalse(CBORObject.FromObject(String.Empty).IsFinite);
      Assert.IsFalse(CBORObject.NewArray().IsFinite);
      Assert.IsFalse(CBORObject.NewMap().IsFinite);
      cbor = CBORObject.True;
      Assert.IsFalse(cbor.IsFinite);
      cbor = CBORObject.False;
      Assert.IsFalse(cbor.IsFinite);
      cbor = CBORObject.Null;
      Assert.IsFalse(cbor.IsFinite);
      cbor = CBORObject.Undefined;
      Assert.IsFalse(cbor.IsFinite);
      Assert.IsFalse(CBORObject.NewMap().IsFinite);
      Assert.IsTrue(CBORObject.FromObject(0).IsFinite);
      Assert.IsTrue(CBORObject.FromObject(2.5).IsFinite);
      Assert.IsFalse(CBORObject.FromObject(Double.PositiveInfinity).IsFinite);
      Assert.IsFalse(CBORObject.FromObject(Double.NegativeInfinity).IsFinite);
      Assert.IsFalse(CBORObject.FromObject(Double.NaN).IsFinite);
      Assert.IsFalse(CBORObject.FromObject(
        ExtendedDecimal.PositiveInfinity).IsFinite);
      Assert.IsFalse(CBORObject.FromObject(
        ExtendedDecimal.NegativeInfinity).IsFinite);
      Assert.IsFalse(CBORObject.FromObject(ExtendedDecimal.NaN).IsFinite);
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"
                     ].AsString()));
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
      CBORObject cbor;
      Assert.IsTrue(CBORObject.FromObject(0).IsIntegral);
      Assert.IsFalse(CBORObject.FromObject(String.Empty).IsIntegral);
      Assert.IsFalse(CBORObject.NewArray().IsIntegral);
      Assert.IsFalse(CBORObject.NewMap().IsIntegral);
      Assert.IsTrue(CBORObject.FromObject(BigInteger.One << 63).IsIntegral);
      Assert.IsTrue(CBORObject.FromObject(BigInteger.One << 64).IsIntegral);
      Assert.IsTrue(CBORObject.FromObject(BigInteger.One << 80).IsIntegral);
      Assert.IsTrue(CBORObject.FromObject(
        ExtendedDecimal.FromString("4444e+800")).IsIntegral);

      Assert.IsFalse(CBORObject.FromObject(
        ExtendedDecimal.FromString("4444e-800")).IsIntegral);
      Assert.IsFalse(CBORObject.FromObject(2.5).IsIntegral);
      Assert.IsFalse(CBORObject.FromObject(999.99).IsIntegral);
      Assert.IsFalse(CBORObject.FromObject(Double.PositiveInfinity).IsIntegral);
      Assert.IsFalse(CBORObject.FromObject(Double.NegativeInfinity).IsIntegral);
      Assert.IsFalse(CBORObject.FromObject(Double.NaN).IsIntegral);
      Assert.IsFalse(CBORObject.FromObject(
        ExtendedDecimal.PositiveInfinity).IsIntegral);
      Assert.IsFalse(CBORObject.FromObject(
        ExtendedDecimal.NegativeInfinity).IsIntegral);
      Assert.IsFalse(CBORObject.FromObject(ExtendedDecimal.NaN).IsIntegral);
      cbor = CBORObject.True;
      Assert.IsFalse(cbor.IsIntegral);
      cbor = CBORObject.False;
      Assert.IsFalse(cbor.IsIntegral);
      cbor = CBORObject.Null;
      Assert.IsFalse(cbor.IsIntegral);
      cbor = CBORObject.Undefined;
      Assert.IsFalse(cbor.IsIntegral);
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(
            numberinfo["number"].AsString()));
        if (numberinfo["isintegral"].AsBoolean()) {
          Assert.IsTrue(cbornumber.IsIntegral);
          Assert.IsFalse(cbornumber.IsPositiveInfinity());
          Assert.IsFalse(cbornumber.IsNegativeInfinity());
          Assert.IsFalse(cbornumber.IsNaN());
          Assert.IsFalse(cbornumber.IsNull);
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
      Assert.IsFalse(CBORObject.PositiveInfinity.IsNaN());
      Assert.IsFalse(CBORObject.NegativeInfinity.IsNaN());
      Assert.IsTrue(CBORObject.NaN.IsNaN());
    }
    [TestMethod]
    public void TestIsNegativeInfinity() {
      Assert.IsFalse(CBORObject.True.IsNegativeInfinity());
      Assert.IsFalse(CBORObject.FromObject(String.Empty).IsNegativeInfinity());
      Assert.IsFalse(CBORObject.NewArray().IsNegativeInfinity());
      Assert.IsFalse(CBORObject.NewMap().IsNegativeInfinity());
      Assert.IsFalse(CBORObject.False.IsNegativeInfinity());
      Assert.IsFalse(CBORObject.Null.IsNegativeInfinity());
      Assert.IsFalse(CBORObject.Undefined.IsNegativeInfinity());
      Assert.IsFalse(CBORObject.PositiveInfinity.IsNegativeInfinity());
      Assert.IsTrue(CBORObject.NegativeInfinity.IsNegativeInfinity());
      Assert.IsFalse(CBORObject.NaN.IsNegativeInfinity());
    }
    [TestMethod]
    public void TestIsNull() {
      Assert.IsFalse(CBORObject.True.IsNull);
      Assert.IsFalse(CBORObject.FromObject(String.Empty).IsNull);
      Assert.IsFalse(CBORObject.NewArray().IsNull);
      Assert.IsFalse(CBORObject.NewMap().IsNull);
      Assert.IsFalse(CBORObject.False.IsNull);
      Assert.IsTrue(CBORObject.Null.IsNull);
      Assert.IsFalse(CBORObject.Undefined.IsNull);
      Assert.IsFalse(CBORObject.PositiveInfinity.IsNull);
      Assert.IsFalse(CBORObject.NegativeInfinity.IsNull);
      Assert.IsFalse(CBORObject.NaN.IsNull);
    }
    [TestMethod]
    public void TestIsPositiveInfinity() {
      Assert.IsFalse(CBORObject.True.IsPositiveInfinity());
      Assert.IsFalse(CBORObject.FromObject(String.Empty).IsPositiveInfinity());
      Assert.IsFalse(CBORObject.NewArray().IsPositiveInfinity());
      Assert.IsFalse(CBORObject.NewMap().IsPositiveInfinity());
      Assert.IsFalse(CBORObject.False.IsPositiveInfinity());
      Assert.IsFalse(CBORObject.Null.IsPositiveInfinity());
      Assert.IsFalse(CBORObject.Undefined.IsPositiveInfinity());
      Assert.IsTrue(CBORObject.PositiveInfinity.IsPositiveInfinity());
      Assert.IsFalse(CBORObject.NegativeInfinity.IsPositiveInfinity());
      Assert.IsFalse(CBORObject.NaN.IsPositiveInfinity());
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
      Assert.IsFalse(CBORObject.True.IsUndefined);
      Assert.IsFalse(CBORObject.FromObject(String.Empty).IsUndefined);
      Assert.IsFalse(CBORObject.NewArray().IsUndefined);
      Assert.IsFalse(CBORObject.NewMap().IsUndefined);
      Assert.IsFalse(CBORObject.False.IsUndefined);
      Assert.IsFalse(CBORObject.Null.IsUndefined);
      Assert.IsTrue(CBORObject.Undefined.IsUndefined);
      Assert.IsFalse(CBORObject.PositiveInfinity.IsUndefined);
      Assert.IsFalse(CBORObject.NegativeInfinity.IsUndefined);
      Assert.IsFalse(CBORObject.NaN.IsUndefined);
    }
    [TestMethod]
    public void TestIsZero() {
      // not implemented yet
    }
    [TestMethod]
    public void TestItem() {
      CBORObject cbor = CBORObject.True;
      try {
        CBORObject cbor2 = cbor[0];
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.False;
      try {
        CBORObject cbor2 = cbor[0];
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.FromObject(0);
      try {
        CBORObject cbor2 = cbor[0];
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.FromObject(2);
      try {
        CBORObject cbor2 = cbor[0];
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewArray();
      try {
        CBORObject cbor2 = cbor[0];
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestKeys() {
      // not implemented yet
    }
    [TestMethod]
    public void TestMultiply() {
      try {
        CBORObject.Multiply(null, CBORObject.FromObject(2));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Multiply(CBORObject.FromObject(2), null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Multiply(CBORObject.Null, CBORObject.FromObject(2));
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Multiply(CBORObject.FromObject(2), CBORObject.Null);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestNegate() {
      try {
        CBORObject.True.Negate();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.Negate();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewArray().Negate();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().Negate();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
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
      CBORObject cbor = CBORObject.FromObjectAndTag(CBORObject.True, 999);
      cbor = CBORObject.FromObjectAndTag(CBORObject.True, 1000);
      Assert.AreEqual((BigInteger)1000, cbor.OutermostTag);
      cbor = CBORObject.True;
      Assert.AreEqual((BigInteger)(-1), cbor.OutermostTag);
    }
    [TestMethod]
    public void TestRead() {
      // not implemented yet
    }
    [TestMethod]
    public void TestReadJSON() {
      try {
      using (var ms = new MemoryStream(new byte[] { 0xef, 0xbb, 0xbf, 0x7b,
        0x7d })) {
        try {
          CBORObject.ReadJSON(ms);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      // whitespace followed by BOM
      using (var ms2 = new MemoryStream(new byte[] { 0x20, 0xef, 0xbb, 0xbf,
        0x7b, 0x7d })) {
        try {
          CBORObject.ReadJSON(ms2);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var ms2a = new MemoryStream(new byte[] { 0x7b, 0x05, 0x7d })) {
        try {
          CBORObject.ReadJSON(ms2a);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var ms2b = new MemoryStream(new byte[] { 0x05, 0x7b, 0x7d })) {
        try {
          CBORObject.ReadJSON(ms2b);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      // two BOMs
      using (var ms3 = new MemoryStream(new byte[] { 0xef, 0xbb, 0xbf, 0xef,
        0xbb, 0xbf, 0x7b, 0x7d })) {
        try {
          CBORObject.ReadJSON(ms3);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
   using (var msjson = new MemoryStream(new byte[] { 0, 0, 0xfe, 0xff, 0, 0,
        0,
                    0x74, 0, 0, 0, 0x72, 0, 0, 0, 0x75, 0, 0, 0,
                    0x65 })) {
        Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
      }
      using (var msjson = new MemoryStream(new byte[] { 0, 0, 0, 0x74, 0, 0,
        0, 0x72, 0,
                    0, 0, 0x75, 0, 0, 0, 0x65 })) {
        Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
      }
      using (var msjson = new MemoryStream(new byte[] { 0xff, 0xfe, 0, 0,
        0x74, 0, 0, 0,
                    0x72, 0, 0, 0, 0x75, 0, 0, 0, 0x65, 0, 0, 0 })) {
        Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
      }
using (var msjson = new MemoryStream(new byte[] { 0x74, 0, 0, 0, 0x72, 0, 0,
        0,
                    0x75, 0, 0, 0, 0x65, 0, 0, 0 })) {
        Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
      }
      using (var msjson = new MemoryStream(new byte[] { 0xfe, 0xff, 0, 0x74,
        0, 0x72, 0,
                    0x75, 0, 0x65 })) {
        Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
      }
      using (var msjson = new MemoryStream(new byte[] { 0, 0x74, 0, 0x72, 0,
        0x75, 0, 0x65 })) {
        Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
      }
using (var msjson = new MemoryStream(new byte[] { 0xff, 0xfe, 0x74, 0, 0x72,
        0,
                    0x75,
                    0, 0x65, 0 })) {
        Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
      }
      using (var msjson = new MemoryStream(new byte[] { 0x74, 0, 0x72, 0,
        0x75, 0, 0x65, 0 })) {
        Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
      }
      using (var msjson = new MemoryStream(new byte[] { 0xef, 0xbb, 0xbf,
        0x74, 0x72, 0x75,
       0x65 })) {
        Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
      }
 using (var msjson = new MemoryStream(new byte[] { 0x74, 0x72, 0x75, 0x65 })) {
        Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
      }
      using (var msjson = new MemoryStream(new byte[] { 0, 0, 0xfe, 0xff, 0,
        0, 0, 0x22,
                    0, 1, 0, 0, 0, 0, 0, 0x22 })) {
        {
          string stringTemp = CBORObject.ReadJSON(msjson).AsString();
          Assert.AreEqual(
          "\ud800\udc00",
          stringTemp);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0, 0, 0, 0x22, 0, 1,
        0, 0, 0, 0,
                    0, 0x22 })) {
        {
          string stringTemp = CBORObject.ReadJSON(msjson).AsString();
          Assert.AreEqual(
          "\ud800\udc00",
          stringTemp);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0xff, 0xfe, 0, 0,
        0x22, 0, 0, 0,
                    0, 0, 1, 0, 0x22, 0, 0, 0 })) {
        {
          string stringTemp = CBORObject.ReadJSON(msjson).AsString();
          Assert.AreEqual(
          "\ud800\udc00",
          stringTemp);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0x22, 0, 0, 0, 0, 0,
        1, 0, 0x22,
                    0,
                    0, 0 })) {
        {
          string stringTemp = CBORObject.ReadJSON(msjson).AsString();
          Assert.AreEqual(
          "\ud800\udc00",
          stringTemp);
        }
      }
using (var msjson = new MemoryStream(new byte[] { 0xfe, 0xff, 0, 0x22, 0xd8,
        0,
                    0xdc, 0, 0, 0x22 })) {
        {
          string stringTemp = CBORObject.ReadJSON(msjson).AsString();
          Assert.AreEqual(
          "\ud800\udc00",
          stringTemp);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0, 0x22, 0xd8, 0,
        0xdc, 0, 0, 0x22 })) {
        {
          string stringTemp = CBORObject.ReadJSON(msjson).AsString();
          Assert.AreEqual(
          "\ud800\udc00",
          stringTemp);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0xff, 0xfe, 0x22, 0,
        0, 0xd8, 0,
                    0xdc, 0x22, 0 })) {
        {
          string stringTemp = CBORObject.ReadJSON(msjson).AsString();
          Assert.AreEqual(
          "\ud800\udc00",
          stringTemp);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0x22, 0, 0, 0xd8, 0,
        0xdc, 0x22, 0 })) {
        {
          string stringTemp = CBORObject.ReadJSON(msjson).AsString();
          Assert.AreEqual(
          "\ud800\udc00",
          stringTemp);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0, 0, 0xfe, 0xff, 0,
        0, 0, 0x22,
                    0, 0, 0xd8, 0, 0, 0, 0, 0x22 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0, 0, 0, 0x22, 0, 0,
        0xd8, 0, 0,
                    0,
                    0, 0x22 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0xff, 0xfe, 0, 0,
        0x22, 0, 0, 0,
                    0, 0xd8, 0, 0, 0x22, 0, 0, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
using (var msjson = new MemoryStream(new byte[] { 0x22, 0, 0, 0, 0, 0xd8, 0,
        0,
                    0x22, 0, 0, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0xfe, 0xff, 0, 0x22,
        0, 0xdc, 0,
                    0xdc, 0, 0, 0x22 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0, 0x22, 0, 0xdc, 0,
        0xdc, 0, 0,
                    0x22 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0xff, 0xfe, 0x22, 0,
        0, 0xdc, 0,
                    0xdc, 0x22, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0x22, 0, 0, 0xdc, 0,
        0xdc, 0x22, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0xfc })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      // Illegal UTF-16
      using (var msjson = new MemoryStream(new byte[] { 0xfe, 0xff, 0x20,
        0x20, 0x20 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0xff, 0xfe, 0x20,
        0x20, 0x20 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
 using (var msjson = new MemoryStream(new byte[] { 0xfe, 0xff, 0xd8, 0x00 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
 using (var msjson = new MemoryStream(new byte[] { 0xfe, 0xff, 0xdc, 0x00 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0xfe, 0xff, 0xd8,
        0x00, 0x20, 0x00 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0xfe, 0xff, 0xdc,
        0x00, 0x20, 0x00 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0xfe, 0xff, 0xd8,
        0x00, 0xd8, 0x00 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0xfe, 0xff, 0xdc,
        0x00, 0xd8, 0x00 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0xfe, 0xff, 0xdc,
        0x00, 0xd8, 0x00, 0xdc, 0x00 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0xfe, 0xff, 0xdc,
        0x00, 0xdc, 0x00 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }

 using (var msjson = new MemoryStream(new byte[] { 0xff, 0xfe, 0x00, 0xd8 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
 using (var msjson = new MemoryStream(new byte[] { 0xff, 0xfe, 0x00, 0xdc })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0xff, 0xfe, 0x00,
        0xd8, 0x00, 0x20 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0xff, 0xfe, 0x00,
        0xdc, 0x00, 0x20 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0xff, 0xfe, 0x00,
        0xd8, 0x00, 0xd8 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0xff, 0xfe, 0x00,
        0xdc, 0x00, 0xd8 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0xff, 0xfe, 0x00,
        0xdc, 0x00, 0xd8, 0x00, 0xdc })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0xff, 0xfe, 0x00,
        0xdc, 0x00, 0xdc })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }

      // Illegal UTF-32
      using (var msjson = new MemoryStream(new byte[] { 0, 0, 0, 0x20, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
    using (var msjson = new MemoryStream(new byte[] { 0, 0, 0, 0x20, 0, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
 using (var msjson = new MemoryStream(new byte[] { 0, 0, 0, 0x20, 0, 0, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0, 0, 0, 0x20, 0, 0,
        0xd8, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0, 0, 0, 0x20, 0, 0,
        0xdc, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0, 0, 0, 0x20, 0,
        0x11, 0x00, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0, 0, 0, 0x20, 0,
        0xff, 0x00, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0, 0, 0, 0x20, 0x1,
        0, 0x00, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
    using (var msjson = new MemoryStream(new byte[] { 0, 0, 0xfe, 0xff, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
 using (var msjson = new MemoryStream(new byte[] { 0, 0, 0xfe, 0xff, 0, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0, 0, 0xfe, 0xff, 0,
        0, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0, 0, 0xfe, 0xff, 0,
        0, 0xd8, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0, 0, 0xfe, 0xff, 0,
        0, 0xdc, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0, 0, 0xfe, 0xff, 0,
        0x11, 0x00, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0, 0, 0xfe, 0xff, 0,
        0xff, 0x00, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var msjson = new MemoryStream(new byte[] { 0, 0, 0xfe, 0xff,
        0x1, 0, 0x00, 0 })) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException ex) {
          Console.Write(String.Empty);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
    } catch (IOException ex) {
      Assert.Fail(ex.Message);
    }
    }

    [TestMethod]
    public void TestRemainder() {
      try {
        CBORObject.Remainder(null, CBORObject.FromObject(2));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Remainder(CBORObject.FromObject(2), null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Remainder(CBORObject.Null, CBORObject.FromObject(2));
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Remainder(CBORObject.FromObject(2), CBORObject.Null);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestRemove() {
      // not implemented yet
    }
    [TestMethod]
    public void TestSet() {
      CBORObject cbor = CBORObject.NewMap().Add("x", 0).Add("y", 1);
      Assert.AreEqual(0, cbor["x"].AsInt32());
      Assert.AreEqual(1, cbor["y"].AsInt32());
      cbor.Set("x", 5).Set("z", 6);
      Assert.AreEqual(5, cbor["x"].AsInt32());
      Assert.AreEqual(6, cbor["z"].AsInt32());
    }
    [TestMethod]
    public void TestSign() {
      try {
        int sign = CBORObject.True.Sign;
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        int sign = CBORObject.False.Sign;
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        int sign = CBORObject.NewArray().Sign;
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        int sign = CBORObject.NewMap().Sign;
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo["number"
                     ].AsString()));
        if (cbornumber.IsNaN()) {
          try {
            Assert.Fail(String.Empty + cbornumber.Sign);
            Assert.Fail("Should have failed");
          } catch (InvalidOperationException ex) {
            Console.Write(String.Empty);
} catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        } else if (numberinfo["number"].AsString().IndexOf('-') == 0) {
          Assert.AreEqual(-1, cbornumber.Sign);
        } else if (numberinfo["number"].AsString().Equals("0")) {
          Assert.AreEqual(0, cbornumber.Sign);
        } else {
          Assert.AreEqual(1, cbornumber.Sign);
        }
      }
    }
    [TestMethod]
    public void TestSimpleValue() {
      // not implemented yet
    }
    [TestMethod]
    public void TestSubtract() {
      try {
        CBORObject.Subtract(null, CBORObject.FromObject(2));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Subtract(CBORObject.FromObject(2), null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Subtract(CBORObject.Null, CBORObject.FromObject(2));
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Subtract(CBORObject.FromObject(2), CBORObject.Null);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [TestMethod]
    public void TestToJSONString() {
      {
        string stringTemp = CBORObject.FromObject(
        "\u2027\u2028\u2029\u202a").ToJSONString();
        Assert.AreEqual(
        "\"\u2027\\u2028\\u2029\u202a\"",
        stringTemp);
      }
      {
        string stringTemp = CBORObject.FromObject(
        "\u0085\ufeff\ufffe\uffff").ToJSONString();
        Assert.AreEqual(
        "\"\\u0085\\uFEFF\\uFFFE\\uFFFF\"",
        stringTemp);
      }
      {
        string stringTemp = CBORObject.True.ToJSONString();
        Assert.AreEqual(
        "true",
        stringTemp);
      }
      {
        string stringTemp = CBORObject.False.ToJSONString();
        Assert.AreEqual(
        "false",
        stringTemp);
      }
      {
        string stringTemp = CBORObject.Null.ToJSONString();
        Assert.AreEqual(
        "null",
        stringTemp);
      }
      {
        string stringTemp =
          CBORObject.FromObject(Single.PositiveInfinity).ToJSONString();
        Assert.AreEqual(
        "null",
        stringTemp);
      }
      {
        string stringTemp =
          CBORObject.FromObject(Single.NegativeInfinity).ToJSONString();
        Assert.AreEqual(
        "null",
        stringTemp);
      }
      {
        string stringTemp = CBORObject.FromObject(Single.NaN).ToJSONString();
        Assert.AreEqual(
        "null",
        stringTemp);
      }
      {
        string stringTemp =
          CBORObject.FromObject(Double.PositiveInfinity).ToJSONString();
        Assert.AreEqual(
        "null",
        stringTemp);
      }
      {
        string stringTemp =
          CBORObject.FromObject(Double.NegativeInfinity).ToJSONString();
        Assert.AreEqual(
        "null",
        stringTemp);
      }
      {
        string stringTemp = CBORObject.FromObject(Double.NaN).ToJSONString();
        Assert.AreEqual(
        "null",
        stringTemp);
      }
      // Base64 tests
      CBORObject o;
      o = CBORObject.FromObjectAndTag(
        new byte[] { 0x9a, 0xd6, 0xf0, 0xe8 }, 22);
      {
        string stringTemp = o.ToJSONString();
        Assert.AreEqual(
        "\"mtbw6A\"",
        stringTemp);
      }
      o = CBORObject.FromObject(new byte[] { 0x9a, 0xd6, 0xf0, 0xe8 });
      {
        string stringTemp = o.ToJSONString();
        Assert.AreEqual(
        "\"mtbw6A\"",
        stringTemp);
      }
      o = CBORObject.FromObjectAndTag(
        new byte[] { 0x9a, 0xd6, 0xf0, 0xe8 },
        23);
      {
        string stringTemp = o.ToJSONString();
        Assert.AreEqual(
        "\"9AD6F0E8\"",
        stringTemp);
      }
      o = CBORObject.FromObject(new byte[] { 0x9a, 0xd6, 0xff, 0xe8 });
      // Encode with Base64URL by default
      {
        string stringTemp = o.ToJSONString();
        Assert.AreEqual(
        "\"mtb_6A\"",
        stringTemp);
      }
      o = CBORObject.FromObjectAndTag(
        new byte[] { 0x9a, 0xd6, 0xff, 0xe8 },
        22);
      // Encode with Base64
      {
        string stringTemp = o.ToJSONString();
        Assert.AreEqual(
        "\"mtb/6A\"",
        stringTemp);
      }
    }
    [TestMethod]
    public void TestToString() {
      {
        string stringTemp = CBORObject.Undefined.ToString();
        Assert.AreEqual(
        "undefined",
        stringTemp);
      }
      {
        string stringTemp = CBORObject.FromSimpleValue(50).ToString();
        Assert.AreEqual(
        "simple(50)",
        stringTemp);
      }
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
