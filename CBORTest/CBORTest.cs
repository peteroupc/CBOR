/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Globalization;
using System.Numerics;
using System.IO;
using PeterO;
using System.Text;

using NUnit.Framework;

namespace Test
{
	
	[TestFixture]
	public class CBORTest
	{
		private void TestBigFloatDoubleCore(double d, string s){
			double oldd=d;
			BigFloat bf=BigFloat.FromDouble(d);
			if(s!=null){
				Assert.AreEqual(s,bf.ToString());
			}
			d=bf.ToDouble();
			Assert.AreEqual((double)oldd,d);
			TestCommon.AssertRoundTrip(CBORObject.FromObject(bf));
			TestCommon.AssertRoundTrip(CBORObject.FromObject(d));
		}
		
		private void TestBigFloatSingleCore(float d, string s){
			float oldd=d;
			BigFloat bf=BigFloat.FromSingle(d);
			if(s!=null){
				Assert.AreEqual(s,bf.ToString());
			}
			d=bf.ToSingle();
			Assert.AreEqual((float)oldd,d);
			TestCommon.AssertRoundTrip(CBORObject.FromObject(bf));
			TestCommon.AssertRoundTrip(CBORObject.FromObject(d));
		}

		private double RandomDouble(System.Random rand, int exponent){
			long r=rand.Next(0x10000);
			r|=((long)rand.Next(0x10000))<<16;
			if(rand.Next(2)==0){
				r|=((long)rand.Next(0x10000))<<32;
				if(rand.Next(2)==0){
					r|=((long)rand.Next(0x10000))<<48;
				}
			}
			r&=~0x7FF0000000000000L; // clear exponent
			r|=((long)exponent)<<52; // set exponent
			return ConverterInternal.Int64BitsToDouble(r);
		}
		
		private float RandomSingle(System.Random rand, int exponent){
			int r=rand.Next(0x10000);
			if(rand.Next(2)==0){
				r|=((int)rand.Next(0x10000))<<16;
			}
			r&=~0x7F800000; // clear exponent
			r|=((int)exponent)<<23; // set exponent
			return ConverterInternal.Int32BitsToSingle(r);
		}

		[Test]
		public void TestBigFloatSingle(){
			System.Random rand=new System.Random();
			for(int i=0;i<255;i++){ // Try a random float with a given exponent
				TestBigFloatSingleCore(RandomSingle(rand,i),null);
				TestBigFloatSingleCore(RandomSingle(rand,i),null);
				TestBigFloatSingleCore(RandomSingle(rand,i),null);
				TestBigFloatSingleCore(RandomSingle(rand,i),null);
			}
		}

		[Test]
		public void TestBigFloatDouble(){
			TestBigFloatDoubleCore(3.5,"3.5");
			TestBigFloatDoubleCore(7,"7");
			TestBigFloatDoubleCore(1.75,"1.75");
			TestBigFloatDoubleCore(3.5,"3.5");
			System.Random rand=new System.Random();
			for(int i=0;i<2047;i++){ // Try a random double with a given exponent
				TestBigFloatDoubleCore(RandomDouble(rand,i),null);
				TestBigFloatDoubleCore(RandomDouble(rand,i),null);
				TestBigFloatDoubleCore(RandomDouble(rand,i),null);
				TestBigFloatDoubleCore(RandomDouble(rand,i),null);
			}
		}
		
		
		[Test]
		[ExpectedException(typeof(CBORException))]
		public void TestTagThenBreak(){
			TestCommon.FromBytesTestAB(new byte[]{0xD1,0xFF});
		}
		
		[Test]
		public void TestJSON(){
			CBORObject o;
			o=CBORObject.FromJSONString("[1,2,3]");
			Assert.AreEqual(3,o.Count);
			Assert.AreEqual(1,o[0].AsInt32());
			Assert.AreEqual(2,o[1].AsInt32());
			Assert.AreEqual(3,o[2].AsInt32());
			o=CBORObject.FromJSONString("[1.5,2.6,3.7,4.0,222.22]");
			double actual=o[0].AsDouble();
			Assert.AreEqual((double)1.5,actual);
		}
		
		[Test]
		public void TestByte(){
			for(int i=0;i<=255;i++){
				TestCommon.AssertSer(
					CBORObject.FromObject((byte)i),
					String.Format(CultureInfo.InvariantCulture,"{0}",i));
			}
		}
		
