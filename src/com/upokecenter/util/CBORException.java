package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */




	/**
	 * Description of CBORException.
	 */
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
