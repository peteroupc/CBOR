/*
Written in 2013 by Peter Occil.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
https://creativecommons.org/publicdomain/zero/1.0/

*/
using System;
using System.Collections.Generic;

namespace PeterO.Cbor {
  internal static class JSONPatch {
    private static CBORObject AddOperation(
      CBORObject o,
      string valueOpStr,
      string path,
      CBORObject value) {
      if (path == null) {
        throw new CBORException("Patch " + valueOpStr);
      }
      if (path.Length == 0) {
        o = value;
      } else {
        // DebugUtility.Log("pointer--->"+path);
        JSONPointer pointer = JSONPointer.FromPointer(o, path);
        CBORObject parent = pointer.GetParent();
        // DebugUtility.Log("addop pointer "+path+" ["+parent+"]");
        if (pointer.GetParent().Type == CBORType.Array) {
          int index = pointer.GetIndex();
          // DebugUtility.Log("index "+index);
          if (index < 0) {
            throw new CBORException("Patch " + valueOpStr + " path");
          }
          // DebugUtility.Log("before "+parent+"");
          parent.Insert(index, value);
          // DebugUtility.Log("after "+parent+"");
        } else if (pointer.GetParent().Type == CBORType.Map) {
          string key = pointer.GetKey();
          parent.Set(key, value);
        } else {
          throw new CBORException("Patch " + valueOpStr + " path");
        }
      }
      return o;
    }

    private static CBORObject CloneCbor(CBORObject o) {
      switch (o.Type) {
        case CBORType.ByteString:
        case CBORType.Map:
        case CBORType.Array:
          return CBORObject.DecodeFromBytes(o.EncodeToBytes());
        default: return o;
      }
    }

    private static string GetString(CBORObject o, string str) {
      #if DEBUG
      if (o == null) {
        throw new ArgumentNullException(nameof(o));
      }
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      #endif

      CBORObject co = o.GetOrDefault(str, null);
      if (co == null) {
        throw new CBORException(str + " not found");
      }
      if (co.Type != CBORType.TextString) {
        throw new CBORException("Not a text string type");
      }
      return co.AsString();
    }

