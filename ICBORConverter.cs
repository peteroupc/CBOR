/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/26/2014
 * Time: 11:48 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PeterO
{
    /// <summary>Description of ICBORConverter.</summary>
  public interface ICBORConverter<T>
  {
    CBORObject ToCBORObject(T obj);
  }
}
