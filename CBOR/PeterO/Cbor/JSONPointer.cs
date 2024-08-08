/*
Written in 2013-2018 by Peter Occil.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

*/
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using PeterO.Numbers;

namespace PeterO.Cbor {
  internal sealed class JSONPointer {
    private readonly string refValue;
    private readonly bool isRoot;
    private readonly CBORObject jsonobj;

    public static JSONPointer FromPointer(CBORObject obj, string pointer) {
      var index = 0;
      if (pointer == null) {
        throw new ArgumentNullException(nameof(pointer));
      }
      if (obj == null) {
        throw new ArgumentNullException(nameof(obj));
      }
      if (pointer.Length == 0) {
        return new JSONPointer(obj, pointer, true);
      }
      while (true) {
        if (obj == null) {
          throw new CBORException("Invalid pointer: obj is null");
        }
        if (obj.Type == CBORType.Array) {
          if (index >= pointer.Length || pointer[index] != '/') {
            throw new CBORException("Invalid pointer");
          }
          ++index;
          var value = new int[] { 0 };
          // DebugUtility.Log("index parse 0: " + (pointer.Substring(index)));
          int newIndex = ReadPositiveInteger(pointer, index, value);
          // DebugUtility.Log("index parse 1: " + (pointer.Substring(newIndex)));
          if (value[0] < 0) {
            if (index < pointer.Length && pointer[index] == '-' &&
              (index + 1 == pointer.Length || pointer[index + 1] == '/')) {
              // Index at the end of the array
              return new JSONPointer(obj, "-");
            }
            throw new CBORException("Invalid pointer");
          }
          if (newIndex == pointer.Length) {
            return new JSONPointer(obj, pointer.Substring(index));
          } else if (value[0] > obj.Count) {
            throw new CBORException("Invalid array index in pointer");
          } else if (value[0] == obj.Count) {
            return newIndex + 1 == pointer.Length ?
              new JSONPointer(obj, pointer.Substring(index)) :
              throw new CBORException("Invalid array index in pointer");
          } else {
            obj = obj[value[0]];
#if DEBUG
            if (obj == null) {
              throw new ArgumentNullException(nameof(obj));
            }
#endif

            index = newIndex;
          }
          index = newIndex;
        } else if (obj.Type == CBORType.Map) {
          // DebugUtility.Log("Parsing map key(0) " + (pointer.Substring(index)));
          if (index >= pointer.Length || pointer[index] != '/') {
            throw new CBORException("Invalid pointer");
          }
          ++index;
          // DebugUtility.Log("Parsing map key " + (pointer.Substring(index)));
          string key = null;
          int oldIndex = index;
          var tilde = false;
          while (index < pointer.Length) {
            int c = pointer[index];
            if (c == '/') {
              break;
            }
            if (c == '~') {
              tilde = true;
              break;
            }
            ++index;
          }
          if (!tilde) {
            key = pointer.Substring(
              oldIndex,
              index - oldIndex);
          } else {
            index = oldIndex;
            var sb = new StringBuilder();
            while (index < pointer.Length) {
              int c = pointer[index];
              if (c == '/') {
                break;
              }
              if (c == '~') {
                if (index + 1 < pointer.Length) {
                  if (pointer[index + 1] == '1') {
                    index += 2;
                    sb.Append('/');
                    continue;
                  } else if (pointer[index + 1] == '0') {
                    index += 2;
                    sb.Append('~');
                    continue;
                  }
                }
                throw new CBORException("Invalid pointer");
              } else {
                sb.Append((char)c);
              }
              ++index;
            }
            key = sb.ToString();
          }
          if (index == pointer.Length) {
            return new JSONPointer(obj, key);
          } else {
            obj = obj.GetOrDefault(key, null);
            if (obj == null) {
              throw new CBORException("Invalid pointer; key not found");
            }
          }
        } else {
          throw new CBORException("Invalid pointer");
        }
      }
    }
    public static CBORObject GetObject(
      CBORObject obj,
      string pointer,
      CBORObject defaultValue) {
      if (obj == null) {
        throw new CBORException("obj");
      }
      if (pointer == null) {
        return defaultValue;
      }
      if (pointer.Length == 0) {
        return obj;
      }
      if (obj.Type != CBORType.Array && obj.Type != CBORType.Map) {
        return defaultValue;
      }
      try {
        CBORObject cobj = JSONPointer.FromPointer(obj, pointer).GetValue();
        return cobj ?? defaultValue;
      } catch (CBORException) {
        return defaultValue;
      }
    }

