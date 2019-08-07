/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using PeterO.Numbers;

namespace PeterO {
    /// <include file='../docs.xml'
    /// path='docs/doc[@name="T:PeterO.TrapException"]/*'/>
  [Obsolete(
  "Use ETrapException from PeterO.Numbers/com.upokecenter.numbers.")]
  [System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Design",
    "CA1032",
    "This class is obsolete.")]
  public class TrapException : ArithmeticException {
    private ETrapException ete;

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.TrapException.Context"]/*'/>
    public PrecisionContext Context { get {
        return new PrecisionContext(this.ete.Context);
} }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.TrapException.Result"]/*'/>
    public Object Result { get {
        return this.ete.Result;
} }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.TrapException.Error"]/*'/>
    public int Error { get {
        return this.ete.Error;
} }

    private TrapException() : base() {
    }

    internal static TrapException Create(ETrapException ete) {
      var ex = new TrapException();
      ex.ete = ete;
      return ex;
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.TrapException'/> class.</summary>
    /// <param name='flag'>A flag that specifies the kind of error
    /// (PrecisionContext.FlagXXX). This will only be one flag, such as
    /// FlagInexact or FlagSubnormal.</param>
    /// <param name='ctx'>A context object for arbitrary-precision
    /// arithmetic settings.</param>
    /// <param name='result'>The desired result of the operation that
    /// caused the trap, such as an <c>ExtendedDecimal</c> or
    /// <c>ExtendedFloat</c>.</param>
    public TrapException(int flag, PrecisionContext ctx, Object result)
      : base(String.Empty) {
      Object wrappedResult = result;
      var ed = result as EDecimal;
      var er = result as ERational;
      var ef = result as EFloat;
      if (ed != null) {
        wrappedResult = new ExtendedDecimal(ed);
      }
      if (er != null) {
        wrappedResult = new ExtendedRational(er);
      }
      if (ef != null) {
        wrappedResult = new ExtendedFloat(ef);
      }
      this.ete = new ETrapException(
        flag,
        ctx == null ? null : ctx.Ec,
        wrappedResult);
    }
  }
}
