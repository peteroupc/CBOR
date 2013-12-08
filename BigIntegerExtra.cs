/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 12/1/2013
 * Time: 11:34 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
namespace PeterO
{
    /// <summary> </summary>
  public sealed partial class BigInteger
  {
    
    /// <summary>Converts the value of a Int64 object to BigInteger.</summary>
    /// <returns>A BigInteger object with the same value as the Int64 object.</returns>
    /// <param name='bigValue'>A 64-bit signed integer.</param>
    public static implicit operator BigInteger(long bigValue){
      return valueOf(bigValue);
    }
    
    /// <summary>Adds a BigInteger object and a BigInteger object.</summary>
    /// <param name='bthis'>A BigInteger object.</param>
    /// <param name='t'>A BigInteger object.</param>
    /// <returns>The sum of the two objects.</returns>
    public static BigInteger operator+(BigInteger bthis, BigInteger t)
    {
      if((bthis)==null)throw new ArgumentNullException("bthis");
      return bthis.add(t);
    }

    
    /// <summary> Subtracts two BigInteger values. </summary>
    /// <param name='bthis'>A BigInteger value.</param>
    /// <param name='t'>Another BigInteger value.</param>
    /// <returns>The difference of the two objects.</returns>
    public static BigInteger operator-(BigInteger bthis, BigInteger t)
    {
      if((bthis)==null)throw new ArgumentNullException("bthis");
      return bthis.subtract(t);
    }
    
    /// <summary>Multiplies a BigInteger object by the value of a BigInteger
    /// object.</summary>
    /// <param name='a'>A BigInteger object.</param>
    /// <param name='b'>A BigInteger object.</param>
    /// <returns>The product of the two objects.</returns>
    public static BigInteger operator*(BigInteger a, BigInteger b){
      if((a)==null)throw new ArgumentNullException("a");
      return a.multiply(b);
    }
    
    /// <summary>Divides a BigInteger object by the value of a BigInteger
    /// object.</summary>
    /// <param name='a'>A BigInteger object.</param>
    /// <param name='b'>A BigInteger object.</param>
    /// <returns>The quotient of the two objects.</returns>
    public static BigInteger operator/(BigInteger a, BigInteger b){
      if((a)==null)throw new ArgumentNullException("a");
      return a.divide(b);
    }

    /// <summary>Finds the remainder that results when a BigInteger object
    /// is divided by the value of a BigInteger object.</summary>
    /// <param name='a'>A BigInteger object.</param>
    /// <param name='b'>A BigInteger object.</param>
    /// <returns>The remainder of the two objects.</returns>
    public static BigInteger operator%(BigInteger a, BigInteger b){
      if((a)==null)throw new ArgumentNullException("a");
      return a.remainder(b);
    }
    
    /// <summary> </summary>
    /// <param name='bthis'>A BigInteger object.</param>
    /// <param name='n'>A 32-bit unsigned integer.</param>
    /// <returns></returns>
    /// <remarks/>
    public static BigInteger operator<<(BigInteger bthis, int n)
    {
      if((bthis)==null)throw new ArgumentNullException("bthis");
      return bthis.shiftLeft(n);
    }

    
    /// <summary> Calculates the remainder when a BigInteger raised to a
    /// certain power is divided by another BigInteger. </summary>
    /// <param name='bigintValue'>A BigInteger object.</param>
    /// <param name='pow'>A BigInteger object.</param>
    /// <param name='mod'>A BigInteger object.</param>
    /// <returns>(bigintValue^pow)%mod</returns>
    /// <remarks/>
    public static BigInteger ModPow(BigInteger bigintValue, BigInteger pow, BigInteger mod) {
      if ((bigintValue) == null) throw new ArgumentNullException("value");
      return bigintValue.ModPow(pow, mod);
    }

    /// <summary> Shifts the bits of a BigInteger instance to the right. </summary>
    /// <param name='bthis'>A BigInteger object.</param>
    /// <param name='n'>The number of bits to shift to the right. If negative,
    /// this treated as shifting left the absolute value of this number of
    /// bits.</param>
    /// <returns></returns>
    /// <remarks>For this operation, the BigInteger is treated as a two's
    /// complement representation. Thus, for negative values, the BigInteger
    /// is sign-extended.</remarks>
    public static BigInteger operator>>(BigInteger bthis, int n)
    {
      if((bthis)==null)throw new ArgumentNullException("bthis");
      return bthis.shiftRight(n);
    }

