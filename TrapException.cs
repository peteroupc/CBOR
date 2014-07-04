/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;

namespace PeterO {
    /// <summary>Exception thrown for arithmetic trap errors.</summary>
  public class TrapException : ArithmeticException {
    private readonly Object result;
    private readonly PrecisionContext ctx;

    /// <summary>Gets the precision context used during the operation that triggered
    /// the trap. May be null.</summary>
    /// <value>The precision context used during the operation that triggered the
    /// trap. May be null.</value>
    public PrecisionContext Context {
      get {
        return this.ctx;
      }
    }

    private readonly int error;

    /// <summary>Gets the defined result of the operation that caused the
    /// trap.</summary>
    /// <value>The defined result of the operation that caused the trap.</value>
    public Object Result {
      get {
        return this.result;
      }
    }

    /// <summary>Gets the flag that specifies the kind of error
    /// (PrecisionContext.FlagXXX). This will only be one flag, such as FlagInexact
    /// or FlagSubnormal.</summary>
    /// <value>The flag that specifies the kind of error (PrecisionContext.FlagXXX).
    /// This will only be one flag, such as FlagInexact or FlagSubnormal.</value>
    public int Error {
      get {
        return this.error;
      }
    }

    private static string FlagToMessage(int flag) {
      return (flag == PrecisionContext.FlagClamped) ? "Clamped" : ((flag ==
        PrecisionContext.FlagDivideByZero) ? "DivideByZero" : ((flag ==
        PrecisionContext.FlagInexact) ? "Inexact" : ((flag ==
        PrecisionContext.FlagInvalid) ? "Invalid" : ((flag ==
        PrecisionContext.FlagOverflow) ? "Overflow" : ((flag ==
        PrecisionContext.FlagRounded) ? "Rounded" : ((flag ==
        PrecisionContext.FlagSubnormal) ? "Subnormal" : ((flag ==
        PrecisionContext.FlagUnderflow) ? "Underflow" : "Trap")))))));
    }

    /// <summary>Initializes a new instance of the TrapException class.</summary>
    /// <param name='flag'>A 32-bit signed integer.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <param name='result'>An arbitrary object.</param>
    public TrapException(int flag, PrecisionContext ctx, Object result) :
      base(FlagToMessage(flag)) {
      this.error = flag;
      this.ctx = (ctx == null) ? null : ctx.Copy();
      this.result = result;
    }
/*
    public TrapException() {
    }

    public TrapException(string message):
      base(message) {
    }

    public TrapException(string message, Exception innerException):
      base(message, innerException) {
    }
  */
  }
}
