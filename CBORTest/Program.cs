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

using NUnit.Framework;

namespace PeterO
{
	
	[TestFixture]
	class CBORTest
	{
		private static CultureInfo Inv=System.Globalization.CultureInfo.InvariantCulture;
		public static void AssertEqualsHashCode(CBORObject o, CBORObject o2){
			if(o.Equals(o2)){
				Assert.IsTrue(o2.Equals(o),
				              "{0} equals {1}, but {1} does not equal {0}",o,o2);
				// Test for the guarantee that equal objects
				// must have equal hash codes
				Assert.AreEqual(o2.GetHashCode(),o.GetHashCode(),
				                "{0} and {1} don't have equal hash codes",o,o2);
			} else {
				Assert.IsFalse(o2.Equals(o),
				               "{0} does not equal {1}, but {1} equals {0}",o,o2);
			}
		}
		public static void AssertSer(CBORObject o, String s){
			Assert.AreEqual(s,o.ToString());
			CBORObject o2=CBORObject.FromBytes(o.ToBytes());
			Assert.AreEqual(s,o2.ToString());
			AssertEqualsHashCode(o,o2);
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
			Assert.AreEqual(1.5,o[0].AsDouble());
			Assert.AreEqual("[4([-1, 15]), 4([-1, 26]), 4([-1, 37]), 4, 4([-2, 22222])]",
			                o.ToString());
		}
		
		[Test]
		public void TestByte(){
			for(int i=0;i<=Byte.MaxValue;i++){
				AssertSer(
					CBORObject.FromObject((byte)i),
					String.Format(Inv,"{0}",i));
			}
		}
		
		[Test]
		public void TestSByte(){
			for(int i=SByte.MinValue;i<=SByte.MaxValue;i++){
				AssertSer(
					CBORObject.FromObject((sbyte)i),
					String.Format(Inv,"{0}",i));
			}
		}
		
		public static String DateTimeToString(DateTime bi){
			DateTime dt=bi.ToUniversalTime();
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append(String.Format(
				CultureInfo.InvariantCulture,
				"{0:d4}-{1:d2}-{2:d2}T{3:d2}:{4:d2}:{5:d2}",
				dt.Year,dt.Month,dt.Day,dt.Hour,
				dt.Minute,dt.Second));
			if(dt.Millisecond>0){
				sb.Append(String.Format(CultureInfo.InvariantCulture,
				                        ".{0:d3}",dt.Millisecond));
			}
			sb.Append("Z");
			return sb.ToString();
		}
		
		[Test]
		public void TestDateTime(){
			DateTime[] ranges=new DateTime[]{
				new DateTime(1,1,1,0,0,0,DateTimeKind.Utc),
				new DateTime(100,1,1,0,0,0,DateTimeKind.Utc),
				new DateTime(1998,1,1,0,0,0,DateTimeKind.Utc),
				new DateTime(2030,1,1,0,0,0,DateTimeKind.Utc),
				new DateTime(9998,1,1,0,0,0,DateTimeKind.Utc),
				new DateTime(9999,12,31,23,59,59,DateTimeKind.Utc)
			};
			for(int i=0;i<ranges.Length;i+=2){
				DateTime j=ranges[i];
				while(true){
					AssertSer(
						CBORObject.FromObject(j),
						"0(\""+DateTimeToString(j)+"\")");
					if(j>=ranges[i+1])break;
					try {
						j=j.AddHours(10);
					} catch(ArgumentOutOfRangeException){
						// Can't add more hours, so break
						break;
					}
				}
			}
		}

