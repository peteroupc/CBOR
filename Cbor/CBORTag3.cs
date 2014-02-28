/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/27/2014
 * Time: 2:06 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PeterO.Cbor
{
    /// <summary/>
public class CBORTag3 : ICBORTag
  {
    /// <summary>Not documented yet.</summary>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.ByteString;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>A CBORObject object. (2).</param>
    /// <returns>A CBORObject object.</returns>
    public CBORObject ValidateObject(CBORObject obj) {
      return CBORTag2.ConvertToBigNum(obj, true);
    }
  }
}