		public void DoTestReadUtf8(byte[] bytes,
		                           int expectedRet, string expectedString,
		                           int noReplaceRet, string noReplaceString
		                          ){
			DoTestReadUtf8(bytes,bytes.Length,expectedRet,expectedString,
			               noReplaceRet,noReplaceString);
		}

		public void DoTestReadUtf8(byte[] bytes,int length,
		                           int expectedRet, string expectedString,
		                           int noReplaceRet, string noReplaceString
		                          ){
			try {
				StringBuilder builder=new StringBuilder();
				int ret=0;
				using(MemoryStream ms=new MemoryStream(bytes)){
					ret=CBORDataUtilities.ReadUtf8(ms,length,builder,true);
					Assert.AreEqual(expectedRet,ret);
					if(expectedRet==0){
						Assert.AreEqual(expectedString,builder.ToString());
					}
					ms.Position=0;
					builder.Clear();
					ret=CBORDataUtilities.ReadUtf8(ms,length,builder,false);
					Assert.AreEqual(noReplaceRet,ret);
					if(noReplaceRet==0){
						Assert.AreEqual(noReplaceString,builder.ToString());
					}
				}
				if(bytes.Length>=length){
					builder.Clear();
					ret=CBORDataUtilities.ReadUtf8FromBytes(bytes,0,length,builder,true);
					Assert.AreEqual(expectedRet,ret);
					if(expectedRet==0){
						Assert.AreEqual(expectedString,builder.ToString());
					}
					builder.Clear();
					ret=CBORDataUtilities.ReadUtf8FromBytes(bytes,0,length,builder,false);
					Assert.AreEqual(noReplaceRet,ret);
					if(noReplaceRet==0){
						Assert.AreEqual(noReplaceString,builder.ToString());
					}
				}
			} catch(IOException ex){
				throw new CBORException("",ex);
			}
		}
		
		[Test]
		public void TestFPToBigInteger(){
			Assert.AreEqual("0",CBORObject.FromObject((float)0.75).AsBigInteger().ToString());
			Assert.AreEqual("0",CBORObject.FromObject((float)0.99).AsBigInteger().ToString());
			Assert.AreEqual("0",CBORObject.FromObject((float)0.0000000000000001).AsBigInteger().ToString());
			Assert.AreEqual("0",CBORObject.FromObject((float)0.5).AsBigInteger().ToString());
			Assert.AreEqual("1",CBORObject.FromObject((float)1.5).AsBigInteger().ToString());
			Assert.AreEqual("2",CBORObject.FromObject((float)2.5).AsBigInteger().ToString());
			Assert.AreEqual("328323",CBORObject.FromObject((float)328323f).AsBigInteger().ToString());
			Assert.AreEqual("0",CBORObject.FromObject((double)0.75).AsBigInteger().ToString());
			Assert.AreEqual("0",CBORObject.FromObject((double)0.99).AsBigInteger().ToString());
			Assert.AreEqual("0",CBORObject.FromObject((double)0.0000000000000001).AsBigInteger().ToString());
			Assert.AreEqual("0",CBORObject.FromObject((double)0.5).AsBigInteger().ToString());
			Assert.AreEqual("1",CBORObject.FromObject((double)1.5).AsBigInteger().ToString());
			Assert.AreEqual("2",CBORObject.FromObject((double)2.5).AsBigInteger().ToString());
			Assert.AreEqual("328323",CBORObject.FromObject((double)328323).AsBigInteger().ToString());
			Assert.Throws(typeof(OverflowException),()=>CBORObject.FromObject(Single.PositiveInfinity).AsBigInteger());
			Assert.Throws(typeof(OverflowException),()=>CBORObject.FromObject(Single.NegativeInfinity).AsBigInteger());
			Assert.Throws(typeof(OverflowException),()=>CBORObject.FromObject(Single.NaN).AsBigInteger());
			Assert.Throws(typeof(OverflowException),()=>CBORObject.FromObject(Double.PositiveInfinity).AsBigInteger());
			Assert.Throws(typeof(OverflowException),()=>CBORObject.FromObject(Double.NegativeInfinity).AsBigInteger());
			Assert.Throws(typeof(OverflowException),()=>CBORObject.FromObject(Double.NaN).AsBigInteger());
		}
		
