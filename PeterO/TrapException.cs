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
    /// <include file='docs.xml'
    /// path='docs/doc[@name="T:PeterO.Numbers.ETrapException"]'/>
  public class TrapException : ArithmeticException {
    internal ETrapException ete;

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.ETrapException.Context"]'/>
    public PrecisionContext Context { get {
        return new PrecisionContext(ete.Context);
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.ETrapException.Result"]'/>
    public Object Result { get {
        return ete.Result;
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.ETrapException.Error"]'/>
    public int Error { get {
        return ete.Error;
} }

    private TrapException() : base() {
    }

    internal static TrapException Create(ETrapException ete) {
      var ex = new TrapException();
      ex.ete = ete;
      return ex;
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ETrapException.#ctor(System.Int32,PeterO.Numbers.EContext,System.Object)"]'/>
    public TrapException(int flag, PrecisionContext ctx, Object result) :
      base(String.Empty) {
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
