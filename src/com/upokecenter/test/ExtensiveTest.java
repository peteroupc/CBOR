package com.upokecenter.test; import com.upokecenter.util.*;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 11/30/2013
 * Time: 10:11 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

import java.io.*;
import com.upokecenter.util.*;
import org.junit.Assert;
import org.junit.Test;
import java.util.regex.*;
using Test;
namespace CBOR
{
  /**
   * Description of ExtensiveTest.
   */
  
  public class ExtensiveTest
  {
    
    static String Path="..\\..\\..\\.settings\\test";
    static Pattern LineRegex=Pattern.compile(
      @"^(d64|d128)(\+|\-|\*|\/|\*\+|\*\-|V)[ \t]+(>|<|0|\=0|h>)[ \t]+"+
      @"([+\-][0-9]+E.get(+\-)?[0-9]+)[ \t]+([+\-][0-9]+E.get(+\-)?[0-9]+)[ \t]+\->[ \t]+"+
      @"([+\-][0-9]+E.get(+\-)?[0-9]+|[+\-]inf)[ \t]*(x?u?o?z?i?)[ \t]*",
      RegexOptions.Compiled);
    static Pattern ThreeOpLineRegex=Pattern.compile(
      @"^(d64|d128)(\+|\-|\*|\/|\*\+|\*\-|V)[ \t]+(>|<|0|\=0|h>)[ \t]+"+
      @"([+\-][0-9]+E.get(+\-)?[0-9]+)[ \t]+([+\-][0-9]+E.get(+\-)?[0-9]+)[ \t]+([+\-][0-9]+E.get(+\-)?[0-9]+)[ \t]+\->[ \t]+"+
      @"([+\-][0-9]+E.get(+\-)?[0-9]+|[+\-]inf)[ \t]*(x?u?o?z?i?)[ \t]*",
      RegexOptions.Compiled);
    
    public static void AssertFlags(int expected, int actual, String str) {
      actual&=(PrecisionContext.FlagInexact|
               PrecisionContext.FlagUnderflow|
               PrecisionContext.FlagOverflow);
      if(expected==actual)return;
      Assert.assertEquals("Inexact: "+str,(expected&PrecisionContext.FlagInexact)!=0,(expected&PrecisionContext.FlagInexact)!=0);
      Assert.assertEquals("Overflow: "+str,(expected&PrecisionContext.FlagOverflow)!=0,(expected&PrecisionContext.FlagOverflow)!=0);
      Assert.assertEquals("Underflow: "+str,(expected&PrecisionContext.FlagUnderflow)!=0,(expected&PrecisionContext.FlagUnderflow)!=0);
    }

