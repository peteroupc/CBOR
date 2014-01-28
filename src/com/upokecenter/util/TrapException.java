package com.upokecenter.util;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

    /**
     * Exception thrown for arithmetic trap errors.
     */
  public class TrapException extends ArithmeticException {
private static final long serialVersionUID=1L;
    private Object result;
    private PrecisionContext ctx;

    /**
     * Gets the precision context used during the operation that triggered
     * the trap. May be null.
     */
    public PrecisionContext getContext() {
        return this.ctx;
      }

    private int error;

    /**
     * Gets the defined result of the operation that caused the trap.
     */
    public Object getResult() {
        return this.result;
      }

    /**
     * Gets the flag that specifies the kind of error (PrecisionContext.FlagXXX).
     * This will only be one flag, such as FlagInexact or FlagSubnormal.
     */
    public int getError() {
        return this.error;
      }

    private static String FlagToMessage(int flag) {
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

    /**
     * Initializes a new instance of the TrapException class.
     * @param flag A 32-bit signed integer.
     * @param ctx A PrecisionContext object.
     * @param result An arbitrary object.
     */
    public TrapException (int flag, PrecisionContext ctx, Object result) {
 super(FlagToMessage(flag));
      this.error = flag;
      this.ctx = (ctx == null) ? null : ctx.Copy();
      this.result = result;
    }
/*
    public TrapException () {
    }

    public TrapException (String message) {
 super(message);
    }

    public TrapException (String message, Throwable innerException) {
 super(message,innerException);
    }
  */
  }
