package com.upokecenter.util;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 10/29/2013
 * Time: 11:05 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */



	/// <summary>
	/// Represents a type that a CBOR Object can have.
	/// </summary>
	/// 
	public enum CBORType {
		/// <summary>
		/// A number of any kind, including integers,
		/// big integers, floating point numbers,
		/// and decimal fractions.  The floating-point
		/// value Not-a-Number is also included in the
		/// Number type.
		/// </summary>
		Number,
		/// <summary>
		/// The simple values true and false.
		/// </summary>
		Boolean,
		/// <summary>
		/// A "simple value" other than floating point
		/// values, true, and false.
		/// </summary>
		SimpleValue,
		/// <summary>
		/// An array of bytes.
		/// </summary>
		ByteString,
		/// <summary>
		/// A text String.
		/// </summary>
		TextString,
		/// <summary>
		/// An array of CBOR objects.
		/// </summary>
		Array,
		/// <summary>
		/// A map of CBOR objects.
		/// </summary>
		Map
	}
