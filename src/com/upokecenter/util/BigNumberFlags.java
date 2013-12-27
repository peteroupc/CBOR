package com.upokecenter.util;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 12/22/2013
 * Time: 7:45 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

    /**
     * Description of BigNumberFlags.
     */
  class BigNumberFlags
  {
    static final int FlagNegative = 1;
    static final int FlagQuietNaN = 4;
    static final int FlagSignalingNaN = 8;
    static final int FlagInfinity = 2;
    static final int FlagSpecial=(FlagQuietNaN|FlagSignalingNaN|FlagInfinity);
    static final int FlagNaN=(FlagQuietNaN|FlagSignalingNaN);

    static final int FiniteOnly = 0;
    static final int FiniteAndNonFinite = 1;
    static final int X3Dot274 = 2;
  }
