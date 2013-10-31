package com.upokecenter.util;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 10/30/2013
 * Time: 4:21 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */




	/// <summary>
	/// Description of CBORException.
	/// </summary>
	public class CBORException extends RuntimeException
	{
		public CBORException()
		{
		}

	 	public CBORException(String message){
 super(message);
		}

		public CBORException(String message, Throwable innerException){
 super(message, innerException);
		}

		// This constructor is needed for serialization.
		private static final long serialVersionUID=1;
	}
