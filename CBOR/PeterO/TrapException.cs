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
    /// <summary><para><b>This class is obsolete. It will be replaced by a new version of this
    /// class in a different namespace/package and library, called
    /// <c>PeterO.Numbers.ETrapException
    /// </c>
    /// in the
    /// <a href='https://www.nuget.org/packages/PeterO.Numbers'>
    /// <c>PeterO.Numbers
    /// </c>
    /// </a>
    /// library (in .NET), or
    /// <c>com.upokecenter.numbers.ETrapException
    /// </c>
    /// in the
    /// <a href='https://github.com/peteroupc/numbers-java'>
    /// <c>com.github.peteroupc/numbers
    /// </c>
    /// </a>
    /// artifact (in Java).
    /// </b>
    /// </para>
    /// Exception thrown for arithmetic trap errors.</summary>
  [Obsolete(
  "Use ETrapException from PeterO.Numbers/com.upokecenter.numbers.")]
  public class TrapException : ArithmeticException {
    private ETrapException ete;

    /// <summary>Gets the precision context used during the operation that triggered the
    /// trap. May be null.</summary><value>The precision context used during the operation that triggered the trap.
    /// May be null.
    /// </value>
    public PrecisionContext Context { get {
        return new PrecisionContext(this.ete.Context);
} }

    /// <summary>Gets the defined result of the operation that caused the trap.</summary><value>The defined result of the operation that caused the trap.
    /// </value>
    public Object Result { get {
        return this.ete.Result;
} }

    /// <summary>Gets the flag that specifies the kind of error (PrecisionContext.FlagXXX).
    /// This will only be one flag, such as FlagInexact or FlagSubnormal.</summary><value>The flag that specifies the kind of error (PrecisionContext.FlagXXX). This
    /// will only be one flag, such as FlagInexact or FlagSubnormal.
    /// </value>
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

    /// <summary>Initializes a new instance of the <see cref='PeterO.TrapException'/> class.</summary><param name='flag'>A flag that specifies the kind of error (PrecisionContext.FlagXXX). This
    /// will only be one flag, such as FlagInexact or FlagSubnormal.
    /// </param><param name='ctx'>A context object for arbitrary-precision arithmetic settings.
    /// </param><param name='result'>The desired result of the operation that caused the trap, such as an
    /// <c>ExtendedDecimal
    /// </c>
    /// or
    /// <c>ExtendedFloat
    /// </c>
    /// .
    /// </param>
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
