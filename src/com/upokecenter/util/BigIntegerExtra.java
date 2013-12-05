package com.upokecenter.util;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 12/1/2013
 * Time: 11:34 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */


    /**
     * 
     */
  public final class BigInteger
  {
    
    static BigInteger(){
      LittleEndianSerialize=true;
    }
    
    /**
     * Converts the value of a Int64 object to BigInteger.
     * @param bigValue A 64-bit signed integer.
     * @return A BigInteger object with the same value as the Int64 object.
     */
    public static implicit operator BigInteger(long bigValue) {
      return valueOf(bigValue);
    }
    
    /**
     * Adds a BigInteger object and a BigInteger object.
     * @param bthis A BigInteger object.
     * @param t A BigInteger object.
     * @return The sum of the two objects.
     */
    public static BigInteger operator+(BigInteger bthis, BigInteger t)
    {
      if((bthis)==null)throw new NullPointerException("bthis");
      return bthis.add(t);
    }

    
    /**
     * Subtracts two BigInteger values.
     * @param bthis A BigInteger value.
     * @param t Another BigInteger value.
     * @return The difference of the two objects.
     */
    public static BigInteger operator-(BigInteger bthis, BigInteger t)
    {
      if((bthis)==null)throw new NullPointerException("bthis");
      return bthis.subtract(t);
    }
    
    /**
     * Multiplies a BigInteger object by the value of a BigInteger object.
     * @param a A BigInteger object.
     * @param b A BigInteger object.
     * @return The product of the two objects.
     */
    public static BigInteger operator*(BigInteger a, BigInteger b){
      if((a)==null)throw new NullPointerException("a");
      return a.multiply(b);
    }
    
    /**
     * Divides a BigInteger object by the value of a BigInteger object.
     * @param a A BigInteger object.
     * @param b A BigInteger object.
     * @return The quotient of the two objects.
     */
    public static BigInteger operator/(BigInteger a, BigInteger b){
      if((a)==null)throw new NullPointerException("a");
      return a.divide(b);
    }

    /**
     * Finds the remainder that results when a BigInteger object is divided
     * by the value of a BigInteger object.
     * @param a A BigInteger object.
     * @param b A BigInteger object.
     * @return The remainder of the two objects.
     */
    public static BigInteger operator%(BigInteger a, BigInteger b){
      if((a)==null)throw new NullPointerException("a");
      return a.remainder(b);
    }
    
    /**
     * 
     * @param bthis A BigInteger object.
     * @param n A 32-bit unsigned integer.
     */
    public static BigInteger operator<<(BigInteger bthis, int n)
    {
      if((bthis)==null)throw new NullPointerException("bthis");
      return bthis.shiftLeft(n);
    }


    /**
     * Shifts the bits of a BigInteger instance to the right.
     * @param bthis A BigInteger object.
     * @param n The number of bits to shift to the right. If negative, this
     * treated as shifting left the absolute value of this number of bits.
     */
    public static BigInteger operator>>(BigInteger bthis, int n)
    {
      if((bthis)==null)throw new NullPointerException("bthis");
      return bthis.shiftRight(n);
    }

    /**
     * Negates a BigInteger object.
     * @param bigValue A BigInteger object.
     */
    public static BigInteger operator-(BigInteger bigValue){
      if((bigValue)==null)throw new NullPointerException("bigValue");
      return bigValue.negate();
    }
    
    
    /**
     * Converts the value of a BigInteger object to Int64.
     * @param bigValue A BigInteger object.
     * @return A Int64 object with the same value as the BigInteger object.
     */
    public static explicit operator long(BigInteger bigValue) {
      return bigValue.longValue();
    }

    /**
     * Converts the value of a BigInteger object to Int32.
     * @param bigValue A BigInteger object.
     * @return A Int32 object with the same value as the BigInteger object.
     */
    public static explicit operator int(BigInteger bigValue) {
      return bigValue.intValue();
    }
    
    /**
     * Determines whether a BigInteger instance is less than another BigInteger
     * instance.
     * @param thisValue A BigInteger object.
     * @param otherValue A BigInteger object.
     * @return True if &apos;thisValue&apos; is less than &apos;otherValue&apos;;
     * otherwise, false.
     */
    public static boolean operator<(BigInteger thisValue,BigInteger otherValue){
      if(thisValue==null)return (otherValue!=null);
      return (thisValue.compareTo(otherValue)<0);
    }
    /**
     * Determines whether a BigInteger instance is less than or equal to
     * another BigInteger instance.
     * @param thisValue A BigInteger object.
     * @param otherValue A BigInteger object.
     * @return True if &apos;thisValue&apos; is less than or equal to &apos;otherValue&apos;;
     * otherwise, false.
     */
    public static boolean operator<=(BigInteger thisValue,BigInteger otherValue){
      if(thisValue==null)return true;
      return (thisValue.compareTo(otherValue)<=0);
    }
    /**
     * Determines whether a BigInteger instance is greater than another
     * BigInteger instance.
     * @param thisValue A BigInteger object.
     * @param otherValue A BigInteger object.
     * @return True if &apos;thisValue&apos; is greater than &apos;otherValue&apos;;
     * otherwise, false.
     */
    public static boolean operator>(BigInteger thisValue,BigInteger otherValue){
      if(thisValue==null)return false;
      return (thisValue.compareTo(otherValue)>0);
    }
    
    /**
     * Determines whether a BigInteger value is greater than another BigInteger
     * value.
     * @param thisValue A BigInteger object.
     * @param otherValue A BigInteger object.
     * @return True if &apos;thisValue&apos; is greater than or equal to
     * &apos;otherValue&apos;; otherwise, false.
     */
    public static boolean operator>=(BigInteger thisValue,BigInteger otherValue){
      if(thisValue==null)return (otherValue==null);
      return (thisValue.compareTo(otherValue)>=0);
    }


    /**
     * 
     */
    public boolean isPowerOfTwo() {
        int bits=BitLength();
        int ret=0;
        for(int i=0;i<bits;i++){
          ret+=(GetUnsignedBit(i)) ? 1 : 0;
          if(ret>=2)return false;
        }
        return (ret==1);
      }
    
    /**
     * 
     * @param thisValue A BigInteger object.
     */
    
    public static BigInteger Abs(BigInteger thisValue) {
      return thisValue.abs();
    }
    
    /**
     * Gets the BigInteger object for zero.
     */
    
    public static BigInteger Zero {
      get { return ZERO; }
    }
    
    /**
     * Gets the BigInteger object for one.
     */
    
    public static BigInteger One {
      get { return ONE; }
    }
    
    
    /**
     * 
     * @param index A 32-bit signed integer.
     * @param n A 32-bit signed integer.
     */
    public long GetBits(int index, int n) {
      if(n<0 || n>64)throw new IllegalArgumentException("n");
      long v = 0;
      //Debugif(!(n <= 8*8))Assert.fail("{0} line {1}: n <= sizeof(v)*8","integer.cpp",2939);
      for (int j=0; j<n; j++)
        v |= (long)(testBit((int)(index+j)) ? 1 : 0) << j;
      return v;
    }
    
    /**
     * 
     * @param dividend A BigInteger object.
     * @param divisor A BigInteger object.
     * @param remainder A BigInteger object.
     */
    public static BigInteger DivRem(BigInteger dividend, BigInteger divisor, out BigInteger remainder) {
      BigInteger[] result=dividend.divideAndRemainder(divisor);
      remainder=result[1];
      return result[0];
    }
    
    /**
     * 
     * @param bigintFirst A BigInteger object.
     * @param bigintSecond A BigInteger object.
     */
    public static BigInteger GreatestCommonDivisor(BigInteger bigintFirst, BigInteger bigintSecond) {
      return bigintFirst.gcd(bigintSecond);
    }
    
    /**
     * 
     */
    
    public byte[] ToByteArray() {
      return toByteArray();
    }
    
    /**
     * 
     * @param bigValue A BigInteger object.
     * @param power A BigInteger object.
     */
    
    public static BigInteger Pow(BigInteger bigValue, BigInteger power) {
      return bigValue.pow(power);
    }
    
    private static void OrWords(short[] r, short[] a, short[] b, int n) {
      for (int i=0; i<n; i++)
        r[i] = ((short)(a[i] | b[i]));
    }

    private static void XorWords(short[] r, short[] a, short[] b, int n) {
      for (int i=0; i<n; i++)
        r[i] = ((short)(a[i] ^ b[i]));
    }

    private static void NotWords(short[] r, int n) {
      for (int i=0; i<n; i++)
        r[i] =((short)(~r[i]));
    }

    private static void AndWords(short[] r, short[] a, short[] b, int n) {
      for (int i=0; i<n; i++)
        r[i] = ((short)(a[i] & b[i]));
    }


    /**
     * Returns a BigInteger with every bit flipped.
     * @param a A BigInteger object.
     */
    public static BigInteger Not(BigInteger a) {
      if((a)==null)throw new NullPointerException("a");
      BigInteger xa=new BigInteger(a);
      if(xa.signum()<0){ TwosComplement(xa.reg,xa.intValue().reg.length); }
      xa.negative=!(xa.signum()<0);
      NotWords((xa.reg),xa.intValue().reg.length);
      if(xa.signum()<0){ TwosComplement(xa.reg,xa.intValue().reg.length); }
      xa.wordCount=xa.CalcWordCount();
      if(xa.wordCount==0)xa.negative=false;
      return xa;
    }
    /**
     * Does an AND operation between two BigInteger values.
     * @param a A BigInteger instance.
     * @param b Another BigInteger instance.
     */
    public static BigInteger And(BigInteger a, BigInteger b) {
      if((a)==null)throw new NullPointerException("a");
      if((b)==null)throw new NullPointerException("b");
      if(b.signum()==0 || a.signum()==0)return Zero;
      BigInteger xa=new BigInteger(a);
      BigInteger xb=new BigInteger(b);
      xa.reg=CleanGrow(xa.reg,Math.max(xa.reg.length,xb.reg.length));
      xb.reg=CleanGrow(xb.reg,Math.max(xa.reg.length,xb.reg.length));
      if(xa.signum()<0){ TwosComplement(xa.reg,xa.intValue().reg.length); }
      if(xb.signum()<0){ TwosComplement(xb.reg,xb.intValue().reg.length); }
      xa.negative&=(xb.signum()<0);
      AndWords((xa.reg),(xa.reg),(xb.reg),xa.intValue().reg.length);
      if(xa.signum()<0){ TwosComplement(xa.reg,xa.intValue().reg.length); }
      xa.wordCount=xa.CalcWordCount();
      if(xa.wordCount==0)xa.negative=false;
      return xa;
    }

    /**
     * Does an OR operation between two BigInteger instances.
     * @param a A BigInteger instance.
     * @param b Another BigInteger instance.
     */
    public static BigInteger Or(BigInteger a, BigInteger b) {
      BigInteger xa=new BigInteger(a);
      BigInteger xb=new BigInteger(b);
      xa.reg=CleanGrow(xa.reg,Math.max(xa.reg.length,xb.reg.length));
      xb.reg=CleanGrow(xb.reg,Math.max(xa.reg.length,xb.reg.length));
      if(xa.signum()<0){ TwosComplement(xa.reg,xa.intValue().reg.length); }
      if(xb.signum()<0){ TwosComplement(xb.reg,xb.intValue().reg.length); }
      xa.negative|=(xb.signum()<0);
      OrWords((xa.reg),(xa.reg),(xb.reg),xa.intValue().reg.length);
      if(xa.signum()<0){ TwosComplement(xa.reg,xa.intValue().reg.length); }
      xa.wordCount=xa.CalcWordCount();
      if(xa.wordCount==0)xa.negative=false;
      return xa;
    }
    
    /**
     * 
     * @param a A BigInteger instance.
     * @param b Another BigInteger instance.
     */
    public static BigInteger Xor(BigInteger a, BigInteger b) {
      BigInteger xa=new BigInteger(a);
      BigInteger xb=new BigInteger(b);
      xa.reg=CleanGrow(xa.reg,Math.max(xa.reg.length,xb.reg.length));
      xb.reg=CleanGrow(xb.reg,Math.max(xa.reg.length,xb.reg.length));
      if(xa.signum()<0){ TwosComplement(xa.reg,xa.intValue().reg.length); }
      if(xb.signum()<0){ TwosComplement(xb.reg,xb.intValue().reg.length); }
      xa.negative^=(xb.signum()<0);
      XorWords((xa.reg),(xa.reg),(xb.reg),xa.intValue().reg.length);
      if(xa.signum()<0){ TwosComplement(xa.reg,xa.intValue().reg.length); }
      xa.wordCount=xa.CalcWordCount();
      if(xa.wordCount==0)xa.negative=false;
      return xa;
    }

  }
