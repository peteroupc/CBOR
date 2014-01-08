/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;

namespace PeterO
{
  /// <summary> Description of DecimalUtility. </summary>
  internal static class DecimalUtility
  {
    private static BigInteger[] BigIntPowersOfTen = new BigInteger[]{
      (BigInteger)1, (BigInteger)10, (BigInteger)100, (BigInteger)1000, (BigInteger)10000, (BigInteger)100000, (BigInteger)1000000, (BigInteger)10000000, (BigInteger)100000000, (BigInteger)1000000000,
      (BigInteger)10000000000L, (BigInteger)100000000000L, (BigInteger)1000000000000L, (BigInteger)10000000000000L,
      (BigInteger)100000000000000L, (BigInteger)1000000000000000L, (BigInteger)10000000000000000L,
      (BigInteger)100000000000000000L, (BigInteger)1000000000000000000L
    };

    private static BigInteger[] BigIntPowersOfFive = new BigInteger[]{
      (BigInteger)1, (BigInteger)5, (BigInteger)25, (BigInteger)125, (BigInteger)625, (BigInteger)3125, (BigInteger)15625, (BigInteger)78125, (BigInteger)390625,
      (BigInteger)1953125, (BigInteger)9765625, (BigInteger)48828125, (BigInteger)244140625, (BigInteger)1220703125,
      (BigInteger)6103515625L, (BigInteger)30517578125L, (BigInteger)152587890625L, (BigInteger)762939453125L,
      (BigInteger)3814697265625L, (BigInteger)19073486328125L, (BigInteger)95367431640625L,
      (BigInteger)476837158203125L, (BigInteger)2384185791015625L, (BigInteger)11920928955078125L,
      (BigInteger)59604644775390625L, (BigInteger)298023223876953125L, (BigInteger)1490116119384765625L,
      (BigInteger)7450580596923828125L
    };

    internal static int ShiftLeftOne(int[] arr){
      unchecked {
        int carry=0;
        for(int i=0;i<arr.Length;i++){
          int item=arr[i];
          arr[i]=(int)(arr[i]<<1)|(int)carry;
          carry=((item>>31)!=0) ? 1 : 0;
        }
        return carry;
      }
    }

    internal static int CountTrailingZeros(int numberValue) {
      if (numberValue == 0)
        return 32;
      int i=0;
      unchecked {
        if ((numberValue << 16) == 0) {
          numberValue >>= 16;
          i += 16;
        }

        if ((numberValue << 24) == 0) {
          numberValue >>= 8;
          i += 8;
        }

        if ((numberValue << 28) == 0) {
          numberValue >>= 4;
          i += 4;
        }

        if ((numberValue << 30) == 0) {
          numberValue >>= 2;
          i += 2;
        }

        if ((numberValue << 31) == 0)
          ++i;
      }
      return i;
    }

    internal static int BitPrecisionInt(int numberValue) {
      if (numberValue == 0)
        return 0;
      int i=32;
      unchecked {
        if ((numberValue >> 16) == 0) {
          numberValue <<= 16;
          i -= 8;
        }

        if ((numberValue >> 24) == 0) {
          numberValue <<= 8;
          i -= 8;
        }

        if ((numberValue >> 28) == 0) {
          numberValue <<= 4;
          i -= 4;
        }

        if ((numberValue >> 30) == 0) {
          numberValue <<= 2;
          i -= 2;
        }

        if ((numberValue >> 31) == 0)
          --i;
      }
      return i;
    }

    internal static int ShiftAwayTrailingZerosTwoElements(int[] arr){
      int a0=arr[0];
      int a1=arr[1];
      int tz=CountTrailingZeros(a0);
      if(tz==0)return 0;
      unchecked {
        if(tz<32){
          int carry=a1<<(32-tz);
          arr[0]=(int)((a0>>tz)&(0x7FFFFFFF>>(tz-1)))|(int)(carry);
          arr[1]=((a1>>tz)&(0x7FFFFFFF>>(tz-1)));
          return tz;
        } else {
          tz=CountTrailingZeros(a1);
          if(tz==32){
            arr[0]=0;
          } else if(tz>0){
            arr[0]=((a1>>tz)&(0x7FFFFFFF>>(tz-1)));
          } else {
            arr[0]=a1;
          }
          arr[1]=0;
          return 32+tz;
        }
      }
    }

