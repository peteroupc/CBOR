# Written in 2013 by Peter Occil.
# Any copyright is dedicated to the Public Domain.
# http://creativecommons.org/publicdomain/zero/1.0/
# 
# If you like this, you should donate to Peter O.
# at: http://upokecenter.com/d/

# Generates test cases for the JavaScript version.

require 'bigdecimal'

def randomnum()
  ret=""
  for i in 0...rand(25)+1
   ret+=rand(10).to_s
  end
  return ret.to_i
end
def randomdec()
  return BigDecimal.new((rand(2)==0 ? "-" : "")+randomnum().to_s+"."+randomnum().to_s)
end

def signstr(a,neg)
  return "\"#{neg && a!=0 ? '-' : ''}#{a.to_s}\""
end

puts "var TestCommon=require('./BigIntegerTest.js')"
for i in 0...100
  a=randomnum()*(rand(2)==0 ? -1 : 1)
  b=randomnum()*(rand(2)==0 ? -1 : 1)
  puts "TestCommon.DoTestAdd(#{a.to_s.inspect},#{b.to_s.inspect},#{(a+b).to_s.inspect});"
  a=randomnum()*(rand(2)==0 ? -1 : 1)
  b=randomnum()*(rand(2)==0 ? -1 : 1)
  puts "TestCommon.DoTestSubtract(#{a.to_s.inspect},#{b.to_s.inspect},#{(a-b).to_s.inspect});"
  a=randomnum()*(rand(2)==0 ? -1 : 1)
  b=randomnum()*(rand(2)==0 ? -1 : 1)
  puts "TestCommon.DoTestMultiply(#{a.to_s.inspect},#{b.to_s.inspect},#{(a*b).to_s.inspect});"
  a=randomnum()
  b=randomnum()
  aneg=(a!=0 && rand(2)==0)
  bneg=(b!=0 && rand(2)==0)
  puts "TestCommon.DoTestDivide(#{signstr(a,aneg)},#{signstr(b,bneg)},#{b==0 ? 'null' : signstr(a/b,aneg^bneg)});"
  a=randomnum()
  b=randomnum()
  aneg=(a!=0 && rand(2)==0)
  bneg=(b!=0 && rand(2)==0)
  puts "TestCommon.DoTestRemainder(#{signstr(a,aneg)},#{signstr(b,bneg)},#{b==0 ? 'null' : signstr(a%b,aneg)});"
  a=randomnum()*randomnum()
  aneg=(a!=0 && rand(2)==0)
  puts "TestCommon.DoTestDigitCount(#{signstr(a,aneg)},#{a.to_s.length});"
  a=randomnum()
  b=randomnum()
  aneg=(a!=0 && rand(2)==0)
  bneg=(b!=0 && rand(2)==0)
  puts "TestCommon.DoTestDivideAndRemainder(#{signstr(a,aneg)},#{signstr(b,bneg)},
    #{b==0 ? 'null' : signstr(a/b,aneg^bneg)},
    #{b==0 ? 'null' : signstr(a%b,aneg)}
    );"
  a=randomnum()*(rand(2)==0 ? -1 : 1)
  b=rand(10)
  puts "TestCommon.DoTestPow(#{a.to_s.inspect},#{b.inspect},#{(a**b).to_s.inspect});"
  a=randomnum()*(rand(2)==0 ? -1 : 1)
  b=rand(200)
  puts "TestCommon.DoTestShiftLeft(#{a.to_s.inspect},#{b.inspect},#{(a<<b).to_s.inspect});"
  a=randomnum()*(rand(2)==0 ? -1 : 1)
  b=rand(200)
  puts "TestCommon.DoTestShiftRight(#{a.to_s.inspect},#{b.inspect},#{(a>>b).to_s.inspect});"
end