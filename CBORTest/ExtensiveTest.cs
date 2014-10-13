/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeterO;

namespace CBOR {
  [TestClass]
  public class ExtensiveTest
  {
    public static void AssertFlags(int expected, int actual, string str) {
      actual &= PrecisionContext.FlagInexact | PrecisionContext.FlagUnderflow |
        PrecisionContext.FlagOverflow | PrecisionContext.FlagInvalid |
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
      return (sub.Length == 1) ? (str.IndexOf(sub[0]) >= 0) :
        (str.IndexOf(sub, StringComparison.Ordinal) >= 0);
    }

    private bool StartsWith(string str, string sub) {
      return str.StartsWith(sub, StringComparison.Ordinal);
    }

    private bool EndsWith(string str, string sub) {
      return str.EndsWith(sub, StringComparison.Ordinal);
    }

    private int HexInt(string str) {
      return Int32.Parse(
        str,
        System.Globalization.NumberStyles.AllowHexSpecifier,
        System.Globalization.CultureInfo.InvariantCulture);
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
      return "ExtendedDecimal.FromString(\"" + df + "\")";
    }

    private static string ConvertOp(string s) {
      return s.Equals("S") ? "sNaN" : ((s.Equals("Q") || s.Equals("#")) ?
                "NaN" : s);
    }

    private interface IExtendedNumber {
      object Value { get; }

      IExtendedNumber Add(IExtendedNumber b, PrecisionContext ctx);

      IExtendedNumber Subtract(IExtendedNumber b, PrecisionContext ctx);

      IExtendedNumber Multiply(IExtendedNumber b, PrecisionContext ctx);

      IExtendedNumber Divide(IExtendedNumber b, PrecisionContext ctx);

      IExtendedNumber SquareRoot(PrecisionContext ctx);

      IExtendedNumber MultiplyAndAdd(
IExtendedNumber b,
IExtendedNumber c,
PrecisionContext ctx);

      IExtendedNumber MultiplyAndSubtract(
IExtendedNumber b,
IExtendedNumber c,
PrecisionContext ctx);

      bool IsQuietNaN();

      bool IsSignalingNaN();

      bool IsInfinity();

      bool IsZeroValue();
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
        return (other != null) && object.Equals(this.ed, other.ed);
      }

