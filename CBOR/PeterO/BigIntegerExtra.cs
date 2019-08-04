/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using PeterO.Numbers;

namespace PeterO {
    /// <xmlbegin id='5'/>
    /// <summary>
    /// <para><b>This class is largely obsolete. It will be replaced by a
    /// new version of this class in a different namespace/package and
    /// library, called <c>PeterO.Numbers.EInteger</c> in the
    /// <a
    ///   href='https://www.nuget.org/packages/PeterO.Numbers'><c>PeterO.Numbers</c></a>
    /// library (in .NET), or <c>com.upokecenter.numbers.EInteger</c> in
    /// the
    /// <a
    ///   href='https://github.com/peteroupc/numbers-java'><c>com.github.peteroupc/numbers</c></a>
    /// artifact (in Java). This new class can be used in the
    /// <c>CBORObject.FromObject(object)</c> method (by including the new
    /// library in your code, among other things).</b></para> An
    /// arbitrary-precision integer.
    /// <para><b>Thread safety:</b> Instances of this class are immutable,
    /// so they are inherently safe for use by multiple threads. Multiple
    /// instances of this object with the same value are interchangeable,
    /// but they should be compared using the "Equals" method rather than
    /// the "==" operator.</para></summary>
  public sealed partial class BigInteger {
    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.BigInteger.Zero"]/*'/>
    [CLSCompliant(false)] [Obsolete(
  "Use EInteger from PeterO.Numbers/com.upokecenter.numbers and the output" +
"\u0020of this class's ToString method.")]
        public static BigInteger Zero {
      get {
        return ValueZeroValue;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.BigInteger.One"]/*'/>
    [CLSCompliant(false)] [Obsolete(
  "Use EInteger from PeterO.Numbers/com.upokecenter.numbers and the output" +
"\u0020of this class's ToString method.")]
        public static BigInteger One {
      get {
        return ValueOneValue;
      }
    }

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.BigInteger.Equals(PeterO.BigInteger)"]/*'/>
        [Obsolete(
  "Use EInteger from PeterO.Numbers/com.upokecenter.numbers and the output" +
"\u0020of this class's ToString method.")]
        public bool Equals(BigInteger other) {
      return this.Equals((object)other);
    }
  }
}
