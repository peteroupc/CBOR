/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 10/29/2013
 * Time: 2:23 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace PeterO
{
	[TestFixture]
	public class StringPerformance
	{
		CBORObject cbor;
		
		[TestFixtureSetUp]
		public void SetUp(){
			string json=System.IO.File.ReadAllText("C:\\Users\\Peter\\Documents\\SharpDevelop Projects\\CBOR\\bin\\jsonstrings.json",System.Text.Encoding.UTF8);
			cbor=CBORObject.FromJSONString(json);
		}

		[Test]
		public void TestStreamingI(){
			using(var ms=new MemoryStream()){
				for(var i=0;i<cbor.Count;i++){
					CBORObject.WriteStreamedString(cbor[i].AsString(),ms);
				}
			}
		}
		[Test]
		public void TestStreamingII(){
			using(var ms=new MemoryStream()){
				for(var i=0;i<cbor.Count;i++){
				//	CBORObject.WriteStreamedStringII(cbor[i].AsString(),ms);
				}
			}
		}
	}
}
