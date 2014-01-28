/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 11/30/2013
 * Time: 10:11 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using PeterO;
using Test;

namespace CBOR
{
    /// <summary>Description of ExtensiveTest.</summary>
  [TestFixture]
  public class ExtensiveTest
  {
    private static string valuePath = "..\\..\\..\\.settings\\test";

    public static void AssertFlags(int expected, int actual, string str) {
      actual &= PrecisionContext.FlagInexact |
        PrecisionContext.FlagUnderflow |
        PrecisionContext.FlagOverflow |
        PrecisionContext.FlagInvalid |
        PrecisionContext.FlagDivideByZero;
      if (expected == actual) {
        return;
      }
      Assert.AreEqual(
        (expected & PrecisionContext.FlagInexact) != 0,
        (actual & PrecisionContext.FlagInexact) != 0,
        "Inexact: " + str);
      Assert.AreEqual(
        (expected & PrecisionContext.FlagOverflow) != 0,
        (actual & PrecisionContext.FlagOverflow) != 0,
        "Overflow: " + str);
      Assert.AreEqual(
        (expected & PrecisionContext.FlagUnderflow) != 0,
        (actual & PrecisionContext.FlagUnderflow) != 0,
        "Underflow: " + str);
      Assert.AreEqual(
        (expected & PrecisionContext.FlagInvalid) != 0,
        (actual & PrecisionContext.FlagInvalid) != 0,
        "Invalid: " + str);
      Assert.AreEqual(
        (expected & PrecisionContext.FlagDivideByZero) != 0,
        (actual & PrecisionContext.FlagDivideByZero) != 0,
        "DivideByZero: " + str);
    }

    private bool Contains(string str, string sub) {
      if (sub.Length == 1) {
        return str.IndexOf(sub[0]) >= 0;
      }
      return str.IndexOf(sub, StringComparison.Ordinal) >= 0;
    }

    private bool StartsWith(string str, string sub) {
      return str.StartsWith(sub, StringComparison.Ordinal);
    }

    private PrecisionContext SetRounding(PrecisionContext ctx, string round) {
      if (round.Equals(">")) {
        ctx = ctx.WithRounding(Rounding.Ceiling);
      }
      if (round.Equals("<")) {
        ctx = ctx.WithRounding(Rounding.Floor);
      }
      if (round.Equals("0")) {
        ctx = ctx.WithRounding(Rounding.Down);
      }
      if (round.Equals("=0")) {
        ctx = ctx.WithRounding(Rounding.HalfEven);
      }
      if (round.Equals("h>") || round.Equals("=^")) {
        ctx = ctx.WithRounding(Rounding.HalfUp);
      }
      if (round.Equals("h<")) {
        ctx = ctx.WithRounding(Rounding.HalfDown);
      }
      return ctx;
    }

    private string DecFracString(ExtendedDecimal df) {
      return "ExtendedDecimal.FromString(\"" + df.ToString() + "\")";
    }

    private static string ConvertOp(string s) {
      if (s.Equals("S")) {
        return "sNaN";
      }
      if (s.Equals("Q") || s.Equals("#")) {
        return "NaN";
      }
      return s;
    }

    private interface IExtendedNumber {
      object Value { get; }

      IExtendedNumber Add(IExtendedNumber b, PrecisionContext ctx);

      IExtendedNumber Subtract(IExtendedNumber b, PrecisionContext ctx);

      IExtendedNumber Multiply(IExtendedNumber b, PrecisionContext ctx);

      IExtendedNumber Divide(IExtendedNumber b, PrecisionContext ctx);

      IExtendedNumber SquareRoot(PrecisionContext ctx);

      IExtendedNumber MultiplyAndAdd(IExtendedNumber b, IExtendedNumber c, PrecisionContext ctx);

      IExtendedNumber MultiplyAndSubtract(IExtendedNumber b, IExtendedNumber c, PrecisionContext ctx);

      bool IsQuietNaN();

      bool IsSignalingNaN();
    }

    private sealed class DecimalNumber : IExtendedNumber {
      private ExtendedDecimal ed;

