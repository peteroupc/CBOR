using System.IO;
using PeterO.Cbor;

namespace Test {
  /// <summary>Helper class to write CBOR objects using a "fluent"
  /// interface. Inspired by suggestions by GitHub user
  /// "sbernard31".</summary>
  public sealed class CBORWriterHelper {
    private readonly Stream outputStream;

    /// <summary>Initializes a new instance of the
    /// <see cref='CBORWriterHelper'/> class.</summary>
    /// <param name='outputStream'/>
    public CBORWriterHelper(Stream outputStream) {
      this.outputStream = outputStream;
    }

    public CBORWriterHelper WriteStartArray(int size) {
      _ = CBORObject.WriteValue(this.outputStream, 4, size);
      return this;
    }

    public CBORWriterHelper WriteStartMap(int size) {
      _ = CBORObject.WriteValue(this.outputStream, 5, size);
      return this;
    }

    public CBORWriterHelper WriteStartArray(long size) {
      _ = CBORObject.WriteValue(this.outputStream, 4, size);
      return this;
    }

    public CBORWriterHelper WriteStartMap(long size) {
      _ = CBORObject.WriteValue(this.outputStream, 5, size);
      return this;
    }

    public CBORWriterHelper WriteStartArray() {
      this.outputStream.WriteByte(0x9f);
      return this;
    }

    public CBORWriterHelper WriteStartMap() {
      this.outputStream.WriteByte(0xbf);
      return this;
    }

    public CBORWriterHelper WriteEndArray() {
      this.outputStream.WriteByte(0xff);
      return this;
    }

    public CBORWriterHelper WriteEndMap() {
      this.outputStream.WriteByte(0xff);
      return this;
    }

    public CBORWriterHelper Write(object key, object value) {
      CBORObject.Write(key, this.outputStream);
      CBORObject.Write(value, this.outputStream);
      return this;
    }

    public CBORWriterHelper Write(object value) {
      CBORObject.Write(value, this.outputStream);
      return this;
    }
  }
}
