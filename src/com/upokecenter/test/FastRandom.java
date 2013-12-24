package com.upokecenter.test;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 11/11/2013
 * Time: 1:13 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */


    /**
     * The system's random number generator will be called many times during
     * testing. Unfortunately it can be very slow. So we use this wrapper
     * class.
     */
  public class FastRandom
  {
    private static final int ReseedCount = 10000;
    
    java.util.Random rand;
    int count;
    
    int m_w = 521288629;
    int m_z = 362436069;
    
    public FastRandom () {
      rand=new java.util.Random();
      count=ReseedCount;
    }
    
    private int NextValueInternal() {
      int w = m_w, z = m_z;
      // Use George Marsaglia's multiply-with-carry
      // algorithm.
      m_z = z = (36969 * (z & 65535) + ((z >> 16)&0xFFFF));
      m_w = w = (18000 * (w & 65535) + ((z >> 16)&0xFFFF));
      return ((z << 16) | (w & 65535))&0x7FFFFFFF;
    }
    
    /**
     * 
     * @param v A 32-bit signed integer.
     * @return A 32-bit signed integer.
     */
public int NextValue(int v) {
      if((v)<0)throw new IllegalArgumentException(
        "v"+" not greater or equal to "+"0"+" ("+
        Integer.toString((int)v)+")");
      if(v<=1)return 0;
      if(count>=ReseedCount){
        // Call the default random number generator
        // every once in a while, to reseed
        count=0;
        if(rand!=null){
          int seed=rand.nextInt(0x10000);
          seed|=(rand.nextInt(0x10000))<<16;
          m_z^=seed;
          return rand.nextInt(v);
        }
      }
      count+=1;
      int maxExclusive=(Integer.MAX_VALUE/v)*v;
      while(true){
        int vi=NextValueInternal();
        if(vi<maxExclusive)
          return vi%v;
      }
    }
  }