    public static CBORObject Patch(CBORObject o, CBORObject ptch) {
      // clone the object in case of failure
      if (o == null) {
        throw new CBORException("object is null");
      }
      if (ptch == null) {
        throw new CBORException("patch is null");
      }
      if (ptch.Type != CBORType.Array) {
        throw new CBORException("patch is not an array");
      }
      o = CloneCbor(o);
      for (int i = 0; i < ptch.Count; ++i) {
        CBORObject patchOp = ptch[i];
        #if DEBUG
        if (patchOp == null) {
          throw new ArgumentNullException(nameof(patchOp));
        }
        #endif

        // NOTE: This algorithm requires "op" to exist
        // only once; the CBORObject, however, does not
        // allow duplicates
        string valueOpStr = GetString(patchOp, "op");
        if ("add".Equals(valueOpStr, StringComparison.Ordinal)) {
          // operation
          CBORObject value = patchOp.GetOrDefault("value", null);
          if (value == null) {
            throw new CBORException("Patch " + valueOpStr + " value");
          }
          value = patchOp["value"];
          o = AddOperation(
              o,
              valueOpStr,
              GetString(patchOp, "path"),
              value);
        } else if ("replace".Equals(valueOpStr, StringComparison.Ordinal)) {
          // operation
          CBORObject value = patchOp.GetOrDefault("value", null);
          if (value == null) {
            throw new CBORException("Patch " + valueOpStr + " value");
          }
          #if DEBUG
          if (o == null) {
            throw new ArgumentNullException(nameof(o));
          }
          #endif

          o = ReplaceOperation(
              o,
              valueOpStr,
              GetString(patchOp, "path"),
              CloneCbor(value));
        } else if ("remove".Equals(valueOpStr, StringComparison.Ordinal)) {
          // Remove operation
          string path = GetString(patchOp, "path");
          if (path == null) {
            throw new CBORException("Patch " + valueOpStr + " path");
          }
          if (path.Length == 0) {
            o = null;
          } else {
            RemoveOperation(o, valueOpStr, GetString(patchOp, "path"));
          }
        } else if ("move".Equals(valueOpStr, StringComparison.Ordinal)) {
          string path = GetString(patchOp, "path");
          if (path == null) {
            throw new CBORException("Patch " + valueOpStr + " path");
          }
          string fromPath = GetString(patchOp, "from");
          if (fromPath == null) {
            throw new CBORException("Patch " + valueOpStr + " from");
          }
          if (path.Equals(fromPath)) {
            JSONPointer pointer = JSONPointer.FromPointer(o, path);
            if (pointer.Exists()) {
              // Moving to the same path, so return
              return o;
            }
          }
          // if (path.StartsWith(fromPath, StringComparison.Ordinal)) {
          // throw new CBORException("Patch " + valueOpStr + ": startsWith failed " +
          // "[" + path + "] [" + fromPath + "]");
          // }
          CBORObject movedObj = RemoveOperation(o, valueOpStr, fromPath);
          o = AddOperation(o, valueOpStr, path, CloneCbor(movedObj));
        } else if ("copy".Equals(valueOpStr, StringComparison.Ordinal)) {
          string path = GetString(patchOp, "path");
          string fromPath = GetString(patchOp, "from");
          if (path == null) {
            throw new CBORException("Patch " + valueOpStr + " path");
          }
          if (fromPath == null) {
            throw new CBORException("Patch " + valueOpStr + " from");
          }
          JSONPointer pointer = JSONPointer.FromPointer(o, fromPath);
          if (!pointer.Exists()) {
            throw new CBORException("Patch " +
              valueOpStr + " " + fromPath);
          }
          CBORObject copiedObj = pointer.GetValue();
          o = AddOperation(
              o,
              valueOpStr,
              path,
              CloneCbor(copiedObj));
        } else if ("test".Equals(valueOpStr, StringComparison.Ordinal)) {
          string path = GetString(patchOp, "path");
          if (path == null) {
            throw new CBORException("Patch " + valueOpStr + " path");
          }
          CBORObject value = null;
          if (!patchOp.ContainsKey("value")) {
            throw new CBORException("Patch " + valueOpStr + " value");
          }
          value = patchOp["value"];
          JSONPointer pointer = JSONPointer.FromPointer(o, path);
          if (!pointer.Exists()) {
            throw new CBORException("Patch " +
              valueOpStr + " " + path);
          }
          Object testedObj = pointer.GetValue();
          if ((testedObj == null) ? (value != null) :
            !testedObj.Equals(value)) {
            throw new CBORException("Patch " + valueOpStr);
          }
        } else {
          throw new CBORException("Unrecognized op");
        }
      }
      return (o == null) ? CBORObject.Null : o;
    }

    private static CBORObject RemoveOperation(
      CBORObject o,
      string valueOpStr,
      string path) {
      if (path == null) {
        throw new CBORException("Patch " + valueOpStr);
      }
      if (path.Length == 0) {
        return o;
      } else {
        JSONPointer pointer = JSONPointer.FromPointer(o, path);
        if (!pointer.Exists()) {
          throw new CBORException("Patch " +
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
        throw new CBORException("Patch " + valueOpStr);
      }
      #if DEBUG
      if (o == null) {
        throw new ArgumentNullException(nameof(o));
      }
      #endif

      if (path.Length == 0) {
        o = value;
      } else {
        JSONPointer pointer = JSONPointer.FromPointer(o, path);
        if (!pointer.Exists()) {
          throw new CBORException("Patch " +
            valueOpStr + " " + path);
        }
        if (pointer.GetParent().Type == CBORType.Array) {
          int index = pointer.GetIndex();
          if (index < 0) {
            throw new CBORException("Patch " + valueOpStr + " path");
          }
          ((CBORObject)pointer.GetParent()).Set(index, value);
        } else if (pointer.GetParent().Type == CBORType.Map) {
          string key = pointer.GetKey();
          ((CBORObject)pointer.GetParent()).Set(key, value);
        } else {
          throw new CBORException("Patch " + valueOpStr + " path");
        }
      }
      return o;
    }
  }
}
