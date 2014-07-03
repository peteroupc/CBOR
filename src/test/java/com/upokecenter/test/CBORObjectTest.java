package com.upokecenter.test;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;
import com.upokecenter.cbor.*;

  public class CBORObjectTest {
    @Test
    public void TestAbs() {
      Assert.assertEquals(
        CBORObject.FromObject(2),
        CBORObject.FromObject(-2).Abs());
      Assert.assertEquals(
        CBORObject.FromObject(2),
        CBORObject.FromObject(2).Abs());
      Assert.assertEquals(
        CBORObject.FromObject(2.5),
        CBORObject.FromObject(-2.5).Abs());
      Assert.assertEquals(
        CBORObject.FromObject(ExtendedDecimal.FromString("6.63")),
        CBORObject.FromObject(ExtendedDecimal.FromString("-6.63")).Abs());
      Assert.assertEquals(
        CBORObject.FromObject(ExtendedFloat.FromString("2.75")),
        CBORObject.FromObject(ExtendedFloat.FromString("-2.75")).Abs());
      Assert.assertEquals(
        CBORObject.FromObject(ExtendedRational.FromDouble(2.5)),
        CBORObject.FromObject(ExtendedRational.FromDouble(-2.5)).Abs());
    }
    @Test
    public void TestAdd() {
      // not implemented yet
    }
    @Test
    public void TestAddConverter() {
      // not implemented yet
    }
    @Test
    public void TestAddition() {
      // not implemented yet
    }
    @Test
    public void TestAddTagHandler() {
      // not implemented yet
    }
    @Test
    public void TestAsBigInteger() {
      try {
        CBORObject.FromObject((Object)null).AsBigInteger();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.Null.AsBigInteger();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.True.AsBigInteger();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.False.AsBigInteger();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.Undefined.AsBigInteger();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.NewArray().AsBigInteger();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.NewMap().AsBigInteger();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number"
).AsString()));
        if (!numberinfo.get("integer").equals(CBORObject.Null)) {
          Assert.assertEquals(numberinfo.get("integer").AsString(),
                          cbornumber.AsBigInteger().toString());
        } else {
          try {
            cbornumber.AsBigInteger();
            Assert.fail("Should have failed");
          } catch (ArithmeticException ex) {
          } catch (Exception ex) {
            Assert.fail(ex.toString());
            throw new IllegalStateException("", ex);
          }
        }
      }
    }
    @Test
    public void TestAsBoolean() {
      if (!(CBORObject.True.AsBoolean()))Assert.fail();
      if (!(CBORObject.FromObject(0).AsBoolean()))Assert.fail();
      if (!(CBORObject.FromObject("").AsBoolean()))Assert.fail();
      if (CBORObject.False.AsBoolean())Assert.fail();
      if (CBORObject.Null.AsBoolean())Assert.fail();
      if (CBORObject.Undefined.AsBoolean())Assert.fail();
      if (!(CBORObject.NewArray().AsBoolean()))Assert.fail();
      if (!(CBORObject.NewMap().AsBoolean()))Assert.fail();
    }
    @Test
    public void TestAsByte() {
      try {
        CBORObject.NewArray().AsByte();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.NewMap().AsByte();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.True.AsByte();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.False.AsByte();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.Undefined.AsByte();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject("").AsByte();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number"
).AsString()));
        if (numberinfo.get("byte").AsBoolean()) {
          Assert.assertEquals(BigInteger.fromString(numberinfo.get("integer"
).AsString()).intValue(), ((int)cbornumber.AsByte()) &
                                                            0xff);
        } else {
          try {
            cbornumber.AsByte();
            Assert.fail("Should have failed " + cbornumber);
          } catch (ArithmeticException ex) {
          } catch (Exception ex) {
            Assert.fail(ex.toString() + cbornumber);
            throw new IllegalStateException("", ex);
          }
        }
      }
    }

    @Test
    public void TestAsDouble() {
      try {
        CBORObject.NewArray().AsDouble();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.NewMap().AsDouble();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.True.AsDouble();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.False.AsDouble();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.Undefined.AsDouble();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject("").AsDouble();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number"
).AsString()));
        Assert.assertEquals((double)ExtendedDecimal.FromString(numberinfo.get("number"
).AsString()).ToDouble(), cbornumber.AsDouble(), 0);
      }
    }
    @Test
    public void TestAsExtendedDecimal() {
      Assert.assertEquals(ExtendedDecimal.PositiveInfinity,
CBORObject.FromObject(Float.POSITIVE_INFINITY) .AsExtendedDecimal());
      Assert.assertEquals(ExtendedDecimal.NegativeInfinity,
CBORObject.FromObject(Float.NEGATIVE_INFINITY) .AsExtendedDecimal());
      if (!(CBORObject.FromObject(Float.NaN) .AsExtendedDecimal()
                    .IsNaN()))Assert.fail();
      Assert.assertEquals(ExtendedDecimal.PositiveInfinity,
CBORObject.FromObject(Double.POSITIVE_INFINITY) .AsExtendedDecimal());
      Assert.assertEquals(ExtendedDecimal.NegativeInfinity,
CBORObject.FromObject(Double.NEGATIVE_INFINITY) .AsExtendedDecimal());
      if (!(CBORObject.FromObject(Double.NaN) .AsExtendedDecimal()
                    .IsNaN()))Assert.fail();
      try {
        CBORObject.NewArray().AsExtendedDecimal();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.NewMap().AsExtendedDecimal();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.True.AsExtendedDecimal();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.False.AsExtendedDecimal();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.Undefined.AsExtendedDecimal();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject("").AsExtendedDecimal();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestAsExtendedFloat() {
      Assert.assertEquals(ExtendedFloat.PositiveInfinity,
CBORObject.FromObject(Float.POSITIVE_INFINITY) .AsExtendedFloat());
      Assert.assertEquals(ExtendedFloat.NegativeInfinity,
CBORObject.FromObject(Float.NEGATIVE_INFINITY) .AsExtendedFloat());
      if (!(CBORObject.FromObject(Float.NaN) .AsExtendedFloat()
                    .IsNaN()))Assert.fail();
      Assert.assertEquals(ExtendedFloat.PositiveInfinity,
CBORObject.FromObject(Double.POSITIVE_INFINITY) .AsExtendedFloat());
      Assert.assertEquals(ExtendedFloat.NegativeInfinity,
CBORObject.FromObject(Double.NEGATIVE_INFINITY) .AsExtendedFloat());
      if (!(CBORObject.FromObject(Double.NaN) .AsExtendedFloat()
                    .IsNaN()))Assert.fail();
    }
    @Test
    public void TestAsExtendedRational() {
      Assert.assertEquals(ExtendedRational.PositiveInfinity,
CBORObject.FromObject(Float.POSITIVE_INFINITY) .AsExtendedRational());
      Assert.assertEquals(ExtendedRational.NegativeInfinity,
CBORObject.FromObject(Float.NEGATIVE_INFINITY) .AsExtendedRational());
      if (!(CBORObject.FromObject(Float.NaN) .AsExtendedRational()
                    .IsNaN()))Assert.fail();
      Assert.assertEquals(ExtendedRational.PositiveInfinity,
CBORObject.FromObject(Double.POSITIVE_INFINITY) .AsExtendedRational());
      Assert.assertEquals(ExtendedRational.NegativeInfinity,
CBORObject.FromObject(Double.NEGATIVE_INFINITY) .AsExtendedRational());
      if (!(CBORObject.FromObject(Double.NaN) .AsExtendedRational()
                    .IsNaN()))Assert.fail();
    }
    @Test
    public void TestAsInt16() {
      try {
        CBORObject.NewArray().AsInt16();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.NewMap().AsInt16();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.True.AsInt16();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.False.AsInt16();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.Undefined.AsInt16();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject("").AsInt16();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number"
).AsString()));
        if (numberinfo.get("int16").AsBoolean()) {
          Assert.assertEquals(BigInteger.fromString(numberinfo.get("integer"
).AsString()).intValue(),
  cbornumber.AsInt16());
        } else {
          try {
            cbornumber.AsInt16();
            Assert.fail("Should have failed " + cbornumber);
          } catch (ArithmeticException ex) {
          } catch (Exception ex) {
            Assert.fail(ex.toString() + cbornumber);
            throw new IllegalStateException("", ex);
          }
        }
      }
    }

    @Test
    public void TestAsInt32() {
      try {
        CBORObject.NewArray().AsInt32();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.NewMap().AsInt32();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.True.AsInt32();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.False.AsInt32();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.Undefined.AsInt32();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject("").AsInt32();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        ExtendedDecimal edec =
          ExtendedDecimal.FromString(numberinfo.get("number").AsString());
        CBORObject cbornumber = CBORObject.FromObject(edec);
        boolean isdouble = numberinfo.get("double").AsBoolean();
        CBORObject cbornumberdouble = CBORObject.FromObject(edec.ToDouble());
        boolean issingle = numberinfo.get("single").AsBoolean();
        CBORObject cbornumbersingle = CBORObject.FromObject(edec.ToSingle());
        if (numberinfo.get("int32").AsBoolean()) {
          Assert.assertEquals(BigInteger.fromString(numberinfo.get("integer"
).AsString()).intValue(),
  cbornumber.AsInt32());
          if (isdouble) {
            Assert.assertEquals(BigInteger.fromString(numberinfo.get("integer"
).AsString()).intValue(),
  cbornumberdouble.AsInt32());
          }
          if (issingle) {
            Assert.assertEquals(BigInteger.fromString(numberinfo.get("integer"
).AsString()).intValue(),
  cbornumbersingle.AsInt32());
          }
        } else {
          try {
            cbornumber.AsInt32();
            Assert.fail("Should have failed " + cbornumber);
          } catch (ArithmeticException ex) {
          } catch (Exception ex) {
            Assert.fail(ex.toString() + cbornumber);
            throw new IllegalStateException("", ex);
          }
          if (isdouble) {
            try {
              cbornumberdouble.AsInt32();
              Assert.fail("Should have failed");
            } catch (ArithmeticException ex) {
            } catch (Exception ex) {
              Assert.fail(ex.toString());
              throw new IllegalStateException("", ex);
            }
          }
          if (issingle) {
            try {
              cbornumbersingle.AsInt32();
              Assert.fail("Should have failed");
            } catch (ArithmeticException ex) {
            } catch (Exception ex) {
              Assert.fail(ex.toString());
              throw new IllegalStateException("", ex);
            }
          }
        }
      }
    }
    @Test
    public void TestAsInt64() {
      try {
        CBORObject.NewArray().AsInt64();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.NewMap().AsInt64();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.True.AsInt64();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.False.AsInt64();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.Undefined.AsInt64();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject("").AsInt64();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        ExtendedDecimal edec =
          ExtendedDecimal.FromString(numberinfo.get("number").AsString());
        CBORObject cbornumber = CBORObject.FromObject(edec);
        boolean isdouble = numberinfo.get("double").AsBoolean();
        CBORObject cbornumberdouble = CBORObject.FromObject(edec.ToDouble());
        boolean issingle = numberinfo.get("single").AsBoolean();
        CBORObject cbornumbersingle = CBORObject.FromObject(edec.ToSingle());
        if (numberinfo.get("int64").AsBoolean()) {
          Assert.assertEquals(BigInteger.fromString(numberinfo.get("integer"
).AsString()).longValue(),
  cbornumber.AsInt64());
          if (isdouble) {
            Assert.assertEquals(BigInteger.fromString(numberinfo.get("integer"
).AsString()).longValue(),
  cbornumberdouble.AsInt64());
          }
          if (issingle) {
            Assert.assertEquals(BigInteger.fromString(numberinfo.get("integer"
).AsString()).longValue(),
  cbornumbersingle.AsInt64());
          }
        } else {
          try {
            cbornumber.AsInt64();
            Assert.fail("Should have failed " + cbornumber);
          } catch (ArithmeticException ex) {
          } catch (Exception ex) {
            Assert.fail(ex.toString() + cbornumber);
            throw new IllegalStateException("", ex);
          }
          if (isdouble) {
            try {
              cbornumberdouble.AsInt64();
              Assert.fail("Should have failed");
            } catch (ArithmeticException ex) {
            } catch (Exception ex) {
              Assert.fail(ex.toString());
              throw new IllegalStateException("", ex);
            }
          }
          if (issingle) {
            try {
              cbornumbersingle.AsInt64();
              Assert.fail("Should have failed");
            } catch (ArithmeticException ex) {
            } catch (Exception ex) {
              Assert.fail(ex.toString());
              throw new IllegalStateException("", ex);
            }
          }
        }
      }
    }
    @Test
    public void TestAsSByte() {
      // not implemented yet
    }
    @Test
    public void TestAsSingle() {
      try {
        CBORObject.NewArray().AsSingle();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.NewMap().AsSingle();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.True.AsSingle();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.False.AsSingle();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.Undefined.AsSingle();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject("").AsSingle();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number"
).AsString()));
        Assert.assertEquals((float)ExtendedDecimal.FromString(numberinfo.get("number"
).AsString()).ToSingle(), cbornumber.AsSingle(), 0f);
      }
    }
    @Test
    public void TestAsString() {
      // not implemented yet
    }
    @Test
    public void TestAsUInt16() {
      // not implemented yet
    }
    @Test
    public void TestAsUInt32() {
      // not implemented yet
    }
    @Test
    public void TestAsUInt64() {
      // not implemented yet
    }
    @Test
    public void TestCanFitInDouble() {
      if (!(CBORObject.FromObject(0).CanFitInDouble()))Assert.fail();
      if (CBORObject.True.CanFitInDouble())Assert.fail();
      if (CBORObject.FromObject("").CanFitInDouble())Assert.fail();
      if (CBORObject.NewArray().CanFitInDouble())Assert.fail();
      if (CBORObject.NewMap().CanFitInDouble())Assert.fail();
      if (CBORObject.False.CanFitInDouble())Assert.fail();
      if (CBORObject.Null.CanFitInDouble())Assert.fail();
      if (CBORObject.Undefined.CanFitInDouble())Assert.fail();
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number"
).AsString()));
        if (numberinfo.get("double").AsBoolean()) {
          if (!(cbornumber.CanFitInDouble()))Assert.fail();
        } else {
          if (cbornumber.CanFitInDouble())Assert.fail();
        }
      }
    }
    @Test
    public void TestCanFitInInt32() {
      if (!(CBORObject.FromObject(0).CanFitInInt32()))Assert.fail();
      if (CBORObject.True.CanFitInInt32())Assert.fail();
      if (CBORObject.FromObject("").CanFitInInt32())Assert.fail();
      if (CBORObject.NewArray().CanFitInInt32())Assert.fail();
      if (CBORObject.NewMap().CanFitInInt32())Assert.fail();
      if (CBORObject.False.CanFitInInt32())Assert.fail();
      if (CBORObject.Null.CanFitInInt32())Assert.fail();
      if (CBORObject.Undefined.CanFitInInt32())Assert.fail();
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number"
).AsString()));
        if (numberinfo.get("int32").AsBoolean() && numberinfo.get("isintegral"
).AsBoolean()) {
          if (!(cbornumber.CanFitInInt32()))Assert.fail();
   if (!(CBORObject.FromObject(cbornumber.AsInt32())
            .CanFitInInt32()))Assert.fail();
        } else {
          if (cbornumber.CanFitInInt32())Assert.fail();
        }
      }
    }
    @Test
    public void TestCanFitInInt64() {
      if (!(CBORObject.FromObject(0).CanFitInSingle()))Assert.fail();
      if (CBORObject.True.CanFitInSingle())Assert.fail();
      if (CBORObject.FromObject("").CanFitInSingle())Assert.fail();
      if (CBORObject.NewArray().CanFitInSingle())Assert.fail();
      if (CBORObject.NewMap().CanFitInSingle())Assert.fail();
      if (CBORObject.False.CanFitInSingle())Assert.fail();
      if (CBORObject.Null.CanFitInSingle())Assert.fail();
      if (CBORObject.Undefined.CanFitInSingle())Assert.fail();
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number"
).AsString()));
        if (numberinfo.get("int64").AsBoolean() && numberinfo.get("isintegral"
).AsBoolean()) {
          if (!(cbornumber.CanFitInInt64()))Assert.fail();
   if (!(CBORObject.FromObject(cbornumber.AsInt64())
            .CanFitInInt64()))Assert.fail();
        } else {
          if (cbornumber.CanFitInInt64())Assert.fail();
        }
      }
    }
    @Test
    public void TestCanFitInSingle() {
      if (!(CBORObject.FromObject(0).CanFitInSingle()))Assert.fail();
      if (CBORObject.True.CanFitInSingle())Assert.fail();
      if (CBORObject.FromObject("").CanFitInSingle())Assert.fail();
      if (CBORObject.NewArray().CanFitInSingle())Assert.fail();
      if (CBORObject.NewMap().CanFitInSingle())Assert.fail();
      if (CBORObject.False.CanFitInSingle())Assert.fail();
      if (CBORObject.Null.CanFitInSingle())Assert.fail();
      if (CBORObject.Undefined.CanFitInSingle())Assert.fail();
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number"
).AsString()));
        if (numberinfo.get("single").AsBoolean()) {
          if (!(cbornumber.CanFitInSingle()))Assert.fail();
        } else {
          if (cbornumber.CanFitInSingle())Assert.fail();
        }
      }
    }
    @Test
    public void TestCanTruncatedIntFitInInt32() {
      if (!(CBORObject.FromObject(ExtendedFloat.Create(-2,
11)) .CanTruncatedIntFitInInt32()))Assert.fail();
      if (!(CBORObject.FromObject(ExtendedFloat.Create(-2,
12)) .CanTruncatedIntFitInInt32()))Assert.fail();
      if (!(CBORObject.FromObject(ExtendedFloat.Create(-2,
13)) .CanTruncatedIntFitInInt32()))Assert.fail();
      if (!(CBORObject.FromObject(ExtendedFloat.Create(-2,
14)) .CanTruncatedIntFitInInt32()))Assert.fail();
      if (!(CBORObject.FromObject(ExtendedFloat.Create(-2,
15)) .CanTruncatedIntFitInInt32()))Assert.fail();
      if (!(CBORObject.FromObject(ExtendedFloat.Create(-2,
16)) .CanTruncatedIntFitInInt32()))Assert.fail();
      if (!(CBORObject.FromObject(ExtendedFloat.Create(-2,
17)) .CanTruncatedIntFitInInt32()))Assert.fail();
      if (!(CBORObject.FromObject(ExtendedFloat.Create(-2,
18)) .CanTruncatedIntFitInInt32()))Assert.fail();
      if (!(CBORObject.FromObject(ExtendedFloat.Create(-2,
19)) .CanTruncatedIntFitInInt32()))Assert.fail();
      if (!(CBORObject.FromObject(0).CanTruncatedIntFitInInt32()))Assert.fail();
      if (CBORObject.True.CanTruncatedIntFitInInt32())Assert.fail();
      if (CBORObject.FromObject("")
                     .CanTruncatedIntFitInInt32())Assert.fail();
      if (CBORObject.NewArray().CanTruncatedIntFitInInt32())Assert.fail();
      if (CBORObject.NewMap().CanTruncatedIntFitInInt32())Assert.fail();
      if (CBORObject.False.CanTruncatedIntFitInInt32())Assert.fail();
      if (CBORObject.Null.CanTruncatedIntFitInInt32())Assert.fail();
      if (CBORObject.Undefined.CanTruncatedIntFitInInt32())Assert.fail();
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number"
).AsString()));
        if (numberinfo.get("int32").AsBoolean()) {
          if (!(cbornumber.CanTruncatedIntFitInInt32()))Assert.fail();
        } else {
          if (cbornumber.CanTruncatedIntFitInInt32())Assert.fail();
        }
      }
    }
    @Test
    public void TestCanTruncatedIntFitInInt64() {
      if (!(CBORObject.FromObject(ExtendedFloat.Create(-2,
11)) .CanTruncatedIntFitInInt64()))Assert.fail();
      if (!(CBORObject.FromObject(ExtendedFloat.Create(-2,
12)) .CanTruncatedIntFitInInt64()))Assert.fail();
      if (!(CBORObject.FromObject(ExtendedFloat.Create(-2,
13)) .CanTruncatedIntFitInInt64()))Assert.fail();
      if (!(CBORObject.FromObject(ExtendedFloat.Create(-2,
14)) .CanTruncatedIntFitInInt64()))Assert.fail();
      if (!(CBORObject.FromObject(ExtendedFloat.Create(-2,
15)) .CanTruncatedIntFitInInt64()))Assert.fail();
      if (!(CBORObject.FromObject(ExtendedFloat.Create(-2,
16)) .CanTruncatedIntFitInInt64()))Assert.fail();
      if (!(CBORObject.FromObject(ExtendedFloat.Create(-2,
17)) .CanTruncatedIntFitInInt64()))Assert.fail();
      if (!(CBORObject.FromObject(ExtendedFloat.Create(-2,
18)) .CanTruncatedIntFitInInt64()))Assert.fail();
      if (!(CBORObject.FromObject(ExtendedFloat.Create(-2,
19)) .CanTruncatedIntFitInInt64()))Assert.fail();
      if (!(CBORObject.FromObject(0).CanTruncatedIntFitInInt64()))Assert.fail();
      if (CBORObject.True.CanTruncatedIntFitInInt64())Assert.fail();
      if (CBORObject.FromObject("")
                     .CanTruncatedIntFitInInt64())Assert.fail();
      if (CBORObject.NewArray().CanTruncatedIntFitInInt64())Assert.fail();
      if (CBORObject.NewMap().CanTruncatedIntFitInInt64())Assert.fail();
      if (CBORObject.False.CanTruncatedIntFitInInt64())Assert.fail();
      if (CBORObject.Null.CanTruncatedIntFitInInt64())Assert.fail();
      if (CBORObject.Undefined.CanTruncatedIntFitInInt64())Assert.fail();
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number"
).AsString()));
        if (numberinfo.get("int64").AsBoolean()) {
          if (!(cbornumber.CanTruncatedIntFitInInt64()))Assert.fail();
        } else {
          if (cbornumber.CanTruncatedIntFitInInt64())Assert.fail();
        }
      }
    }
    @Test
    public void TestCompareTo() {
      Assert.assertEquals(1, CBORObject.True.compareTo(null));
      Assert.assertEquals(1, CBORObject.False.compareTo(null));
      Assert.assertEquals(1, CBORObject.Null.compareTo(null));
      Assert.assertEquals(1, CBORObject.NewArray().compareTo(null));
      Assert.assertEquals(1, CBORObject.NewMap().compareTo(null));
      Assert.assertEquals(1, CBORObject.FromObject(100).compareTo(null));
      Assert.assertEquals(1, CBORObject.FromObject(Double.NaN).compareTo(null));
      CBORTest.CompareTestLess(CBORObject.Undefined, CBORObject.Null);
      CBORTest.CompareTestLess(CBORObject.Null, CBORObject.False);
      CBORTest.CompareTestLess(CBORObject.False, CBORObject.True);
      CBORTest.CompareTestLess(CBORObject.False, CBORObject.FromObject(0));
      CBORTest.CompareTestLess(CBORObject.False, CBORObject.FromSimpleValue(0));
      CBORTest.CompareTestLess(CBORObject.FromSimpleValue(0),
                               CBORObject.FromSimpleValue(1));
      CBORTest.CompareTestLess(CBORObject.FromObject(0),
                               CBORObject.FromObject(1));
      CBORTest.CompareTestLess(CBORObject.FromObject(0.0f),
                               CBORObject.FromObject(1.0f));
      CBORTest.CompareTestLess(CBORObject.FromObject(0.0),
                               CBORObject.FromObject(1.0));
    }
    @Test
    public void TestContainsKey() {
      // not implemented yet
    }
    @Test
    public void TestCount() {
      Assert.assertEquals(0, CBORObject.True.size());
      Assert.assertEquals(0, CBORObject.False.size());
      Assert.assertEquals(0, CBORObject.NewArray().size());
      Assert.assertEquals(0, CBORObject.NewMap().size());
    }

    @Test
    public void TestDecodeFromBytesVersion2Dot0() {
      try {
        CBORObject.DecodeFromBytes(new byte[] { });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    @Test
    public void TestDecodeFromBytes() {
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0x1c  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0x1e  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xfe  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xff  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestDivide() {
      // not implemented yet
    }
    @Test
    public void TestEncodeToBytes() {
      // not implemented yet
    }
    @Test
    public void TestEquals() {
      // not implemented yet
    }
    @Test
    public void TestFromJSONString() {
      try {
        CBORObject.FromJSONString("\"\\uxxxx\"");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("\"\\ud800\udc00\"");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("\"\ud800\\udc00\"");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("\"\\U0023\"");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("\"\\u002x\"");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("\"\\u00xx\"");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("\"\\u0xxx\"");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("\"\\u0\"");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("\"\\u00\"");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("\"\\u000\"");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("trbb");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("trub");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("falsb");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("nulb");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[true");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[true,");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[true]!");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\ud800\udc00\"]");
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new
          IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\\udc00\"]");
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new
          IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\ud800\\udc00\"]");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new
          IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\udc00\"]");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new
          IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\udc00\ud800\udc00\"]");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new
          IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\ud800\udc00\"]");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new
          IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\\udc00\ud800\udc00\"]");
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new
          IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\"]");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new
          IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[1,2,"); Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new
          IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[1,2,3"); Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new
          IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("{,\"0\":0,\"1\":1}");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new
          IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("{\"0\":0,,\"1\":1}");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new
          IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("{\"0\":0,\"1\":1,}");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new
          IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[,0,1,2]");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new
          IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[0,,1,2]");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new
          IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[0,1,,2]");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new
          IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[0,1,2,]");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new
          IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[0001]");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("{a:true}");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\"://comment\ntrue}");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\":/*comment*/true}");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("{'a':true}");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\":'b'}");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\t\":true}");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\r\":true}");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\n\":true}");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("['a']");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\":\"a\t\"}");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"a\\'\"]");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[NaN]");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[+Infinity]");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[-Infinity]");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[Infinity]");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\":\"a\r\"}");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("{\"a\":\"a\n\"}");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"a\t\"]");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      Assert.assertEquals(CBORObject.True, CBORObject.FromJSONString("true"));
      Assert.assertEquals(CBORObject.False, CBORObject.FromJSONString("false"));
      Assert.assertEquals(CBORObject.Null, CBORObject.FromJSONString("null"));
      Assert.assertEquals(5, CBORObject.FromJSONString(" 5 ").AsInt32());
      Assert.assertEquals("/\b", CBORObject.FromJSONString("\"\\/\\b\""
).AsString());
    }
    @Test
    public void TestFromObject() {
      CBORObject[] cborarray = new CBORObject[2];
      cborarray[0] = CBORObject.False;
      cborarray[1] = CBORObject.True;
      CBORObject cbor = CBORObject.FromObject(cborarray);
      Assert.assertEquals(2, cbor.size());
      Assert.assertEquals(CBORObject.False, cbor.get(0));
      Assert.assertEquals(CBORObject.True, cbor.get(1));
      TestCommon.AssertRoundTrip(cbor);
      Assert.assertEquals(CBORObject.Null, CBORObject.FromObject((int[])null));
      long[] longarray = { 2, 3 };
      cbor = CBORObject.FromObject(longarray);
      Assert.assertEquals(2, cbor.size());
      if (!(CBORObject.FromObject(2).compareTo(cbor.get(0)) == 0))Assert.fail();
      if (!(CBORObject.FromObject(3).compareTo(cbor.get(1)) == 0))Assert.fail();
      TestCommon.AssertRoundTrip(cbor);
      Assert.assertEquals(CBORObject.Null,
                      CBORObject.FromObject((ExtendedRational)null));
      Assert.assertEquals(CBORObject.Null,
                      CBORObject.FromObject((ExtendedDecimal)null));
      Assert.assertEquals(CBORObject.FromObject(10),
                      CBORObject.FromObject(ExtendedRational.Create(10, 1)));
      try {
        CBORObject.FromObject(ExtendedRational.Create(10, 2));
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestFromObjectAndTag() {
      BigInteger bigvalue = BigInteger.ONE.shiftLeft(100);
      try {
        CBORObject.FromObjectAndTag(2, bigvalue);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObjectAndTag(2, -1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObjectAndTag(CBORObject.Null, -1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObjectAndTag(CBORObject.Null, 999999);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObjectAndTag(2, null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObjectAndTag(2, BigInteger.ZERO.subtract(BigInteger.ONE));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestFromSimpleValue() {
      try {
        CBORObject.FromSimpleValue(-1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromSimpleValue(256);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      for (int i = 0; i < 256; ++i) {
        if (i >= 24 && i < 32) {
          try {
            CBORObject.FromSimpleValue(i);
            Assert.fail("Should have failed");
          } catch (IllegalArgumentException ex) {
          } catch (Exception ex) {
            Assert.fail(ex.toString());
            throw new IllegalStateException("", ex);
          }
        } else {
          CBORObject cbor = CBORObject.FromSimpleValue(i);
          Assert.assertEquals(i, cbor.getSimpleValue());
        }
      }
    }
    @Test
    public void TestGetByteString() {
      try {
        CBORObject.True.GetByteString();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(0).GetByteString();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject("test").GetByteString();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.False.GetByteString();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.NewArray().GetByteString();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.NewMap().GetByteString();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestGetHashCode() {
      // not implemented yet
    }
    @Test
    public void TestGetTags() {
      // not implemented yet
    }
    @Test
    public void TestHasTag() {
      try {
        CBORObject.True.HasTag(-1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger bigintNull = null;
        CBORObject.True.HasTag(bigintNull);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.True.HasTag(BigInteger.ONE.negate());
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      if (CBORObject.True.HasTag(0))Assert.fail();
      if (CBORObject.True.HasTag(BigInteger.ZERO))Assert.fail();
    }
    @Test
    public void TestInnermostTag() {
      // not implemented yet
    }
    @Test
    public void TestInsert() {
      // not implemented yet
    }
    @Test
    public void TestIsFalse() {
      // not implemented yet
    }
    @Test
    public void TestIsFinite() {
      CBORObject cbor;
      if (!(CBORObject.FromObject(0).isFinite()))Assert.fail();
      if (CBORObject.FromObject("").isFinite())Assert.fail();
      if (CBORObject.NewArray().isFinite())Assert.fail();
      if (CBORObject.NewMap().isFinite())Assert.fail();
      cbor = CBORObject.True;
      if (cbor.isFinite())Assert.fail();
      cbor = CBORObject.False;
      if (cbor.isFinite())Assert.fail();
      cbor = CBORObject.Null;
      if (cbor.isFinite())Assert.fail();
      cbor = CBORObject.Undefined;
      if (cbor.isFinite())Assert.fail();
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number"
).AsString()));
        if (!numberinfo.get("integer").equals(CBORObject.Null)) {
          if (!(cbornumber.isFinite()))Assert.fail();
        } else {
          if (cbornumber.isFinite())Assert.fail();
        }
      }
    }
    @Test
    public void TestIsInfinity() {
      // not implemented yet
    }

    public static CBORObject GetNumberData() {
      return new AppResources("Resources").GetJSON("numbers");
    }

    @Test
    public void TestIsIntegral() {
      CBORObject cbor;
      if (!(CBORObject.FromObject(0).isIntegral()))Assert.fail();
      if (CBORObject.FromObject("").isIntegral())Assert.fail();
      if (CBORObject.NewArray().isIntegral())Assert.fail();
      if (CBORObject.NewMap().isIntegral())Assert.fail();
      if (!(CBORObject.FromObject(BigInteger.ONE.shiftLeft(63)).isIntegral()))Assert.fail();
      if (!(CBORObject.FromObject(BigInteger.ONE.shiftLeft(64)).isIntegral()))Assert.fail();
      if (!(CBORObject.FromObject(BigInteger.ONE.shiftLeft(80)).isIntegral()))Assert.fail();
      if (!(CBORObject.FromObject(ExtendedDecimal.FromString("4444e+800"
)) .IsIntegral))Assert.fail();

  if (CBORObject.FromObject(ExtendedDecimal.FromString("4444e-800"
)) .IsIntegral)Assert.fail();
      if (CBORObject.FromObject(2.5).isIntegral())Assert.fail();
      if (CBORObject.FromObject(999.99).isIntegral())Assert.fail();
      if (CBORObject.FromObject(Double.POSITIVE_INFINITY).isIntegral())Assert.fail();
      if (CBORObject.FromObject(Double.NEGATIVE_INFINITY).isIntegral())Assert.fail();
      if (CBORObject.FromObject(Double.NaN).isIntegral())Assert.fail();
      if (CBORObject.FromObject(ExtendedDecimal.PositiveInfinity)
                     .IsIntegral)Assert.fail();
      if (CBORObject.FromObject(ExtendedDecimal.NegativeInfinity)
                     .IsIntegral)Assert.fail();
      if (CBORObject.FromObject(ExtendedDecimal.NaN).isIntegral())Assert.fail();
      cbor = CBORObject.True;
      if (cbor.isIntegral())Assert.fail();
      cbor = CBORObject.False;
      if (cbor.isIntegral())Assert.fail();
      cbor = CBORObject.Null;
      if (cbor.isIntegral())Assert.fail();
      cbor = CBORObject.Undefined;
      if (cbor.isIntegral())Assert.fail();
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number"
).AsString()));
        if (numberinfo.get("isintegral").AsBoolean()) {
          if (!(cbornumber.isIntegral()))Assert.fail();
        } else {
          if (cbornumber.isIntegral())Assert.fail();
        }
      }
    }
    @Test
    public void TestIsNaN() {
      if (CBORObject.True.IsNaN())Assert.fail();
      if (CBORObject.FromObject("").IsNaN())Assert.fail();
      if (CBORObject.NewArray().IsNaN())Assert.fail();
      if (CBORObject.NewMap().IsNaN())Assert.fail();
      if (CBORObject.False.IsNaN())Assert.fail();
      if (CBORObject.Null.IsNaN())Assert.fail();
      if (CBORObject.Undefined.IsNaN())Assert.fail();
      if (CBORObject.PositiveInfinity.IsNaN())Assert.fail();
      if (CBORObject.PositiveInfinity.IsNaN())Assert.fail();
      if (!(CBORObject.NaN.IsNaN()))Assert.fail();
    }
    @Test
    public void TestIsNegativeInfinity() {
      // not implemented yet
    }
    @Test
    public void TestIsNull() {
      // not implemented yet
    }
    @Test
    public void TestIsPositiveInfinity() {
      // not implemented yet
    }
    @Test
    public void TestIsTagged() {
      // not implemented yet
    }
    @Test
    public void TestIsTrue() {
      // not implemented yet
    }
    @Test
    public void TestIsUndefined() {
      // not implemented yet
    }
    @Test
    public void TestIsZero() {
      // not implemented yet
    }
    @Test
    public void TestItem() {
      CBORObject cbor = CBORObject.True;
      try {
        CBORObject cbor2 = cbor.get(0);
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      cbor = CBORObject.False;
      try {
        CBORObject cbor2 = cbor.get(0);
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      cbor = CBORObject.FromObject(0);
      try {
        CBORObject cbor2 = cbor.get(0);
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      cbor = CBORObject.FromObject(2);
      try {
        CBORObject cbor2 = cbor.get(0);
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      cbor = CBORObject.NewArray();
      try {
        CBORObject cbor2 = cbor.get(0);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestKeys() {
      // not implemented yet
    }
    @Test
    public void TestMultiply() {
      // not implemented yet
    }
    @Test
    public void TestNegate() {
      try {
        CBORObject.True.Negate();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.False.Negate();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.NewArray().Negate();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.NewMap().Negate();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestNewArray() {
      // not implemented yet
    }
    @Test
    public void TestNewMap() {
      // not implemented yet
    }
    @Test
    public void TestOperatorAddition() {
      // not implemented yet
    }
    @Test
    public void TestOperatorDivision() {
      // not implemented yet
    }
    @Test
    public void TestOperatorModulus() {
      // not implemented yet
    }
    @Test
    public void TestOperatorMultiply() {
      // not implemented yet
    }
    @Test
    public void TestOperatorSubtraction() {
      // not implemented yet
    }
    @Test
    public void TestOutermostTag() {
      CBORObject cbor = CBORObject.FromObjectAndTag(CBORObject.True, 999);
      cbor = CBORObject.FromObjectAndTag(CBORObject.True, 1000);
      Assert.assertEquals(BigInteger.valueOf(1000), cbor.getOutermostTag());
      cbor = CBORObject.True;
      Assert.assertEquals(BigInteger.valueOf(-1), cbor.getOutermostTag());
    }
    @Test
    public void TestRead() {
      // not implemented yet
    }
    @Test
    public void TestReadJSON() {
      // not implemented yet
    }
    @Test
    public void TestRemainder() {
      // not implemented yet
    }
    @Test
    public void TestRemove() {
      // not implemented yet
    }
    @Test
    public void TestSet() {
      CBORObject cbor = CBORObject.NewMap().Add("x", 0).Add("y", 1);
      Assert.assertEquals(0, cbor.get("x").AsInt32());
      Assert.assertEquals(1, cbor.get("y").AsInt32());
      cbor.Set("x", 5).Set("z", 6);
      Assert.assertEquals(5, cbor.get("x").AsInt32());
      Assert.assertEquals(6, cbor.get("z").AsInt32());
    }
    @Test
    public void TestSign() {
      try {
        int sign = CBORObject.True.signum();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        int sign = CBORObject.False.signum();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        int sign = CBORObject.NewArray().signum();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        int sign = CBORObject.NewMap().signum();
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber =
          CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number"
).AsString()));
        if (cbornumber.IsNaN()) {
          try {
            Assert.fail("" + cbornumber.signum());
            Assert.fail("Should have failed");
          } catch (IllegalStateException ex) {
          } catch (Exception ex) {
            Assert.fail(ex.toString());
            throw new IllegalStateException("", ex);
          }
        } else if (numberinfo.get("number").AsString().indexOf('-') == 0) {
          Assert.assertEquals(-1, cbornumber.signum());
        } else if (numberinfo.get("number").AsString().equals("0")) {
          Assert.assertEquals(0, cbornumber.signum());
        } else {
          Assert.assertEquals(1, cbornumber.signum());
        }
      }
    }
    @Test
    public void TestSimpleValue() {
      // not implemented yet
    }
    @Test
    public void TestSubtract() {
      // not implemented yet
    }

    @Test
    public void TestToJSONStringFor2Dot0() {
      Assert.assertEquals("\"\u2027\\u2028\\u2029\u202a\"" ,
             CBORObject.FromObject("\u2027\u2028\u2029\u202a"
).ToJSONString());
    }

    @Test
    public void TestToJSONString() {
      Assert.assertEquals("true", CBORObject.True.ToJSONString());
      Assert.assertEquals("false", CBORObject.False.ToJSONString());
      Assert.assertEquals("null", CBORObject.Null.ToJSONString());
      Assert.assertEquals("null" ,
CBORObject.FromObject(Float.POSITIVE_INFINITY) .ToJSONString());
      Assert.assertEquals("null" ,
CBORObject.FromObject(Float.NEGATIVE_INFINITY) .ToJSONString());
      Assert.assertEquals("null", CBORObject.FromObject(Float.NaN).ToJSONString());
      Assert.assertEquals("null" ,
CBORObject.FromObject(Double.POSITIVE_INFINITY) .ToJSONString());
      Assert.assertEquals("null" ,
CBORObject.FromObject(Double.NEGATIVE_INFINITY) .ToJSONString());
      Assert.assertEquals("null", CBORObject.FromObject(Double.NaN).ToJSONString());
      // Base64 tests
      CBORObject o;
      o = CBORObject.FromObjectAndTag(new byte[] { (byte)0x9a, (byte)0xd6, (byte)0xf0, (byte)0xe8  },
                                      22);
      Assert.assertEquals("\"mtbw6A\"", o.ToJSONString());
      o = CBORObject.FromObject(new byte[] { (byte)0x9a, (byte)0xd6, (byte)0xf0, (byte)0xe8  });
      Assert.assertEquals("\"mtbw6A\"", o.ToJSONString());
      o = CBORObject.FromObjectAndTag(new byte[] { (byte)0x9a, (byte)0xd6, (byte)0xf0, (byte)0xe8  },
                                      23);
      Assert.assertEquals("\"9AD6F0E8\"", o.ToJSONString());
      o = CBORObject.FromObject(new byte[] { (byte)0x9a, (byte)0xd6, (byte)0xff, (byte)0xe8  });
      // Encode with Base64URL by default
      Assert.assertEquals("\"mtb_6A\"", o.ToJSONString());
      o = CBORObject.FromObjectAndTag(new byte[] { (byte)0x9a, (byte)0xd6, (byte)0xff, (byte)0xe8  },
                                      22);
      // Encode with Base64
      Assert.assertEquals("\"mtb/6A\"", o.ToJSONString());
    }
    @Test
    public void TestToString() {
      Assert.assertEquals("undefined", CBORObject.Undefined.toString());
      Assert.assertEquals("simple(50)", CBORObject.FromSimpleValue(50).toString());
    }
    @Test
    public void TestType() {
      // not implemented yet
    }
    @Test
    public void TestUntag() {
      // not implemented yet
    }
    @Test
    public void TestUntagOne() {
      // not implemented yet
    }
    @Test
    public void TestValues() {
      // not implemented yet
    }
    @Test
    public void TestWrite() {
      // not implemented yet
    }
    @Test
    public void TestWriteJSON() {
      // not implemented yet
    }
    @Test
    public void TestWriteJSONTo() {
      // not implemented yet
    }
    @Test
    public void TestWriteTo() {
      // not implemented yet
    }
  }
