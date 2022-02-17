using System;
using System.IO;

namespace Test {
  /// <summary>Writable stream with a maximum supported byte
  /// size.</summary>
  public sealed class LimitedMemoryStream : Stream {
    private readonly Test.DelayingStream ms;
    private readonly int maxSize;

    public LimitedMemoryStream(int maxSize) {
      if (maxSize < 0) {
        throw new ArgumentException(
          "maxSize (" + maxSize + ") is not greater or equal to 0");
      }
      this.ms = new Test.DelayingStream();
      this.maxSize = maxSize;
    }

    public new void Dispose() {
      this.ms.Dispose();
    }

    public override long Length {
      get {
        return this.ms.Length;
      }
    }

    public override long Seek(long pos, SeekOrigin origin) {
      throw new NotSupportedException();
    }

    public override void SetLength(long len) {
      if (len > this.maxSize) {
        throw new NotSupportedException();
      }
      this.ms.SetLength(len);
    }

    public override long Position {
      get {
        return this.ms.Position;
      }

      set {
        throw new NotSupportedException();
      }
    }

    public override bool CanRead {
      get {
        return false;
      }
    }

    public override bool CanSeek {
      get {
        return false;
      }
    }

    public override bool CanWrite {
      get {
        return this.ms.CanWrite;
      }
    }

    public override int Read(byte[] bytes, int offset, int count) {
      throw new NotSupportedException();
    }

    public override void Flush() {
      this.ms.Flush();
    }

    public override void Write(byte[] bytes, int offset, int count) {
      if (this.ms.Position + count > this.maxSize) {
        throw new NotSupportedException();
      }
      this.ms.Write(bytes, offset, count);
    }

    public override void WriteByte(byte c) {
      if (this.ms.Position >= this.maxSize) {
        throw new NotSupportedException();
      }
      this.ms.WriteByte(c);
    }
  }
}
