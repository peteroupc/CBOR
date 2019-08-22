using System;
using System.Collections.Generic;
using System.IO;

namespace PeterO.Cbor {
  internal static class CBORCanonical {
    internal static readonly IComparer<CBORObject> Comparer =
      new CtapComparer();

    private sealed class CtapComparer : IComparer<CBORObject> {
      public int Compare(CBORObject a, CBORObject b) {
        if (a == null) {
          return b == null ? 0 : -1;
        }
        if (b == null) {
          return 1;
        }
        byte[] abs;
        byte[] bbs;
        var bothBytes = false;
        if (a.Type == CBORType.ByteString && b.Type == CBORType.ByteString) {
          abs = a.GetByteString();
          bbs = b.GetByteString();
          bothBytes = true;
        } else {
          abs = CtapCanonicalEncode(a);
          bbs = CtapCanonicalEncode(b);
        }
        if (!bothBytes && (abs[0] & 0xe0) != (bbs[0] & 0xe0)) {
          // different major types
          return (abs[0] & 0xe0) < (bbs[0] & 0xe0) ? -1 : 1;
        }
        if (abs.Length != bbs.Length) {
          // different lengths
          return abs.Length < bbs.Length ? -1 : 1;
        }
        for (var i = 0; i < abs.Length; ++i) {
          if (abs[i] != bbs[i]) {
            int ai = ((int)abs[i]) & 0xff;
            int bi = ((int)bbs[i]) & 0xff;
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

    private static byte[] CtapCanonicalEncode(CBORObject a, int depth) {
      CBORObject cbor = a.Untag();
      CBORType valueAType = cbor.Type;
      try {
        if (valueAType == CBORType.Array) {
          using (var ms = new MemoryStream()) {
            CBORObject.WriteValue(ms, 4, cbor.Count);
            for (var i = 0; i < cbor.Count; ++i) {
              if (depth >= 3 && IsArrayOrMap(cbor[i])) {
                throw new CBORException("Nesting level too deep");
              }
              byte[] bytes = CtapCanonicalEncode(cbor[i], depth + 1);
              ms.Write(bytes, 0, bytes.Length);
            }
            return ms.ToArray();
          }
        } else if (valueAType == CBORType.Map) {
          var sortedKeys = new List<CBORObject>();
          foreach (CBORObject key in cbor.Keys) {
            if (depth >= 3 && (IsArrayOrMap(key) ||
               IsArrayOrMap(cbor[key]))) {
              throw new CBORException("Nesting level too deep");
            }
            sortedKeys.Add(key);
          }
          sortedKeys.Sort(Comparer);
          using (var ms = new MemoryStream()) {
            CBORObject.WriteValue(ms, 5, cbor.Count);
            foreach (CBORObject key in sortedKeys) {
              byte[] bytes = CtapCanonicalEncode(key, depth + 1);
              ms.Write(bytes, 0, bytes.Length);
              bytes = CtapCanonicalEncode(cbor[key], depth + 1);
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
        using (var ms = new MemoryStream()) {
          CBORObject.WriteFloatingPointBits(ms, bits, 8);
          return ms.ToArray();
        }
      } else if (valueAType == CBORType.Integer) {
        return cbor.EncodeToBytes(CBOREncodeOptions.Default);
      } else {
        throw new ArgumentException("Invalid CBOR type.");
      }
    }
  }
}
