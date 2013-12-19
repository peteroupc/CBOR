var DecTestUtil={}
var BigNumber=require("./BigNumberMin.js"),
   PrecisionContext=BigNumber.PrecisionContext,
   BigInteger=BigNumber.BigInteger,
   DecimalFraction=BigNumber.DecimalFraction;
var Assert=require("./Assert.js");

var TestCommon={}
TestCommon.AssertDecFrac=function(a,b,x){
 if(b==null){
   Assert.AreEqual(null,a,x||"");
 } else {
   Assert.AreEqual(DecimalFraction.FromString(b),a,x||"");
 }
}
TestCommon.AssertFlags=function(a,b){
 //Assert.AreEqual(a,b,"Flags");
}

DecTestUtil.TestOp_abs=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    
    var d3=d1.Abs(ctx);
    
   // sys.puts([input1]+"")
    TestCommon.AssertDecFrac(d3,output,"abs "+[input1,output]);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_abs_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
  
    var d1=DecimalFraction.FromString(input1);
    
    
    var d3=d1.Abs(ctx);
    
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_add=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    var d3=d1.Add(d2,ctx);
    
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_add_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.Add(d2,ctx,"add "+[input1,input2,output]);
    
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_subtract=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.Subtract(d2,ctx);
    
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_subtract_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.Subtract(d2,ctx);
    
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_compare=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);

    var d3=DecimalFraction.FromInt64(d1.compareTo(d2));
    d3=d3.RoundToPrecision(ctx);
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_compare_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=DecimalFraction.FromInt64(d1.compareTo(d2));
    d3=d3.RoundToPrecision(ctx);
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_comparesig=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=DecimalFraction.FromInt64(d1.compareTo(d2));
    d3=d3.RoundToPrecision(ctx);
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_comparesig_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=DecimalFraction.FromInt64(d1.compareTo(d2));
    d3=d3.RoundToPrecision(ctx);
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_divide=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.Divide(d2,ctx);
    
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_divide_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.Divide(d2,ctx);
    
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_divide_DivideByZero=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.Divide(d2,ctx);
    
    Assert.Fail("Expected divide by zero error");
    } catch(DivideByZeroException){ }
   }
DecTestUtil.TestOp_divideint=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.DivideToIntegerZeroScale(d2,ctx);
    
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_divideint_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.DivideToIntegerZeroScale(d2,ctx);
    
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_divideint_DivideByZero=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.DivideToIntegerZeroScale(d2,ctx);
    
    Assert.Fail("Expected divide by zero error");
    } catch(DivideByZeroException){ }
   }
DecTestUtil.TestOp_fma=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, input3, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    var d2a=DecimalFraction.FromString(input3);
    var d3=d1.MultiplyAndAdd(d2,d2a,ctx);
    
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_fma_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, input3, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    var d2a=DecimalFraction.FromString(input3);
    var d3=d1.MultiplyAndAdd(d2,d2a,ctx);
    
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_max=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=DecimalFraction.Max(d1,d2);
    d3=d3.RoundToPrecision(ctx);
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_max_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=DecimalFraction.Max(d1,d2);
    d3=d3.RoundToPrecision(ctx);
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_maxmag=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=DecimalFraction.MaxMagnitude(d1,d2);
    d3=d3.RoundToPrecision(ctx);
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_maxmag_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=DecimalFraction.MaxMagnitude(d1,d2);
    d3=d3.RoundToPrecision(ctx);
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_min=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=DecimalFraction.Min(d1,d2);
    d3=d3.RoundToPrecision(ctx);
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_min_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=DecimalFraction.Min(d1,d2);
    d3=d3.RoundToPrecision(ctx);
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_minmag=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=DecimalFraction.MinMagnitude(d1,d2);
    d3=d3.RoundToPrecision(ctx);
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_minmag_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=DecimalFraction.MinMagnitude(d1,d2);
    d3=d3.RoundToPrecision(ctx);
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_minus=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    
    
    var d3=d1.Negate(ctx);
    
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_minus_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    
    
    var d3=d1.Negate(ctx);
    
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_multiply=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.Multiply(d2,ctx);
    
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_multiply_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.Multiply(d2,ctx);
    
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_nextminus=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    
    
    var d3=d1.NextMinus(ctx);
    
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_nextminus_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    
    
    var d3=d1.NextMinus(ctx);
    
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_nextplus=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    
    
    var d3=d1.NextPlus(ctx);
    
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_nextplus_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    
    
    var d3=d1.NextPlus(ctx);
    
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_nexttoward=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.NextToward(d2,ctx);
    
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_nexttoward_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.NextToward(d2,ctx);
    
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_plus=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    
    
    var d3=d1.RoundToPrecision(ctx);
    
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_plus_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    
    
    var d3=d1.RoundToPrecision(ctx);
    
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_quantize=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.Quantize(d2,ctx);
    
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_quantize_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.Quantize(d2,ctx);
    
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_reduce=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    
    
    var d3=d1.Reduce(ctx);
    
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_reduce_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    
    
    var d3=d1.Reduce(ctx);
    
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_remainder=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.Remainder(d2,ctx);
    
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_remainder_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.Remainder(d2,ctx);
    
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_remainder_DivideByZero=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.Remainder(d2,ctx);
    
    Assert.Fail("Expected divide by zero error");
    } catch(DivideByZeroException){ }
   }
DecTestUtil.TestOp_remaindernear=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.RemainderNear(d2,ctx);
    
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_remaindernear_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.RemainderNear(d2,ctx);
    
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_remaindernear_DivideByZero=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, input2, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    var d2=DecimalFraction.FromString(input2);
    
    var d3=d1.RemainderNear(d2,ctx);
    
    Assert.Fail("Expected divide by zero error");
    } catch(DivideByZeroException){ }
   }
DecTestUtil.TestOp_tointegralx=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    
    
    var d3=d1.RoundToIntegralExact(ctx);
    
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_tointegralx_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    
    
    var d3=d1.RoundToIntegralExact(ctx);
    
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }
DecTestUtil.TestOp_tointegral=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, output, flags){
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    
    
    var d3=d1.RoundToIntegralNoRoundedFlag(ctx);
    
    TestCommon.AssertDecFrac(d3,output);
    TestCommon.AssertFlags(flags,ctx.Flags);
   }
DecTestUtil.TestOp_tointegral_Invalid=function(precision,
     rounding, minExponent, maxExponent, clamp, input1, output, flags){
    try {
    var ctx=new PrecisionContext(precision,rounding,minExponent,maxExponent,clamp).WithBlankFlags();
    var d1=DecimalFraction.FromString(input1);
    
    
    var d3=d1.RoundToIntegralNoRoundedFlag(ctx);
    
    Assert.Fail("Expected invalid op error");
    } catch(e){ }
   }

module.exports=DecTestUtil;