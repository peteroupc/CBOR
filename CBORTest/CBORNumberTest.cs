using System;
using NUnit.Framework;
using PeterO.Cbor;
namespace Test {
[TestFixture]
public class CBORNumberTest {
[Test]
public void TestToCBORObject() {
// not implemented yet
}
[Test]
public void TestFromCBORObject() {
// not implemented yet
}
[Test]
public void TestToString() {
// not implemented yet
}
[Test]
public void TestCanFitInInt32() {
// not implemented yet
}
[Test]
public void TestCanFitInInt64() {
// not implemented yet
}
[Test]
public void TestIsInfinity() {
// not implemented yet
}
[Test]
public void TestIsNaN() {
      Assert.IsFalse(CBORObject.FromObject(0).AsNumber().IsNaN());
      Assert.IsFalse(CBORObject.FromObject(99).AsNumber().IsNaN());
      Assert.IsFalse(CBORObject.PositiveInfinity.AsNumber().IsNaN());
      Assert.IsFalse(CBORObject.NegativeInfinity.AsNumber().IsNaN());
      Assert.IsTrue(CBORObject.NaN.AsNumber().IsNaN());
}
[Test]
public void TestNegate() {
// not implemented yet
}
[Test]
public void TestAdd() {
// not implemented yet
}
[Test]
public void TestSubtract() {
// not implemented yet
}
[Test]
public void TestMultiply() {
// not implemented yet
}
[Test]
public void TestDivide() {
// not implemented yet
}
[Test]
public void TestRemainder() {
// not implemented yet
}
[Test]
public void TestCompareTo() {
// not implemented yet
}
[Test]
public void TestLessThan() {
// not implemented yet
}
[Test]
public void TestLessThanOrEqual() {
// not implemented yet
}
[Test]
public void TestGreaterThan() {
// not implemented yet
}
[Test]
public void TestGreaterThanOrEqual() {
// not implemented yet
}
[Test]
public void TestGetType() {
// not implemented yet
}
}
}
