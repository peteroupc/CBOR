/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using PeterO;
using PeterO.Numbers;

namespace PeterO.Cbor {
  internal class CBORTag5 : ICBORTag
  {
    internal static readonly CBORTypeFilter Filter = new
    CBORTypeFilter().WithArrayExactLength(
  2,
  CBORTypeFilter.UnsignedInteger.WithNegativeInteger(),
  CBORTypeFilter.UnsignedInteger.WithNegativeInteger().WithTags(2, 3));

    internal static readonly CBORTypeFilter ExtendedFilter = new
    CBORTypeFilter().WithArrayExactLength(
  2,
  CBORTypeFilter.UnsignedInteger.WithNegativeInteger().WithTags(2, 3),
  CBORTypeFilter.UnsignedInteger.WithNegativeInteger().WithTags(2, 3));

    public CBORTag5() : this(false) {
    }

    private readonly bool extended;

    public CBORTag5(bool extended) {
      this.extended = extended;
    }

    public CBORTypeFilter GetTypeFilter() {
      return this.extended ? ExtendedFilter : Filter;
    }

    public CBORObject ValidateObject(CBORObject obj) {
      return ConvertToDecimalFrac(obj, false, this.extended);
    }
  }
}
