/*
Written in 2013-2018 by Peter Occil.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
https://creativecommons.org/publicdomain/zero/1.0/

*/
using System;
using System.Collections.Generic;
using System.Text;
using PeterO.Numbers;

namespace PeterO.Cbor {
  internal sealed class JSONPointer {
    public static JSONPointer FromPointer(CBORObject obj, string pointer) {
      var index = 0;
      if (pointer == null) {
        throw new ArgumentNullException(nameof(pointer));
      }
      if (pointer.Length == 0) {
        return new JSONPointer(obj, pointer);
      }
      while (true) {
        if (obj == null) {
          throw new ArgumentNullException(nameof(obj));
        }
        if (obj.Type == CBORType.Array) {
          if (index >= pointer.Length || pointer[index] != '/') {
            throw new CBORException("Invalid pointer");
          }
          ++index;
          var value = new int[] { 0 };
          int newIndex = ReadPositiveInteger(pointer, index, value);
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
          } else {
            obj = obj[value[0]];
            index = newIndex;
          }
          index = newIndex;
        } else if (obj.Type == CBORType.Map) {
          if (obj.Equals(CBORObject.Null)) {
            throw new CBORException("Invalid pointer");
          }
          if (index >= pointer.Length || pointer[index] != '/') {
            throw new CBORException("Invalid pointer");
          }
          ++index;
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
            obj = ((CBORObject)obj)[key];
          }
        } else {
          throw new CBORException("Invalid pointer");
        }
      }
    }

    /// <summary>Gets the JSON object referred to by a JSON Pointer
    /// according to RFC6901. The syntax for pointers is:
    /// <pre>'/' KEY '/' KEY [...]</pre> where KEY represents a key into
    /// the JSON object or its sub-objects in the hierarchy. For example,
    /// <pre>/foo/2/bar</pre> means the same as
    /// <pre>obj['foo'][2]['bar']</pre> in JavaScript. If "~" and/or "/"
    /// occurs in a key, it must be escaped with "~0" or "~1",
    /// respectively, in a JSON pointer. JSON pointers also support the
    /// special key "-" (as in "/foo/-") to indicate the end of an array,
    /// but this method treats this key as an error since it refers to a
    /// nonexistent item.</summary>
    /// <param name='obj'>A CBOR object.</param>
    /// <param name='pointer'>A JSON pointer according to RFC 6901.</param>
    /// <returns>An object within the specified JSON object. Returns
    /// <paramref name='obj'/> if pointer is the empty string (even if
    /// "obj" has a CBOR type other than array or map). Returns <paramref
    /// name='defaultValue'/> if the pointer is null, if the pointer is
    /// invalid, or if there is no object at the given pointer, or the
    /// special key "-" appears in the pointer. (Note that RFC 6901 was
    /// published before JSON was extended to support top-level values
    /// other than arrays and key-value dictionaries.)</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='pointer'/> or <paramref name='obj'/> is null.</exception>
    /// <param name='defaultValue'/>
    public static CBORObject GetObject(
      CBORObject obj,
      string pointer,
      CBORObject defaultValue) {
      if (obj == null) {
        throw new ArgumentNullException(nameof(obj));
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
      CBORObject cobj = JSONPointer.FromPointer(obj, pointer).GetValue();
      return cobj == null ? defaultValue : cobj;
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
      if (str.Length - 1 == index && str[index] =='0') {
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

    private string refValue;

    private CBORObject jsonobj;

    private JSONPointer(CBORObject jsonobj, string refValue) {
      #if DEBUG
      if (!(refValue != null)) {
        throw new InvalidOperationException("doesn't satisfy refValue!=null");
      }
      #endif
      this.jsonobj = jsonobj;
      this.refValue = refValue;
    }

    public bool Exists() {
      if (this.jsonobj.Type == CBORType.Array) {
        if (this.refValue.Equals("-", StringComparison.Ordinal)) {
          return false;
        }
        EInteger eivalue = EInteger.FromString(this.refValue);
        int icount = ((CBORObject)this.jsonobj).Count;
        return eivalue.Sign >= 0 &&
          eivalue.CompareTo(EInteger.FromInt32(icount)) < 0;
        } else if (this.jsonobj.Type == CBORType.Map) {
          return ((CBORObject)this.jsonobj).ContainsKey(this.refValue);
        } else {
        return this.refValue.Length == 0;
      }
    }

    /// <summary>Gets an index into the specified object, if the object is
    /// an array and is not greater than the array's length.</summary>
    /// <returns>The index contained in this instance, or -1 if the object
    /// isn't a JSON array or is greater than the array's length.</returns>
    public int GetIndex() {
      if (this.jsonobj.Type == CBORType.Array) {
        if (this.refValue.Equals("-", StringComparison.Ordinal)) {
          return ((CBORObject)this.jsonobj).Count;
        }
        EInteger value = EInteger.FromString(this.refValue);
        int icount = ((CBORObject)this.jsonobj).Count;
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
      if (this.refValue.Length == 0) {
        return this.jsonobj;
      }
      CBORObject tmpcbor = null;
      if (this.jsonobj.Type == CBORType.Array) {
        int index = this.GetIndex();
        if (index >= 0 && index < ((CBORObject)this.jsonobj).Count) {
          tmpcbor = this.jsonobj;
          return tmpcbor[index];
        } else {
          return null;
        }
      } else if (this.jsonobj.Type == CBORType.Map) {
        tmpcbor = this.jsonobj;
        return tmpcbor[this.refValue];
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
    /// will remain unchanged. @param root object to search @param
    /// keyToFind the key to search for. @return a map:
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
    /// <param name='root'>The parameter <paramref name='root'/> is not
    /// documented yet.</param>
    /// <param name='keyToFind'>The parameter <paramref name='keyToFind'/>
    /// is not documented yet.</param>
    /// <returns>An IDictionary(string, CBORObject) object.</returns>
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

    private static String Replace(string str, char c, string srep) {
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
      sb.Append(str.Substring(0, j));
      sb.Append(srep);
      for (int i = j + 1; i < str.Length; ++i) {
        if (str[i] == c) {
          sb.Append(srep);
        } else {
          sb.Append(str[i]);
        }
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
        var rootObj = (CBORObject)root;
        if (rootObj.ContainsKey(keyToFind)) {
          // Key found in this object,
          // add this object's JSON pointer
          CBORObject pointerKey = rootObj[keyToFind];
          pointerList.Add(currentPointer, pointerKey);
          // and remove the key from the object
          // if necessary
          if (remove) {
            rootObj.Remove(CBORObject.FromObject(keyToFind));
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
