/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#if !NET20
using System.Linq;
#endif
using NUnit.Framework;
using PeterO;
using PeterO.Cbor;
using PeterO.Numbers;

namespace Test {
  [TestFixture]
  public partial class CBORExtraTest {
    private static decimal RandomDecimal(RandomGenerator rand, int exponent) {
      var x = new int[4];
      int r = rand.UniformInt(0x10000);
      r |= rand.UniformInt(0x10000) << 16;
      x[0] = r;
      if (rand.UniformInt(2) == 0) {
        r = rand.UniformInt(0x10000);
        r |= rand.UniformInt(0x10000) << 16;
        x[1] = r;
        if (rand.UniformInt(2) == 0) {
          r = rand.UniformInt(0x10000);
          r |= rand.UniformInt(0x10000) << 16;
          x[2] = r;
        }
      }
      x[3] = exponent << 16;
      if (rand.UniformInt(2) == 0) {
        x[3] |= 1 << 31;
      }
      return new decimal(x);
    }

    [Test]
    public void TestCBORObjectDecimal() {
      var rand = new RandomGenerator();
      for (int i = 0; i <= 28; ++i) {
        // Try a random decimal with a given
        // exponent
        for (int j = 0; j < 8; ++j) {
          decimal d = RandomDecimal(rand, i);
          CBORObject obj = ToObjectTest.TestToFromObjectRoundTrip(d);
          CBORTestCommon.AssertRoundTrip(obj);
          decimal decimalOther;
          try {
            decimalOther = obj.ToObject<decimal>();
          } catch (Exception ex) {
            Assert.Fail(ex.ToString() + "\r\n" + CBORTest.ObjectMessage(obj));
            throw new InvalidOperationException(String.Empty, ex);
          }
          Assert.AreEqual(d, decimalOther);
        }
      }
      try {
        _ = CBORObject.FromEDecimal(EDecimal.NaN).ToObject<decimal>();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.FromEDecimal(
          EDecimal.SignalingNaN).ToObject<decimal>();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.DecPosInf)
        .ToObject<decimal>();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.DecNegInf)
        .ToObject<decimal>();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(EFloat.NaN).ToObject<decimal>();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.FromEFloat(EFloat.SignalingNaN).ToObject<decimal>();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatPosInf)
        .ToObject<decimal>();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf)
        .ToObject<decimal>();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestSByte() {
      for (int i = sbyte.MinValue; i <= sbyte.MaxValue; ++i) {
        CBORTestCommon.AssertJSONSer(
          ToObjectTest.TestToFromObjectRoundTrip((sbyte)i),
          TestCommon.LongToString(i));
      }
    }

    private static IEnumerable<int> RangeExclusive(int min, int maxExclusive) {
      for (int i = min; i < maxExclusive; ++i) {
        yield return i;
      }
    }

    public enum CustomEnum
    {
      A,
      B,
      C,
    }

    [Flags]
    public enum CustomBits
    {
      A = 1,
      B = 2,
      C = 4,
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design",
      "CA1034",
      Justification = "Testing whether serialization works " + "on nested public types")]
    public sealed class CustomCollectionContainer {
      [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Usage",
        "CA2227",
        Justification = "Testing whether serialization works " + "on public properties of nested public types")]
      public CustomCollection CList {
        get;
        set;
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design",
      "CA1034",
      Justification = "Testing whether serialization works " + "on nested public types")]
    public sealed class CustomCollection : IList<CustomEnum>
    {
      private readonly List<CustomEnum> w = new();

      /// <inheritdoc/>
      /// <returns/>
      /// <param name='index'>Not documented yet.</param>
      public CustomEnum this[int index] {
        get => ((IList<CustomEnum>)this.w)[index];

        set => ((IList<CustomEnum>)this.w)[index] = value;
      }

      /// <inheritdoc/>
      public int Count => ((IList<CustomEnum>)this.w).Count;

      /// <inheritdoc/>
      public bool IsReadOnly => ((IList<CustomEnum>)this.w).IsReadOnly;

      /// <inheritdoc/>
      /// <param name='item'>Not documented yet.</param>
      public void Add(CustomEnum item) {
        ((IList<CustomEnum>)this.w).Add(item);
      }

      /// <inheritdoc/>
      public void Clear() {
        ((IList<CustomEnum>)this.w).Clear();
      }

      /// <inheritdoc/>
      /// <returns/>
      /// <param name='item'>Not documented yet.</param>
      public bool Contains(CustomEnum item) {
        return ((IList<CustomEnum>)this.w).Contains(item);
      }

      /// <inheritdoc/>
      /// <param name='array'>Not documented yet.</param>
      /// <param name='arrayIndex'>Not documented yet.</param>
      public void CopyTo(CustomEnum[] array, int arrayIndex) {
        ((IList<CustomEnum>)this.w).CopyTo(array, arrayIndex);
      }

      /// <inheritdoc/>
      /// <returns/>
      /// <param name='item'>Not documented yet.</param>
      public int IndexOf(CustomEnum item) {
        return ((IList<CustomEnum>)this.w).IndexOf(item);
      }

      /// <inheritdoc/>
      /// <param name='index'>Not documented yet.</param>
      /// <param name='item'>Not documented yet.</param>
      public void Insert(int index, CustomEnum item) {
        ((IList<CustomEnum>)this.w).Insert(index, item);
      }

      /// <inheritdoc/>
      /// <returns/>
      /// <param name='item'>Not documented yet.</param>
      public bool Remove(CustomEnum item) {
        return ((IList<CustomEnum>)this.w).Remove(item);
      }

      /// <inheritdoc/>
      /// <param name='index'>Not documented yet.</param>
      public void RemoveAt(int index) {
        ((IList<CustomEnum>)this.w).RemoveAt(index);
      }

      /// <inheritdoc/>
      public System.Collections.Generic.IEnumerator<CustomEnum>
GetEnumerator() {
        return ((IList<CustomEnum>)this.w).GetEnumerator();
      }

      /// <inheritdoc/>
      System.Collections.IEnumerator
System.Collections.IEnumerable.GetEnumerator() {
        return ((IList<CustomEnum>)this.w).GetEnumerator();
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design",
      "CA1034",
      Justification = "Testing whether serialization works " + "on nested public types")]
    public sealed class CustomByteCollection : IList<byte>
    {
      private readonly List<byte> w = new();

      /// <inheritdoc/>
      /// <returns/>
      /// <param name='index'>Not documented yet.</param>
      public byte this[int index] {
        get => ((IList<byte>)this.w)[index];

        set => ((IList<byte>)this.w)[index] = value;
      }

      /// <inheritdoc/>
      public int Count => ((IList<byte>)this.w).Count;

      /// <inheritdoc/>
      public bool IsReadOnly => ((IList<byte>)this.w).IsReadOnly;

      /// <inheritdoc/>
      /// <param name='item'>Not documented yet.</param>
      public void Add(byte item) {
        ((IList<byte>)this.w).Add(item);
      }

      /// <inheritdoc/>
      public void Clear() {
        ((IList<byte>)this.w).Clear();
      }

      /// <inheritdoc/>
      /// <returns/>
      /// <param name='item'>Not documented yet.</param>
      public bool Contains(byte item) {
        return ((IList<byte>)this.w).Contains(item);
      }

      /// <inheritdoc/>
      /// <param name='array'>Not documented yet.</param>
      /// <param name='arrayIndex'>Not documented yet.</param>
      public void CopyTo(byte[] array, int arrayIndex) {
        ((IList<byte>)this.w).CopyTo(array, arrayIndex);
      }

      /// <inheritdoc/>
      /// <returns/>
      /// <param name='item'>Not documented yet.</param>
      public int IndexOf(byte item) {
        return ((IList<byte>)this.w).IndexOf(item);
      }

      /// <inheritdoc/>
      /// <param name='index'>Not documented yet.</param>
      /// <param name='item'>Not documented yet.</param>
      public void Insert(int index, byte item) {
        ((IList<byte>)this.w).Insert(index, item);
      }

      /// <inheritdoc/>
      /// <returns/>
      /// <param name='item'>Not documented yet.</param>
      public bool Remove(byte item) {
        return ((IList<byte>)this.w).Remove(item);
      }

      /// <inheritdoc/>
      /// <param name='index'>Not documented yet.</param>
      public void RemoveAt(int index) {
        ((IList<byte>)this.w).RemoveAt(index);
      }

      /// <inheritdoc/>
      public System.Collections.Generic.IEnumerator<byte>
GetEnumerator() {
        return ((IList<byte>)this.w).GetEnumerator();
      }

      /// <inheritdoc/>
      System.Collections.IEnumerator
System.Collections.IEnumerable.GetEnumerator() {
        return ((IList<byte>)this.w).GetEnumerator();
      }
    }

    [Test]
    public void TestCustomFlagsEnum() {
      var cbor = CBORObject.FromObject(CustomBits.A | CustomBits.B);
      Assert.AreEqual(CBORObject.FromInt32(3), cbor);
      CustomBits cfe = cbor.ToObject<CustomBits>();
      Assert.AreEqual(CustomBits.A | CustomBits.B, cfe);
      cbor = CBORObject.FromObject(CustomBits.A);
      Assert.AreEqual(CBORObject.FromInt32(1), cbor);
      cfe = cbor.ToObject<CustomBits>();
      Assert.AreEqual(CustomBits.A, cfe);
    }

    [Test]
    public void TestCustomCollection() {
      var clist = new CustomCollection
      {
        CustomEnum.A,
        CustomEnum.B,
        CustomEnum.C,
      };
      var cbor = CBORObject.FromObject(clist);
      Console.WriteLine(cbor);
      if (cbor == null) {
        Assert.Fail();
      }
      CustomCollection clist2 = cbor.ToObject<CustomCollection>();
      Assert.AreEqual(3, clist2.Count);
      Assert.AreEqual(CustomEnum.A, clist2[0]);
      Assert.AreEqual(CustomEnum.B, clist2[1]);
      Assert.AreEqual(CustomEnum.C, clist2[2]);
      if (clist2 == null) {
        Assert.Fail();
      }
      var clc = new CustomCollectionContainer {
        CList = clist2,
      };
      cbor = CBORObject.FromObject(clc);
      Console.WriteLine(cbor);
      if (cbor == null) {
        Assert.Fail();
      }
      CustomCollectionContainer clistc =
cbor.ToObject<CustomCollectionContainer>();
      Assert.AreEqual(3, clistc.CList.Count);
      Assert.AreEqual(CustomEnum.A, clistc.CList[0]);
      Assert.AreEqual(CustomEnum.B, clistc.CList[1]);
      Assert.AreEqual(CustomEnum.C, clistc.CList[2]);
      if (clist2 == null) {
        Assert.Fail();
      }
    }

    private enum AByte : byte
    {
      /// <summary>An arbitrary value.</summary>
      A = 254,

      /// <summary>An arbitrary value.</summary>
      B,
    }

    private enum AInt
    {
      /// <summary>An arbitrary value.</summary>
      A = 256,

      /// <summary>An arbitrary value.</summary>
      B,
    }

    private enum AULong : ulong
    {
      /// <summary>An arbitrary value.</summary>
      A = 999999,

      /// <summary>An arbitrary value.</summary>
      B,
    }

    [Test]
    public void TestCPOD2() {
      var m = new CPOD2
      {
        Aa = "Test",
        IsAa = false,
      };
      var cbor = CBORObject.FromObject(m);
      // ambiguous properties
      Assert.IsFalse(cbor.ContainsKey("aa"), cbor.ToString());
      Assert.IsFalse(cbor.ContainsKey("Aa"), cbor.ToString());
    }

    [Test]
    [Timeout(5000)]
    public void TestPODOptions() {
      var ao = new { PropA = 0, PropB = 0, IsPropC = false };
      var valueCcTF = new PODOptions("removeisprefix=true;usecamelcase=false");
      var valueCcFF = new PODOptions("removeisprefix=false;usecamelcase=false");
      var valueCcFT = new PODOptions("removeisprefix=false;usecamelcase=true");
      var valueCcTT = new PODOptions("removeisprefix=true;usecamelcase=true");
      CBORObjectTest.CheckPropertyNames(ao);
      var arrao = new[] { ao, ao };
      var co = CBORObject.FromObject(arrao, valueCcTF);
      CBORObjectTest.CheckArrayPropertyNames(
        CBORObject.FromObject(arrao, valueCcTF),
        2,
        "PropA",
        "PropB",
        "IsPropC");
      CBORObjectTest.CheckArrayPropertyNames(
        CBORObject.FromObject(arrao, valueCcFF),
        2,
        "PropA",
        "PropB",
        "IsPropC");
      CBORObjectTest.CheckArrayPropertyNames(
        CBORObject.FromObject(arrao, valueCcFT),
        2,
        "propA",
        "propB",
        "propC");
      CBORObjectTest.CheckArrayPropertyNames(
        CBORObject.FromObject(arrao, valueCcTT),
        2,
        "propA",
        "propB",
        "propC");
#if !NET20
      var queryao =
from x in arrao
select x;
      co = CBORObject.FromObject(queryao, valueCcTF);
      CBORObjectTest.CheckArrayPropertyNames(
        CBORObject.FromObject(queryao, valueCcTF),
        2,
        "PropA",
        "PropB",
        "IsPropC");
      CBORObjectTest.CheckArrayPropertyNames(
        CBORObject.FromObject(queryao, valueCcFF),
        2,
        "PropA",
        "PropB",
        "IsPropC");
      CBORObjectTest.CheckArrayPropertyNames(
        CBORObject.FromObject(queryao, valueCcFT),
        2,
        "propA",
        "propB",
        "propC");
      {
        CBORObjectTest.CheckArrayPropertyNames(
          CBORObject.FromObject(queryao, valueCcTT),
          2,
          "propA",
          "propB",
          "propC");
      }
#endif
      var ao2 = new {
        PropValue = new { PropA = 0, PropB = 0, IsPropC = false, },
      };
      CBORObjectTest.CheckPODPropertyNames(
        CBORObject.FromObject(ao2, valueCcTF),
        valueCcTF,
        "PropA",
        "PropB",
        "IsPropC");
      CBORObjectTest.CheckPODPropertyNames(
        CBORObject.FromObject(ao2, valueCcFF),
        valueCcFF,
        "PropA",
        "PropB",
        "IsPropC");
      CBORObjectTest.CheckPODPropertyNames(
        CBORObject.FromObject(ao2, valueCcFT),
        valueCcFT,
        "propA",
        "propB",
        "propC");
      CBORObjectTest.CheckPODPropertyNames(
        CBORObject.FromObject(ao2, valueCcTT),
        valueCcTT,
        "propA",
        "propB",
        "propC");
      var aodict = new Dictionary<string, object>
      {
        ["PropValue"] = new { PropA = 0, PropB = 0, IsPropC = false, },
      };
      CBORObjectTest.CheckPODInDictPropertyNames(
        CBORObject.FromObject(aodict, valueCcTF),
        "PropA",
        "PropB",
        "IsPropC");
      CBORObjectTest.CheckPODInDictPropertyNames(
        CBORObject.FromObject(aodict, valueCcFF),
        "PropA",
        "PropB",
        "IsPropC");
      CBORObjectTest.CheckPODInDictPropertyNames(
        CBORObject.FromObject(aodict, valueCcFT),
        "propA",
        "propB",
        "propC");
      CBORObjectTest.CheckPODInDictPropertyNames(
        CBORObject.FromObject(aodict, valueCcTT),
        "propA",
        "propB",
        "propC");
    }

    [Test]
    public void TestArbitraryTypes() {
      var obj = CBORObject.FromObject(new
      {
        AByte.A,
        B = AInt.A,
        C = AULong.A,
      });
      if (obj == null) {
        Assert.Fail();
      }
      if (obj["a"] == null) {
        Assert.Fail();
      }
      if (obj["b"] == null) {
        Assert.Fail();
      }
      if (obj["c"] == null) {
        Assert.Fail();
      }
      Assert.AreEqual(254, obj["a"].AsInt32());
      Assert.AreEqual(256, obj["b"].AsInt32());
      Assert.AreEqual(999999, obj["c"].AsInt32());
      obj = CBORObject.FromObject(new { A = "a", B = "b" });
      {
        string stringTemp = obj["a"].AsString();
        Assert.AreEqual(
          "a",
          stringTemp);
      }
      {
        string stringTemp = obj["b"].AsString();
        Assert.AreEqual(
          "b",
          stringTemp);
      }
      CBORTestCommon.AssertRoundTrip(obj);
      obj = CBORObject.FromObject(new { A = "c", B = "b" });
      {
        string stringTemp = obj["a"].AsString();
        Assert.AreEqual(
          "c",
          stringTemp);
      }
      {
        string stringTemp = obj["b"].AsString();
        Assert.AreEqual(
          "b",
          stringTemp);
      }
      CBORTestCommon.AssertRoundTrip(obj);
      obj = CBORObject.FromObject(RangeExclusive(0, 10));
      Assert.AreEqual(10, obj.Count);
      Assert.AreEqual(0, obj[0].AsInt32());
      Assert.AreEqual(1, obj[1].AsInt32());
      obj = CBORObject.FromObject(RangeExclusive(0, 10));

      Assert.AreEqual(10, obj.Count);
      Assert.AreEqual(0, obj[0].AsInt32());
      Assert.AreEqual(1, obj[1].AsInt32());
      CBORTestCommon.AssertRoundTrip(obj);
#if !NET20
      // Select all even numbers
      IEnumerable<int> query =
from i in RangeExclusive(0, 10)
where i % 2 == 0
select i;
      obj = CBORObject.FromObject(query);
      Assert.AreEqual(5, obj.Count);
      Assert.AreEqual(0, obj[0].AsInt32());
      Assert.AreEqual(2, obj[1].AsInt32());
      CBORTestCommon.AssertRoundTrip(obj);
      // Select all even numbers
      var query2 =
from i in RangeExclusive(0, 10)
        where i % 2 == 0
 select new { A = i, B = i + 1, };
      obj = CBORObject.FromObject(query2);
      Assert.AreEqual(5, obj.Count);
      Assert.AreEqual(0, obj[0]["a"].AsInt32());
      Assert.AreEqual(3, obj[1]["b"].AsInt32());
      CBORTestCommon.AssertRoundTrip(obj);
#endif
    }

