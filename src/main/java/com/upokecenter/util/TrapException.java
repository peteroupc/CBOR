package com.upokecenter.util;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * Exception thrown for arithmetic trap errors.
     */
  public class TrapException extends ArithmeticException {
private static final long serialVersionUID = 1L;
    private final Object result;
    private final PrecisionContext ctx;

    /**
     * Gets the precision context used during the operation that triggered the
     * trap. May be null.
     * @return The precision context used during the operation that triggered the
     * trap. May be null.
     */
    public final PrecisionContext getContext() {
        return this.ctx;
      }

    private final int error;

    /**
     * Gets the defined result of the operation that caused the trap.
     * @return The defined result of the operation that caused the trap.
     */
    public final Object getResult() {
        return this.result;
      }

    /**
     * Gets the flag that specifies the kind of error (PrecisionContext.FlagXXX).
     * This will only be one flag, such as FlagInexact or FlagSubnormal.
     * @return The flag that specifies the kind of error
     * (PrecisionContext.FlagXXX). This will only be one flag, such as
     * FlagInexact or FlagSubnormal.
     */
    public final int getError() {
        return this.error;
      }

    private static String FlagToMessage(int flag) {
      return (flag == PrecisionContext.FlagClamped) ? "Clamped" : ((flag ==
        PrecisionContext.FlagDivideByZero) ? "DivideByZero" : ((flag ==
        PrecisionContext.FlagInexact) ? "Inexact" : ((flag ==
        PrecisionContext.FlagInvalid) ? "Invalid" : ((flag ==
        PrecisionContext.FlagOverflow) ? "Overflow" : ((flag ==
        PrecisionContext.FlagRounded) ? "Rounded" : ((flag ==
        PrecisionContext.FlagSubnormal) ? "Subnormal" : ((flag ==
        PrecisionContext.FlagUnderflow) ? "Underflow" : "Trap")))))));
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
 super(message, innerException);
    }
  */
  }
