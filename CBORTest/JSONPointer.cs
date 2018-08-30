/*
Written in 2013 by Peter Occil.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
*/
using System;
using System.Collections.Generic;
using System.Text;
using PeterO.Cbor;
using PeterO.Numbers;

namespace Test {
  public sealed class JSONPointer {
    public static JSONPointer fromPointer(CBORObject obj, string pointer) {
      var index = 0;
      if (pointer == null) {
        throw new ArgumentNullException(nameof(pointer));
      }
      if (pointer.Length == 0) {
        return new JSONPointer(obj, pointer);
      }
      while (true) {
        if (obj is CBORObject) {
          if (index >= pointer.Length || pointer[index] != '/') {
            throw new ArgumentException(pointer);
          }
          ++index;
          var value = new int[] { 0 };
          int newIndex = readPositiveInteger(pointer, index, value);
          if (value[0] < 0) {
            if (index < pointer.Length && pointer[index] == '-' &&
                (index + 1 == pointer.Length || pointer[index + 1] == '/')) {
              // Index at the end of the array
              return new JSONPointer(obj, "-");
            }
            throw new ArgumentException(pointer);
          }
          if (newIndex == pointer.Length) {
            return new JSONPointer(obj, pointer.Substring(index));
          } else {
            obj = ((CBORObject)obj)[value[0]];
            index = newIndex;
          }
          index = newIndex;
        } else if (obj is CBORObject) {
          if (obj.Equals(CBORObject.Null)) {
            throw new KeyNotFoundException(pointer);
          }
          if (index >= pointer.Length || pointer[index] != '/') {
            throw new ArgumentException(pointer);
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
                throw new ArgumentException(pointer);
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
          throw new KeyNotFoundException(pointer);
        }
      }
    }

    /// <summary>Gets the JSON object referred to by a JSON Pointer
    /// according to RFC6901. The syntax for pointers is:
    /// <pre>'/' KEY '/' KEY [...]</pre> where KEY represents a key into
    /// the JSON object or its sub-objects in the hierarchy. For example,
    /// <pre>/foo/2/bar</pre> means the same as
    /// <pre>obj['foo'][2]['bar']</pre> in JavaScript. If "~" and "/" occur
    /// in a key, they must be escaped with "~0" and "~1", respectively, in
    /// a JSON pointer. @param obj An object, especially a CBORObject or
    /// @param pointer A JSON pointer according to RFC 6901. @return An
    /// object within the specified JSON object, or _obj_ if pointer is the
    /// empty _string, if the pointer is null, if the pointer is invalid ,
    /// if there is no JSON object at the given pointer, or if _obj_ is not
    /// of type CBORObject, unless pointer is the empty _string.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is not
    /// documented yet.</param>
    /// <param name='pointer'>The parameter <paramref name='pointer'/> is
    /// not documented yet.</param>
    /// <returns>An arbitrary object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='pointer'/> is null.</exception>
    public static Object getObject(CBORObject obj, string pointer) {
      if (pointer == null) {
        throw new ArgumentNullException(nameof(pointer));
      }
      return (pointer.Length == 0) ? obj :
        JSONPointer.fromPointer(obj, pointer).getValue();
    }

    private static int readPositiveInteger(
        string str,
        int index,
        int[] result) {
      var haveNumber = false;
      var haveZeros = false;
      int oldIndex = index;
      result[0] = -1;
      while (index < str.Length) {  // skip zeros
        int c = str[index++];
        if (c != '0') {
          --index;
          break;
        }
        if (haveZeros) {
          --index;
          return index;
        }
        haveNumber = true;
        haveZeros = true;
      }
      long value = 0;
      while (index < str.Length) {
        int number = str[index++];
        if (number >= '0' && number <= '9') {
          value = (value * 10) + (number - '0');
          haveNumber = true;
          if (haveZeros) {
            return oldIndex + 1;
          }
        } else {
          --index;
          break;
        }
        if (value > Int32.MaxValue) {
          return index - 1;
        }
      }
      if (!haveNumber) {
        return index;
      }
      result[0] = (int)value;
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

    public bool exists() {
      if (this.jsonobj.Type == CBORType.Array) {
        if (this.refValue.Equals("-")) {
          return false;
        }
        EInteger
        value = PeterO.Numbers.EInteger.FromString(this.refValue);
        return value.Sign >= 0 &&
                    value.CompareTo(((CBORObject)this.jsonobj).Count) < 0;
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
    public int getIndex() {
      if (this.jsonobj.Type == CBORType.Array) {
        if (this.refValue.Equals("-")) {
          return ((CBORObject)this.jsonobj).Count;
        }
        EInteger
        value = PeterO.Numbers.EInteger.FromString(this.refValue);
        return (value.Sign < 0) ? (-1) :
          ((value.CompareTo(((CBORObject)this.jsonobj).Count) > 0) ? (-1) :
           value.ToInt32Unchecked()); } else {
        return -1;
      }
    }

    public string getKey() {
      return this.refValue;
    }

    public CBORObject getParent() {
      return this.jsonobj;
    }

    public CBORObject getValue() {
      if (this.refValue.Length == 0) {
        return this.jsonobj;
      }
      if (this.jsonobj.Type == CBORType.Array) {
        int index = this.getIndex();
        if (index >= 0 && index < ((CBORObject)this.jsonobj).Count) {
          return ((CBORObject)this.jsonobj)[index];
        } else {
          return null;
        }
      } else if (this.jsonobj.Type == CBORType.Map) {
        return ((CBORObject)this.jsonobj)[this.refValue];
      } else {
        return (this.refValue.Length == 0) ? this.jsonobj : null;
      }
    }

    /// <summary>Gets all children of the specified JSON object that
    /// contain the specified key. The method will not remove matching
    /// keys. As an example, consider this object:
    /// <pre>[{"key":"value1","foo":"foovalue"},
    /// {"key":"value2","bar":"barvalue"}, {"baz":"bazvalue"}]</pre> If
    /// getPointersToKey is called on this object with a keyToFind called
    /// "key", we get the following Map as the return value:
    /// <pre>{ "/0" => "value1", // "/0" points to {"foo":"foovalue"} "/1"
    /// => "value2" // "/1" points to {"bar":"barvalue"} }</pre> and the
    /// JSON object will change to the following:
    /// <pre>[{"foo":"foovalue"}, {"bar":"barvalue"},
    /// {"baz","bazvalue"}]</pre> @param root object to search @param
    /// keyToFind the key to search for. @return a map:
    /// <list>
    /// <item>The keys in the map are JSON Pointers to the objects within
    /// <i>root</i> that contained a key named
    /// <i>keyToFind</i>. To get the actual JSON object, call
    /// JSONPointer.getObject, passing
    /// <i>root</i> and the pointer as arguments.</item>
    /// <item>The values in the map are the values of each of those keys
    /// named
    /// <i>keyToFind</i>.</item></list> The JSON Pointers are relative to
    /// the root object.</summary>
    /// <param name='root'>The parameter <paramref name='root'/> is not
    /// documented yet.</param>
    /// <param name='keyToFind'>The parameter <paramref name='keyToFind'/>
    /// is not documented yet.</param>
    /// <returns>An IDictionary(string, Object) object.</returns>
    public static IDictionary<string, Object>
      getPointersWithKeyAndRemove(CBORObject root, string keyToFind) {
      IDictionary<string, Object> list = new Dictionary<string, Object>();
      getPointersWithKey(root, keyToFind, String.Empty, list, true);
      return list;
    }

    /// <summary>Gets all children of the specified JSON object that
    /// contain the specified key. The method will remove matching keys. As
    /// an example, consider this object:
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
    /// JSONPointer.getObject, passing
    /// <i>root</i> and the pointer as arguments.</item>
    /// <item>The values in the map are the values of each of those keys
    /// named
    /// <i>keyToFind</i>.</item></list> The JSON Pointers are relative to
    /// the root object.</summary>
    /// <param name='root'>The parameter <paramref name='root'/> is not
    /// documented yet.</param>
    /// <param name='keyToFind'>The parameter <paramref name='keyToFind'/>
    /// is not documented yet.</param>
    /// <returns>An IDictionary(string, Object) object.</returns>
    public static IDictionary<string, Object> getPointersWithKey(
        CBORObject root,
        string keyToFind) {
      IDictionary<string, Object> list = new Dictionary<string, Object>();
      getPointersWithKey(root, keyToFind, String.Empty, list, false);
      return list;
    }

    private static void getPointersWithKey(
        CBORObject root,
        string keyToFind,
        string currentPointer,
        IDictionary<string, Object> pointerList,
        bool remove) {
      if (root.Type == CBORType.Map) {
        var rootObj = (CBORObject)root;
        if (rootObj.ContainsKey(keyToFind)) {
          // Key found in this object,
          // add this object's JSON pointer
          Object pointerKey = rootObj[keyToFind];
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
          ptrkey = ptrkey.Replace("~", "~0");
          ptrkey = ptrkey.Replace("/", "~1");
          getPointersWithKey(
  rootObj[key],
  keyToFind,
  currentPointer + "/" + ptrkey,
  pointerList,
  remove);
        }
      } else if (root.Type == CBORType.Array) {
        for (int i = 0; i < root.Count; ++i) {
          string ptrkey = PeterO.Numbers.EInteger.FromInt32(i).ToString();
          getPointersWithKey(
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
