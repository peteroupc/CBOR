package com.upokecenter.test;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */


import java.math.*;
import java.io.*;
import com.upokecenter.util.*;


import org.junit.Assert;
import org.junit.Test;


	
	
	public class CBORTest
	{
		
		@Test
		public void TestTagThenBreak() {
try {

			TestCommon.FromBytesTestAB(new byte[]{(byte)0xD1,(byte)0xFF});
		
} catch(Exception ex){
if(!(ex instanceof CBORException))Assert.fail(ex.toString());
}
}
		
		@Test
		public void TestJSON(){
			CBORObject o;
			o=CBORObject.FromJSONString("[1,2,3]");
			Assert.assertEquals(3,o.size());
			Assert.assertEquals(1,o.get(0).AsInt32());
			Assert.assertEquals(2,o.get(1).AsInt32());
			Assert.assertEquals(3,o.get(2).AsInt32());
			o=CBORObject.FromJSONString("[1.5,2.6,3.7,4.0,222.22]");
			double actual=o.get(0).AsDouble();
			Assert.assertEquals((double)1.5,actual,0);
		}
		
		@Test
		public void TestByte(){
			for(int i=0;i<=255;i++){
				TestCommon.AssertSer(
					CBORObject.FromObject((byte)i),
					String.format(java.util.Locale.US,"%s",i));
			}
		}
		
		public void DoTestReadUtf8(byte[] bytes,
		                           int expectedRet, String expectedString,
		                           int noReplaceRet, String noReplaceString
		                          ){
			DoTestReadUtf8(bytes,bytes.length,expectedRet,expectedString,
			               noReplaceRet,noReplaceString);
		}

		public void DoTestReadUtf8(byte[] bytes,int length,
		                           int expectedRet, String expectedString,
		                           int noReplaceRet, String noReplaceString
		                          ){
			try {
				StringBuilder builder=new StringBuilder();
				int ret=0;
				java.io.ByteArrayInputStream ms=null;
try {
ms=new ByteArrayInputStream(bytes);

					ret=CBORDataUtilities.ReadUtf8(ms,length,builder,true);
					Assert.assertEquals(expectedRet,ret);
					if(expectedRet==0){
						Assert.assertEquals(expectedString,builder.toString());
					}
					ms.reset();
					builder.setLength(0);
					ret=CBORDataUtilities.ReadUtf8(ms,length,builder,false);
					Assert.assertEquals(noReplaceRet,ret);
					if(noReplaceRet==0){
						Assert.assertEquals(noReplaceString,builder.toString());
					}
}
finally {
try { if(ms!=null)ms.close(); } catch(IOException ex){}
}
				if(bytes.length>=length){
					builder.setLength(0);
					ret=CBORDataUtilities.ReadUtf8FromBytes(bytes,0,length,builder,true);
					Assert.assertEquals(expectedRet,ret);
					if(expectedRet==0){
						Assert.assertEquals(expectedString,builder.toString());
					}
					builder.setLength(0);
					ret=CBORDataUtilities.ReadUtf8FromBytes(bytes,0,length,builder,false);
					Assert.assertEquals(noReplaceRet,ret);
					if(noReplaceRet==0){
						Assert.assertEquals(noReplaceString,builder.toString());
					}
				}
			} catch(IOException ex){
				throw new CBORException("",ex);
			}
		}
		
		@Test
		public void TestReadUtf8(){
			DoTestReadUtf8(new byte[]{0x20,0x20,0x20},
			               0,"   ",0,"   ");
			DoTestReadUtf8(new byte[]{0x20,(byte)0xc2,(byte)0x80},
			               0," \u0080",0," \u0080");
			DoTestReadUtf8(new byte[]{0x20,(byte)0xc2,(byte)0x80,0x20},
			               0," \u0080 ",0," \u0080 ");
			DoTestReadUtf8(new byte[]{0x20,(byte)0xc2,(byte)0x80,(byte)0xc2},
			               0," \u0080\ufffd",-1,null);
			DoTestReadUtf8(new byte[]{0x20,(byte)0xc2,0x20,0x20},
			               0," \ufffd  ",-1,null);
			DoTestReadUtf8(new byte[]{0x20,(byte)0xc2,(byte)0xff,0x20},
			               0," \ufffd\ufffd ",-1,null);
			DoTestReadUtf8(new byte[]{0x20,(byte)0xe0,(byte)0xa0,(byte)0x80},
			               0," \u0800",0," \u0800");
			DoTestReadUtf8(new byte[]{0x20,(byte)0xe0,(byte)0xa0,(byte)0x80,0x20},
			               0," \u0800 ",0," \u0800 ");
			DoTestReadUtf8(new byte[]{0x20,(byte)0xf0,(byte)0x90,(byte)0x80,(byte)0x80},
			               0," \ud800\udc00",0," \ud800\udc00");
			DoTestReadUtf8(new byte[]{0x20,(byte)0xf0,(byte)0x90,(byte)0x80,(byte)0x80},3,
			               0," \ufffd",-1,null);
			DoTestReadUtf8(new byte[]{0x20,(byte)0xf0,(byte)0x90},5,
			               -2,null,-1,null);
			DoTestReadUtf8(new byte[]{0x20,0x20,0x20},5,
			               -2,null,-2,null);
			DoTestReadUtf8(new byte[]{0x20,(byte)0xf0,(byte)0x90,(byte)0x80,(byte)0x80,0x20},
			               0," \ud800\udc00 ",0," \ud800\udc00 ");
			DoTestReadUtf8(new byte[]{0x20,(byte)0xf0,(byte)0x90,(byte)0x80,0x20},
			               0," \ufffd ",-1,null);
			DoTestReadUtf8(new byte[]{0x20,(byte)0xf0,(byte)0x90,0x20},
			               0," \ufffd ",-1,null);
			DoTestReadUtf8(new byte[]{0x20,(byte)0xf0,(byte)0x90,(byte)0x80,(byte)0xff},
			               0," \ufffd\ufffd",-1,null);
			DoTestReadUtf8(new byte[]{0x20,(byte)0xf0,(byte)0x90,(byte)0xff},
			               0," \ufffd\ufffd",-1,null);
			DoTestReadUtf8(new byte[]{0x20,(byte)0xe0,(byte)0xa0,0x20},
			               0," \ufffd ",-1,null);
			DoTestReadUtf8(new byte[]{0x20,(byte)0xe0,0x20},
			               0," \ufffd ",-1,null);
			DoTestReadUtf8(new byte[]{0x20,(byte)0xe0,(byte)0xa0,(byte)0xff},
			               0," \ufffd\ufffd",-1,null);
			DoTestReadUtf8(new byte[]{0x20,(byte)0xe0,(byte)0xff},
			               0," \ufffd\ufffd",-1,null);
		}
		
		@Test
		public void TestArray(){
			CBORObject cbor=CBORObject.FromJSONString("[]");
			cbor.Add(CBORObject.FromObject(3));
			cbor.Add(CBORObject.FromObject(4));
			byte[] bytes=cbor.EncodeToBytes();
			Assert.assertArrayEquals(new byte[]{(byte)(0x80|2),3,4},bytes);
		}
		@Test
		public void TestMap(){
			CBORObject cbor=CBORObject.FromJSONString("{\"a\":2,\"b\":4}");
			Assert.assertEquals(2,cbor.size());
			TestCommon.AssertEqualsHashCode(
				CBORObject.FromObject(2),
				cbor.get(CBORObject.FromObject("a")));
			TestCommon.AssertEqualsHashCode(
				CBORObject.FromObject(4),
				cbor.get(CBORObject.FromObject("b")));
			Assert.assertEquals(2,cbor.get(CBORObject.FromObject("a")).AsInt32());
			Assert.assertEquals(4,cbor.get(CBORObject.FromObject("b")).AsInt32());
		}
		
		
		private static String Repeat(char c, int num){
			StringBuilder sb=new StringBuilder();
			for(int i=0;i<num;i++){
				sb.append(c);
			}
			return sb.toString();
		}
		
		private static String Repeat(String c, int num){
			StringBuilder sb=new StringBuilder();
			for(int i=0;i<num;i++){
				sb.append(c);
			}
			return sb.toString();
		}
		
		@Test
		public void TestTextStringStream(){
			CBORObject cbor=TestCommon.FromBytesTestAB(
				new byte[]{0x7F,0x61,0x20,0x61,0x20,(byte)0xFF});
			Assert.assertEquals("  ",cbor.AsString());
			// Test streaming of long strings
			String longString=Repeat('x',200000);
			CBORObject cbor2;
			cbor=CBORObject.FromObject(longString);
			cbor2=TestCommon.FromBytesTestAB(cbor.EncodeToBytes());
			TestCommon.AssertEqualsHashCode(cbor,cbor2);
			Assert.assertEquals(longString,cbor2.AsString());
			longString=Repeat('\u00e0',200000);
			cbor=CBORObject.FromObject(longString);
			cbor2=TestCommon.FromBytesTestAB(cbor.EncodeToBytes());
			TestCommon.AssertEqualsHashCode(cbor,cbor2);
			Assert.assertEquals(longString,cbor2.AsString());
			longString=Repeat('\u3000',200000);
			cbor=CBORObject.FromObject(longString);
			cbor2=TestCommon.FromBytesTestAB(cbor.EncodeToBytes());
			TestCommon.AssertEqualsHashCode(cbor,cbor2);
			Assert.assertEquals(longString,cbor2.AsString());
			longString=Repeat("\ud800\udc00",200000);
			cbor=CBORObject.FromObject(longString);
			cbor2=TestCommon.FromBytesTestAB(cbor.EncodeToBytes());
			TestCommon.AssertEqualsHashCode(cbor,cbor2);
			Assert.assertEquals(longString,cbor2.AsString());
		}

		@Test
		public void TestTextStringStreamNoTagsBeforeDefinite() {
try {

			TestCommon.FromBytesTestAB(
				new byte[]{0x7F,0x61,0x20,(byte)0xC0,0x61,0x20,(byte)0xFF});
		
} catch(Exception ex){
if(!(ex instanceof CBORException))Assert.fail(ex.toString());
}
}

		@Test
		public void TestTextStringStreamNoIndefiniteWithinDefinite() {
try {

			TestCommon.FromBytesTestAB(
				new byte[]{0x7F,0x61,0x20,0x7F,0x61,0x20,(byte)0xFF,(byte)0xFF});
		
} catch(Exception ex){
if(!(ex instanceof CBORException))Assert.fail(ex.toString());
}
}
		@Test
		public void TestByteStringStream(){
			TestCommon.FromBytesTestAB(
				new byte[]{0x5F,0x41,0x20,0x41,0x20,(byte)0xFF});
		}
		@Test
		public void TestByteStringStreamNoTagsBeforeDefinite() {
try {

			TestCommon.FromBytesTestAB(
				new byte[]{0x5F,0x41,0x20,(byte)0xC2,0x41,0x20,(byte)0xFF});
		
} catch(Exception ex){
if(!(ex instanceof CBORException))Assert.fail(ex.toString());
}
}

		public static void AssertDecimalsEquivalent(String a, String b){
			CBORObject ca=CBORDataUtilities.ParseJSONNumber(a);
			CBORObject cb=CBORDataUtilities.ParseJSONNumber(b);
			Assert.assertEquals(a+" not equal to "+b,0,ca.compareTo(cb));
		}
		
		@Test
		public void TestDecimalsEquivalent()
		{
			AssertDecimalsEquivalent("1.310E-7","131.0E-9");
			AssertDecimalsEquivalent("0.001231","123.1E-5");
			AssertDecimalsEquivalent("3.0324E+6","303.24E4");
			AssertDecimalsEquivalent("3.726E+8","372.6E6");
			AssertDecimalsEquivalent("2663.6","266.36E1");
			AssertDecimalsEquivalent("34.24","342.4E-1");
			AssertDecimalsEquivalent("3492.5","349.25E1");
			AssertDecimalsEquivalent("0.31919","319.19E-3");
			AssertDecimalsEquivalent("2.936E-7","293.6E-9");
			AssertDecimalsEquivalent("6.735E+10","67.35E9");
			AssertDecimalsEquivalent("7.39E+10","7.39E10");
			AssertDecimalsEquivalent("0.0020239","202.39E-5");
			AssertDecimalsEquivalent("1.6717E+6","167.17E4");
			AssertDecimalsEquivalent("1.7632E+9","176.32E7");
			AssertDecimalsEquivalent("39.526","395.26E-1");
			AssertDecimalsEquivalent("0.002939","29.39E-4");
			AssertDecimalsEquivalent("0.3165","316.5E-3");
			AssertDecimalsEquivalent("3.7910E-7","379.10E-9");
			AssertDecimalsEquivalent("0.000016035","160.35E-7");
			AssertDecimalsEquivalent("0.001417","141.7E-5");
			AssertDecimalsEquivalent("7.337E+5","73.37E4");
			AssertDecimalsEquivalent("3.4232E+12","342.32E10");
			AssertDecimalsEquivalent("2.828E+8","282.8E6");
			AssertDecimalsEquivalent("4.822E-7","48.22E-8");
			AssertDecimalsEquivalent("2.6328E+9","263.28E7");
			AssertDecimalsEquivalent("2.9911E+8","299.11E6");
			AssertDecimalsEquivalent("3.636E+9","36.36E8");
			AssertDecimalsEquivalent("0.20031","200.31E-3");
			AssertDecimalsEquivalent("1.922E+7","19.22E6");
			AssertDecimalsEquivalent("3.0924E+8","309.24E6");
			AssertDecimalsEquivalent("2.7236E+7","272.36E5");
			AssertDecimalsEquivalent("0.01645","164.5E-4");
			AssertDecimalsEquivalent("0.000292","29.2E-5");
			AssertDecimalsEquivalent("1.9939","199.39E-2");
			AssertDecimalsEquivalent("2.7929E+9","279.29E7");
			AssertDecimalsEquivalent("1.213E+7","121.3E5");
			AssertDecimalsEquivalent("2.765E+6","276.5E4");
			AssertDecimalsEquivalent("270.11","270.11E0");
			AssertDecimalsEquivalent("0.017718","177.18E-4");
			AssertDecimalsEquivalent("0.003607","360.7E-5");
			AssertDecimalsEquivalent("0.00038618","386.18E-6");
			AssertDecimalsEquivalent("0.0004230","42.30E-5");
			AssertDecimalsEquivalent("1.8410E+5","184.10E3");
			AssertDecimalsEquivalent("0.00030427","304.27E-6");
			AssertDecimalsEquivalent("6.513E+6","65.13E5");
			AssertDecimalsEquivalent("0.06717","67.17E-3");
			AssertDecimalsEquivalent("0.00031123","311.23E-6");
			AssertDecimalsEquivalent("0.0031639","316.39E-5");
			AssertDecimalsEquivalent("1.146E+5","114.6E3");
			AssertDecimalsEquivalent("0.00039937","399.37E-6");
			AssertDecimalsEquivalent("3.3817","338.17E-2");
			AssertDecimalsEquivalent("0.00011128","111.28E-6");
			AssertDecimalsEquivalent("7.818E+7","78.18E6");
			AssertDecimalsEquivalent("2.6417E-7","264.17E-9");
			AssertDecimalsEquivalent("1.852E+9","185.2E7");
			AssertDecimalsEquivalent("0.0016216","162.16E-5");
			AssertDecimalsEquivalent("2.2813E+6","228.13E4");
			AssertDecimalsEquivalent("3.078E+12","307.8E10");
			AssertDecimalsEquivalent("0.00002235","22.35E-6");
			AssertDecimalsEquivalent("0.0032827","328.27E-5");
			AssertDecimalsEquivalent("1.334E+9","133.4E7");
			AssertDecimalsEquivalent("34.022","340.22E-1");
			AssertDecimalsEquivalent("7.19E+6","7.19E6");
			AssertDecimalsEquivalent("35.311","353.11E-1");
			AssertDecimalsEquivalent("3.4330E+6","343.30E4");
			AssertDecimalsEquivalent("0.000022923","229.23E-7");
			AssertDecimalsEquivalent("2.899E+4","289.9E2");
			AssertDecimalsEquivalent("0.00031","3.1E-4");
			AssertDecimalsEquivalent("2.0418E+5","204.18E3");
			AssertDecimalsEquivalent("3.3412E+11","334.12E9");
			AssertDecimalsEquivalent("1.717E+10","171.7E8");
			AssertDecimalsEquivalent("2.7024E+10","270.24E8");
			AssertDecimalsEquivalent("1.0219E+9","102.19E7");
			AssertDecimalsEquivalent("15.13","151.3E-1");
			AssertDecimalsEquivalent("91.23","91.23E0");
			AssertDecimalsEquivalent("3.4114E+6","341.14E4");
			AssertDecimalsEquivalent("33.832","338.32E-1");
			AssertDecimalsEquivalent("0.19234","192.34E-3");
			AssertDecimalsEquivalent("16835","168.35E2");
			AssertDecimalsEquivalent("0.00038610","386.10E-6");
			AssertDecimalsEquivalent("1.6624E+9","166.24E7");
			AssertDecimalsEquivalent("2.351E+9","235.1E7");
			AssertDecimalsEquivalent("0.03084","308.4E-4");
			AssertDecimalsEquivalent("0.00429","42.9E-4");
			AssertDecimalsEquivalent("9.718E-8","97.18E-9");
			AssertDecimalsEquivalent("0.00003121","312.1E-7");
			AssertDecimalsEquivalent("3.175E+4","317.5E2");
			AssertDecimalsEquivalent("376.6","376.6E0");
			AssertDecimalsEquivalent("0.0000026110","261.10E-8");
			AssertDecimalsEquivalent("7.020E+11","70.20E10");
			AssertDecimalsEquivalent("2.1533E+9","215.33E7");
			AssertDecimalsEquivalent("3.8113E+7","381.13E5");
			AssertDecimalsEquivalent("7.531","75.31E-1");
			AssertDecimalsEquivalent("991.0","99.10E1");
			AssertDecimalsEquivalent("2.897E+8","289.7E6");
			AssertDecimalsEquivalent("0.0000033211","332.11E-8");
			AssertDecimalsEquivalent("0.03169","316.9E-4");
			AssertDecimalsEquivalent("2.7321E+12","273.21E10");
			AssertDecimalsEquivalent("394.38","394.38E0");
			AssertDecimalsEquivalent("5.912E+7","59.12E6");
		}
		
		@Test
		public void TestByteStringStreamNoIndefiniteWithinDefinite() {
try {

			TestCommon.FromBytesTestAB(
				new byte[]{0x5F,0x41,0x20,0x5F,0x41,0x20,(byte)0xFF,(byte)0xFF});
		
} catch(Exception ex){
if(!(ex instanceof CBORException))Assert.fail(ex.toString());
}
}
		
		@Test
		public void TestDecimalFrac(){
			TestCommon.FromBytesTestAB(
				new byte[]{(byte)0xc4,(byte)0x82,0x3,0x1a,1,2,3,4});
		}
		@Test
		public void TestDecimalFracExponentMustNotBeBignum() {
try {

			TestCommon.FromBytesTestAB(
				new byte[]{(byte)0xc4,(byte)0x82,(byte)0xc2,0x41,1,0x1a,1,2,3,4});
		
} catch(Exception ex){
if(!(ex instanceof CBORException))Assert.fail(ex.toString());
}
}
		
		@Test
		public void TestDoubleToOther(){
			CBORObject dbl1=CBORObject.FromObject((double)Integer.MIN_VALUE);
			CBORObject dbl2=CBORObject.FromObject((double)Integer.MAX_VALUE);
			try { dbl1.AsInt16(); } catch(ArithmeticException ex){ } catch(Throwable ex){ Assert.fail(ex.toString()); }
			try { dbl1.AsByte(); } catch(ArithmeticException ex){ } catch(Throwable ex){ Assert.fail(ex.toString()); }
			try { dbl1.AsInt32(); } catch(Throwable ex){ Assert.fail(ex.toString()); }
			try { dbl1.AsInt64(); } catch(Throwable ex){ Assert.fail(ex.toString()); }
			try { dbl1.AsBigInteger(); } catch(Throwable ex){ Assert.fail(ex.toString()); }
			try { dbl2.AsInt16(); } catch(ArithmeticException ex){ } catch(Throwable ex){ Assert.fail(ex.toString()); }
			try { dbl2.AsByte(); } catch(ArithmeticException ex){ } catch(Throwable ex){ Assert.fail(ex.toString()); }
			try { dbl2.AsInt32(); } catch(Throwable ex){ Assert.fail(ex.toString()); }
			try { dbl2.AsInt64(); } catch(Throwable ex){ Assert.fail(ex.toString()); }
			try { dbl2.AsBigInteger(); } catch(Throwable ex){ Assert.fail(ex.toString()); }
		}
		
		@Test
		public void TestBigTag(){
			CBORObject.FromObjectAndTag(CBORObject.Null,new BigInteger("18446744073709551615"));
		}
		
		@Test
		public void TestDecimalFracExactlyTwoElements() {
try {

			TestCommon.FromBytesTestAB(
				new byte[]{(byte)0xc4,(byte)0x82,(byte)0xc2,0x41,1});
		
} catch(Exception ex){
if(!(ex instanceof CBORException))Assert.fail(ex.toString());
}
}
		@Test
		public void TestDecimalFracMantissaMayBeBignum() {
			TestCommon.FromBytesTestAB(
				new byte[]{(byte)0xc4,(byte)0x82,0x3,(byte)0xc2,0x41,1});
		}
		
		@Test
		public void TestShort() {
			for(int i=Short.MIN_VALUE;i<=Short.MAX_VALUE;i++){
				TestCommon.AssertSer(
					CBORObject.FromObject((short)i),
					String.format(java.util.Locale.US,"%s",i));
			}
		}
		@Test
		public void TestBigInteger() {
			BigInteger bi=BigInteger.valueOf(3);
			BigInteger negseven=BigInteger.valueOf((-7));
			for(int i=0;i<500;i++){
				TestCommon.AssertSer(
					CBORObject.FromObject(bi),
					String.format(java.util.Locale.US,"%s",bi));
				bi=bi.multiply(negseven);
			}
			BigInteger[] ranges=new BigInteger[]{
				BigInteger.valueOf(Long.MIN_VALUE).subtract(BigInteger.valueOf(512)),
				BigInteger.valueOf(Long.MIN_VALUE).add(BigInteger.valueOf(512)),
				new BigInteger("-18446744073709551616").subtract(BigInteger.valueOf(512)),
				new BigInteger("-18446744073709551616").add(BigInteger.valueOf(512)),
				BigInteger.valueOf(Long.MAX_VALUE).subtract(BigInteger.valueOf(512)),
				BigInteger.valueOf(Long.MAX_VALUE).add(BigInteger.valueOf(512)),
				new BigInteger("18446744073709551615").subtract(BigInteger.valueOf(512)),
				new BigInteger("18446744073709551615").add(BigInteger.valueOf(512)),
			};
			for(int i=0;i<ranges.length;i+=2){
				BigInteger bigintTemp=ranges[i];
				while(true){
					TestCommon.AssertSer(
						CBORObject.FromObject(bigintTemp),
						String.format(java.util.Locale.US,"%s",bigintTemp));
					if(bigintTemp.equals(ranges[i+1]))break;
					bigintTemp=bigintTemp.add(BigInteger.ONE);
				}
			}
		}
		@Test
		public void TestLong() {
			long[] ranges=new long[]{
				-65539,65539,
				0xFFFFF000L,0x100000400L,
				Long.MAX_VALUE-1000,Long.MAX_VALUE,
				Long.MIN_VALUE,Long.MIN_VALUE+1000
			};
			for(int i=0;i<ranges.length;i+=2){
				long j=ranges[i];
				while(true){
					TestCommon.AssertSer(
						CBORObject.FromObject(j),
						String.format(java.util.Locale.US,"%s",j));
					Assert.assertEquals(
						CBORObject.FromObject(j),
						CBORObject.FromObject(BigInteger.valueOf(j)));
					CBORObject obj=CBORObject.FromJSONString(
						String.format(java.util.Locale.US,"[%s]",j));
					TestCommon.AssertSer(obj,
					                     String.format(java.util.Locale.US,"[%s]",j));
					if(j==ranges[i+1])break;
					j++;
				}
			}
		}

		@Test
		public void TestFloat() {
			TestCommon.AssertSer(CBORObject.FromObject(Float.POSITIVE_INFINITY),
			                     "Infinity");
			TestCommon.AssertSer(CBORObject.FromObject(Float.NEGATIVE_INFINITY),
			                     "-Infinity");
			TestCommon.AssertSer(CBORObject.FromObject(Float.NaN),
			                     "NaN");
			for(int i=-65539;i<=65539;i++){
				TestCommon.AssertSer(
					CBORObject.FromObject((float)i),
					String.format(java.util.Locale.US,"%s",i));
			}
		}
		
		@Test
		public void TestSimpleValues() {
			TestCommon.AssertSer(CBORObject.FromObject(true),
			                     "true");
			TestCommon.AssertSer(CBORObject.FromObject(false),
			                     "false");
			TestCommon.AssertSer(CBORObject.FromObject((Object)null),
			                     "null");
		}
		
		@Test
		public void TestDouble() {
			TestCommon.AssertSer(CBORObject.FromObject(Double.POSITIVE_INFINITY),
			                     "Infinity");
			TestCommon.AssertSer(CBORObject.FromObject(Double.NEGATIVE_INFINITY),
			                     "-Infinity");
			TestCommon.AssertSer(CBORObject.FromObject(Double.NaN),
			                     "NaN");
			for(int i=-65539;i<=65539;i++){
				TestCommon.AssertSer(
					CBORObject.FromObject((double)i),
					String.format(java.util.Locale.US,"%s",i));
			}
		}


		@Test
		public void TestTags() {
			BigInteger maxuint=new BigInteger("18446744073709551615");
			BigInteger[] ranges=new BigInteger[]{
				BigInteger.valueOf(5),
				BigInteger.valueOf(65539),
				BigInteger.valueOf(Integer.MAX_VALUE).subtract(BigInteger.valueOf(500)),
				BigInteger.valueOf(Integer.MAX_VALUE).add(BigInteger.valueOf(500)),
				BigInteger.valueOf(Long.MAX_VALUE).subtract(BigInteger.valueOf(500)),
				BigInteger.valueOf(Long.MAX_VALUE).add(BigInteger.valueOf(500)),
				new BigInteger("18446744073709551615").subtract(BigInteger.valueOf(500)),
				maxuint,
			};
			for(int i=0;i<ranges.length;i+=2){
				BigInteger bigintTemp=ranges[i];
				while(true){
					CBORObject obj=CBORObject.FromObjectAndTag(0,bigintTemp);
					if(!(obj.isTagged()))Assert.fail("obj not tagged");
					if(!obj.getInnermostTag().equals(bigintTemp))
						Assert.assertEquals(String.format(java.util.Locale.US,"obj tag doesn't match: %s",obj),bigintTemp,obj.getInnermostTag());
					TestCommon.AssertSer(
						obj,
						String.format(java.util.Locale.US,"%s(0)",bigintTemp));
					if(!(bigintTemp.equals(maxuint))){
						// Test multiple tags
						CBORObject obj2=CBORObject.FromObjectAndTag(obj,bigintTemp.add(BigInteger.ONE));
						BigInteger[] bi=obj2.GetTags();
						if(bi.length!=2)
							Assert.assertEquals(String.format(java.util.Locale.US,"Expected 2 tags: %s",obj2),2,bi.length);
						if(!bi[0].equals((BigInteger)bigintTemp.add(BigInteger.ONE)))
							Assert.assertEquals(String.format(java.util.Locale.US,"Outer tag doesn't match: %s",obj2),bigintTemp.add(BigInteger.ONE),bi[0]);
						if(!(bi[1].equals(bigintTemp)))
							Assert.assertEquals(String.format(java.util.Locale.US,"Inner tag doesn't match: %s",obj2),bigintTemp,bi[1]);
						if(!(obj2.getInnermostTag().equals(bigintTemp)))
							Assert.assertEquals(String.format(java.util.Locale.US,"Innermost tag doesn't match: %s",obj2),bigintTemp,bi[0]);
						TestCommon.AssertSer(
							obj2,
							String.format(java.util.Locale.US,"%s(%s(0))",
							              bigintTemp.add(BigInteger.ONE),bigintTemp));
					}
					if(bigintTemp.equals(ranges[i+1]))break;
					bigintTemp=bigintTemp.add(BigInteger.ONE);
				}
			}
		}
		
	}
