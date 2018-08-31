using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using PeterO;

namespace Test {
  [TestFixture]
  public class DataUtilitiesTest {
    public static IList<byte[]> GenerateIllegalUtf8Sequences() {
      List<byte[]> list = new List<byte[]>();
      // Generate illegal single bytes
      for (int i = 0x80; i <= 0xff; ++i) {
        if (i < 0xc2 || i > 0xf4) {
          list.Add(new byte[] { (byte)i, 0x80 });
        }
        list.Add(new[] { (byte)i });
      }
      list.Add(new byte[] { 0xe0, 0xa0 });
      list.Add(new byte[] { 0xe1, 0x80 });
      list.Add(new byte[] { 0xef, 0x80 });
      list.Add(new byte[] { 0xf0, 0x90 });
      list.Add(new byte[] { 0xf1, 0x80 });
      list.Add(new byte[] { 0xf3, 0x80 });
      list.Add(new byte[] { 0xf4, 0x80 });
      list.Add(new byte[] { 0xf0, 0x90, 0x80 });
      list.Add(new byte[] { 0xf1, 0x80, 0x80 });
      list.Add(new byte[] { 0xf3, 0x80, 0x80 });
      list.Add(new byte[] { 0xf4, 0x80, 0x80 });
      // Generate illegal multibyte sequences
      for (int i = 0x00; i <= 0xff; ++i) {
        if (i < 0x80 || i > 0xbf) {
          list.Add(new byte[] { 0xc2, (byte)i });
          list.Add(new byte[] { 0xdf, (byte)i });
          list.Add(new byte[] { 0xe1, (byte)i, 0x80 });
          list.Add(new byte[] { 0xef, (byte)i, 0x80 });
          list.Add(new byte[] { 0xf1, (byte)i, 0x80, 0x80 });
          list.Add(new byte[] { 0xf3, (byte)i, 0x80, 0x80 });
          list.Add(new byte[] { 0xe0, 0xa0, (byte)i });
          list.Add(new byte[] { 0xe1, 0x80, (byte)i });
          list.Add(new byte[] { 0xef, 0x80, (byte)i });
          list.Add(new byte[] { 0xf0, 0x90, (byte)i, 0x80 });
          list.Add(new byte[] { 0xf1, 0x80, (byte)i, 0x80 });
          list.Add(new byte[] { 0xf3, 0x80, (byte)i, 0x80 });
          list.Add(new byte[] { 0xf4, 0x80, (byte)i, 0x80 });
          list.Add(new byte[] { 0xf0, 0x90, 0x80, (byte)i });
          list.Add(new byte[] { 0xf1, 0x80, 0x80, (byte)i });
          list.Add(new byte[] { 0xf3, 0x80, 0x80, (byte)i });
          list.Add(new byte[] { 0xf4, 0x80, 0x80, (byte)i });
        }
        if (i < 0xa0 || i > 0xbf) {
          list.Add(new byte[] { 0xe0, (byte)i, 0x80 });
        }
        if (i < 0x90 || i > 0xbf) {
          list.Add(new byte[] { 0xf0, (byte)i, 0x80, 0x80 });
        }
        if (i < 0x80 || i > 0x8f) {
          list.Add(new byte[] { 0xf4, (byte)i, 0x80, 0x80 });
        }
      }
      return list;
    }