      public static DecimalNumber Create(ExtendedDecimal dec) {
        DecimalNumber dn = new ExtensiveTest.DecimalNumber();
        dn.ed = dec;
        return dn;
      }

      #region Equals and GetHashCode implementation
      public override bool Equals(object obj) {
        ExtensiveTest.DecimalNumber other = obj as ExtensiveTest.DecimalNumber;
        if (other == null) {
          return false;
        }
        return object.Equals(this.ed, other.ed);
      }

      public override int GetHashCode() {
        int hashCode = 0;
        unchecked {
          if (this.ed != null) {
            hashCode += 1000000007 * this.ed.GetHashCode();
          }
        }
        return hashCode;
      }
      #endregion

      public override string ToString() {
        return this.ed.ToString();
      }

      public object Value {
        get {
          return this.ed;
        }
      }

      private static ExtendedDecimal ToValue(IExtendedNumber en) {
        return (ExtendedDecimal)en.Value;
      }

      public ExtensiveTest.IExtendedNumber Add(ExtensiveTest.IExtendedNumber b, PrecisionContext ctx) {
        return Create(this.ed.Add(ToValue(b), ctx));
      }

      public ExtensiveTest.IExtendedNumber Subtract(ExtensiveTest.IExtendedNumber b, PrecisionContext ctx) {
        return Create(this.ed.Subtract(ToValue(b), ctx));
      }

      public ExtensiveTest.IExtendedNumber Multiply(ExtensiveTest.IExtendedNumber b, PrecisionContext ctx) {
        return Create(this.ed.Multiply(ToValue(b), ctx));
      }

      public ExtensiveTest.IExtendedNumber Divide(ExtensiveTest.IExtendedNumber b, PrecisionContext ctx) {
        return Create(this.ed.Divide(ToValue(b), ctx));
      }

      public ExtensiveTest.IExtendedNumber SquareRoot(PrecisionContext ctx) {
        return Create(this.ed.SquareRoot(ctx));
      }

      public ExtensiveTest.IExtendedNumber MultiplyAndAdd(ExtensiveTest.IExtendedNumber b, ExtensiveTest.IExtendedNumber c, PrecisionContext ctx) {
        return Create(this.ed.MultiplyAndAdd(ToValue(b), ToValue(c), ctx));
      }

      public ExtensiveTest.IExtendedNumber MultiplyAndSubtract(ExtensiveTest.IExtendedNumber b, ExtensiveTest.IExtendedNumber c, PrecisionContext ctx) {
        return Create(this.ed.MultiplyAndSubtract(ToValue(b), ToValue(c), ctx));
      }

      public bool IsQuietNaN() {
        return this.ed != null && ToValue(this).IsQuietNaN();
      }

      public bool IsSignalingNaN() {
        return this.ed != null && ToValue(this).IsSignalingNaN();
      }
    }

    private static PrecisionContext valueBinary32 =
      PrecisionContext.ForPrecisionAndRounding(24, Rounding.HalfEven)
      .WithExponentClamp(true)
      .WithExponentRange(-126, 127);

    private sealed class BinaryNumber : IExtendedNumber {
      private ExtendedFloat ef;

      public static BinaryNumber Create(ExtendedFloat dec) {
        BinaryNumber dn = new ExtensiveTest.BinaryNumber();
        if (dec == null) {
          throw new ArgumentNullException("dec");
        }
        dn.ef = dec;
        return dn;
      }