    /// <summary> Negates a BigInteger object. </summary>
    /// <param name='bigValue'>A BigInteger object.</param>
    /// <returns></returns>
    public static BigInteger operator-(BigInteger bigValue){
      if((bigValue)==null)throw new ArgumentNullException("bigValue");
      return bigValue.negate();
    }
    
    
    /// <summary>Converts the value of a BigInteger object to Int64.</summary>
    /// <returns>A Int64 object with the same value as the BigInteger object.</returns>
    /// <param name='bigValue'>A BigInteger object.</param>
    public static explicit operator long(BigInteger bigValue){
      return bigValue.longValue();
    }

    /// <summary>Converts the value of a BigInteger object to Int32.</summary>
    /// <returns>A Int32 object with the same value as the BigInteger object.</returns>
    /// <param name='bigValue'>A BigInteger object.</param>
    public static explicit operator int(BigInteger bigValue){
      return bigValue.intValue();
    }
    
    /// <summary> Determines whether a BigInteger instance is less than
    /// another BigInteger instance. </summary>
    /// <param name='thisValue'>A BigInteger object.</param>
    /// <param name='otherValue'>A BigInteger object.</param>
    /// <returns>True if &apos;thisValue&apos; is less than &apos;otherValue&apos;;
    /// otherwise, false.</returns>
    /// <remarks/>
    public static bool operator<(BigInteger thisValue,BigInteger otherValue){
      if(thisValue==null)return (otherValue!=null);
      return (thisValue.CompareTo(otherValue)<0);
    }
    /// <summary> Determines whether a BigInteger instance is less than
    /// or equal to another BigInteger instance. </summary>
    /// <param name='thisValue'>A BigInteger object.</param>
    /// <param name='otherValue'>A BigInteger object.</param>
    /// <returns>True if &apos;thisValue&apos; is less than or equal to
    /// &apos;otherValue&apos;; otherwise, false.</returns>
    /// <remarks/>
    public static bool operator<=(BigInteger thisValue,BigInteger otherValue){
      if(thisValue==null)return true;
      return (thisValue.CompareTo(otherValue)<=0);
    }
    /// <summary> Determines whether a BigInteger instance is greater than
    /// another BigInteger instance. </summary>
    /// <param name='thisValue'>A BigInteger object.</param>
    /// <param name='otherValue'>A BigInteger object.</param>
    /// <returns>True if &apos;thisValue&apos; is greater than &apos;otherValue&apos;;
    /// otherwise, false.</returns>
    /// <remarks/>
    public static bool operator>(BigInteger thisValue,BigInteger otherValue){
      if(thisValue==null)return false;
      return (thisValue.CompareTo(otherValue)>0);
    }
    
    /// <summary> Determines whether a BigInteger value is greater than
    /// another BigInteger value. </summary>
    /// <param name='thisValue'>A BigInteger object.</param>
    /// <param name='otherValue'>A BigInteger object.</param>
    /// <returns>True if &apos;thisValue&apos; is greater than or equal
    /// to &apos;otherValue&apos;; otherwise, false.</returns>
    public static bool operator>=(BigInteger thisValue,BigInteger otherValue){
      if(thisValue==null)return (otherValue==null);
      return (thisValue.CompareTo(otherValue)>=0);
    }


    /// <summary> </summary>
    /// <remarks/><returns/>
    public bool IsPowerOfTwo{
      get {
        int bits=BitLength();
        int ret=0;
        for(int i=0;i<bits;i++){
          ret+=(GetUnsignedBit(i)) ? 1 : 0;
          if(ret>=2)return false;
        }
        return (ret==1);
      }
    }
    
    /// <summary> </summary>
    /// <param name='thisValue'>A BigInteger object.</param>
    /// <returns></returns>
    [CLSCompliant(false)]
    public static BigInteger Abs(BigInteger thisValue){
      return thisValue.abs();
    }
    
    /// <summary> Gets the BigInteger object for zero. </summary>
    [CLSCompliant(false)]
    public static BigInteger Zero {
      get { return ZERO; }
    }
    
    /// <summary> Gets the BigInteger object for one. </summary>
    [CLSCompliant(false)]
    public static BigInteger One {
      get { return ONE; }
    }
    
    
    /// <summary> </summary>
    /// <param name='index'>A 32-bit signed integer.</param>
    /// <param name='n'>A 32-bit signed integer.</param>
    /// <returns></returns>
    public long GetBits(int index, int n)
    {
      if(n<0 || n>64)throw new ArgumentOutOfRangeException("n");
      long v = 0;
      //DebugAssert.IsTrue(n <= 8*8,"{0} line {1}: n <= sizeof(v)*8","integer.cpp",2939);
      for (int j=0; j<n; j++)
        v |= (long)(testBit((int)(index+j)) ? 1 : 0) << j;
      return v;
    }
    
