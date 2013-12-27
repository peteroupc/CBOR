/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 11/11/2013
 * Time: 1:13 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
namespace Test
{
    /// <summary> The system's random number generator will be called many
    /// times during testing. Unfortunately it can be very slow. So we use
    /// this wrapper class. </summary>
  public class FastRandom
  {
    private const int ReseedCount = 10000;

    System.Random rand;
    int count;

    int m_w = 521288629;
    int m_z = 362436069;

    public FastRandom()
    {
      rand=new System.Random();
      count=ReseedCount;
    }

    private int NextValueInternal(){
      int w = m_w, z = m_z;
      // Use George Marsaglia's multiply-with-carry
      // algorithm.
      m_z = z = unchecked(36969 * (z & 65535) + ((z >> 16)&0xFFFF));
      m_w = w = unchecked(18000 * (w & 65535) + ((z >> 16)&0xFFFF));
      return ((z << 16) | (w & 65535))&0x7FFFFFFF;
    }

    /// <summary> </summary>
    /// <param name='v'>A 32-bit signed integer.</param>
    /// <returns>A 32-bit signed integer.</returns>
public int NextValue(int v){
      if((v)<0)throw new ArgumentException(
        "v"+" not greater or equal to "+"0"+" ("+
        Convert.ToString((int)v,System.Globalization.CultureInfo.InvariantCulture)+")");
      if(v<=1)return 0;
      if(count>=ReseedCount){
        // Call the default random number generator
        // every once in a while, to reseed
        count=0;
        if(rand!=null){
          int seed=rand.Next(0x10000);
          seed|=(rand.Next(0x10000))<<16;
          m_z^=seed;
          return rand.Next(v);
        }
      }
      count+=1;
      int maxExclusive=(Int32.MaxValue/v)*v;
      while(true){
        int vi=NextValueInternal();
        if(vi<maxExclusive)
          return vi%v;
      }
    }
  }
}