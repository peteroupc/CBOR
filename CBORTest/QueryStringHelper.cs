// Written by Peter O.
// Any copyright to this work is released to the Public Domain.
// https://creativecommons.org/publicdomain/zero/1.0/

using PeterO.Cbor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public sealed class QueryStringHelper
    {
        private QueryStringHelper()
        {
        }
        private static string[] SplitAt(string s, string delimiter)
        {
            if (delimiter == null || delimiter.Length == 0)
            {
                throw new ArgumentException();
            }
            if (s == null || s.Length == 0)
            {
                return new string[] { string.Empty };
            }
            int index = 0;
            bool first = true;
            List<string> strings = null;
            int delimLength = delimiter.Length;
            while (true)
            {
                int index2 = s.IndexOf(delimiter, index, StringComparison.Ordinal);
                if (index2 < 0)
                {
                    if (first)
                    {
                        return new string[] { s };
                    }
                    strings.Add(s[index..]);
                    break;
                }
                else
                {
                    if (first)
                    {
                        strings = new List<string>();
                        first = false;
                    }
                    string newstr = s[index..index2];
                    strings.Add(newstr);
                    index = index2 + delimLength;
                }
            }
            return strings.ToArray();
        }

        private static int ToHexNumber(int c)
        {
            if (c is >= 'A' and <= 'Z')
            {
                return 10 + c - 'A';
            }
            else if (c is >= 'a' and <= 'z')
            {
                return 10 + c - 'a';
            }
            else
            {
                return (c is >= '0' and <= '9') ? (c - '0') : (-1);
            }
        }
        private static string PercentDecodeUTF8(string str)
        {
            int len = str.Length;
            bool percent = false;
            for (int i = 0; i < len; ++i)
            {
                char c = str[i];
                if (c == '%')
                {
                    percent = true;
                }
                else if (c >= 0x80)
                {
                    // Non-ASCII characters not allowed
                    throw new InvalidOperationException();
                }
            }
            if (!percent)
            {
                return str; // return early if there are no percent decodings
            }
            int cp = 0;
            int bytesSeen = 0;
            int bytesNeeded = 0;
            int lower = 0x80;
            int upper = 0xbf;
            int markedPos = -1;
            StringBuilder retString = new();
            for (int i = 0; i < len; ++i)
            {
                int c = str[i];
                if (c == '%')
                {
                    if (i + 2 < len)
                    {
                        int a = ToHexNumber(str[i + 1]);
                        int b = ToHexNumber(str[i + 2]);
                        if (a >= 0 && b >= 0)
                        {
                            b = (byte)((a * 16) + b);
                            i += 2;
                            // b now contains the byte read
                            if (bytesNeeded == 0)
                            {
                                // this is the lead byte
                                if (b < 0x80)
                                {
                                    retString.Append((char)b);
                                    continue;
                                }
                                else if (b is >= 0xc2 and <= 0xdf)
                                {
                                    markedPos = i;
                                    bytesNeeded = 1;
                                    cp = b - 0xc0;
                                }
                                else if (b is >= 0xe0 and <= 0xef)
                                {
                                    markedPos = i;
                                    lower = (b == 0xe0) ? 0xa0 : 0x80;
                                    upper = (b == 0xed) ? 0x9f : 0xbf;
                                    bytesNeeded = 2;
                                    cp = b - 0xe0;
                                }
                                else if (b is >= 0xf0 and <= 0xf4)
                                {
                                    markedPos = i;
                                    lower = (b == 0xf0) ? 0x90 : 0x80;
                                    upper = (b == 0xf4) ? 0x8f : 0xbf;
                                    bytesNeeded = 3;
                                    cp = b - 0xf0;
                                }
                                else
                                {
                                    // illegal byte in UTF-8
                                    throw new InvalidOperationException();
                                }
                                cp <<= 6 * bytesNeeded;
                                continue;
                            }
                            else
                            {
                                // this is a second or further byte
                                if (b < lower || b > upper)
                                {
                                    // illegal trailing byte
                                    throw new InvalidOperationException();
                                }
                                // reset lower and upper for the third
                                // and further bytes
                                lower = 0x80;
                                upper = 0xbf;
                                ++bytesSeen;
                                cp += (b - 0x80) << (6 * (bytesNeeded - bytesSeen));
                                markedPos = i;
                                if (bytesSeen != bytesNeeded)
                                {
                                    // continue if not all bytes needed
                                    // were read yet
                                    continue;
                                }
                                int ret = cp;
                                cp = 0;
                                bytesSeen = 0;
                                bytesNeeded = 0;
                                // append the Unicode character
                                if (ret <= 0xffff)
                                {
                                    {
                                        retString.Append((char)ret);
                                    }
                                }
                                else
                                {
                                    retString.Append((char)((((ret - 0x10000) >> 10) &
                                          0x3ff) | 0xd800));
                                    retString.Append((char)(((ret - 0x10000) & 0x3ff) |
                                        0xdc00));
                                }
                                continue;
                            }
                        }
                    }
                }
                if (bytesNeeded > 0)
                {
                    // we expected further bytes here,
                    // so throw an exception
                    throw new InvalidOperationException();
                }
                // append the code point as is (we already
                // checked for ASCII characters so this will
                // be simple
                retString.Append((char)(c & 0xff));
            }
            if (bytesNeeded > 0)
            {
                // we expected further bytes here,
                // so throw an exception
                throw new InvalidOperationException();
            }
            return retString.ToString();
        }
        public static IList<string[]> ParseQueryString(
          string input)
        {
            return ParseQueryString(input, null);
        }

        public static IList<string[]> ParseQueryString(
          string input,
          string delimiter)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            delimiter ??= "&";
            // Check input for non-ASCII characters
            for (int i = 0; i < input.Length; ++i)
            {
                if (input[i] > 0x7f)
                {
                    throw new ArgumentException("input contains a non-ASCII character");
                }
            }
            // split on delimiter
            string[] strings = SplitAt(input, delimiter);
            List<string[]> pairs = new();
            foreach (string str in strings)
            {
                if (str.Length == 0)
                {
                    continue;
                }
                // split on key
                int index = str.IndexOf('=');
                string name = str;
                string value = string.Empty; // value is empty if there is no key
                if (index >= 0)
                {
                    name = str[..index];
                    value = str[(index + 1)..];
                }
                name = name.Replace('+', ' ');
                value = value.Replace('+', ' ');
                string[] pair = new string[] { name, value };
                pairs.Add(pair);
            }
            foreach (string[] pair in pairs)
            {
                // percent decode the key and value if necessary
                pair[0] = PercentDecodeUTF8(pair[0]);
                pair[1] = PercentDecodeUTF8(pair[1]);
            }
            return pairs;
        }

        private static string[] GetKeyPath(string s)
        {
            int index = s.IndexOf('[');
            if (index < 0)
            { // start bracket not found
                return new string[] { s };
            }
            List<string> path = new()
            {
                s[..index]
            };
            ++index; // move to after the bracket
            while (true)
            {
                int endBracket = s.IndexOf(']', index);
                if (endBracket < 0)
                { // end bracket not found
                    path.Add(s[index..]);
                    break;
                }
                path.Add(s[index..endBracket]);
                index = endBracket + 1; // move to after the end bracket
                index = s.IndexOf('[', index);
                if (index < 0)
                { // start bracket not found
                    break;
                }
                ++index; // move to after the start bracket
            }
            return path.ToArray();
        }

        private static readonly string Digits = "0123456789";

        public static string IntToString(int value)
        {
            if (value == 0)
            {
                return "0";
            }
            if (value == int.MinValue)
            {
                return "-2147483648";
            }
            bool neg = value < 0;
            if (neg)
            {
                value = -value;
            }
            char[] chars;
            int count;
            if (value < 100000)
            {
                if (neg)
                {
                    chars = new char[6];
                    count = 5;
                }
                else
                {
                    chars = new char[5];
                    count = 4;
                }
                while (value > 9)
                {
                    int intdivvalue = unchecked((((value >> 1) * 52429) >> 18) & 16383);
                    char digit = Digits[value - (intdivvalue * 10)];
                    chars[count--] = digit;
                    value = intdivvalue;
                }
                if (value != 0)
                {
                    chars[count--] = Digits[value];
                }
                if (neg)
                {
                    chars[count] = '-';
                }
                else
                {
                    ++count;
                }
                return new string(chars, count, chars.Length - count);
            }
            chars = new char[12];
            count = 11;
            while (value >= 163840)
            {
                int intdivvalue = value / 10;
                char digit = Digits[value - (intdivvalue * 10)];
                chars[count--] = digit;
                value = intdivvalue;
            }
            while (value > 9)
            {
                int intdivvalue = unchecked((((value >> 1) * 52429) >> 18) & 16383);
                char digit = Digits[value - (intdivvalue * 10)];
                chars[count--] = digit;
                value = intdivvalue;
            }
            if (value != 0)
            {
                chars[count--] = Digits[value];
            }
            if (neg)
            {
                chars[count] = '-';
            }
            else
            {
                ++count;
            }
            return new string(chars, count, 12 - count);
        }

        private static bool IsList(IDictionary<string, object> dict)
        {
            if (dict == null)
            {
                return false;
            }
            int index = 0;
            int count = dict.Count;
            if (count == 0)
            {
                return false;
            }
            while (true)
            {
                if (index == count)
                {
                    return true;
                }
                string indexString = IntToString(index);
                if (!dict.ContainsKey(indexString))
                {
                    return false;
                }
                ++index;
            }
        }

        private static IList<object> ConvertToList(IDictionary<string, object>
          dict)
        {
            List<object> ret = new();
            int index = 0;
            int count = dict.Count;
            while (index < count)
            {
                string indexString = IntToString(index);
                if (!dict.ContainsKey(indexString))
                {
                    throw new InvalidOperationException();
                }
                ret.Add(dict[indexString]);
                ++index;
            }
            return ret;
        }

        private static CBORObject ConvertListsToCBOR(IList<object> dict)
        {
            CBORObject cbor = CBORObject.NewArray();
            for (int i = 0; i < dict.Count; ++i)
            {
                object di = dict[i];
                IDictionary<string, object> value = di as IDictionary<string, object>;
                // A list contains only indexes 0, 1, 2, and so on,
                // with no gaps.
                if (IsList(value))
                {
                    IList<object> newList = ConvertToList(value);
                    cbor.Add(ConvertListsToCBOR(newList));
                }
                else if (value != null)
                {
                    // Convert the list's descendents
                    // if they are lists
                    cbor.Add(ConvertListsToCBOR(value));
                }
                else
                {
                    cbor.Add(dict[i]);
                }
            }
            return cbor;
        }

        private static CBORObject ConvertListsToCBOR(IDictionary<string, object>
          dict)
        {
            CBORObject cbor = CBORObject.NewMap();
            foreach (string key in new List<string>(dict.Keys))
            {
                object di = dict[key];
                IDictionary<string, object> value = di as IDictionary<string, object>;
                // A list contains only indexes 0, 1, 2, and so on,
                // with no gaps.
                if (IsList(value))
                {
                    IList<object> newList = ConvertToList(value);
                    cbor.Add(key, ConvertListsToCBOR(newList));
                }
                else if (value != null)
                {
                    // Convert the dictionary's descendents
                    // if they are lists
                    cbor.Add(key, ConvertListsToCBOR(value));
                }
                else
                {
                    cbor.Add(key, dict[key]);
                }
            }
            return cbor;
        }

        private static void ConvertLists(IList<object> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                object di = list[i];
                IDictionary<string, object> value = di as IDictionary<string, object>;
                // A list contains only indexes 0, 1, 2, and so on,
                // with no gaps.
                if (IsList(value))
                {
                    IList<object> newList = ConvertToList(value);
                    list[i] = newList;
                    ConvertLists(newList);
                }
                else if (value != null)
                {
                    // Convert the list's descendents
                    // if they are lists
                    ConvertLists(value);
                }
            }
        }

        private static IDictionary<string, object> ConvertLists(
          IDictionary<string, object> dict)
        {
            foreach (string key in new List<string>(dict.Keys))
            {
                object di = dict[key];
                IDictionary<string, object> value = di as IDictionary<string, object>;
                // A list contains only indexes 0, 1, 2, and so on,
                // with no gaps.
                if (IsList(value))
                {
                    IList<object> newList = ConvertToList(value);
                    dict[key] = newList;
                    ConvertLists(newList);
                }
                else if (value != null)
                {
                    // Convert the dictionary's descendents
                    // if they are lists
                    ConvertLists(value);
                }
            }
            return dict;
        }

        public static CBORObject QueryStringToCBOR(string query)
        {
            return QueryStringToCBOR(query, "&");
        }

        public static IDictionary<string, object> QueryStringToDict(string query)
        {
            return QueryStringToDict(query, "&");
        }

        private static IDictionary<string, object> QueryStringToDictInternal(
          string query,
          string delimiter)
        {
            IDictionary<string, object> root = new Dictionary<string, object>();
            foreach (string[] keyvalue in ParseQueryString(query, delimiter))
            {
                string[] path = GetKeyPath(keyvalue[0]);
                IDictionary<string, object> leaf = root;
                for (int i = 0; i < path.Length - 1; ++i)
                {
                    if (!leaf.ContainsKey(path[i]))
                    {
                        // node doesn't exist so add it
                        IDictionary<string, object> newLeaf = new Dictionary<string, object>();
                        if (leaf.ContainsKey(path[i]))
                        {
                            throw new InvalidOperationException();
                        }
                        leaf.Add(path[i], newLeaf);
                        leaf = newLeaf;
                    }
                    else
                    {
                        object di = leaf[path[i]];
                        if (di is IDictionary<string, object> o)
                        {
                            leaf = o;
                        }
                        else
                        {
                            // error, not a dictionary
                            throw new InvalidOperationException();
                        }
                    }
                }
                if (leaf != null)
                {
                    if (leaf.ContainsKey(path[^1]))
                    {
                        throw new InvalidOperationException();
                    }
                    leaf.Add(path[^1], keyvalue[1]);
                }
            }
            return root;
        }

        public static IDictionary<string, object> QueryStringToDict(string query,
          string delimiter)
        {
            // Convert array-like dictionaries to ILists
            return ConvertLists(QueryStringToDictInternal(query, delimiter));
        }

        public static CBORObject QueryStringToCBOR(string query,
          string delimiter)
        {
            // Convert array-like dictionaries to ILists
            return ConvertListsToCBOR(QueryStringToDictInternal(query, delimiter));
        }
    }
}