using System;

using PeterO;

namespace PeterO.Cbor {
  internal class CharacterInputWithCount : ICharacterInput {
    private int offset;
    private readonly ICharacterInput ci;

    public CharacterInputWithCount(ICharacterInput ci) {
      this.ci = ci;
    }

    public int GetOffset() {
      return this.offset;
    }

    private string NewErrorString(string str) {
      return str + " (offset " + this.GetOffset() + ")";
    }

    public void RaiseError(string str) {
      throw new CBORException(this.NewErrorString(str));
    }

    public int ReadChar() {
      var c = -1;
      try {
        c = this.ci.ReadChar();
      } catch (InvalidOperationException ex) {
        if (ex.InnerException == null) {
          throw new CBORException(
this.NewErrorString(ex.Message),
ex);
        } else {
          throw new CBORException(
this.NewErrorString(ex.Message),
ex.InnerException);
        }
      }
      if (c >= 0) {
        ++this.offset;
      }
      return c;
    }

    public int Read(int[] chars, int index, int length) {
      if (chars == null) {
        throw new ArgumentNullException("chars");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + index +
          ") is less than " + 0);
      }
      if (index > chars.Length) {
        throw new ArgumentException("index (" + index +
          ") is more than " + chars.Length);
      }
      if (length < 0) {
        throw new ArgumentException("length (" + length +
          ") is less than " + 0);
      }
      if (length > chars.Length) {
        throw new ArgumentException("length (" + length +
          ") is more than " + chars.Length);
      }
      if (chars.Length - index < length) {
        throw new ArgumentException("chars's length minus " + index + " (" +
          (chars.Length - index) + ") is less than " + length);
      }
      int ret = this.ci.Read(chars, index, length);
      if (ret > 0) {
        this.offset += ret;
      }
      return ret;
    }
  }
}
