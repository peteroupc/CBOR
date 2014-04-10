/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/31/2014
 * Time: 3:18 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Mail
{
  internal sealed class Tokener : ITokener, IComparer<int[]> {
    private List<int[]> tokenStack = new List<int[]>();

    public int GetState() {
      return this.tokenStack.Count;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='state'>A 32-bit signed integer.</param>
    public void RestoreState(int state) {
      #if DEBUG
      if (state > this.tokenStack.Count) {
        throw new ArgumentException("state (" + Convert.ToString((long)state, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)this.tokenStack.Count, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (state < 0) {
        throw new ArgumentException("state (" + Convert.ToString((long)state, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      #endif
      // if (tokenStack.Count != state) {
      // Console.WriteLine("Rolling back from " + tokenStack.Count + " to " + (state));
      // }
      while (state < this.tokenStack.Count) {
        this.tokenStack.RemoveAt(state);
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='token'>A 32-bit signed integer.</param>
    /// <param name='startIndex'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    public void Commit(int token, int startIndex, int endIndex) {
      // Console.WriteLine("Committing token " + token + ", size now " + (tokenStack.Count+1));
      this.tokenStack.Add(new int[] { token, startIndex, endIndex });
    }

    /// <summary>Not documented yet.</summary>
    public void Clear() {
      this.tokenStack.Clear();
    }

    public IList<int[]> GetTokens() {
      this.tokenStack.Sort(this);
      return this.tokenStack;
    }

    /// <summary>Compares one integer array with another.</summary>
    /// <param name='x'>An integer array.</param>
    /// <param name='y'>An integer array. (2).</param>
    /// <returns>Zero if both values are equal; a negative number if <paramref
    /// name='x'/> is less than <paramref name='y'/>, or a positive number
    /// if <paramref name='x'/> is greater than <paramref name='y'/>.</returns>
    public int Compare(int[] x, int[] y) {
      // Sort by their start indexes
      if (x[1] == y[1]) {
        // Sort by their token numbers
        // NOTE: Some parsers rely on the ordering
        // of token numbers, particularly if one token
        // contains another.  In this case, the containing
        // token has a lower number than the contained
        // token.
        return (x[0] == y[0]) ? 0 : ((x[0] < y[0]) ? -1 : 1);
      }
      return (x[1] < y[1]) ? -1 : 1;
    }
  }
}
