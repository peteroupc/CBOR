/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 12/22/2013
 * Time: 7:45 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PeterO
{
    /// <summary>Description of BigNumberFlags.</summary>
  internal static class BigNumberFlags
  {
    internal const int FlagNegative = 1;
    internal const int FlagQuietNaN = 4;
    internal const int FlagSignalingNaN = 8;
    internal const int FlagInfinity = 2;
    internal const int FlagSpecial = FlagQuietNaN | FlagSignalingNaN | FlagInfinity;
    internal const int FlagNaN = FlagQuietNaN | FlagSignalingNaN;

    internal const int FiniteOnly = 0;
    internal const int FiniteAndNonFinite = 1;
    internal const int X3Dot274 = 2;
  }
}
