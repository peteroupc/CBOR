/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 11/2/2013
 * Time: 3:11 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Runtime.Serialization;

namespace CBORTest
{
	/// <summary>
	/// Desctiption of BEncoding.
	/// </summary>
	public class BEncoding : Exception, ISerializable
	{
		public BEncoding()
		{
		}

	 	public BEncoding(string message) : base(message)
		{
		}

		public BEncoding(string message, Exception innerException) : base(message, innerException)
		{
		}

		// This constructor is needed for serialization.
		protected BEncoding(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}