		[Test]
		public void TestDecFracFP(){
			Assert.AreEqual("0.75",DecimalFraction.FromDouble(0.75).ToString());
			Assert.AreEqual("0.5",DecimalFraction.FromDouble(0.5).ToString());
			Assert.AreEqual("0.25",DecimalFraction.FromDouble(0.25).ToString());
			Assert.AreEqual("0.875",DecimalFraction.FromDouble(0.875).ToString());
			Assert.AreEqual("0.125",DecimalFraction.FromDouble(0.125).ToString());
			Assert.AreEqual("0.75",DecimalFraction.FromSingle(0.75f).ToString());
			Assert.AreEqual("0.5",DecimalFraction.FromSingle(0.5f).ToString());
			Assert.AreEqual("0.25",DecimalFraction.FromSingle(0.25f).ToString());
			Assert.AreEqual("0.875",DecimalFraction.FromSingle(0.875f).ToString());
			Assert.AreEqual("0.125",DecimalFraction.FromSingle(0.125f).ToString());
		}
		
		[Test]
		public void TestReadUtf8(){
			DoTestReadUtf8(new byte[]{0x20,0x20,0x20},
			               0,"   ",0,"   ");
			DoTestReadUtf8(new byte[]{0x20,0xc2,0x80},
			               0," \u0080",0," \u0080");
			DoTestReadUtf8(new byte[]{0x20,0xc2,0x80,0x20},
			               0," \u0080 ",0," \u0080 ");
			DoTestReadUtf8(new byte[]{0x20,0xc2,0x80,0xc2},
			               0," \u0080\ufffd",-1,null);
			DoTestReadUtf8(new byte[]{0x20,0xc2,0x20,0x20},
			               0," \ufffd  ",-1,null);
			DoTestReadUtf8(new byte[]{0x20,0xc2,0xff,0x20},
			               0," \ufffd\ufffd ",-1,null);
			DoTestReadUtf8(new byte[]{0x20,0xe0,0xa0,0x80},
			               0," \u0800",0," \u0800");
			DoTestReadUtf8(new byte[]{0x20,0xe0,0xa0,0x80,0x20},
			               0," \u0800 ",0," \u0800 ");
			DoTestReadUtf8(new byte[]{0x20,0xf0,0x90,0x80,0x80},
			               0," \ud800\udc00",0," \ud800\udc00");
			DoTestReadUtf8(new byte[]{0x20,0xf0,0x90,0x80,0x80},3,
			               0," \ufffd",-1,null);
			DoTestReadUtf8(new byte[]{0x20,0xf0,0x90},5,
			               -2,null,-1,null);
			DoTestReadUtf8(new byte[]{0x20,0x20,0x20},5,
			               -2,null,-2,null);
			DoTestReadUtf8(new byte[]{0x20,0xf0,0x90,0x80,0x80,0x20},
			               0," \ud800\udc00 ",0," \ud800\udc00 ");
			DoTestReadUtf8(new byte[]{0x20,0xf0,0x90,0x80,0x20},
			               0," \ufffd ",-1,null);
			DoTestReadUtf8(new byte[]{0x20,0xf0,0x90,0x20},
			               0," \ufffd ",-1,null);
			DoTestReadUtf8(new byte[]{0x20,0xf0,0x90,0x80,0xff},
			               0," \ufffd\ufffd",-1,null);
			DoTestReadUtf8(new byte[]{0x20,0xf0,0x90,0xff},
			               0," \ufffd\ufffd",-1,null);
			DoTestReadUtf8(new byte[]{0x20,0xe0,0xa0,0x20},
			               0," \ufffd ",-1,null);
			DoTestReadUtf8(new byte[]{0x20,0xe0,0x20},
			               0," \ufffd ",-1,null);
			DoTestReadUtf8(new byte[]{0x20,0xe0,0xa0,0xff},
			               0," \ufffd\ufffd",-1,null);
			DoTestReadUtf8(new byte[]{0x20,0xe0,0xff},
			               0," \ufffd\ufffd",-1,null);
		}
		