      public override int GetHashCode() {
        int hashCode = 703582279;
        unchecked {
          if (this.ed != null) {
            hashCode += 703582387 * this.ed.GetHashCode();
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

      public ExtensiveTest.IExtendedNumber Add(
        ExtensiveTest.IExtendedNumber b,
        PrecisionContext ctx) {
        return Create(this.ed.Add(ToValue(b), ctx));
      }

      public ExtensiveTest.IExtendedNumber
        Subtract(ExtensiveTest.IExtendedNumber b, PrecisionContext ctx) {
        return Create(this.ed.Subtract(ToValue(b), ctx));
      }

      public ExtensiveTest.IExtendedNumber
        Multiply(ExtensiveTest.IExtendedNumber b, PrecisionContext ctx) {
        return Create(this.ed.Multiply(ToValue(b), ctx));
      }

      public ExtensiveTest.IExtendedNumber
        Divide(ExtensiveTest.IExtendedNumber b, PrecisionContext ctx) {
        return Create(this.ed.Divide(ToValue(b), ctx));
      }

      public ExtensiveTest.IExtendedNumber SquareRoot(PrecisionContext ctx) {
        return Create(this.ed.SquareRoot(ctx));
      }

      public ExtensiveTest.IExtendedNumber MultiplyAndAdd(
          ExtensiveTest.IExtendedNumber b,
          ExtensiveTest.IExtendedNumber c,
          PrecisionContext ctx) {
        return Create(this.ed.MultiplyAndAdd(ToValue(b), ToValue(c), ctx));
      }

      public ExtensiveTest.IExtendedNumber MultiplyAndSubtract(
          ExtensiveTest.IExtendedNumber b,
          ExtensiveTest.IExtendedNumber c,
          PrecisionContext ctx) {
        return Create(this.ed.MultiplyAndSubtract(ToValue(b), ToValue(c), ctx));
      }

      public bool IsQuietNaN() {
        return this.ed != null && ToValue(this).IsQuietNaN();
      }

      public bool IsSignalingNaN() {
        return this.ed != null && ToValue(this).IsSignalingNaN();
      }

      public bool IsInfinity() {
        return this.ed != null && ToValue(this).IsInfinity();
      }

      public bool IsZeroValue() {
        return this.ed != null && ToValue(this).IsZero;
      }
    }

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
            var thisdigit = (int)(str[i] - '0');
            if ((beforeDec >> 28) != 0) {
              throw new FormatException(str);
            }
            beforeDec <<= 4;
            beforeDec |= thisdigit;
            haveDigits = true;
          } else if (str[i] >= 'A' && str[i] <= 'F') {
            var thisdigit = (int)(str[i] - 'A') + 10;
            if ((beforeDec >> 28) != 0) {
              throw new FormatException(str);
            }
            beforeDec <<= 4;
            beforeDec |= thisdigit;
            haveDigits = true;
          } else if (str[i] >= 'a' && str[i] <= 'f') {
            var thisdigit = (int)(str[i] - 'a') + 10;
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
              var thisdigit = (int)(str[i] - '0');
              if ((afterDec >> 28) != 0) {
                throw new FormatException(str);
              }
              afterDec <<= 4;
              afterDec |= thisdigit;
              haveDigits = true;
            } else if (str[i] >= 'A' && str[i] <= 'F') {
              var thisdigit = (int)(str[i] - 'A') + 10;
              if ((afterDec >> 28) != 0) {
                throw new FormatException(str);
              }
              afterDec <<= 4;
              afterDec |= thisdigit;
              haveDigits = true;
            } else if (str[i] >= 'a' && str[i] <= 'f') {
              var thisdigit = (int)(str[i] - 'a') + 10;
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
              var thisdigit = (int)(str[i] - '0');
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
        // Console.WriteLine("mant=" + mantissa + " exp=" + (exponent));
        return Create(
ExtendedFloat.Create(
(BigInteger)mantissa,
(BigInteger)exponent));
      }

      public static BinaryNumber FromFloatWords(int[] words) {
        if (words == null) {
          throw new ArgumentException("words");
        }
        if (words.Length == 1) {
          bool neg = (words[0] >> 31) != 0;
          int exponent = (words[0] >> 23) & 0xff;
          int mantissa = words[0] & 0x7fffff;
          if (exponent == 255) {
         return (mantissa == 0) ? Create(neg ? ExtendedFloat.NegativeInfinity :
                    ExtendedFloat.PositiveInfinity) : (((mantissa &
                0x00400000) != 0) ? Create(ExtendedFloat.NaN) :
  Create(ExtendedFloat.SignalingNaN));
          }
          if (exponent == 0) {
            if (mantissa == 0) {
              return Create(neg ? ExtendedFloat.NegativeZero :
                            ExtendedFloat.Zero);
            }
            // subnormal
            exponent = -126;
          } else {
            // normal
            exponent -= 127;
            mantissa |= 0x800000;
          }
          var bigmantissa = (BigInteger)mantissa;
          if (neg) {
            bigmantissa = -bigmantissa;
          }
          exponent -= 23;
          return Create(
ExtendedFloat.Create(
bigmantissa,
(BigInteger)exponent));
        }
        if (words.Length == 2) {
          bool neg = (words[0] >> 31) != 0;
          int exponent = (words[0] >> 20) & 0x7ff;
          int mantissa = words[0] & 0xfffff;
          int mantissaNonzero = mantissa | words[1];
          if (exponent == 2047) {
            return (mantissaNonzero == 0) ? Create(neg ?
  ExtendedFloat.NegativeInfinity : ExtendedFloat.PositiveInfinity) :
    (((mantissa & 0x00080000) != 0) ? Create(ExtendedFloat.NaN) :
  Create(ExtendedFloat.SignalingNaN));
          }
          if (exponent == 0) {
            if (mantissaNonzero == 0) {
              return Create(neg ? ExtendedFloat.NegativeZero :
                            ExtendedFloat.Zero);
            }
            // subnormal
            exponent = -1022;
          } else {
            // normal
            exponent -= 1023;
            mantissa |= 0x100000;
          }
          BigInteger bigmantissa = BigInteger.Zero;
          bigmantissa += (BigInteger)((mantissa >> 16) & 0xffff);
          bigmantissa <<= 16;
          bigmantissa += (BigInteger)(mantissa & 0xffff);
          bigmantissa <<= 16;
          bigmantissa += (BigInteger)((words[1] >> 16) & 0xffff);
          bigmantissa <<= 16;
          bigmantissa += (BigInteger)(words[1] & 0xffff);
          if (neg) {
            bigmantissa = -bigmantissa;
          }
          exponent -= 52;
          return Create(
ExtendedFloat.Create(
bigmantissa,
(BigInteger)exponent));
        }
        if (words.Length == 4) {
          bool neg = (words[0] >> 31) != 0;
          int exponent = (words[0] >> 16) & 0x7fff;
          int mantissa = words[0] & 0xffff;
          int mantissaNonzero = mantissa | words[3] | words[1] | words[2];
          if (exponent == 0x7fff) {
            return (mantissaNonzero == 0) ? Create(neg ?
  ExtendedFloat.NegativeInfinity : ExtendedFloat.PositiveInfinity) :
    (((mantissa & 0x00008000) != 0) ? Create(ExtendedFloat.NaN) :
  Create(ExtendedFloat.SignalingNaN));
          }
          if (exponent == 0) {
            if (mantissaNonzero == 0) {
              return Create(neg ? ExtendedFloat.NegativeZero :
                            ExtendedFloat.Zero);
            }
            // subnormal
            exponent = -16382;
          } else {
            // normal
            exponent -= 16383;
            mantissa |= 0x10000;
          }
          BigInteger bigmantissa = BigInteger.Zero;
          bigmantissa += (BigInteger)((mantissa >> 16) & 0xffff);
          bigmantissa <<= 16;
          bigmantissa += (BigInteger)(mantissa & 0xffff);
          bigmantissa <<= 16;
          bigmantissa += (BigInteger)((words[1] >> 16) & 0xffff);
          bigmantissa <<= 16;
          bigmantissa += (BigInteger)(words[1] & 0xffff);
          bigmantissa <<= 16;
          bigmantissa += (BigInteger)((words[2] >> 16) & 0xffff);
          bigmantissa <<= 16;
          bigmantissa += (BigInteger)(words[2] & 0xffff);
          bigmantissa <<= 16;
          bigmantissa += (BigInteger)((words[3] >> 16) & 0xffff);
          bigmantissa <<= 16;
          bigmantissa += (BigInteger)(words[3] & 0xffff);
          if (neg) {
            bigmantissa = -bigmantissa;
          }
          exponent -= 112;
          return Create(
ExtendedFloat.Create(
bigmantissa,
(BigInteger)exponent));
        }
        throw new ArgumentException("words has a bad length");
      }

      #region Equals and GetHashCode implementation
      public override bool Equals(object obj) {
        ExtensiveTest.BinaryNumber other = obj as ExtensiveTest.BinaryNumber;
        return (other != null) && (this.ef.CompareTo(other.ef) == 0);
      }

      public override int GetHashCode() {
        int hashCode = 703582379;
        unchecked {
          if (this.ef != null) {
            hashCode += 703582447 * this.ef.GetHashCode();
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

      public static ExtendedFloat ToValue(IExtendedNumber en) {
        return (ExtendedFloat)en.Value;
      }

      public ExtensiveTest.IExtendedNumber Add(
        ExtensiveTest.IExtendedNumber b,
        PrecisionContext ctx) {
        return Create(this.ef.Add(ToValue(b), ctx));
      }

      public ExtensiveTest.IExtendedNumber
        Subtract(ExtensiveTest.IExtendedNumber b, PrecisionContext ctx) {
        return Create(this.ef.Subtract(ToValue(b), ctx));
      }

      public ExtensiveTest.IExtendedNumber
        Multiply(ExtensiveTest.IExtendedNumber b, PrecisionContext ctx) {
        return Create(this.ef.Multiply(ToValue(b), ctx));
      }

      public ExtensiveTest.IExtendedNumber
        Divide(ExtensiveTest.IExtendedNumber b, PrecisionContext ctx) {
        return Create(this.ef.Divide(ToValue(b), ctx));
      }

      public BinaryNumber Pow(
ExtensiveTest.IExtendedNumber b,
PrecisionContext ctx) {
        return Create(this.ef.Pow(ToValue(b), ctx));
      }

      public ExtensiveTest.IExtendedNumber SquareRoot(PrecisionContext ctx) {
        return Create(this.ef.SquareRoot(ctx));
      }

      public ExtensiveTest.IExtendedNumber MultiplyAndAdd(
          ExtensiveTest.IExtendedNumber b,
          ExtensiveTest.IExtendedNumber c,
          PrecisionContext ctx) {
        return Create(this.ef.MultiplyAndAdd(ToValue(b), ToValue(c), ctx));
      }

      public ExtensiveTest.IExtendedNumber MultiplyAndSubtract(
          ExtensiveTest.IExtendedNumber b,
          ExtensiveTest.IExtendedNumber c,
          PrecisionContext ctx) {
        return Create(this.ef.MultiplyAndSubtract(ToValue(b), ToValue(c), ctx));
      }

      public bool IsNear(IExtendedNumber bn) {
        // ComparePrint(bn);
        ExtendedFloat ulpdiff = ExtendedFloat.Create(
          (BigInteger)2,
          ToValue(this).Exponent);
        return ToValue(this).Subtract(ToValue(bn)).Abs().CompareTo(ulpdiff) <=
          0;
      }

      public void ComparePrint(IExtendedNumber bn) {
        Console.WriteLine(String.Empty + ToValue(this).Mantissa + " man, " +
                          ToValue(bn).Mantissa + " exp");
      }

      public BinaryNumber RoundToIntegralExact(PrecisionContext ctx) {
        return Create(this.ef.RoundToIntegralExact(ctx));
      }

      public BinaryNumber Log(PrecisionContext ctx) {
        return Create(this.ef.Log(ctx));
      }

      public BinaryNumber Remainder(IExtendedNumber bn, PrecisionContext ctx) {
        return Create(this.ef.Remainder(ToValue(bn), ctx));
      }

      public BinaryNumber Exp(PrecisionContext ctx) {
        return Create(this.ef.Exp(ctx));
      }

      public BinaryNumber Abs(PrecisionContext ctx) {
        return Create(this.ef.Abs(ctx));
      }

      public BinaryNumber Log10(PrecisionContext ctx) {
        return Create(this.ef.Log10(ctx));
      }

      public bool IsQuietNaN() {
        return this.ef != null && ToValue(this).IsQuietNaN();
      }

      public bool IsSignalingNaN() {
        return this.ef != null && ToValue(this).IsSignalingNaN();
      }

      public bool IsInfinity() {
        return this.ef != null && ToValue(this).IsInfinity();
      }

      public bool IsZeroValue() {
        return this.ef != null && ToValue(this).IsZero;
      }
    }

    private int ParseLineInput(string ln) {
      string[] chunks = this.Contains(ln, " " + " ") ?
        Regex.Split(ln, " +") : ln.Split(' ');
      if (chunks.Length < 4) {
        return 0;
      }
      string type = chunks[0];
      PrecisionContext ctx = null;
      string op = String.Empty;
      int size = 0;
      if (this.EndsWith(type, "d")) {
        op = type.Substring(0, type.Length - 1);
        ctx = PrecisionContext.Binary64;
        size = 1;
      } else if (this.EndsWith(type, "s")) {
        op = type.Substring(0, type.Length - 1);
        ctx = PrecisionContext.Binary32;
        size = 0;
      } else if (this.EndsWith(type, "q")) {
        op = type.Substring(0, type.Length - 1);
        ctx = PrecisionContext.Binary128;
        size = 2;
      }
      if (ctx == null) {
        return 0;
      }
      string round = chunks[1];
      string flags = chunks[3];
      string compareOp = chunks[2];
      if (round == "m") {
        ctx = ctx.WithRounding(Rounding.Floor);
      } else if (round == "p") {
        ctx = ctx.WithRounding(Rounding.Ceiling);
      } else if (round == "z") {
        ctx = ctx.WithRounding(Rounding.Down);
      } else if (round == "n") {
        ctx = ctx.WithRounding(Rounding.HalfEven);
      } else {
        return 0;
      }
      BinaryNumber op1, op2, result;
      if (size == 0) {
        // single
        if (chunks.Length < 6) {
          return 0;
        }
        op1 = BinaryNumber.FromFloatWords(new[] { this.HexInt(chunks[4]) });
        op2 = BinaryNumber.FromFloatWords(new[] { this.HexInt(chunks[5]) });
        if (chunks.Length == 6 || chunks[6].Length == 0) {
          result = op2;
          op2 = null;
        } else {
          result = BinaryNumber.FromFloatWords(new[] { this.HexInt(chunks[6])
                              });
        }
      } else if (size == 1) {
        // double
        if (chunks.Length < 8) {
          return 0;
        }
        op1 = BinaryNumber.FromFloatWords(new[] { this.HexInt(chunks[4]),
                              this.HexInt(chunks[5]) });
        op2 = BinaryNumber.FromFloatWords(new[] { this.HexInt(chunks[6]),
                              this.HexInt(chunks[7]) });
        if (chunks.Length == 8 || chunks[8].Length == 0) {
          result = op2;
          op2 = null;
          return 0;
        }
        result = BinaryNumber.FromFloatWords(new[] { this.HexInt(chunks[8]),
                              this.HexInt(chunks[9]) });
      } else if (size == 2) {
        // quad
        if (chunks.Length < 12) {
          return 0;
        }
        op1 = BinaryNumber.FromFloatWords(new[] { this.HexInt(chunks[4]),
                this.HexInt(chunks[5]), this.HexInt(chunks[6]),
                              this.HexInt(chunks[7]) });
        op2 = BinaryNumber.FromFloatWords(new[] { this.HexInt(chunks[8]),
                this.HexInt(chunks[9]), this.HexInt(chunks[10]),
                              this.HexInt(chunks[11]) });
        if (chunks.Length == 12 || chunks[12].Length == 0) {
          result = op2;
          op2 = null;
        } else {
          result = BinaryNumber.FromFloatWords(new[] {
                this.HexInt(chunks[12]), this.HexInt(chunks[13]),
                this.HexInt(chunks[14]), this.HexInt(chunks[15]) });
        }
      } else {
        return 0;
      }
      if (compareOp.Equals("uo")) {
        result = BinaryNumber.FromString("NaN");
      }
      int expectedFlags = 0;
      int ignoredFlags = 0;
      if (this.Contains(flags, "?x")) {
        ignoredFlags |= PrecisionContext.FlagInexact;
      } else if (this.Contains(flags, "x")) {
        expectedFlags |= PrecisionContext.FlagInexact;
      }
      if (this.Contains(flags, "u")) {
        expectedFlags |= PrecisionContext.FlagUnderflow;
      }
      if (this.Contains(flags, "o")) {
        expectedFlags |= PrecisionContext.FlagOverflow;
      }
      if (this.Contains(flags, "v")) {
        expectedFlags |= PrecisionContext.FlagInvalid;
      }
      if (this.Contains(flags, "d")) {
        expectedFlags |= PrecisionContext.FlagDivideByZero;
      }

      ctx = ctx.WithBlankFlags();
      if (op.Equals("add")) {
        IExtendedNumber d3 = op1.Add(op2, ctx);
        if (!result.Equals(d3)) {
          Assert.AreEqual(result, d3, ln);
        }
        if (op1.IsQuietNaN() && op2.IsSignalingNaN()) {
          // Don't check flags for binary test cases involving quiet
          // NaN followed by signaling NaN, as the semantics for
          // the invalid operation flag in those cases are different
          // than in the General Decimal Arithmetic Specification
        } else {
          AssertFlags(expectedFlags, ctx.Flags, ln);
        }
      } else if (op.Equals("sub")) {
        IExtendedNumber d3 = op1.Subtract(op2, ctx);
        if (!result.Equals(d3)) {
          Assert.AreEqual(result, d3, ln);
        }
        if (op1.IsQuietNaN() && op2.IsSignalingNaN()) {
          // Don't check flags for binary test cases involving quiet
          // NaN followed by signaling NaN, as the semantics for
          // the invalid operation flag in those cases are different
          // than in the General Decimal Arithmetic Specification
        } else {
          AssertFlags(expectedFlags, ctx.Flags, ln);
        }
      } else if (op.Equals("mul")) {
        IExtendedNumber d3 = op1.Multiply(op2, ctx);
        if (!result.Equals(d3)) {
          Assert.AreEqual(result, d3, ln);
        }
        if (op1.IsQuietNaN() && op2.IsSignalingNaN()) {
          // Don't check flags for binary test cases involving quiet
          // NaN followed by signaling NaN, as the semantics for
          // the invalid operation flag in those cases are different
          // than in the General Decimal Arithmetic Specification
        } else {
          AssertFlags(expectedFlags, ctx.Flags, ln);
        }
      } else if (op.Equals("pow")) {
        BinaryNumber d3 = op1.Pow(op2, ctx);
        // Check for cases that contradict the General Decimal
        // Arithmetic spec
        if (op1.IsZeroValue() && op2.IsZeroValue()) {
          return 0;
        }
        if (((ExtendedFloat)op1.Value).Sign < 0 && op2.IsInfinity()) {
          return 0;
        }
        bool powIntegral = op2.Equals(op2.RoundToIntegralExact(null));
        if (((ExtendedFloat)op1.Value).Sign < 0 &&
            !powIntegral) {
          return 0;
        }
        if ((op1.IsQuietNaN() || op1.IsSignalingNaN()) && op2.IsZeroValue()) {
          return 0;
        }
        if (op2.IsInfinity() && op1.Abs(null).Equals(
          BinaryNumber.FromString("1"))) {
          return 0;
        }
        expectedFlags &= ~PrecisionContext.FlagDivideByZero;
        expectedFlags &= ~PrecisionContext.FlagInexact;
        expectedFlags &= ~PrecisionContext.FlagUnderflow;
        expectedFlags &= ~PrecisionContext.FlagOverflow;
        ignoredFlags |= PrecisionContext.FlagInexact;
        ignoredFlags |= PrecisionContext.FlagUnderflow;
        ignoredFlags |= PrecisionContext.FlagOverflow;
        if (!result.Equals(d3)) {
          if (compareOp.Equals("vn")) {
            if (!result.IsNear(d3)) {
              Assert.AreEqual(result, d3, ln);
            }
          } else if (compareOp.Equals("nb")) {
            if (!result.IsNear(d3)) {
              Assert.AreEqual(result, d3, ln);
            }
          } else {
            Assert.AreEqual(result, d3, ln);
          }
        }
        if (op1.IsQuietNaN() && op2.IsSignalingNaN()) {
          // Don't check flags for binary test cases involving quiet
          // NaN followed by signaling NaN, as the semantics for
          // the invalid operation flag in those cases are different
          // than in the General Decimal Arithmetic Specification
        } else {
          ctx.Flags &= ~ignoredFlags;
          AssertFlags(expectedFlags, ctx.Flags, ln);
        }
      } else if (op.Equals("floor")) {
        ctx = ctx.WithRounding(Rounding.Floor);
        IExtendedNumber d3 = op1.RoundToIntegralExact(ctx);
        if (!result.Equals(d3)) {
          Assert.AreEqual(result, d3, ln);
        }
        AssertFlags(expectedFlags, ctx.Flags, ln);
      } else if (op.Equals("fabs")) {
        // NOTE: Fabs never sets flags
        IExtendedNumber d3 = op1.Abs(ctx);
        if (!result.Equals(d3)) {
          Assert.AreEqual(result, d3, ln);
        }
      } else if (op.Equals("ceil")) {
        ctx = ctx.WithRounding(Rounding.Ceiling);
        IExtendedNumber d3 = op1.RoundToIntegralExact(ctx);
        if (!result.Equals(d3)) {
          Assert.AreEqual(result, d3, ln);
        }
        AssertFlags(expectedFlags, ctx.Flags, ln);
      } else if (op.Equals("sqrt")) {
        IExtendedNumber d3 = op1.SquareRoot(ctx);
        if (!result.Equals(d3)) {
          Assert.AreEqual(result, d3, ln);
        }
        AssertFlags(expectedFlags, ctx.Flags, ln);
      } else if (op.Equals("log")) {
        IExtendedNumber d3 = op1.Log(ctx);
        if (!result.Equals(d3)) {
          if (compareOp.Equals("vn")) {
            if (!result.IsNear(d3)) {
              Assert.AreEqual(result, d3, ln);
            }
          } else if (compareOp.Equals("nb")) {
            if (!result.IsNear(d3)) {
              Assert.AreEqual(result, d3, ln);
            }
          } else {
            Assert.AreEqual(result, d3, ln);
          }
        }
        if (!op1.IsZeroValue()) {
          // ignore flags for zero operand, expects
          // divide by zero flag where general decimal
          // spec doesn't set flags in this case
          AssertFlags(expectedFlags, ctx.Flags, ln);
        }
      } else if (op.Equals("exp")) {
        IExtendedNumber d3 = op1.Exp(ctx);
        if (!result.Equals(d3)) {
          if (compareOp.Equals("vn")) {
            if (!result.IsNear(d3)) {
              Assert.AreEqual(result, d3, ln);
            }
          } else if (compareOp.Equals("nb")) {
            if (!result.IsNear(d3)) {
              Assert.AreEqual(result, d3, ln);
            }
          } else {
            Assert.AreEqual(result, d3, ln);
          }
        }
        AssertFlags(expectedFlags, ctx.Flags, ln);
      } else if (op.Equals("log10")) {
        IExtendedNumber d3 = op1.Log10(ctx);
        if (!result.Equals(d3)) {
          if (compareOp.Equals("vn")) {
            if (!result.IsNear(d3)) {
              Console.WriteLine("op1=..." + op1 + " result=" + result +
                " d3=...." + d3);
              Assert.AreEqual(result, d3, ln);
            }
          } else if (compareOp.Equals("nb")) {
            if (!result.IsNear(d3)) {
              Console.WriteLine("op1=..." + op1 + " result=" + result +
                " d3=...." + d3);
              Assert.AreEqual(result, d3, ln);
            }
          } else {
            Console.WriteLine("op1=..." + op1 + " result=" + result +
                " d3=...." + d3);
            Assert.AreEqual(result, d3, ln);
          }
        }
        expectedFlags &= ~PrecisionContext.FlagInexact;
        ignoredFlags |= PrecisionContext.FlagInexact;
        ctx.Flags &= ~ignoredFlags;
        if (!op1.IsZeroValue()) {
          // ignore flags for zero operand, expects
          // divide by zero flag where general decimal
          // spec doesn't set flags in this case
          AssertFlags(expectedFlags, ctx.Flags, ln);
        }
      } else if (op.Equals("div")) {
        IExtendedNumber d3 = op1.Divide(op2, ctx);
        if (!result.Equals(d3)) {
          Assert.AreEqual(result, d3, ln);
        }
        if (op1.IsQuietNaN() && op2.IsSignalingNaN()) {
          // Don't check flags for binary test cases involving quiet
          // NaN followed by signaling NaN, as the semantics for
          // the invalid operation flag in those cases are different
          // than in the General Decimal Arithmetic Specification
        } else {
          AssertFlags(expectedFlags, ctx.Flags, ln);
        }
      } else if (op.Equals("fmod")) {
        IExtendedNumber d3 = op1.Remainder(op2, ctx);
        if ((ctx.Flags & PrecisionContext.FlagInvalid) != 0 &&
            (expectedFlags & PrecisionContext.FlagInvalid) == 0) {
          // Skip since the quotient may be too high to fit an integer,
          // which triggers an invalid operation under the General
          // Decimal Arithmetic specification
          return 0;
        }
        if (!result.Equals(d3)) {
          Console.WriteLine("op1=..." + op1 + "\nop2=..." + op2 + "\nresult=" +
                result + "\nd3=...." + d3);
          Assert.AreEqual(result, d3, ln);
        }
        if (op1.IsQuietNaN() && op2.IsSignalingNaN()) {
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
        ctx = PrecisionContext.Binary32;
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
      if (this.Contains(chunks[2], "x") || chunks[2].Equals("i") ||
          this.StartsWith(chunks[2], "o")) {
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
        result = BinaryNumber.FromString(sresult);
      } else {
        op1 = DecimalNumber.Create(ExtendedDecimal.FromString(op1str));
        op2 = String.IsNullOrEmpty(op2str) ? null :
          DecimalNumber.Create(ExtendedDecimal.FromString(op2str));
        op3 = String.IsNullOrEmpty(op3str) ? null :
          DecimalNumber.Create(ExtendedDecimal.FromString(op3str));
        result = DecimalNumber.Create(ExtendedDecimal.FromString(sresult));
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

    public static string[] GetTestFiles() {
      return Directory.GetFiles(".");
    }

    [TestMethod]
    public void TestParser() {
      long failures = 0;
      List<string> errors = new List<string>();
      List<string> dirfiles = new List<string>();
      var sw = new System.Diagnostics.Stopwatch();
      sw.Start();
      TextWriter nullWriter = TextWriter.Null;
      TextWriter standardOut = Console.Out;
      int x = 0;
      dirfiles.AddRange(GetTestFiles());
      foreach (var f in dirfiles) {
        Console.WriteLine(f);
        ++x;
        string lowerF = f.ToLowerInvariant();
        bool isinput = lowerF.Contains(".input");
        if (!lowerF.Contains(".input") && !lowerF.Contains(".txt") &&
            !lowerF.Contains(".dectest") && !lowerF.Contains(".fptest")) {
          continue;
        }
        using (var w = new StreamReader(f)) {
          while (!w.EndOfStream) {
            var ln = w.ReadLine();
            {
              try {
                // Console.SetOut(nullWriter);
                if (isinput) {
                  this.ParseLineInput(ln);
                } else {
                  this.ParseLine(ln);
                }
              } catch (Exception ex) {
                errors.Add(ex.Message);
                ++failures;
                try {
                  Console.SetOut(standardOut);
                  if (isinput) {
                    this.ParseLineInput(ln);
                  } else {
                    this.ParseLine(ln);
                  }
                } catch (Exception ex2) {
                  Console.WriteLine(ln);
                  Console.SetOut(nullWriter);
                  Console.WriteLine(ex2.Message);
                }
              }
            }
          }
        }
      }
      Console.SetOut(standardOut);
      sw.Stop();
      Console.WriteLine("Time: " + (sw.ElapsedMilliseconds / 1000.0) + " s");
      if (failures > 0) {
        foreach (string err in errors) {
          Console.WriteLine(err);
        }
        Assert.Fail(failures + " failure(s)");
      }
    }
  }
}
