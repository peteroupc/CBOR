/*
Written in 2013 by Peter Occil.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
*/
using System;
using System.Collections.Generic;
using PeterO.Cbor;

namespace Test {
  public static class JSONPatch {
    private static CBORObject AddOperation(
      CBORObject o,
      string valueOpStr,
      string path,
      CBORObject value) {
      if (path == null) {
        throw new ArgumentException("Patch " + valueOpStr);
      }
      if (path.Length == 0) {
        o = value;
      } else {
        JSONPointer pointer = JSONPointer.FromPointer(o, path);
        if (pointer.GetParent().Type == CBORType.Array) {
          int index = pointer.GetIndex();
          if (index < 0) {
            throw new ArgumentException("Patch " + valueOpStr + " path");
          }
          ((CBORObject)pointer.GetParent()).Insert(index, value);
        } else if (pointer.GetParent().Type == CBORType.Map) {
          string key = pointer.GetKey();
          ((CBORObject)pointer.GetParent()).Set(key, value);
        } else {
          throw new ArgumentException("Patch " + valueOpStr + " path");
        }
      }
      return o;
    }

    private static CBORObject CloneCbor(CBORObject o) {
      return CBORObject.FromJSONString(o.ToJSONString());
    }

    private static string GetString(CBORObject o, string key) {
      return o.ContainsKey(key) ? o[key].AsString() : null;
    }

    public static CBORObject Patch(CBORObject o, CBORObject ptch) {
      // clone the object in case of failure
      if (o == null) {
        throw new ArgumentNullException(nameof(o));
      }
      o = CloneCbor(o);
      if (ptch == null) {
        throw new ArgumentNullException(nameof(ptch));
      }
      for (int i = 0; i < ptch.Count; ++i) {
        CBORObject patchOp = ptch[i];
        // NOTE: This algorithm requires "op" to exist
        // only once; the CBORObject, however, does not
        // allow duplicates
        string valueOpStr = GetString(patchOp, "op");
        if (valueOpStr == null) {
          throw new ArgumentException("Patch");
        }
        if ("add".Equals(valueOpStr, StringComparison.Ordinal)) {
          // operation
          CBORObject value = null;
          if (!patchOp.ContainsKey("value")) {
            throw new ArgumentException("Patch " + valueOpStr + " value");
          }
          value = patchOp["value"];
          o = AddOperation(o, valueOpStr, GetString(patchOp, "path"), value);
        } else if ("replace".Equals(valueOpStr, StringComparison.Ordinal)) {
          // operation
          CBORObject value = null;
          if (!patchOp.ContainsKey("value")) {
            throw new ArgumentException("Patch " + valueOpStr + " value");
          }
          value = patchOp["value"];
          o = ReplaceOperation(
              o,
              valueOpStr,
              GetString(patchOp, "path"),
              value);
        } else if ("remove".Equals(valueOpStr, StringComparison.Ordinal)) {
          // Remove operation
          string path = patchOp["path"].AsString();
          if (path == null) {
            throw new ArgumentException("Patch " + valueOpStr + " path");
          }
          if (path.Length == 0) {
            o = null;
          } else {
            RemoveOperation(o, valueOpStr, GetString(patchOp, "path"));
          }
        } else if ("move".Equals(valueOpStr, StringComparison.Ordinal)) {
          string path = patchOp["path"].AsString();
          if (path == null) {
            throw new ArgumentException("Patch " + valueOpStr + " path");
          }
          string fromPath = patchOp["from"].AsString();
          if (fromPath == null) {
            throw new ArgumentException("Patch " + valueOpStr + " from");
          }
          if (path.StartsWith(fromPath, StringComparison.Ordinal)) {
            throw new ArgumentException("Patch " + valueOpStr);
          }
          CBORObject movedObj = RemoveOperation(o, valueOpStr, fromPath);
          o = AddOperation(o, valueOpStr, path, CloneCbor(movedObj));
        } else if ("copy".Equals(valueOpStr, StringComparison.Ordinal)) {
          string path = patchOp["path"].AsString();
          string fromPath = patchOp["from"].AsString();
          if (path == null) {
            throw new ArgumentException("Patch " + valueOpStr + " path");
          }
          if (fromPath == null) {
            throw new ArgumentException("Patch " + valueOpStr + " from");
          }
          JSONPointer pointer = JSONPointer.FromPointer(o, path);
          if (!pointer.Exists()) {
            throw new KeyNotFoundException("Patch " +
              valueOpStr + " " + fromPath);
          }
          CBORObject copiedObj = pointer.GetValue();
          o = AddOperation(
              o,
              valueOpStr,
              path,
              CloneCbor(copiedObj));
        } else if ("test".Equals(valueOpStr, StringComparison.Ordinal)) {
          string path = patchOp["path"].AsString();
          if (path == null) {
            throw new ArgumentException("Patch " + valueOpStr + " path");
          }
          CBORObject value = null;
          if (!patchOp.ContainsKey("value")) {
            throw new ArgumentException("Patch " + valueOpStr + " value");
          }
          value = patchOp["value"];
          JSONPointer pointer = JSONPointer.FromPointer(o, path);
          if (!pointer.Exists()) {
            throw new ArgumentException("Patch " +
              valueOpStr + " " + path);
          }
          Object testedObj = pointer.GetValue();
          if ((testedObj == null) ? (value != null) :
            !testedObj.Equals(value)) {
            throw new InvalidOperationException("Patch " + valueOpStr);
          }
        }
      }
      return (o == null) ? CBORObject.Null : o;
    }

    private static CBORObject RemoveOperation(
      CBORObject o,
      string valueOpStr,
      string path) {
      if (path == null) {
        throw new ArgumentException("Patch " + valueOpStr);
      }
      if (path.Length == 0) {
        return o;
      } else {
        JSONPointer pointer = JSONPointer.FromPointer(o, path);
        if (!pointer.Exists()) {
          throw new KeyNotFoundException("Patch " +
            valueOpStr + " " + path);
        }
        o = pointer.GetValue();
        if (pointer.GetParent().Type == CBORType.Array) {
          ((CBORObject)pointer.GetParent()).RemoveAt(pointer.GetIndex());
        } else if (pointer.GetParent().Type == CBORType.Map) {
((CBORObject)pointer.GetParent()).Remove(
            CBORObject.FromObject(pointer.GetKey()));
        }
        return o;
      }
    }

    private static CBORObject ReplaceOperation(
      CBORObject o,
      string valueOpStr,
      string path,
      CBORObject value) {
      if (path == null) {
        throw new ArgumentException("Patch " + valueOpStr);
      }
      if (path.Length == 0) {
        o = value;
      } else {
        JSONPointer pointer = JSONPointer.FromPointer(o, path);
        if (!pointer.Exists()) {
          throw new KeyNotFoundException("Patch " +
            valueOpStr + " " + path);
        }
        if (pointer.GetParent().Type == CBORType.Array) {
          int index = pointer.GetIndex();
          if (index < 0) {
            throw new ArgumentException("Patch " + valueOpStr + " path");
          }
          ((CBORObject)pointer.GetParent()).Set(index, value);
        } else if (pointer.GetParent().Type == CBORType.Map) {
          string key = pointer.GetKey();
          ((CBORObject)pointer.GetParent()).Set(key, value);
        } else {
          throw new ArgumentException("Patch " + valueOpStr + " path");
        }
      }
      return o;
    }
  }
}