      public static BinaryNumber FromString(String str) {
        if (str.Equals("NaN")) {
          return Create(ExtendedFloat.NaN);
        }
        if (str.Equals("sNaN")) {
          return Create(ExtendedFloat.SignalingNaN);
        }
        if (str.Equals("+Zero")) {
          return Create(ExtendedFloat.Zero);
        }
        if (str.Equals("0x0")) {
          return Create(ExtendedFloat.Zero);
        }
        if (str.Equals("0x1")) {
          return Create(ExtendedFloat.One);
        }
        if (str.Equals("-Zero")) {
          return Create(ExtendedFloat.NegativeZero);
        }
        if (str.Equals("-Inf")) {
          return Create(ExtendedFloat.NegativeInfinity);
        }
        if (str.Equals("+Inf")) {
          return Create(ExtendedFloat.PositiveInfinity);
        }
        int offset = 0;
        bool negative = false;
        if (str[0] == '+' || str[0] == '-') {
          negative = str[0] == '-';
          ++offset;
        }
        int i = offset;
        int beforeDec = 0;
        int mantissa = 0;
        int exponent = 0;
        bool haveDec = false;
        bool haveBinExp = false;
        bool haveDigits = false;
        for (; i < str.Length; ++i) {
          if (str[i] >= '0' && str[i] <= '9') {
            int thisdigit = (int)(str[i] - '0');
            if ((beforeDec >> 28) != 0) {
              throw new FormatException(str);
            }
            beforeDec <<= 4;
            beforeDec |= thisdigit;
            haveDigits = true;
          } else if (str[i] >= 'A' && str[i] <= 'F') {
            int thisdigit = (int)(str[i] - 'A') + 10;
            if ((beforeDec >> 28) != 0) {
              throw new FormatException(str);
            }
            beforeDec <<= 4;
            beforeDec |= thisdigit;
            haveDigits = true;
          } else if (str[i] >= 'a' && str[i] <= 'f') {
            int thisdigit = (int)(str[i] - 'a') + 10;
            if ((beforeDec >> 28) != 0) {
              throw new FormatException(str);
            }
            beforeDec <<= 4;
            beforeDec |= thisdigit;
            haveDigits = true;
          } else if (str[i] == '.') {
            // Decimal point reached
            haveDec = true;
            ++i;
            break;
          } else if (str[i] == 'P' || str[i] == 'p') {
            // Binary exponent reached
            haveBinExp = true;
            ++i;
            break;
          } else {
            throw new FormatException(str);
          }
        }
        if (!haveDigits) {
          throw new FormatException(str);
        }
        if (haveDec) {
          haveDigits = false;
          int afterDec = 0;
          for (; i < str.Length; ++i) {
            if (str[i] >= '0' && str[i] <= '9') {
              int thisdigit = (int)(str[i] - '0');
              if ((afterDec >> 28) != 0) {
                throw new FormatException(str);
              }
              afterDec <<= 4;
              afterDec |= thisdigit;
              haveDigits = true;
            } else if (str[i] >= 'A' && str[i] <= 'F') {
              int thisdigit = (int)(str[i] - 'A') + 10;
              if ((afterDec >> 28) != 0) {
                throw new FormatException(str);
              }
              afterDec <<= 4;
              afterDec |= thisdigit;
              haveDigits = true;
            } else if (str[i] >= 'a' && str[i] <= 'f') {
              int thisdigit = (int)(str[i] - 'a') + 10;
              if ((afterDec >> 28) != 0) {
                throw new FormatException(str);
              }
              afterDec <<= 4;
              afterDec |= thisdigit;
              haveDigits = true;
            } else if (str[i] == 'P' || str[i] == 'p') {
              // Binary exponent reached
              haveBinExp = true;
              ++i;
              break;
            } else {
              throw new FormatException(str);
            }
          }
          if (!haveDigits) {
            throw new FormatException(str);
          }
          mantissa = (beforeDec << 23) | afterDec;
        } else {
          mantissa = beforeDec;
        }
        if (negative) {
          mantissa = -mantissa;
        }
        if (haveBinExp) {
          haveDigits = false;
          bool negexp = false;
          if (i < str.Length && str[i] == '-') {
            negexp = true;
            ++i;
          }
          for (; i < str.Length; ++i) {
            if (str[i] >= '0' && str[i] <= '9') {
              int thisdigit = (int)(str[i] - '0');
              if ((exponent >> 28) != 0) {
                throw new FormatException(str);
              }
              exponent *= 10;
              exponent += thisdigit;
              haveDigits = true;
            } else {
              throw new FormatException(str);
            }
          }
          if (!haveDigits) {
            throw new FormatException(str);
          }
          if (negexp) {
            exponent = -exponent;
          }
          exponent -= 23;
        }
        if (i != str.Length) {
          throw new FormatException(str);
        }
        // Console.WriteLine("val={0}",Create(ExtendedFloat.Create(
        // (BigInteger)mantissa,  // (BigInteger)exponent
        // )));
        // Console.WriteLine("mant={0} exp={1}", mantissa,exponent);
        return Create(ExtendedFloat.Create(
          (BigInteger)mantissa,
          (BigInteger)exponent));
      }

