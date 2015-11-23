using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeterO;

namespace Test {
  [TestClass]
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

    [TestMethod]
    public void TestCodePointAt() {
      try {
        DataUtilities.CodePointAt(null, 0);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
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
  Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\ud800\ud800\udc00" , 0));
  Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\udc00\ud800\udc00" , 0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\udc00\udc00", 0));
      Assert.AreEqual(0x10000, DataUtilities.CodePointAt("\ud800\udc00", 0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\ud800", 0, 0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\udc00", 0, 0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\ud800X", 0, 0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\udc00X", 0, 0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\ud800\ud800", 0, 0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\udc00\ud800", 0, 0));
Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\ud800\ud800\udc00" , 0,0));
Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\udc00\ud800\udc00" , 0,0));
      Assert.AreEqual(0xfffd, DataUtilities.CodePointAt("\udc00\udc00", 0, 0));
      Assert.AreEqual(0x10000, DataUtilities.CodePointAt("\ud800\udc00", 0, 0));

      Assert.AreEqual(0xd800, DataUtilities.CodePointAt("\ud800", 0, 1));
      Assert.AreEqual(0xdc00, DataUtilities.CodePointAt("\udc00", 0, 1));
      Assert.AreEqual(0xd800, DataUtilities.CodePointAt("\ud800X", 0, 1));
      Assert.AreEqual(0xdc00, DataUtilities.CodePointAt("\udc00X", 0, 1));
      Assert.AreEqual(0xd800, DataUtilities.CodePointAt("\ud800\ud800", 0, 1));
Assert.AreEqual(0xd800, DataUtilities.CodePointAt("\ud800\ud800\udc00" , 0,1));
      Assert.AreEqual(0xdc00, DataUtilities.CodePointAt("\udc00\ud800", 0, 1));
Assert.AreEqual(0xdc00, DataUtilities.CodePointAt("\udc00\ud800\udc00" , 0,1));
      Assert.AreEqual(0xdc00, DataUtilities.CodePointAt("\udc00\udc00", 0, 1));
      Assert.AreEqual(0x10000, DataUtilities.CodePointAt("\ud800\udc00", 0, 1));

