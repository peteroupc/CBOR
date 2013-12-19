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
    private bool StartsWith(string str, string sub){
      return str.IndexOf(sub,StringComparison.Ordinal)>=0;
    }
    
    
    private int ParseLine(string ln){
      string[] chunks=ln.Split(' ');
      if(chunks.Length<4)
        return 0;
      string type=chunks[0];
      PrecisionContext ctx=(type.StartsWith("d64")) ?
        PrecisionContext.Decimal64 :
        PrecisionContext.Decimal128;
      if(Contains(type,"!"))
        return 0;
      bool squroot=Contains(type,"V");
      bool div=Contains(type,"/");
      bool fma=Contains(type,"*+");
      bool fms=Contains(type,"*-");
      bool addop=!fma && Contains(type,"+");
      bool subop=!fms && Contains(type,"-");
      bool mul=!fma && !fms && Contains(type,"*");
      if(!fma && !fms && !addop && !subop)
        return 0;
      string round=chunks[1];
      if(round.Equals(">"))ctx=ctx.WithRounding(Rounding.Ceiling);
      if(round.Equals("<"))ctx=ctx.WithRounding(Rounding.Floor);
      if(round.Equals("0"))ctx=ctx.WithRounding(Rounding.Down);
      if(round.Equals("=0"))ctx=ctx.WithRounding(Rounding.HalfEven);
      if(round.Equals("h>"))ctx=ctx.WithRounding(Rounding.HalfUp);
      string op1str=chunks[2];
      if(op1str.Equals("+inf") || op1str.Equals("-inf") || 
         op1str.Equals("+Inf") || op1str.Equals("-Inf") || op1str.Equals("S") || op1str.Equals("Q"))
        return 0;
      string op2str=chunks[3];
      if(op2str.Equals("+inf") || op2str.Equals("-inf") || 
         op2str.Equals("+Inf") || op2str.Equals("-Inf") || op2str.Equals("S") || op2str.Equals("Q"))
        return 0;
      string op3str=String.Empty;
      if(chunks.Length<=4)
        return 0;
      string sresult=String.Empty;
      string flags=String.Empty;
      op3str=chunks[4];
      if(op3str.Equals("->")){
        if(chunks.Length<=6)
          return 0;
        op3str=String.Empty;
        sresult=chunks[5];
        flags=chunks[6];
      } else {
        if(chunks.Length<=7)
          return 0;
        if(op3str.Equals("+inf") || op3str.Equals("-inf") || 
           op3str.Equals("+Inf") || op3str.Equals("-Inf") || op3str.Equals("S") || op3str.Equals("Q"))
          return 0;
        sresult=chunks[6];
        flags=chunks[7];
      }
      if(sresult.Equals("S") || sresult.Equals("Q"))
        return 0;
      DecimalFraction op1=DecimalFraction.FromString(op1str);
      DecimalFraction op2=DecimalFraction.FromString(op2str);
      DecimalFraction op3=(String.IsNullOrEmpty(op3str)) ? null : DecimalFraction.FromString(op3str);
      DecimalFraction result;
      if(Contains(sresult,"inf") || Contains(sresult,"Inf")){
        result=null;
      } else {
        result=DecimalFraction.FromString(sresult.ToString());
      }
      int expectedFlags=0;
      if(Contains(flags,"x"))expectedFlags|=PrecisionContext.FlagInexact;
      if(Contains(flags,"u"))expectedFlags|=PrecisionContext.FlagUnderflow;
      if(Contains(flags,"o"))expectedFlags|=PrecisionContext.FlagOverflow;
      bool invalid=(Contains(flags,"i"));
      bool divzero=(Contains(flags,"z"));
      ctx=ctx.WithBlankFlags();
      if(addop){
        if(invalid){
          try { op1.Add(op2,ctx); Assert.Fail("Should have failed\n"+ln); } catch(ArithmeticException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
        } else {
          DecimalFraction d3=op1.Add(op2,ctx);
          if(!Object.Equals(d3,result))
            Assert.AreEqual(result,d3,ln);
          AssertFlags(expectedFlags,ctx.Flags,ln);
        }
      }
      else if(subop){
        if(invalid){
          try { op1.Subtract(op2,ctx); Assert.Fail("Should have failed\n"+ln); } catch(ArithmeticException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
        } else {
          DecimalFraction d3=op1.Subtract(op2,ctx);
          if(!Object.Equals(d3,result))
            Assert.AreEqual(result,d3,ln);
          AssertFlags(expectedFlags,ctx.Flags,ln);
        }
      }
      else if(mul){
        if(invalid){
          try { op1.Multiply(op2,ctx); Assert.Fail("Should have failed\n"+ln); } catch(ArithmeticException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
        } else {
          DecimalFraction d3=op1.Multiply(op2,ctx);
          if(!Object.Equals(d3,result))
            Assert.AreEqual(result,d3,ln);
          AssertFlags(expectedFlags,ctx.Flags,ln);
        }
      }
      else if(div){
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
      else if(fma){
        if(invalid){
          try { op1.MultiplyAndAdd(op2,op3,ctx); Assert.Fail("Should have failed\n"+ln); } catch(ArithmeticException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
        } else {
          DecimalFraction d3=op1.MultiplyAndAdd(op2,op3,ctx);
          if(!Object.Equals(d3,result))
            Assert.AreEqual(result,d3,ln);
          AssertFlags(expectedFlags,ctx.Flags,ln);
        }
      }
      else if(fms){
        if(invalid){
          try { op1.MultiplyAndAdd(op2,op3.Negate(),ctx); Assert.Fail("Should have failed\n"+ln); } catch(ArithmeticException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
        } else {
          DecimalFraction d3=op1.MultiplyAndAdd(op2,op3.Negate(),ctx);
          if(!Object.Equals(d3,result))
            Assert.AreEqual(result,d3,ln);
          AssertFlags(expectedFlags,ctx.Flags,ln);
        }
      }
      return 0;
    }
    
    [Test]
    public void TestParser(){
      long failures=0;
      foreach(var p in Directory.GetDirectories(Path)){
        foreach(var f in Directory.GetFiles(p)){
          Console.WriteLine(f);
          string[] lines=File.ReadAllLines(f);
          foreach(var ln in lines){
            try {
              ParseLine(ln);
            } catch(Exception ex){
              Console.WriteLine(ln);
              Console.WriteLine(ex.Message);
              failures++;
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
