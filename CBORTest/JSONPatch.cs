/*
Written in 2013 by Peter Occil.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
*/
using System;
using System.Collections.Generic;
using PeterO.Cbor;

namespace Test {
  public class JSONPatch {
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
        JSONPointer pointer = JSONPointer.fromPointer(o, path);
        if (pointer.getParent().Type == CBORType.Array) {
          int index = pointer.getIndex();
          if (index < 0) {
            throw new ArgumentException("Patch " + valueOpStr + " path");
          }
          ((CBORObject)pointer.getParent()).Insert(index, value);
        } else if (pointer.getParent().Type == CBORType.Map) {
          string key = pointer.getKey();
          ((CBORObject)pointer.getParent()).Set(key, value);
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

    public static CBORObject Patch(CBORObject o, CBORObject Patch) {
     // clone the object in case of failure
      o = CloneCbor(o);
      for (int i = 0; i < Patch.Count; ++i) {
        CBORObject patchOp = Patch[i];
       // NOTE: This algorithm requires "op" to exist
       // only once; the CBORObject, however, does not
       // allow duplicates
        string valueOpStr = GetString(patchOp, "op");
        if (valueOpStr == null) {
          throw new ArgumentException("Patch");
        }
        if ("add".Equals(valueOpStr)) {
         // operation
          CBORObject value = null;
          if (!patchOp.ContainsKey("value")) {
throw new ArgumentException("Patch " + valueOpStr + " value");
          }
          value = patchOp["value"];
          o = AddOperation(o, valueOpStr, GetString(patchOp, "path"), value);
        } else if ("replace".Equals(valueOpStr)) {
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
        } else if ("remove".Equals(valueOpStr)) {
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
        } else if ("move".Equals(valueOpStr)) {
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
        } else if ("copy".Equals(valueOpStr)) {
          string path = patchOp["path"].AsString();
          string fromPath = patchOp["from"].AsString();
          if (path == null) {
            throw new ArgumentException("Patch " + valueOpStr + " path");
          }
          if (fromPath == null) {
            throw new ArgumentException("Patch " + valueOpStr + " from");
          }
          JSONPointer pointer = JSONPointer.fromPointer(o, path);
          if (!pointer.exists()) {
            throw new KeyNotFoundException("Patch " +
              valueOpStr + " " + fromPath);
          }
          CBORObject copiedObj = pointer.getValue();
          o = AddOperation(
  o,
  valueOpStr,
  path,
  CloneCbor(copiedObj));
        } else if ("test".Equals(valueOpStr)) {
          string path = patchOp["path"].AsString();
          if (path == null) {
            throw new ArgumentException("Patch " + valueOpStr + " path");
          }
          CBORObject value = null;
          if (!patchOp.ContainsKey("value")) {
throw new ArgumentException("Patch " + valueOpStr + " value");
          }
          value = patchOp["value"];
          JSONPointer pointer = JSONPointer.fromPointer(o, path);
          if (!pointer.exists()) {
            throw new ArgumentException("Patch " +
              valueOpStr + " " + path);
          }
          Object testedObj = pointer.getValue();
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
        JSONPointer pointer = JSONPointer.fromPointer(o, path);
        if (!pointer.exists()) {
          throw new KeyNotFoundException("Patch " +
            valueOpStr + " " + path);
        }
        o = pointer.getValue();
        if (pointer.getParent().Type == CBORType.Array) {
          ((CBORObject)pointer.getParent()).RemoveAt(pointer.getIndex());
        } else if (pointer.getParent().Type == CBORType.Map) {
          ((CBORObject)pointer.getParent()).Remove(
              CBORObject.FromObject(pointer.getKey()));
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
        JSONPointer pointer = JSONPointer.fromPointer(o, path);
        if (!pointer.exists()) {
          throw new KeyNotFoundException("Patch " +
            valueOpStr + " " + path);
        }
        if (pointer.getParent().Type == CBORType.Array) {
          int index = pointer.getIndex();
          if (index < 0) {
            throw new ArgumentException("Patch " + valueOpStr + " path");
          }
          ((CBORObject)pointer.getParent()).Set(index, value);
        } else if (pointer.getParent().Type == CBORType.Map) {
          string key = pointer.getKey();
          ((CBORObject)pointer.getParent()).Set(key, value);
        } else {
          throw new ArgumentException("Patch " + valueOpStr + " path");
        }
      }
      return o;
    }
  }
}
