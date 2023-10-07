using System;
using System.Collections.Generic;
using System.IO;

namespace PeterO.Cbor {
  internal static class CBORCanonical {
    internal static readonly IComparer<CBORObject> Comparer =
      new CtapComparer();

    private static readonly IComparer<KeyValuePair<byte[], byte[]>>
    ByteComparer = new CtapByteComparer();

    private sealed class CtapByteComparer : IComparer<KeyValuePair<byte[],
      byte[]>>
    {
      public int Compare(
        KeyValuePair<byte[], byte[]> kva,
        KeyValuePair<byte[], byte[]> kvb) {
        byte[] bytesA = kva.Key;
        byte[] bytesB = kvb.Key;
        if (bytesA == null) {
          return bytesB == null ? 0 : -1;
        }
        if (bytesB == null) {
          return 1;
        }
        if (bytesA.Length == 0) {
          return bytesB.Length == 0 ? 0 : -1;
        }
        if (bytesB.Length == 0) {
          return 1;
        }
        if (bytesA == bytesB) {
          // NOTE: Assumes reference equality of CBORObjects
          return 0;
        }
        // check major types
        if ((bytesA[0] & 0xe0) != (bytesB[0] & 0xe0)) {
          return (bytesA[0] & 0xe0) < (bytesB[0] & 0xe0) ? -1 : 1;
        }
        // check lengths
        if (bytesA.Length != bytesB.Length) {
          return bytesA.Length < bytesB.Length ? -1 : 1;
        }
        // check bytes
        for (int i = 0; i < bytesA.Length; ++i) {
          if (bytesA[i] != bytesB[i]) {
            int ai = bytesA[i] & 0xff;
            int bi = bytesB[i] & 0xff;
            return (ai < bi) ? -1 : 1;
          }
        }
        return 0;
      }
    }

    private sealed class CtapComparer : IComparer<CBORObject>
    {
      private static int MajorType(CBORObject a) {
        if (a.IsTagged) {
          return 6;
        }
        switch (a.Type) {
          case CBORType.Integer:
            return a.AsNumber().IsNegative() ? 1 : 0;
          case CBORType.SimpleValue:
          case CBORType.Boolean:
          case CBORType.FloatingPoint:
            return 7;
          case CBORType.ByteString:
            return 2;
          case CBORType.TextString:
            return 3;
          case CBORType.Array:
            return 4;
          case CBORType.Map:
            return 5;
          default: throw new InvalidOperationException();
        }
      }

      public int Compare(CBORObject a, CBORObject b) {
        if (a == null) {
          return b == null ? 0 : -1;
        }
        if (b == null) {
          return 1;
        }
        if (a == b) {
          // NOTE: Assumes reference equality of CBORObjects
          return 0;
        }
        a = a.Untag();
        b = b.Untag();
        byte[] abs;
        byte[] bbs;
        int amt = MajorType(a);
        int bmt = MajorType(b);
        if (amt != bmt) {
          return amt < bmt ? -1 : 1;
        }
        // DebugUtility.Log("a="+a);
        // DebugUtility.Log("b="+b);
        if (amt == 2) {
          // Both objects are byte strings
          abs = a.GetByteString();
          bbs = b.GetByteString();
        } else {
          // Might store arrays or maps, where
          // canonical encoding can fail due to too-deep
          // nesting
          abs = CtapCanonicalEncode(a);
          bbs = CtapCanonicalEncode(b);
        }
        if (abs.Length != bbs.Length) {
          // different lengths
          return abs.Length < bbs.Length ? -1 : 1;
        }
        for (int i = 0; i < abs.Length; ++i) {
          if (abs[i] != bbs[i]) {
            int ai = abs[i] & 0xff;
            int bi = bbs[i] & 0xff;
            return (ai < bi) ? -1 : 1;
          }
        }
        return 0;
      }
    }

    private static bool IsArrayOrMap(CBORObject a) {
      return a.Type == CBORType.Array || a.Type == CBORType.Map;
    }

    public static byte[] CtapCanonicalEncode(CBORObject a) {
      return CtapCanonicalEncode(a, 0);
    }

    private static bool ByteArraysEqual(byte[] bytesA, byte[] bytesB) {
      if (bytesA == bytesB) {
        return true;
      }
      if (bytesA == null || bytesB == null) {
        return false;
      }
      if (bytesA.Length == bytesB.Length) {
        for (int j = 0; j < bytesA.Length; ++j) {
          if (bytesA[j] != bytesB[j]) {
            return false;
          }
        }
        return true;
      }
      return false;
    }

