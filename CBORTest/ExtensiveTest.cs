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
using PeterO;
using NUnit.Framework;
using System.Text.RegularExpressions;
using Test;
namespace CBOR
{
  /// <summary>
  /// Description of ExtensiveTest.
  /// </summary>
  ///
  [TestFixture]
  public class ExtensiveTest
  {

    static string Path="..\\..\\..\\.settings\\test";
    public static void AssertFlags(int expected, int actual, string str){
      actual&=(PrecisionContext.FlagInexact|
               PrecisionContext.FlagUnderflow|
               PrecisionContext.FlagOverflow|
               PrecisionContext.FlagInvalid|
               PrecisionContext.FlagDivideByZero);
      if(expected==actual)return;
      Assert.AreEqual((expected&PrecisionContext.FlagInexact)!=0,
                      (actual&PrecisionContext.FlagInexact)!=0,"Inexact: "+str);
      Assert.AreEqual((expected&PrecisionContext.FlagOverflow)!=0,
                      (actual&PrecisionContext.FlagOverflow)!=0,"Overflow: "+str);
      Assert.AreEqual((expected&PrecisionContext.FlagUnderflow)!=0,
                      (actual&PrecisionContext.FlagUnderflow)!=0,"Underflow: "+str);
      Assert.AreEqual((expected&PrecisionContext.FlagInvalid)!=0,
                      (actual&PrecisionContext.FlagInvalid)!=0,"Invalid: "+str);
      Assert.AreEqual((expected&PrecisionContext.FlagDivideByZero)!=0,
                      (actual&PrecisionContext.FlagDivideByZero)!=0,"DivideByZero: "+str);
    }

    private bool Contains(string str, string sub){
      if(sub.Length==1)return str.IndexOf(sub[0])>=0;
      return str.IndexOf(sub,StringComparison.Ordinal)>=0;
    }
    private bool StartsWith(string str, string sub){
      return str.StartsWith(sub,StringComparison.Ordinal);
    }

    private PrecisionContext SetRounding(PrecisionContext ctx, string round){
      if(round.Equals(">"))ctx=ctx.WithRounding(Rounding.Ceiling);
      if(round.Equals("<"))ctx=ctx.WithRounding(Rounding.Floor);
      if(round.Equals("0"))ctx=ctx.WithRounding(Rounding.Down);
      if(round.Equals("=0"))ctx=ctx.WithRounding(Rounding.HalfEven);
      if(round.Equals("h>"))ctx=ctx.WithRounding(Rounding.HalfUp);
      if(round.Equals("h<"))ctx=ctx.WithRounding(Rounding.HalfDown);
      return ctx;
    }

    private string DecFracString(ExtendedDecimal df){
      return "ExtendedDecimal.FromString(\""+df.ToString()+"\")";
    }

    private string RoundingString(PrecisionContext ctx, string round){
      if(round.Equals(">"))return ".WithRounding(Rounding.Ceiling)";
      if(round.Equals("<"))return ".WithRounding(Rounding.Floor)";
      if(round.Equals("0"))return ".WithRounding(Rounding.Down)";
      if(round.Equals("=0"))return ".WithRounding(Rounding.HalfEven)";
      if(round.Equals("h>"))return ".WithRounding(Rounding.HalfUp)";
      if(round.Equals("h<"))return ".WithRounding(Rounding.HalfDown)";
      if(round.Equals("+"))return ".WithRounding(Rounding.Up)";
      if(round.Equals("u"))return ".WithRounding(Rounding.Unnecessary)";
      return "";
    }

    private static string ConvertOp(string s){
      if(s.Equals("S"))return "sNaN";
      if(s.Equals("Q"))return "NaN";
      return s;
    }

