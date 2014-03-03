/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/26/2014
 * Time: 11:48 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PeterO.Cbor
{
    /// <summary>Description of ICBORConverter.</summary>
    /// <typeparam name='T'>Type to convert to a CBOR object.</typeparam>
  public interface ICBORConverter<T>
  {
    CBORObject ToCBORObject(T obj);
  }
}