    /// <summary> </summary>
    /// <param name='dividend'>A BigInteger object.</param>
    /// <param name='divisor'>A BigInteger object.</param>
    /// <param name='remainder'>A BigInteger object.</param>
    /// <returns></returns>
    public static BigInteger DivRem(BigInteger dividend, BigInteger divisor, out BigInteger remainder)
    {
      BigInteger[] result=dividend.divideAndRemainder(divisor);
      remainder=result[1];
      return result[0];
    }
    
    /// <summary> </summary>
    /// <param name='bigintFirst'>A BigInteger object.</param>
    /// <param name='bigintSecond'>A BigInteger object.</param>
    /// <returns></returns>
    public static BigInteger GreatestCommonDivisor(BigInteger bigintFirst, BigInteger bigintSecond){
      return bigintFirst.gcd(bigintSecond);
    }
    
    /// <summary> </summary>
    /// <returns></returns>
    [CLSCompliant(false)]
    public byte[] ToByteArray(){
      return toByteArray(true);
    }
    
    /// <summary> </summary>
    /// <param name='bigValue'>A BigInteger object.</param>
    /// <param name='power'>A BigInteger object.</param>
    /// <returns></returns>
    [CLSCompliant(false)]
    public static BigInteger Pow(BigInteger bigValue, BigInteger power){
      if(power.Sign<0)
        throw new ArgumentException("power");
      BigInteger val=BigInteger.One;
      while(power.Sign>0){
        BigInteger p=(power>(BigInteger)5000000) ? 
          (BigInteger)5000000 : power;
        val*=bigValue.pow((int)p);
        power-=p;
      }
      return val;
    }
    
    /// <summary> </summary>
    /// <param name='bigValue'>A BigInteger object.</param>
    /// <param name='power'>A 32-bit signed integer.</param>
    /// <returns></returns>
    [CLSCompliant(false)]
    public static BigInteger Pow(BigInteger bigValue, int power){
      if(power<0)
        throw new ArgumentException("power");
      return bigValue.pow(power);
    }
    
    private static void OrWords(short[] r, short[] a, short[] b, int n)
    {
      for (int i=0; i<n; i++)
        r[i] = unchecked((short)(a[i] | b[i]));
    }

    private static void XorWords(short[] r, short[] a, short[] b, int n)
    {
      for (int i=0; i<n; i++)
        r[i] = unchecked((short)(a[i] ^ b[i]));
    }

    private static void NotWords(short[] r, int n)
    {
      for (int i=0; i<n; i++)
        r[i] =unchecked((short)(~r[i]));
    }

    private static void AndWords(short[] r, short[] a, short[] b, int n)
    {
      for (int i=0; i<n; i++)
        r[i] = unchecked((short)(a[i] & b[i]));
    }

    /// <summary> </summary>
    /// <param name='bytes'>A byte[] object.</param>
    public BigInteger(byte[] bytes){
      fromByteArrayInternal(bytes,true);
    }

    /// <summary> </summary>
    /// <param name='other'>A BigInteger object.</param>
    /// <returns></returns>
    /// <remarks/>
    public bool Equals(BigInteger other) {
      if (other == null) return false;
      return this.CompareTo(other) == 0;
    }

    /// <summary> Returns a BigInteger with every bit flipped. </summary>
    /// <param name='a'>A BigInteger object.</param>
    /// <returns></returns>
    public static BigInteger Not(BigInteger a){
      if((a)==null)throw new ArgumentNullException("a");
      BigInteger xa=new BigInteger().Allocate(a.wordCount);
      Array.Copy(a.reg,xa.reg,xa.reg.Length);
      xa.negative=a.negative;
      xa.wordCount=a.wordCount;
      if(xa.Sign<0){ TwosComplement(xa.reg,0,(int)xa.reg.Length); }
      xa.negative=!(xa.Sign<0);
      NotWords((xa.reg),(int)xa.reg.Length);
      if(xa.Sign<0){ TwosComplement(xa.reg,0,(int)xa.reg.Length); }
      xa.wordCount=xa.CalcWordCount();
      if(xa.wordCount==0)xa.negative=false;
      return xa;
    }
    /// <summary> Does an AND operation between two BigInteger values. </summary>
    /// <param name='a'>A BigInteger instance.</param>
    /// <param name='b'>Another BigInteger instance.</param>
    /// <remarks>Each BigInteger instance is treated as a two's complement
    /// representation for the purposes of this operator.</remarks>
    /// <returns></returns>
    public static BigInteger And(BigInteger a, BigInteger b){
      if((a)==null)throw new ArgumentNullException("a");
      if((b)==null)throw new ArgumentNullException("b");
      if(b.IsZero || a.IsZero)return Zero;
      BigInteger xa=new BigInteger().Allocate(a.wordCount);
      Array.Copy(a.reg,xa.reg,xa.reg.Length);
      BigInteger xb=new BigInteger().Allocate(b.wordCount);
      Array.Copy(b.reg,xb.reg,xb.reg.Length);
      xa.negative=a.negative;
      xa.wordCount=a.wordCount;
      xb.negative=b.negative;
      xb.wordCount=b.wordCount;
      xa.reg=CleanGrow(xa.reg,Math.Max(xa.reg.Length,xb.reg.Length));
      xb.reg=CleanGrow(xb.reg,Math.Max(xa.reg.Length,xb.reg.Length));
      if(xa.Sign<0){ TwosComplement(xa.reg,0,(int)xa.reg.Length); }
      if(xb.Sign<0){ TwosComplement(xb.reg,0,(int)xb.reg.Length); }
      xa.negative&=(xb.Sign<0);
      AndWords((xa.reg),(xa.reg),(xb.reg),(int)xa.reg.Length);
      if(xa.Sign<0){ TwosComplement(xa.reg,0,(int)xa.reg.Length); }
      xa.wordCount=xa.CalcWordCount();
      if(xa.wordCount==0)xa.negative=false;
      return xa;
    }

