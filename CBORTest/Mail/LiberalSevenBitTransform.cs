/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/24/2014
 * Time: 10:26 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PeterO.Mail
{
  // A seven-bit transform used for text/plain data
  internal sealed class LiberalSevenBitTransform : ITransform {
    private ITransform stream;

    public LiberalSevenBitTransform(ITransform stream) {
      this.stream = stream;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    public int ReadByte() {
      int ret = this.stream.ReadByte();
      if (ret > 0x80 || ret == 0) {
        return '?';
      }
      return ret;
    }
  }
}
