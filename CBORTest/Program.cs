/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 10/24/2013
 * Time: 3:36 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
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
		
		public static void AssertSer(CBORObject o, String s){
			Assert.AreEqual(o.ToString(),s);
			CBORObject o2=CBORObject.FromBytes(o.ToBytes());
			Assert.AreEqual(o2.ToString(),s);
		}
		
		[Test]
		public static void TestByte(){
			for(int i=0;i<=Byte.MaxValue;i++){
				AssertSer(
					CBORObject.FromObject((byte)i),
					String.Format(Inv,"{0}",i));
			}
		}
		
		[Test]
		public static void TestSByte(){
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
		public static void TestDateTime(){
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
						break;
					}
				}
			}
		}

		[Test]
		public static void TestShort(){
			for(int i=Int16.MinValue;i<=Int16.MaxValue;i++){
				AssertSer(
					CBORObject.FromObject((Int16)i),
					String.Format(Inv,"{0}",i));
			}
		}
		[Test]
		public static void TestBigInteger(){
			BigInteger bi=3;
			for(int i=0;i<500;i++){
				AssertSer(
					CBORObject.FromObject(bi),
					String.Format(Inv,"{0}",bi));
				bi*=-7;
			}
		}
		[Test]
		public static void TestLong(){
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
					if(j==ranges[i+1])break;
					j++;
				}
			}
		}

		[Test]
		public static void TestFloat(){
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
		public static void TestSimpleValues(){
			AssertSer(CBORObject.FromObject(true),
			          "true");
			AssertSer(CBORObject.FromObject(false),
			          "false");
			AssertSer(CBORObject.FromObject((Object)null),
			          "null");
		}
		
		[Test]
		public static void TestDouble(){
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
		public static void TestULong(){
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
					if(j==ranges[i+1])break;
					j++;
				}
			}
		}
		
		[Test]
		public static void TestUShort(){
			for(int i=UInt16.MinValue;i<=UInt16.MaxValue;i++){
				AssertSer(
					CBORObject.FromObject((UInt16)i),
					String.Format(Inv,"{0}",i));
			}
		}
		public static void Main(string[] args)
		{
		}
	}
}