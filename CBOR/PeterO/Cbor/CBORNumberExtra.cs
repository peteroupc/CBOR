using System;

namespace PeterO.Cbor {
  public sealed partial class CBORNumber {
    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORNumber.op_LessThan(PeterO.Cbor.CBORNumber,PeterO.Cbor.CBORNumber)"]/*'/>
    public static bool operator <(CBORNumber a, CBORNumber b) {
      return a == null ? b != null : a.CompareTo(b) < 0;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORNumber.op_LessThanOrEqual(PeterO.Cbor.CBORNumber,PeterO.Cbor.CBORNumber)"]/*'/>
    public static bool operator <=(CBORNumber a, CBORNumber b) {
      return a == null || a.CompareTo(b) <= 0;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORNumber.op_GreaterThan(PeterO.Cbor.CBORNumber,PeterO.Cbor.CBORNumber)"]/*'/>
    public static bool operator >(CBORNumber a, CBORNumber b) {
      return a != null && a.CompareTo(b) > 0;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORNumber.op_GreaterThanOrEqual(PeterO.Cbor.CBORNumber,PeterO.Cbor.CBORNumber)"]/*'/>
    public static bool operator >=(CBORNumber a, CBORNumber b) {
      return a == null ? b == null : a.CompareTo(b) >= 0;
    }
  }
}
