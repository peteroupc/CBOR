/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 11/2/2013
 * Time: 3:22 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Runtime.Serialization;

namespace CBORTest
{
	/// <summary>
	/// Desctiption of BEncodingTest.
	/// </summary>
	public class BEncodingTest : Exception, ISerializable
	{
		public BEncodingTest()
		{
		}

	 	public BEncodingTest(string message) : base(message)
		{
		}

		public BEncodingTest(string message, Exception innerException) : base(message, innerException)
		{
		}

		// This constructor is needed for serialization.
		protected BEncodingTest(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}