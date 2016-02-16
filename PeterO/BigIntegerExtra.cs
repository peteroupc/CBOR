/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using PeterO.Numbers;

namespace PeterO {
    /// <include file='../docs.xml'
    /// path='docs/doc[@name="T:PeterO.BigInteger"]/*'/>
  public sealed partial class BigInteger {
    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.BigInteger.Zero"]/*'/>
    [CLSCompliant(false)]
    public static BigInteger Zero {
      get {
        return ValueZeroValue;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.BigInteger.One"]/*'/>
    [CLSCompliant(false)]
    public static BigInteger One {
      get {
        return ValueOneValue;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.Equals(PeterO.BigInteger)"]/*'/>
    public bool Equals(BigInteger other) {
      return this.Equals((object)other);
    }
  }
}
