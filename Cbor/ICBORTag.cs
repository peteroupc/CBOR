/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/27/2014
 * Time: 2:03 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PeterO.Cbor
{
    /// <summary>Description of ICBORTag.</summary>
  public interface ICBORTag
  {
    CBORTypeFilter GetTypeFilter();

    // NOTE: Will be passed an object with the corresponding tag
    CBORObject ValidateObject(CBORObject obj);
  }
}
