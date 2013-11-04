package com.upokecenter.test;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import org.junit.Assert;


import com.upokecenter.util.*;
import java.io.*;


	final class TestCommon {
private TestCommon(){}
		private static CBORObject FromBytesA(byte[] b) {
			return CBORObject.DecodeFromBytes(b);
		}
		
		private static CBORObject FromBytesB(byte[] b) {
			java.io.ByteArrayInputStream ms=null;
try {
ms=new ByteArrayInputStream(b);
int startingAvailable=ms.available();

				CBORObject o=CBORObject.Read(ms);
				if((startingAvailable-ms.available())!=startingAvailable)
					throw new CBORException("not at EOF");
				return o;
}
finally {
try { if(ms!=null)ms.close(); } catch(IOException ex){}
}
		}

		//
		//  Tests the equivalence of the FromBytes and Read methods.
		//		
		public static CBORObject FromBytesTestAB(byte[] b) {
			CBORObject oa=FromBytesA(b);
			CBORObject ob=FromBytesB(b);
			Assert.assertEquals(oa,ob);
			return oa;
		}
		
		public static void AssertEqualsHashCode(CBORObject o, CBORObject o2) {
			if(o.equals(o2)){
				if(!o2.equals(o))
					Assert.fail(
						String.format(java.util.Locale.US,"%s equals %s, but not vice versa",o,o2));
				// Test for the guarantee that equal objects
				// must have equal hash codes
				if(o2.hashCode()!=o.hashCode()){
					// Don't use Assert.assertEquals directly because it has
					// quite a lot of overhead
					Assert.fail(
						String.format(java.util.Locale.US,"%s and %s don't have equal hash codes",o,o2));
				}
			} else {
				if(o2.equals(o))Assert.fail(
				               String.format(java.util.Locale.US,"%s does not equal %s, but not vice versa",o,o2));
			}
		}
		public static void AssertSer(CBORObject o, String s) {
			if(!s.equals(o.toString()))
				Assert.assertEquals("o is not equal to s",s,o.toString());
			// Test round-tripping
			CBORObject o2=FromBytesTestAB(o.EncodeToBytes());
			if(!s.equals(o2.toString()))
				Assert.assertEquals("o2 is not equal to s",s,o2.toString());
			AssertEqualsHashCode(o,o2);
		}

	}
