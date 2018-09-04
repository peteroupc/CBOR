using System;
using System.Collections.Generic;
using System.IO;
namespace PeterO.Cbor {
  internal static class CBORCanonical {

    private sealed class CtapComparer : IComparer<CBORObject> {
      public int Compare(CBORObject a, CBORObject b) {
        byte[] abs;
        byte[] bbs;
        if (a.Type == CBORType.ByteString && b.Type == CBORType.ByteString) {
          abs = a.GetByteString();
          bbs = b.GetByteString();
        } else {
          abs = CtapCanonicalEncode(a);
          bbs = CtapCanonicalEncode(b);
        }
        if (abs.Length != bbs.Length) return abs.Length < bbs.Length ? -1 : 1;
        for (var i = 0; i < abs.Length; i++) {
          if (abs[i] != bbs[i]) {
            int ai = ((int)abs[i]) & 0xff;
            int bi = ((int)bbs[i]) & 0xff;
            return (ai < bi) ? -1 : 1;
          }
        }
        return 0;
      }
    }

    public static byte[] CtapCanonicalEncode(CBORObject a) {
      CBORObject cbor = a.Untag();
      CBORType aType = cbor.Type;
      try {
        if (aType == CBORType.Array) {
          using (var ms = new MemoryStream()) {
            CBORObject.WriteValue(ms, 4, cbor.Count);
            for (var i = 0; i < cbor.Count; i++) {
              byte[] bytes = CtapCanonicalEncode(cbor[i]);
              ms.Write(bytes, 0, bytes.Length);
            }
            return ms.ToArray();
          }
        } else if (aType == CBORType.Map) {
          var sortedKeys = new List<CBORObject>();
          foreach (CBORObject key in cbor.Keys) {
            sortedKeys.Add(key);
          }
          sortedKeys.Sort(new CtapComparer());
          using (var ms = new MemoryStream()) {
            CBORObject.WriteValue(ms, 5, cbor.Count);
            foreach (CBORObject key in sortedKeys) {
              byte[] bytes = CtapCanonicalEncode(key);
              ms.Write(bytes, 0, bytes.Length);
              bytes = CtapCanonicalEncode(cbor[key]);
              ms.Write(bytes, 0, bytes.Length);
            }
            return ms.ToArray();
          }
        }
      } catch (IOException ex) {
        throw new InvalidOperationException(ex.ToString(), ex);
      }
      if (aType == CBORType.SimpleValue ||
       aType == CBORType.Boolean ||
       aType == CBORType.ByteString ||
       aType == CBORType.TextString) {
        return cbor.EncodeToBytes(CBOREncodeOptions.Default);
      } else if (aType == CBORType.Number) {
        if (cbor.CanFitInInt64()) {
          return cbor.EncodeToBytes(CBOREncodeOptions.Default);
        } else {
          cbor = CBORObject.FromObject(cbor.AsDouble());
          return cbor.EncodeToBytes(CBOREncodeOptions.Default);
        }
      } else {
        throw new ArgumentException("Invalid CBOR type.");
      }
    }
  }
}
