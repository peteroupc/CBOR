/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;

namespace PeterO
{
    /// <summary>Exception thrown for arithmetic trap errors.</summary>
  public partial class TrapException : ArithmeticException {
    private Object result;
    private PrecisionContext ctx;

    /// <summary>Gets the precision context used during the operation that
    /// triggered the trap. May be null.</summary>
    /// <value>The precision context used during the operation that triggered
    /// the trap. May be null.</value>
    public PrecisionContext Context {
      get {
        return this.ctx;
      }
    }

    private int error;

    /// <summary>Gets the defined result of the operation that caused the
    /// trap.</summary>
    /// <value>The defined result of the operation that caused the trap.</value>
    public Object Result {
      get {
        return this.result;
      }
    }

    /// <summary>Gets the flag that specifies the kind of error (PrecisionContext.FlagXXX).
    /// This will only be one flag, such as FlagInexact or FlagSubnormal.</summary>
    /// <value>The flag that specifies the kind of error (PrecisionContext.FlagXXX).
    /// This will only be one flag, such as FlagInexact or FlagSubnormal.</value>
    public int Error {
      get {
        return this.error;
      }
    }

    private static string FlagToMessage(int flag) {
      if (flag == PrecisionContext.FlagClamped) {
        return "Clamped";
      } else if (flag == PrecisionContext.FlagDivideByZero) {
        return "DivideByZero";
      } else if (flag == PrecisionContext.FlagInexact) {
        return "Inexact";
      } else if (flag == PrecisionContext.FlagInvalid) {
        return "Invalid";
      } else if (flag == PrecisionContext.FlagOverflow) {
        return "Overflow";
      } else if (flag == PrecisionContext.FlagRounded) {
        return "Rounded";
      } else if (flag == PrecisionContext.FlagSubnormal) {
        return "Subnormal";
      } else if (flag == PrecisionContext.FlagUnderflow) {
        return "Underflow";
      }
      return "Trap";
    }

    /// <summary>Initializes a new instance of the TrapException class.</summary>
    /// <param name='flag'>A 32-bit signed integer.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <param name='result'>An arbitrary object.</param>
    public TrapException(int flag, PrecisionContext ctx, Object result)
      : base(FlagToMessage(flag)) {
      this.error = flag;
      this.ctx = (ctx == null) ? null : ctx.Copy();
      this.result = result;
    }
/*
    public TrapException() {
    }

    public TrapException(string message)
      : base(message) {
    }

    public TrapException(string message, Exception innerException)
      : base(message, innerException) {
    }
  */
  }
}