    private static void CheckDepth(CBORObject cbor, int depth) {
      if (cbor.Type == CBORType.Array) {
        for (int i = 0; i < cbor.Count; ++i) {
          if (depth >= 3 && IsArrayOrMap(cbor[i])) {
            throw new CBORException("Nesting level too deep");
          }
          CheckDepth(cbor[i], depth + 1);
        }
      } else if (cbor.Type == CBORType.Map) {
        foreach (CBORObject key in cbor.Keys) {
          if (depth >= 3 && (IsArrayOrMap(key) || IsArrayOrMap(cbor[key]))) {
            throw new CBORException("Nesting level too deep");
          }
          CheckDepth(key, depth + 1);
          CheckDepth(cbor[key], depth + 1);
        }
      }
    }

    private static byte[] CtapCanonicalEncode(CBORObject a, int depth) {
      CBORObject cbor = a.Untag();
      CBORType valueAType = cbor.Type;
      try {
        if (valueAType == CBORType.Array) {
          using (var ms = new MemoryStream()) {
            _ = CBORObject.WriteValue(ms, 4, cbor.Count);
            for (int i = 0; i < cbor.Count; ++i) {
              if (depth >= 3 && IsArrayOrMap(cbor[i])) {
                throw new CBORException("Nesting level too deep");
              }
              byte[] bytes = CtapCanonicalEncode(cbor[i], depth + 1);
              ms.Write(bytes, 0, bytes.Length);
            }
            return ms.ToArray();
          }
        } else if (valueAType == CBORType.Map) {
          KeyValuePair<byte[], byte[]> kv1;
          List<KeyValuePair<byte[], byte[]>> sortedKeys;
          sortedKeys = new List<KeyValuePair<byte[], byte[]>>();
          foreach (CBORObject key in cbor.Keys) {
            if (depth >= 3 && (IsArrayOrMap(key) || IsArrayOrMap(cbor[key]))) {
              throw new CBORException("Nesting level too deep");
            }
            CheckDepth(key, depth + 1);
            CheckDepth(cbor[key], depth + 1);
            // Check if key and value can be canonically encoded
            // (will throw an exception if they cannot)
            kv1 = new KeyValuePair<byte[], byte[]>(
              CtapCanonicalEncode(key, depth + 1),
              CtapCanonicalEncode(cbor[key], depth + 1));
            sortedKeys.Add(kv1);
          }
          sortedKeys.Sort(ByteComparer);
          using (var ms = new MemoryStream()) {
            _ = CBORObject.WriteValue(ms, 5, cbor.Count);
            byte[] lastKey = null;
            for (int i = 0; i < sortedKeys.Count; ++i) {
              kv1 = sortedKeys[i];
              byte[] bytes = kv1.Key;
              if (lastKey != null && ByteArraysEqual(bytes, lastKey)) {
                throw new CBORException("duplicate canonical CBOR key");
              }
              lastKey = bytes;
              ms.Write(bytes, 0, bytes.Length);
              bytes = kv1.Value;
              ms.Write(bytes, 0, bytes.Length);
            }
            return ms.ToArray();
          }
        }
      } catch (IOException ex) {
        throw new InvalidOperationException(ex.ToString(), ex);
      }
      if (valueAType == CBORType.SimpleValue ||
        valueAType == CBORType.Boolean || valueAType == CBORType.ByteString ||
        valueAType == CBORType.TextString) {
        return cbor.EncodeToBytes(CBOREncodeOptions.Default);
      } else if (valueAType == CBORType.FloatingPoint) {
        long bits = cbor.AsDoubleBits();
        return new byte[] {
          0xfb,
          (byte)((bits >> 56) & 0xffL),
          (byte)((bits >> 48) & 0xffL),
          (byte)((bits >> 40) & 0xffL),
          (byte)((bits >> 32) & 0xffL),
          (byte)((bits >> 24) & 0xffL),
          (byte)((bits >> 16) & 0xffL),
          (byte)((bits >> 8) & 0xffL),
          (byte)(bits & 0xffL),
        };
      } else {
        return valueAType == CBORType.Integer ?
          cbor.EncodeToBytes(CBOREncodeOptions.Default) :
          throw new ArgumentException("Invalid CBOR type.");
      }
    }
  }
}
