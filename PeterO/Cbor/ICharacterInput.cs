using System;

namespace PeterO.Cbor {
    /// <summary>An interface for reading Unicode characters from a data
    /// source.</summary>
  internal interface ICharacterInput {
    /// <summary>Reads a Unicode character from a data source.</summary>
    /// <returns>Either a Unicode code point (from 0-0xd7ff or from 0xe000
    /// to 0x10ffff), or the value -1 indicating the end of the
    /// source.</returns>
    int ReadChar();

    /// <summary>Reads a sequence of Unicode code points from a data
    /// source.</summary>
    /// <param name='chars'>Output buffer.</param>
    /// <param name='index'>Index in the output buffer to start writing
    /// to.</param>
    /// <param name='length'>Maximum number of code points to
    /// write.</param>
    /// <returns>The number of Unicode code points read, or 0 if the end of
    /// the source is reached.</returns>
    int Read(int[] chars, int index, int length);
  }
}