		[Test]
		public void TestArray(){
			var cbor=CBORObject.FromJSONString("[]");
			cbor.Add(CBORObject.FromObject(3));
			cbor.Add(CBORObject.FromObject(4));
			var bytes=cbor.ToBytes();
			Assert.AreEqual(
				new byte[]{(byte)(0x80|2),3,4},bytes);
		}
		[Test]
		public void TestMap(){
			var cbor=CBORObject.FromJSONString("{\"a\":2,\"b\":4}");
			Assert.AreEqual(2,cbor.Count);
			AssertEqualsHashCode(
				CBORObject.FromObject(2),
				cbor[CBORObject.FromObject("a")]);
			AssertEqualsHashCode(
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
			var cbor=CBORObject.FromBytes(
				new byte[]{0x7F,0x61,0x20,0x61,0x20,0xFF});
			Assert.AreEqual("  ",cbor.AsString());
			// Test streaming of long strings
			var longString=Repeat('x',200000);
			CBORObject cbor2;
			cbor=CBORObject.FromObject(longString);
			cbor2=CBORObject.FromBytes(cbor.ToBytes());
			AssertEqualsHashCode(cbor,cbor2);
			Assert.AreEqual(longString,cbor2.AsString());
			longString=Repeat('\u00e0',200000);
			cbor=CBORObject.FromObject(longString);
			cbor2=CBORObject.FromBytes(cbor.ToBytes());
			AssertEqualsHashCode(cbor,cbor2);
			Assert.AreEqual(longString,cbor2.AsString());
			longString=Repeat('\u3000',200000);
			cbor=CBORObject.FromObject(longString);
			cbor2=CBORObject.FromBytes(cbor.ToBytes());
			AssertEqualsHashCode(cbor,cbor2);
			Assert.AreEqual(longString,cbor2.AsString());
			longString=Repeat("\ud800\udc00",200000);
			cbor=CBORObject.FromObject(longString);
			cbor2=CBORObject.FromBytes(cbor.ToBytes());
			AssertEqualsHashCode(cbor,cbor2);
			Assert.AreEqual(longString,cbor2.AsString());
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void TestTextStringStreamNoTagsBeforeDefinite(){
			CBORObject.FromBytes(
				new byte[]{0x7F,0x61,0x20,0xC0,0x61,0x20,0xFF});
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void TestTextStringStreamNoIndefiniteWithinDefinite(){
			CBORObject.FromBytes(
				new byte[]{0x7F,0x61,0x20,0x7F,0x61,0x20,0xFF,0xFF});
		}
		[Test]
		public void TestByteStringStream(){
			var cbor=CBORObject.FromBytes(
				new byte[]{0x5F,0x41,0x20,0x41,0x20,0xFF});
		}
		[Test]
		[ExpectedException(typeof(FormatException))]
		public void TestByteStringStreamNoTagsBeforeDefinite(){
			CBORObject.FromBytes(
				new byte[]{0x5F,0x41,0x20,0xC2,0x41,0x20,0xFF});
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void TestByteStringStreamNoIndefiniteWithinDefinite(){
			CBORObject.FromBytes(
				new byte[]{0x5F,0x41,0x20,0x5F,0x41,0x20,0xFF,0xFF});
		}
		
		[Test]
		public void TestDecimalFrac(){
			CBORObject.FromBytes(
				new byte[]{0xc4,0x82,0x3,0x1a,1,2,3,4});
		}
		[Test]
		[ExpectedException(typeof(FormatException))]
		public void TestDecimalFracExponentMustNotBeBignum(){
			CBORObject.FromBytes(
				new byte[]{0xc4,0x82,0xc2,0x41,1,0x1a,1,2,3,4});
		}
		
		[Test]
		public void TestBigTag(){
			CBORObject.FromObjectAndTag(CBORObject.Null,(BigInteger)UInt64.MaxValue);
		}
		
		[Test]
		[ExpectedException(typeof(FormatException))]
		public void TestDecimalFracExactlyTwoElements(){
			CBORObject.FromBytes(
				new byte[]{0xc4,0x82,0xc2,0x41,1});
		}
		[Test]
		public void TestDecimalFracMantissaMayBeBignum(){
			CBORObject.FromBytes(
				new byte[]{0xc4,0x82,0x3,0xc2,0x41,1});
		}
		
		[Test]
		public void TestShort(){
			for(int i=Int16.MinValue;i<=Int16.MaxValue;i++){
				AssertSer(
					CBORObject.FromObject((Int16)i),
					String.Format(Inv,"{0}",i));
			}
		}
		[Test]
		public void TestBigInteger(){
			BigInteger bi=3;
			for(int i=0;i<500;i++){
				AssertSer(
					CBORObject.FromObject(bi),
					String.Format(Inv,"{0}",bi));
				bi*=-7;
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
				BigInteger j=ranges[i];
				while(true){
					AssertSer(
						CBORObject.FromObject(j),
						String.Format(Inv,"{0}",j));
					if(j==ranges[i+1])break;
					j++;
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
					AssertSer(
						CBORObject.FromObject(j),
						String.Format(Inv,"{0}",j));
					Assert.AreEqual(
						CBORObject.FromObject(j),
						CBORObject.FromObject((BigInteger)j));
					if(j==ranges[i+1])break;
					j++;
				}
			}
		}

		[Test]
		public void TestFloat(){
			AssertSer(CBORObject.FromObject(Single.PositiveInfinity),
			          "Infinity");
			AssertSer(CBORObject.FromObject(Single.NegativeInfinity),
			          "-Infinity");
			AssertSer(CBORObject.FromObject(Single.NaN),
			          "NaN");
			for(int i=-65539;i<=65539;i++){
				AssertSer(
					CBORObject.FromObject((float)i),
					String.Format(Inv,"{0}",i));
			}
		}
		
		[Test]
		public void TestSimpleValues(){
			AssertSer(CBORObject.FromObject(true),
			          "true");
			AssertSer(CBORObject.FromObject(false),
			          "false");
			AssertSer(CBORObject.FromObject((Object)null),
			          "null");
		}
		
		[Test]
		public void TestDouble(){
			AssertSer(CBORObject.FromObject(Double.PositiveInfinity),
			          "Infinity");
			AssertSer(CBORObject.FromObject(Double.NegativeInfinity),
			          "-Infinity");
			AssertSer(CBORObject.FromObject(Double.NaN),
			          "NaN");
			for(int i=-65539;i<=65539;i++){
				AssertSer(
					CBORObject.FromObject((double)i),
					String.Format(Inv,"{0}",i));
			}
		}

		[Test]
		public void TestULong(){
			ulong[] ranges=new ulong[]{
				0,65539,
				0xFFFFF000UL,0x100000400UL,
				0x7FFFFFFFFFFFF000UL,0x8000000000000400UL,
				UInt64.MaxValue-1000,UInt64.MaxValue
			};
			for(int i=0;i<ranges.Length;i+=2){
				ulong j=ranges[i];
				while(true){
					AssertSer(
						CBORObject.FromObject(j),
						String.Format(Inv,"{0}",j));
					Assert.AreEqual(
						CBORObject.FromObject(j),
						CBORObject.FromObject((BigInteger)j));
					
					if(j==ranges[i+1])break;
					j++;
				}
			}
		}
		
		[Test]
		public void TestUShort(){
			for(int i=UInt16.MinValue;i<=UInt16.MaxValue;i++){
				AssertSer(
					CBORObject.FromObject((UInt16)i),
					String.Format(Inv,"{0}",i));
			}
		}
		
		public static void Main(){
			var test=new CBORTest();
			foreach(var method in test.GetType().GetMethods()){
				if(method.Name.StartsWith("Test")){
					Console.WriteLine(method.Name);
					Type exctype=null;
					foreach(var a in method.GetCustomAttributes(false)){
						if(a is ExpectedExceptionAttribute){
							exctype=((ExpectedExceptionAttribute)a).ExpectedException;
							break;
						}
					}
					if(exctype==null){
						method.Invoke(test,new object[]{});
					} else {
						try {
							method.Invoke(test,new object[]{});
						} catch(System.Reflection.TargetInvocationException e){
							if(!e.InnerException.GetType().Equals(exctype))
								throw;
						}
					}
				}
			}
		}
	}
}