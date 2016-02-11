using System;
using System.IO;

namespace PeterO.Cbor {
  internal sealed class CBORStream {
    private Stream input;
    private long position;
    private long length;

    public CBORStream(Stream stream, long longLength) {
      this.input = stream;
      this.length = longLength;
    }

    public int Read(byte[] bytes, int offset, int length) {
      int ret = this.input.Read(bytes, offset, length);
      this.position += Math.Max(0, ret);
      return ret;
    }

    public int ReadByte() {
      int ret = this.input.ReadByte();
      if (ret >= 0) {
 ++this.position;
}
      return ret;
    }

    public void AddLength(int length) {
      this.position += length;
    }

    public Stream Input {
      get {
        return this.input;
      }
    }

    public long LengthRemaining {
      get {
      return (
this.length < 0) ? -1 : Math.Max(0,

 this.length - this.position);
      }
    }
  }
}
