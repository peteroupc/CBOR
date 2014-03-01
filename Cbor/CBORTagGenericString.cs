/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/28/2014
 * Time: 9:16 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using PeterO;

namespace PeterO.Cbor
{
    /// <summary/>
public class CBORTagGenericString : ICBORTag
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
      if (obj.Type == CBORType.TextString) {
 throw new CBORException("Not a text string");
}
      return obj;
    }
  }
}
