using System;
using NUnit.Framework;
using PeterO.Cbor;

namespace Test {
  [TestFixture]
  public class CBORTypeMapperTest {
    [Test]
    public void TestAddTypeName() {
      var tm = new CBORTypeMapper();
      try {
        _ = tm.AddTypeName(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = tm.AddTypeName(String.Empty);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = tm.AddTypeName("System.Uri");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestAddTypePrefix() {
      var tm = new CBORTypeMapper();
      try {
        _ = tm.AddTypePrefix(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = tm.AddTypePrefix(String.Empty);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = tm.AddTypePrefix("System.Uri");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
  }
}