#if !NET20 && !NET40
    [Test]
    public void TestReadOnlyCollection() {
      IReadOnlyCollection<int> roc = new ReadOnlyCollection<int>(new int[] {
        0, 1, 99, 2, 3, 99,
      });
      CBORObject cbor;
      _ = CBORObject.NewArray().Add(0).Add(1).Add(99).Add(2).Add(3).Add(99);
      cbor = CBORObject.FromObject(roc);
      roc = cbor.ToObject<ReadOnlyCollection<int>>();
      List<int> list;
      list = new List<int>(roc);
      Assert.AreEqual(6, list.Count);
      Assert.AreEqual(0, list[0]);
      Assert.AreEqual(1, list[1]);
      Assert.AreEqual(99, list[2]);
      Assert.AreEqual(2, list[3]);
      Assert.AreEqual(3, list[4]);
      Assert.AreEqual(99, list[5]);
      roc = cbor.ToObject<IReadOnlyCollection<int>>();
      list = new List<int>(roc);
      Assert.AreEqual(6, list.Count);
      Assert.AreEqual(0, list[0]);
      Assert.AreEqual(1, list[1]);
      Assert.AreEqual(99, list[2]);
      Assert.AreEqual(2, list[3]);
      Assert.AreEqual(3, list[4]);
      Assert.AreEqual(99, list[5]);
      roc = cbor.ToObject<IReadOnlyList<int>>();
      list = new List<int>(roc);
      Assert.AreEqual(6, list.Count);
      Assert.AreEqual(0, list[0]);
      Assert.AreEqual(1, list[1]);
      Assert.AreEqual(99, list[2]);
      Assert.AreEqual(2, list[3]);
      Assert.AreEqual(3, list[4]);
      Assert.AreEqual(99, list[5]);
      try {
        _ = cbor.ToObject<ReadOnlyDictionary<int, int>>();
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = cbor.ToObject<IReadOnlyDictionary<int, int>>();
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // TODO: In next major version, change to CBORException rather than
      // InvalidOperationException (due to AsString)
      try {
        _ = cbor.ToObject<ReadOnlyCollection<string>>();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = cbor.ToObject<IReadOnlyList<string>>();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = cbor.ToObject<IReadOnlyCollection<string>>();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestReadOnlyDictionary() {
      var dict = new Dictionary<string, int>
      {
        ["a"] = 1,
        ["b"] = 2,
        ["c"] = 3,
      };
      IReadOnlyDictionary<string, int> roc = new
ReadOnlyDictionary<string, int>(dict);
      CBORObject cbor;
      CBORObject
expected = CBORObject.NewMap().Add("a", 1).Add("b", 2).Add("c", 3);
      cbor = CBORObject.FromObject(roc);
      Assert.AreEqual(expected, cbor);
      roc = cbor.ToObject<ReadOnlyDictionary<string, int>>();
      Assert.AreEqual(3, roc.Count);
      Assert.AreEqual(1, roc["a"]);
      Assert.AreEqual(2, roc["b"]);
      Assert.AreEqual(3, roc["c"]);
      roc = cbor.ToObject<IReadOnlyDictionary<string, int>>();
      Assert.AreEqual(3, roc.Count);
      Assert.AreEqual(1, roc["a"]);
      Assert.AreEqual(2, roc["b"]);
      Assert.AreEqual(3, roc["c"]);
      // TODO: In next major version, change to CBORException rather than
      // InvalidOperationException
      try {
        _ = cbor.ToObject<ReadOnlyDictionary<int, int>>();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = cbor.ToObject<IReadOnlyDictionary<int, int>>();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
#endif

    [Test]
    public void TestMultidimArray() {
      int[,] arr = { { 0, 1, 99 }, { 2, 3, 299 } };
      var cbor = CBORObject.FromObject(arr);
      Assert.AreEqual(0, cbor[0][0].AsInt32());
      Assert.AreEqual(1, cbor[0][1].AsInt32());
      Assert.AreEqual(99, cbor[0][2].AsInt32());
      Assert.AreEqual(2, cbor[1][0].AsInt32());
      Assert.AreEqual(3, cbor[1][1].AsInt32());
      Assert.AreEqual(299, cbor[1][2].AsInt32());
      object arr2 = cbor.ToObject(typeof(int[,]));
      Assert.AreEqual(arr, arr2);
      int[,,] arr3 = new int[2, 2, 2];
      arr3[0, 0, 0] = 0;
      arr3[0, 0, 1] = 1;
      arr3[0, 1, 0] = 99;
      arr3[0, 1, 1] = 100;
      arr3[1, 0, 0] = 2;
      arr3[1, 0, 1] = 3;
      arr3[1, 1, 0] = 299;
      arr3[1, 1, 1] = 300;
      cbor = CBORObject.FromObject(arr3);
      Assert.AreEqual(0, cbor[0][0][0].AsInt32());
      Assert.AreEqual(1, cbor[0][0][1].AsInt32());
      Assert.AreEqual(99, cbor[0][1][0].AsInt32());
      Assert.AreEqual(100, cbor[0][1][1].AsInt32());
      Assert.AreEqual(2, cbor[1][0][0].AsInt32());
      Assert.AreEqual(299, cbor[1][1][0].AsInt32());
      object arr4 = cbor.ToObject(typeof(int[,,]));
      Assert.AreEqual(arr3, arr4);
    }

    [Test]
    public void TestFloatCloseToEdge() {
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  2.147483647E9d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.147483647E9d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.147483647E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.147483647E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836470000002E9d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836470000002E9d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836470000002E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836470000002E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836469999998E9d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836469999998E9d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836469999998E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836469999998E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  2.147483648E9d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.147483648E9d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.147483648E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.147483648E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836480000005E9d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836480000005E9d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836480000005E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836480000005E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836479999998E9d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836479999998E9d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836479999998E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836479999998E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  2.147483646E9d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.147483646E9d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.147483646E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.147483646E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836460000002E9d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836460000002E9d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836460000002E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836460000002E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836459999998E9d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836459999998E9d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836459999998E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.1474836459999998E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.147483648E9d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.147483648E9d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.147483648E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.147483648E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836479999998E9d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836479999998E9d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836479999998E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836479999998E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836480000005E9d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836480000005E9d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836480000005E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836480000005E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.147483647E9d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.147483647E9d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.147483647E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.147483647E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836469999998E9d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836469999998E9d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836469999998E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836469999998E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836470000002E9d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836470000002E9d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836470000002E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836470000002E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.147483649E9d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.147483649E9d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.147483649E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.147483649E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836489999995E9d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836489999995E9d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836489999995E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836489999995E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836490000005E9d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836490000005E9d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836490000005E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474836490000005E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          9.223372036854776E18d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          9.223372036854776E18d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          9.223372036854776E18d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          9.223372036854776E18d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          9.223372036854778E18d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          9.223372036854778E18d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          9.223372036854778E18d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          9.223372036854778E18d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          9.2233720368547748E18d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          9.2233720368547748E18d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          9.2233720368547748E18d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          9.2233720368547748E18d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -9.223372036854776E18d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -9.223372036854776E18d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -9.223372036854776E18d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -9.223372036854776E18d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -9.2233720368547748E18d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -9.2233720368547748E18d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -9.2233720368547748E18d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -9.2233720368547748E18d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -9.223372036854778E18d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -9.223372036854778E18d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -9.223372036854778E18d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -9.223372036854778E18d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32767.0d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32767.0d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32767.0d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32767.0d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32767.000000000004d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32767.000000000004d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32767.000000000004d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32767.000000000004d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32766.999999999996d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32766.999999999996d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32766.999999999996d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32766.999999999996d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32768.0d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32768.0d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32768.0d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32768.0d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32768.00000000001d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32768.00000000001d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32768.00000000001d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32768.00000000001d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32767.999999999996d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32767.999999999996d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32767.999999999996d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32767.999999999996d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32766.0d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32766.0d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32766.0d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32766.0d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32766.000000000004d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32766.000000000004d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32766.000000000004d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32766.000000000004d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32765.999999999996d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32765.999999999996d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32765.999999999996d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          32765.999999999996d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-32768.0d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-32768.0d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32768.0d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-32768.0d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32767.999999999996d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32767.999999999996d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32767.999999999996d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32767.999999999996d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32768.00000000001d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32768.00000000001d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32768.00000000001d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32768.00000000001d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-32767.0d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-32767.0d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32767.0d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-32767.0d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32766.999999999996d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32766.999999999996d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32766.999999999996d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32766.999999999996d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32767.000000000004d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32767.000000000004d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32767.000000000004d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32767.000000000004d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-32769.0d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-32769.0d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32769.0d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-32769.0d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32768.99999999999d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32768.99999999999d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32768.99999999999d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32768.99999999999d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32769.00000000001d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32769.00000000001d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32769.00000000001d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -32769.00000000001d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(0.0d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(0.0d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(0.0d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(0.0d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(4.9E-324d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(4.9E-324d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(4.9E-324d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(4.9E-324d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-4.9E-324d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-4.9E-324d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-4.9E-324d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-4.9E-324d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(1.0d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(1.0d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(1.0d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(1.0d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          1.0000000000000002d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          1.0000000000000002d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          1.0000000000000002d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          1.0000000000000002d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          0.9999999999999999d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          0.9999999999999999d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          0.9999999999999999d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          0.9999999999999999d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-1.0d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-1.0d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-1.0d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-1.0d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -0.9999999999999999d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -0.9999999999999999d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -0.9999999999999999d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -0.9999999999999999d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -1.0000000000000002d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -1.0000000000000002d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -1.0000000000000002d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -1.0000000000000002d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(255.0d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(255.0d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(255.0d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(255.0d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          255.00000000000003d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          255.00000000000003d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          255.00000000000003d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          255.00000000000003d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          254.99999999999997d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          254.99999999999997d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          254.99999999999997d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          254.99999999999997d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(256.0d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(256.0d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(256.0d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(256.0d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          256.00000000000006d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          256.00000000000006d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          256.00000000000006d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          256.00000000000006d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          255.99999999999997d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          255.99999999999997d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          255.99999999999997d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          255.99999999999997d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(254.0d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(254.0d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(254.0d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(254.0d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          254.00000000000003d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          254.00000000000003d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          254.00000000000003d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          254.00000000000003d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          253.99999999999997d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          253.99999999999997d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          253.99999999999997d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          253.99999999999997d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(2.14748365E9f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  2.14748365E9f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.14748365E9f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  2.14748365E9f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(2.1474839E9f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(2.1474839E9f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  2.1474839E9f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(2.1474839E9f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(2.14748352E9f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  2.14748352E9f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          2.14748352E9f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  2.14748352E9f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  -2.14748365E9f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.14748365E9f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.14748365E9f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.14748365E9f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  -2.14748352E9f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.14748352E9f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.14748352E9f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.14748352E9f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-2.1474839E9f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  -2.1474839E9f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -2.1474839E9f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  -2.1474839E9f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(9.223372E18f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(9.223372E18f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  9.223372E18f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(9.223372E18f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(9.223373E18f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(9.223373E18f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  9.223373E18f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(9.223373E18f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(9.2233715E18f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  9.2233715E18f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          9.2233715E18f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  9.2233715E18f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-9.223372E18f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  -9.223372E18f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -9.223372E18f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  -9.223372E18f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  -9.2233715E18f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -9.2233715E18f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -9.2233715E18f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -9.2233715E18f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-9.223373E18f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  -9.223373E18f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -9.223373E18f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  -9.223373E18f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32767.0f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32767.0f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32767.0f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32767.0f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32767.002f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(32767.002f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(32767.002f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(32767.002f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32766.998f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(32766.998f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(32766.998f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(32766.998f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32768.0f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32768.0f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32768.0f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32768.0f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32768.004f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(32768.004f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(32768.004f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(32768.004f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32767.998f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(32767.998f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(32767.998f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(32767.998f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32766.0f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32766.0f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32766.0f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32766.0f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32766.002f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(32766.002f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(32766.002f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(32766.002f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(32765.998f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(32765.998f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(32765.998f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(32765.998f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-32768.0f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-32768.0f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32768.0f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-32768.0f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32767.998f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32767.998f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32767.998f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32767.998f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32768.004f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32768.004f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32768.004f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32768.004f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-32767.0f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-32767.0f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32767.0f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-32767.0f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32766.998f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32766.998f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32766.998f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32766.998f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32767.002f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32767.002f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32767.002f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32767.002f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-32769.0f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-32769.0f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32769.0f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-32769.0f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32768.996f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32768.996f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32768.996f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32768.996f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32769.004f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32769.004f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32769.004f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-32769.004f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(0.0f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(0.0f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(0.0f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(0.0f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(1.4E-45f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(1.4E-45f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(1.4E-45f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(1.4E-45f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-1.4E-45f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-1.4E-45f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-1.4E-45f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-1.4E-45f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(1.0f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(1.0f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(1.0f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(1.0f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(1.0000001f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(1.0000001f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(1.0000001f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(1.0000001f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(0.99999994f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(0.99999994f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(0.99999994f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(0.99999994f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-1.0f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-1.0f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-1.0f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-1.0f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-0.99999994f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-0.99999994f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  -0.99999994f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-0.99999994f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-1.0000001f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-1.0000001f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-1.0000001f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-1.0000001f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(255.0f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(255.0f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(255.0f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(255.0f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(255.00002f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(255.00002f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(255.00002f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(255.00002f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(254.99998f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(254.99998f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(254.99998f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(254.99998f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(256.0f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(256.0f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(256.0f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(256.0f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(256.00003f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(256.00003f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(256.00003f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(256.00003f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(255.99998f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(255.99998f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(255.99998f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(255.99998f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(254.0f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(254.0f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(254.0f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(254.0f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(254.00002f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(254.00002f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(254.00002f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(254.00002f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(253.99998f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(253.99998f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(253.99998f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(253.99998f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65535.0d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65535.0d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65535.0d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65535.0d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65535.00000000001d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65535.00000000001d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65535.00000000001d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65535.00000000001d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65534.99999999999d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65534.99999999999d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65534.99999999999d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65534.99999999999d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65536.0d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65536.0d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65536.0d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65536.0d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65536.00000000001d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65536.00000000001d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65536.00000000001d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65536.00000000001d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65535.99999999999d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65535.99999999999d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65535.99999999999d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65535.99999999999d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65534.0d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65534.0d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65534.0d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65534.0d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65534.00000000001d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65534.00000000001d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65534.00000000001d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65534.00000000001d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65533.99999999999d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65533.99999999999d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65533.99999999999d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          65533.99999999999d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  4.294967295E9d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.294967295E9d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.294967295E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.294967295E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.2949672950000005E9d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.2949672950000005E9d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.2949672950000005E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.2949672950000005E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.2949672949999995E9d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.2949672949999995E9d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.2949672949999995E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.2949672949999995E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  4.294967296E9d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.294967296E9d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.294967296E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.294967296E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.294967296000001E9d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.294967296000001E9d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.294967296000001E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.294967296000001E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.2949672959999995E9d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.2949672959999995E9d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.2949672959999995E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.2949672959999995E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  4.294967294E9d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.294967294E9d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.294967294E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.294967294E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.2949672940000005E9d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.2949672940000005E9d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.2949672940000005E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.2949672940000005E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.2949672939999995E9d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.2949672939999995E9d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.2949672939999995E9d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.2949672939999995E9d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          1.8446744073709552E19d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          1.8446744073709552E19d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          1.8446744073709552E19d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          1.8446744073709552E19d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          1.8446744073709556E19d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          1.8446744073709556E19d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          1.8446744073709556E19d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          1.8446744073709556E19d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          1.844674407370955E19d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          1.844674407370955E19d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          1.844674407370955E19d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          1.844674407370955E19d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-128.0d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-128.0d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-128.0d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-128.0d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -127.99999999999999d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -127.99999999999999d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -127.99999999999999d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -127.99999999999999d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -128.00000000000003d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -128.00000000000003d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -128.00000000000003d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -128.00000000000003d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-127.0d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-127.0d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-127.0d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-127.0d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -126.99999999999999d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -126.99999999999999d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -126.99999999999999d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -126.99999999999999d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -127.00000000000001d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -127.00000000000001d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -127.00000000000001d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -127.00000000000001d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-129.0d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-129.0d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-129.0d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-129.0d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -128.99999999999997d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -128.99999999999997d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -128.99999999999997d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -128.99999999999997d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -129.00000000000003d).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -129.00000000000003d).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -129.00000000000003d).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          -129.00000000000003d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(127.0d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(127.0d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(127.0d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(127.0d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          127.00000000000001d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          127.00000000000001d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          127.00000000000001d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          127.00000000000001d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          126.99999999999999d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          126.99999999999999d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          126.99999999999999d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          126.99999999999999d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(128.0d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(128.0d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(128.0d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(128.0d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          128.00000000000003d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          128.00000000000003d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          128.00000000000003d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          128.00000000000003d).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          127.99999999999999d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          127.99999999999999d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          127.99999999999999d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          127.99999999999999d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(126.0d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(126.0d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(126.0d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(126.0d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          126.00000000000001d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          126.00000000000001d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          126.00000000000001d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          126.00000000000001d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          125.99999999999999d).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          125.99999999999999d).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          125.99999999999999d).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          125.99999999999999d).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65535.0f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65535.0f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65535.0f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65535.0f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65535.004f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(65535.004f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(65535.004f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(65535.004f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65534.996f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(65534.996f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(65534.996f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(65534.996f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65536.0f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65536.0f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65536.0f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65536.0f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65536.01f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65536.01f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(65536.01f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65536.01f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65535.996f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(65535.996f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(65535.996f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(65535.996f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65534.0f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65534.0f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65534.0f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65534.0f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65534.004f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(65534.004f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(65534.004f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(65534.004f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(65533.996f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(65533.996f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(65533.996f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(65533.996f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(4.2949673E9f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(4.2949673E9f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  4.2949673E9f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(4.2949673E9f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(4.2949678E9f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(4.2949678E9f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  4.2949678E9f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(4.2949678E9f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(4.29496704E9f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  4.29496704E9f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          4.29496704E9f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  4.29496704E9f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(1.8446744E19f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  1.8446744E19f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          1.8446744E19f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  1.8446744E19f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(1.8446746E19f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  1.8446746E19f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          1.8446746E19f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  1.8446746E19f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(1.8446743E19f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  1.8446743E19f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
          1.8446743E19f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
  1.8446743E19f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-128.0f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-128.0f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-128.0f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-128.0f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-127.99999f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-127.99999f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-127.99999f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-127.99999f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-128.00002f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-128.00002f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-128.00002f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-128.00002f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-127.0f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-127.0f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-127.0f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-127.0f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-126.99999f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-126.99999f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-126.99999f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-126.99999f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-127.00001f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-127.00001f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-127.00001f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-127.00001f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-129.0f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-129.0f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-129.0f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(-129.0f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-128.99998f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-128.99998f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-128.99998f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-128.99998f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-129.00002f).ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-129.00002f).ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-129.00002f).ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(-129.00002f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(127.0f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(127.0f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(127.0f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(127.0f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(127.00001f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(127.00001f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(127.00001f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(127.00001f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(126.99999f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(126.99999f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(126.99999f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(126.99999f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(128.0f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(128.0f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(128.0f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(128.0f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(128.00002f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(128.00002f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(128.00002f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(128.00002f).ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(127.99999f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(127.99999f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(127.99999f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(127.99999f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(126.0f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(126.0f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(126.0f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(126.0f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(126.00001f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(126.00001f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(126.00001f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(126.00001f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(125.99999f).ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(125.99999f).ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(125.99999f).ToObject<ushort>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ =
ToObjectTest.TestToFromObjectRoundTrip(125.99999f).ToObject<sbyte>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
    }

    [Test]
    public void TestULong() {
      ulong[] ranges = {
        0, 65539, 0xfffff000UL, 0x100000400UL,
        0x7ffffffffffff000UL, 0x8000000000000400UL,
        UInt64.MaxValue - 1000, UInt64.MaxValue,
      };
      for (int i = 0; i < ranges.Length; i += 2) {
        ulong j = ranges[i];
        while (true) {
          CBORTestCommon.AssertJSONSer(
            ToObjectTest.TestToFromObjectRoundTrip(j),
            ((PeterO.Numbers.EDecimal)j).ToString());
          if (j == ranges[i + 1]) {
            break;
          }
          ++j;
        }
      }
    }

    private static short Divide32By16(
      int dividendLow,
      short divisor,
      bool returnRemainder) {
      int t;
      var dividendHigh = 0;
      int intDivisor = divisor & 0xffff;
      for (int i = 0; i < 32; ++i) {
        t = dividendHigh >> 31;
        dividendHigh <<= 1;
        dividendHigh = unchecked(dividendHigh | ((dividendLow >>
                    31) & 1));
        dividendLow <<= 1;
        t |= dividendHigh;
        // unsigned greater-than-or-equal check
        if (((t >> 31) != 0) || (t >= intDivisor)) {
          unchecked {
            dividendHigh -= intDivisor;
            ++dividendLow;
          }
        }
      }
      return returnRemainder ? unchecked((short)(dividendHigh &
            0xffff)) : unchecked((short)(dividendLow & 0xffff));
    }

    private static short DivideUnsigned(int x, short y) {
      unchecked {
        int iy = y & 0xffff;
        if ((x >> 31) == 0) {
          // x is already nonnegative
          return (short)((x / iy) & 0xffff);
        }
        return Divide32By16(x, y, false);
      }
    }

    [Test]
    public void TestOther() {
      int[,,] arr3 = new int[2, 3, 2];
      var cbor = CBORObject.FromObject(arr3);
      string stringTemp = cbor.ToJSONString();
      string str145009 = "[[[0,0],[0,0],[0,0]],[[0,0],[0,0],[0,0]]]";
      Assert.AreEqual(
        str145009,
        stringTemp);
      CBORTestCommon.AssertRoundTrip(cbor);
    }

    [Test]
    public void TestDivideUnsigned() {
      var fr = new RandomGenerator();
      unchecked {
        for (int i = 0; i < 1000; ++i) {
          var x = (uint)fr.UniformInt(0x10000);
          x |= ((uint)fr.UniformInt(0x10000)) << 16;
          var y = (ushort)fr.UniformInt(0x10000);
          var dx = (int)x;
          var dy = (short)y;
          if (dy == 0) {
            continue;
          }
          var expected = (short)(x / y);
          short actual = DivideUnsigned(dx, dy);
          if (expected != actual) {
            Assert.AreEqual(expected, actual, "Dividing " + x + " by " + y);
          }
        }
      }
    }

    [Test]
    public void TestUInt() {
      uint[] ranges = {
        0, 65539,
        0x7ffff000U, 0x80000400U, UInt32.MaxValue - 1000, UInt32.MaxValue,
      };
      for (int i = 0; i < ranges.Length; i += 2) {
        uint j = ranges[i];
        while (true) {
          CBORTestCommon.AssertJSONSer(
            ToObjectTest.TestToFromObjectRoundTrip(j),
            TestCommon.LongToString(j));
          if (j == ranges[i + 1]) {
            break;
          }
          ++j;
        }
      }
    }

    [Test]
    public void TestDecimal() {
      CBORObject cbor = ToObjectTest.TestToFromObjectRoundTrip(
          decimal.MinValue);
      Assert.IsTrue(cbor.IsNumber, cbor.ToString());
      CBORTestCommon.AssertJSONSer(
        ToObjectTest.TestToFromObjectRoundTrip(decimal.MinValue),
        ((EDecimal)decimal.MinValue).ToString());
      CBORTestCommon.AssertJSONSer(
        ToObjectTest.TestToFromObjectRoundTrip(decimal.MaxValue),
        ((EDecimal)decimal.MaxValue).ToString());
      for (int i = -100; i <= 100; ++i) {
        CBORTestCommon.AssertJSONSer(
          ToObjectTest.TestToFromObjectRoundTrip((decimal)i),
          TestCommon.IntToString(i));
        {
          CBORObject objectTemp = ToObjectTest.TestToFromObjectRoundTrip(i +
              0.1m);
          string objectTemp2 = ((EDecimal)(i + 0.1m)).ToString();
          CBORTestCommon.AssertJSONSer(objectTemp, objectTemp2);
        }
        CBORTestCommon.AssertJSONSer(
          ToObjectTest.TestToFromObjectRoundTrip(i + 0.1111m),
          ((EDecimal)(i + 0.1111m)).ToString());
      }
    }

    [Test]
    public void TestUShort() {
      for (int i = ushort.MinValue; i <= ushort.MaxValue; ++i) {
        CBORTestCommon.AssertJSONSer(
          ToObjectTest.TestToFromObjectRoundTrip((ushort)i),
          TestCommon.LongToString(i));
      }
    }

    private readonly struct ExoticStruct
    {
      public readonly int Pvalue;

      public ExoticStruct(int pv) {
        this.Pvalue = pv;
      }
    }

    [Test]
    public void TestToObjectNull() {
      CBORObject cbor = CBORObject.Null;
      Assert.AreEqual(null, (string)cbor.ToObject(typeof(string)));
      Assert.AreEqual(null, cbor.ToObject<string>());
    }

    [Test]
    public void TestNullable() {
      int? nvalue = 1;
      var cbor = CBORObject.FromObject((object)nvalue);
      Assert.AreEqual(CBORObject.FromInt32(1), cbor);
      nvalue = null;
      _ = CBORObject.FromObject((object)nvalue);
      uint? unvalue = 1u;
      cbor = CBORObject.FromObject((object)unvalue);
      Assert.AreEqual(CBORObject.FromInt32(1), cbor);
      unvalue = null;
      cbor = CBORObject.FromObject((object)unvalue);
      Assert.AreEqual(CBORObject.Null, cbor);
      Assert.AreEqual(null, CBORObject.Null.ToObject<int?>());
      Assert.AreEqual(1, CBORObject.FromInt32(1).ToObject<int?>());
      Assert.AreEqual(null, CBORObject.Null.ToObject<uint?>());
      Assert.AreEqual(1u, CBORObject.FromInt32(1).ToObject<uint?>());
      Assert.AreEqual(null, CBORObject.Null.ToObject<double?>());
      if (CBORObject.FromDouble(3.5).ToObject<double?>() != 3.5) {
        Assert.Fail();
      }
      ExoticStruct? es = null;
      cbor = CBORObject.FromObject(es);
      Assert.AreEqual(CBORObject.Null, cbor);
    }

    [Test]
    public void TestDoubleToOther() {
      CBORObject dbl1 =
        ToObjectTest.TestToFromObjectRoundTrip((double)Int32.MinValue);
      CBORObject dbl2 =
        ToObjectTest.TestToFromObjectRoundTrip((double)Int32.MaxValue);
      try {
        _ = dbl1.ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = dbl1.ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = dbl1.ToObject<uint>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = dbl1.ToObject<ulong>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = dbl2.ToObject<ushort>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = dbl2.ToObject<sbyte>();
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = dbl2.ToObject<uint>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        _ = dbl2.ToObject<ulong>();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
    }
  }
}
