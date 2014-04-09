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
  internal sealed class QEncodingStringTransform : ITransform {
    private String input;
    private int inputIndex;
    private byte[] buffer;
    private int bufferIndex;
    private int bufferCount;

    public QEncodingStringTransform(
      String input) {
      this.input = input;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='size'>A 32-bit signed integer.</param>
    private void ResizeBuffer(int size) {
      if (this.buffer == null) {
        this.buffer = new byte[size + 10];
      } else if (size > this.buffer.Length) {
        byte[] newbuffer = new byte[size + 10];
        Array.Copy(this.buffer, newbuffer, this.buffer.Length);
        this.buffer = newbuffer;
      }
      this.bufferCount = size;
      this.bufferIndex = 0;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    public int ReadByte() {
      if (this.bufferIndex < this.bufferCount) {
        int ret = this.buffer[this.bufferIndex];
        ++this.bufferIndex;
        if (this.bufferIndex == this.bufferCount) {
          this.bufferCount = 0;
          this.bufferIndex = 0;
        }
        ret &= 0xff;
        return ret;
      }
      int endIndex = this.input.Length;
      while (true) {
        int c = (this.inputIndex < endIndex) ? this.input[this.inputIndex++] : -1;
        if (c < 0) {
          // End of stream
          return -1;
        } else if (c == 0x0d) {
          // Can't occur in the Q-encoding; replace
          return '?';
        } else if (c == 0x0a) {
          // Can't occur in the Q-encoding; replace
          return '?';
        } else if (c == '=') {
          int b1 = (this.inputIndex < endIndex) ? this.input[this.inputIndex++] : -1;
          c = 0;
          if (b1 >= '0' && b1 <= '9') {
            c <<= 4;
            c |= b1 - '0';
          } else if (b1 >= 'A' && b1 <= 'F') {
            c <<= 4;
            c |= b1 + 10 - 'A';
          } else if (b1 >= 'a' && b1 <= 'f') {
            c <<= 4;
            c |= b1 + 10 - 'a';
          } else {
            --this.inputIndex;
            return '?';
          }
          int b2 = (this.inputIndex < endIndex) ? this.input[this.inputIndex++] : -1;
          if (b2 >= '0' && b2 <= '9') {
            c <<= 4;
            c |= b2 - '0';
          } else if (b2 >= 'A' && b2 <= 'F') {
            c <<= 4;
            c |= b2 + 10 - 'A';
          } else if (b1 >= 'a' && b2 <= 'f') {
            c <<= 4;
            c |= b2 + 10 - 'a';
          } else {
            --this.inputIndex;
            this.ResizeBuffer(1);
            this.buffer[0] = (byte)b1;  // will be 0-9 or a-f or A-F
            return '?';
          }
          return c;
        } else if (c <= 0x20 || c >= 0x7f) {
          // Can't occur in the Q-encoding; replace
          return '?';
        } else if (c == '_') {
          // Underscore, use space
          return ' ';
        } else {
          // printable ASCII, return that byte
          return c;
        }
      }
    }
  }
}
