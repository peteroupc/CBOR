import static org.junit.Assert.*;

import java.math.BigInteger;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;

public class TestCBORObject {

	public void AssertEqualsHashCode(CBORObject o, CBORObject o2){
		if(o.equals(o2)){
			Assert.assertTrue(o2.equals(o));
			// Test for the guarantee that equal objects
			// must have equal hash codes
			Assert.assertEquals(o2.hashCode(),o.hashCode());
		} else {
			Assert.assertTrue(!o2.equals(o));				
		}
	}
	public void AssertSer(CBORObject o, String s) {
		Assert.assertEquals(s,o.toString());
		CBORObject o2=CBORObject.FromBytes(o.ToBytes());
		Assert.assertEquals(s,o2.toString());
		AssertEqualsHashCode(o,o2);
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
		Assert.assertEquals(1.5,o.get(0).AsDouble(),0.01);
		Assert.assertEquals("[4([-1, 15]), 4([-1, 26]), 4([-1, 37]), 4, 4([-2, 22222])]",
				o.toString());
	}

	@Test
	public void TestByte() {
		for(int i=0;i<=Byte.MAX_VALUE;i++){
			AssertSer(
					CBORObject.FromObject((byte)i),
					String.format("%s",i));
		}
	}

	@Test
	public void TestArray() {
		CBORObject cbor=CBORObject.FromJSONString("[]");
		cbor.Add(CBORObject.FromObject(3));
		cbor.Add(CBORObject.FromObject(4));
		byte[] bytes=cbor.ToBytes();
		Assert.assertArrayEquals(
				new byte[]{(byte)(0x80|2),3,4},bytes);
	}
	@Test
	public void TestMap(){
		CBORObject cbor=CBORObject.FromJSONString("{\"a\":2,\"b\":4}");
		Assert.assertEquals(2,cbor.size());
		AssertEqualsHashCode(
				CBORObject.FromObject(2),
				cbor.get(CBORObject.FromObject("a")));
		AssertEqualsHashCode(
				CBORObject.FromObject(4),
				cbor.get(CBORObject.FromObject("b")));
		Assert.assertEquals(2,cbor.get(CBORObject.FromObject("a")).AsInt32());
		Assert.assertEquals(4,cbor.get(CBORObject.FromObject("b")).AsInt32());
	}

	private String repeat(char c, int num){
		StringBuilder sb=new StringBuilder();
		for(int i=0;i<num;i++){
			sb.append(c);
		}
		return sb.toString();
	}

	@SuppressWarnings("unused")
	private String repeat(String c, int num){
		StringBuilder sb=new StringBuilder();
		for(int i=0;i<num;i++){
			sb.append(c);
		}
		return sb.toString();
	}
	@Test
	public void TestTextStringStream() {
		CBORObject cbor=CBORObject.FromBytes(
				new byte[]{0x7F,0x61,0x20,0x61,0x20,(byte)0xFF});
		Assert.assertEquals("  ",cbor.AsString());
		// Test streaming of long strings
		String longString=repeat('x',200000);
		CBORObject cbor2;
		cbor=CBORObject.FromObject(longString);
		cbor2=CBORObject.FromBytes(cbor.ToBytes());
		AssertEqualsHashCode(cbor,cbor2);
		Assert.assertEquals(longString,cbor2.AsString());
		longString=repeat('\u00e0',200000);
		cbor=CBORObject.FromObject(longString);
		cbor2=CBORObject.FromBytes(cbor.ToBytes());
		AssertEqualsHashCode(cbor,cbor2);
		Assert.assertEquals(longString,cbor2.AsString());
		longString=repeat('\u3000',200000);
		cbor=CBORObject.FromObject(longString);
		cbor2=CBORObject.FromBytes(cbor.ToBytes());
		AssertEqualsHashCode(cbor,cbor2);
		Assert.assertEquals(longString,cbor2.AsString());
		StringBuilder b=new StringBuilder();
		for(int i=0;i<200000;i++){
			b.append("\ud800\udc00");
		}
		longString=b.toString();
		cbor=CBORObject.FromObject(longString);
		cbor2=CBORObject.FromBytes(cbor.ToBytes());
		AssertEqualsHashCode(cbor,cbor2);
		Assert.assertEquals(longString,cbor2.AsString());
	}

	@Test
	public void TestTextStringStreamNoTagsBeforeDefinite() {
		try {
			CBORObject.FromBytes(
					new byte[]{0x7F,0x61,0x20,(byte)0xC0,0x61,0x20,(byte)0xFF});
		} catch(CBORException ex){return;}
		fail("Expected exception");		}

	@Test
	public void TestTextStringStreamNoIndefiniteWithinDefinite() {
		try {
			CBORObject.FromBytes(
					new byte[]{0x7F,0x61,0x20,0x7F,0x61,0x20,(byte)0xFF,(byte)0xFF});
		} catch(CBORException ex){return;}
		fail("Expected exception");
	}
	@Test
	public void TestByteStringStream() {
		CBORObject.FromBytes(
				new byte[]{0x5F,0x41,0x20,0x41,0x20,(byte)0xFF});
	}
	@Test
	public void TestByteStringStreamNoTagsBeforeDefinite() {
		try {
			CBORObject.FromBytes(
					new byte[]{0x5F,0x41,0x20,(byte)0xc2,0x41,0x20,(byte)0xFF});
		} catch(CBORException ex){return;}
		fail("Expected exception");
	}