      #region Equals and GetHashCode implementation
      public override bool Equals(object obj) {
        ExtensiveTest.BinaryNumber other = obj as ExtensiveTest.BinaryNumber;
        if (other == null) {
          return false;
        }
        return this.ef.CompareTo(other.ef) == 0;
      }

      public override int GetHashCode() {
        int hashCode = 0;
        unchecked {
          if (this.ef != null) {
            hashCode += 1000000007 * this.ef.GetHashCode();
          }
        }
        return hashCode;
      }
      #endregion

      public override string ToString() {
        return this.ef.ToString();
      }

      public object Value {
        get {
          return this.ef;
        }
      }

      private static ExtendedFloat ToValue(IExtendedNumber en) {
        return (ExtendedFloat)en.Value;
      }

      public ExtensiveTest.IExtendedNumber Add(ExtensiveTest.IExtendedNumber b, PrecisionContext ctx) {
        return Create(this.ef.Add(ToValue(b), ctx));
      }

      public ExtensiveTest.IExtendedNumber Subtract(ExtensiveTest.IExtendedNumber b, PrecisionContext ctx) {
        return Create(this.ef.Subtract(ToValue(b), ctx));
      }

      public ExtensiveTest.IExtendedNumber Multiply(ExtensiveTest.IExtendedNumber b, PrecisionContext ctx) {
        return Create(this.ef.Multiply(ToValue(b), ctx));
      }

      public ExtensiveTest.IExtendedNumber Divide(ExtensiveTest.IExtendedNumber b, PrecisionContext ctx) {
        return Create(this.ef.Divide(ToValue(b), ctx));
      }

      public ExtensiveTest.IExtendedNumber SquareRoot(PrecisionContext ctx) {
        return Create(this.ef.SquareRoot(ctx));
      }

      public ExtensiveTest.IExtendedNumber MultiplyAndAdd(ExtensiveTest.IExtendedNumber b, ExtensiveTest.IExtendedNumber c, PrecisionContext ctx) {
        return Create(this.ef.MultiplyAndAdd(ToValue(b), ToValue(c), ctx));
      }

      public ExtensiveTest.IExtendedNumber MultiplyAndSubtract(ExtensiveTest.IExtendedNumber b, ExtensiveTest.IExtendedNumber c, PrecisionContext ctx) {
        return Create(this.ef.MultiplyAndSubtract(ToValue(b), ToValue(c), ctx));
      }

      public bool IsQuietNaN() {
        return this.ef != null && ToValue(this).IsQuietNaN();
      }

      public bool IsSignalingNaN() {
        return this.ef != null && ToValue(this).IsSignalingNaN();
      }
    }

