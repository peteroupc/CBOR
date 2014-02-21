/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/21/2014
 * Time: 12:43 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;
using PeterO;

namespace Test
{
  [TestFixture]
  public class CBORTest2
  {
    
    [Test]
    public void TestCanFitIn2(){
      CBORObject o=CBORObject.DecodeFromBytes(new byte[] { (byte)0xD8,0x1E,(byte)0x82,(byte)0xC3,0x58,0x39,0x42,0x7D,0x11,0x7D,(byte)0xD7,(byte)0xF8,0x7D,0x7D,0x6D,0x53,(byte)0xB8,0x55,(byte)0xDB,0x6E,(byte)0x90,(byte)0xE4,0x2D,(byte)0xCD,(byte)0xD0,(byte)0xCD,(byte)0xB9,0x7C,0x60,0x21,(byte)0xC2,0x36,(byte)0xF9,0x58,0x13,(byte)0xC5,(byte)0x9B,(byte)0xB9,(byte)0xF8,(byte)0xEF,0x54,(byte)0xC1,(byte)0xD3,(byte)0x9A,0x6F,0x48,0x6C,0x5D,0x6B,0x09,0x51,(byte)0xB1,0x07,0x59,(byte)0xE5,0x49,(byte)0xD9,0x20,(byte)0xDC,(byte)0xC8,(byte)0xF1,0x67,(byte)0xF3,(byte)0xC2,0x58,0x38,0x30,(byte)0x90,0x4A,(byte)0xD9,(byte)0xA4,(byte)0x84,(byte)0xEF,(byte)0xC9,(byte)0xE4,0x60,(byte)0xF6,0x6E,0x06,0x3F,(byte)0xD0,(byte)0xCF,0x62,0x6E,0x5E,(byte)0xB4,(byte)0x9B,(byte)0xBF,(byte)0xE2,(byte)0xFE,(byte)0xCA,0x69,(byte)0xE9,(byte)0x8D,(byte)0x95,(byte)0x98,0x10,0x33,(byte)0xA9,0x76,(byte)0x9B,0x4B,0x79,(byte)0xAD,0x02,0x58,0x2B,0x43,0x18,0x41,(byte)0x80,0x7B,0x7F,0x53,0x5C,0x31,(byte)0xED,(byte)0xE7,(byte)0xAE,0x23,0x0C,(byte)0x97});
      Console.WriteLine(o.AsDouble());
      Console.WriteLine(o.AsBigInteger());
    }

    
    [Test]
    public void TestToBigIntegerNonFinite() {
      try {
        ExtendedDecimal.PositiveInfinity.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.NegativeInfinity.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.NaN.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.SignalingNaN.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.PositiveInfinity.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.NegativeInfinity.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.NaN.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.SignalingNaN.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
  }
}