	@Test
	public void TestByteStringStreamNoIndefiniteWithinDefinite() {
		try {
			CBORObject.FromBytes(
					new byte[]{0x5F,0x41,0x20,0x5F,0x41,0x20,(byte)0xFF,(byte)0xFF});
		} catch(CBORException ex){return;}
		fail("Expected exception");
	}

	@Test
	public void TestDecimalFrac() {
		CBORObject.FromBytes(
				new byte[]{(byte)0xc4,(byte)0x82,0x3,0x1a,1,2,3,4});
	}
	@Test
	public void TestDecimalFracExponentMustNotBeBignum() {
		try {
			CBORObject.FromBytes(
					new byte[]{(byte) (byte)0xc4,(byte) 
							(byte)0x82,(byte) (byte)0xc2,0x41,1,0x1a,1,2,3,4});
		} catch(CBORException ex){return;}
		fail("Expected exception");
	}

	@Test
	public void TestBigTag(){
		CBORObject.FromObjectAndTag(CBORObject.Null,
				BigInteger.valueOf(Long.MAX_VALUE).multiply(BigInteger.valueOf(2)));
	}

	@Test
	public void TestDecimalFracExactlyTwoElements() {
		try {
			CBORObject.FromBytes(
					new byte[]{(byte)(byte)0xc4,(byte)(byte)0x82,(byte)(byte)0xc2,0x41,1});
		} catch(CBORException ex){return;}
		fail("Expected exception");
	}
	@Test
	public void TestDecimalFracMantissaMayBeBignum() {
		CBORObject.FromBytes(
				new byte[]{(byte)(byte)0xc4,(byte)(byte)0x82,0x3,(byte)(byte)0xc2,0x41,1});
	}

	@Test
	public void TestShort() {
		for(int i=Short.MIN_VALUE;i<=Short.MAX_VALUE;i++){
			AssertSer(
					CBORObject.FromObject((short)i),
					String.format("%s",i));
		}
	}
	@Test
	public void TestBigInteger() {
		BigInteger bi=BigInteger.valueOf(3);
		for(int i=0;i<500;i++){
			AssertSer(
					CBORObject.FromObject(bi),
					String.format("%s",bi));
			bi=bi.multiply(BigInteger.valueOf(-7));
		}
		BigInteger[] ranges=new BigInteger[]{
				BigInteger.valueOf(Long.MIN_VALUE).subtract(BigInteger.valueOf(513)),
				BigInteger.valueOf(Long.MIN_VALUE).add(BigInteger.valueOf(513)),
				BigInteger.valueOf(0).subtract(BigInteger.valueOf(513)),
				BigInteger.valueOf(0).add(BigInteger.valueOf(513)),
				BigInteger.valueOf(Long.MAX_VALUE).subtract(BigInteger.valueOf(513)),
				BigInteger.valueOf(Long.MAX_VALUE).add(BigInteger.valueOf(513)),
				BigInteger.valueOf(Long.MAX_VALUE).multiply(BigInteger.valueOf(2)).subtract(BigInteger.valueOf(513)),
				BigInteger.valueOf(Long.MAX_VALUE).multiply(BigInteger.valueOf(2)).add(BigInteger.valueOf(513)),
		};
		for(int i=0;i<ranges.length;i+=2){
			BigInteger j=ranges[i];
			while(true){
				AssertSer(
						CBORObject.FromObject(j),
						String.format("%s",j));
				if(j.equals(ranges[i+1]))break;
				j=j.add(BigInteger.ONE);
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
				AssertSer(
						CBORObject.FromObject(j),
						String.format("%s",j));
				Assert.assertEquals(
						CBORObject.FromObject(j),
						CBORObject.FromObject(BigInteger.valueOf(j)));
				CBORObject obj=CBORObject.FromJSONString(
						String.format("[%s]",j));
				AssertSer(obj,
						String.format("[%s]",j));
				if(j==ranges[i+1])break;
				j++;
			}
		}
	}

	@Test
	public void TestFloat() {
		AssertSer(CBORObject.FromObject(Float.POSITIVE_INFINITY),
				"Infinity");
		AssertSer(CBORObject.FromObject(Float.NEGATIVE_INFINITY),
				"-Infinity");
		AssertSer(CBORObject.FromObject(Float.NaN),
				"NaN");
		for(int i=-65539;i<=65539;i++){
			AssertSer(
					CBORObject.FromObject((float)i),
					String.format("%s.0",i));
		}
	}

	@Test
	public void TestSimpleValues() {
		AssertSer(CBORObject.FromObject(true),
				"true");
		AssertSer(CBORObject.FromObject(false),
				"false");
		AssertSer(CBORObject.FromObject((Object)null),
				"null");
	}

	@Test
	public void TestDouble() {
		AssertSer(CBORObject.FromObject(Double.POSITIVE_INFINITY),
				"Infinity");
		AssertSer(CBORObject.FromObject(Double.NEGATIVE_INFINITY),
				"-Infinity");
		AssertSer(CBORObject.FromObject(Double.NaN),
				"NaN");
		for(int i=-65539;i<=65539;i++){
			AssertSer(
					CBORObject.FromObject((double)i),
					String.format("%s.0",i));
		}
	}


}
