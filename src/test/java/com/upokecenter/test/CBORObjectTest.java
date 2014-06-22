package com.upokecenter.test;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;
import com.upokecenter.cbor.*;

  public class CBORObjectTest {
    @Test
    public void TestAbs() {
      // not implemented yet
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
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number").AsString()));
        if (!numberinfo.get("integer").equals(CBORObject.Null)) {
          Assert.assertEquals(numberinfo.get("integer").AsString(), cbornumber.AsBigInteger().toString());
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
      if(!(CBORObject.True.AsBoolean()))Assert.fail();
      if(!(CBORObject.FromObject(0).AsBoolean()))Assert.fail();
      if(!(CBORObject.FromObject("").AsBoolean()))Assert.fail();
      if(CBORObject.False.AsBoolean())Assert.fail();
      if(CBORObject.Null.AsBoolean())Assert.fail();
      if(CBORObject.Undefined.AsBoolean())Assert.fail();
      if(!(CBORObject.NewArray().AsBoolean()))Assert.fail();
      if(!(CBORObject.NewMap().AsBoolean()))Assert.fail();
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
    }
    @Test
    public void TestAsExtendedDecimal() {
      Assert.assertEquals(ExtendedDecimal.PositiveInfinity, CBORObject.FromObject(Float.POSITIVE_INFINITY).AsExtendedDecimal());
      Assert.assertEquals(ExtendedDecimal.NegativeInfinity, CBORObject.FromObject(Float.NEGATIVE_INFINITY).AsExtendedDecimal());
      if(!(CBORObject.FromObject(Float.NaN).AsExtendedDecimal().IsNaN()))Assert.fail();
      Assert.assertEquals(ExtendedDecimal.PositiveInfinity, CBORObject.FromObject(Double.POSITIVE_INFINITY).AsExtendedDecimal());
      Assert.assertEquals(ExtendedDecimal.NegativeInfinity, CBORObject.FromObject(Double.NEGATIVE_INFINITY).AsExtendedDecimal());
      if(!(CBORObject.FromObject(Double.NaN).AsExtendedDecimal().IsNaN()))Assert.fail();
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
      Assert.assertEquals(ExtendedFloat.PositiveInfinity, CBORObject.FromObject(Float.POSITIVE_INFINITY).AsExtendedFloat());
      Assert.assertEquals(ExtendedFloat.NegativeInfinity, CBORObject.FromObject(Float.NEGATIVE_INFINITY).AsExtendedFloat());
      if(!(CBORObject.FromObject(Float.NaN).AsExtendedFloat().IsNaN()))Assert.fail();
      Assert.assertEquals(ExtendedFloat.PositiveInfinity, CBORObject.FromObject(Double.POSITIVE_INFINITY).AsExtendedFloat());
      Assert.assertEquals(ExtendedFloat.NegativeInfinity, CBORObject.FromObject(Double.NEGATIVE_INFINITY).AsExtendedFloat());
      if(!(CBORObject.FromObject(Double.NaN).AsExtendedFloat().IsNaN()))Assert.fail();
    }
    @Test
    public void TestAsExtendedRational() {
      Assert.assertEquals(ExtendedRational.PositiveInfinity, CBORObject.FromObject(Float.POSITIVE_INFINITY).AsExtendedRational());
      Assert.assertEquals(ExtendedRational.NegativeInfinity, CBORObject.FromObject(Float.NEGATIVE_INFINITY).AsExtendedRational());
      if(!(CBORObject.FromObject(Float.NaN).AsExtendedRational().IsNaN()))Assert.fail();
      Assert.assertEquals(ExtendedRational.PositiveInfinity, CBORObject.FromObject(Double.POSITIVE_INFINITY).AsExtendedRational());
      Assert.assertEquals(ExtendedRational.NegativeInfinity, CBORObject.FromObject(Double.NEGATIVE_INFINITY).AsExtendedRational());
      if(!(CBORObject.FromObject(Double.NaN).AsExtendedRational().IsNaN()))Assert.fail();
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
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number").AsString()));
        if (numberinfo.get("int32").AsBoolean()) {
          Assert.assertEquals(BigInteger.fromString(numberinfo.get("integer").AsString()).intValue(), cbornumber.AsInt32());
        } else {
          try {
            cbornumber.AsInt32();
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
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number").AsString()));
        if (numberinfo.get("int64").AsBoolean()) {
          Assert.assertEquals(BigInteger.fromString(numberinfo.get("integer").AsString()).longValue(), cbornumber.AsInt64());
        } else {
          try {
            cbornumber.AsInt64();
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
    public void TestAsSByte() {
      // not implemented yet
    }
    @Test
    public void TestAsSingle() {
      // not implemented yet
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
      if(!(CBORObject.FromObject(0).CanFitInDouble()))Assert.fail();
      if(CBORObject.True.CanFitInDouble())Assert.fail();
      if(CBORObject.FromObject("").CanFitInDouble())Assert.fail();
      if(CBORObject.NewArray().CanFitInDouble())Assert.fail();
      if(CBORObject.NewMap().CanFitInDouble())Assert.fail();
      if(CBORObject.False.CanFitInDouble())Assert.fail();
      if(CBORObject.Null.CanFitInDouble())Assert.fail();
      if(CBORObject.Undefined.CanFitInDouble())Assert.fail();
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number").AsString()));
        if (numberinfo.get("double").AsBoolean()) {
          if(!(cbornumber.CanFitInDouble()))Assert.fail();
        } else {
          if(cbornumber.CanFitInDouble())Assert.fail();
        }
      }
    }
    @Test
    public void TestCanFitInInt32() {
      if(!(CBORObject.FromObject(0).CanFitInInt32()))Assert.fail();
      if(CBORObject.True.CanFitInInt32())Assert.fail();
      if(CBORObject.FromObject("").CanFitInInt32())Assert.fail();
      if(CBORObject.NewArray().CanFitInInt32())Assert.fail();
      if(CBORObject.NewMap().CanFitInInt32())Assert.fail();
      if(CBORObject.False.CanFitInInt32())Assert.fail();
      if(CBORObject.Null.CanFitInInt32())Assert.fail();
      if(CBORObject.Undefined.CanFitInInt32())Assert.fail();
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number").AsString()));
        if (numberinfo.get("int32").AsBoolean() && numberinfo.get("isintegral").AsBoolean()) {
          if(!(cbornumber.CanFitInInt32()))Assert.fail();
        } else {
          if(cbornumber.CanFitInInt32())Assert.fail();
        }
      }
    }
    @Test
    public void TestCanFitInInt64() {
      if(!(CBORObject.FromObject(0).CanFitInSingle()))Assert.fail();
      if(CBORObject.True.CanFitInSingle())Assert.fail();
      if(CBORObject.FromObject("").CanFitInSingle())Assert.fail();
      if(CBORObject.NewArray().CanFitInSingle())Assert.fail();
      if(CBORObject.NewMap().CanFitInSingle())Assert.fail();
      if(CBORObject.False.CanFitInSingle())Assert.fail();
      if(CBORObject.Null.CanFitInSingle())Assert.fail();
      if(CBORObject.Undefined.CanFitInSingle())Assert.fail();
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number").AsString()));
        if (numberinfo.get("int64").AsBoolean() && numberinfo.get("isintegral").AsBoolean()) {
          if(!(cbornumber.CanFitInInt64()))Assert.fail();
        } else {
          if(cbornumber.CanFitInInt64())Assert.fail();
        }
      }
    }
    @Test
    public void TestCanFitInSingle() {
      if(!(CBORObject.FromObject(0).CanFitInSingle()))Assert.fail();
      if(CBORObject.True.CanFitInSingle())Assert.fail();
      if(CBORObject.FromObject("").CanFitInSingle())Assert.fail();
      if(CBORObject.NewArray().CanFitInSingle())Assert.fail();
      if(CBORObject.NewMap().CanFitInSingle())Assert.fail();
      if(CBORObject.False.CanFitInSingle())Assert.fail();
      if(CBORObject.Null.CanFitInSingle())Assert.fail();
      if(CBORObject.Undefined.CanFitInSingle())Assert.fail();
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number").AsString()));
        if (numberinfo.get("single").AsBoolean()) {
          if(!(cbornumber.CanFitInSingle()))Assert.fail();
        } else {
          if(cbornumber.CanFitInSingle())Assert.fail();
        }
      }
    }
    @Test
    public void TestCanTruncatedIntFitInInt32() {
      if(!(CBORObject.FromObject(ExtendedFloat.Create(-2, 11)).CanTruncatedIntFitInInt32()))Assert.fail();
      if(!(CBORObject.FromObject(ExtendedFloat.Create(-2, 12)).CanTruncatedIntFitInInt32()))Assert.fail();
      if(!(CBORObject.FromObject(ExtendedFloat.Create(-2, 13)).CanTruncatedIntFitInInt32()))Assert.fail();
      if(!(CBORObject.FromObject(ExtendedFloat.Create(-2, 14)).CanTruncatedIntFitInInt32()))Assert.fail();
      if(!(CBORObject.FromObject(ExtendedFloat.Create(-2, 15)).CanTruncatedIntFitInInt32()))Assert.fail();
      if(!(CBORObject.FromObject(ExtendedFloat.Create(-2, 16)).CanTruncatedIntFitInInt32()))Assert.fail();
      if(!(CBORObject.FromObject(ExtendedFloat.Create(-2, 17)).CanTruncatedIntFitInInt32()))Assert.fail();
      if(!(CBORObject.FromObject(ExtendedFloat.Create(-2, 18)).CanTruncatedIntFitInInt32()))Assert.fail();
      if(!(CBORObject.FromObject(ExtendedFloat.Create(-2, 19)).CanTruncatedIntFitInInt32()))Assert.fail();
      if(!(CBORObject.FromObject(0).CanTruncatedIntFitInInt32()))Assert.fail();
      if(CBORObject.True.CanTruncatedIntFitInInt32())Assert.fail();
      if(CBORObject.FromObject("").CanTruncatedIntFitInInt32())Assert.fail();
      if(CBORObject.NewArray().CanTruncatedIntFitInInt32())Assert.fail();
      if(CBORObject.NewMap().CanTruncatedIntFitInInt32())Assert.fail();
      if(CBORObject.False.CanTruncatedIntFitInInt32())Assert.fail();
      if(CBORObject.Null.CanTruncatedIntFitInInt32())Assert.fail();
      if(CBORObject.Undefined.CanTruncatedIntFitInInt32())Assert.fail();
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number").AsString()));
        if (numberinfo.get("int32").AsBoolean()) {
          if(!(cbornumber.CanTruncatedIntFitInInt32()))Assert.fail();
        } else {
          if(cbornumber.CanTruncatedIntFitInInt32())Assert.fail();
        }
      }
    }
    @Test
    public void TestCanTruncatedIntFitInInt64() {
      if(!(CBORObject.FromObject(ExtendedFloat.Create(-2, 11)).CanTruncatedIntFitInInt64()))Assert.fail();
      if(!(CBORObject.FromObject(ExtendedFloat.Create(-2, 12)).CanTruncatedIntFitInInt64()))Assert.fail();
      if(!(CBORObject.FromObject(ExtendedFloat.Create(-2, 13)).CanTruncatedIntFitInInt64()))Assert.fail();
      if(!(CBORObject.FromObject(ExtendedFloat.Create(-2, 14)).CanTruncatedIntFitInInt64()))Assert.fail();
      if(!(CBORObject.FromObject(ExtendedFloat.Create(-2, 15)).CanTruncatedIntFitInInt64()))Assert.fail();
      if(!(CBORObject.FromObject(ExtendedFloat.Create(-2, 16)).CanTruncatedIntFitInInt64()))Assert.fail();
      if(!(CBORObject.FromObject(ExtendedFloat.Create(-2, 17)).CanTruncatedIntFitInInt64()))Assert.fail();
      if(!(CBORObject.FromObject(ExtendedFloat.Create(-2, 18)).CanTruncatedIntFitInInt64()))Assert.fail();
      if(!(CBORObject.FromObject(ExtendedFloat.Create(-2, 19)).CanTruncatedIntFitInInt64()))Assert.fail();
      if(!(CBORObject.FromObject(0).CanTruncatedIntFitInInt64()))Assert.fail();
      if(CBORObject.True.CanTruncatedIntFitInInt64())Assert.fail();
      if(CBORObject.FromObject("").CanTruncatedIntFitInInt64())Assert.fail();
      if(CBORObject.NewArray().CanTruncatedIntFitInInt64())Assert.fail();
      if(CBORObject.NewMap().CanTruncatedIntFitInInt64())Assert.fail();
      if(CBORObject.False.CanTruncatedIntFitInInt64())Assert.fail();
      if(CBORObject.Null.CanTruncatedIntFitInInt64())Assert.fail();
      if(CBORObject.Undefined.CanTruncatedIntFitInInt64())Assert.fail();
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number").AsString()));
        if (numberinfo.get("int64").AsBoolean()) {
          if(!(cbornumber.CanTruncatedIntFitInInt64()))Assert.fail();
        } else {
          if(cbornumber.CanTruncatedIntFitInInt64())Assert.fail();
        }
      }
    }
    @Test
    public void TestCompareTo() {
      // not implemented yet
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
    public void TestDecodeFromBytes() {
      // not implemented yet
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
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\\udc00\"]");
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\ud800\\udc00\"]");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\udc00\"]");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\udc00\ud800\udc00\"]");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\ud800\udc00\"]");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\\udc00\ud800\udc00\"]");
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\"]"); Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[1,2,"); Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[1,2,3"); Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("{,\"0\":0,\"1\":1}");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("{\"0\":0,,\"1\":1}"); Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("{\"0\":0,\"1\":1,}"); Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[,0,1,2]"); Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[0,,1,2]"); Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[0,1,,2]"); Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[0,1,2,]"); Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
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
      Assert.assertEquals("/\b", CBORObject.FromJSONString("\"\\/\\b\"").AsString());
    }
    @Test
    public void TestFromObject() {
      // not implemented yet
    }
    @Test
    public void TestFromObjectAndTag() {
      CBORObject.FromObjectAndTag(CBORObject.Null, (BigInteger.ONE.shiftLeft(64)).subtract(BigInteger.ONE));
    }
    @Test
    public void TestFromSimpleValue() {
      // not implemented yet
    }
    @Test
    public void TestGetByteString() {
      // not implemented yet
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
      if(CBORObject.True.HasTag(0))Assert.fail();
      if(CBORObject.True.HasTag(BigInteger.ZERO))Assert.fail();
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
      if(!(CBORObject.FromObject(0).isFinite()))Assert.fail();
      if(CBORObject.FromObject("").isFinite())Assert.fail();
      if(CBORObject.NewArray().isFinite())Assert.fail();
      if(CBORObject.NewMap().isFinite())Assert.fail();
     cbor = CBORObject.True;
      if(cbor.isFinite())Assert.fail();
      cbor = CBORObject.False;
      if(cbor.isFinite())Assert.fail();
      cbor = CBORObject.Null;
      if(cbor.isFinite())Assert.fail();
      cbor = CBORObject.Undefined;
      if(cbor.isFinite())Assert.fail();
        CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number").AsString()));
        if (!numberinfo.get("integer").equals(CBORObject.Null)) {
          if(!(cbornumber.isFinite()))Assert.fail();
        } else {
          if(cbornumber.isFinite())Assert.fail();
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
      if(!(CBORObject.FromObject(0).isIntegral()))Assert.fail();
      if(CBORObject.FromObject("").isIntegral())Assert.fail();
      if(CBORObject.NewArray().isIntegral())Assert.fail();
      if(CBORObject.NewMap().isIntegral())Assert.fail();
      cbor = CBORObject.True;
      if(cbor.isIntegral())Assert.fail();
      cbor = CBORObject.False;
      if(cbor.isIntegral())Assert.fail();
      cbor = CBORObject.Null;
      if(cbor.isIntegral())Assert.fail();
      cbor = CBORObject.Undefined;
      if(cbor.isIntegral())Assert.fail();
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.size(); ++i) {
        CBORObject numberinfo = numbers.get(i);
        CBORObject cbornumber = CBORObject.FromObject(ExtendedDecimal.FromString(numberinfo.get("number").AsString()));
        if (numberinfo.get("isintegral").AsBoolean()) {
          if(!(cbornumber.isIntegral()))Assert.fail();
        } else {
          if(cbornumber.isIntegral())Assert.fail();
        }
      }
    }
    @Test
    public void TestIsNaN() {
      if(CBORObject.True.IsNaN())Assert.fail();
      if(CBORObject.FromObject("").IsNaN())Assert.fail();
      if(CBORObject.NewArray().IsNaN())Assert.fail();
      if(CBORObject.NewMap().IsNaN())Assert.fail();
      if(CBORObject.False.IsNaN())Assert.fail();
      if(CBORObject.Null.IsNaN())Assert.fail();
      if(CBORObject.Undefined.IsNaN())Assert.fail();
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
      // not implemented yet
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
      // not implemented yet
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
      // not implemented yet
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
      // not implemented yet
    }
    @Test
    public void TestSign() {
      // not implemented yet
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
    public void TestToJSONString() {
      Assert.assertEquals("true", CBORObject.True.ToJSONString());
      Assert.assertEquals("false", CBORObject.False.ToJSONString());
      Assert.assertEquals("null", CBORObject.Null.ToJSONString());
      Assert.assertEquals("null", CBORObject.FromObject(Float.POSITIVE_INFINITY).ToJSONString());
      Assert.assertEquals("null", CBORObject.FromObject(Float.NEGATIVE_INFINITY).ToJSONString());
      Assert.assertEquals("null", CBORObject.FromObject(Float.NaN).ToJSONString());
      Assert.assertEquals("null", CBORObject.FromObject(Double.POSITIVE_INFINITY).ToJSONString());
      Assert.assertEquals("null", CBORObject.FromObject(Double.NEGATIVE_INFINITY).ToJSONString());
      Assert.assertEquals("null", CBORObject.FromObject(Double.NaN).ToJSONString());
    }
    @Test
    public void TestToString() {
      // not implemented yet
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
