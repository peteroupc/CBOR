// Written by Peter O.
// Any copyright to this work is released to the Public Domain.
// In case this is not possible, this work is also
// licensed under the Unlicense: https://unlicense.org/

using System;
using System.Collections.Generic;
using System.Text;
using PeterO.Cbor;

namespace PeterO {
  public sealed class QueryStringHelperCBOR {
    private QueryStringHelperCBOR() {
    }
    private static CBORObject ConvertListsToCBOR(IList<object> dict) {
      var cbor = CBORObject.NewArray();
      for (int i = 0; i < dict.Count; ++i) {
        object di = dict[i];
        var value = di as IDictionary<string, object>;
        // A list contains only integer indices,
        // with no gaps.
        if (QueryStringHelper.IsList(value)) {
          IList<object> newList = QueryStringHelper.ConvertToList(value);
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
        // A list contains only integer indices,
        // with no gaps.
        if (QueryStringHelper.IsList(value)) {
          IList<object> newList = QueryStringHelper.ConvertToList(value);
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

    public static CBORObject QueryStringToCBOR(string query) {
      return QueryStringToCBOR(query, "&");
    }
    public static CBORObject QueryStringToCBOR(string query,
      string delimiter) {
      // Convert array-like dictionaries to ILists
      return
        ConvertListsToCBOR(QueryStringHelper.QueryStringToDictInternal(query,
        delimiter));
    }
  }
}
