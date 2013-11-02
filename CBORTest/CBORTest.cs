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
	class CBORTest
	{		
		private static CultureInfo Inv=System.Globalization.CultureInfo.InvariantCulture;
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
			TestCommon.FromBytesTestAB(new byte[]{0xD1,0xFF});
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
					o=TestCommon.FromBytesTestAB(bytes);
				} catch(CBORException){
				}
				if(o!=null){
					//	if(bytes.Length>1)Console.WriteLine(BytesToString(bytes));
					TestCommon.AssertSer(o,o.ToString());
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
							o=TestCommon.FromBytesTestAB(bytes);
						} catch(CBORException){
							Assert.Fail("Old: {0} New: {1}",
							            BytesToString(oldbytes),BytesToString(bytes));
						}
						//		if(bytes.Length>1)Console.WriteLine(BytesToString(bytes));
						TestCommon.AssertSer(o,o.ToString());
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
			for(int i=0;i<=255;i++){
				TestCommon.AssertSer(
					CBORObject.FromObject((byte)i),
					String.Format(Inv,"{0}",i));
			}
		}
		
		private static string ToBase16(byte[] data){
			StringBuilder str=new StringBuilder();
			string alphabet="0123456789ABCDEF";
			int length = data.Length;
			for (int i = 0; i < length;i++) {
				str.Append(alphabet[(data[i]>>4)&15]);
				str.Append(alphabet[data[i]&15]);
			}
			return str.ToString();
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
			Console.WriteLine(ToBase16(Encoding.UTF8.GetBytes("\u0800")));
			Console.WriteLine(ToBase16(Encoding.UTF8.GetBytes("\ud800\udc00")));
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
			var cbor=TestCommon.FromBytesTestAB(
				new byte[]{0x7F,0x61,0x20,0x61,0x20,0xFF});
			Assert.AreEqual("  ",cbor.AsString());
			// Test streaming of long strings
			var longString=Repeat('x',200000);
			CBORObject cbor2;
			cbor=CBORObject.FromObject(longString);
			cbor2=TestCommon.FromBytesTestAB(cbor.ToBytes());
			TestCommon.AssertEqualsHashCode(cbor,cbor2);
			Assert.AreEqual(longString,cbor2.AsString());
			longString=Repeat('\u00e0',200000);
			cbor=CBORObject.FromObject(longString);
			cbor2=TestCommon.FromBytesTestAB(cbor.ToBytes());
			TestCommon.AssertEqualsHashCode(cbor,cbor2);
			Assert.AreEqual(longString,cbor2.AsString());
			longString=Repeat('\u3000',200000);
			cbor=CBORObject.FromObject(longString);
			cbor2=TestCommon.FromBytesTestAB(cbor.ToBytes());
			TestCommon.AssertEqualsHashCode(cbor,cbor2);
			Assert.AreEqual(longString,cbor2.AsString());
			longString=Repeat("\ud800\udc00",200000);
			cbor=CBORObject.FromObject(longString);
			cbor2=TestCommon.FromBytesTestAB(cbor.ToBytes());
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
			var cbor=TestCommon.FromBytesTestAB(
				new byte[]{0x5F,0x41,0x20,0x41,0x20,0xFF});
		}
		[Test]
		[ExpectedException(typeof(CBORException))]
		public void TestByteStringStreamNoTagsBeforeDefinite(){
			TestCommon.FromBytesTestAB(
				new byte[]{0x5F,0x41,0x20,0xC2,0x41,0x20,0xFF});
		}

		[Test]
		[ExpectedException(typeof(CBORException))]
		public void TestByteStringStreamNoIndefiniteWithinDefinite(){
			TestCommon.FromBytesTestAB(
				new byte[]{0x5F,0x41,0x20,0x5F,0x41,0x20,0xFF,0xFF});
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
					CBORObject.FromObject((Int16)i),
					String.Format(Inv,"{0}",i));
			}
		}
		[Test]
		public void TestBigInteger(){
			BigInteger bi=3;
			for(int i=0;i<500;i++){
				TestCommon.AssertSer(
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
					TestCommon.AssertSer(
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
					TestCommon.AssertSer(
						CBORObject.FromObject(j),
						String.Format(Inv,"{0}",j));
					Assert.AreEqual(
						CBORObject.FromObject(j),
						CBORObject.FromObject((BigInteger)j));
					CBORObject obj=CBORObject.FromJSONString(
						String.Format(Inv,"[{0}]",j));
					TestCommon.AssertSer(obj,
					          String.Format(Inv,"[{0}]",j));
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
					String.Format(Inv,"{0}",i));
			}
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
		public void TestDouble(){
			TestCommon.AssertSer(CBORObject.FromObject(Double.PositiveInfinity),
			          "Infinity");
			TestCommon.AssertSer(CBORObject.FromObject(Double.NegativeInfinity),
			          "-Infinity");
			TestCommon.AssertSer(CBORObject.FromObject(Double.NaN),
			          "NaN");
			for(int i=-65539;i<=65539;i++){
				TestCommon.AssertSer(
					CBORObject.FromObject((double)i),
					String.Format(Inv,"{0}",i));
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
					TestCommon.AssertSer(
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
						TestCommon.AssertSer(
							obj2,
							String.Format(Inv,"{0}({1}(0))",j+1,j));
					}
					if(j==ranges[i+1])break;
					j=j+1;
				}
			}
		}
		
	}
}