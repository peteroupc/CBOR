// Written by Peter O.
// Any copyright to this work is released to the Public Domain.
// https://creativecommons.org/publicdomain/zero/1.0/

using System;
using System.Collections.Generic;
using System.Text;
using PeterO.Cbor;

namespace Test {
  public sealed class QueryStringHelper {
    private QueryStringHelper() {
    }
    private static string[] SplitAt(string stringToSplit, string delimiter) {
      if (delimiter == null || delimiter.Length == 0) {
        throw new ArgumentException();
      }
      if (stringToSplit == null || stringToSplit.Length == 0) {
        return new string[] { String.Empty };
      }
      var index = 0;
      var first = true;
      List<string> strings = null;
      int delimLength = delimiter.Length;
      while (true) {
        int index2 = stringToSplit.IndexOf(
          delimiter,
          index,
          StringComparison.Ordinal);
        if (index2 < 0) {
          if (first) {
            return new string[] { stringToSplit };
          }
          strings.Add(stringToSplit[index..]);
          break;
        } else {
          if (first) {
            strings = new List<string>();
            first = false;
          }
          string newstr = stringToSplit[index..index2];
          strings.Add(newstr);
          index = index2 + delimLength;
        }
      }
      return strings.ToArray();
    }

    private static int ToHexNumber(int c) {
      return c is >= 'A' and <= 'Z' ? 10 + c - 'A' : c is >= 'a' and <= 'z' ?
10 + c - 'a' : (c is >= '0' and <= '9') ? (c - '0') : (-1);
    }
    private static string PercentDecodeUTF8(string str) {
      int len = str.Length;
      var percent = false;
      for (int i = 0; i < len; ++i) {
        char c = str[i];
        if (c == '%') {
          percent = true;
        } else if (c >= 0x80) {
          // Non-ASCII characters not allowed
          throw new InvalidOperationException();
        }
      }
      if (!percent) {
        return str; // return early if there are no percent decodings
      }
      var cp = 0;
      var bytesSeen = 0;
      var bytesNeeded = 0;
      var lower = 0x80;
      var upper = 0xbf;
      var retString = new StringBuilder();
      for (int i = 0; i < len; ++i) {
        int c = str[i];
        if (c == '%') {
          if (i + 2 < len) {
            int a = ToHexNumber(str[i + 1]);
            int b = ToHexNumber(str[i + 2]);
            if (a >= 0 && b >= 0) {
              b = (byte)((a * 16) + b);
              i += 2;
              // b now contains the byte read
              if (bytesNeeded == 0) {
                // this is the lead byte
                if (b < 0x80) {
                  _ = retString.Append((char)b);
                  continue;
                } else if (b is >= 0xc2 and <= 0xdf) {
                  bytesNeeded = 1;
                  cp = b - 0xc0;
                } else if (b is >= 0xe0 and <= 0xef) {
                  lower = (b == 0xe0) ? 0xa0 : 0x80;
                  upper = (b == 0xed) ? 0x9f : 0xbf;
                  bytesNeeded = 2;
                  cp = b - 0xe0;
                } else if (b is >= 0xf0 and <= 0xf4) {
                  lower = (b == 0xf0) ? 0x90 : 0x80;
                  upper = (b == 0xf4) ? 0x8f : 0xbf;
                  bytesNeeded = 3;
                  cp = b - 0xf0;
                } else {
                  // illegal byte in UTF-8
                  throw new InvalidOperationException();
                }
                cp <<= 6 * bytesNeeded;
                continue;
              } else {
                // this is a second or further byte
                if (b < lower || b > upper) {
                  // illegal trailing byte
                  throw new InvalidOperationException();
                }
                // reset lower and upper for the third
                // and further bytes
                lower = 0x80;
                upper = 0xbf;
                ++bytesSeen;
                cp += (b - 0x80) << (6 * (bytesNeeded - bytesSeen));
                if (bytesSeen != bytesNeeded) {
                  // continue if not all bytes needed
                  // were read yet
                  continue;
                }
                int ret = cp;
                cp = 0;
                bytesSeen = 0;
                bytesNeeded = 0;
                // append the Unicode character
                if (ret <= 0xffff) {
                  {
                    _ = retString.Append((char)ret);
                  }
                } else {
                  _ = retString.Append((char)((((ret - 0x10000) >> 10) &
                        0x3ff) | 0xd800));
                  _ = retString.Append((char)(((ret - 0x10000) & 0x3ff) |
                      0xdc00));
                }
                continue;
              }
            }
          }
        }
        if (bytesNeeded > 0) {
          // we expected further bytes here,
          // so throw an exception
          throw new InvalidOperationException();
        }
        // append the code point as is (we already
        // checked for ASCII characters so this will
        // be simple
        _ = retString.Append((char)(c & 0xff));
      }
      if (bytesNeeded > 0) {
        // we expected further bytes here,
        // so throw an exception
        throw new InvalidOperationException();
      }
      return retString.ToString();
    }
    public static IList<string[]> ParseQueryString(
      string input) {
      return ParseQueryString(input, null);
    }

