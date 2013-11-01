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

using NUnit.Framework;

namespace PeterO
{
	
	[TestFixture]
	class CBORTest
	{
		
		private static CBORObject FromBytesA(byte[] b){
			return CBORObject.FromBytes(b);
		}
		
		private static CBORObject FromBytesB(byte[] b){
			using(MemoryStream ms=new MemoryStream(b)){
				CBORObject o=CBORObject.Read(ms);
				if(ms.Position!=ms.Length)
					throw new CBORException("not at EOF");
				return o;
			}
		}

		private static CBORObject FromBytesTestAB(byte[] b){
			CBORObject oa=FromBytesA(b);
			CBORObject ob=FromBytesB(b);
			Assert.AreEqual(oa,ob);
			return oa;
		}
		
		private static CultureInfo Inv=System.Globalization.CultureInfo.InvariantCulture;
		public static void AssertEqualsHashCode(CBORObject o, CBORObject o2){
			if(o.Equals(o2)){
				if(!o2.Equals(o))
					Assert.Fail("{0} equals {1}, but {1} does not equal {0}",o,o2);
				// Test for the guarantee that equal objects
				// must have equal hash codes
				if(o2.GetHashCode()!=o.GetHashCode()){
					// Don't use Assert.AreEqual directly because it has
					// quite a lot of overhead
					Assert.Fail("{0} and {1} don't have equal hash codes",o,o2);
				}
			} else {
				Assert.IsFalse(o2.Equals(o),
				               "{0} does not equal {1}, but {1} equals {0}",o,o2);
			}
		}
		public static void AssertSer(CBORObject o, String s){
			if(!s.Equals(o.ToString()))
				Assert.AreEqual(s,o.ToString(),"o is not equal to s");
			// Test round-tripping
			CBORObject o2=FromBytesTestAB(o.ToBytes());
			if(!s.Equals(o2.ToString()))
				Assert.AreEqual(s,o2.ToString(),"o2 is not equal to s");
			AssertEqualsHashCode(o,o2);
		}
		
		private static string BytesToString(byte[] bytes){
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			string hex="0123456789ABCDEF";
			sb.Append("new byte[]{");
			for(int j=0;j<bytes.Length;j++){
				if(j>0)sb.Append(',');
				sb.Append("0x");
				sb.Append(hex[(int)((bytes[j]>>4)&15)]);
				sb.Append(hex[(int)(bytes[j]&15)]);
			}
			sb.Append("},");
			return sb.ToString();
		}
		
		[Test]
		[ExpectedException(typeof(CBORException))]
		public void TestTagThenBreak(){
			FromBytesTestAB(new byte[]{0xD1,0xFF});
		}
		
