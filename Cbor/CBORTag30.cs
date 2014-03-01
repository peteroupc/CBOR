/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/27/2014
 * Time: 2:08 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PeterO.Cbor
{
    /// <summary>Description of CBORTag30.</summary>
  public class CBORTag30 : ICBORTag
  {
    /// <summary>Not documented yet.</summary>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter GetTypeFilter() {
      return new CBORTypeFilter().WithArray(
        2,
        CBORTypeFilter.UnsignedInteger.WithNegativeInteger().WithTags(2, 3),
        CBORTypeFilter.UnsignedInteger.WithTags(2));
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>A CBORObject object. (2).</param>
    /// <returns>A CBORObject object.</returns>
    public CBORObject ValidateObject(CBORObject obj) {
      if (obj.Type != CBORType.Array) {
        throw new CBORException("Rational number must be an array");
      }
      if (obj.Count != 2) {
        throw new CBORException("Rational number requires exactly 2 items");
      }
      CBORObject first = obj[0];
      CBORObject second = obj[1];
      if (!first.IsIntegral) {
        throw new CBORException("Rational number requires integer numerator");
      }
      if (!second.IsIntegral) {
        throw new CBORException("Rational number requires integer denominator");
      }
      if (second.Sign <= 0) {
        throw new CBORException("Rational number requires denominator greater than 0");
      }
      BigInteger denom = second.AsBigInteger();
      // NOTE: Discards tags.  See comment in CBORTag2.
      if (denom.Equals(BigInteger.One)) {
        return CBORObject.FromObject(first.AsBigInteger());
      }
      return CBORObject.FromObject(new ExtendedRational(first.AsBigInteger(), denom));
    }
  }
}