		[Test]
		public void TestArray(){
			CBORObject cbor=CBORObject.FromJSONString("[]");
			cbor.Add(CBORObject.FromObject(3));
			cbor.Add(CBORObject.FromObject(4));
			byte[] bytes=cbor.EncodeToBytes();
			Assert.AreEqual(
				new byte[]{(byte)(0x80|2),3,4},bytes);
		}
		[Test]
		public void TestMap(){
			CBORObject cbor=CBORObject.FromJSONString("{\"a\":2,\"b\":4}");
			Assert.AreEqual(2,cbor.Count);
			TestCommon.AssertEqualsHashCode(
				CBORObject.FromObject(2),
				cbor[CBORObject.FromObject("a")]);
			TestCommon.AssertEqualsHashCode(
				CBORObject.FromObject(4),
				cbor[CBORObject.FromObject("b")]);
			Assert.AreEqual(2,cbor[CBORObject.FromObject("a")].AsInt32());
			Assert.AreEqual(4,cbor[CBORObject.FromObject("b")].AsInt32());
		}
		
		
		private static String Repeat(char c, int num){
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			for(int i=0;i<num;i++){
				sb.Append(c);
			}
			return sb.ToString();
		}
		
		private static String Repeat(String c, int num){
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			for(int i=0;i<num;i++){
				sb.Append(c);
			}
			return sb.ToString();
		}
		
		[Test]
		public void TestTextStringStream(){
			CBORObject cbor=TestCommon.FromBytesTestAB(
				new byte[]{0x7F,0x61,0x20,0x61,0x20,0xFF});
			Assert.AreEqual("  ",cbor.AsString());
			// Test streaming of long strings
			string longString=Repeat('x',200000);
			CBORObject cbor2;
			cbor=CBORObject.FromObject(longString);
			cbor2=TestCommon.FromBytesTestAB(cbor.EncodeToBytes());
			TestCommon.AssertEqualsHashCode(cbor,cbor2);
			Assert.AreEqual(longString,cbor2.AsString());
			longString=Repeat('\u00e0',200000);
			cbor=CBORObject.FromObject(longString);
			cbor2=TestCommon.FromBytesTestAB(cbor.EncodeToBytes());
			TestCommon.AssertEqualsHashCode(cbor,cbor2);
			Assert.AreEqual(longString,cbor2.AsString());
			longString=Repeat('\u3000',200000);
			cbor=CBORObject.FromObject(longString);
			cbor2=TestCommon.FromBytesTestAB(cbor.EncodeToBytes());
			TestCommon.AssertEqualsHashCode(cbor,cbor2);
			Assert.AreEqual(longString,cbor2.AsString());
			longString=Repeat("\ud800\udc00",200000);
			cbor=CBORObject.FromObject(longString);
			cbor2=TestCommon.FromBytesTestAB(cbor.EncodeToBytes());
			TestCommon.AssertEqualsHashCode(cbor,cbor2);
			Assert.AreEqual(longString,cbor2.AsString());
		}

		[Test]
		[ExpectedException(typeof(CBORException))]
		public void TestTextStringStreamNoTagsBeforeDefinite(){
			TestCommon.FromBytesTestAB(
				new byte[]{0x7F,0x61,0x20,0xC0,0x61,0x20,0xFF});
		}

		[Test]
		[ExpectedException(typeof(CBORException))]
		public void TestTextStringStreamNoIndefiniteWithinDefinite(){
			TestCommon.FromBytesTestAB(
				new byte[]{0x7F,0x61,0x20,0x7F,0x61,0x20,0xFF,0xFF});
		}
		[Test]
		public void TestByteStringStream(){
			TestCommon.FromBytesTestAB(
				new byte[]{0x5F,0x41,0x20,0x41,0x20,0xFF});
		}
		[Test]
		[ExpectedException(typeof(CBORException))]
		public void TestByteStringStreamNoTagsBeforeDefinite(){
			TestCommon.FromBytesTestAB(
				new byte[]{0x5F,0x41,0x20,0xC2,0x41,0x20,0xFF});
		}

		public static void AssertDecimalsEquivalent(string a, string b){
			CBORObject ca=CBORDataUtilities.ParseJSONNumber(a);
			CBORObject cb=CBORDataUtilities.ParseJSONNumber(b);
			Assert.AreEqual(0,ca.CompareTo(cb),a+" not equal to "+b);
		}
		