		[Test]
		public void TestRandomCBOR(){
			/*
			Random r=new Random();
			for(int i=0;i<5000;i++){
				byte[] bytes=new byte[r.Next(32)+2];
				for(int j=0;j<bytes.Length;j++){
					bytes[j]=(byte)r.Next(256);
				}
				CBORObject o=null;
				try {
					o=FromBytesTestAB(bytes);
				} catch(CBORException){
				}
				if(o!=null){
					//	if(bytes.Length>1)Console.WriteLine(BytesToString(bytes));
					AssertSer(o,o.ToString());
				} else {
					try {
						using(var ms=new System.IO.MemoryStream(bytes)){
							o=CBORObject.Read(ms);
						}
					} catch(CBORException){
					}
					if(o!=null){
						byte[] oldbytes=bytes;
						bytes=o.ToBytes();
						try {
							o=FromBytesTestAB(bytes);
						} catch(CBORException){
							Assert.Fail("Old: {0} New: {1}",
							            BytesToString(oldbytes),BytesToString(bytes));
						}
						//		if(bytes.Length>1)Console.WriteLine(BytesToString(bytes));
						AssertSer(o,o.ToString());
					}
				}
			}
			*/
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
		
		
		private static string DateTimeToString(DateTime bi){
			DateTime dt=bi.ToUniversalTime();
			int year=dt.Year;
			int month=dt.Month;
			int day=dt.Day;
			int hour=dt.Hour;
			int minute=dt.Minute;
			int second=dt.Second;
			int millisecond=dt.Millisecond;
			char[] charbuf=new char[millisecond>0 ? 24 : 20];
			charbuf[0]=(char)('0'+((year/1000)%10));
			charbuf[1]=(char)('0'+((year/100)%10));
			charbuf[2]=(char)('0'+((year/10)%10));
			charbuf[3]=(char)('0'+((year)%10));
			charbuf[4]='-';
			charbuf[5]=(char)('0'+((month/10)%10));
			charbuf[6]=(char)('0'+((month)%10));
			charbuf[7]='-';
			charbuf[8]=(char)('0'+((day/10)%10));
			charbuf[9]=(char)('0'+((day)%10));
			charbuf[10]='T';
			charbuf[11]=(char)('0'+((hour/10)%10));
			charbuf[12]=(char)('0'+((hour)%10));
			charbuf[13]=':';
			charbuf[14]=(char)('0'+((minute/10)%10));
			charbuf[15]=(char)('0'+((minute)%10));
			charbuf[16]=':';
			charbuf[17]=(char)('0'+((second/10)%10));
			charbuf[18]=(char)('0'+((second)%10));
			if(millisecond>0){
				charbuf[19]='.';
				charbuf[20]=(char)('0'+((millisecond/100)%10));
				charbuf[21]=(char)('0'+((millisecond/10)%10));
				charbuf[22]=(char)('0'+((millisecond)%10));
				charbuf[23]='Z';
			} else {
				charbuf[19]='Z';
			}
			return new String(charbuf);
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
			var cbor=FromBytesTestAB(
				new byte[]{0x7F,0x61,0x20,0x61,0x20,0xFF});
			Assert.AreEqual("  ",cbor.AsString());
			// Test streaming of long strings
			var longString=Repeat('x',200000);
			CBORObject cbor2;
			cbor=CBORObject.FromObject(longString);
			cbor2=FromBytesTestAB(cbor.ToBytes());
			AssertEqualsHashCode(cbor,cbor2);
			Assert.AreEqual(longString,cbor2.AsString());
			longString=Repeat('\u00e0',200000);
			cbor=CBORObject.FromObject(longString);
			cbor2=FromBytesTestAB(cbor.ToBytes());
			AssertEqualsHashCode(cbor,cbor2);
			Assert.AreEqual(longString,cbor2.AsString());
			longString=Repeat('\u3000',200000);
			cbor=CBORObject.FromObject(longString);
			cbor2=FromBytesTestAB(cbor.ToBytes());
			AssertEqualsHashCode(cbor,cbor2);
			Assert.AreEqual(longString,cbor2.AsString());
			longString=Repeat("\ud800\udc00",200000);
			cbor=CBORObject.FromObject(longString);
			cbor2=FromBytesTestAB(cbor.ToBytes());
			AssertEqualsHashCode(cbor,cbor2);
			Assert.AreEqual(longString,cbor2.AsString());
		}

		[Test]
		[ExpectedException(typeof(CBORException))]
		public void TestTextStringStreamNoTagsBeforeDefinite(){
			FromBytesTestAB(
				new byte[]{0x7F,0x61,0x20,0xC0,0x61,0x20,0xFF});
		}

		[Test]
		[ExpectedException(typeof(CBORException))]
		public void TestTextStringStreamNoIndefiniteWithinDefinite(){
			FromBytesTestAB(
				new byte[]{0x7F,0x61,0x20,0x7F,0x61,0x20,0xFF,0xFF});
		}
		[Test]
		public void TestByteStringStream(){
			var cbor=FromBytesTestAB(
				new byte[]{0x5F,0x41,0x20,0x41,0x20,0xFF});
		}
		[Test]
		[ExpectedException(typeof(CBORException))]
		public void TestByteStringStreamNoTagsBeforeDefinite(){
			FromBytesTestAB(
				new byte[]{0x5F,0x41,0x20,0xC2,0x41,0x20,0xFF});
		}

		[Test]
		[ExpectedException(typeof(CBORException))]
		public void TestByteStringStreamNoIndefiniteWithinDefinite(){
			FromBytesTestAB(
				new byte[]{0x5F,0x41,0x20,0x5F,0x41,0x20,0xFF,0xFF});
		}
		
		[Test]
		public void TestDecimalFrac(){
			FromBytesTestAB(
				new byte[]{0xc4,0x82,0x3,0x1a,1,2,3,4});
		}
		[Test]
		[ExpectedException(typeof(CBORException))]
		public void TestDecimalFracExponentMustNotBeBignum(){
			FromBytesTestAB(
				new byte[]{0xc4,0x82,0xc2,0x41,1,0x1a,1,2,3,4});
		}
		
		[Test]
		public void TestBigTag(){
			CBORObject.FromObjectAndTag(CBORObject.Null,(BigInteger)UInt64.MaxValue);
		}
		
		[Test]
		[ExpectedException(typeof(CBORException))]
		public void TestDecimalFracExactlyTwoElements(){
			FromBytesTestAB(
				new byte[]{0xc4,0x82,0xc2,0x41,1});
		}
		[Test]
		public void TestDecimalFracMantissaMayBeBignum(){
			FromBytesTestAB(
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
					if(j.Equals(ranges[i+1]))break;
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
					CBORObject obj=CBORObject.FromJSONString(
						String.Format(Inv,"[{0}]",j));
					AssertSer(obj,
					          String.Format(Inv,"[{0}]",j));
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
		public void TestDecimal(){
			AssertSer(
				CBORObject.FromObject(Decimal.MinValue),
				String.Format(Inv,"{0}",Decimal.MinValue));
			AssertSer(
				CBORObject.FromObject(Decimal.MaxValue),
				String.Format(Inv,"{0}",Decimal.MaxValue));
			for(int i=-100;i<=100;i++){
				AssertSer(
					CBORObject.FromObject((decimal)i),
					String.Format(Inv,"{0}",i));
				AssertSer(
					CBORObject.FromObject((decimal)i+0.1m),
					String.Format(Inv,"{0}",(decimal)i+0.1m));
				AssertSer(
					CBORObject.FromObject((decimal)i+0.1111m),
					String.Format(Inv,"{0}",(decimal)i+0.1111m));
			}
		}

		[Test]
		public void TestTags(){
			BigInteger maxuint=(BigInteger)UInt64.MaxValue;
			BigInteger[] ranges=new BigInteger[]{
				(BigInteger)5,
				(BigInteger)65539,
				(BigInteger)Int32.MaxValue-(BigInteger)500,
				(BigInteger)Int32.MaxValue+(BigInteger)500,
				(BigInteger)Int64.MaxValue-(BigInteger)500,
				(BigInteger)Int64.MaxValue+(BigInteger)500,
				(BigInteger)UInt64.MaxValue-(BigInteger)500,
				maxuint,
			};
			for(int i=0;i<ranges.Length;i+=2){
				BigInteger j=ranges[i];
				while(true){
					CBORObject obj=CBORObject.FromObjectAndTag(0,j);
					Assert.IsTrue(obj.IsTagged,"obj not tagged");
					if(!obj.InnermostTag.Equals(j))
						Assert.AreEqual(j,obj.InnermostTag,"obj tag doesn't match: {0}",obj);
					AssertSer(
						obj,
						String.Format(Inv,"{0}(0)",j));
					if(j!=maxuint){
						// Test multiple tags
						CBORObject obj2=CBORObject.FromObjectAndTag(obj,j+1);
						BigInteger[] bi=obj2.GetTags();
						if(bi.Length!=2)
							Assert.AreEqual(2,bi.Length,"Expected 2 tags: {0}",obj2);
						if(bi[0]!=j+1)
							Assert.AreEqual(j+1,bi[0],"Outer tag doesn't match: {0}",obj2);
						if(bi[1]!=j)
							Assert.AreEqual(j,bi[1],"Inner tag doesn't match: {0}",obj2);
						if(obj2.InnermostTag!=j)
							Assert.AreEqual(j,bi[0],"Innermost tag doesn't match: {0}",obj2);
						AssertSer(
							obj2,
							String.Format(Inv,"{0}({1}(0))",j+1,j));
					}
					if(j==ranges[i+1])break;
					j=j+1;
				}
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
		
	}
}