    /// <summary> Does an OR operation between two BigInteger instances.
    /// </summary>
    /// <param name='a'>A BigInteger instance.</param>
    /// <param name='b'>Another BigInteger instance.</param>
    /// <remarks>Each BigInteger instance is treated as a two's complement
    /// representation for the purposes of this operator.</remarks>
    /// <returns></returns>
    public static BigInteger Or(BigInteger a, BigInteger b){
      BigInteger xa=new BigInteger().Allocate(a.wordCount);
      Array.Copy(a.reg,xa.reg,xa.reg.Length);
      BigInteger xb=new BigInteger().Allocate(b.wordCount);
      Array.Copy(b.reg,xb.reg,xb.reg.Length);
      xa.negative=a.negative;
      xa.wordCount=a.wordCount;
      xb.negative=b.negative;
      xb.wordCount=b.wordCount;
      xa.reg=CleanGrow(xa.reg,Math.Max(xa.reg.Length,xb.reg.Length));
      xb.reg=CleanGrow(xb.reg,Math.Max(xa.reg.Length,xb.reg.Length));
      if(xa.Sign<0){ TwosComplement(xa.reg,0,(int)xa.reg.Length); }
      if(xb.Sign<0){ TwosComplement(xb.reg,0,(int)xb.reg.Length); }
      xa.negative|=(xb.Sign<0);
      OrWords((xa.reg),(xa.reg),(xb.reg),(int)xa.reg.Length);
      if(xa.Sign<0){ TwosComplement(xa.reg,0,(int)xa.reg.Length); }
      xa.wordCount=xa.CalcWordCount();
      if(xa.wordCount==0)xa.negative=false;
      return xa;
    }
    
    /// <summary> </summary>
    /// <param name='a'>A BigInteger instance.</param>
    /// <param name='b'>Another BigInteger instance.</param>
    /// <remarks>Each BigInteger instance is treated as a two's complement
    /// representation for the purposes of this operator.</remarks>
    /// <returns></returns>
    public static BigInteger Xor(BigInteger a, BigInteger b){
      BigInteger xa=new BigInteger().Allocate(a.wordCount);
      Array.Copy(a.reg,xa.reg,xa.reg.Length);
      BigInteger xb=new BigInteger().Allocate(b.wordCount);
      Array.Copy(b.reg,xb.reg,xb.reg.Length);
      xa.negative=a.negative;
      xa.wordCount=a.wordCount;
      xb.negative=b.negative;
      xb.wordCount=b.wordCount;
      xa.reg=CleanGrow(xa.reg,Math.Max(xa.reg.Length,xb.reg.Length));
      xb.reg=CleanGrow(xb.reg,Math.Max(xa.reg.Length,xb.reg.Length));
      if(xa.Sign<0){ TwosComplement(xa.reg,0,(int)xa.reg.Length); }
      if(xb.Sign<0){ TwosComplement(xb.reg,0,(int)xb.reg.Length); }
      xa.negative^=(xb.Sign<0);
      XorWords((xa.reg),(xa.reg),(xb.reg),(int)xa.reg.Length);
      if(xa.Sign<0){ TwosComplement(xa.reg,0,(int)xa.reg.Length); }
      xa.wordCount=xa.CalcWordCount();
      if(xa.wordCount==0)xa.negative=false;
      return xa;
    }

  }
}