		[Test]
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
		
		[Test]
		[ExpectedException(typeof(CBORException))]
		public void TestByteStringStreamNoIndefiniteWithinDefinite(){
			TestCommon.FromBytesTestAB(
				new byte[]{0x5F,0x41,0x20,0x5F,0x41,0x20,0xFF,0xFF});
		}
		
		
		[Test]
		public void TestBigFloatDecFrac(){
			BigFloat bf;
			bf=new BigFloat(20);
			Assert.AreEqual("20",DecimalFraction.FromBigFloat(bf).ToString());
			bf=new BigFloat(-1,(BigInteger)3);
			Assert.AreEqual("1.5",DecimalFraction.FromBigFloat(bf).ToString());
			bf=new BigFloat(-1,(BigInteger)(-3));
			Assert.AreEqual("-1.5",DecimalFraction.FromBigFloat(bf).ToString());
			DecimalFraction df;
			df=new DecimalFraction(20);
			Assert.AreEqual("20",BigFloat.FromDecimalFraction(df).ToString());
			df=new DecimalFraction(-20);
			Assert.AreEqual("-20",BigFloat.FromDecimalFraction(df).ToString());
			df=new DecimalFraction(-1,(BigInteger)15);
			Assert.AreEqual("1.5",BigFloat.FromDecimalFraction(df).ToString());
			df=new DecimalFraction(-1,(BigInteger)(-15));
			Assert.AreEqual("-1.5",BigFloat.FromDecimalFraction(df).ToString());
		}
		
		[Test]
		public void TestDecimalFrac(){
			TestCommon.FromBytesTestAB(
				new byte[]{0xc4,0x82,0x3,0x1a,1,2,3,4});
		}
		[Test]
		[ExpectedException(typeof(CBORException))]
		public void TestDecimalFracExponentMustNotBeBignum(){
			TestCommon.FromBytesTestAB(
				new byte[]{0xc4,0x82,0xc2,0x41,1,0x1a,1,2,3,4});
		}
		
		[Test]
		public void TestDoubleToOther(){
			CBORObject dbl1=CBORObject.FromObject((double)Int32.MinValue);
			CBORObject dbl2=CBORObject.FromObject((double)Int32.MaxValue);
			Assert.Throws(typeof(OverflowException),()=>dbl1.AsInt16());
			Assert.Throws(typeof(OverflowException),()=>dbl1.AsByte());
			Assert.DoesNotThrow(()=>dbl1.AsInt32());
			Assert.DoesNotThrow(()=>dbl1.AsInt64());
			Assert.DoesNotThrow(()=>dbl1.AsBigInteger());
			Assert.Throws(typeof(OverflowException),()=>dbl2.AsInt16());
			Assert.Throws(typeof(OverflowException),()=>dbl2.AsByte());
			Assert.DoesNotThrow(()=>dbl2.AsInt32());
			Assert.DoesNotThrow(()=>dbl2.AsInt64());
			Assert.DoesNotThrow(()=>dbl2.AsBigInteger());
		}
		
		[Test]
		public void TestBigTag(){
			CBORObject.FromObjectAndTag(CBORObject.Null,(BigInteger)UInt64.MaxValue);
		}
		
		[Test]
		[ExpectedException(typeof(CBORException))]
		public void TestDecimalFracExactlyTwoElements(){
			TestCommon.FromBytesTestAB(
				new byte[]{0xc4,0x82,0xc2,0x41,1});
		}
		[Test]
		public void TestDecimalFracMantissaMayBeBignum(){
			TestCommon.FromBytesTestAB(
				new byte[]{0xc4,0x82,0x3,0xc2,0x41,1});
		}
		
		[Test]
		public void TestShort(){
			for(int i=Int16.MinValue;i<=Int16.MaxValue;i++){
				TestCommon.AssertSer(
					CBORObject.FromObject((short)i),
					String.Format(CultureInfo.InvariantCulture,"{0}",i));
			}
		}
		
