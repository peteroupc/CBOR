/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
// using System.Numerics;
using System.Text;

namespace PeterO
{
	internal interface ICBORNumber
	{
		bool IsPositiveInfinity(Object obj);
		bool IsInfinity(Object obj);
		bool IsNegativeInfinity(Object obj);
		bool IsNaN(Object obj);
		double AsDouble(Object obj);
		ExtendedDecimal AsExtendedDecimal(Object obj);
		ExtendedFloat AsExtendedFloat(Object obj);
		float AsSingle(Object obj);
		BigInteger AsBigInteger(Object obj);
		long AsInt64(Object obj);
		bool CanFitInSingle(Object obj);
		bool CanFitInDouble(Object obj);
		bool CanFitInInt32(Object obj);
		bool CanFitInInt64(Object obj);
		bool CanFitInTypeZeroOrOne(Object obj);
		bool CanTruncatedIntFitInInt64(Object obj);
		bool CanTruncatedIntFitInInt32(Object obj);
		int AsInt32(Object obj, int minValue, int maxValue);
		bool IsZero(Object obj);
		int Sign(Object obj);
		bool IsIntegral(Object obj);
	}
}
