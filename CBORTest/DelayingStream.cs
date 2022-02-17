using System;
using System.IO;

namespace Test {
  /// <summary>Stream that can return fewer bytes than
  /// requested.</summary>
  public sealed class DelayingStream : Stream {
    private readonly Stream ms;

    public DelayingStream(Stream ms) {
      if (ms == null) {
        throw new ArgumentNullException(nameof(ms));
      }
      this.ms = ms;
    }

    public DelayingStream(byte[] bytes) : this(new MemoryStream(bytes)) {
    }

    public DelayingStream() : this(new MemoryStream()) {
    }

    public DelayingStream(int size) : this(new MemoryStream(size)) {
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
      return this.ms.Seek(pos, origin);
    }

    public override void SetLength(long len) {
      this.ms.SetLength(len);
    }

    public override long Position {
      get {
        return this.ms.Position;
      }

      set {
        this.ms.Position = value;
      }
    }

    public byte[] ToArray() {
      var ms = this.ms as MemoryStream;
      if (ms != null) {
        return ms.ToArray();
      }
      throw new NotSupportedException();
    }

    public override bool CanRead {
      get {
        return this.ms.CanRead;
      }
    }

    public override bool CanSeek {
      get {
        return this.ms.CanSeek;
      }
    }

    public override bool CanWrite {
      get {
        return this.ms.CanWrite;
      }
    }

    public override int Read(byte[] bytes, int offset, int count) {
      if (bytes == null) {
        throw new ArgumentNullException(nameof(bytes));
      }
      if (offset < 0) {
        throw new ArgumentException("\"offset\" (" + offset + ") is not" +
"\u0020greater or equal to 0");
      }
      if (offset > bytes.Length) {
        throw new ArgumentException("\"offset\" (" + offset + ") is not less" +
"\u0020or equal to " + bytes.Length);
      }
      if (count < 0) {
        throw new ArgumentException(" (" + count + ") is not greater or" +
"\u0020equal to 0");
      }
      if (count > bytes.Length) {
        throw new ArgumentException(" (" + count + ") is not less or equal" +
"\u0020to " + bytes.Length);
      }
      if (bytes.Length - offset < count) {
        throw new ArgumentException("\"bytes\" + \"'s length minus \" +" +
"\u0020offset (" + (bytes.Length - offset) + ") is not greater or equal to " +
count);
      }
      if (count == 0) {
        return 0;
      }
      int b = this.ms.ReadByte();
      if (b < 0) {
        return 0;
      }
      bytes[offset] = (byte)b;
      return 1;
    }

    public override void Flush() {
      this.ms.Flush();
    }

    public override void Write(byte[] bytes, int offset, int count) {
      this.ms.Write(bytes, offset, count);
    }

    public override void WriteByte(byte c) {
      this.ms.WriteByte(c);
    }
  }
}