    private boolean Contains(String str, String sub) {
      return str.IndexOf(sub,StringComparison.Ordinal)>=0;
    }
    
    
    private void ParseLine(String ln) {
      Matcher match=LineRegex.matcher(ln);
      if(match.matches() && false){
        String type=(match.group(1));
        PrecisionContext ctx=(type.equals("d64")) ?
          PrecisionContext.Decimal64 :
          PrecisionContext.Decimal128;
        String op=(match.group(2));
        String round=(match.group(3));
        if(round.equals(">"))ctx=ctx.WithRounding(Rounding.Ceiling);
        if(round.equals("<"))ctx=ctx.WithRounding(Rounding.Floor);
        if(round.equals("0"))ctx=ctx.WithRounding(Rounding.Down);
        if(round.equals("=0"))ctx=ctx.WithRounding(Rounding.HalfEven);
        if(round.equals("h>"))ctx=ctx.WithRounding(Rounding.HalfUp);
        String op1str=match.group(4);
        String op2str=match.group(5);
        DecimalFraction op1=DecimalFraction.FromString(match.group(4));
        DecimalFraction op2=DecimalFraction.FromString(match.group(5));
        DecimalFraction result;
        if(Contains(match.group(6),"inf")){
          result=null;
        } else {
          result=DecimalFraction.FromString(match.group(6));
        }
        String flags=match.group(7);
        int expectedFlags=0;
        if(Contains(flags,"x"))expectedFlags|=PrecisionContext.FlagInexact;
        if(Contains(flags,"u"))expectedFlags|=PrecisionContext.FlagUnderflow;
        if(Contains(flags,"o"))expectedFlags|=PrecisionContext.FlagOverflow;
        boolean invalid=(Contains(flags,"i"));
        boolean divzero=(Contains(flags,"z"));
        ctx=ctx.WithBlankFlags();
        if(op.equals("+")){
          if(invalid){
            try { op1.Add(op2,ctx); Assert.fail("Should have failed\n"+ln); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
          } else {
            DecimalFraction d3=op1.Add(op2,ctx);
            if(!(((d3)==null) ? ((result)==null) : (d3).equals(result)))
              Assert.assertEquals(ln,result,d3);
            AssertFlags(expectedFlags,ctx.getFlags(),ln);
          }
        }
        else if(op.equals("-")){
          if(invalid){
            try { op1.Subtract(op2,ctx); Assert.fail("Should have failed\n"+ln); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
          } else {
            DecimalFraction d3=op1.Subtract(op2,ctx);
            if(!(((d3)==null) ? ((result)==null) : (d3).equals(result)))
              Assert.assertEquals(ln,result,d3);
            AssertFlags(expectedFlags,ctx.getFlags(),ln);
          }
        }
        else if(op.equals("*")){
          if(invalid){
            try { op1.Multiply(op2,ctx); Assert.fail("Should have failed\n"+ln); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
          } else {
            DecimalFraction d3=op1.Multiply(op2,ctx);
            if(!(((d3)==null) ? ((result)==null) : (d3).equals(result)))
              Assert.assertEquals(ln,result,d3);
            AssertFlags(expectedFlags,ctx.getFlags(),ln);
          }
        }
        else if(op.equals("/")){
          if(invalid){
            try { op1.Divide(op2,ctx); Assert.fail("Should have failed\n"+ln); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
          } else if(divzero){
            try { op1.Divide(op2,ctx); Assert.fail("Should have failed\n"+ln); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
          } else {
            DecimalFraction d3=op1.Divide(op2,ctx);
            if(!(((d3)==null) ? ((result)==null) : (d3).equals(result)))
              Assert.assertEquals(ln,result,d3);
            AssertFlags(expectedFlags,ctx.getFlags(),ln);
          }
        }
        return;
      } 
      match=ThreeOpLineRegex.matcher(ln);
      if(match.matches()){
        String type=(match.group(1));
        PrecisionContext ctx=(type.equals("d64")) ?
          PrecisionContext.Decimal64 :
          PrecisionContext.Decimal128;
        String op=(match.group(2));
        String round=(match.group(3));
        if(round.equals(">"))ctx=ctx.WithRounding(Rounding.Ceiling);
        if(round.equals("<"))ctx=ctx.WithRounding(Rounding.Floor);
        if(round.equals("0"))ctx=ctx.WithRounding(Rounding.Down);
        if(round.equals("=0"))ctx=ctx.WithRounding(Rounding.HalfEven);
        if(round.equals("h>"))ctx=ctx.WithRounding(Rounding.HalfUp);
        String op1str=match.group(4);
        String op2str=match.group(5);
        String op3str=match.group(6);
        DecimalFraction op1=DecimalFraction.FromString(match.group(4));
        DecimalFraction op2=DecimalFraction.FromString(match.group(5));
        DecimalFraction op3=DecimalFraction.FromString(match.group(6));
        DecimalFraction result;
        if(Contains(match.group(7),"inf")){
          result=null;
        } else {
          result=DecimalFraction.FromString(match.group(7));
        }
        String flags=match.group(8);
        int expectedFlags=0;
        if(Contains(flags,"x"))expectedFlags|=PrecisionContext.FlagInexact;
        if(Contains(flags,"u"))expectedFlags|=PrecisionContext.FlagUnderflow;
        if(Contains(flags,"o"))expectedFlags|=PrecisionContext.FlagOverflow;
        boolean invalid=(Contains(flags,"i"));
        boolean divzero=(Contains(flags,"z"));
        ctx=ctx.WithBlankFlags();
        if(op.equals("*+")){
          if(invalid){
            try { op1.MultiplyAndAdd(op2,op3,ctx); Assert.fail("Should have failed\n"+ln); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
          } else {
            DecimalFraction d3=op1.MultiplyAndAdd(op2,op3,ctx);
            if(!(((d3)==null) ? ((result)==null) : (d3).equals(result)))
              Assert.assertEquals(ln,result,d3);
            AssertFlags(expectedFlags,ctx.getFlags(),ln);
          }
        }
        else if(op.equals("*-")){
          if(invalid){
            try { op1.MultiplyAndAdd(op2,op3.Negate(),ctx); Assert.fail("Should have failed\n"+ln); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
          } else {
            DecimalFraction d3=op1.MultiplyAndAdd(op2,op3.Negate(),ctx);
            if(!(((d3)==null) ? ((result)==null) : (d3).equals(result)))
              Assert.assertEquals(ln,result,d3);
            AssertFlags(expectedFlags,ctx.getFlags(),ln);
          }
        }
      }
    }
    
    @Test
    public void TestParser() {
      for(Object p : Directory.GetDirectories(Path)){
        for(Object f : Directory.GetFiles(p)){
          String[] lines=File.ReadAllLines(f);
          for(Object ln : lines){
            ParseLine(ln);
          }
        }
      }
    }
  }