    [Test]
    public void TestCodePointAt() {
      try {
        DataUtilities.CodePointAt(null, 0);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(-1, DataUtilities.CodePointAt("A", -1));
      Assert.AreEqual(-1, DataUtilities.CodePointAt("A", 1));
      Assert.AreEqual(0x41, DataUtilities.CodePointAt("A", 0));

      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\ud800", 0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\udc00", 0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\ud800X", 0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\udc00X", 0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\ud800\ud800", 0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\udc00\ud800", 0));
      Assert.AreEqual(
     0xfffd,
     DataUtilities.CodePointAt("\ud800\ud800\udc00", 0));
      Assert.AreEqual(
     0xfffd,
     DataUtilities.CodePointAt("\udc00\ud800\udc00", 0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\udc00\udc00", 0));
      Assert.AreEqual(0x10000, DataUtilities.CodePointAt("\ud800\udc00", 0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\ud800", 0, 0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\udc00", 0, 0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\ud800X", 0, 0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\udc00X", 0, 0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\ud800\ud800", 0, 0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\udc00\ud800", 0, 0));
      Assert.AreEqual(
        0xfffd,
        DataUtilities.CodePointAt("\ud800\ud800\udc00", 0, 0));
      Assert.AreEqual(
        0xfffd,
        DataUtilities.CodePointAt("\udc00\ud800\udc00", 0, 0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\udc00\udc00", 0, 0));
      Assert.AreEqual(0x10000, DataUtilities.CodePointAt("\ud800\udc00", 0, 0));

      Assert.AreEqual(0xd800, DataUtilities.CodePointAt("\ud800", 0, 1));
      Assert.AreEqual(0xdc00, DataUtilities.CodePointAt("\udc00", 0, 1));
      Assert.AreEqual(0xd800, DataUtilities.CodePointAt("\ud800X", 0, 1));
      Assert.AreEqual(0xdc00, DataUtilities.CodePointAt("\udc00X", 0, 1));
      Assert.AreEqual(0xd800, DataUtilities.CodePointAt("\ud800\ud800", 0, 1));
      Assert.AreEqual(
        0xd800,
        DataUtilities.CodePointAt("\ud800\ud800\udc00", 0, 1));
      Assert.AreEqual(0xdc00, DataUtilities.CodePointAt("\udc00\ud800", 0, 1));
      Assert.AreEqual(
        0xdc00,
        DataUtilities.CodePointAt("\udc00\ud800\udc00", 0, 1));
      Assert.AreEqual(0xdc00, DataUtilities.CodePointAt("\udc00\udc00", 0, 1));
      Assert.AreEqual(0x10000, DataUtilities.CodePointAt("\ud800\udc00", 0, 1));

      Assert.AreEqual(-1, DataUtilities.CodePointAt("\ud800", 0, 2));
      Assert.AreEqual(-1, DataUtilities.CodePointAt("\udc00", 0, 2));
      Assert.AreEqual(-1, DataUtilities.CodePointAt("\ud800X", 0, 2));
      Assert.AreEqual(-1, DataUtilities.CodePointAt("\udc00X", 0, 2));
      Assert.AreEqual(-1, DataUtilities.CodePointAt("\ud800\ud800", 0, 2));
      {
        long numberTemp = DataUtilities.CodePointAt("\ud800\ud800\udc00", 0, 2);
        Assert.AreEqual(-1, numberTemp);
      }
      Assert.AreEqual(-1, DataUtilities.CodePointAt("\udc00\ud800", 0, 2));
      {
        long numberTemp = DataUtilities.CodePointAt("\udc00\ud800\udc00", 0, 2);
        Assert.AreEqual(-1, numberTemp);
      }
      Assert.AreEqual(-1, DataUtilities.CodePointAt("\udc00\udc00", 0, 2));
      Assert.AreEqual(0x10000, DataUtilities.CodePointAt("\ud800\udc00", 0, 2));
    }
    [Test]
    public void TestCodePointBefore() {
      try {
        DataUtilities.CodePointBefore(null, 0);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(-1, DataUtilities.CodePointBefore("A", 0));
      Assert.AreEqual(-1, DataUtilities.CodePointBefore("A", -1));
      Assert.AreEqual((int)'A', DataUtilities.CodePointBefore("A", 1));
      Assert.AreEqual(-1, DataUtilities.CodePointBefore("A", 2));
      Assert.AreEqual(
       (int)'A',
       DataUtilities.CodePointBefore("A\ud800\udc00B", 1));
      Assert.AreEqual(
      0x10000,
      DataUtilities.CodePointBefore("A\ud800\udc00B", 3));
      Assert.AreEqual(
     0xfffd,
     DataUtilities.CodePointBefore("A\ud800\udc00B", 2));
      Assert.AreEqual(
       0xd800,
       DataUtilities.CodePointBefore("A\ud800\udc00B", 2, 1));
      {
        long numberTemp = DataUtilities.CodePointBefore(
          "A\ud800\udc00B",
          2,
          2);
        Assert.AreEqual(-1, numberTemp);
      }
      Assert.AreEqual(0xfffd, DataUtilities.CodePointBefore("\udc00B", 1));
      Assert.AreEqual(0xdc00, DataUtilities.CodePointBefore("\udc00B", 1, 1));
      Assert.AreEqual(-1, DataUtilities.CodePointBefore("\udc00B", 1, 2));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointBefore("A\udc00B", 2));
      Assert.AreEqual(0xdc00, DataUtilities.CodePointBefore("A\udc00B", 2, 1));
      Assert.AreEqual(-1, DataUtilities.CodePointBefore("A\udc00B", 2, 2));
    }
    [Test]
    public void TestCodePointCompare() {
      Assert.AreEqual(-1, Math.Sign(DataUtilities.CodePointCompare(null, "A")));
      Assert.AreEqual(1, Math.Sign(DataUtilities.CodePointCompare("A", null)));
      Assert.AreEqual(0, Math.Sign(DataUtilities.CodePointCompare(null, null)));
      {
        long numberTemp = Math.Sign(
          DataUtilities.CodePointCompare("abc", "abc"));
        Assert.AreEqual(0, numberTemp);
      }
      {
        long numberTemp = Math.Sign(
          DataUtilities.CodePointCompare(
  "\ud800\udc00",
  "\ud800\udc00"));
        Assert.AreEqual(0, numberTemp);
      }
      {
        long numberTemp = Math.Sign(
          DataUtilities.CodePointCompare(
  "abc",
  "\ud800\udc00"));
        Assert.AreEqual(-1, numberTemp);
      }
      {
        long numberTemp = Math.Sign(
          DataUtilities.CodePointCompare(
  "\uf000",
  "\ud800\udc00"));
        Assert.AreEqual(-1, numberTemp);
      }
      {
        long numberTemp = Math.Sign(
  DataUtilities.CodePointCompare(
  "\uf000",
  "\ud800"));
        Assert.AreEqual(1, numberTemp);
      }
      Assert.IsTrue(DataUtilities.CodePointCompare("abc", "def") < 0);
      Assert.IsTrue(
        DataUtilities.CodePointCompare(
          "a\ud800\udc00",
          "a\ud900\udc00") < 0);
      Assert.IsTrue(
        DataUtilities.CodePointCompare(
          "a\ud800\udc00",
          "a\ud800\udc00") == 0);
      Assert.IsTrue(DataUtilities.CodePointCompare("a\ud800", "a\ud800") == 0);
      Assert.IsTrue(DataUtilities.CodePointCompare("a\udc00", "a\udc00") == 0);
      Assert.IsTrue(
        DataUtilities.CodePointCompare(
          "a\ud800\udc00",
          "a\ud800\udd00") < 0);
      Assert.IsTrue(
        DataUtilities.CodePointCompare(
          "a\ud800\ufffd",
          "a\ud800\udc00") < 0);
      Assert.IsTrue(
        DataUtilities.CodePointCompare(
          "a\ud800\ud7ff",
          "a\ud800\udc00") < 0);
      Assert.IsTrue(
        DataUtilities.CodePointCompare(
          "a\ufffd\udc00",
          "a\ud800\udc00") < 0);
      Assert.IsTrue(
        DataUtilities.CodePointCompare(
          "a\ud7ff\udc00",
          "a\ud800\udc00") < 0);
    }

    public static String Repeat(String c, int num) {
      var sb = new StringBuilder();
      for (var i = 0; i < num; ++i) {
        sb.Append(c);
      }
      return sb.ToString();
    }

    private void TestUtf8RoundTrip(string str) {
      Assert.AreEqual(
  str,
  DataUtilities.GetUtf8String(DataUtilities.GetUtf8Bytes(str, true), true));
    }

    [Test]
    public void TestGetUtf8Bytes() {
      try {
        DataUtilities.GetUtf8Bytes("\ud800", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\udc00", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\ud800\ud800", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\udc00\udc00", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\udc00\ud800", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes(null, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\ud800", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\udc00", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\ud800X", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\udc00X", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\ud800\ud800", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\udc00\ud800", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\udc00\ud800\udc00", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\ud800\ud800\udc00", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\udc00\udc00", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      TestCommon.AssertByteArraysEqual(
        new byte[] { 0xf0, 0x90, 0x80, 0x80 },
        DataUtilities.GetUtf8Bytes("\ud800\udc00", false));
      TestCommon.AssertByteArraysEqual(
        new byte[] { 0xef, 0xbf, 0xbd },
        DataUtilities.GetUtf8Bytes("\ud800", true));
      TestCommon.AssertByteArraysEqual(
        new byte[] { 0xef, 0xbf, 0xbd },
        DataUtilities.GetUtf8Bytes("\udc00", true));
      TestCommon.AssertByteArraysEqual(
        new byte[] { 0xef, 0xbf, 0xbd, 88 },
        DataUtilities.GetUtf8Bytes("\ud800X", true));
      TestCommon.AssertByteArraysEqual(
        new byte[] { 0xef, 0xbf, 0xbd, 88 },
        DataUtilities.GetUtf8Bytes("\udc00X", true));
      TestCommon.AssertByteArraysEqual(
        new byte[] { 0xef, 0xbf, 0xbd, 0xef, 0xbf, 0xbd },
        DataUtilities.GetUtf8Bytes("\ud800\ud800", true));
      TestCommon.AssertByteArraysEqual(
        new byte[] { 0xef, 0xbf, 0xbd, 0xef, 0xbf, 0xbd },
        DataUtilities.GetUtf8Bytes("\udc00\ud800", true));
      TestCommon.AssertByteArraysEqual(
        new byte[] { 0xef, 0xbf, 0xbd, 0xf0, 0x90, 0x80, 0x80 },
        DataUtilities.GetUtf8Bytes("\udc00\ud800\udc00", true));
      TestCommon.AssertByteArraysEqual(
        new byte[] { 0xef, 0xbf, 0xbd, 0xf0, 0x90, 0x80, 0x80 },
        DataUtilities.GetUtf8Bytes("\ud800\ud800\udc00", true));
      TestCommon.AssertByteArraysEqual(
        new byte[] { 0xef, 0xbf, 0xbd, 0xef, 0xbf, 0xbd },
        DataUtilities.GetUtf8Bytes("\udc00\udc00", true));
      TestCommon.AssertByteArraysEqual(
        new byte[] { 0xf0, 0x90, 0x80, 0x80 },
        DataUtilities.GetUtf8Bytes("\ud800\udc00", false));
    }
    [Test]
    public void TestGetUtf8Length() {
      try {
        DataUtilities.GetUtf8Length(null, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(6, DataUtilities.GetUtf8Length("ABC\ud800", true));
      Assert.AreEqual(-1, DataUtilities.GetUtf8Length("ABC\ud800", false));
      try {
        DataUtilities.GetUtf8Length(null, true);
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Length(null, false);
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(3, DataUtilities.GetUtf8Length("abc", true));
      Assert.AreEqual(4, DataUtilities.GetUtf8Length("\u0300\u0300", true));
      Assert.AreEqual(6, DataUtilities.GetUtf8Length("\u3000\u3000", true));
      Assert.AreEqual(6, DataUtilities.GetUtf8Length("\ud800\ud800", true));
      Assert.AreEqual(-1, DataUtilities.GetUtf8Length("\ud800\ud800", false));
      long numberTemp;
      {
        numberTemp = DataUtilities.GetUtf8Length("\ud800", false);
        Assert.AreEqual(-1, numberTemp);
      }
      {
        numberTemp = DataUtilities.GetUtf8Length("\udc00", false);
        Assert.AreEqual(-1, numberTemp);
      }
      {
        numberTemp = DataUtilities.GetUtf8Length("\ud800X", false);
        Assert.AreEqual(-1, numberTemp);
      }
      {
        numberTemp = DataUtilities.GetUtf8Length("\udc00X", false);
        Assert.AreEqual(-1, numberTemp);
      }
      {
        numberTemp = DataUtilities.GetUtf8Length("\ud800\ud800", false);
        Assert.AreEqual(-1, numberTemp);
      }
      {
        numberTemp = DataUtilities.GetUtf8Length("\udc00\ud800", false);
        Assert.AreEqual(-1, numberTemp);
      }
      {
        numberTemp = DataUtilities.GetUtf8Length("\udc00\ud800\udc00", false);
        Assert.AreEqual(-1, numberTemp);
      }
      {
        numberTemp = DataUtilities.GetUtf8Length("\ud800\ud800\udc00", false);
        Assert.AreEqual(-1, numberTemp);
      }
      {
        numberTemp = DataUtilities.GetUtf8Length("\udc00\udc00", false);
        Assert.AreEqual(-1, numberTemp);
      }
      {
        numberTemp = DataUtilities.GetUtf8Length("\ud800\udc00", false);
        Assert.AreEqual(4, numberTemp);
      }
      {
        numberTemp = DataUtilities.GetUtf8Length("\ud800", true);
        Assert.AreEqual(3, numberTemp);
      }
      {
        numberTemp = DataUtilities.GetUtf8Length("\udc00", true);
        Assert.AreEqual(3, numberTemp);
      }
      {
        numberTemp = DataUtilities.GetUtf8Length("\ud800X", true);
        Assert.AreEqual(4, numberTemp);
      }
      {
        numberTemp = DataUtilities.GetUtf8Length("\udc00X", true);
        Assert.AreEqual(4, numberTemp);
      }
      {
        numberTemp = DataUtilities.GetUtf8Length("\ud800\ud800", true);
        Assert.AreEqual(6, numberTemp);
      }
      {
        numberTemp = DataUtilities.GetUtf8Length("\udc00\ud800", true);
        Assert.AreEqual(6, numberTemp);
      }
      {
        numberTemp = DataUtilities.GetUtf8Length("\udc00\ud800\udc00", true);
        Assert.AreEqual(7, numberTemp);
      }
      {
        numberTemp = DataUtilities.GetUtf8Length("\ud800\ud800\udc00", true);
        Assert.AreEqual(7, numberTemp);
      }
      {
        numberTemp = DataUtilities.GetUtf8Length("\udc00\udc00", true);
        Assert.AreEqual(6, numberTemp);
      }
      {
        numberTemp = DataUtilities.GetUtf8Length("\ud800\udc00", false);
        Assert.AreEqual(4, numberTemp);
      }
    }
    [Test]
    public void TestGetUtf8String() {
      this.TestUtf8RoundTrip("A" + Repeat("\u00e0", 10000));
      this.TestUtf8RoundTrip("AA" + Repeat("\u00e0", 10000));
      this.TestUtf8RoundTrip("AAA" + Repeat("\u00e0", 10000));
      this.TestUtf8RoundTrip("AAAA" + Repeat("\u00e0", 10000));
      this.TestUtf8RoundTrip("A" + Repeat("\u0ae0", 10000));
      this.TestUtf8RoundTrip("AA" + Repeat("\u0ae0", 10000));
      this.TestUtf8RoundTrip("AAA" + Repeat("\u0ae0", 10000));
      this.TestUtf8RoundTrip("AAAA" + Repeat("\u0ae0", 10000));
      this.TestUtf8RoundTrip("A" + Repeat("\ud800\udc00", 10000));
      this.TestUtf8RoundTrip("AA" + Repeat("\ud800\udc00", 10000));
      this.TestUtf8RoundTrip("AAA" + Repeat("\ud800\udc00", 10000));
      this.TestUtf8RoundTrip("AAAA" + Repeat("\ud800\udc00", 10000));

      try {
        DataUtilities.GetUtf8String(null, 0, 1, false);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8String(null, false);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8String(null, 0, 1, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8String(new byte[] { 0 }, -1, 1, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8String(new byte[] { 0 }, 2, 1, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8String(new byte[] { 0 }, 0, -1, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8String(new byte[] { 0 }, 0, 2, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8String(new byte[] { 0 }, 1, 1, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      string strtemp = DataUtilities.GetUtf8String(
        new byte[] { 0x41, 0x42, 0x43 },
        0,
        3,
        true);
      Assert.AreEqual(
        "ABC",
        strtemp);
      {
        string stringTemp = DataUtilities.GetUtf8String(
          new byte[] { 0x41, 0x42, 0x43, 0x80 },
          0,
          4,
          true);
        Assert.AreEqual(
          "ABC\ufffd",
          stringTemp);
      }
      try {
        DataUtilities.GetUtf8String(
          new byte[] { 0x41, 0x42, 0x43, 0x80 },
          0,
          4,
          false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      IList<byte[]> illegalSeqs = GenerateIllegalUtf8Sequences();
      foreach (byte[] seq in illegalSeqs) {
        try {
          DataUtilities.GetUtf8String(seq, false);
          Assert.Fail("Should have failed");
        } catch (ArgumentException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        string strret = DataUtilities.GetUtf8String(seq, true);
        Assert.IsTrue(strret.Length > 0);
        Assert.AreEqual('\ufffd', strret[0]);
        try {
          DataUtilities.GetUtf8String(seq, 0, seq.Length, false);
          Assert.Fail("Should have failed");
        } catch (ArgumentException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        strret = DataUtilities.GetUtf8String(seq, 0, seq.Length, true);
        Assert.IsTrue(strret.Length > 0);
        Assert.AreEqual('\ufffd', strret[0]);
      }
    }

    public static void DoTestReadUtf8(
  byte[] bytes,
  int expectedRet,
  string expectedString,
  int noReplaceRet,
  string noReplaceString) {
      DoTestReadUtf8(
        bytes,
        bytes.Length,
        expectedRet,
        expectedString,
        noReplaceRet,
        noReplaceString);
    }

    public static void DoTestReadUtf8(
      byte[] bytes,
      int length,
      int expectedRet,
      string expectedString,
      int noReplaceRet,
      string noReplaceString) {
      try {
        var builder = new StringBuilder();
        var ret = 0;
        using (var ms = new MemoryStream(bytes)) {
          ret = DataUtilities.ReadUtf8(ms, length, builder, true);
          Assert.AreEqual(expectedRet, ret);
          if (expectedRet == 0) {
            Assert.AreEqual(expectedString, builder.ToString());
          }
          ms.Position = 0;
          builder.Clear();
          ret = DataUtilities.ReadUtf8(ms, length, builder, false);
          Assert.AreEqual(noReplaceRet, ret);
          if (noReplaceRet == 0) {
            Assert.AreEqual(noReplaceString, builder.ToString());
          }
        }
        if (bytes.Length >= length) {
          builder.Clear();
          ret = DataUtilities.ReadUtf8FromBytes(
            bytes,
            0,
            length,
            builder,
            true);
          Assert.AreEqual(expectedRet, ret);
          if (expectedRet == 0) {
            Assert.AreEqual(expectedString, builder.ToString());
          }
          builder.Clear();
          ret = DataUtilities.ReadUtf8FromBytes(
            bytes,
            0,
            length,
            builder,
            false);
          Assert.AreEqual(noReplaceRet, ret);
          if (noReplaceRet == 0) {
            Assert.AreEqual(noReplaceString, builder.ToString());
          }
        }
      } catch (IOException ex) {
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestReadUtf8() {
      try {
        DataUtilities.ReadUtf8(null, 1, null, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      {
        using (var ms = new MemoryStream(new byte[] { 0 })) {
          try {
            DataUtilities.ReadUtf8(ms, 1, null, true);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
      }
      {
        using (var ms = new MemoryStream(new byte[] { 0 })) {
          try {
            DataUtilities.ReadUtf8(ms, 1, null, false);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
      }
      DoTestReadUtf8(
  new byte[] { 0x21, 0x21, 0x21 },
  0,
 "!!!",
 0,
 "!!!");
      DoTestReadUtf8(
        new byte[] { 0x20, 0xc2, 0x80 },
        0,
 " \u0080",
 0,
 " \u0080");
      DoTestReadUtf8(
        new byte[] { 0x20, 0xc2, 0x80, 0x20 },
        0,
 " \u0080 ",
 0,
 " \u0080 ");
      DoTestReadUtf8(
        new byte[] { 0x20, 0xc2, 0x80, 0xc2 },
        0,
 " \u0080\ufffd",
 -1,
 null);
      DoTestReadUtf8(
        new byte[] { 0x20, 0xc2, 0x21, 0x21 },
        0,
 " \ufffd!!",
 -1,
        null);
      DoTestReadUtf8(
        new byte[] { 0x20, 0xc2, 0xff, 0x20 },
        0,
 " \ufffd\ufffd ",
 -1,
 null);
      DoTestReadUtf8(
        new byte[] { 0x20, 0xe0, 0xa0, 0x80 },
        0,
 " \u0800",
 0,
 " \u0800");
      DoTestReadUtf8(
    new byte[] { 0x20, 0xe0, 0xa0, 0x80, 0x20 }, 0, " \u0800 ", 0, " \u0800 ");
      DoTestReadUtf8(
        new byte[] { 0x20, 0xf0, 0x90, 0x80, 0x80 },
 0,
 " \ud800\udc00",
 0,
          " \ud800\udc00");
      DoTestReadUtf8(
        new byte[] { 0x20, 0xf0, 0x90, 0x80, 0x80 },
        3,
        0,
 " \ufffd",
 -1,
 null);
      DoTestReadUtf8(
        new byte[] { 0x20, 0xf0, 0x90 },
        5,
        -2,
        null,
        -1,
        null);
      DoTestReadUtf8(
        new byte[] { 0x20, 0x20, 0x20 },
        5,
        -2,
        null,
        -2,
        null);
      DoTestReadUtf8(
        new byte[] { 0x20, 0xf0, 0x90, 0x80, 0x80, 0x20 },
 0,
 " \ud800\udc00 ",
          0,
 " \ud800\udc00 ");
      DoTestReadUtf8(
        new byte[] { 0x20, 0xf0, 0x90, 0x80, 0x20 },
 0,
 " \ufffd ",
 -1,
        null);
      DoTestReadUtf8(
        new byte[] { 0x20, 0xf0, 0x90, 0x20 },
        0,
 " \ufffd ",
 -1,
 null);
      DoTestReadUtf8(
        new byte[] { 0x20, 0xf0, 0x90, 0x80, 0xff },
        0,
 " \ufffd\ufffd",
 -1,
 null);
      DoTestReadUtf8(
        new byte[] { 0x20, 0xf0, 0x90, 0xff },
        0,
 " \ufffd\ufffd",
 -1,
        null);
      DoTestReadUtf8(
        new byte[] { 0x20, 0xe0, 0xa0, 0x20 },
        0,
 " \ufffd ",
 -1,
 null);
      DoTestReadUtf8(
        new byte[] { 0x20, 0xe0, 0x20 },
        0,
 " \ufffd ",
 -1,
 null);
      DoTestReadUtf8(
        new byte[] { 0x20, 0xe0, 0xa0, 0xff },
        0,
 " \ufffd\ufffd",
 -1,
        null);
      DoTestReadUtf8(
        new byte[] { 0x20, 0xe0, 0xff },
 0,
 " \ufffd\ufffd",
 -1,
        null);
    }
    [Test]
    public void TestReadUtf8FromBytes() {
      var builder = new StringBuilder();
      try {
        DataUtilities.WriteUtf8("x", 0, 1, null, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.ReadUtf8FromBytes(null, 0, 1, new StringBuilder(), true);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.ReadUtf8FromBytes(
          new byte[] { 0 },
          -1,
          1,
          new StringBuilder(),
          true);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.ReadUtf8FromBytes(
          new byte[] { 0 },
          2,
          1,
          new StringBuilder(),
          true);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.ReadUtf8FromBytes(
          new byte[] { 0 },
          0,
          -1,
          new StringBuilder(),
          true);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.ReadUtf8FromBytes(
          new byte[] { 0 },
          0,
          2,
          new StringBuilder(),
          true);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.ReadUtf8FromBytes(
          new byte[] { 0 },
          1,
          1,
          new StringBuilder(),
          true);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.ReadUtf8FromBytes(new byte[] { 0 }, 0, 1, null, false);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      builder = new StringBuilder();
      {
        long numberTemp = DataUtilities.ReadUtf8FromBytes(
          new byte[] { 0xf0, 0x90, 0x80, 0x80 },
          0,
          4,
          builder,
          false);
        Assert.AreEqual(0, numberTemp);
      }
      {
        string stringTemp = builder.ToString();
        Assert.AreEqual(
          "\ud800\udc00",
          stringTemp);
      }
      foreach (byte[] seq in GenerateIllegalUtf8Sequences()) {
        {
          long numberTemp = DataUtilities.ReadUtf8FromBytes(
            seq,
            0,
            seq.Length,
            builder,
            false);
          Assert.AreEqual(-1, numberTemp);
        }
      }
    }
    [Test]
    public void TestReadUtf8ToString() {
      try {
        DataUtilities.ReadUtf8ToString(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.ReadUtf8ToString(null, 1, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      IList<byte[]> illegalSeqs = GenerateIllegalUtf8Sequences();
      foreach (byte[] seq in illegalSeqs) {
        using (var ms = new MemoryStream(seq)) {
          try {
            DataUtilities.ReadUtf8ToString(ms, -1, false);
            Assert.Fail("Should have failed");
          } catch (IOException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var ms2 = new MemoryStream(seq)) {
          String strret = null;
          try {
            strret = DataUtilities.ReadUtf8ToString(ms2, -1, true);
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          Assert.IsTrue(strret.Length > 0);
          Assert.AreEqual('\ufffd', strret[0]);
        }
      }
    }
    [Test]
    public void TestToLowerCaseAscii() {
      if (DataUtilities.ToLowerCaseAscii(null) != null) {
        Assert.Fail();
      }
      {
        string stringTemp = DataUtilities.ToLowerCaseAscii("abc012-=?");
        Assert.AreEqual(
          "abc012-=?",
          stringTemp);
      }
      {
        string stringTemp = DataUtilities.ToLowerCaseAscii("ABC012-=?");
        Assert.AreEqual(
          "abc012-=?",
          stringTemp);
      }
    }
    [Test]
    public void TestToUpperCaseAscii() {
      if (DataUtilities.ToUpperCaseAscii(null) != null) {
        Assert.Fail();
      }
      {
        string stringTemp = DataUtilities.ToUpperCaseAscii("abc012-=?");
        Assert.AreEqual(
          "ABC012-=?",
          stringTemp);
      }
      {
        string stringTemp = DataUtilities.ToUpperCaseAscii("ABC012-=?");
        Assert.AreEqual(
          "ABC012-=?",
          stringTemp);
      }
    }
    [Test]
    public void TestWriteUtf8() {
      try {
        try {
          DataUtilities.WriteUtf8(null, 0, 1, null, false);
          Assert.Fail("Should have failed");
        } catch (ArgumentNullException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        try {
          DataUtilities.WriteUtf8("xyz", 0, 1, null, false);
          Assert.Fail("Should have failed");
        } catch (ArgumentNullException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        try {
          DataUtilities.WriteUtf8(null, null, false);
          Assert.Fail("Should have failed");
        } catch (ArgumentNullException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        try {
          DataUtilities.WriteUtf8("xyz", null, false);
          Assert.Fail("Should have failed");
        } catch (ArgumentNullException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        {
          using (var ms = new MemoryStream()) {
            try {
              DataUtilities.WriteUtf8("x", null, true);
              Assert.Fail("Should have failed");
            } catch (ArgumentNullException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            try {
              DataUtilities.WriteUtf8("x", 0, 1, null, true);
              Assert.Fail("Should have failed");
            } catch (ArgumentNullException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            try {
              DataUtilities.WriteUtf8("x", 0, 1, null, true, true);
              Assert.Fail("Should have failed");
            } catch (ArgumentNullException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            try {
              DataUtilities.WriteUtf8(null, 0, 1, ms, true);
              Assert.Fail("Should have failed");
            } catch (ArgumentNullException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            try {
              DataUtilities.WriteUtf8("x", -1, 1, ms, true);
              Assert.Fail("Should have failed");
            } catch (ArgumentException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            try {
              DataUtilities.WriteUtf8("x", 2, 1, ms, true);
              Assert.Fail("Should have failed");
            } catch (ArgumentException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            try {
              DataUtilities.WriteUtf8("x", 0, -1, ms, true);
              Assert.Fail("Should have failed");
            } catch (ArgumentException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            try {
              DataUtilities.WriteUtf8("x", 0, 2, ms, true);
              Assert.Fail("Should have failed");
            } catch (ArgumentException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            try {
              DataUtilities.WriteUtf8("x", 1, 1, ms, true);
              Assert.Fail("Should have failed");
            } catch (ArgumentException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            try {
              DataUtilities.WriteUtf8(null, 0, 1, ms, true, true);
              Assert.Fail("Should have failed");
            } catch (ArgumentNullException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            try {
              DataUtilities.WriteUtf8("x", -1, 1, ms, true, true);
              Assert.Fail("Should have failed");
            } catch (ArgumentException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            try {
              DataUtilities.WriteUtf8("x", 2, 1, ms, true, true);
              Assert.Fail("Should have failed");
            } catch (ArgumentException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            try {
              DataUtilities.WriteUtf8("x", 0, -1, ms, true, true);
              Assert.Fail("Should have failed");
            } catch (ArgumentException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            try {
              DataUtilities.WriteUtf8("x", 0, 2, ms, true, true);
              Assert.Fail("Should have failed");
            } catch (ArgumentException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            try {
              DataUtilities.WriteUtf8("x", 1, 1, ms, true, true);
              Assert.Fail("Should have failed");
            } catch (ArgumentException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            try {
              DataUtilities.WriteUtf8(null, null, false);
              Assert.Fail("Should have failed");
            } catch (ArgumentNullException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            try {
              DataUtilities.WriteUtf8(null, ms, false);
              Assert.Fail("Should have failed");
            } catch (ArgumentNullException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
          }
        }
        {
          {
            using (var ms = new MemoryStream()) {
              DataUtilities.WriteUtf8("0\r1", 0, 3, ms, true, true);
              TestCommon.AssertByteArraysEqual(
                new byte[] { 0x30, 0x0d, 0x0a, 0x31 },
                ms.ToArray());
            }
          }
          {
            using (var ms = new MemoryStream()) {
              DataUtilities.WriteUtf8("0\n1", 0, 3, ms, true, true);
              TestCommon.AssertByteArraysEqual(
                new byte[] { 0x30, 0x0d, 0x0a, 0x31 },
                ms.ToArray());
            }
          }
          {
            using (var ms = new MemoryStream()) {
              DataUtilities.WriteUtf8("0\r\n1", 0, 4, ms, true, true);
              TestCommon.AssertByteArraysEqual(
                new byte[] { 0x30, 0x0d, 0x0a, 0x31 },
                ms.ToArray());
            }
          }
          {
            using (var ms = new MemoryStream()) {
              DataUtilities.WriteUtf8("0\r\r1", 0, 4, ms, true, true);
              TestCommon.AssertByteArraysEqual(
                new byte[] { 0x30, 0x0d, 0x0a, 0x0d, 0x0a, 0x31 },
                ms.ToArray());
            }
          }
          {
            using (var ms = new MemoryStream()) {
              DataUtilities.WriteUtf8("0\n\r1", 0, 4, ms, true, true);
              TestCommon.AssertByteArraysEqual(
                new byte[] { 0x30, 0x0d, 0x0a, 0x0d, 0x0a, 0x31 },
                ms.ToArray());
            }
          }
          {
            using (var ms = new MemoryStream()) {
              DataUtilities.WriteUtf8("0\r\r\n1", 0, 5, ms, true, true);
              TestCommon.AssertByteArraysEqual(
                new byte[] { 0x30, 0x0d, 0x0a, 0x0d, 0x0a, 0x31 },
                ms.ToArray());
            }
          }
          {
            using (var ms = new MemoryStream()) {
              DataUtilities.WriteUtf8("0\n\r\n1", 0, 5, ms, true, true);
              TestCommon.AssertByteArraysEqual(
                new byte[] { 0x30, 0x0d, 0x0a, 0x0d, 0x0a, 0x31 },
                ms.ToArray());
            }
          }
          {
            using (var ms = new MemoryStream()) {
              DataUtilities.WriteUtf8("0\n\n\r1", 0, 5, ms, true, true);
              TestCommon.AssertByteArraysEqual(
                new byte[] { 0x30, 0x0d, 0x0a, 0x0d, 0x0a, 0x0d, 0x0a, 0x31 },
                ms.ToArray());
            }
          }
          {
            using (var ms = new MemoryStream()) {
              DataUtilities.WriteUtf8("0\r\r\r1", 0, 5, ms, true, true);
              TestCommon.AssertByteArraysEqual(
                new byte[] { 0x30, 0x0d, 0x0a, 0x0d, 0x0a, 0x0d, 0x0a, 0x31 },
                ms.ToArray());
            }
          }
        }
      } catch (Exception ex) {
        throw new InvalidOperationException(ex.Message, ex);
      }
    }
  }
}
