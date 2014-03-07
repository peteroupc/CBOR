/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/6/2014
 * Time: 10:06 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PeterO.Cbor
{
    /// <summary>Description of CBORTag256.</summary>
  public class CBORTag256 : ICBORTag
  {
    /// <summary>Not documented yet.</summary>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.Any;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>A CBORObject object. (2).</param>
    /// <returns>A CBORObject object.</returns>
    public CBORObject ValidateObject(CBORObject obj) {
      #if DEBUG
      if (obj == null) {
        throw new ArgumentNullException("obj");
      }
      #endif

      return obj;
    }
  }
}
