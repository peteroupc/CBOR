/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/24/2014
 * Time: 10:26 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;

namespace PeterO.Mail
{
  internal sealed class Base64Transform : ITransform {
    internal static readonly int[] Alphabet = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
      -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
      -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63,
      52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1,
      -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
      15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
      -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
      41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, -1, -1, -1, -1, -1 };

    private StreamWithUnget input;
    private int lineCharCount;
    private bool lenientLineBreaks;
    private byte[] buffer;
    private int bufferIndex;
    private int bufferCount;

    private const int MaxLineSize = 76;

    public Base64Transform(ITransform input, bool lenientLineBreaks) {
      this.input = new StreamWithUnget(input);
      this.lenientLineBreaks = lenientLineBreaks;
      this.buffer = new byte[4];
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='size'>A 32-bit signed integer.</param>
    private void ResizeBuffer(int size) {
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
      int value = 0;
      int count = 0;
      while (count < 4) {
        int c = this.input.ReadByte();
        if (c < 0) {
          // End of stream
          if (count == 1) {
            // Not supposed to happen
            throw new InvalidDataException("Invalid number of base64 characters");
          } else if (count == 2) {
            this.input.Unget();
            value <<= 12;
            return (byte)((value >> 16) & 0xff);
          } else if (count == 3) {
            this.input.Unget();
            value <<= 18;
            this.ResizeBuffer(1);
            this.buffer[0] = (byte)((value >> 8) & 0xff);
            return (byte)((value >> 16) & 0xff);
          }
          return -1;
        } else if (c == 0x0d) {
          c = this.input.ReadByte();
          if (c == 0x0a) {
            this.lineCharCount = 0;
          } else {
            this.input.Unget();
            if (this.lenientLineBreaks) {
              this.lineCharCount = 0;
            }
          }
        } else if (c == 0x0a) {
          if (this.lenientLineBreaks) {
            this.lineCharCount = 0;
          }
        } else if (c >= 0x80) {
          ++this.lineCharCount;
          if (this.lineCharCount > MaxLineSize) {
            throw new InvalidDataException("Encoded base64 line too long");
          }
        } else {
          ++this.lineCharCount;
          if (this.lineCharCount > MaxLineSize) {
            throw new InvalidDataException("Encoded base64 line too long");
          }
          c = Alphabet[c];
          if (c >= 0) {
            value <<= 6;
            value |= c;
            ++count;
          }
        }
      }
      this.ResizeBuffer(2);
      this.buffer[0] = (byte)((value >> 8) & 0xff);
      this.buffer[1] = (byte)(value & 0xff);
      return (byte)((value >> 16) & 0xff);
    }
  }
}
