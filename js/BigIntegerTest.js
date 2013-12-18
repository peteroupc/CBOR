var BigInteger=require("./BigNumber.js").BigInteger,
  Assert=require("./Assert.js"),
  DecimalFraction=require("./BigNumber.js").DecimalFraction,
  sys=require("sys");

function AssertBigIntegersEqual(a,b){
      Assert.AreEqual(a,b.toString());
      var a2=BigInteger.fromString(a);
      Assert.AreEqual(a2,b);
    }

var BigIntegerTest={}
BigIntegerTest.DoTestDivide=function(dividend,divisor,result){
      var bigintA=BigInteger.fromString(dividend);
      var bigintB=BigInteger.fromString(divisor);
      if(bigintB.isZero()){
        try { bigintA.divide(bigintB); Assert.Fail("Expected divide by 0 error");
        } catch(e){ }
      } else {
        AssertBigIntegersEqual(result,bigintA.divide(bigintB));
      }
    }
BigIntegerTest.DoTestRemainder=function(dividend,divisor,result){
      var bigintA=BigInteger.fromString(dividend);
      var bigintB=BigInteger.fromString(divisor);
      if(bigintB.isZero()){
        try { bigintA.remainder(bigintB); Assert.Fail("Expected divide by 0 error");
        } catch(e){ }
      } else {
        AssertBigIntegersEqual(result,(bigintA.remainder(bigintB)));
      }
    }
BigIntegerTest.DoTestDivideAndRemainder=function(dividend,divisor,result, rem){
      var bigintA=BigInteger.fromString(dividend);
      var bigintB=BigInteger.fromString(divisor);
      if(bigintB.isZero()){
        try {
          bigintA.divideAndRemainder(bigintB);
          Assert.Fail("Expected divide by 0 error");
        } catch(e){ }
      } else {
        var rembi;
        var quo=bigintA.divideAndRemainder(bigintB);
        rembi=quo[1];
        quo=quo[0];
        AssertBigIntegersEqual(result,quo);
        AssertBigIntegersEqual(rem,rembi);
      }
    }
BigIntegerTest.DoTestMultiply=function(m1,m2,result){
      var bigintA=BigInteger.fromString(m1);
      var bigintB=BigInteger.fromString(m2);
      AssertBigIntegersEqual(result,(bigintA.multiply(bigintB)));
    }
BigIntegerTest.DoTestAdd=function(m1,m2,result){
      var bigintA=BigInteger.fromString(m1);
      var bigintB=BigInteger.fromString(m2);
      AssertBigIntegersEqual(result,(bigintA.add(bigintB)));
    }
BigIntegerTest.DoTestAddDec=function(m1,m2,result){
      var bigintA=DecimalFraction.FromString(m1);
      var bigintB=DecimalFraction.FromString(m2);
      Assert.AreEqual(DecimalFraction.FromString(result),(bigintA.Add(bigintB)));
    }
BigIntegerTest.DoTestSubtractDec=function(m1,m2,result){
      var bigintA=DecimalFraction.FromString(m1);
      var bigintB=DecimalFraction.FromString(m2);
      Assert.AreEqual(DecimalFraction.FromString(result),(bigintA.Subtract(bigintB)));
    }
BigIntegerTest.DoTestMultiplyDec=function(m1,m2,result){
      var bigintA=DecimalFraction.FromString(m1);
      var bigintB=DecimalFraction.FromString(m2);
      Assert.AreEqual(DecimalFraction.FromString(result),(bigintA.Multiply(bigintB)));
    }
BigIntegerTest.DoTestSubtract=function(m1,m2,result){
      var bigintA=BigInteger.fromString(m1);
      var bigintB=BigInteger.fromString(m2);
      AssertBigIntegersEqual(result,(bigintA.subtract(bigintB)));
    }
BigIntegerTest.DoTestPow=function(m1,m2,result){
      var bigintA=BigInteger.fromString(m1);
      AssertBigIntegersEqual(result,(bigintA.pow(m2)));
    }
BigIntegerTest.DoTestShiftLeft=function(m1,m2,result){
      var bigintA=BigInteger.fromString(m1);
      AssertBigIntegersEqual(result,(bigintA.shiftLeft(m2)));
      AssertBigIntegersEqual(result,(bigintA.shiftRight(-m2)));
    }
BigIntegerTest.DoTestShiftRight=function(m1,m2,result){
      var bigintA=BigInteger.fromString(m1);
      AssertBigIntegersEqual(result,(bigintA.shiftRight(m2)));
      AssertBigIntegersEqual(result,(bigintA.shiftLeft(-m2)));
    }

module.exports=BigIntegerTest;