    internal static bool HasBitSet(int[] arr, int bit){
      return ((bit>>5)<arr.Length && (arr[bit>>5]&(1<<(bit&31)))!=0);
    }

    private sealed class PowerCache {
      private const int MaxSize=64;
      private BigInteger[] outputs;
      private BigInteger[] inputs;
      public PowerCache(){
        outputs=new BigInteger[MaxSize];
        inputs=new BigInteger[MaxSize];
      }
      int size;
      public BigInteger[] FindCachedPowerOrSmaller(BigInteger bi){
        BigInteger[] ret=null;
        BigInteger minValue=BigInteger.Zero;
        lock(outputs){
          for(int i=0;i<size;i++){
            if(inputs[i].CompareTo(bi)<=0 && inputs[i].CompareTo(minValue)>=0){
              //Console.WriteLine("Have cached power ({0}, {1})",inputs[i],bi);
              ret=new BigInteger[]{inputs[i],outputs[i]};
              minValue=inputs[i];
            }
          }
        }
        return ret;
      }
      /// <summary> </summary>
      /// <param name='bi'>A BigInteger object.</param>
      /// <returns>A BigInteger object.</returns>
      public BigInteger GetCachedPower(BigInteger bi){
        lock(outputs){
          for(int i=0;i<size;i++){
            if(bi.Equals(inputs[i])){
              if(i!=0){
                BigInteger tmp;
                // Move to head of cache if it isn't already
                tmp=inputs[i];inputs[i]=inputs[0];inputs[0]=tmp;
                tmp=outputs[i];outputs[i]=outputs[0];outputs[0]=tmp;
                // Move formerly newest to next newest
                if(i!=1){
                  tmp=inputs[i];inputs[i]=inputs[1];inputs[1]=tmp;
                  tmp=outputs[i];outputs[i]=outputs[1];outputs[1]=tmp;
                }
              }
              return outputs[0];
            }
          }
        }
        return null;
      }
      /// <summary> </summary>
      /// <param name='input'>A BigInteger object.</param>
      /// <param name='output'>A BigInteger object.</param>
      /// <returns></returns>
      public void AddPower(BigInteger input, BigInteger output){
        lock(outputs){
          if(size<MaxSize){
            // Shift newer entries down
            for(int i=size;i>0;i--){
              inputs[i]=inputs[i-1];
              outputs[i]=outputs[i-1];
            }
            inputs[0]=input;
            outputs[0]=output;
            size++;
          } else {
            // Shift newer entries down
            for(int i=MaxSize-1;i>0;i--){
              inputs[i]=inputs[i-1];
              outputs[i]=outputs[i-1];
            }
            inputs[0]=input;
            outputs[0]=output;
          }
        }
      }
    }

    private static PowerCache powerOfFiveCache=new DecimalUtility.PowerCache();
    private static PowerCache powerOfTenCache=new DecimalUtility.PowerCache();

    internal static BigInteger FindPowerOfFiveFromBig(BigInteger diff) {
      int sign=diff.Sign;
      if(sign<0)return BigInteger.Zero;
      if (sign == 0) return BigInteger.One;
      FastInteger intcurexp = FastInteger.FromBig(diff);
      if (intcurexp.CompareToInt(54) <= 0) {
        return FindPowerOfFive(intcurexp.AsInt32());
      }
      BigInteger mantissa = BigInteger.One;
      BigInteger bigpow;
      BigInteger origdiff=diff;
      bigpow=powerOfFiveCache.GetCachedPower(origdiff);
      if(bigpow!=null)return bigpow;
      BigInteger[] otherPower=powerOfFiveCache.FindCachedPowerOrSmaller(origdiff);
      if(otherPower!=null){
        intcurexp.SubtractBig(otherPower[0]);
        bigpow=otherPower[1];
        mantissa=bigpow;
      } else {
        bigpow = BigInteger.Zero;
      }
      while (intcurexp.Sign > 0) {
        if (intcurexp.CompareToInt(27) <= 0) {
          bigpow = FindPowerOfFive(intcurexp.AsInt32());
          mantissa *= (BigInteger)bigpow;
          break;
        } else if (intcurexp.CompareToInt(9999999) <= 0) {
          bigpow = BigInteger.Pow(FindPowerOfFive(1), intcurexp.AsInt32());
          mantissa *= (BigInteger)bigpow;
          break;
        } else {
          if (bigpow.IsZero)
            bigpow = BigInteger.Pow(FindPowerOfFive(1), 9999999);
          mantissa *= bigpow;
          intcurexp.AddInt(-9999999);
        }
      }
      powerOfFiveCache.AddPower(origdiff,mantissa);
      return mantissa;
    }

