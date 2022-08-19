/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
https://creativecommons.org/publicdomain/zero/1.0/

 */
using PeterO.Numbers;
using System;

namespace PeterO.Cbor
{
    internal class CBORInteger : ICBORNumber
    {
        public object Abs(object obj)
        {
            long val = (long)obj;
            return (val == int.MinValue) ? (EInteger.One << 63) : ((val < 0) ?
                -val : obj);
        }

        public EInteger AsEInteger(object obj)
        {
            return (EInteger)(long)obj;
        }

        public double AsDouble(object obj)
        {
            return (long)obj;
        }

        public EDecimal AsEDecimal(object obj)
        {
            return EDecimal.FromInt64((long)obj);
        }

        public EFloat AsEFloat(object obj)
        {
            return EFloat.FromInt64((long)obj);
        }

        public ERational AsERational(object obj)
        {
            return ERational.FromInt64((long)obj);
        }

        public int AsInt32(object obj, int minValue, int maxValue)
        {
            long val = (long)obj;
            if (val >= minValue && val <= maxValue)
            {
                return (int)val;
            }
            throw new OverflowException("This object's value is out of range");
        }

        public long AsInt64(object obj)
        {
            return (long)obj;
        }

        public float AsSingle(object obj)
        {
            return (long)obj;
        }

        public bool CanFitInDouble(object obj)
        {
            long intItem = (long)obj;
            if (intItem == long.MinValue)
            {
                return true;
            }
            intItem = (intItem < 0) ? -intItem : intItem;
            while (intItem >= (1L << 53) && (intItem & 1) == 0)
            {
                intItem >>= 1;
            }
            return intItem < (1L << 53);
        }

        public bool CanFitInInt32(object obj)
        {
            long val = (long)obj;
            return val >= int.MinValue && val <= int.MaxValue;
        }

        public bool CanFitInInt64(object obj)
        {
            return true;
        }

        public bool CanFitInSingle(object obj)
        {
            long intItem = (long)obj;
            if (intItem == long.MinValue)
            {
                return true;
            }
            intItem = (intItem < 0) ? -intItem : intItem;
            while (intItem >= (1L << 24) && (intItem & 1) == 0)
            {
                intItem >>= 1;
            }
            return intItem < (1L << 24);
        }

        public bool CanTruncatedIntFitInInt32(object obj)
        {
            long val = (long)obj;
            return val >= int.MinValue && val <= int.MaxValue;
        }

        public bool CanTruncatedIntFitInUInt64(object obj)
        {
            long val = (long)obj;
            return val >= 0;
        }

        public bool CanFitInUInt64(object obj)
        {
            long val = (long)obj;
            return val >= 0;
        }

        public bool CanTruncatedIntFitInInt64(object obj)
        {
            return true;
        }

        public bool IsInfinity(object obj)
        {
            return false;
        }

        public bool IsIntegral(object obj)
        {
            return true;
        }

        public bool IsNaN(object obj)
        {
            return false;
        }

        public bool IsNegative(object obj)
        {
            return ((long)obj) < 0;
        }

        public bool IsNegativeInfinity(object obj)
        {
            return false;
        }

        public bool IsPositiveInfinity(object obj)
        {
            return false;
        }

        public bool IsNumberZero(object obj)
        {
            return ((long)obj) == 0;
        }

        public object Negate(object obj)
        {
            return (((long)obj) == long.MinValue) ? (EInteger.One << 63) :
      (-(long)obj);
        }

        public int Sign(object obj)
        {
            long val = (long)obj;
            return (val == 0) ? 0 : ((val < 0) ? -1 : 1);
        }
    }
}