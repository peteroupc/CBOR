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
  internal sealed class StreamWithUnget : ITransform {
    private ITransform stream;
    private int lastByte;
    private bool unget;

    public StreamWithUnget(Stream stream) {
      this.lastByte = -1;
      this.stream = new WrappedStream(stream);
    }

    public StreamWithUnget(ITransform stream) {
      this.lastByte = -1;
      this.stream = stream;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    public int ReadByte() {
      if (this.unget) {
        this.unget = false;
      } else {
        this.lastByte = this.stream.ReadByte();
      }
      return this.lastByte;
    }

    /// <summary>Not documented yet.</summary>
    public void Unget() {
      this.unget = true;
    }
  }
}