    public static IList<string[]> ParseQueryString(
      string input,
      string delimiter) {
      if (input == null) {
        throw new ArgumentNullException(nameof(input));
      }
      // set default delimiter to ampersand
      delimiter ??= "&";
      // Check input for non-ASCII characters
      for (int i = 0; i < input.Length; ++i) {
        if (input[i] > 0x7f) {
          throw new ArgumentException("input contains a non-ASCII character");
        }
      }
      // split on delimiter
      string[] strings = SplitAt(input, delimiter);
      var pairs = new List<string[]>();
      foreach (string str in strings) {
        if (str.Length == 0) {
          continue;
        }
        // split on key
        int index = str.IndexOf('=');
        string name = str;
        string value = String.Empty; // value is empty if there is no key
        if (index >= 0) {
          name = str[..index];
          value = str[(index + 1)..];
        }
        name = name.Replace('+', ' ');
        value = value.Replace('+', ' ');
        var pair = new string[] { name, value };
        pairs.Add(pair);
      }
      foreach (string[] pair in pairs) {
        // percent decode the key and value if necessary
        pair[0] = PercentDecodeUTF8(pair[0]);
        pair[1] = PercentDecodeUTF8(pair[1]);
      }
      return pairs;
    }

    private static string[] GetKeyPath(string s) {
      int index = s.IndexOf('[');
      if (index < 0) { // start bracket not found
        return new string[] { s };
      }
      var path = new List<string> {
        s[..index],
      };
      ++index; // move to after the bracket
      while (true) {
        int endBracket = s.IndexOf(']', index);
        if (endBracket < 0) { // end bracket not found
          path.Add(s[index..]);
          break;
        }
        path.Add(s[index..endBracket]);
        index = endBracket + 1; // move to after the end bracket
        index = s.IndexOf('[', index);
        if (index < 0) { // start bracket not found
          break;
        }
        ++index; // move to after the start bracket
      }
      return path.ToArray();
    }

    private static readonly string Digits = "0123456789";

    public static string IntToString(int value) {
      if (value == 0) {
        return "0";
      }
      if (value == Int32.MinValue) {
        return "-2147483648";
      }
      bool neg = value < 0;
      if (neg) {
        value = -value;
      }
      char[] chars;
      int count;
      if (value < 100000) {
        if (neg) {
          chars = new char[6];
          count = 5;
        } else {
          chars = new char[5];
          count = 4;
        }
        while (value > 9) {
          int intdivvalue = unchecked((((value >> 1) * 52429) >> 18) & 16383);
          char digit = Digits[value - (intdivvalue * 10)];
          chars[count--] = digit;
          value = intdivvalue;
        }
        if (value != 0) {
          chars[count--] = Digits[value];
        }
        if (neg) {
          chars[count] = '-';
        } else {
          ++count;
        }
        return new String(chars, count, chars.Length - count);
      }
      chars = new char[12];
      count = 11;
      while (value >= 163840) {
        int intdivvalue = value / 10;
        char digit = Digits[value - (intdivvalue * 10)];
        chars[count--] = digit;
        value = intdivvalue;
      }
      while (value > 9) {
        int intdivvalue = unchecked((((value >> 1) * 52429) >> 18) & 16383);
        char digit = Digits[value - (intdivvalue * 10)];
        chars[count--] = digit;
        value = intdivvalue;
      }
      if (value != 0) {
        chars[count--] = Digits[value];
      }
      if (neg) {
        chars[count] = '-';
      } else {
        ++count;
      }
      return new String(chars, count, 12 - count);
    }

    private static bool IsList(IDictionary<string, object> dict) {
      if (dict == null) {
        return false;
      }
      var index = 0;
      int count = dict.Count;
      if (count == 0) {
        return false;
      }
      while (true) {
        if (index == count) {
          return true;
        }
        string indexString = IntToString(index);
        if (!dict.ContainsKey(indexString)) {
          return false;
        }
        ++index;
      }
    }

    private static IList<object> ConvertToList(IDictionary<string, object>
      dict) {
      var ret = new List<object>();
      var index = 0;
      int count = dict.Count;
      while (index < count) {
        string indexString = IntToString(index);
        if (!dict.TryGetValue(indexString, out object o)) {
          throw new InvalidOperationException();
        }
        ret.Add(o);
        ++index;
      }
      return ret;
    }