    private int ParseLine(string ln){
      string[] chunks=ln.Split(' ');
      if(chunks.Length<4)
        return 0;
      string type=chunks[0];
      PrecisionContext ctx=(StartsWith(type,"d64")) ?
        PrecisionContext.Decimal64 :
        PrecisionContext.Decimal128;
      if(StartsWith(type,"d32"))
        ctx=PrecisionContext.Decimal32;
      if(Contains(type,"!"))
        return 0;
      if(!Contains(type,"d"))
        return 0;
      bool squroot=Contains(type,"V");
      bool mod=Contains(type,"%");
      bool div=Contains(type,"/");
      bool fma=Contains(type,"*+");
      bool fms=Contains(type,"*-");
      bool addop=!fma && Contains(type,"+");
      bool subop=!fms && Contains(type,"-");
      bool mul=!fma && !fms && Contains(type,"*");
      string round=chunks[1];
      ctx=SetRounding(ctx,round);
      string ctxstring="PrecisionContext.Unlimited";
      if(StartsWith(type,"d64"))ctxstring="PrecisionContext.Decimal64";
      if(StartsWith(type,"d128"))ctxstring="PrecisionContext.Decimal128";
      ctxstring+=RoundingString(ctx,round);
      string op1str=ConvertOp(chunks[2]);
      string op2str=ConvertOp(chunks[3]);
      string op3str=String.Empty;
      if(chunks.Length<=4)
        return 0;
      string sresult=String.Empty;
      string flags=String.Empty;
      op3str=chunks[4];
      if(op2str.Equals("->")){
        if(chunks.Length<=5)
          return 0;
        op2str=String.Empty;
        op3str=String.Empty;
        sresult=chunks[4];
        flags=chunks[5];
      } else if(op3str.Equals("->")){
        if(chunks.Length<=6)
          return 0;
        op3str=String.Empty;
        sresult=chunks[5];
        flags=chunks[6];
      } else {
        if(chunks.Length<=7)
          return 0;
        op3str=ConvertOp(op3str);
        sresult=chunks[6];
        flags=chunks[7];
      }
      sresult=ConvertOp(sresult);
      ExtendedDecimal op1=ExtendedDecimal.FromString(op1str);
      ExtendedDecimal op2=(String.IsNullOrEmpty(op2str)) ? null : ExtendedDecimal.FromString(op2str);
      ExtendedDecimal op3=(String.IsNullOrEmpty(op3str)) ? null : ExtendedDecimal.FromString(op3str);
      ExtendedDecimal result=ExtendedDecimal.FromString(sresult.ToString());
      int expectedFlags=0;
      if(Contains(flags,"x"))expectedFlags|=PrecisionContext.FlagInexact;
      if(Contains(flags,"u"))expectedFlags|=PrecisionContext.FlagUnderflow;
      if(Contains(flags,"o"))expectedFlags|=PrecisionContext.FlagOverflow;
      if(Contains(flags,"i"))expectedFlags|=PrecisionContext.FlagInvalid;
      if(Contains(flags,"z"))expectedFlags|=PrecisionContext.FlagDivideByZero;
      ctx=ctx.WithBlankFlags();
      if(addop){
        ExtendedDecimal d3=op1.Add(op2,ctx);
        Assert.AreEqual(result,d3,ln);
        AssertFlags(expectedFlags,ctx.Flags,ln);
      }
      else if(subop){
        ExtendedDecimal d3=op1.Subtract(op2,ctx);
        Assert.AreEqual(result,d3,ln);
        AssertFlags(expectedFlags,ctx.Flags,ln);
      }
      else if(mul){
        ExtendedDecimal d3=op1.Multiply(op2,ctx);
        Assert.AreEqual(result,d3,ln);
        AssertFlags(expectedFlags,ctx.Flags,ln);
      }
      else if(div){
        ExtendedDecimal d3=op1.Divide(op2,ctx);
        Assert.AreEqual(result,d3,ln);
        AssertFlags(expectedFlags,ctx.Flags,ln);
      }
      else if(squroot){
        if(ctx.Rounding== Rounding.HalfEven){
          // Square root only defined for the HalfEven
          // rounding mode
          ExtendedDecimal d3=op1.SquareRoot(ctx);
          Assert.AreEqual(result,d3,ln);
          AssertFlags(expectedFlags,ctx.Flags,ln);
        }
      }
      else if(fma){
        ExtendedDecimal d3=op1.MultiplyAndAdd(op2,op3,ctx);
        Assert.AreEqual(result,d3,ln);
        AssertFlags(expectedFlags,ctx.Flags,ln);
      }
      else if(fms){
        ExtendedDecimal d3=op1.MultiplyAndSubtract(op2,op3,ctx);
        Assert.AreEqual(result,d3,ln);
        AssertFlags(expectedFlags,ctx.Flags,ln);
      }
      return 0;
    }

    [Test]
    public void TestParser(){
      long failures=0;
      if(!Directory.Exists(Path))
        return;
      for(int i=0;i<1;i++){
        foreach(var p in Directory.GetDirectories(Path)){
          foreach(var f in Directory.GetFiles(p)){
            Console.WriteLine("//"+f);
            using(StreamReader w=new StreamReader(f)){
              while(!w.EndOfStream){
                var ln=w.ReadLine();
                {
                  try {
                    ParseLine(ln);
                  } catch(Exception ex){
                    Console.WriteLine(ln);
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    failures++;
                    throw;
                  }
                }
              }
            }
          }
        }
      }
      if(failures>0){
        Assert.Fail(failures+" failure(s)");
      }
    }
  }
}