		[Test]
		public void TestByteArray(){
			TestCommon.AssertSer(
				CBORObject.FromObject(new byte[]{0x20,0x78}),"h'2078'");
		}
		[Test]
		public void TestBigInteger(){
			BigInteger bi=(BigInteger)3;
			BigInteger negseven=(BigInteger)(-7);
			for(int i=0;i<500;i++){
				TestCommon.AssertSer(
					CBORObject.FromObject(bi),
					String.Format(CultureInfo.InvariantCulture,"{0}",bi));
				bi*=(BigInteger)negseven;
			}
			BigInteger[] ranges=new BigInteger[]{
				(BigInteger)Int64.MinValue-(BigInteger)512,
				(BigInteger)Int64.MinValue+(BigInteger)512,
				(BigInteger)UInt64.MinValue-(BigInteger)512,
				(BigInteger)UInt64.MinValue+(BigInteger)512,
				(BigInteger)Int64.MaxValue-(BigInteger)512,
				(BigInteger)Int64.MaxValue+(BigInteger)512,
				(BigInteger)UInt64.MaxValue-(BigInteger)512,
				(BigInteger)UInt64.MaxValue+(BigInteger)512,
			};
			for(int i=0;i<ranges.Length;i+=2){
				BigInteger bigintTemp=ranges[i];
				while(true){
					TestCommon.AssertSer(
						CBORObject.FromObject(bigintTemp),
						String.Format(CultureInfo.InvariantCulture,"{0}",bigintTemp));
					if(bigintTemp.Equals(ranges[i+1]))break;
					bigintTemp=bigintTemp+BigInteger.One;
				}
			}
		}
		[Test]
		public void TestLong(){
			long[] ranges=new long[]{
				-65539,65539,
				0xFFFFF000L,0x100000400L,
				Int64.MaxValue-1000,Int64.MaxValue,
				Int64.MinValue,Int64.MinValue+1000
			};
			for(int i=0;i<ranges.Length;i+=2){
				long j=ranges[i];
				while(true){
					TestCommon.AssertSer(
						CBORObject.FromObject(j),
						String.Format(CultureInfo.InvariantCulture,"{0}",j));
					Assert.AreEqual(
						CBORObject.FromObject(j),
						CBORObject.FromObject((BigInteger)j));
					CBORObject obj=CBORObject.FromJSONString(
						String.Format(CultureInfo.InvariantCulture,"[{0}]",j));
					TestCommon.AssertSer(obj,
					                     String.Format(CultureInfo.InvariantCulture,"[{0}]",j));
					if(j==ranges[i+1])break;
					j++;
				}
			}
		}

		[Test]
		public void TestFloat(){
			TestCommon.AssertSer(CBORObject.FromObject(Single.PositiveInfinity),
			                     "Infinity");
			TestCommon.AssertSer(CBORObject.FromObject(Single.NegativeInfinity),
			                     "-Infinity");
			TestCommon.AssertSer(CBORObject.FromObject(Single.NaN),
			                     "NaN");
			for(int i=-65539;i<=65539;i++){
				TestCommon.AssertSer(
					CBORObject.FromObject((float)i),
					String.Format(CultureInfo.InvariantCulture,"{0}",i));
			}
		}
		
		[Test]
		public void TestCodePointCompare(){
			Assert.AreEqual(0,Math.Sign(CBORDataUtilities.CodePointCompare("abc","abc")));
			Assert.AreEqual(0,Math.Sign(CBORDataUtilities.CodePointCompare("\ud800\udc00","\ud800\udc00")));
			Assert.AreEqual(-1,Math.Sign(CBORDataUtilities.CodePointCompare("abc","\ud800\udc00")));
			Assert.AreEqual(-1,Math.Sign(CBORDataUtilities.CodePointCompare("\uf000","\ud800\udc00")));
			Assert.AreEqual(1,Math.Sign(CBORDataUtilities.CodePointCompare("\uf000","\ud800")));
		}
		
		[Test]
		public void TestSimpleValues(){
			TestCommon.AssertSer(CBORObject.FromObject(true),
			                     "true");
			TestCommon.AssertSer(CBORObject.FromObject(false),
			                     "false");
			TestCommon.AssertSer(CBORObject.FromObject((Object)null),
			                     "null");
		}
		