    private static BigInteger BigInt36 = (BigInteger)36;

    internal static BigInteger FindPowerOfTenFromBig(BigInteger bigintExponent) {
      int sign=bigintExponent.Sign;
      if(sign<0)return BigInteger.Zero;
      if (sign == 0) return BigInteger.One;
      if (bigintExponent.CompareTo(BigInt36) <= 0) {
        return FindPowerOfTen((int)bigintExponent);
      }
      FastInteger intcurexp = FastInteger.FromBig(bigintExponent);
      BigInteger mantissa = BigInteger.One;
      BigInteger bigpow = BigInteger.Zero;
      while (intcurexp.Sign > 0) {
        if (intcurexp.CompareToInt(18) <= 0) {
          bigpow = FindPowerOfTen(intcurexp.AsInt32());
          mantissa *= (BigInteger)bigpow;
          break;
        } else if (intcurexp.CompareToInt(9999999) <= 0) {
          int val = intcurexp.AsInt32();
          bigpow = FindPowerOfFive(val);
          bigpow <<= val;
          mantissa *= (BigInteger)bigpow;
          break;
        } else {
          if (bigpow.IsZero) {
            bigpow = FindPowerOfFive(9999999);
            bigpow <<= 9999999;
          }
          mantissa *= bigpow;
          intcurexp.AddInt(-9999999);
        }
      }
      return mantissa;
    }

    private static BigInteger FivePower40=((BigInteger)95367431640625L)*(BigInteger)(95367431640625L);

    internal static BigInteger FindPowerOfFive(int precision) {
      if(precision<0)return BigInteger.Zero;
      if (precision == 0) return BigInteger.One;
      BigInteger bigpow;
      BigInteger ret;
      if (precision <= 27)
        return BigIntPowersOfFive[(int)precision];
      if(precision==40)
        return FivePower40;
      int startPrecision=precision;
      BigInteger origPrecision=(BigInteger)precision;
      bigpow=powerOfFiveCache.GetCachedPower(origPrecision);
      if(bigpow!=null)return bigpow;
      if (precision <= 54) {
        if((precision&1)==0){
          ret = BigIntPowersOfFive[(int)(precision>>1)];
          ret *= (BigInteger)ret;
          powerOfFiveCache.AddPower(origPrecision,ret);
          return ret;
        } else {
          ret = BigIntPowersOfFive[27];
          bigpow = BigIntPowersOfFive[((int)precision) - 27];
          ret *= (BigInteger)bigpow;
          powerOfFiveCache.AddPower(origPrecision,ret);
          return ret;
        }
      }
      if(precision>40 && precision<=94){
        ret = FivePower40;
        bigpow = FindPowerOfFive(precision-40);
        ret *= (BigInteger)bigpow;
        powerOfFiveCache.AddPower(origPrecision,ret);
        return ret;
      }
      BigInteger[] otherPower;
      bool first = true;
      bigpow=BigInteger.Zero;
      while(true){
        otherPower=powerOfFiveCache.FindCachedPowerOrSmaller((BigInteger)precision);
        if(otherPower!=null){
          BigInteger otherPower0=otherPower[0];
          BigInteger otherPower1=otherPower[1];
          precision-=(int)otherPower0;
          if (first)
            bigpow=otherPower[1];
          else {
            bigpow *= (BigInteger)otherPower1;
          }
          first=false;
        } else {
          break;
        }
      }
      ret = (!first ? bigpow : BigInteger.One );
      while (precision > 0) {
        if (precision <= 27) {
          bigpow = BigIntPowersOfFive[(int)precision];
          if (first)
            ret = bigpow;
          else
            ret *= (BigInteger)bigpow;
          first = false;
          break;
        } else if (precision <= 9999999) {
          //          Console.WriteLine("calcing pow for "+precision);
          bigpow = BigInteger.Pow(BigIntPowersOfFive[1], precision);
          if(precision!=startPrecision){
            BigInteger bigprec=(BigInteger)precision;
            powerOfFiveCache.AddPower(bigprec,bigpow);
          }
          if (first)
            ret = bigpow;
          else
            ret *= (BigInteger)bigpow;
          first = false;
          break;
        } else {
          if (bigpow.IsZero)
            bigpow = FindPowerOfFive(9999999);
          if (first)
            ret = bigpow;
          else
            ret *= (BigInteger)bigpow;
          first = false;
          precision -= 9999999;
        }
      }
      powerOfFiveCache.AddPower(origPrecision,ret);
      return ret;
    }