    private static CBORObject ConvertListsToCBOR(IList<object> dict) {
      var cbor = CBORObject.NewArray();
      for (int i = 0; i < dict.Count; ++i) {
        object di = dict[i];
        var value = di as IDictionary<string, object>;
        // A list contains only indexes 0, 1, 2, and so on,
        // with no gaps.
        if (IsList(value)) {
          IList<object> newList = ConvertToList(value);
          _ = cbor.Add(ConvertListsToCBOR(newList));
        } else if (value != null) {
          // Convert the list's descendents
          // if they are lists
          _ = cbor.Add(ConvertListsToCBOR(value));
        } else {
          _ = cbor.Add(dict[i]);
        }
      }
      return cbor;
    }

    private static CBORObject ConvertListsToCBOR(IDictionary<string, object>
      dict) {
      var cbor = CBORObject.NewMap();
      foreach (string key in new List<string>(dict.Keys)) {
        object di = dict[key];
        var value = di as IDictionary<string, object>;
        // A list contains only indexes 0, 1, 2, and so on,
        // with no gaps.
        if (IsList(value)) {
          IList<object> newList = ConvertToList(value);
          _ = cbor.Add(key, ConvertListsToCBOR(newList));
        } else if (value != null) {
          // Convert the dictionary's descendents
          // if they are lists
          _ = cbor.Add(key, ConvertListsToCBOR(value));
        } else {
          _ = cbor.Add(key, dict[key]);
        }
      }
      return cbor;
    }

    private static void ConvertLists(IList<object> list) {
      for (int i = 0; i < list.Count; ++i) {
        object di = list[i];
        var value = di as IDictionary<string, object>;
        // A list contains only indexes 0, 1, 2, and so on,
        // with no gaps.
        if (IsList(value)) {
          IList<object> newList = ConvertToList(value);
          list[i] = newList;
          ConvertLists(newList);
        } else if (value != null) {
          // Convert the list's descendents
          // if they are lists
          _ = ConvertLists(value);
        }
      }
    }

    private static IDictionary<string, object> ConvertLists(
      IDictionary<string, object> dict) {
      foreach (string key in new List<string>(dict.Keys)) {
        object di = dict[key];
        var value = di as IDictionary<string, object>;
        // A list contains only indexes 0, 1, 2, and so on,
        // with no gaps.
        if (IsList(value)) {
          IList<object> newList = ConvertToList(value);
          dict[key] = newList;
          ConvertLists(newList);
        } else if (value != null) {
          // Convert the dictionary's descendents
          // if they are lists
          _ = ConvertLists(value);
        }
      }
      return dict;
    }

    public static CBORObject QueryStringToCBOR(string query) {
      return QueryStringToCBOR(query, "&");
    }

    public static IDictionary<string, object> QueryStringToDict(string query) {
      return QueryStringToDict(query, "&");
    }

    private static IDictionary<string, object> QueryStringToDictInternal(
      string query,
      string delimiter) {
      IDictionary<string, object> root = new Dictionary<string, object>();
      foreach (string[] keyvalue in ParseQueryString(query, delimiter)) {
        string[] path = GetKeyPath(keyvalue[0]);
        IDictionary<string, object> leaf = root;
        for (int i = 0; i < path.Length - 1; ++i) {
          if (!leaf.TryGetValue(path[i], out object di)) {
            // node doesn't exist so add it
            IDictionary<string, object> newLeaf = new Dictionary<string, object>();
            if (leaf.ContainsKey(path[i])) {
              throw new InvalidOperationException();
            }
            leaf.Add(path[i], newLeaf);
            leaf = newLeaf;
          } else {
            if (di is IDictionary<string, object> o) {
              leaf = o;
            } else {
              // error, not a dictionary
              throw new InvalidOperationException();
            }
          }
        }
        if (leaf != null) {
          string last = path[^1];
          if (leaf.ContainsKey(last)) {
            throw new InvalidOperationException();
          }
          leaf.Add(last, keyvalue[1]);
        }
      }
      return root;
    }

    public static IDictionary<string, object> QueryStringToDict(string query,
      string delimiter) {
      // Convert array-like dictionaries to ILists
      return ConvertLists(QueryStringToDictInternal(query, delimiter));
    }

    public static CBORObject QueryStringToCBOR(string query,
      string delimiter) {
      // Convert array-like dictionaries to ILists
      return ConvertListsToCBOR(QueryStringToDictInternal(query, delimiter));
    }
  }
}
