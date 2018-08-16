/*
Written in 2013 by Peter Occil.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
*/
using System;
using PeterO.Cbor;

namespace Test {
  public class JSONPatch {
    private static CBORObject addOperation(
        CBORObject o,
        string valueOpStr,
        string path,
        CBORObject value) {
      if (path == null) {
        throw new ArgumentException("patch " + valueOpStr);
      }
      if (path.Length == 0) {
        o = value;
      } else {
        JSONPointer pointer = JSONPointer.fromPointer(o, path);
        if (pointer.getParent().Type == CBORType.Array) {
          int index = pointer.getIndex();
          if (index < 0) {
            throw new ArgumentException("patch " + valueOpStr + " path");
          }
          ((CBORObject)pointer.getParent()).Insert(index, value);
        } else if (pointer.getParent().Type == CBORType.Map) {
          string key = pointer.getKey();
          ((CBORObject)pointer.getParent()).Set(key, value);
        } else {
          throw new ArgumentException("patch " + valueOpStr + " path");
        }
      }
      return o;
    }

    private static CBORObject cloneCbor(CBORObject o) {
      return CBORObject.FromJSONString(o.ToJSONString());
    }

    private static string getString(CBORObject o, string key) {
      return o.ContainsKey(key) ? o[key].AsString() : null;
    }

    public static CBORObject patch(CBORObject o, CBORObject patch) {
      // clone the object in case of failure
      o = cloneCbor(o);
      for (int i = 0; i < patch.Count; ++i) {
        CBORObject patchOp = patch[i];
        // NOTE: This algorithm requires "op" to exist
        // only once; the CBORObject, however, does not
        // allow duplicates
        string valueOpStr = getString(patchOp, "op");
        if (valueOpStr == null) {
          throw new ArgumentException("patch");
        }
        if ("add".Equals(valueOpStr)) {
          // operation
          CBORObject value = null;
          try {
            value = patchOp["value"];
          } catch (System.Collections.Generic.KeyNotFoundException) {
            throw new ArgumentException("patch " + valueOpStr + " value");
          }
          o = addOperation(o, valueOpStr, getString(patchOp, "path"), value);
        } else if ("replace".Equals(valueOpStr)) {
          // operation
          CBORObject value = null;
          try {
            value = patchOp["value"];
          } catch (System.Collections.Generic.KeyNotFoundException) {
            throw new ArgumentException("patch " + valueOpStr + " value");
          }
        o = replaceOperation(
  o,
  valueOpStr,
  getString(patchOp, "path"),
  value);
        } else if ("remove".Equals(valueOpStr)) {
          // Remove operation
          string path = patchOp["path"].AsString();
          if (path == null) {
            throw new ArgumentException("patch " + valueOpStr + " path");
          }
          if (path.Length == 0) {
            o = null;
          } else {
            removeOperation(o, valueOpStr, getString(patchOp, "path"));
          }
        } else if ("move".Equals(valueOpStr)) {
          string path = patchOp["path"].AsString();
          if (path == null) {
            throw new ArgumentException("patch " + valueOpStr + " path");
          }
          string fromPath = patchOp["from"].AsString();
          if (fromPath == null) {
            throw new ArgumentException("patch " + valueOpStr + " from");
          }
          if (path.StartsWith(fromPath, StringComparison.Ordinal)) {
            throw new ArgumentException("patch " + valueOpStr);
          }
          CBORObject movedObj = removeOperation(o, valueOpStr, fromPath);
          o = addOperation(o, valueOpStr, path, cloneCbor(movedObj));
        } else if ("copy".Equals(valueOpStr)) {
          string path = patchOp["path"].AsString();
          string fromPath = patchOp["from"].AsString();
          if (path == null) {
            throw new ArgumentException("patch " + valueOpStr + " path");
          }
          if (fromPath == null) {
            throw new ArgumentException("patch " + valueOpStr + " from");
          }
          JSONPointer pointer = JSONPointer.fromPointer(o, path);
          if (!pointer.exists()) {
            throw new System.Collections.Generic.KeyNotFoundException("patch " +
              valueOpStr + " " + fromPath);
          }
          CBORObject copiedObj = pointer.getValue();
          o = addOperation(
  o,
  valueOpStr,
  path,
  cloneCbor(copiedObj));
        } else if ("test".Equals(valueOpStr)) {
          string path = patchOp["path"].AsString();
          if (path == null) {
            throw new ArgumentException("patch " + valueOpStr + " path");
          }
          CBORObject value = null;
          try {
            value = patchOp["value"];
          } catch (System.Collections.Generic.KeyNotFoundException) {
            throw new ArgumentException("patch " + valueOpStr + " value");
          }
          JSONPointer pointer = JSONPointer.fromPointer(o, path);
          if (!pointer.exists()) {
            throw new System.Collections.Generic.KeyNotFoundException("patch " +
              valueOpStr + " " + path);
          }
          Object testedObj = pointer.getValue();
        if ((testedObj == null) ? (value != null) :
            !testedObj.Equals(value)) {
            throw new InvalidOperationException("patch " + valueOpStr);
          }
        }
      }
      return (o == null) ? CBORObject.Null : o;
    }

    private static CBORObject removeOperation(
        CBORObject o,
        string valueOpStr,
        string path) {
      if (path == null) {
        throw new ArgumentException("patch " + valueOpStr);
      }
      if (path.Length == 0) {
        return o;
      } else {
        JSONPointer pointer = JSONPointer.fromPointer(o, path);
        if (!pointer.exists()) {
          throw new System.Collections.Generic.KeyNotFoundException("patch " +
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

    private static CBORObject replaceOperation(
        CBORObject o,
        string valueOpStr,
        string path,
        CBORObject value) {
      if (path == null) {
        throw new ArgumentException("patch " + valueOpStr);
      }
      if (path.Length == 0) {
        o = value;
      } else {
        JSONPointer pointer = JSONPointer.fromPointer(o, path);
        if (!pointer.exists()) {
          throw new System.Collections.Generic.KeyNotFoundException("patch " +
            valueOpStr + " " + path);
        }
        if (pointer.getParent().Type == CBORType.Array) {
          int index = pointer.getIndex();
          if (index < 0) {
            throw new ArgumentException("patch " + valueOpStr + " path");
          }
        ((CBORObject)pointer.getParent()).Set(index, value);
        } else if (pointer.getParent().Type == CBORType.Map) {
          string key = pointer.getKey();
          ((CBORObject)pointer.getParent()).Set(key, value);
        } else {
          throw new ArgumentException("patch " + valueOpStr + " path");
        }
      }
      return o;
    }
  }
}
