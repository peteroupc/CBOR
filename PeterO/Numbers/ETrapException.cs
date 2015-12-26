/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Numbers {
    /// <summary>Exception thrown for arithmetic trap errors.</summary>
  internal class ETrapException : ArithmeticException {
    private readonly Object result;
    private readonly EContext ctx;

    /// <summary>Gets the precision context used during the operation that
    /// triggered the trap. May be null.</summary>
    /// <value>The precision context used during the operation that
    /// triggered the trap. May be null.</value>
    public EContext Context {
      get {
        return this.ctx;
      }
    }

    private readonly int error;

    /// <summary>Gets the defined result of the operation that caused the
    /// trap.</summary>
    /// <value>The defined result of the operation that caused the
    /// trap.</value>
    public Object Result {
      get {
        return this.result;
      }
    }

    /// <summary>Gets the flag that specifies the kind of error
    /// (PrecisionContext.FlagXXX). This will only be one flag, such as
    /// FlagInexact or FlagSubnormal.</summary>
    /// <value>The flag that specifies the kind of error
    /// (PrecisionContext.FlagXXX). This will only be one flag, such as
    /// FlagInexact or FlagSubnormal.</value>
    public int Error {
      get {
        return this.error;
      }
    }

    private static string FlagToMessage(int flag) {
      return (flag == EContext.FlagClamped) ? "Clamped" : ((flag ==
        EContext.FlagDivideByZero) ? "DivideByZero" : ((flag ==
        EContext.FlagInexact) ? "Inexact" : ((flag ==
        EContext.FlagInvalid) ? "Invalid" : ((flag ==
        EContext.FlagOverflow) ? "Overflow" : ((flag ==
        EContext.FlagRounded) ? "Rounded" : ((flag ==
        EContext.FlagSubnormal) ? "Subnormal" : ((flag ==
        EContext.FlagUnderflow) ? "Underflow" : "Trap")))))));
    }

    /// <summary>Initializes a new instance of the TrapException
    /// class.</summary>
    /// <param name='flag'>A 32-bit signed integer.</param>
    /// <param name='ctx'>An EContext object.</param>
    /// <param name='result'>An arbitrary object.</param>
    public ETrapException(int flag, EContext ctx, Object result) :
      base(FlagToMessage(flag)) {
      this.error = flag;
      this.ctx = (ctx == null) ? null : ctx.Copy();
      this.result = result;
    }
  }
}
