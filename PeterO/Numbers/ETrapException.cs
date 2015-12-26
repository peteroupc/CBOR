/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Numbers {
    /// <include file='docs.xml' 
    /// path='docs/doc[@name="T:PeterO.Numbers.ETrapException"]'/>
  internal class ETrapException : ArithmeticException {
    private readonly Object result;
    private readonly EContext ctx;

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.Numbers.ETrapException.Context"]'/>
    public EContext Context {
      get {
        return this.ctx;
      }
    }

    private readonly int error;

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.Numbers.ETrapException.Result"]'/>
    public Object Result {
      get {
        return this.result;
      }
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.Numbers.ETrapException.Error"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ETrapException.#ctor(System.Int32,PeterO.Numbers.EContext,System.Object)"]'/>
    public ETrapException(int flag, EContext ctx, Object result) :
      base(FlagToMessage(flag)) {
      this.error = flag;
      this.ctx = (ctx == null) ? null : ctx.Copy();
      this.result = result;
    }
  }
}