    private static int ReadPositiveInteger(
      string str,
      int index,
      int[] result) {
      var haveNumber = false;
      var haveZeros = false;
      int oldIndex = index;
      result[0] = -1;
      if (index == str.Length) {
        return index;
      }
      if (str.Length - 1 == index && str[index] == '0') {
        result[0] = 0;
        return index + 1;
      }
      if (str.Length - 1 > index && str[index] == '0' && str[index + 1] !=
'0') {
        result[0] = 0;
        return index + 1;
      }
      if (str[index] == '0') {
        // NOTE: Leading zeros not allowed in JSON Pointer numbers
        return index;
      }
      long lvalue = 0;
      while (index < str.Length) {
        int number = str[index++];
        if (number >= '0' && number <= '9') {
          lvalue = (lvalue * 10) + (number - '0');
          haveNumber = true;
          if (haveZeros) {
            return oldIndex + 1;
          }
        } else {
          --index;
          break;
        }
        if (lvalue > Int32.MaxValue) {
          return index - 1;
        }
      }
      if (!haveNumber) {
        return index;
      }
      result[0] = (int)lvalue;
      return index;
    }

    private JSONPointer(CBORObject jsonobj, string refValue)
        : this(jsonobj, refValue, false) {
    }

    private JSONPointer(CBORObject jsonobj, string refValue, bool isRoot) {
#if DEBUG
      if (!(refValue != null)) {
        throw new InvalidOperationException("doesn't satisfy refValue!=null");
      }
#endif
      this.isRoot = isRoot;
      this.jsonobj = jsonobj;
      this.refValue = refValue;
    }

    public bool Exists() {
      if (this.refValue.Length == 0) {
        // Root always exists
        return true;
      }
      if (this.jsonobj.Type == CBORType.Array) {
        if (this.refValue.Equals("-", StringComparison.Ordinal)) {
          return false;
        }
        var eivalue = EInteger.FromString(this.refValue);
        int icount = this.jsonobj.Count;
        return eivalue.Sign >= 0 &&
          eivalue.CompareTo(EInteger.FromInt32(icount)) < 0;
      } else {
        return this.jsonobj.Type == CBORType.Map ?
this.jsonobj.ContainsKey(this.refValue) : this.refValue.Length == 0;
      }
    }

    /// <summary>Gets an index into the specified object, if the object is
    /// an array and is not greater than the array's length.</summary>
    /// <returns>The index contained in this instance, or -1 if the object
    /// isn't a JSON array or is greater than the array's length.</returns>
    public int GetIndex() {
      if (this.jsonobj.Type == CBORType.Array) {
        if (this.refValue.Equals("-", StringComparison.Ordinal)) {
          return this.jsonobj.Count;
        }
        var value = EInteger.FromString(this.refValue);
        int icount = this.jsonobj.Count;
        return (value.Sign < 0) ? (-1) :
((value.CompareTo(EInteger.FromInt32(icount)) > 0) ? (-1) :

            value.ToInt32Unchecked());
      } else {
        return -1;
      }
    }

    public string GetKey() {
      return this.refValue;
    }

    public CBORObject GetParent() {
      return this.jsonobj;
    }

    public CBORObject GetValue() {
      if (this.isRoot) {
        // Root always exists
        return this.jsonobj;
      }
      CBORObject tmpcbor;
      if (this.jsonobj.Type == CBORType.Array) {
        int index = this.GetIndex();
        if (index >= 0 && index < this.jsonobj.Count) {
          tmpcbor = this.jsonobj;
          return tmpcbor[index];
        } else {
          return null;
        }
      } else if (this.jsonobj.Type == CBORType.Map) {
        // DebugUtility.Log("jsonobj=" + this.jsonobj + " refValue=[" + this.refValue
        // + "]");
        tmpcbor = this.jsonobj;
        return tmpcbor.GetOrDefault(this.refValue, null);
      } else {
        return (this.refValue.Length == 0) ? this.jsonobj : null;
      }
    }

