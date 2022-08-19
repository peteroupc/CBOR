using NUnit.Framework;
using PeterO.Cbor;
using System;

namespace Test
{
    [TestFixture]
    public class CBORTypeMapperTest
    {
        [Test]
        public void TestAddTypeName()
        {
            CBORTypeMapper tm = new();
            try
            {
                tm.AddTypeName(null);
                Assert.Fail("Should have failed");
            }
            catch (ArgumentNullException)
            {
                // NOTE: Intentionally empty
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
                throw new InvalidOperationException(string.Empty, ex);
            }
            try
            {
                tm.AddTypeName(string.Empty);
                Assert.Fail("Should have failed");
            }
            catch (ArgumentException)
            {
                // NOTE: Intentionally empty
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
                throw new InvalidOperationException(string.Empty, ex);
            }
            try
            {
                tm.AddTypeName("System.Uri");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
                throw new InvalidOperationException(string.Empty, ex);
            }
        }

        [Test]
        public void TestAddTypePrefix()
        {
            CBORTypeMapper tm = new();
            try
            {
                tm.AddTypePrefix(null);
                Assert.Fail("Should have failed");
            }
            catch (ArgumentNullException)
            {
                // NOTE: Intentionally empty
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
                throw new InvalidOperationException(string.Empty, ex);
            }
            try
            {
                tm.AddTypePrefix(string.Empty);
                Assert.Fail("Should have failed");
            }
            catch (ArgumentException)
            {
                // NOTE: Intentionally empty
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
                throw new InvalidOperationException(string.Empty, ex);
            }
            try
            {
                tm.AddTypePrefix("System.Uri");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
                throw new InvalidOperationException(string.Empty, ex);
            }
        }
    }
}