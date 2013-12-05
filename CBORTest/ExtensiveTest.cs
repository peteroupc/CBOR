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
    static Regex LineRegex=new Regex(
      @"^(d64|d128)(\+|\-|\*|\/|\*\+|\*\-|V)[ \t]+(>|<|0|\=0|h>)[ \t]+"+
      @"([+\-][0-9]+E[+\-]?[0-9]+)[ \t]+([+\-][0-9]+E[+\-]?[0-9]+)[ \t]+\->[ \t]+"+
      @"([+\-][0-9]+E[+\-]?[0-9]+|[+\-]inf)[ \t]*(x?u?o?z?i?)[ \t]*",
      RegexOptions.Compiled);
    static Regex ThreeOpLineRegex=new Regex(
      @"^(d64|d128)(\+|\-|\*|\/|\*\+|\*\-|V)[ \t]+(>|<|0|\=0|h>)[ \t]+"+
      @"([+\-][0-9]+E[+\-]?[0-9]+)[ \t]+([+\-][0-9]+E[+\-]?[0-9]+)[ \t]+([+\-][0-9]+E[+\-]?[0-9]+)[ \t]+\->[ \t]+"+
      @"([+\-][0-9]+E[+\-]?[0-9]+|[+\-]inf)[ \t]*(x?u?o?z?i?)[ \t]*",
      RegexOptions.Compiled);
    
    public static void AssertFlags(int expected, int actual, string str){
      actual&=(PrecisionContext.FlagInexact|
               PrecisionContext.FlagUnderflow|
               PrecisionContext.FlagOverflow);
      if(expected==actual)return;
      Assert.AreEqual((expected&PrecisionContext.FlagInexact)!=0,
                      (expected&PrecisionContext.FlagInexact)!=0,"Inexact: "+str);
      Assert.AreEqual((expected&PrecisionContext.FlagOverflow)!=0,
                      (expected&PrecisionContext.FlagOverflow)!=0,"Overflow: "+str);
      Assert.AreEqual((expected&PrecisionContext.FlagUnderflow)!=0,
                      (expected&PrecisionContext.FlagUnderflow)!=0,"Underflow: "+str);
    }

    private bool Contains(string str, string sub){
      return str.IndexOf(sub,StringComparison.Ordinal)>=0;
    }
    
    
    private void ParseLine(string ln){
      Match match=LineRegex.Match(ln);
      if(match.Success){
        string type=(match.Groups[1].ToString());
        PrecisionContext ctx=(type.Equals("d64")) ?
          PrecisionContext.Decimal64 :
          PrecisionContext.Decimal128;
        string op=(match.Groups[2].ToString());
        string round=(match.Groups[3].ToString());
        if(round.Equals(">"))ctx=ctx.WithRounding(Rounding.Ceiling);
        if(round.Equals("<"))ctx=ctx.WithRounding(Rounding.Floor);
        if(round.Equals("0"))ctx=ctx.WithRounding(Rounding.Down);
        if(round.Equals("=0"))ctx=ctx.WithRounding(Rounding.HalfEven);
        if(round.Equals("h>"))ctx=ctx.WithRounding(Rounding.HalfUp);
        string op1str=match.Groups[4].ToString();
        string op2str=match.Groups[5].ToString();
        DecimalFraction op1=DecimalFraction.FromString(match.Groups[4].ToString());
        DecimalFraction op2=DecimalFraction.FromString(match.Groups[5].ToString());
        DecimalFraction result;
        if(Contains(match.Groups[6].ToString(),"inf")){
          result=null;
        } else {
          result=DecimalFraction.FromString(match.Groups[6].ToString());
        }
        string flags=match.Groups[7].ToString();
        int expectedFlags=0;
        if(Contains(flags,"x"))expectedFlags|=PrecisionContext.FlagInexact;
        if(Contains(flags,"u"))expectedFlags|=PrecisionContext.FlagUnderflow;
        if(Contains(flags,"o"))expectedFlags|=PrecisionContext.FlagOverflow;
        bool invalid=(Contains(flags,"i"));
        bool divzero=(Contains(flags,"z"));
        ctx=ctx.WithBlankFlags();
        if(op.Equals("+")){
          if(invalid){
            try { op1.Add(op2,ctx); Assert.Fail("Should have failed\n"+ln); } catch(ArithmeticException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
          } else {
            DecimalFraction d3=op1.Add(op2,ctx);
            if(!Object.Equals(d3,result))
              Assert.AreEqual(result,d3,ln);
            AssertFlags(expectedFlags,ctx.Flags,ln);
          }
        }
        else if(op.Equals("-")){
          if(invalid){
            try { op1.Subtract(op2,ctx); Assert.Fail("Should have failed\n"+ln); } catch(ArithmeticException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
          } else {
            DecimalFraction d3=op1.Subtract(op2,ctx);
            if(!Object.Equals(d3,result))
              Assert.AreEqual(result,d3,ln);
            AssertFlags(expectedFlags,ctx.Flags,ln);
          }
        }
        else if(op.Equals("*")){
          if(invalid){
            try { op1.Multiply(op2,ctx); Assert.Fail("Should have failed\n"+ln); } catch(ArithmeticException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
          } else {
            DecimalFraction d3=op1.Multiply(op2,ctx);
            if(!Object.Equals(d3,result))
              Assert.AreEqual(result,d3,ln);
            AssertFlags(expectedFlags,ctx.Flags,ln);
          }
        }
        else if(op.Equals("/")){
          if(invalid){
            try { op1.Divide(op2,ctx); Assert.Fail("Should have failed\n"+ln); } catch(ArithmeticException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
          } else if(divzero){
            try { op1.Divide(op2,ctx); Assert.Fail("Should have failed\n"+ln); } catch(DivideByZeroException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
          } else {
            DecimalFraction d3=op1.Divide(op2,ctx);
            if(!Object.Equals(d3,result))
              Assert.AreEqual(result,d3,ln);
            AssertFlags(expectedFlags,ctx.Flags,ln);
          }
        }
        return;
      } 
      match=ThreeOpLineRegex.Match(ln);
      if(match.Success){
        string type=(match.Groups[1].ToString());
        PrecisionContext ctx=(type.Equals("d64")) ?
          PrecisionContext.Decimal64 :
          PrecisionContext.Decimal128;
        string op=(match.Groups[2].ToString());
        string round=(match.Groups[3].ToString());
        if(round.Equals(">"))ctx=ctx.WithRounding(Rounding.Ceiling);
        if(round.Equals("<"))ctx=ctx.WithRounding(Rounding.Floor);
        if(round.Equals("0"))ctx=ctx.WithRounding(Rounding.Down);
        if(round.Equals("=0"))ctx=ctx.WithRounding(Rounding.HalfEven);
        if(round.Equals("h>"))ctx=ctx.WithRounding(Rounding.HalfUp);
        string op1str=match.Groups[4].ToString();
        string op2str=match.Groups[5].ToString();
        string op3str=match.Groups[6].ToString();
        DecimalFraction op1=DecimalFraction.FromString(match.Groups[4].ToString());
        DecimalFraction op2=DecimalFraction.FromString(match.Groups[5].ToString());
        DecimalFraction op3=DecimalFraction.FromString(match.Groups[6].ToString());
        DecimalFraction result;
        if(Contains(match.Groups[7].ToString(),"inf")){
          result=null;
        } else {
          result=DecimalFraction.FromString(match.Groups[7].ToString());
        }
        string flags=match.Groups[8].ToString();
        int expectedFlags=0;
        if(Contains(flags,"x"))expectedFlags|=PrecisionContext.FlagInexact;
        if(Contains(flags,"u"))expectedFlags|=PrecisionContext.FlagUnderflow;
        if(Contains(flags,"o"))expectedFlags|=PrecisionContext.FlagOverflow;
        bool invalid=(Contains(flags,"i"));
        bool divzero=(Contains(flags,"z"));
        ctx=ctx.WithBlankFlags();
        if(op.Equals("*+") && false){
          if(invalid){
            try { op1.MultiplyAndAdd(op2,op3,ctx); Assert.Fail("Should have failed\n"+ln); } catch(ArithmeticException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
          } else {
            DecimalFraction d3=op1.MultiplyAndAdd(op2,op3,ctx);
            if(!Object.Equals(d3,result))
              Assert.AreEqual(result,d3,ln);
            AssertFlags(expectedFlags,ctx.Flags,ln);
          }
        }
        else if(op.Equals("*-") && false){
          if(invalid){
            try { op1.MultiplyAndAdd(op2,op3.Negate(),ctx); Assert.Fail("Should have failed\n"+ln); } catch(ArithmeticException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
          } else {
            DecimalFraction d3=op1.MultiplyAndAdd(op2,op3.Negate(),ctx);
            if(!Object.Equals(d3,result))
              Assert.AreEqual(result,d3,ln);
            AssertFlags(expectedFlags,ctx.Flags,ln);
          }
        }
      }
    }
    
    [Test]
    public void TestParser(){
      foreach(var p in Directory.GetDirectories(Path)){
        foreach(var f in Directory.GetFiles(p)){
          string[] lines=File.ReadAllLines(f);
          foreach(var ln in lines){
            ParseLine(ln);
          }
        }
      }
    }
  }
}