    internal static BigInteger FindPowerOfTen(int precision) {
      if(precision<0)return BigInteger.Zero;
      if (precision == 0) return BigInteger.One;
      BigInteger bigpow;
      BigInteger ret;
      if (precision <= 18)
        return BigIntPowersOfTen[(int)precision];
      int startPrecision=precision;
      BigInteger origPrecision=(BigInteger)precision;
      bigpow=powerOfTenCache.GetCachedPower(origPrecision);
      if(bigpow!=null)return bigpow;
      if (precision <= 27) {
        int prec = (int)precision;
        ret = BigIntPowersOfFive[prec];
        ret <<= prec;
          powerOfTenCache.AddPower(origPrecision,ret);
        return ret;
      }
      if (precision <= 36) {
        if((precision&1)==0){
          ret = BigIntPowersOfTen[(int)(precision>>1)];
          ret *= (BigInteger)ret;
          powerOfTenCache.AddPower(origPrecision,ret);
          return ret;
        } else {
          ret = BigIntPowersOfTen[18];
          bigpow = BigIntPowersOfTen[((int)precision) - 18];
          ret *= (BigInteger)bigpow;
          powerOfTenCache.AddPower(origPrecision,ret);
          return ret;
        }
      }
      BigInteger[] otherPower;
      bool first = true;
      bigpow=BigInteger.Zero;
      while(true){
        otherPower=powerOfTenCache.FindCachedPowerOrSmaller((BigInteger)precision);
        if(otherPower!=null){
          BigInteger otherPower0=otherPower[0];
          BigInteger otherPower1=otherPower[1];
          precision-=(int)otherPower0;
          if (first)
            bigpow=otherPower[1];
          else {
            bigpow *= (BigInteger)otherPower1;
          }
          first=false;
        } else {
          break;
        }
      }
      ret = (!first ? bigpow : BigInteger.One );
      while (precision > 0) {
        if (precision <= 27) {
          bigpow = BigIntPowersOfTen[(int)precision];
          if (first)
            ret = bigpow;
          else
            ret *= (BigInteger)bigpow;
          first = false;
          break;
        } else if (precision <= 9999999) {
          //          Console.WriteLine("calcing pow for "+precision);
          bigpow = FindPowerOfFive(precision);
          bigpow <<= precision;
          if(precision!=startPrecision){
            BigInteger bigprec=(BigInteger)precision;
            powerOfTenCache.AddPower(bigprec,bigpow);
          }
          if (first)
            ret = bigpow;
          else
            ret *= (BigInteger)bigpow;
          first = false;
          break;
        } else {
          if (bigpow.IsZero)
            bigpow = FindPowerOfTen(9999999);
          if (first)
            ret = bigpow;
          else
            ret *= (BigInteger)bigpow;
          first = false;
          precision -= 9999999;
        }
      }
      powerOfTenCache.AddPower(origPrecision,ret);
      return ret;
    }
  }
}
