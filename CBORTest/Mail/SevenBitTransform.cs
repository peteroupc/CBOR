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
  internal sealed class SevenBitTransform : ITransform {
    private ITransform stream;

    public SevenBitTransform(ITransform stream) {
      this.stream = stream;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    public int ReadByte() {
      int ret = this.stream.ReadByte();
      if (ret > 0x80 || ret == 0) {
        throw new InvalidDataException("Invalid character in message body");
      }
      return ret;
    }
  }
}