		[Test]
		public void TestGetUtf8Length(){
			Assert.Throws(typeof(ArgumentNullException),()=>CBORDataUtilities.GetUtf8Length(null,true));
			Assert.Throws(typeof(ArgumentNullException),()=>CBORDataUtilities.GetUtf8Length(null,false));
			Assert.AreEqual(3,CBORDataUtilities.GetUtf8Length("abc",true));
			Assert.AreEqual(6,CBORDataUtilities.GetUtf8Length("\ud800\ud800",true));
			Assert.AreEqual(-1,CBORDataUtilities.GetUtf8Length("\ud800\ud800",false));
		}
		
		[Test]
		public void TestDouble(){
			TestCommon.AssertSer(CBORObject.FromObject(Double.PositiveInfinity),
			                     "Infinity");
			TestCommon.AssertSer(CBORObject.FromObject(Double.NegativeInfinity),
			                     "-Infinity");
			TestCommon.AssertSer(CBORObject.FromObject(Double.NaN),
			                     "NaN");
			CBORObject oldobj=null;
			for(int i=-65539;i<=65539;i++){
				CBORObject o=CBORObject.FromObject((double)i);
				TestCommon.AssertSer(o,
				                     String.Format(CultureInfo.InvariantCulture,"{0}",i));
				if(oldobj!=null){
					Assert.AreEqual(1,o.CompareTo(oldobj));
					Assert.AreEqual(-1,oldobj.CompareTo(o));
				}
				oldobj=o;
			}
		}


		[Test]
		public void TestTags(){
			BigInteger maxuint=(BigInteger)UInt64.MaxValue;
			BigInteger[] ranges=new BigInteger[]{
				(BigInteger)6,
				(BigInteger)65539,
				(BigInteger)Int32.MaxValue-(BigInteger)500,
				(BigInteger)Int32.MaxValue+(BigInteger)500,
				(BigInteger)Int64.MaxValue-(BigInteger)500,
				(BigInteger)Int64.MaxValue+(BigInteger)500,
				(BigInteger)UInt64.MaxValue-(BigInteger)500,
				maxuint,
			};
			for(int i=0;i<ranges.Length;i+=2){
				BigInteger bigintTemp=ranges[i];
				while(true){
					CBORObject obj=CBORObject.FromObjectAndTag(0,bigintTemp);
					Assert.IsTrue(obj.IsTagged,"obj not tagged");
					BigInteger[] tags=obj.GetTags();
					Assert.AreEqual(1,tags.Length);
					Assert.AreEqual(bigintTemp,tags[0]);
					if(!obj.InnermostTag.Equals(bigintTemp))
						Assert.AreEqual(bigintTemp,obj.InnermostTag,
						                String.Format(CultureInfo.InvariantCulture,
						                              "obj tag doesn't match: {0}",obj));
					TestCommon.AssertSer(
						obj,
						String.Format(CultureInfo.InvariantCulture,"{0}(0)",bigintTemp));
					if(!(bigintTemp.Equals(maxuint))){
						// Test multiple tags
						CBORObject obj2=CBORObject.FromObjectAndTag(obj,bigintTemp+BigInteger.One);
						BigInteger[] bi=obj2.GetTags();
						if(bi.Length!=2)
							Assert.AreEqual(2,bi.Length,
							                String.Format(CultureInfo.InvariantCulture,
							                              "Expected 2 tags: {0}",obj2));
						if(!bi[0].Equals((BigInteger)bigintTemp+BigInteger.One))
							Assert.AreEqual(bigintTemp+BigInteger.One,bi[0],
							                String.Format(CultureInfo.InvariantCulture,
							                              "Outer tag doesn't match: {0}",obj2));
						if(!(bi[1]==(BigInteger)bigintTemp))
							Assert.AreEqual(bigintTemp,bi[1],
							                String.Format(CultureInfo.InvariantCulture,
							                              "Inner tag doesn't match: {0}",obj2));
						if(!(obj2.InnermostTag==(BigInteger)bigintTemp))
							Assert.AreEqual(bigintTemp,bi[0],
							                String.Format(CultureInfo.InvariantCulture,
							                              "Innermost tag doesn't match: {0}",obj2));
						TestCommon.AssertSer(
							obj2,
							String.Format(CultureInfo.InvariantCulture,"{0}({1}(0))",
							              bigintTemp+BigInteger.One,bigintTemp));
					}
					if(bigintTemp.Equals(ranges[i+1]))break;
					bigintTemp=bigintTemp+BigInteger.One;
				}
			}
		}
		
	}
}