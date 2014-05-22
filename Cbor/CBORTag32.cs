/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using PeterO;

namespace PeterO.Cbor {
    /// <summary>Description of CBORTag32.</summary>
  internal class CBORTag32 : ICBORTag, ICBORConverter<System.Uri>
  {
    /// <summary>Not documented yet.</summary>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.TextString;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>A CBORObject object. (2).</param>
    /// <returns>A CBORObject object.</returns>
    public CBORObject ValidateObject(CBORObject obj) {
      if (obj.Type != CBORType.TextString) {
        throw new CBORException("URI must be a text string");
      }
      // TODO: Validate URIs
      return obj;
    }

    internal static void AddConverter() {
      CBORObject.AddConverter(typeof(System.Uri), new CBORTag32());
    }

    /// <summary>Converts a UUID to a CBOR object.</summary>
    /// <returns>A CBORObject object.</returns>
    /// <param name='uri'>A System.Uri object.</param>
    public CBORObject ToCBORObject(System.Uri uri) {
      if (uri == null) {
        throw new ArgumentNullException("uri");
      }
      return CBORObject.FromObjectAndTag(uri.ToString(), (int)32);
    }
  }
}