    /// <summary>Gets all children of the specified JSON object that
    /// contain the specified key; the method will remove matching keys. As
    /// an example, consider this object:
    /// <pre>[{"key":"value1","foo":"foovalue"},
    /// {"key":"value2","bar":"barvalue"}, {"baz":"bazvalue"}]</pre> If
    /// getPointersToKey is called on this object with a keyToFind called
    /// "key", we get the following Map as the return value:
    /// <pre>{ "/0" => "value1", // "/0" points to {"foo":"foovalue"} "/1"
    /// => "value2" /* "/1" points to {"bar":"barvalue"} */ }</pre> and the
    /// JSON object will change to the following:
    /// <pre>[{"foo":"foovalue"}, {"bar":"barvalue"},
    /// {"baz","bazvalue"}]</pre>.</summary>
    /// <param name='root'>The object to search.</param>
    /// <param name='keyToFind'>The key to search for.</param>
    /// <returns>A map:
    /// <list>
    /// <item>The keys in the map are JSON Pointers to the objects within
    /// <i>root</i> that contained a key named
    /// <i>keyToFind</i>. To get the actual JSON object, call
    /// JSONPointer.GetObject, passing
    /// <i>root</i> and the pointer as arguments.</item>
    /// <item>The values in the map are the values of each of those keys
    /// named
    /// <i>keyToFind</i>.</item></list> The JSON Pointers are relative to
    /// the root object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='root'/> is null.</exception>
    public static IDictionary<string, CBORObject> GetPointersWithKeyAndRemove(
      CBORObject root,
      string keyToFind) {
      IDictionary<string, CBORObject> list = new Dictionary<string, CBORObject>();
      if (root == null) {
        throw new ArgumentNullException(nameof(root));
      }
      GetPointersWithKey(root, keyToFind, String.Empty, list, true);
      return list;
    }

    /// <summary>Gets all children of the specified JSON object that
    /// contain the specified key; the method will not remove matching
    /// keys. As an example, consider this object:
    /// <pre>[{"key":"value1","foo":"foovalue"},
    /// {"key":"value2","bar":"barvalue"}, {"baz":"bazvalue"}]</pre> If
    /// getPointersToKey is called on this object with a keyToFind called
    /// "key", we get the following Map as the return value:
    /// <pre>{ "/0" => "value1", // "/0" points to
    /// {"key":"value1","foo":"foovalue"} "/1" => "value2" // "/1" points
    /// to {"key":"value2","bar":"barvalue"} }</pre> and the JSON object
    /// will remain unchanged.
    /// <list>
    /// <item>The keys in the map are JSON Pointers to the objects within
    /// <i>root</i> that contained a key named
    /// <i>keyToFind</i>. To get the actual JSON object, call
    /// JSONPointer.GetObject, passing
    /// <i>root</i> and the pointer as arguments.</item>
    /// <item>The values in the map are the values of each of those keys
    /// named
    /// <i>keyToFind</i>.</item></list> The JSON Pointers are relative to
    /// the root object.</summary>
    /// <param name='root'>Object to search.</param>
    /// <param name='keyToFind'>The key to search for.</param>
    /// <returns>A map:.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='root'/> is null.</exception>
    public static IDictionary<string, CBORObject> GetPointersWithKey(
      CBORObject root,
      string keyToFind) {
      IDictionary<string, CBORObject> list = new Dictionary<string, CBORObject>();
      if (root == null) {
        throw new ArgumentNullException(nameof(root));
      }
      GetPointersWithKey(root, keyToFind, String.Empty, list, false);
      return list;
    }

    private static string Replace(string str, char c, string srep) {
      var j = -1;
      for (int i = 0; i < str.Length; ++i) {
        if (str[i] == c) {
          j = i;
          break;
        }
      }
      if (j == -1) {
        return str;
      }
      var sb = new StringBuilder();
      _ = sb.Append(str.Substring(0, j));
      _ = sb.Append(srep);
      for (int i = j + 1; i < str.Length; ++i) {
        sb = str[i] == c ? sb.Append(srep) : sb.Append(str[i]);
      }
      return sb.ToString();
    }

    private static void GetPointersWithKey(
      CBORObject root,
      string keyToFind,
      string currentPointer,
      IDictionary<string, CBORObject> pointerList,
      bool remove) {
      if (root.Type == CBORType.Map) {
        CBORObject rootObj = root;
        if (rootObj.ContainsKey(keyToFind)) {
          // Key found in this object,
          // add this object's JSON pointer
          CBORObject pointerKey = rootObj[keyToFind];
          pointerList.Add(currentPointer, pointerKey);
          // and remove the key from the object
          // if necessary
          if (remove) {
            _ = rootObj.Remove(CBORObject.FromString(keyToFind));
          }
        }
        // Search the key's values
        foreach (CBORObject key in rootObj.Keys) {
          string ptrkey = key.AsString();
          ptrkey = Replace(ptrkey, '~', "~0");
          ptrkey = Replace(ptrkey, '/', "~1");
          GetPointersWithKey(
            rootObj[key],
            keyToFind,
            currentPointer + "/" + ptrkey,
            pointerList,
            remove);
        }
      } else if (root.Type == CBORType.Array) {
        for (int i = 0; i < root.Count; ++i) {
          string ptrkey = EInteger.FromInt32(i).ToString();
          GetPointersWithKey(
            root[i],
            keyToFind,
            currentPointer + "/" + ptrkey,
            pointerList,
            remove);
        }
      }
    }
  }
}
