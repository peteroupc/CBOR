/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using PeterO.Numbers;

namespace PeterO {
    /// <summary>Exception thrown for arithmetic trap errors.</summary>
  public class TrapException : ArithmeticException {
    /// <summary>Gets the precision context used during the operation that
    /// triggered the trap. May be null.</summary>
    /// <value>The precision context used during the operation that
    /// triggered the trap. May be null.</value>
    ETrapException ete;

  /// <summary>Not documented yet.</summary>
    public PrecisionContext Context { get {
        return new PrecisionContext(ete.Context);
} }

    /// <summary>Gets the defined result of the operation that caused the
    /// trap.</summary>
    /// <value>The defined result of the operation that caused the
    /// trap.</value>
    public Object Result { get {
        return ete.Result;
} }

    /// <summary>Gets the flag that specifies the kind of error
    /// (PrecisionContext.FlagXXX). This will only be one flag, such as
    /// FlagInexact or FlagSubnormal.</summary>
    /// <value>The flag that specifies the kind of error
    /// (PrecisionContext.FlagXXX). This will only be one flag, such as
    /// FlagInexact or FlagSubnormal.</value>
    public int Error { get {
        return ete.Error;
} }

    /// <summary>Initializes a new instance of the TrapException
    /// class.</summary>
    /// <param name='flag'>A 32-bit signed integer.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <param name='result'>An arbitrary object.</param>
    public TrapException(int flag, PrecisionContext ctx, Object result) {
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
      ete = new ETrapException(flag, ctx == null ? null : ctx.ec,
        wrappedResult);
    }
  }
}