    private int ParseLine(string ln) {
      string[] chunks = ln.Split(' ');
      if (chunks.Length < 4) {
        return 0;
      }
      string type = chunks[0];
      PrecisionContext ctx = null;
      bool binaryFP = false;
      string op = String.Empty;
      if (this.StartsWith(type, "d32")) {
        ctx = PrecisionContext.Decimal32;
        op = type.Substring(3);
      }
      if (this.StartsWith(type, "d64")) {
        ctx = PrecisionContext.Decimal64;
        op = type.Substring(3);
      }
      if (this.StartsWith(type, "b32")) {
        ctx = valueBinary32;
        binaryFP = true;
        op = type.Substring(3);
      }
      if (this.StartsWith(type, "d128")) {
        ctx = PrecisionContext.Decimal128;
        op = type.Substring(4);
      }
      if (ctx == null) {
        return 0;
      }
      if (this.Contains(type, "!")) {
        return 0;
      }
       if (op.Contains("cff")) {
        // skip test cases for
        // conversion to another floating point format
        return 0;
      }
      bool squroot = op.Equals("V");
      bool mod = op.Equals("%");
      bool div = op.Equals("/");
      bool fma = op.Equals("*+");
      bool fms = op.Equals("*-");
      bool addop = op.Equals("+");
      bool subop = op.Equals("-");
      bool mul = op.Equals("*");
      string round = chunks[1];
      ctx = this.SetRounding(ctx, round);
      int offset = 0;
      string traps = String.Empty;
      if (this.Contains(chunks[2], "x") || chunks[2].Equals("i") || this.StartsWith(chunks[2], "o")) {
        // traps
        ++offset;
        traps = chunks[2];
      }
      if (this.Contains(traps, "u") || this.Contains(traps, "o")) {
        // skip tests that trap underflow or overflow,
        // the results there may be wrong
        return 0;
      }
      string op1str = ConvertOp(chunks[2 + offset]);
      string op2str = ConvertOp(chunks[3 + offset]);
      string op3str = String.Empty;
      if (chunks.Length <= 4 + offset) {
        return 0;
      }
      string sresult = String.Empty;
      string flags = String.Empty;
      op3str = chunks[4 + offset];
      if (op2str.Equals("->")) {
        if (chunks.Length <= 5 + offset) {
          return 0;
        }
        op2str = String.Empty;
        op3str = String.Empty;
        sresult = chunks[4 + offset];
        flags = chunks[5 + offset];
      } else if (op3str.Equals("->")) {
        if (chunks.Length <= 6 + offset) {
          return 0;
        }
        op3str = String.Empty;
        sresult = chunks[5 + offset];
        flags = chunks[6 + offset];
      } else {
        if (chunks.Length <= 7 + offset) {
          return 0;
        }
        op3str = ConvertOp(op3str);
        sresult = chunks[6 + offset];
        flags = chunks[7 + offset];
      }
      sresult = ConvertOp(sresult);
      IExtendedNumber op1, op2, op3, result;
      if (binaryFP) {
        op1 = BinaryNumber.FromString(op1str);
        op2 = String.IsNullOrEmpty(op2str) ? null :
          BinaryNumber.FromString(op2str);
        op3 = String.IsNullOrEmpty(op3str) ? null :
          BinaryNumber.FromString(op3str);
        result = BinaryNumber.FromString(sresult.ToString());
      } else {
        op1 = DecimalNumber.Create(ExtendedDecimal.FromString(op1str));
        op2 = String.IsNullOrEmpty(op2str) ? null :
          DecimalNumber.Create(ExtendedDecimal.FromString(op2str));
        op3 = String.IsNullOrEmpty(op3str) ? null :
          DecimalNumber.Create(ExtendedDecimal.FromString(op3str));
        result = DecimalNumber.Create(
          ExtendedDecimal.FromString(sresult.ToString()));
      }
      int expectedFlags = 0;
      if (this.Contains(flags, "x")) {
        expectedFlags |= PrecisionContext.FlagInexact;
      }
      if (this.Contains(flags, "u")) {
        expectedFlags |= PrecisionContext.FlagUnderflow;
      }
      if (this.Contains(flags, "o")) {
        expectedFlags |= PrecisionContext.FlagOverflow;
      }
      if (this.Contains(flags, "i")) {
        expectedFlags |= PrecisionContext.FlagInvalid;
      }
      if (this.Contains(flags, "z")) {
        expectedFlags |= PrecisionContext.FlagDivideByZero;
      }
      ctx = ctx.WithBlankFlags();
      if (addop) {
        IExtendedNumber d3 = op1.Add(op2, ctx);
        if (!result.Equals(d3)) {
          Assert.AreEqual(result, d3, ln);
        }
        if (op1.IsQuietNaN() && op2.IsSignalingNaN() && binaryFP) {
          // Don't check flags for binary test cases involving quiet
          // NaN followed by signaling NaN, as the semantics for
          // the invalid operation flag in those cases are different
          // than in the General Decimal Arithmetic Specification
        } else {
          AssertFlags(expectedFlags, ctx.Flags, ln);
        }
      } else if (subop) {
        IExtendedNumber d3 = op1.Subtract(op2, ctx);
        if (!result.Equals(d3)) {
          Assert.AreEqual(result, d3, ln);
        }
        if (op1.IsQuietNaN() && op2.IsSignalingNaN() && binaryFP) {
          // Don't check flags for binary test cases involving quiet
          // NaN followed by signaling NaN, as the semantics for
          // the invalid operation flag in those cases are different
          // than in the General Decimal Arithmetic Specification
        } else {
          AssertFlags(expectedFlags, ctx.Flags, ln);
        }
      } else if (mul) {
        IExtendedNumber d3 = op1.Multiply(op2, ctx);
        if (!result.Equals(d3)) {
          Assert.AreEqual(result, d3, ln);
        }
        if (op1.IsQuietNaN() && op2.IsSignalingNaN() && binaryFP) {
          // Don't check flags for binary test cases involving quiet
          // NaN followed by signaling NaN, as the semantics for
          // the invalid operation flag in those cases are different
          // than in the General Decimal Arithmetic Specification
        } else {
          AssertFlags(expectedFlags, ctx.Flags, ln);
        }
      } else if (div) {
        IExtendedNumber d3 = op1.Divide(op2, ctx);
        if (!result.Equals(d3)) {
          Assert.AreEqual(result, d3, ln);
        }
        if (op1.IsQuietNaN() && op2.IsSignalingNaN() && binaryFP) {
          // Don't check flags for binary test cases involving quiet
          // NaN followed by signaling NaN, as the semantics for
          // the invalid operation flag in those cases are different
          // than in the General Decimal Arithmetic Specification
        } else {
          AssertFlags(expectedFlags, ctx.Flags, ln);
        }
      } else if (squroot) {
        IExtendedNumber d3 = op1.SquareRoot(ctx);
        if (!result.Equals(d3)) {
          Assert.AreEqual(result, d3, ln);
        }
        AssertFlags(expectedFlags, ctx.Flags, ln);
      } else if (fma) {
        IExtendedNumber d3 = op1.MultiplyAndAdd(op2, op3, ctx);
        if (!result.Equals(d3)) {
          Assert.AreEqual(result, d3, ln);
        }
        if (binaryFP && (
            (op1.IsQuietNaN() && (op2.IsSignalingNaN() || op3.IsSignalingNaN())) ||
            (op2.IsQuietNaN() && op3.IsSignalingNaN()))) {
          // Don't check flags for binary test cases involving quiet
          // NaN followed by signaling NaN, as the semantics for
          // the invalid operation flag in those cases are different
          // than in the General Decimal Arithmetic Specification
        } else {
          AssertFlags(expectedFlags, ctx.Flags, ln);
        }
      } else if (fms) {
        IExtendedNumber d3 = op1.MultiplyAndSubtract(op2, op3, ctx);
        if (!result.Equals(d3)) {
          Assert.AreEqual(result, d3, ln);
        }
        if (op1.IsQuietNaN() && op2.IsSignalingNaN() && binaryFP) {
          // Don't check flags for binary test cases involving quiet
          // NaN followed by signaling NaN, as the semantics for
          // the invalid operation flag in those cases are different
          // than in the General Decimal Arithmetic Specification
        } else {
          AssertFlags(expectedFlags, ctx.Flags, ln);
        }
      } else {
       // Console.WriteLine(ln);
      }
      return 0;
    }

    [Test]
    public void TestParser() {
      long failures = 0;
      System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
      if (!Directory.Exists(valuePath)) {
        return;
      }
      sw.Start();
      for (int i = 0; i < 1; ++i) {
        foreach (var p in Directory.GetDirectories(valuePath)) {
          foreach (var f in Directory.GetFiles(p)) {
            Console.WriteLine("//" + f);
            using (StreamReader w = new StreamReader(f)) {
              while (!w.EndOfStream) {
                var ln = w.ReadLine();
                {
                  try {
                    this.ParseLine(ln);
                  } catch (Exception ex) {
                    Console.WriteLine(ln);
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    ++failures;
                    throw;
                  }
                }
              }
            }
          }
        }
      }
      if (failures > 0) {
        Assert.Fail(failures + " failure(s)");
      }
      sw.Stop();
      Console.WriteLine("Time: {0} s", sw.ElapsedMilliseconds / 1000.0);
    }
  }
}