      Assert.AreEqual(-1, DataUtilities.CodePointAt("\ud800", 0, 2));
      Assert.AreEqual(-1, DataUtilities.CodePointAt("\udc00", 0, 2));
      Assert.AreEqual(-1, DataUtilities.CodePointAt("\ud800X", 0, 2));
      Assert.AreEqual(-1, DataUtilities.CodePointAt("\udc00X", 0, 2));
      Assert.AreEqual(-1, DataUtilities.CodePointAt("\ud800\ud800", 0, 2));
   Assert.AreEqual(-1, DataUtilities.CodePointAt("\ud800\ud800\udc00" , 0, 2));
      Assert.AreEqual(-1, DataUtilities.CodePointAt("\udc00\ud800", 0, 2));
   Assert.AreEqual(-1, DataUtilities.CodePointAt("\udc00\ud800\udc00" , 0, 2));
      Assert.AreEqual(-1, DataUtilities.CodePointAt("\udc00\udc00", 0, 2));
      Assert.AreEqual(0x10000, DataUtilities.CodePointAt("\ud800\udc00", 0, 2));
    }
    [TestMethod]
    public void TestCodePointBefore() {
      try {
        DataUtilities.CodePointBefore(null, 0);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // not implemented yet
    }
    [TestMethod]
    public void TestCodePointCompare() {
      Assert.AreEqual(-1, Math.Sign(DataUtilities.CodePointCompare(null, "A")));
      Assert.AreEqual(1, Math.Sign(DataUtilities.CodePointCompare("A", null)));
      Assert.AreEqual(0, Math.Sign(DataUtilities.CodePointCompare(null, null)));
      {
     long numberTemp = Math.Sign(DataUtilities.CodePointCompare("abc" , "abc"
));
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
          "a\ud800\udc00") ==0);
      Assert.IsTrue(DataUtilities.CodePointCompare("a\ud800", "a\ud800") ==0);
      Assert.IsTrue(DataUtilities.CodePointCompare("a\udc00", "a\udc00") ==0);
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
    [TestMethod]
    public void TestGetUtf8Bytes() {
      try {
        DataUtilities.GetUtf8Bytes(null, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\ud800", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\udc00", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\ud800X", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\udc00X", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\ud800\ud800", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\udc00\ud800", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\udc00\ud800\udc00", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\ud800\ud800\udc00", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Bytes("\udc00\udc00", false);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
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
    [TestMethod]
    public void TestGetUtf8Length() {
      try {
        DataUtilities.GetUtf8Length(null, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(6, DataUtilities.GetUtf8Length("ABC\ud800", true));
      Assert.AreEqual(-1, DataUtilities.GetUtf8Length("ABC\ud800", false));
      try {
        DataUtilities.GetUtf8Length(null, true);
      } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8Length(null, false);
      } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString()); throw new
          InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(3, DataUtilities.GetUtf8Length("abc", true));
      Assert.AreEqual(4, DataUtilities.GetUtf8Length("\u0300\u0300", true));
      Assert.AreEqual(6, DataUtilities.GetUtf8Length("\u3000\u3000", true));
      Assert.AreEqual(6, DataUtilities.GetUtf8Length("\ud800\ud800", true));
      Assert.AreEqual(-1, DataUtilities.GetUtf8Length("\ud800\ud800", false));
      {
        long numberTemp = DataUtilities.GetUtf8Length("\ud800", false);
        Assert.AreEqual(-1, numberTemp);
      }
      {
        long numberTemp = DataUtilities.GetUtf8Length("\udc00", false);
        Assert.AreEqual(-1, numberTemp);
      }
      {
        long numberTemp = DataUtilities.GetUtf8Length("\ud800X", false);
        Assert.AreEqual(-1, numberTemp);
      }
      {
        long numberTemp = DataUtilities.GetUtf8Length("\udc00X", false);
        Assert.AreEqual(-1, numberTemp);
      }
      {
        long numberTemp = DataUtilities.GetUtf8Length("\ud800\ud800", false);
        Assert.AreEqual(-1, numberTemp);
      }
      {
        long numberTemp = DataUtilities.GetUtf8Length("\udc00\ud800", false);
        Assert.AreEqual(-1, numberTemp);
      }
      {
    long numberTemp = DataUtilities.GetUtf8Length("\udc00\ud800\udc00" , false);
        Assert.AreEqual(-1, numberTemp);
      }
      {
    long numberTemp = DataUtilities.GetUtf8Length("\ud800\ud800\udc00" , false);
        Assert.AreEqual(-1, numberTemp);
      }
      {
        long numberTemp = DataUtilities.GetUtf8Length("\udc00\udc00", false);
        Assert.AreEqual(-1, numberTemp);
      }
      {
        long numberTemp = DataUtilities.GetUtf8Length("\ud800\udc00", false);
        Assert.AreEqual(4, numberTemp);
      }
      {
        long numberTemp = DataUtilities.GetUtf8Length("\ud800", true);
        Assert.AreEqual(3, numberTemp);
      }
      {
        long numberTemp = DataUtilities.GetUtf8Length("\udc00", true);
        Assert.AreEqual(3, numberTemp);
      }
      {
        long numberTemp = DataUtilities.GetUtf8Length("\ud800X", true);
        Assert.AreEqual(4, numberTemp);
      }
      {
        long numberTemp = DataUtilities.GetUtf8Length("\udc00X", true);
        Assert.AreEqual(4, numberTemp);
      }
      {
        long numberTemp = DataUtilities.GetUtf8Length("\ud800\ud800", true);
        Assert.AreEqual(6, numberTemp);
      }
      {
        long numberTemp = DataUtilities.GetUtf8Length("\udc00\ud800", true);
        Assert.AreEqual(6, numberTemp);
      }
      {
     long numberTemp = DataUtilities.GetUtf8Length("\udc00\ud800\udc00" , true);
        Assert.AreEqual(7, numberTemp);
      }
      {
     long numberTemp = DataUtilities.GetUtf8Length("\ud800\ud800\udc00" , true);
        Assert.AreEqual(7, numberTemp);
      }
      {
        long numberTemp = DataUtilities.GetUtf8Length("\udc00\udc00", true);
        Assert.AreEqual(6, numberTemp);
      }
      {
        long numberTemp = DataUtilities.GetUtf8Length("\ud800\udc00", false);
        Assert.AreEqual(4, numberTemp);
      }
    }
    [TestMethod]
    public void TestGetUtf8String() {
      try {
        DataUtilities.GetUtf8String(null, false);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8String(null, 0, 1, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8String(new byte[] { 0 }, -1, 1, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8String(new byte[] { 0 }, 2, 1, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8String(new byte[] { 0 }, 0, -1, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8String(new byte[] { 0 }, 0, 2, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.GetUtf8String(new byte[] { 0 }, 1, 1, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
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
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      IList<byte[]> illegalSeqs = GenerateIllegalUtf8Sequences();
      foreach (byte[] seq in illegalSeqs) {
        try {
          DataUtilities.GetUtf8String(seq, false);
          Assert.Fail("Should have failed");
        } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
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
        } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        strret = DataUtilities.GetUtf8String(seq, 0, seq.Length, true);
        Assert.IsTrue(strret.Length > 0);
        Assert.AreEqual('\ufffd', strret[0]);
      }
    }
    [TestMethod]
    public void TestReadUtf8() {
      try {
        DataUtilities.ReadUtf8(null, 1, null, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      var ms = new MemoryStream(new byte[] { 0 });
      try {
 DataUtilities.ReadUtf8(ms, 1, null, true);
Assert.Fail("Should have failed");
} catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      ms = new MemoryStream(new byte[] { 0 });
      try {
 DataUtilities.ReadUtf8(ms, 1, null, false);
Assert.Fail("Should have failed");
} catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
    }
    [TestMethod]
    public void TestReadUtf8FromBytes() {
      var builder = new StringBuilder();
      try {
        DataUtilities.WriteUtf8("x", 0, 1, null, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.ReadUtf8FromBytes(null, 0, 1, new StringBuilder(), true);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
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
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
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
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
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
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
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
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
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
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.ReadUtf8FromBytes(new byte[] { 0 }, 0, 1, null, false);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
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
    [TestMethod]
    public void TestReadUtf8ToString() {
      try {
        DataUtilities.ReadUtf8ToString(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        DataUtilities.ReadUtf8ToString(null, 1, true);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
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
          } catch (IOException ex) {
Console.WriteLine(ex.Message);
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
    [TestMethod]
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
    [TestMethod]
    public void TestWriteUtf8() {
      try {
        {
           using (var ms = new MemoryStream()) {
          try {
            DataUtilities.WriteUtf8("x", null, true);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            DataUtilities.WriteUtf8("x", 0, 1, null, true);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            DataUtilities.WriteUtf8("x", 0, 1, null, true, true);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            DataUtilities.WriteUtf8(null, 0, 1, ms, true);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            DataUtilities.WriteUtf8("x", -1, 1, ms, true);
            Assert.Fail("Should have failed");
          } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            DataUtilities.WriteUtf8("x", 2, 1, ms, true);
            Assert.Fail("Should have failed");
          } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            DataUtilities.WriteUtf8("x", 0, -1, ms, true);
            Assert.Fail("Should have failed");
          } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            DataUtilities.WriteUtf8("x", 0, 2, ms, true);
            Assert.Fail("Should have failed");
          } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            DataUtilities.WriteUtf8("x", 1, 1, ms, true);
            Assert.Fail("Should have failed");
          } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            DataUtilities.WriteUtf8(null, 0, 1, ms, true, true);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            DataUtilities.WriteUtf8("x", -1, 1, ms, true, true);
            Assert.Fail("Should have failed");
          } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            DataUtilities.WriteUtf8("x", 2, 1, ms, true, true);
            Assert.Fail("Should have failed");
          } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            DataUtilities.WriteUtf8("x", 0, -1, ms, true, true);
            Assert.Fail("Should have failed");
          } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            DataUtilities.WriteUtf8("x", 0, 2, ms, true, true);
            Assert.Fail("Should have failed");
          } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            DataUtilities.WriteUtf8("x", 1, 1, ms, true, true);
            Assert.Fail("Should have failed");
          } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            DataUtilities.WriteUtf8(null, null, false);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            DataUtilities.WriteUtf8(null, ms, false);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        }
        {
          var ms = new MemoryStream();
          DataUtilities.WriteUtf8("0\r1", 0, 3, ms, true, true);
          TestCommon.AssertByteArraysEqual(
            new byte[] { 0x30, 0x0d, 0x0a, 0x31 },
            ms.ToArray());
          ms = new MemoryStream();
          DataUtilities.WriteUtf8("0\n1", 0, 3, ms, true, true);
          TestCommon.AssertByteArraysEqual(
            new byte[] { 0x30, 0x0d, 0x0a, 0x31 },
            ms.ToArray());
          ms = new MemoryStream();
          DataUtilities.WriteUtf8("0\r\n1", 0, 4, ms, true, true);
          TestCommon.AssertByteArraysEqual(
            new byte[] { 0x30, 0x0d, 0x0a, 0x31 },
            ms.ToArray());
          ms = new MemoryStream();
          DataUtilities.WriteUtf8("0\r\r1", 0, 4, ms, true, true);
          TestCommon.AssertByteArraysEqual(
            new byte[] { 0x30, 0x0d, 0x0a, 0x0d, 0x0a, 0x31 },
            ms.ToArray());
          ms = new MemoryStream();
          DataUtilities.WriteUtf8("0\n\r1", 0, 4, ms, true, true);
          TestCommon.AssertByteArraysEqual(
            new byte[] { 0x30, 0x0d, 0x0a, 0x0d, 0x0a, 0x31 },
            ms.ToArray());
          ms = new MemoryStream();
          DataUtilities.WriteUtf8("0\r\r\n1", 0, 5, ms, true, true);
          TestCommon.AssertByteArraysEqual(
            new byte[] { 0x30, 0x0d, 0x0a, 0x0d, 0x0a, 0x31 },
            ms.ToArray());
          ms = new MemoryStream();
          DataUtilities.WriteUtf8("0\n\r\n1", 0, 5, ms, true, true);
          TestCommon.AssertByteArraysEqual(
            new byte[] { 0x30, 0x0d, 0x0a, 0x0d, 0x0a, 0x31 },
            ms.ToArray());
          ms = new MemoryStream();
          DataUtilities.WriteUtf8("0\n\n\r1", 0, 5, ms, true, true);
          TestCommon.AssertByteArraysEqual(
            new byte[] { 0x30, 0x0d, 0x0a, 0x0d, 0x0a, 0x0d, 0x0a, 0x31 },
            ms.ToArray());
          ms = new MemoryStream();
          DataUtilities.WriteUtf8("0\r\r\r1", 0, 5, ms, true, true);
          TestCommon.AssertByteArraysEqual(
            new byte[] { 0x30, 0x0d, 0x0a, 0x0d, 0x0a, 0x0d, 0x0a, 0x31 },
            ms.ToArray());
          }
      } catch (Exception ex) {
        throw new InvalidOperationException(ex.Message, ex);
      }
    }
  }
}
