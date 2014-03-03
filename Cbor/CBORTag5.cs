/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/27/2014
 * Time: 7:43 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PeterO.Cbor
{
    /// <summary>Description of CBORTag5.</summary>
    public class CBORTag5 : ICBORTag
  {
    private static CBORTypeFilter valueFilter = new CBORTypeFilter().WithArray(
      2,
      CBORTypeFilter.UnsignedInteger.WithNegativeInteger(),
      CBORTypeFilter.UnsignedInteger.WithNegativeInteger().WithTags(2, 3));

    /// <summary>Not documented yet.</summary>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter GetTypeFilter() {
      return valueFilter;
    }

    internal static CBORObject ConvertToDecimalFrac(CBORObject o, bool isDecimal) {
      if (o.Type != CBORType.Array) {
        throw new CBORException("Big fraction must be an array");
      }
      if (o.Count != 2) {
        throw new CBORException("Big fraction requires exactly 2 items");
      }
      if (!o[0].IsIntegral) {
        throw new CBORException("Exponent is not an integer");
      }
      if (!o[1].IsIntegral) {
        throw new CBORException("Mantissa is not an integer");
      }
      BigInteger exponent = o[0].AsBigInteger();
      BigInteger mantissa = o[1].AsBigInteger();
      if (exponent.bitLength() > 64) {
        throw new CBORException("Exponent is too big");
      }
      if (exponent.IsZero) {
        // Exponent is 0, so return mantissa instead
        return o[1];
      }
      // NOTE: Discards tags. See comment in CBORTag2.
      if (isDecimal) {
        return CBORObject.FromObject(ExtendedDecimal.Create(mantissa, exponent));
      } else {
        return CBORObject.FromObject(ExtendedFloat.Create(mantissa, exponent));
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>A CBORObject object. (2).</param>
    /// <returns>A CBORObject object.</returns>
    public CBORObject ValidateObject(CBORObject obj) {
      return ConvertToDecimalFrac(obj, false);
    }
  }
}
